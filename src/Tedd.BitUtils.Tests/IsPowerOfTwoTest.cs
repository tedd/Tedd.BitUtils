using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class IsPowerOfTwoTest
    {
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(4, true)]
        [InlineData(128, true)]
        [InlineData(255, false)]
        public void TestByte_SpecificValues(byte value, bool expected)
        {
            Assert.Equal(expected, value.IsPowerOfTwo());
        }

        [Fact]
        public void TestByte_AllValues()
        {
            for (var i = 0; i <= byte.MaxValue; i++)
            {
                var v = (byte)i;
                Assert.Equal(i != 0 && (i & (i - 1)) == 0, v.IsPowerOfTwo());
            }
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(4, true)]
        [InlineData(64, true)]
        [InlineData(127, false)]
        [InlineData(-1, false)]
        [InlineData(-2, false)]
        [InlineData(-128, false)]
        public void TestSByte_SpecificValues(sbyte value, bool expected)
        {
            Assert.Equal(expected, value.IsPowerOfTwo());
        }

        [Fact]
        public void TestSByte_NegativeIsNeverPowerOfTwo()
        {
            for (sbyte v = sbyte.MinValue; v < 0; v++)
                Assert.False(v.IsPowerOfTwo());
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(4, true)]
        [InlineData(1024, true)]
        [InlineData(32767, false)]
        [InlineData(-1, false)]
        [InlineData(-32768, false)]
        public void TestInt16_SpecificValues(short value, bool expected)
        {
            Assert.Equal(expected, value.IsPowerOfTwo());
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(4, true)]
        [InlineData(1024, true)]
        [InlineData(32768, true)]
        [InlineData(65535, false)]
        public void TestUInt16_SpecificValues(ushort value, bool expected)
        {
            Assert.Equal(expected, value.IsPowerOfTwo());
        }

        [Fact]
        public void TestInt32_PowersOfTwo()
        {
            for (var n = 0; n < 31; n++)
            {
                var v = 1 << n;
                Assert.True(v.IsPowerOfTwo(), $"1 << {n}");
            }
        }

        [Fact]
        public void TestInt32_Zero_And_Negative()
        {
            var zero = 0;
            Assert.False(zero.IsPowerOfTwo());
            var negative = -8; // -8 as two's complement is not a single bit
            Assert.False(negative.IsPowerOfTwo());
            var minValue = int.MinValue; // single high bit set, but negative -> false by contract
            Assert.False(minValue.IsPowerOfTwo());
        }

        [Theory]
        [InlineData(0L, false)]
        [InlineData(1L, true)]
        [InlineData(2L, true)]
        [InlineData(3L, false)]
        [InlineData(4L, true)]
        [InlineData(1024L, true)]
        [InlineData(1073741824L, true)] // 2^30
        [InlineData(4611686018427387904L, true)] // 2^62
        [InlineData(-1L, false)]
        [InlineData(-2L, false)]
        [InlineData(-9223372036854775808L, false)] // long.MinValue
        public void TestInt64_SpecificValues(long value, bool expected)
        {
            Assert.Equal(expected, value.IsPowerOfTwo());
        }

        [Fact]
        public void TestUInt32_HighBitSet()
        {
            var v = 0x80000000u;
            Assert.True(v.IsPowerOfTwo());
        }

        [Theory]
        [InlineData(0ul, false)]
        [InlineData(1ul, true)]
        [InlineData(2ul, true)]
        [InlineData(3ul, false)]
        [InlineData(4ul, true)]
        [InlineData(1024ul, true)]
        [InlineData(1073741824ul, true)] // 2^30
        [InlineData(4611686018427387904ul, true)] // 2^62
        [InlineData(9223372036854775808ul, true)] // 2^63
        public void TestUInt64_SpecificValues(ulong value, bool expected)
        {
            Assert.Equal(expected, value.IsPowerOfTwo());
        }

        [Fact]
        public void TestUInt64_PowersOfTwo()
        {
            for (var n = 0; n < 64; n++)
            {
                var v = 1ul << n;
                Assert.True(v.IsPowerOfTwo(), $"1ul << {n}");
            }
            var zero = 0ul;
            Assert.False(zero.IsPowerOfTwo());
            var three = 3ul;
            Assert.False(three.IsPowerOfTwo());
        }
    }
}
