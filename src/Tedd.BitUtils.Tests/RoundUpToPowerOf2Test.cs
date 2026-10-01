using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class RoundUpToPowerOf2Test
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

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
        [Theory]
        [InlineData((sbyte)5, (sbyte)8)]
        [InlineData((sbyte)16, (sbyte)16)]
        [InlineData((sbyte)0, (sbyte)0)]
        [InlineData((sbyte)-5, (sbyte)0)] // Negative treats as unsigned (fb for sbyte -> 0, or positive wrap? actually -5 is 0xFB, which is 251, so next is 256 which overflows -> 0)
        public void TestSByte_MatchesExpected(sbyte v, sbyte expected)
        {
            var original = v;
            var r = v.RoundUpToPowerOf2Copy();
            Assert.Equal(original, v);
            Assert.Equal(expected, r);
        }

        [Theory]
        [InlineData((byte)5, (byte)8)]
        [InlineData((byte)16, (byte)16)]
        [InlineData((byte)0, (byte)0)]
        [InlineData((byte)255, (byte)0)]
        public void TestByte_MatchesExpected(byte v, byte expected)
        {
            var original = v;
            var r = v.RoundUpToPowerOf2Copy();
            Assert.Equal(original, v);
            Assert.Equal(expected, r);
        }

        [Theory]
        [InlineData((short)100, (short)128)]
        [InlineData((short)0, (short)0)]
        public void TestInt16_MatchesExpected(short v, short expected)
        {
            var original = v;
            var r = v.RoundUpToPowerOf2Copy();
            Assert.Equal(original, v);
            Assert.Equal(expected, r);
        }

        [Theory]
        [InlineData((ushort)100, (ushort)128)]
        [InlineData((ushort)0, (ushort)0)]
        public void TestUInt16_MatchesExpected(ushort v, ushort expected)
        {
            var original = v;
            var r = v.RoundUpToPowerOf2Copy();
            Assert.Equal(original, v);
            Assert.Equal(expected, r);
        }

        [Theory]
        [InlineData((int)100, (int)128)]
        [InlineData((int)0, (int)0)]
        public void TestInt32_MatchesExpected(int v, int expected)
        {
            var original = v;
            var r = v.RoundUpToPowerOf2Copy();
            Assert.Equal(original, v);
            Assert.Equal(expected, r);
        }

        [Theory]
        [InlineData((uint)100, (uint)128)]
        [InlineData((uint)0, (uint)0)]
        public void TestUInt32_MatchesExpected(uint v, uint expected)
        {
            var original = v;
            var r = v.RoundUpToPowerOf2Copy();
            Assert.Equal(original, v);
            Assert.Equal(expected, r);
        }

        [Theory]
        [InlineData((long)100, (long)128)]
        [InlineData((long)0, (long)0)]
        public void TestInt64_MatchesExpected(long v, long expected)
        {
            var original = v;
            var r = v.RoundUpToPowerOf2Copy();
            Assert.Equal(original, v);
            Assert.Equal(expected, r);
        }

        [Theory]
        [InlineData((ulong)100, (ulong)128)]
        [InlineData((ulong)0, (ulong)0)]
        public void TestUInt64_MatchesExpected(ulong v, ulong expected)
        {
            var original = v;
            var r = v.RoundUpToPowerOf2Copy();
            Assert.Equal(original, v);
            Assert.Equal(expected, r);
        }
    }
}