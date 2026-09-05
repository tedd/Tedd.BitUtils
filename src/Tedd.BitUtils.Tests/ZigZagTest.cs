using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class ZigZagTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Fact]
        public void KnownSequence_InterleavesSigns()
        {
            // 0, -1, 1, -2, 2, ... must map onto 0, 1, 2, 3, 4, ...
            ReadOnlySpan<long> values = [0, -1, 1, -2, 2, -3, 3];
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                Assert.Equal((ulong)i, value.ZigZagEncode());
            }
        }

        [Fact]
        public void Extremes_MapToExtremes()
        {
            sbyte sbyteMin = sbyte.MinValue, sbyteMax = sbyte.MaxValue;
            Assert.Equal(byte.MaxValue, sbyteMin.ZigZagEncode());
            Assert.Equal((byte)(byte.MaxValue - 1), sbyteMax.ZigZagEncode());

            short shortMin = short.MinValue, shortMax = short.MaxValue;
            Assert.Equal(ushort.MaxValue, shortMin.ZigZagEncode());
            Assert.Equal((ushort)(ushort.MaxValue - 1), shortMax.ZigZagEncode());

            int intMin = int.MinValue, intMax = int.MaxValue;
            Assert.Equal(uint.MaxValue, intMin.ZigZagEncode());
            Assert.Equal(uint.MaxValue - 1, intMax.ZigZagEncode());

            long longMin = long.MinValue, longMax = long.MaxValue;
            Assert.Equal(ulong.MaxValue, longMin.ZigZagEncode());
            Assert.Equal(ulong.MaxValue - 1, longMax.ZigZagEncode());
        }

        [Fact]
        public void SByte_RoundTripsEveryValue()
        {
            var seen = new bool[256];
            for (var i = sbyte.MinValue; ; i++)
            {
                var value = (sbyte)i;
                var encoded = value.ZigZagEncode();
                Assert.False(seen[encoded], $"ZigZag is not injective: {encoded} produced twice.");
                seen[encoded] = true;
                Assert.Equal(value, encoded.ZigZagDecode());
                if (i == sbyte.MaxValue) break;
            }
            Assert.DoesNotContain(false, seen);   // and surjective, so the full byte range is used
        }

        [Fact]
        public void Int16_RoundTripsEveryValue()
        {
            for (var i = short.MinValue; ; i++)
            {
                var value = (short)i;
                var encoded = value.ZigZagEncode();
                Assert.Equal(value, encoded.ZigZagDecode());
                if (i == short.MaxValue) break;
            }
        }

        [Fact]
        public void Int32_RoundTripsRandomValues()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var value = (int)(uint)_rnd.NextInt64(uint.MinValue, (long)uint.MaxValue + 1);
                var encoded = value.ZigZagEncode();
                Assert.Equal(value, encoded.ZigZagDecode());
            }
        }

        [Fact]
        public void Int64_RoundTripsRandomValues()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var value = _rnd.NextInt64(long.MinValue, long.MaxValue);
                var encoded = value.ZigZagEncode();
                Assert.Equal(value, encoded.ZigZagDecode());
            }
        }

        [Fact]
        public void Magnitude_DeterminesEncodedSize()
        {
            // The point of ZigZag: a small negative number stays small instead of becoming a 64 bit pattern.
            long minusOne = -1;
            Assert.Equal(1UL, minusOne.ZigZagEncode());
            long minusSixtyFour = -64;
            Assert.Equal(127UL, minusSixtyFour.ZigZagEncode());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(ulong.MaxValue)]
        [InlineData(ulong.MaxValue - 1)]
        public void UInt64_DecodeEncodeIsIdentity(ulong encoded)
        {
            var decoded = encoded.ZigZagDecode();
            Assert.Equal(encoded, decoded.ZigZagEncode());
        }
    }
}
