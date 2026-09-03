using System;
using System.Numerics;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class ExtractLowestSetBitTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Fact]
        public void TestByte_KnownValues()
        {
            byte v = 0b0110_1100;
            Assert.Equal((byte)0b0000_0100, v.ExtractLowestSetBit());
            byte zero = 0;
            Assert.Equal((byte)0, zero.ExtractLowestSetBit());
        }

        [Fact]
        public void TestUInt32_Random()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next(1, int.MaxValue);
                var expected = 1u << BitOperations.TrailingZeroCount(v);
                Assert.Equal(expected, v.ExtractLowestSetBit());
            }
        }

        [Fact]
        public void TestUInt64_Random()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next(1, int.MaxValue) << 32) | (uint)_rnd.Next();
                var expected = 1ul << BitOperations.TrailingZeroCount(v);
                Assert.Equal(expected, v.ExtractLowestSetBit());
            }
        }
    }

    public class ResetLowestSetBitTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Fact]
        public void TestByte_KnownValues()
        {
            byte v = 0b0110_1100;
            Assert.Equal((byte)0b0110_1000, v.ResetLowestSetBit());
        }

        [Fact]
        public void TestUInt32_CombinedWithExtractGivesOriginal()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next(1, int.MaxValue);
                var reset = v.ResetLowestSetBit();
                var lowest = v.ExtractLowestSetBit();
                Assert.Equal(v, reset | lowest);
                Assert.Equal(0u, reset & lowest);
                Assert.True(BitOperations.PopCount(reset) == BitOperations.PopCount(v) - 1);
            }
        }
    }

    public class GetMaskUpToLowestSetBitTest
    {
        [Fact]
        public void TestByte_KnownValues()
        {
            byte v = 0b0110_1000;
            Assert.Equal((byte)0b0000_1111, v.GetMaskUpToLowestSetBit());
        }

        [Fact]
        public void TestUInt32_Zero_GivesAllOnes()
        {
            uint zero = 0;
            Assert.Equal(uint.MaxValue, zero.GetMaskUpToLowestSetBit());
        }

        [Fact]
        public void TestUInt32_CoversBitsUpToAndIncludingLowestSetBit()
        {
            uint v = 0b1100u; // lowest set bit is at index 2
            Assert.Equal(0b0111u, v.GetMaskUpToLowestSetBit()); // bits [0,2] set
        }
    }

    public class ExtractHighestSetBitTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Fact]
        public void TestByte_KnownValues()
        {
            byte v = 0b0110_1100;
            Assert.Equal((byte)0b0100_0000, v.ExtractHighestSetBit());
            byte zero = 0;
            Assert.Equal((byte)0, zero.ExtractHighestSetBit());
        }

        [Fact]
        public void TestUInt32_Random()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next(1, int.MaxValue);
                var expected = 1u << BitOperations.Log2(v);
                Assert.Equal(expected, v.ExtractHighestSetBit());
            }
        }

        [Fact]
        public void TestUInt64_Random()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next(1, int.MaxValue) << 32) | (uint)_rnd.Next();
                var expected = 1ul << BitOperations.Log2(v);
                Assert.Equal(expected, v.ExtractHighestSetBit());
            }
        }
    }

    public class ZeroHighBitsTest
    {
        [Fact]
        public void TestUInt32_KeepsOnlyLowIndexBits()
        {
            var v = 0xFFFFFFFFu;
            Assert.Equal(0u, v.ZeroHighBits(0));
            Assert.Equal(0b1111u, v.ZeroHighBits(4));
            Assert.Equal(0xFFFFFFFFu, v.ZeroHighBits(32));
        }

        [Fact]
        public void TestUInt64_KeepsOnlyLowIndexBits()
        {
            var v = 0xFFFFFFFF_FFFFFFFFul;
            Assert.Equal(0ul, v.ZeroHighBits(0));
            Assert.Equal(0xFFul, v.ZeroHighBits(8));
            Assert.Equal(0xFFFFFFFF_FFFFFFFFul, v.ZeroHighBits(64));
        }

        [Fact]
        public void TestByte()
        {
            byte v = 0xFF;
            Assert.Equal((byte)0b0011, v.ZeroHighBits(2));
        }
    }

    public class ZeroLowBitsTest
    {
        [Fact]
        public void TestUInt32_ClearsOnlyLowIndexBits()
        {
            var v = 0xFFFFFFFFu;
            Assert.Equal(0xFFFFFFFFu, v.ZeroLowBits(0));
            Assert.Equal(0xFFFFFFF0u, v.ZeroLowBits(4));
            Assert.Equal(0u, v.ZeroLowBits(32));
        }

        [Fact]
        public void TestUInt32_ComplementsZeroHighBits()
        {
            var v = 0b1101_1010u;
            for (var index = 0; index <= 32; index++)
                Assert.Equal(v, v.ZeroHighBits(index) | v.ZeroLowBits(index));
        }
    }

    public class ParallelBitExtractTest
    {
        [Fact]
        public void TestUInt32_KnownValue()
        {
            uint value = 0b_1101_1010;
            uint mask = 0b_0000_1111;
            Assert.Equal(0b1010u, value.ParallelBitExtract(mask));
        }

        [Fact]
        public void TestUInt32_HighContiguousMask()
        {
            uint value = 0b_1101_1010;
            uint mask = 0b_1111_0000;
            Assert.Equal(0b1101u, value.ParallelBitExtract(mask)); // AND then shift down 4
        }

        [Fact]
        public void TestUInt64_FullMaskIsIdentity()
        {
            ulong value = 0x0123456789ABCDEFul;
            Assert.Equal(value, value.ParallelBitExtract(ulong.MaxValue));
        }

        [Fact]
        public void TestInt32_MatchesUInt32()
        {
            int value = unchecked((int)0xF0F0F0F0);
            int mask = 0x0F0F0F0F;
            var unsignedValue = (uint)value;
            var expected = (int)unsignedValue.ParallelBitExtract((uint)mask);
            Assert.Equal(expected, value.ParallelBitExtract(mask));
        }
    }

    public class ParallelBitDepositTest
    {
        [Fact]
        public void TestUInt32_KnownValue()
        {
            uint value = 0b1010;
            uint mask = 0b_0000_1111;
            Assert.Equal(0b_0000_1010u, value.ParallelBitDeposit(mask));
        }

        [Fact]
        public void TestUInt32_HighContiguousMask()
        {
            uint value = 0b1010;
            uint mask = 0b_1111_0000;
            Assert.Equal(0b_1010_0000u, value.ParallelBitDeposit(mask)); // shift up 4 then AND with mask
        }

        [Fact]
        public void TestUInt64_FullMaskIsIdentity()
        {
            ulong value = 0x0123456789ABCDEFul;
            Assert.Equal(value, value.ParallelBitDeposit(ulong.MaxValue));
        }

        [Fact]
        public void TestUInt32_IsInverseOfExtractUnderSameMask()
        {
            uint value = 0xDEADBEEFu;
            uint mask = 0xF0F0F0F0u;
            var extracted = value.ParallelBitExtract(mask);
            var deposited = extracted.ParallelBitDeposit(mask);
            Assert.Equal(value & mask, deposited);
        }
    }
}
