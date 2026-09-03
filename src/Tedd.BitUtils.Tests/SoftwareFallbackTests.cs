using System;
using System.Numerics;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    /// <summary>
    /// Exercises the portable software-fallback algorithms directly (accessible via InternalsVisibleTo), comparing
    /// them against System.Numerics.BitOperations as the reference oracle across a large random sample plus the
    /// classic edge cases (0, 1, all-ones, and the single set-bit boundaries).
    /// </summary>
    public class SoftwareFallbackTests
    {
        private readonly Random _rnd = new();
        private const int Iterations = 100_000;

        [Theory]
        [InlineData(0u, 0)]
        [InlineData(1u, 1)]
        [InlineData(0xFFFFFFFFu, 32)]
        [InlineData(0x0F0F0F0Fu, 16)]
        [InlineData(0x55555555u, 16)]
        public void PopCount_UInt32_KnownValues(uint input, int expected)
            => Assert.Equal(expected, BitUtilsExtensions.PopCountSoftwareFallback(input));

        [Fact]
        public void PopCount_UInt32_MatchesBitOperations()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next() ^ ((uint)_rnd.Next() << 16);
                Assert.Equal(BitOperations.PopCount(v), BitUtilsExtensions.PopCountSoftwareFallback(v));
            }
        }

        [Fact]
        public void PopCount_UInt64_MatchesBitOperations()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next() << 32) | (uint)_rnd.Next();
                Assert.Equal(BitOperations.PopCount(v), BitUtilsExtensions.PopCountSoftwareFallback(v));
            }
        }

        [Fact]
        public void LeadingZeroCount_UInt32_MatchesBitOperations()
        {
            Assert.Equal(32, BitUtilsExtensions.LeadingZeroCountSoftwareFallback(0u));
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next() ^ ((uint)_rnd.Next() << 16);
                Assert.Equal(BitOperations.LeadingZeroCount(v), BitUtilsExtensions.LeadingZeroCountSoftwareFallback(v));
            }
        }

        [Fact]
        public void LeadingZeroCount_UInt64_MatchesBitOperations()
        {
            Assert.Equal(64, BitUtilsExtensions.LeadingZeroCountSoftwareFallback(0ul));
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next() << 32) | (uint)_rnd.Next();
                Assert.Equal(BitOperations.LeadingZeroCount(v), BitUtilsExtensions.LeadingZeroCountSoftwareFallback(v));
            }
        }

        [Fact]
        public void TrailingZeroCount_UInt32_MatchesBitOperations()
        {
            Assert.Equal(32, BitUtilsExtensions.TrailingZeroCountSoftwareFallback(0u));
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next() ^ ((uint)_rnd.Next() << 16);
                Assert.Equal(BitOperations.TrailingZeroCount(v), BitUtilsExtensions.TrailingZeroCountSoftwareFallback(v));
            }
        }

        [Fact]
        public void TrailingZeroCount_UInt64_MatchesBitOperations()
        {
            Assert.Equal(64, BitUtilsExtensions.TrailingZeroCountSoftwareFallback(0ul));
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next() << 32) | (uint)_rnd.Next();
                Assert.Equal(BitOperations.TrailingZeroCount(v), BitUtilsExtensions.TrailingZeroCountSoftwareFallback(v));
            }
        }

        [Fact]
        public void Log2_UInt32_MatchesBitOperations()
        {
            Assert.Equal(0, BitUtilsExtensions.Log2SoftwareFallback(0u));
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next() ^ ((uint)_rnd.Next() << 16);
                if (v == 0) continue;
                Assert.Equal(BitOperations.Log2(v), BitUtilsExtensions.Log2SoftwareFallback(v));
            }
        }

        [Fact]
        public void Log2_UInt64_MatchesBitOperations()
        {
            Assert.Equal(0, BitUtilsExtensions.Log2SoftwareFallback(0ul));
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next() << 32) | (uint)_rnd.Next();
                if (v == 0) continue;
                Assert.Equal(BitOperations.Log2(v), BitUtilsExtensions.Log2SoftwareFallback(v));
            }
        }

        [Fact]
        public void Parity_UInt32_MatchesPopCountParity()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next() ^ ((uint)_rnd.Next() << 16);
                Assert.Equal(BitOperations.PopCount(v) & 1, BitUtilsExtensions.ParitySoftwareFallback(v));
            }
        }

        [Fact]
        public void Parity_UInt64_MatchesPopCountParity()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next() << 32) | (uint)_rnd.Next();
                Assert.Equal(BitOperations.PopCount(v) & 1, BitUtilsExtensions.ParitySoftwareFallback(v));
            }
        }

        [Fact]
        public void ReverseBits_Byte_IsInvolution()
        {
            for (var i = 0; i <= byte.MaxValue; i++)
            {
                var v = (byte)i;
                var r = BitUtilsExtensions.ReverseBitsSoftwareFallback(v);
                Assert.Equal(v, BitUtilsExtensions.ReverseBitsSoftwareFallback(r));
            }
        }

        [Theory]
        [InlineData((byte)0b0000_0001, (byte)0b1000_0000)]
        [InlineData((byte)0b1111_0000, (byte)0b0000_1111)]
        [InlineData((byte)0b1010_1010, (byte)0b0101_0101)]
        public void ReverseBits_Byte_KnownValues(byte input, byte expected)
            => Assert.Equal(expected, BitUtilsExtensions.ReverseBitsSoftwareFallback(input));

        [Fact]
        public void ReverseBits_UInt32_MatchesBitByBitReference()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next() ^ ((uint)_rnd.Next() << 16);
                uint expected = 0;
                for (var bit = 0; bit < 32; bit++)
                    if ((v & (1u << bit)) != 0)
                        expected |= 1u << (31 - bit);

                Assert.Equal(expected, BitUtilsExtensions.ReverseBitsSoftwareFallback(v));
            }
        }

        [Fact]
        public void ReverseBits_UInt64_IsInvolution()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = ((ulong)(uint)_rnd.Next() << 32) | (uint)_rnd.Next();
                var r = BitUtilsExtensions.ReverseBitsSoftwareFallback(v);
                Assert.Equal(v, BitUtilsExtensions.ReverseBitsSoftwareFallback(r));
            }
        }

        [Fact]
        public void ReverseBits_UInt16_IsInvolution()
        {
            for (var i = 0; i <= ushort.MaxValue; i += 7) // step to keep runtime reasonable while still covering the full range
            {
                var v = (ushort)i;
                var r = BitUtilsExtensions.ReverseBitsSoftwareFallback(v);
                Assert.Equal(v, BitUtilsExtensions.ReverseBitsSoftwareFallback(r));
            }
        }

        [Theory]
        [InlineData(0u, 0u)]
        [InlineData(1u, 1u)]
        [InlineData(2u, 2u)]
        [InlineData(3u, 4u)]
        [InlineData(5u, 8u)]
        [InlineData(0x80000000u, 0x80000000u)]
        [InlineData(0x80000001u, 0u)] // does not fit in 32 bits -> wraps to 0, matching BitOperations.RoundUpToPowerOf2
        public void RoundUpToPowerOf2_UInt32_KnownValues(uint input, uint expected)
            => Assert.Equal(expected, BitUtilsExtensions.RoundUpToPowerOf2SoftwareFallback(input));

        [Fact]
        public void RoundUpToPowerOf2_UInt32_MatchesBitOperations()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = (uint)_rnd.Next(1, int.MaxValue); // BitOperations.RoundUpToPowerOf2 only guarantees a defined, in-range result for values that fit
                if (v > (1u << 31)) continue;
                Assert.Equal(BitOperations.RoundUpToPowerOf2(v), BitUtilsExtensions.RoundUpToPowerOf2SoftwareFallback(v));
            }
        }

        [Fact]
        public void RoundUpToPowerOf2_UInt64_MatchesBitOperations()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var v = (ulong)_rnd.Next(1, int.MaxValue);
                Assert.Equal(BitOperations.RoundUpToPowerOf2(v), BitUtilsExtensions.RoundUpToPowerOf2SoftwareFallback(v));
            }
        }

        [Fact]
        public void ParallelBitExtract_UInt32_IsInverseOfParallelBitDeposit()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var mask = (uint)_rnd.Next() ^ ((uint)_rnd.Next() << 16);
                var value = (uint)_rnd.Next() ^ ((uint)_rnd.Next() << 16);
                var deposited = BitUtilsExtensions.ParallelBitDepositSoftwareFallback(value, mask);
                var extracted = BitUtilsExtensions.ParallelBitExtractSoftwareFallback(deposited, mask);
                var expectedExtracted = value & (uint)(BitOperations.PopCount(mask) == 32 ? uint.MaxValue : (1u << BitOperations.PopCount(mask)) - 1);

                Assert.Equal(expectedExtracted, extracted);
                Assert.Equal(0u, deposited & ~mask); // every deposited bit landed inside the mask
            }
        }

        [Theory]
        [InlineData(0b_1101_1010u, 0b_0000_1111u, 0b1010u)] // contiguous low mask -> plain AND
        [InlineData(0b_1101_1010u, 0b_1111_0000u, 0b1101u)] // contiguous high mask -> AND then shift down
        public void ParallelBitExtract_UInt32_KnownValues(uint value, uint mask, uint expected)
            => Assert.Equal(expected, BitUtilsExtensions.ParallelBitExtractSoftwareFallback(value, mask));

        [Theory]
        [InlineData(0b1010u, 0b_0000_1111u, 0b_0000_1010u)] // contiguous low mask -> plain AND
        [InlineData(0b1010u, 0b_1111_0000u, 0b_1010_0000u)] // contiguous high mask -> shift up then AND
        public void ParallelBitDeposit_UInt32_KnownValues(uint value, uint mask, uint expected)
            => Assert.Equal(expected, BitUtilsExtensions.ParallelBitDepositSoftwareFallback(value, mask));
    }
}
