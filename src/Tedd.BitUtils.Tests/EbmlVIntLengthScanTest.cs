using System;
using Xunit;

namespace Tedd.BitUtils.Tests;

/// <summary>
/// EbmlVInt derives the encoding length from the position of the first set bit of the first byte. These tests pin
/// that derivation against the marker-shifting loop it replaced, over every possible first byte, so an off-by-one in
/// the leading-zero arithmetic cannot slip through on the byte values that happen to appear in the other tests.
/// </summary>
public class EbmlVIntLengthScanTest
{
    // The original loop, kept as the independent reference.
    private static int ReferenceLength(byte first)
    {
        var marker = 0x80;
        var length = 1;
        while ((first & marker) == 0) { marker >>= 1; length++; }
        return length;
    }

    [Fact]
    public void EveryNonZeroFirstByte_DecodesToTheSameLengthAsTheMarkerLoop()
    {
        // A zero first byte would mean more than eight length bits and is rejected before the scan runs, so 1..255
        // is the whole legal domain.
        for (var first = 1; first <= 255; first++)
        {
            var expected = ReferenceLength((byte)first);

            // Feed it a buffer long enough for any length, so TryRead reaches the length scan rather than failing
            // the truncation check, and read the length back off the decoded value.
            var buffer = new byte[8];
            buffer[0] = (byte)first;
            Assert.True(EbmlVInt.TryRead(buffer, out var value));
            Assert.Equal(expected, value.Length);
        }
    }

    [Theory]
    [InlineData(0x80, 1)]
    [InlineData(0xFF, 1)]
    [InlineData(0x40, 2)]
    [InlineData(0x7F, 2)]
    [InlineData(0x20, 3)]
    [InlineData(0x10, 4)]
    [InlineData(0x08, 5)]
    [InlineData(0x04, 6)]
    [InlineData(0x02, 7)]
    [InlineData(0x01, 8)]
    public void MarkerBitPosition_SelectsTheEncodingLength(byte first, int expectedLength)
    {
        var buffer = new byte[8];
        buffer[0] = first;
        Assert.True(EbmlVInt.TryRead(buffer, out var value));
        Assert.Equal(expectedLength, value.Length);
    }

    [Fact]
    public void ZeroFirstByte_IsStillRejected()
    {
        // The leading-zero count of zero is 32, which would decode as a length of 9 - outside the format. The guard
        // in front of the scan is what keeps that unreachable.
        Assert.False(EbmlVInt.TryRead(new byte[8], out _));
    }

    [Fact]
    public void LengthBeyondMaxLength_IsStillRejected()
    {
        var buffer = new byte[8];
        buffer[0] = 0x01;   // an 8 byte encoding
        Assert.False(EbmlVInt.TryRead(buffer, out _, maxLength: 7));
        Assert.True(EbmlVInt.TryRead(buffer, out _, maxLength: 8));
    }

    [Fact]
    public void TruncatedEncoding_IsStillRejected()
    {
        // Claims four bytes but only three are present.
        Assert.False(EbmlVInt.TryRead(new byte[] { 0x10, 0x00, 0x00 }, out _));
    }

    [Fact]
    public void EveryValue_RoundTripsThroughWriteAndRead()
    {
        var random = new Random(31337);
        Span<byte> buffer = stackalloc byte[EbmlVInt.MaxLength];

        // One value at each size boundary, plus randomised values across the whole range.
        for (var size = 1; size <= 8; size++)
        {
            var highest = (1UL << (7 * size)) - 2;      // all-ones is the reserved unknown-size marker
            var lowest = size == 1 ? 0UL : (1UL << (7 * (size - 1))) - 1;
            foreach (var value in new[] { lowest, highest })
            {
                var written = EbmlVInt.Write(buffer, value);
                var read = EbmlVInt.Read(buffer);
                Assert.Equal(value, read.Value);
                Assert.Equal(written, read.Length);
            }
        }

        for (var i = 0; i < 20_000; i++)
        {
            var value = (ulong)random.NextInt64(0, (long)EbmlVInt.MaxValue);
            var written = EbmlVInt.Write(buffer, value);
            var read = EbmlVInt.Read(buffer);
            Assert.Equal(value, read.Value);
            Assert.Equal(written, read.Length);
        }
    }
}
