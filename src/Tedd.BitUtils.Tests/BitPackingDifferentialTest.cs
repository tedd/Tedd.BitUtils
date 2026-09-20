using System;
using Xunit;

namespace Tedd.BitUtils.Tests;

/// <summary>
/// BitPacking's core reads and writes a field through a 64 bit window where one covers it, and walks the field one
/// byte at a time where none does. These tests pin the two paths to each other: the byte-at-a-time walk is reproduced
/// here verbatim as an independent reference, and every case is checked against it rather than against a hand
/// computed expectation, so a window that is anchored or shifted wrongly cannot pass by agreeing with itself.
/// </summary>
/// <remarks>
/// The buffer lengths are chosen to exercise all three shapes: shorter than a window (no fast path at all), long
/// enough that the field's own byte anchors the window, and a field close enough to the end that the window has to be
/// pulled back to the last eight bytes of the buffer.
/// </remarks>
public class BitPackingDifferentialTest
{
    // Independent reference: the byte-at-a-time algorithm, with no window logic anywhere in it.
    private static ulong ReferenceRead(ReadOnlySpan<byte> source, long bitOffset, int bitCount)
    {
        ulong result = 0;
        for (var i = 0; i < bitCount; i++)
        {
            var bit = (source[(int)((bitOffset + i) >> 3)] >> (7 - (int)((bitOffset + i) & 7))) & 1;
            result = (result << 1) | (uint)bit;
        }
        return result;
    }

    private static void ReferenceWrite(Span<byte> destination, long bitOffset, int bitCount, ulong value)
    {
        for (var i = 0; i < bitCount; i++)
        {
            // Bits go out most significant first, so bit i of the field is bit (bitCount - 1 - i) of the value.
            var state = (value >> (bitCount - 1 - i)) & 1;
            var index = (int)((bitOffset + i) >> 3);
            var mask = (byte)(0x80 >> (int)((bitOffset + i) & 7));
            destination[index] = (byte)(state != 0 ? destination[index] | mask : destination[index] & ~mask);
        }
    }

    public static TheoryData<int> BufferLengths => [1, 2, 3, 7, 8, 9, 16, 33];

    [Theory]
    [MemberData(nameof(BufferLengths))]
    public void ReadBits_MatchesBitByBitReference_AtEveryOffsetAndWidth(int bufferLength)
    {
        var buffer = new byte[bufferLength];
        new Random(1234 + bufferLength).NextBytes(buffer);
        var totalBits = bufferLength * 8;

        for (var offset = 0; offset < totalBits; offset++)
        {
            var widest = Math.Min(64, totalBits - offset);
            for (var width = 0; width <= widest; width++)
            {
                var expected = ReferenceRead(buffer, offset, width);
                var actual = BitPacking.ReadBits(buffer, offset, width);
                Assert.Equal(expected, actual);
            }
        }
    }

    [Theory]
    [MemberData(nameof(BufferLengths))]
    public void WriteBits_MatchesBitByBitReference_AndLeavesNeighboursUntouched(int bufferLength)
    {
        var original = new byte[bufferLength];
        new Random(4321 + bufferLength).NextBytes(original);
        var totalBits = bufferLength * 8;
        var values = new ulong[] { 0, 1, 0xFFFF_FFFF_FFFF_FFFF, 0xA5A5_5A5A_A5A5_5A5A, 0x8000_0000_0000_0000 };

        foreach (var value in values)
        {
            for (var offset = 0; offset < totalBits; offset++)
            {
                var widest = Math.Min(64, totalBits - offset);
                for (var width = 0; width <= widest; width++)
                {
                    var expected = (byte[])original.Clone();
                    ReferenceWrite(expected, offset, width, value);

                    var actual = (byte[])original.Clone();
                    BitPacking.WriteBits(actual, offset, width, value);

                    // Asserting the whole buffer, not just the field, is what catches a window store that clobbers a
                    // neighbouring field outside the one being written.
                    Assert.Equal(expected, actual);
                }
            }
        }
    }

    [Fact]
    public void WriteThenRead_RoundTripsEveryWidthAtEveryOffset_AcrossWindowBoundaries()
    {
        // 24 bytes is three windows wide, so a field can start before the last window, straddle it, or sit entirely
        // in the tail that the window anchor has to be pulled back for.
        var buffer = new byte[24];
        var random = new Random(9876);
        var totalBits = buffer.Length * 8;

        for (var offset = 0; offset < totalBits; offset++)
        {
            var widest = Math.Min(64, totalBits - offset);
            for (var width = 1; width <= widest; width++)
            {
                random.NextBytes(buffer);
                var value = ((ulong)(uint)random.Next() << 32) | (uint)random.Next();
                var truncated = width == 64 ? value : value & ((1UL << width) - 1);

                BitPacking.WriteBits(buffer, offset, width, value);
                Assert.Equal(truncated, BitPacking.ReadBits(buffer, offset, width));
            }
        }
    }

