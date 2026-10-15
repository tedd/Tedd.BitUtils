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

        [Theory]
        [InlineData((sbyte)0, (sbyte)0)]
        [InlineData((sbyte)0b0110_1100, (sbyte)0b0000_0100)]
        [InlineData((sbyte)-128, (sbyte)-128)]
        public void TestSByte(sbyte value, sbyte expected)
        {
            var v = value;
            Assert.Equal(expected, v.ExtractLowestSetBit());
        }

        [Theory]
        [InlineData((short)0, (short)0)]
        [InlineData((short)0b0110_1100, (short)0b0000_0100)]
        [InlineData(short.MinValue, short.MinValue)]
        public void TestInt16(short value, short expected)
        {
            var v = value;
            Assert.Equal(expected, v.ExtractLowestSetBit());
        }

        [Theory]
        [InlineData((ushort)0, (ushort)0)]
        [InlineData((ushort)0b0110_1100, (ushort)0b0000_0100)]
        [InlineData(ushort.MaxValue, (ushort)1)]
        public void TestUInt16(ushort value, ushort expected)
        {
            var v = value;
            Assert.Equal(expected, v.ExtractLowestSetBit());
        }

        [Theory]
        [InlineData((int)0, (int)0)]
        [InlineData((int)0b0110_1100, (int)0b0000_0100)]
        [InlineData(int.MinValue, int.MinValue)]
        public void TestInt32(int value, int expected)
        {
            var v = value;
            Assert.Equal(expected, v.ExtractLowestSetBit());
        }

        [Theory]
        [InlineData((long)0, (long)0)]
        [InlineData((long)0b0110_1100, (long)0b0000_0100)]
        [InlineData(long.MinValue, long.MinValue)]
        public void TestInt64(long value, long expected)
        {
            var v = value;
            Assert.Equal(expected, v.ExtractLowestSetBit());
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

        [Theory]
        [InlineData((sbyte)0, (sbyte)0)]
        [InlineData((sbyte)0b0110_1100, (sbyte)0b0110_1000)]
        [InlineData(sbyte.MinValue, (sbyte)0)]
        public void TestSByte(sbyte value, sbyte expected)
        {
            var v = value;
            Assert.Equal(expected, v.ResetLowestSetBit());
        }

        [Theory]
        [InlineData((short)0, (short)0)]
        [InlineData((short)0b0110_1100, (short)0b0110_1000)]
        [InlineData(short.MinValue, (short)0)]
        public void TestInt16(short value, short expected)
        {
            var v = value;
            Assert.Equal(expected, v.ResetLowestSetBit());
        }

        [Theory]
        [InlineData((ushort)0, (ushort)0)]
        [InlineData((ushort)0b0110_1100, (ushort)0b0110_1000)]
        [InlineData(ushort.MaxValue, (ushort)(ushort.MaxValue - 1))]
        public void TestUInt16(ushort value, ushort expected)
        {
            var v = value;
            Assert.Equal(expected, v.ResetLowestSetBit());
        }

        [Theory]
        [InlineData((int)0, (int)0)]
        [InlineData((int)0b0110_1100, (int)0b0110_1000)]
        [InlineData(int.MinValue, (int)0)]
        public void TestInt32(int value, int expected)
        {
            var v = value;
            Assert.Equal(expected, v.ResetLowestSetBit());
        }

        [Theory]
        [InlineData((long)0, (long)0)]
        [InlineData((long)0b0110_1100, (long)0b0110_1000)]
        [InlineData(long.MinValue, (long)0)]
        public void TestInt64(long value, long expected)
        {
            var v = value;
            Assert.Equal(expected, v.ResetLowestSetBit());
        }

        [Theory]
        [InlineData((ulong)0, (ulong)0)]
        [InlineData((ulong)0b0110_1100, (ulong)0b0110_1000)]
        [InlineData(ulong.MaxValue, ulong.MaxValue - 1)]
        public void TestUInt64(ulong value, ulong expected)
        {
            var v = value;
            Assert.Equal(expected, v.ResetLowestSetBit());
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

        [Theory]
        [InlineData((sbyte)0, (sbyte)-1)]
        [InlineData((sbyte)0b0110_1000, (sbyte)0b0000_1111)]
        [InlineData(sbyte.MinValue, (sbyte)-1)]
        public void TestSByte(sbyte value, sbyte expected)
        {
            var v = value;
            Assert.Equal(expected, v.GetMaskUpToLowestSetBit());
        }

        [Theory]
        [InlineData((short)0, (short)-1)]
        [InlineData((short)0b0110_1000, (short)0b0000_1111)]
        [InlineData(short.MinValue, (short)-1)]
        public void TestInt16(short value, short expected)
        {
            var v = value;
            Assert.Equal(expected, v.GetMaskUpToLowestSetBit());
        }

        [Theory]
        [InlineData((ushort)0, ushort.MaxValue)]
        [InlineData((ushort)0b0110_1000, (ushort)0b0000_1111)]
        [InlineData(ushort.MaxValue, (ushort)1)]
        public void TestUInt16(ushort value, ushort expected)
        {
            var v = value;
            Assert.Equal(expected, v.GetMaskUpToLowestSetBit());
        }

        [Theory]
        [InlineData((int)0, (int)-1)]
        [InlineData((int)0b0110_1000, (int)0b0000_1111)]
        [InlineData(int.MinValue, (int)-1)]
        public void TestInt32(int value, int expected)
        {
            var v = value;
            Assert.Equal(expected, v.GetMaskUpToLowestSetBit());
        }

        [Theory]
        [InlineData((long)0, (long)-1)]
        [InlineData((long)0b0110_1000, (long)0b0000_1111)]
        [InlineData(long.MinValue, (long)-1)]
        public void TestInt64(long value, long expected)
        {
            var v = value;
            Assert.Equal(expected, v.GetMaskUpToLowestSetBit());
        }

        [Theory]
        [InlineData((ulong)0, ulong.MaxValue)]
        [InlineData((ulong)0b0110_1000, (ulong)0b0000_1111)]
        [InlineData(ulong.MaxValue, (ulong)1)]
        public void TestUInt64(ulong value, ulong expected)
        {
            var v = value;
            Assert.Equal(expected, v.GetMaskUpToLowestSetBit());
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

        [Theory]
        [InlineData((sbyte)0, (sbyte)0)]
        [InlineData((sbyte)0b0110_1100, (sbyte)0b0100_0000)]
        [InlineData(sbyte.MinValue, (sbyte)-128)]
        public void TestSByte(sbyte value, sbyte expected)
        {
            var v = value;
            Assert.Equal(expected, v.ExtractHighestSetBit());
        }

        [Theory]
        [InlineData((short)0, (short)0)]
        [InlineData((short)0b0110_1100, (short)0b0100_0000)]
        [InlineData(short.MinValue, unchecked((short)0x8000))]
        public void TestInt16(short value, short expected)
        {
            var v = value;
            Assert.Equal(expected, v.ExtractHighestSetBit());
        }

        [Theory]
        [InlineData((ushort)0, (ushort)0)]
        [InlineData((ushort)0b0110_1100, (ushort)0b0100_0000)]
        [InlineData(ushort.MaxValue, (ushort)0x8000)]
        public void TestUInt16(ushort value, ushort expected)
        {
            var v = value;
            Assert.Equal(expected, v.ExtractHighestSetBit());
        }

        [Theory]
        [InlineData((int)0, (int)0)]
        [InlineData((int)0b0110_1100, (int)0b0100_0000)]
        [InlineData(int.MinValue, unchecked((int)0x80000000))]
        public void TestInt32(int value, int expected)
        {
            var v = value;
            Assert.Equal(expected, v.ExtractHighestSetBit());
        }

        [Theory]
        [InlineData((long)0, (long)0)]
        [InlineData((long)0b0110_1100, (long)0b0100_0000)]
        [InlineData(long.MinValue, unchecked((long)0x8000000000000000))]
        public void TestInt64(long value, long expected)
        {
            var v = value;
            Assert.Equal(expected, v.ExtractHighestSetBit());
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

        [Theory]
        [InlineData((sbyte)-1, 0, (sbyte)0)]
        [InlineData((sbyte)-1, 4, (sbyte)0x0F)]
        [InlineData((sbyte)-1, 8, (sbyte)-1)]
        [InlineData((sbyte)-1, -1, (sbyte)0)]
        public void TestSByte(sbyte value, int index, sbyte expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroHighBits(index));
        }

        [Theory]
        [InlineData((byte)0xFF, 0, (byte)0)]
        [InlineData((byte)0xFF, 2, (byte)0b0011)]
        [InlineData((byte)0xFF, 4, (byte)0x0F)]
        [InlineData((byte)0xFF, 8, (byte)0xFF)]
        [InlineData((byte)0xFF, -1, (byte)0)]
        public void TestByte(byte value, int index, byte expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroHighBits(index));
        }

        [Theory]
        [InlineData((short)-1, 0, (short)0)]
        [InlineData((short)-1, 8, (short)0x00FF)]
        [InlineData((short)-1, 16, (short)-1)]
        [InlineData((short)-1, -1, (short)0)]
        public void TestInt16(short value, int index, short expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroHighBits(index));
        }

        [Theory]
        [InlineData((ushort)0xFFFF, 0, (ushort)0)]
        [InlineData((ushort)0xFFFF, 8, (ushort)0x00FF)]
        [InlineData((ushort)0xFFFF, 16, (ushort)0xFFFF)]
        [InlineData((ushort)0xFFFF, -1, (ushort)0)]
        public void TestUInt16(ushort value, int index, ushort expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroHighBits(index));
        }

        [Theory]
        [InlineData((int)-1, 0, (int)0)]
        [InlineData((int)-1, 16, (int)0x0000FFFF)]
        [InlineData((int)-1, 32, (int)-1)]
        [InlineData((int)-1, -1, (int)0)]
        public void TestInt32(int value, int index, int expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroHighBits(index));
        }

        [Theory]
        [InlineData((long)-1, 0, (long)0)]
        [InlineData((long)-1, 32, (long)0x00000000FFFFFFFF)]
        [InlineData((long)-1, 64, (long)-1)]
        [InlineData((long)-1, -1, (long)0)]
        public void TestInt64(long value, int index, long expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroHighBits(index));
        }

        [Theory]
        [InlineData(ulong.MaxValue, 0, 0ul)]
        [InlineData(ulong.MaxValue, 32, 0x00000000FFFFFFFFul)]
        [InlineData(ulong.MaxValue, 64, ulong.MaxValue)]
        [InlineData(ulong.MaxValue, -1, 0ul)]
        public void TestUInt64(ulong value, int index, ulong expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroHighBits(index));
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

        [Theory]
        [InlineData((sbyte)-1, 0, (sbyte)-1)]
        [InlineData((sbyte)-1, 4, (sbyte)-16)]
        [InlineData((sbyte)-1, 8, (sbyte)0x00)]
        [InlineData((sbyte)-1, -1, (sbyte)-1)]
        public void TestSByte(sbyte value, int index, sbyte expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroLowBits(index));
        }

        [Theory]
        [InlineData((byte)0xFF, 0, (byte)0xFF)]
        [InlineData((byte)0xFF, 4, (byte)0xF0)]
        [InlineData((byte)0xFF, 8, (byte)0x00)]
        [InlineData((byte)0xFF, -1, (byte)0xFF)]
        public void TestByte(byte value, int index, byte expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroLowBits(index));
        }

        [Theory]
        [InlineData((short)-1, 0, (short)-1)]
        [InlineData((short)-1, 8, (short)unchecked((short)0xFF00))]
        [InlineData((short)-1, 16, (short)0)]
        [InlineData((short)-1, -1, (short)-1)]
        public void TestInt16(short value, int index, short expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroLowBits(index));
        }

        [Theory]
        [InlineData((ushort)0xFFFF, 0, (ushort)0xFFFF)]
        [InlineData((ushort)0xFFFF, 8, (ushort)0xFF00)]
        [InlineData((ushort)0xFFFF, 16, (ushort)0)]
        [InlineData((ushort)0xFFFF, -1, (ushort)0xFFFF)]
        public void TestUInt16(ushort value, int index, ushort expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroLowBits(index));
        }

        [Theory]
        [InlineData((int)-1, 0, (int)-1)]
        [InlineData((int)-1, 16, unchecked((int)0xFFFF0000))]
        [InlineData((int)-1, 32, (int)0)]
        [InlineData((int)-1, -1, (int)-1)]
        public void TestInt32(int value, int index, int expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroLowBits(index));
        }

        [Theory]
        [InlineData((long)-1, 0, (long)-1)]
        [InlineData((long)-1, 32, unchecked((long)0xFFFFFFFF00000000))]
        [InlineData((long)-1, 64, (long)0)]
        [InlineData((long)-1, -1, (long)-1)]
        public void TestInt64(long value, int index, long expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroLowBits(index));
        }

        [Theory]
        [InlineData(ulong.MaxValue, 0, ulong.MaxValue)]
        [InlineData(ulong.MaxValue, 32, 0xFFFFFFFF00000000)]
        [InlineData(ulong.MaxValue, 64, 0ul)]
        [InlineData(ulong.MaxValue, -1, ulong.MaxValue)]
        public void TestUInt64(ulong value, int index, ulong expected)
        {
            var v = value;
            Assert.Equal(expected, v.ZeroLowBits(index));
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

        [Fact]
        public void TestInt64_MatchesUInt64()
        {
            long value = unchecked((long)0xF0F0F0F0F0F0F0F0);
            long mask = 0x0F0F0F0F0F0F0F0F;
            var unsignedValue = (ulong)value;
            var expected = (long)unsignedValue.ParallelBitExtract((ulong)mask);
            Assert.Equal(expected, value.ParallelBitExtract(mask));
        }

        [Theory]
        [InlineData(0b_1101_1010ul, 0b_0000_1111ul, 0b1010ul)]
        [InlineData(0ul, 0ul, 0ul)]
        [InlineData(ulong.MaxValue, ulong.MaxValue, ulong.MaxValue)]
        public void TestSoftwareFallbackUInt64(ulong value, ulong mask, ulong expected)
        {
            Assert.Equal(expected, BitUtilsExtensions.ParallelBitExtractSoftwareFallback(value, mask));
        }

        [Theory]
        [InlineData(0b_1101_1010u, 0b_0000_1111u, 0b1010u)]
        [InlineData(0u, 0u, 0u)]
        [InlineData(uint.MaxValue, uint.MaxValue, uint.MaxValue)]
        public void TestSoftwareFallbackUInt32(uint value, uint mask, uint expected)
        {
            Assert.Equal(expected, BitUtilsExtensions.ParallelBitExtractSoftwareFallback(value, mask));
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

        [Theory]
        [InlineData(0b1010, 0b_0000_1111, 0b_0000_1010)]
        [InlineData(0, 0, 0)]
        [InlineData(-1, -1, -1)]
        public void TestInt32(int value, int mask, int expected)
        {
            var v = value;
            Assert.Equal(expected, v.ParallelBitDeposit(mask));
        }

        [Theory]
        [InlineData(0b1010L, 0b_0000_1111L, 0b_0000_1010L)]
        [InlineData(0L, 0L, 0L)]
        [InlineData(-1L, -1L, -1L)]
        public void TestInt64(long value, long mask, long expected)
        {
            var v = value;
            Assert.Equal(expected, v.ParallelBitDeposit(mask));
        }

        [Theory]
        [InlineData(0b1010ul, 0b_0000_1111ul, 0b_0000_1010ul)]
        [InlineData(0ul, 0ul, 0ul)]
        [InlineData(ulong.MaxValue, ulong.MaxValue, ulong.MaxValue)]
        public void TestSoftwareFallbackUInt64(ulong value, ulong mask, ulong expected)
        {
            Assert.Equal(expected, BitUtilsExtensions.ParallelBitDepositSoftwareFallback(value, mask));
        }

        [Theory]
        [InlineData(0b1010u, 0b_0000_1111u, 0b_0000_1010u)]
        [InlineData(0u, 0u, 0u)]
        [InlineData(uint.MaxValue, uint.MaxValue, uint.MaxValue)]
        public void TestSoftwareFallbackUInt32(uint value, uint mask, uint expected)
        {
            Assert.Equal(expected, BitUtilsExtensions.ParallelBitDepositSoftwareFallback(value, mask));
        }
    }
}
