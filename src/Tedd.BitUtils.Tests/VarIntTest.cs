using System;
using System.IO;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class VarIntTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 20_000;

        private static byte[] Encode(Func<byte[], int> write)
        {
            var buffer = new byte[VarInt.MaxLength];
            var length = write(buffer);
            return buffer[..length];
        }

        #region Unsigned (ULEB128)
        [Theory]
        // Byte for byte the ULEB128 / Protocol Buffers varint vectors.
        [InlineData(0UL, new byte[] { 0x00 })]
        [InlineData(1UL, new byte[] { 0x01 })]
        [InlineData(127UL, new byte[] { 0x7F })]
        [InlineData(128UL, new byte[] { 0x80, 0x01 })]
        [InlineData(300UL, new byte[] { 0xAC, 0x02 })]
        [InlineData(16383UL, new byte[] { 0xFF, 0x7F })]
        [InlineData(16384UL, new byte[] { 0x80, 0x80, 0x01 })]
        [InlineData(624485UL, new byte[] { 0xE5, 0x8E, 0x26 })]        // the canonical DWARF ULEB128 example
        [InlineData(ulong.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
        public void Unsigned_MatchesKnownVectors(ulong value, byte[] expected)
        {
            var encoded = Encode(b => VarInt.WriteUnsigned(b, value));
            Assert.Equal(expected, encoded);
            Assert.Equal(expected.Length, VarInt.MeasureUnsigned(value));
            Assert.Equal(value, VarInt.ReadUnsigned(encoded, out var bytesRead));
            Assert.Equal(expected.Length, bytesRead);
        }

        [Fact]
        public void Unsigned_RoundTripsRandomValues()
        {
            var buffer = new byte[VarInt.MaxLength];
            for (var i = 0; i < Iterations; i++)
            {
                var value = unchecked((ulong)_rnd.NextInt64()) >> _rnd.Next(0, 64);
                var written = VarInt.WriteUnsigned(buffer, value);
                Assert.Equal(VarInt.MeasureUnsigned(value), written);
                Assert.Equal(value, VarInt.ReadUnsigned(buffer, out var read));
                Assert.Equal(written, read);
            }
        }

        [Fact]
        public void Unsigned_RejectsValueWiderThanRequested()
        {
            var encoded = Encode(b => VarInt.WriteUnsigned(b, 256));
            Assert.Throws<OverflowException>(() => VarInt.ReadUnsigned(encoded, out _, bits: 8));
            Assert.Equal(256UL, VarInt.ReadUnsigned(encoded, out _, bits: 16));
            Assert.False(VarInt.TryReadUnsigned(encoded, out _, out _, out var overflow, bits: 8));
            Assert.True(overflow);
        }

        [Fact]
        public void Unsigned_RejectsTruncatedInput()
        {
            var encoded = Encode(b => VarInt.WriteUnsigned(b, 300));
            Assert.Throws<EndOfStreamException>(() => VarInt.ReadUnsigned(encoded[..1], out _));
            Assert.False(VarInt.TryReadUnsigned(encoded[..1], out _, out _, out var overflow));
            Assert.False(overflow);      // truncated, not out of range
            Assert.False(VarInt.TryReadUnsigned([], out _, out _));
        }
        #endregion

        #region Signed (SLEB128)
        [Theory]
        // Standard SLEB128 vectors (DWARF / WebAssembly).
        [InlineData(0L, new byte[] { 0x00 })]
        [InlineData(1L, new byte[] { 0x01 })]
        [InlineData(-1L, new byte[] { 0x7F })]
        [InlineData(63L, new byte[] { 0x3F })]
        [InlineData(64L, new byte[] { 0xC0, 0x00 })]
        [InlineData(-64L, new byte[] { 0x40 })]
        [InlineData(-65L, new byte[] { 0xBF, 0x7F })]
        [InlineData(2L, new byte[] { 0x02 })]
        [InlineData(-2L, new byte[] { 0x7E })]
        [InlineData(127L, new byte[] { 0xFF, 0x00 })]
        [InlineData(-123456L, new byte[] { 0xC0, 0xBB, 0x78 })]        // the canonical DWARF SLEB128 example
        [InlineData(long.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 })]
        [InlineData(long.MinValue, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x7F })]
        public void Signed_MatchesKnownVectors(long value, byte[] expected)
        {
            var encoded = Encode(b => VarInt.WriteSigned(b, value));
            Assert.Equal(expected, encoded);
            Assert.Equal(expected.Length, VarInt.MeasureSigned(value));
            Assert.Equal(value, VarInt.ReadSigned(encoded, out var bytesRead));
            Assert.Equal(expected.Length, bytesRead);
        }

        [Fact]
        public void Signed_RoundTripsRandomValues()
        {
            var buffer = new byte[VarInt.MaxLength];
            for (var i = 0; i < Iterations; i++)
            {
                var value = _rnd.NextInt64() >> _rnd.Next(0, 64);
                var written = VarInt.WriteSigned(buffer, value);
                Assert.Equal(VarInt.MeasureSigned(value), written);
                Assert.Equal(value, VarInt.ReadSigned(buffer, out var read));
                Assert.Equal(written, read);
            }
        }

        [Fact]
        public void Signed_RoundTripsEveryNarrowValue()
        {
            var buffer = new byte[VarInt.MaxLength];
            for (var value = -70000; value <= 70000; value++)
            {
                var written = VarInt.WriteSigned(buffer, value);
                Assert.Equal(VarInt.MeasureSigned(value), written);
                Assert.Equal(value, VarInt.ReadSigned(buffer, out var read));
                Assert.Equal(written, read);
            }
        }

        [Fact]
        public void Signed_HandlesTheMostNegativeValue()
        {
            // Unlike the sign-magnitude format, SLEB128 needs no sentinel for this.
            var encoded = Encode(b => VarInt.WriteSigned(b, long.MinValue));
            Assert.Equal(long.MinValue, VarInt.ReadSigned(encoded, out _));
            Assert.Equal(short.MinValue, (short)VarInt.ReadSigned(Encode(b => VarInt.WriteSigned(b, short.MinValue)), out _, bits: 16));
        }

        [Fact]
        public void Signed_RejectsValueWiderThanRequested()
        {
            var encoded = Encode(b => VarInt.WriteSigned(b, 200));
            Assert.Throws<OverflowException>(() => VarInt.ReadSigned(encoded, out _, bits: 8));
            Assert.Equal(200L, VarInt.ReadSigned(encoded, out _, bits: 16));

            var negative = Encode(b => VarInt.WriteSigned(b, -200));
            Assert.Throws<OverflowException>(() => VarInt.ReadSigned(negative, out _, bits: 8));
            Assert.Equal(-200L, VarInt.ReadSigned(negative, out _, bits: 16));
        }

        [Fact]
        public void Signed_RejectsOverlongEncoding()
        {
            // Eleven continuation bytes: more groups than 64 bits can hold.
            var overlong = new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x00 };
            Assert.False(VarInt.TryReadSigned(overlong, out _, out _, out var overflow));
            Assert.True(overflow);

            // Final byte carrying bits that do not fit in 64.
            var malformed = new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x40 };
            Assert.False(VarInt.TryReadSigned(malformed, out _, out _, out overflow));
            Assert.True(overflow);
        }
        #endregion

        #region ZigZag
        [Theory]
        // Protocol Buffers sint32/sint64 vectors.
        [InlineData(0L, new byte[] { 0x00 })]
        [InlineData(-1L, new byte[] { 0x01 })]
        [InlineData(1L, new byte[] { 0x02 })]
        [InlineData(-2L, new byte[] { 0x03 })]
        [InlineData(2147483647L, new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0x0F })]
        [InlineData(-2147483648L, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F })]
        public void ZigZag_MatchesKnownVectors(long value, byte[] expected)
        {
            var encoded = Encode(b => VarInt.WriteZigZag(b, value));
            Assert.Equal(expected, encoded);
            Assert.Equal(expected.Length, VarInt.MeasureZigZag(value));
            Assert.Equal(value, VarInt.ReadZigZag(encoded, out var bytesRead));
            Assert.Equal(expected.Length, bytesRead);
        }

        [Fact]
        public void ZigZag_RoundTripsRandomValues()
        {
            var buffer = new byte[VarInt.MaxLength];
            for (var i = 0; i < Iterations; i++)
            {
                var value = _rnd.NextInt64() >> _rnd.Next(0, 64);
                var written = VarInt.WriteZigZag(buffer, value);
                Assert.Equal(VarInt.MeasureZigZag(value), written);
                Assert.Equal(value, VarInt.ReadZigZag(buffer, out var read));
                Assert.Equal(written, read);
            }
        }

        [Fact]
        public void ZigZag_AndSigned_AlwaysUseTheSameNumberOfBytes()
        {
            // Folding costs exactly the one bit that sign extension would have, so neither format is more compact
            // than the other - the choice between them is about which one the other side speaks.
            foreach (var value in new[] { 0L, 1L, -1L, 63L, -64L, -65L, int.MinValue - 1L, long.MinValue, long.MaxValue })
                Assert.Equal(VarInt.MeasureSigned(value), VarInt.MeasureZigZag(value));
            for (var i = 0; i < Iterations; i++)
            {
                var value = _rnd.NextInt64() >> _rnd.Next(0, 64);
                Assert.Equal(VarInt.MeasureSigned(value), VarInt.MeasureZigZag(value));
            }
        }

        [Fact]
        public void ZigZag_HandlesTheMostNegativeValue()
        {
            var encoded = Encode(b => VarInt.WriteZigZag(b, long.MinValue));
            Assert.Equal(long.MinValue, VarInt.ReadZigZag(encoded, out _));
        }
        #endregion

        #region Sign-magnitude (Tedd.SpanUtils legacy format)
        // Verbatim copies of the Tedd.SpanUtils implementations, so a divergence in the new code shows up here as a
        // wire format break rather than as a surprise once SpanUtils is switched over to this library.
        private static byte SpanUtilsMeasureVLQ(long value)
        {
            if (value == long.MinValue) return 1;
            var magnitude = (ulong)(value < 0 ? -value : value);
            var length = 1;
            for (magnitude >>= 6; magnitude != 0; magnitude >>= 7) length++;
            return (byte)length;
        }

        private static int SpanUtilsWriteSignedVLQ(Span<byte> span, long value, long minimum)
        {
            if (value == minimum) { span[0] = 0x40; return 1; }
            var length = SpanUtilsMeasureVLQ(value);
            var negative = value < 0;
            var magnitude = (ulong)(negative ? -value : value);
            span[0] = (byte)((magnitude & 0x3F) | (negative ? 0x40UL : 0));
            magnitude >>= 6;
            var index = 0;
            while (magnitude != 0)
            {
                span[index++] |= 0x80;
                span[index] = (byte)(magnitude & 0x7F);
                magnitude >>= 7;
            }
            return length;
        }

        private static byte SpanUtilsMeasureVLQ(ulong value)
        {
            var length = 1;
            while (value >= 0x80) { value >>= 7; length++; }
            return (byte)length;
        }

        private static int SpanUtilsWriteUnsignedVLQ(Span<byte> span, ulong value)
        {
            var length = SpanUtilsMeasureVLQ(value);
            var index = 0;
            while (value >= 0x80)
            {
                span[index++] = (byte)(value | 0x80);
                value >>= 7;
            }
            span[index] = (byte)value;
            return length;
        }

        [Theory]
        [InlineData(0L, new byte[] { 0x00 })]
        [InlineData(1L, new byte[] { 0x01 })]
        [InlineData(-1L, new byte[] { 0x41 })]
        [InlineData(63L, new byte[] { 0x3F })]              // six magnitude bits fit in the first byte
        [InlineData(-63L, new byte[] { 0x7F })]             // same, with the 0x40 sign bit set
        [InlineData(64L, new byte[] { 0x80, 0x01 })]        // one bit over, so a continuation byte appears
        [InlineData(-64L, new byte[] { 0xC0, 0x01 })]
        [InlineData(long.MinValue, new byte[] { 0x40 })]
        public void SignMagnitude_MatchesKnownVectors(long value, byte[] expected)
        {
            var encoded = Encode(b => VarInt.WriteSignMagnitude(b, value));
            Assert.Equal(expected, encoded);
            Assert.Equal(expected.Length, VarInt.MeasureSignMagnitude(value));
            Assert.Equal(value, VarInt.ReadSignMagnitude(encoded, out var bytesRead));
            Assert.Equal(expected.Length, bytesRead);
        }

        [Fact]
        public void SignMagnitude_IsByteForByteWhatSpanUtilsWrites()
        {
            Span<byte> mine = stackalloc byte[VarInt.MaxLength];
            Span<byte> theirs = stackalloc byte[VarInt.MaxLength];
            for (var i = 0; i < Iterations; i++)
            {
                var value = _rnd.NextInt64() >> _rnd.Next(0, 64);
                mine.Clear(); theirs.Clear();
                var mineLength = VarInt.WriteSignMagnitude(mine, value);
                var theirsLength = SpanUtilsWriteSignedVLQ(theirs, value, long.MinValue);
                Assert.Equal(theirsLength, mineLength);
                Assert.Equal(SpanUtilsMeasureVLQ(value), VarInt.MeasureSignMagnitude(value));
                Assert.True(mine.SequenceEqual(theirs), $"Wire format diverged for {value}.");
                Assert.Equal(value, VarInt.ReadSignMagnitude(mine, out var read));
                Assert.Equal(mineLength, read);
            }
        }

        [Fact]
        public void Unsigned_IsByteForByteWhatSpanUtilsWrites()
        {
            Span<byte> mine = stackalloc byte[VarInt.MaxLength];
            Span<byte> theirs = stackalloc byte[VarInt.MaxLength];
            for (var i = 0; i < Iterations; i++)
            {
                var value = unchecked((ulong)_rnd.NextInt64()) >> _rnd.Next(0, 64);
                mine.Clear(); theirs.Clear();
                var mineLength = VarInt.WriteUnsigned(mine, value);
                var theirsLength = SpanUtilsWriteUnsignedVLQ(theirs, value);
                Assert.Equal(theirsLength, mineLength);
                Assert.Equal(SpanUtilsMeasureVLQ(value), VarInt.MeasureUnsigned(value));
                Assert.True(mine.SequenceEqual(theirs), $"Wire format diverged for {value}.");
            }
        }

        [Theory]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        public void SignMagnitude_SentinelIsWidthDependent(int bits)
        {
            var minimum = VarInt.SignMagnitudeMinimum(bits);
            var encoded = Encode(b => VarInt.WriteSignMagnitude(b, minimum, bits));
            Assert.Equal([0x40], encoded);
            Assert.Equal(minimum, VarInt.ReadSignMagnitude(encoded, out _, bits));
        }

        [Fact]
        public void SignMagnitude_RoundTripsEveryNarrowValue()
        {
            var buffer = new byte[VarInt.MaxLength];
            for (var value = short.MinValue; ; value++)
            {
                var written = VarInt.WriteSignMagnitude(buffer, value, 16);
                Assert.Equal(VarInt.MeasureSignMagnitude(value, 16), written);
                Assert.Equal(value, VarInt.ReadSignMagnitude(buffer, out var read, 16));
                Assert.Equal(written, read);
                if (value == short.MaxValue) break;
            }
        }

        [Fact]
        public void SignMagnitude_RejectsValueOutsideTheRequestedWidth()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => VarInt.MeasureSignMagnitude(short.MaxValue + 1, 16));
            Assert.Throws<ArgumentOutOfRangeException>(() => VarInt.WriteSignMagnitude(new byte[10], short.MinValue - 1, 16));
        }
        #endregion

        [Fact]
        public void ShortDestination_ThrowsOrReports()
        {
            Assert.Throws<ArgumentException>(() => VarInt.WriteUnsigned(new byte[1], 300));
            Assert.Throws<ArgumentException>(() => VarInt.WriteSigned(new byte[1], 300));
            Assert.Throws<ArgumentException>(() => VarInt.WriteZigZag(new byte[1], 300));
            Assert.Throws<ArgumentException>(() => VarInt.WriteSignMagnitude(new byte[1], 300));

            Assert.False(VarInt.TryWriteUnsigned(new byte[1], 300, out var written));
            Assert.Equal(0, written);
            Assert.False(VarInt.TryWriteSigned(new byte[1], 300, out written));
            Assert.Equal(0, written);
            Assert.False(VarInt.TryWriteZigZag(new byte[1], 300, out written));
            Assert.Equal(0, written);
            Assert.False(VarInt.TryWriteSignMagnitude(new byte[1], 300, out written));
            Assert.Equal(0, written);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(65)]
        [InlineData(-1)]
        public void InvalidWidth_Throws(int bits)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => VarInt.ReadUnsigned(new byte[] { 0 }, out _, bits));
            Assert.Throws<ArgumentOutOfRangeException>(() => VarInt.ReadSigned(new byte[] { 0 }, out _, bits));
            Assert.Throws<ArgumentOutOfRangeException>(() => VarInt.ReadSignMagnitude(new byte[] { 0 }, out _, bits));
        }

        [Fact]
        public void SignMagnitude_RejectsOneBitWidth()
        {
            // A one bit signed integer has no room for a magnitude at all.
            Assert.Throws<ArgumentOutOfRangeException>(() => VarInt.SignMagnitudeMinimum(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => VarInt.ReadSigned(new byte[] { 0 }, out _, 1));
        }
    }
}
