using System;
using System.Numerics;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class ParityTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Theory]
        [InlineData(0u, 0)]
        [InlineData(1u, 1)]
        [InlineData(3u, 0)]
        [InlineData(0xFFu, 0)]
        [InlineData(0x80000000u, 1)]
        public void TestUInt32_KnownValues(uint input, int expected) { var v = input; Assert.Equal(expected, v.Parity()); }

        [Fact]
        public void TestByte()
        {
            for (var i = 0; i <= byte.MaxValue; i++)
            {
                var v = (byte)i;
                Assert.Equal(BitOperations.PopCount(v) & 1, v.Parity());
            }
        }

        [Fact]
        public void TestUInt16()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = (ushort)_rnd.Next(ushort.MaxValue + 1);
                Assert.Equal(BitOperations.PopCount(v) & 1, v.Parity());
            }
        }

        [Fact]
        public void TestInt32()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = _rnd.Next(int.MinValue, int.MaxValue);
                Assert.Equal(BitOperations.PopCount((uint)v) & 1, v.Parity());
            }
        }

        [Fact]
        public void TestUInt64()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next() << 32) | (uint)_rnd.Next();
                Assert.Equal(BitOperations.PopCount(v) & 1, v.Parity());
            }
        }
    }
}
