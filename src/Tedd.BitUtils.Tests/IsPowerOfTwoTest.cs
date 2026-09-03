using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class IsPowerOfTwoTest
    {
        [Fact]
        public void TestByte_AllValues()
        {
            for (var i = 0; i <= byte.MaxValue; i++)
            {
                var v = (byte)i;
                Assert.Equal(i != 0 && (i & (i - 1)) == 0, v.IsPowerOfTwo());
            }
        }

        [Fact]
        public void TestSByte_NegativeIsNeverPowerOfTwo()
        {
            for (sbyte v = sbyte.MinValue; v < 0; v++)
                Assert.False(v.IsPowerOfTwo());
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

        [Fact]
        public void TestUInt32_HighBitSet()
        {
            var v = 0x80000000u;
            Assert.True(v.IsPowerOfTwo());
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
