using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class BitPackingTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 20_000;

        /// <summary>
        /// Deliberately naive reference: walks the field one bit at a time, most significant bit first, so it shares
        /// no arithmetic with the chunked implementation under test.
        /// </summary>
        private static void WriteBitsNaive(Span<byte> destination, long bitOffset, int bitCount, ulong value)
        {
            for (var i = 0; i < bitCount; i++)
            {
                var bit = (value >> (bitCount - 1 - i)) & 1;
                var position = bitOffset + i;
                var mask = 0x80 >> (int)(position & 7);
                var index = (int)(position >> 3);
                destination[index] = (byte)(bit != 0 ? destination[index] | mask : destination[index] & ~mask);
            }
        }

        private static ulong ReadBitsNaive(ReadOnlySpan<byte> source, long bitOffset, int bitCount)
        {
            ulong result = 0;
            for (var i = 0; i < bitCount; i++)
            {
                var position = bitOffset + i;
                var bit = (uint)(source[(int)(position >> 3)] >> (7 - (int)(position & 7))) & 1u;
                result = (result << 1) | bit;
            }
            return result;
        }

        [Fact]
        public void Layout_IsMostSignificantBitFirst()
        {
            Span<byte> buffer = stackalloc byte[2];
            BitPacking.WriteBits(buffer, 0, 3, 0b101);
            Assert.Equal(0b101_00000, buffer[0]);
            BitPacking.WriteBits(buffer, 3, 5, 0b11011);
            Assert.Equal(0b101_11011, buffer[0]);
            Assert.Equal(0, buffer[1]);
            Assert.Equal(0b101UL, BitPacking.ReadBits(buffer, 0, 3));
            Assert.Equal(0b11011UL, BitPacking.ReadBits(buffer, 3, 5));
        }

        [Fact]
        public void Field_StraddlesByteBoundary()
        {
            Span<byte> buffer = stackalloc byte[3];
            BitPacking.WriteBits(buffer, 4, 12, 0xABC);
            Assert.Equal(0x0A, buffer[0]);
            Assert.Equal(0xBC, buffer[1]);
            Assert.Equal(0x00, buffer[2]);
            Assert.Equal(0xABCUL, BitPacking.ReadBits(buffer, 4, 12));
        }

        [Fact]
        public void Write_LeavesNeighbouringBitsAlone()
        {
            Span<byte> buffer = [0xFF, 0xFF, 0xFF];
            BitPacking.WriteBits(buffer, 6, 4, 0b0000);
            // Bits 6..9 cleared, everything else still set.
            Assert.Equal(0b1111_1100, buffer[0]);
            Assert.Equal(0b0011_1111, buffer[1]);
            Assert.Equal(0xFF, buffer[2]);
        }

        [Fact]
        public void Write_IgnoresBitsAboveTheFieldWidth()
        {
            Span<byte> buffer = stackalloc byte[2];
            BitPacking.WriteBits(buffer, 0, 4, ulong.MaxValue);
            Assert.Equal(0b1111_0000, buffer[0]);
            Assert.Equal(0, buffer[1]);
        }

        [Fact]
        public void FullWidth64_RoundTrips()
        {
            Span<byte> buffer = stackalloc byte[9];
            const ulong value = 0x0123456789ABCDEFUL;
            BitPacking.WriteBits(buffer, 0, 64, value);
            Assert.Equal(value, BitPacking.ReadBits(buffer, 0, 64));

            buffer.Clear();
            BitPacking.WriteBits(buffer, 5, 64, value);   // straddling, nine chunks
            Assert.Equal(value, BitPacking.ReadBits(buffer, 5, 64));
        }

        [Fact]
        public void ZeroWidth_IsANoOp()
        {
            Span<byte> buffer = [0xAA, 0xBB];
            BitPacking.WriteBits(buffer, 3, 0, ulong.MaxValue);
            Assert.Equal(0xAA, buffer[0]);
            Assert.Equal(0xBB, buffer[1]);
            Assert.Equal(0UL, BitPacking.ReadBits(buffer, 3, 0));
        }

        [Fact]
        public void MatchesNaiveReference_AtEveryOffsetAndWidth()
        {
            const int bufferBytes = 12;
            Span<byte> actual = stackalloc byte[bufferBytes];
            Span<byte> expected = stackalloc byte[bufferBytes];
            for (var offset = 0; offset <= 16; offset++)
            {
                for (var width = 0; width <= 64; width++)
                {
                    if (offset + width > bufferBytes * 8) continue;
                    var value = unchecked((ulong)_rnd.NextInt64());
                    actual.Fill(0x5A);
                    expected.Fill(0x5A);
                    BitPacking.WriteBits(actual, offset, width, value);
                    WriteBitsNaive(expected, offset, width, value);
                    Assert.True(actual.SequenceEqual(expected), $"Mismatch at offset {offset}, width {width}.");
                    Assert.Equal(ReadBitsNaive(actual, offset, width), BitPacking.ReadBits(actual, offset, width));
                }
            }
        }

        [Fact]
        public void MatchesNaiveReference_OnRandomFields()
        {
            const int bufferBytes = 16;
            Span<byte> actual = stackalloc byte[bufferBytes];
            Span<byte> expected = stackalloc byte[bufferBytes];
            for (var i = 0; i < Iterations; i++)
            {
                var width = _rnd.Next(0, 65);
                var offset = _rnd.Next(0, bufferBytes * 8 - width + 1);
                var value = unchecked((ulong)_rnd.NextInt64());
                var fill = (byte)_rnd.Next(256);
                actual.Fill(fill);
                expected.Fill(fill);
                BitPacking.WriteBits(actual, offset, width, value);
                WriteBitsNaive(expected, offset, width, value);
                Assert.True(actual.SequenceEqual(expected));
                var masked = width == 64 ? value : value & ((1UL << width) - 1);
                Assert.Equal(masked, BitPacking.ReadBits(actual, offset, width));
            }
        }

        [Fact]
        public void SingleBit_ReadAndWrite()
        {
            Span<byte> buffer = stackalloc byte[2];
            for (var bit = 0; bit < 16; bit++)
            {
                buffer.Clear();
                BitPacking.WriteBit(buffer, bit, true);
                Assert.Equal(0x80 >> (bit & 7), buffer[bit >> 3]);
                for (var probe = 0; probe < 16; probe++)
                    Assert.Equal(probe == bit, BitPacking.ReadBit(buffer, probe));
                BitPacking.WriteBit(buffer, bit, false);
                Assert.False(BitPacking.ReadBit(buffer, bit));
            }
        }

        [Fact]
        public void ByteCount_RoundsUp()
        {
            Assert.Equal(0, BitPacking.ByteCount(0));
            Assert.Equal(1, BitPacking.ByteCount(1));
            Assert.Equal(1, BitPacking.ByteCount(8));
            Assert.Equal(2, BitPacking.ByteCount(9));
            Assert.Throws<ArgumentOutOfRangeException>(() => BitPacking.ByteCount(-1));
        }

        [Fact]
        public void OutOfRangeArguments_Throw()
        {
            var buffer = new byte[2];
            Assert.Throws<ArgumentOutOfRangeException>(() => BitPacking.WriteBits(buffer, -1, 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => BitPacking.WriteBits(buffer, 0, 65, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => BitPacking.WriteBits(buffer, 0, -1, 0));
            Assert.Throws<ArgumentException>(() => BitPacking.WriteBits(buffer, 12, 8, 0));
            Assert.Throws<ArgumentException>(() => BitPacking.ReadBits(buffer, 12, 8));
            Assert.Throws<ArgumentOutOfRangeException>(() => BitPacking.ReadBit(buffer, 16));
            Assert.Throws<ArgumentOutOfRangeException>(() => BitPacking.WriteBit(buffer, 16, true));
        }

        [Fact]
        public void TryVariants_ReportInsteadOfThrowing()
        {
            var buffer = new byte[2];
            Assert.False(BitPacking.TryWriteBits(buffer, 12, 8, 0xFF));
            Assert.Equal(0, buffer[0]);
            Assert.Equal(0, buffer[1]);
            Assert.False(BitPacking.TryReadBits(buffer, 12, 8, out var value));
            Assert.Equal(0UL, value);
            Assert.True(BitPacking.TryWriteBits(buffer, 8, 8, 0xFF));
            Assert.True(BitPacking.TryReadBits(buffer, 8, 8, out value));
            Assert.Equal(0xFFUL, value);
        }
    }
}
