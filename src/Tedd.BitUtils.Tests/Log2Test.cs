using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class Log2Test
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Fact]
        public void TestByte()
        {
            byte zero = 0;
            Assert.Equal(0, zero.Log2());
            for (var i = 1; i <= byte.MaxValue; i++)
            {
                var v = (byte)i;
                Assert.Equal(FloorLog2Reference(v), v.Log2());
            }
        }

        [Fact]
        public void TestUInt16()
        {
            ushort zero = 0;
            Assert.Equal(0, zero.Log2());
            for (var i = 0; i < Iterations; i++)
            {
                var v = (ushort)_rnd.Next(1, ushort.MaxValue + 1);
                Assert.Equal(FloorLog2Reference(v), v.Log2());
            }
        }

        [Fact]
        public void TestInt32()
        {
            var zero = 0;
            Assert.Equal(0, zero.Log2());
            for (var n = 0; n < 32; n++)
            {
                var v = unchecked((int)(1u << n));
                Assert.Equal(n, v.Log2());
                if (n < 31)
                {
                    var withLowBitSet = v | 1;
                    Assert.Equal(n, withLowBitSet.Log2()); // setting the low bit must not change floor(log2)
                }
            }
        }

        [Fact]
        public void TestUInt32_Random()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next(1, int.MaxValue);
                Assert.Equal(FloorLog2Reference(v), v.Log2());
            }
        }

        [Fact]
        public void TestUInt64_Random()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next(1, int.MaxValue) << 32) | (uint)_rnd.Next();
                Assert.Equal(FloorLog2Reference(v), v.Log2());
            }
        }

        private static int FloorLog2Reference(ulong value)
        {
            var n = -1;
            while (value != 0) { value >>= 1; n++; }
            return n;
        }
    }

    public class BitLengthTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Fact]
        public void TestByte()
        {
            byte zero = 0;
            Assert.Equal(0, zero.BitLength());
            for (var i = 1; i <= byte.MaxValue; i++)
            {
                var v = (byte)i;
                Assert.Equal(v.Log2() + 1, v.BitLength());
            }
        }

        [Fact]
        public void TestInt32_PowersOfTwo()
        {
            for (var n = 0; n < 32; n++)
            {
                var v = unchecked((int)(1u << n));
                Assert.Equal(n + 1, v.BitLength());
            }
        }

        [Fact]
        public void TestUInt64_Random()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next() << 32) | (uint)_rnd.Next();
                var expected = v == 0 ? 0 : (int)v.Log2() + 1;
                Assert.Equal(expected, v.BitLength());
            }
        }
    }
}