    [Fact]
    public void WriteBits_RandomisedOverlappingFields_MatchReferenceBuffer()
    {
        // Fields written back to back over a shared buffer: a window write that failed to preserve the bits around
        // its field would show up as a mismatch on a later field rather than on the one being written.
        var random = new Random(555);
        for (var trial = 0; trial < 200; trial++)
        {
            var length = random.Next(1, 40);
            var actual = new byte[length];
            random.NextBytes(actual);
            var expected = (byte[])actual.Clone();
            var totalBits = length * 8;

            for (var field = 0; field < 30; field++)
            {
                var offset = random.Next(totalBits);
                var width = random.Next(0, Math.Min(64, totalBits - offset) + 1);
                var value = ((ulong)(uint)random.Next() << 32) | (uint)random.Next();

                BitPacking.WriteBits(actual, offset, width, value);
                ReferenceWrite(expected, offset, width, value);
                Assert.Equal(expected, actual);
            }
        }
    }

    [Fact]
    public void ReadBits_FullWidthFieldAtByteBoundary_ReturnsAllBits()
    {
        // 64 bits at offset 0 is the case where the field exactly fills the window and the mask cannot be computed
        // as (1UL << 64) - 1, because C# masks the shift count and that expression yields 0.
        Span<byte> buffer = [0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF];
        Assert.Equal(ulong.MaxValue, BitPacking.ReadBits(buffer, 0, 64));

        Span<byte> mixed = [0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF];
        Assert.Equal(0x0123_4567_89AB_CDEFUL, BitPacking.ReadBits(mixed, 0, 64));
    }

    [Fact]
    public void WriteBits_FullWidthFieldStartingMidByte_UsesTheFallbackPathCorrectly()
    {
        // 64 bits at a non-zero offset spans nine bytes, so no window covers it and the byte walk has to run.
        var buffer = new byte[16];
        BitPacking.WriteBits(buffer, 3, 64, 0xFFFF_FFFF_FFFF_FFFFUL);
        Assert.Equal(0x1FUL, buffer[0]);                       // top three bits untouched, low five set
        for (var i = 1; i <= 7; i++) Assert.Equal(0xFF, buffer[i]);
        Assert.Equal(0xE0, buffer[8]);                         // top three bits of the field, rest untouched
        Assert.Equal(0xFFFF_FFFF_FFFF_FFFFUL, BitPacking.ReadBits(buffer, 3, 64));
    }

    [Fact]
    public void ZeroWidthField_ReadsZeroAndWritesNothing()
    {
        var buffer = new byte[16];
        new Random(77).NextBytes(buffer);
        var untouched = (byte[])buffer.Clone();

        Assert.Equal(0UL, BitPacking.ReadBits(buffer, 0, 0));
        Assert.Equal(0UL, BitPacking.ReadBits(buffer, 61, 0));
        BitPacking.WriteBits(buffer, 0, 0, ulong.MaxValue);
        BitPacking.WriteBits(buffer, 61, 0, ulong.MaxValue);
        Assert.Equal(untouched, buffer);
    }

    [Fact]
    public void FieldEndingExactlyAtTheBufferEnd_StillRoundTrips()
    {
        // The window anchor has to be pulled back to the last eight bytes here; getting the rebase wrong reads or
        // writes the field eight bytes away from where it belongs.
        for (var length = 8; length <= 20; length++)
        {
            var buffer = new byte[length];
            var totalBits = length * 8;
            for (var width = 1; width <= 64; width++)
            {
                var offset = totalBits - width;
                if (offset < 0) continue;
                Array.Clear(buffer);
                var value = width == 64 ? ulong.MaxValue : (1UL << width) - 1;
                BitPacking.WriteBits(buffer, offset, width, value);
                Assert.Equal(value, BitPacking.ReadBits(buffer, offset, width));
                Assert.Equal(ReferenceRead(buffer, offset, width), BitPacking.ReadBits(buffer, offset, width));
            }
        }
    }

    [Fact]
    public void TooLargeValue_IsTruncatedToTheFieldWidth_NotSpilledIntoNeighbours()
    {
        var buffer = new byte[16];
        Array.Fill(buffer, (byte)0xFF);
        BitPacking.WriteBits(buffer, 8, 4, 0xFFFF_FFFF_FFFF_FFF0UL);   // low four bits are 0
        Assert.Equal(0xFF, buffer[0]);
        Assert.Equal(0x0F, buffer[1]);                                  // high nibble cleared, low nibble untouched
        Assert.Equal(0xFF, buffer[2]);
    }
}
