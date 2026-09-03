using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class TrailingZeroCountTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Fact]
        public void TestByte()
        {
            byte zero = 0;
            Assert.Equal(8, zero.TrailingZeroCount());
            for (var i = 0; i <= byte.MaxValue; i++)
            {
                var v = (byte)i;
                var expected = v == 0 ? 8 : CountTrailingZerosReference(v, 8);
                Assert.Equal(expected, v.TrailingZeroCount());
            }
        }

        [Fact]
        public void TestUInt16()
        {
            ushort zero = 0;
            Assert.Equal(16, zero.TrailingZeroCount());
            for (var n = 0; n < 16; n++)
            {
                ushort v = (ushort)(1 << n);
                Assert.Equal(n, v.TrailingZeroCount());
            }
        }

        [Fact]
        public void TestInt32()
        {
            var zero = 0;
            Assert.Equal(32, zero.TrailingZeroCount());
            for (var n = 0; n < 32; n++)
            {
                var v = unchecked((int)(1u << n));
                Assert.Equal(n, v.TrailingZeroCount());
            }
        }

        [Fact]
        public void TestUInt32_Random()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next(1, int.MaxValue); // never 0, checked separately above
                Assert.Equal(CountTrailingZerosReference(v, 32), v.TrailingZeroCount());
            }
        }

        [Fact]
        public void TestInt64()
        {
            var zero = 0L;
            Assert.Equal(64, zero.TrailingZeroCount());
            for (var n = 0; n < 64; n++)
            {
                var v = unchecked((long)(1ul << n));
                Assert.Equal(n, v.TrailingZeroCount());
            }
        }

        [Fact]
        public void TestUInt64_Random()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next(1, int.MaxValue) << 32) | (uint)_rnd.Next();
                Assert.Equal(CountTrailingZerosReference(v, 64), v.TrailingZeroCount());
            }
        }

        private static int CountTrailingZerosReference(ulong value, int width)
        {
            for (var i = 0; i < width; i++)
                if ((value & (1ul << i)) != 0)
                    return i;
            return width;
        }
    }
}
