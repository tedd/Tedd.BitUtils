using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class RoundUpToPowerOf2Test
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 4)]
        [InlineData(5, 8)]
        [InlineData(128, 128)]
        [InlineData(129, 0)]
        public void TestByte_SpecificValues(byte value, byte expected)
        {
            value.RoundUpToPowerOf2();
            Assert.Equal(expected, value);
        }

        [Fact]
        public void TestByte_KnownValues()
        {
            byte v;
            v = 0; v.RoundUpToPowerOf2(); Assert.Equal((byte)0, v);
            v = 1; v.RoundUpToPowerOf2(); Assert.Equal((byte)1, v);
            v = 2; v.RoundUpToPowerOf2(); Assert.Equal((byte)2, v);
            v = 3; v.RoundUpToPowerOf2(); Assert.Equal((byte)4, v);
            v = 5; v.RoundUpToPowerOf2(); Assert.Equal((byte)8, v);
            v = 0x80; v.RoundUpToPowerOf2(); Assert.Equal((byte)0x80, v);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 4)]
        [InlineData(5, 8)]
        [InlineData(64, 64)]
        [InlineData(65, -128)]
        [InlineData(-1, 0)]
        [InlineData(-128, -128)]
        public void TestSByte_SpecificValues(sbyte value, sbyte expected)
        {
            value.RoundUpToPowerOf2();
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 4)]
        [InlineData(5, 8)]
        [InlineData(16384, 16384)]
        [InlineData(16385, -32768)]
        [InlineData(-1, 0)]
        [InlineData(-32768, -32768)]
        public void TestInt16_SpecificValues(short value, short expected)
        {
            value.RoundUpToPowerOf2();
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 4)]
        [InlineData(5, 8)]
        [InlineData(32768, 32768)]
        [InlineData(32769, 0)]
        public void TestUInt16_SpecificValues(ushort value, ushort expected)
        {
            value.RoundUpToPowerOf2();
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(0L, 0L)]
        [InlineData(1L, 1L)]
        [InlineData(2L, 2L)]
        [InlineData(3L, 4L)]
        [InlineData(5L, 8L)]
        [InlineData(1073741824L, 1073741824L)]
        [InlineData(1073741825L, 2147483648L)]
        [InlineData(4611686018427387904L, 4611686018427387904L)]
        [InlineData(4611686018427387905L, -9223372036854775808L)]
        [InlineData(-1L, 0L)]
        [InlineData(-9223372036854775808L, -9223372036854775808L)]
        public void TestInt64_SpecificValues(long value, long expected)
        {
            value.RoundUpToPowerOf2();
            Assert.Equal(expected, value);
        }

        [Fact]
        public void TestInt32_Random_AlwaysAPowerOfTwoAtLeastTheInput()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var original = _rnd.Next(1, 1 << 30); // stay well within range that always fits in an Int32
                var v = original;
                v.RoundUpToPowerOf2();

                Assert.True(v.IsPowerOfTwo(), $"{v} (from {original}) should be a power of two");
                Assert.True(v >= original, $"{v} should be >= {original}");
                Assert.True(v < original * 2, $"{v} should be < {original * 2}");
            }
        }

        [Fact]
        public void TestUInt64_Random_AlwaysAPowerOfTwoAtLeastTheInput()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var original = (ulong)_rnd.Next(1, int.MaxValue);
                var v = original;
                v.RoundUpToPowerOf2();

                Assert.True(v.IsPowerOfTwo());
                Assert.True(v >= original);
                Assert.True(v < original * 2);
            }
        }

        [Fact]
        public void TestExactPowersOfTwoAreUnchanged()
        {
            for (var n = 0; n < 31; n++)
            {
                var v = 1 << n;
                v.RoundUpToPowerOf2();
                Assert.Equal(1 << n, v);
            }
        }
    }

    public class RoundUpToPowerOf2CopyTest
    {
        [Fact]
        public void TestByte_DoesNotModifyOriginal()
        {
            byte v = 5;
            var r = v.RoundUpToPowerOf2Copy();
            Assert.Equal((byte)5, v);
            Assert.Equal((byte)8, r);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(100, 128)]
        [InlineData(255, 0)]
        public void TestByte_SpecificValuesCopy(byte value, byte expected)
        {
            Assert.Equal(expected, value.RoundUpToPowerOf2Copy());
        }

        [Theory]
        [InlineData((sbyte)0, (sbyte)0)]
        [InlineData((sbyte)100, (sbyte)-128)] // 128 overflows sbyte to -128
        [InlineData((sbyte)-100, (sbyte)0)] // -100 as unsigned byte is 156, round up to 256 which is 0 for byte
        public void TestSByte_SpecificValuesCopy(sbyte value, sbyte expected)
        {
            Assert.Equal(expected, value.RoundUpToPowerOf2Copy());
        }

        [Theory]
        [InlineData((short)0, (short)0)]
        [InlineData((short)100, (short)128)]
        [InlineData((short)-100, (short)0)] // -100 as ushort is 65436, round up to 65536 which is 0 for ushort
        public void TestInt16_SpecificValuesCopy(short value, short expected)
        {
            Assert.Equal(expected, value.RoundUpToPowerOf2Copy());
        }

        [Theory]
        [InlineData((ushort)0, (ushort)0)]
        [InlineData((ushort)100, (ushort)128)]
        [InlineData((ushort)65535, (ushort)0)]
        public void TestUInt16_SpecificValuesCopy(ushort value, ushort expected)
        {
            Assert.Equal(expected, value.RoundUpToPowerOf2Copy());
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(100, 128)]
        [InlineData(-100, 0)] // -100 as uint is 4294967196, round up to 2^32 which is 0
        public void TestInt32_SpecificValuesCopy(int value, int expected)
        {
            Assert.Equal(expected, value.RoundUpToPowerOf2Copy());
        }

        [Theory]
        [InlineData(0u, 0u)]
        [InlineData(100u, 128u)]
        [InlineData(4294967295u, 0u)]
        public void TestUInt32_SpecificValuesCopy(uint value, uint expected)
        {
            Assert.Equal(expected, value.RoundUpToPowerOf2Copy());
        }

        [Theory]
        [InlineData(0L, 0L)]
        [InlineData(100L, 128L)]
        [InlineData(-100L, 0L)] // -100 as ulong is 18446744073709551516, round up to 2^64 which is 0
        public void TestInt64_SpecificValuesCopy(long value, long expected)
        {
            Assert.Equal(expected, value.RoundUpToPowerOf2Copy());
        }

        [Theory]
        [InlineData(0ul, 0ul)]
        [InlineData(100ul, 128ul)]
        [InlineData(18446744073709551615ul, 0ul)]
        public void TestUInt64_SpecificValuesCopy(ulong value, ulong expected)
        {
            Assert.Equal(expected, value.RoundUpToPowerOf2Copy());
        }
    }
}
