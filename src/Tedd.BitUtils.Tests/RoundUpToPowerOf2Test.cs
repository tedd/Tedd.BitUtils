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
        [Fact]
        public void TestByte_DoesNotModifyOriginal()
        {
            byte v = 5;
            var r = v.RoundUpToPowerOf2Copy();
            Assert.Equal((byte)5, v);
            Assert.Equal((byte)8, r);
        }

        [Fact]
        public void TestUInt32_MatchesInPlace()
        {
            uint v = 100;
            var copy = v.RoundUpToPowerOf2Copy();
            v.RoundUpToPowerOf2();
            Assert.Equal(v, copy);
        }
    }
}
