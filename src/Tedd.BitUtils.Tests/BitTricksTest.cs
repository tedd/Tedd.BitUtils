using System;
using System.Numerics;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class ExtractLowestSetBitTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Theory]
        [InlineData((sbyte)0b0110_1100, (sbyte)0b0000_0100)]
        [InlineData((sbyte)0, (sbyte)0)]
        public void TestSByte_MatchesExpected(sbyte v, sbyte expected) => Assert.Equal(expected, v.ExtractLowestSetBit());

        [Theory]
        [InlineData((byte)0b0110_1100, (byte)0b0000_0100)]
        [InlineData((byte)0, (byte)0)]
        public void TestByte_MatchesExpected(byte v, byte expected) => Assert.Equal(expected, v.ExtractLowestSetBit());

        [Theory]
        [InlineData((short)0b0110_1100, (short)0b0000_0100)]
        [InlineData((short)0, (short)0)]
        public void TestInt16_MatchesExpected(short v, short expected) => Assert.Equal(expected, v.ExtractLowestSetBit());

        [Theory]
        [InlineData((ushort)0b0110_1100, (ushort)0b0000_0100)]
        [InlineData((ushort)0, (ushort)0)]
        public void TestUInt16_MatchesExpected(ushort v, ushort expected) => Assert.Equal(expected, v.ExtractLowestSetBit());

        [Theory]
        [InlineData((int)0b0110_1100, (int)0b0000_0100)]
        [InlineData((int)0, (int)0)]
        public void TestInt32_MatchesExpected(int v, int expected) => Assert.Equal(expected, v.ExtractLowestSetBit());

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

        [Theory]
        [InlineData((long)0b0110_1100, (long)0b0000_0100)]
        [InlineData((long)0, (long)0)]
        public void TestInt64_MatchesExpected(long v, long expected) => Assert.Equal(expected, v.ExtractLowestSetBit());

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

        [Theory]
        [InlineData((sbyte)0b0110_1100, (sbyte)0b0110_1000)]
        [InlineData((sbyte)0, (sbyte)0)]
        public void TestSByte_MatchesExpected(sbyte v, sbyte expected) => Assert.Equal(expected, v.ResetLowestSetBit());

        [Theory]
        [InlineData((byte)0b0110_1100, (byte)0b0110_1000)]
        [InlineData((byte)0, (byte)0)]
        public void TestByte_MatchesExpected(byte v, byte expected) => Assert.Equal(expected, v.ResetLowestSetBit());

        [Theory]
        [InlineData((short)0b0110_1100, (short)0b0110_1000)]
        [InlineData((short)0, (short)0)]
        public void TestInt16_MatchesExpected(short v, short expected) => Assert.Equal(expected, v.ResetLowestSetBit());

        [Theory]
        [InlineData((ushort)0b0110_1100, (ushort)0b0110_1000)]
        [InlineData((ushort)0, (ushort)0)]
        public void TestUInt16_MatchesExpected(ushort v, ushort expected) => Assert.Equal(expected, v.ResetLowestSetBit());

        [Theory]
        [InlineData((int)0b0110_1100, (int)0b0110_1000)]
        [InlineData((int)0, (int)0)]
        public void TestInt32_MatchesExpected(int v, int expected) => Assert.Equal(expected, v.ResetLowestSetBit());

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

        [Theory]
        [InlineData((long)0b0110_1100, (long)0b0110_1000)]
        [InlineData((long)0, (long)0)]
        public void TestInt64_MatchesExpected(long v, long expected) => Assert.Equal(expected, v.ResetLowestSetBit());

        [Theory]
        [InlineData((ulong)0b0110_1100, (ulong)0b0110_1000)]
        [InlineData((ulong)0, (ulong)0)]
        public void TestUInt64_MatchesExpected(ulong v, ulong expected) => Assert.Equal(expected, v.ResetLowestSetBit());
    }

    public class GetMaskUpToLowestSetBitTest
    {
        [Theory]
        [InlineData((sbyte)0b0110_1000, (sbyte)0b0000_1111)]
        [InlineData((sbyte)0, (sbyte)-1)]
        public void TestSByte_MatchesExpected(sbyte v, sbyte expected) => Assert.Equal(expected, v.GetMaskUpToLowestSetBit());

        [Theory]
        [InlineData((byte)0b0110_1000, (byte)0b0000_1111)]
        [InlineData((byte)0, (byte)0xFF)]
        public void TestByte_MatchesExpected(byte v, byte expected) => Assert.Equal(expected, v.GetMaskUpToLowestSetBit());

        [Theory]
        [InlineData((short)0b0110_1000, (short)0b0000_1111)]
        [InlineData((short)0, (short)-1)]
        public void TestInt16_MatchesExpected(short v, short expected) => Assert.Equal(expected, v.GetMaskUpToLowestSetBit());

        [Theory]
        [InlineData((ushort)0b0110_1000, (ushort)0b0000_1111)]
        [InlineData((ushort)0, (ushort)0xFFFF)]
        public void TestUInt16_MatchesExpected(ushort v, ushort expected) => Assert.Equal(expected, v.GetMaskUpToLowestSetBit());

        [Theory]
        [InlineData((int)0b0110_1000, (int)0b0000_1111)]
        [InlineData((int)0, (int)-1)]
        public void TestInt32_MatchesExpected(int v, int expected) => Assert.Equal(expected, v.GetMaskUpToLowestSetBit());

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

        [Theory]
        [InlineData((long)0b0110_1000, (long)0b0000_1111)]
        [InlineData((long)0, (long)-1)]
        public void TestInt64_MatchesExpected(long v, long expected) => Assert.Equal(expected, v.GetMaskUpToLowestSetBit());

        [Theory]
        [InlineData((ulong)0b0110_1000, (ulong)0b0000_1111)]
        [InlineData((ulong)0, ulong.MaxValue)]
        public void TestUInt64_MatchesExpected(ulong v, ulong expected) => Assert.Equal(expected, v.GetMaskUpToLowestSetBit());
    }

    public class ExtractHighestSetBitTest
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Theory]
        [InlineData((sbyte)0b0110_1100, (sbyte)0b0100_0000)]
        [InlineData((sbyte)0, (sbyte)0)]
        [InlineData((sbyte)-1, (sbyte)-128)]
        public void TestSByte_MatchesExpected(sbyte v, sbyte expected) => Assert.Equal(expected, v.ExtractHighestSetBit());

        [Theory]
        [InlineData((byte)0b0110_1100, (byte)0b0100_0000)]
        [InlineData((byte)0, (byte)0)]
        public void TestByte_MatchesExpected(byte v, byte expected) => Assert.Equal(expected, v.ExtractHighestSetBit());

        [Theory]
        [InlineData((short)0b0110_1100, (short)0b0100_0000)]
        [InlineData((short)0, (short)0)]
        [InlineData((short)-1, (short)-32768)]
        public void TestInt16_MatchesExpected(short v, short expected) => Assert.Equal(expected, v.ExtractHighestSetBit());

        [Theory]
        [InlineData((ushort)0b0110_1100, (ushort)0b0100_0000)]
        [InlineData((ushort)0, (ushort)0)]
        public void TestUInt16_MatchesExpected(ushort v, ushort expected) => Assert.Equal(expected, v.ExtractHighestSetBit());

        [Theory]
        [InlineData((int)0b0110_1100, (int)0b0100_0000)]
        [InlineData((int)0, (int)0)]
        [InlineData((int)-1, int.MinValue)]
        public void TestInt32_MatchesExpected(int v, int expected) => Assert.Equal(expected, v.ExtractHighestSetBit());

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

        [Theory]
        [InlineData((long)0b0110_1100, (long)0b0100_0000)]
        [InlineData((long)0, (long)0)]
        [InlineData((long)-1, long.MinValue)]
        public void TestInt64_MatchesExpected(long v, long expected) => Assert.Equal(expected, v.ExtractHighestSetBit());

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
        [Theory]
        [InlineData((sbyte)-1, 0, (sbyte)0)]
        [InlineData((sbyte)-1, 4, (sbyte)0b1111)]
        [InlineData((sbyte)-1, 8, (sbyte)-1)]
        public void TestSByte_KeepsOnlyLowIndexBits(sbyte v, int bits, sbyte expected) => Assert.Equal(expected, v.ZeroHighBits(bits));

        [Theory]
        [InlineData((byte)0xFF, 0, (byte)0)]
        [InlineData((byte)0xFF, 2, (byte)0b0011)]
        [InlineData((byte)0xFF, 8, (byte)0xFF)]
        public void TestByte_KeepsOnlyLowIndexBits(byte v, int bits, byte expected) => Assert.Equal(expected, v.ZeroHighBits(bits));

        [Theory]
        [InlineData((short)-1, 0, (short)0)]
        [InlineData((short)-1, 4, (short)0b1111)]
        [InlineData((short)-1, 16, (short)-1)]
        public void TestInt16_KeepsOnlyLowIndexBits(short v, int bits, short expected) => Assert.Equal(expected, v.ZeroHighBits(bits));

        [Theory]
        [InlineData((ushort)0xFFFF, 0, (ushort)0)]
        [InlineData((ushort)0xFFFF, 4, (ushort)0b1111)]
        [InlineData((ushort)0xFFFF, 16, (ushort)0xFFFF)]
        public void TestUInt16_KeepsOnlyLowIndexBits(ushort v, int bits, ushort expected) => Assert.Equal(expected, v.ZeroHighBits(bits));

        [Theory]
        [InlineData((int)-1, 0, (int)0)]
        [InlineData((int)-1, 4, (int)0b1111)]
        [InlineData((int)-1, 32, (int)-1)]
        public void TestInt32_KeepsOnlyLowIndexBits(int v, int bits, int expected) => Assert.Equal(expected, v.ZeroHighBits(bits));

        [Theory]
        [InlineData(0xFFFFFFFFu, 0, 0u)]
        [InlineData(0xFFFFFFFFu, 4, 0b1111u)]
        [InlineData(0xFFFFFFFFu, 32, 0xFFFFFFFFu)]
        public void TestUInt32_KeepsOnlyLowIndexBits(uint v, int bits, uint expected) => Assert.Equal(expected, v.ZeroHighBits(bits));

        [Theory]
        [InlineData((long)-1, 0, (long)0)]
        [InlineData((long)-1, 8, (long)0xFF)]
        [InlineData((long)-1, 64, (long)-1)]
        public void TestInt64_KeepsOnlyLowIndexBits(long v, int bits, long expected) => Assert.Equal(expected, v.ZeroHighBits(bits));

        [Theory]
        [InlineData(0xFFFFFFFF_FFFFFFFFul, 0, 0ul)]
        [InlineData(0xFFFFFFFF_FFFFFFFFul, 8, 0xFFul)]
        [InlineData(0xFFFFFFFF_FFFFFFFFul, 64, 0xFFFFFFFF_FFFFFFFFul)]
        public void TestUInt64_KeepsOnlyLowIndexBits(ulong v, int bits, ulong expected) => Assert.Equal(expected, v.ZeroHighBits(bits));
    }

    public class ZeroLowBitsTest
    {
        [Theory]
        [InlineData((sbyte)-1, 0, (sbyte)-1)]
        [InlineData((sbyte)-1, 4, (sbyte)-16)]
        [InlineData((sbyte)-1, 8, (sbyte)0)]
        public void TestSByte_ClearsOnlyLowIndexBits(sbyte v, int bits, sbyte expected) => Assert.Equal(expected, v.ZeroLowBits(bits));

        [Theory]
        [InlineData((byte)0xFF, 0, (byte)0xFF)]
        [InlineData((byte)0xFF, 4, (byte)0b1111_0000)]
        [InlineData((byte)0xFF, 8, (byte)0)]
        public void TestByte_ClearsOnlyLowIndexBits(byte v, int bits, byte expected) => Assert.Equal(expected, v.ZeroLowBits(bits));

        [Theory]
        [InlineData((short)-1, 0, (short)-1)]
        [InlineData((short)-1, 4, (short)-16)] // 0xFFF0
        [InlineData((short)-1, 16, (short)0)]
        public void TestInt16_ClearsOnlyLowIndexBits(short v, int bits, short expected) => Assert.Equal(expected, v.ZeroLowBits(bits));

        [Theory]
        [InlineData((ushort)0xFFFF, 0, (ushort)0xFFFF)]
        [InlineData((ushort)0xFFFF, 4, (ushort)0xFFF0)]
        [InlineData((ushort)0xFFFF, 16, (ushort)0)]
        public void TestUInt16_ClearsOnlyLowIndexBits(ushort v, int bits, ushort expected) => Assert.Equal(expected, v.ZeroLowBits(bits));

        [Theory]
        [InlineData((int)-1, 0, (int)-1)]
        [InlineData((int)-1, 4, (int)-16)] // 0xFFFFFFF0
        [InlineData((int)-1, 32, (int)0)]
        public void TestInt32_ClearsOnlyLowIndexBits(int v, int bits, int expected) => Assert.Equal(expected, v.ZeroLowBits(bits));

        [Theory]
        [InlineData(0xFFFFFFFFu, 0, 0xFFFFFFFFu)]
        [InlineData(0xFFFFFFFFu, 4, 0xFFFFFFF0u)]
        [InlineData(0xFFFFFFFFu, 32, 0u)]
        public void TestUInt32_ClearsOnlyLowIndexBits(uint v, int bits, uint expected) => Assert.Equal(expected, v.ZeroLowBits(bits));

        [Fact]
        public void TestUInt32_ComplementsZeroHighBits()
        {
            var v = 0b1101_1010u;
            for (var index = 0; index <= 32; index++)
                Assert.Equal(v, v.ZeroHighBits(index) | v.ZeroLowBits(index));
        }

        [Theory]
        [InlineData((long)-1, 0, (long)-1)]
        [InlineData((long)-1, 4, (long)-16)]
        [InlineData((long)-1, 64, (long)0)]
        public void TestInt64_ClearsOnlyLowIndexBits(long v, int bits, long expected) => Assert.Equal(expected, v.ZeroLowBits(bits));

        [Theory]
        [InlineData(0xFFFFFFFF_FFFFFFFFul, 0, 0xFFFFFFFF_FFFFFFFFul)]
        [InlineData(0xFFFFFFFF_FFFFFFFFul, 4, 0xFFFFFFFF_FFFFFFF0ul)]
        [InlineData(0xFFFFFFFF_FFFFFFFFul, 64, 0ul)]
        public void TestUInt64_ClearsOnlyLowIndexBits(ulong v, int bits, ulong expected) => Assert.Equal(expected, v.ZeroLowBits(bits));
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
