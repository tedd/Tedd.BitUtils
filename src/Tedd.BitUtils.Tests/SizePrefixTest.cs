using System;
using System.Buffers.Binary;
using System.IO;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class SizePrefixTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 20_000;

        private static byte[] Encode(uint value)
        {
            var buffer = new byte[SizePrefix.MaxLength];
            var length = SizePrefix.Write(buffer, value);
            return buffer[..length];
        }

        [Theory]
        [InlineData(0U, new byte[] { 0x00 })]
        [InlineData(1U, new byte[] { 0x01 })]
        [InlineData(0x3FU, new byte[] { 0x3F })]
        [InlineData(0x40U, new byte[] { 0x40, 0x40 })]
        [InlineData(0x3FFFU, new byte[] { 0x7F, 0xFF })]
        [InlineData(0x4000U, new byte[] { 0x80, 0x40, 0x00 })]
        [InlineData(0x3FFFFFU, new byte[] { 0xBF, 0xFF, 0xFF })]
        [InlineData(0x400000U, new byte[] { 0xC0, 0x40, 0x00, 0x00 })]
        [InlineData(SizePrefix.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]
        public void MatchesKnownVectors(uint value, byte[] expected)
        {
            Assert.Equal(expected, Encode(value));
            Assert.Equal(expected.Length, SizePrefix.Measure(value));
            Assert.Equal(expected.Length, SizePrefix.GetLength(expected[0]));
            Assert.Equal(value, SizePrefix.Read(expected, out var bytesRead));
            Assert.Equal(expected.Length, bytesRead);
        }

        [Fact]
        public void RoundTripsEveryLengthBoundary()
        {
            foreach (var value in new uint[] { 0, 0x3F, 0x40, 0x3FFF, 0x4000, 0x3FFFFF, 0x400000, SizePrefix.MaxValue })
            {
                var encoded = Encode(value);
                Assert.Equal(value, SizePrefix.Read(encoded, out var read));
                Assert.Equal(encoded.Length, read);
            }
        }

        [Fact]
        public void RoundTripsRandomValues()
        {
            var buffer = new byte[SizePrefix.MaxLength];
            for (var i = 0; i < Iterations; i++)
            {
                var value = (uint)_rnd.NextInt64(0, SizePrefix.MaxValue + 1L);
                var written = SizePrefix.Write(buffer, value);
                Assert.Equal(SizePrefix.Measure(value), written);
                Assert.Equal(value, SizePrefix.Read(buffer, out var read));
                Assert.Equal(written, read);
            }
        }

        [Fact]
        public void IsByteForByteWhatSpanUtilsWrites()
        {
            // Verbatim copy of the Tedd.SpanUtils WriteSize/ReadSize pair, so a divergence shows up as a wire format
            // break rather than as a surprise once SpanUtils is switched over to this library.
            static int SpanUtilsWriteSize(Span<byte> span, uint value)
            {
                var length = value <= 0x3F ? 1 : value <= 0x3FFF ? 2 : value <= 0x3FFFFF ? 3 : 4;
                switch (length)
                {
                    case 1: span[0] = (byte)value; break;
                    case 2: BinaryPrimitives.WriteUInt16BigEndian(span, (ushort)(value | 0x4000U)); break;
                    case 3:
                        span[0] = (byte)((value | 0x800000U) >> 16);
                        span[1] = (byte)(value >> 8);
                        span[2] = (byte)value;
                        break;
                    default: BinaryPrimitives.WriteUInt32BigEndian(span, value | 0xC0000000U); break;
                }
                return length;
            }

            Span<byte> mine = stackalloc byte[SizePrefix.MaxLength];
            Span<byte> theirs = stackalloc byte[SizePrefix.MaxLength];
            for (var i = 0; i < Iterations; i++)
            {
                var value = (uint)_rnd.NextInt64(0, SizePrefix.MaxValue + 1L);
                mine.Clear(); theirs.Clear();
                var mineLength = SizePrefix.Write(mine, value);
                var theirsLength = SpanUtilsWriteSize(theirs, value);
                Assert.Equal(theirsLength, mineLength);
                Assert.True(mine.SequenceEqual(theirs), $"Wire format diverged for {value}.");
            }
        }

        [Fact]
        public void EncodedValuesSortLikeTheNumbers()
        {
            var previousBytes = Encode(0);
            for (uint value = 1; value < 20000; value++)
            {
                var bytes = Encode(value);
                if (bytes.Length == previousBytes.Length)
                    Assert.True(bytes.AsSpan().SequenceCompareTo(previousBytes) > 0, $"{value} should sort above {value - 1}.");
                previousBytes = bytes;
            }
        }

        [Fact]
        public void RejectsOutOfRangeValues()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => SizePrefix.Measure(SizePrefix.MaxValue + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => SizePrefix.Write(new byte[4], uint.MaxValue));
            Assert.False(SizePrefix.TryWrite(new byte[4], uint.MaxValue, out var written));
            Assert.Equal(0, written);
        }

        [Fact]
        public void RejectsTruncatedInput()
        {
            var encoded = Encode(0x4000);       // three bytes
            Assert.Throws<EndOfStreamException>(() => SizePrefix.Read(encoded[..2], out _));
            Assert.Throws<EndOfStreamException>(() => SizePrefix.Read([], out _));
            Assert.False(SizePrefix.TryRead(encoded[..2], out _, out var read));
            Assert.Equal(0, read);
        }

        [Fact]
        public void ShortDestination_ThrowsOrReports()
        {
            Assert.Throws<ArgumentException>(() => SizePrefix.Write(new byte[1], 0x4000));
            Assert.False(SizePrefix.TryWrite(new byte[1], 0x4000, out var written));
            Assert.Equal(0, written);
        }

        [Fact]
        public void HoldsMoreThanLeb128InFourBytes()
        {
            // The reason this format exists alongside VarInt: 30 payload bits in four bytes rather than 28.
            Assert.Equal(4, SizePrefix.Measure(SizePrefix.MaxValue));
            Assert.Equal(5, VarInt.MeasureUnsigned(SizePrefix.MaxValue));
        }
    }
}
