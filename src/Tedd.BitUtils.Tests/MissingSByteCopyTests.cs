using System;
using Xunit;
using Tedd;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class MissingSByteCopyTests
    {
        [Theory]
        [InlineData(0, true)]
        [InlineData(3, true)]
        [InlineData(7, true)]
        [InlineData(0, false)]
        [InlineData(3, false)]
        [InlineData(7, false)]
        public void TestSetBitCopySByte(int bitPos, bool state)
        {
            sbyte a = state ? unchecked((sbyte)(1 << bitPos)) : (sbyte)0;
            sbyte i = state ? (sbyte)0 : unchecked((sbyte)(1 << bitPos));

            Assert.Equal(a, i.SetBitCopy(bitPos, state));

            if (state)
            {
                Assert.Equal(a, i.SetBit1Copy(bitPos));
            }
            else
            {
                Assert.Equal(a, i.SetBit0Copy(bitPos));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(7)]
        public void TestToggleBitCopySByte(int bitPos)
        {
            sbyte i = 0;
            sbyte a = unchecked((sbyte)(1 << bitPos));
            Assert.Equal(a, i.ToggleBitCopy(bitPos));
        }

        [Theory]
        [InlineData(3, 4)]
        [InlineData(4, 4)]
        [InlineData(31, 32)]
        [InlineData(64, 64)]
        public void TestRoundUpToPowerOf2Copy(int input, int expected)
        {
            sbyte v = (sbyte)input;
            Assert.Equal((sbyte)expected, v.RoundUpToPowerOf2Copy());
            short s = (short)input;
            Assert.Equal((short)expected, s.RoundUpToPowerOf2Copy());
            ushort us = (ushort)input;
            Assert.Equal((ushort)expected, us.RoundUpToPowerOf2Copy());
            int i = input;
            Assert.Equal(expected, i.RoundUpToPowerOf2Copy());
            long l = input;
            Assert.Equal(expected, l.RoundUpToPowerOf2Copy());
            ulong ul = (ulong)input;
            Assert.Equal((ulong)expected, ul.RoundUpToPowerOf2Copy());
        }

        [Theory]
        [InlineData(unchecked((sbyte)0b10000001), unchecked((sbyte)0b10000001))]
        [InlineData(0b01000010, 0b01000010)]
        [InlineData(0b00000011, unchecked((sbyte)0b11000000))]
        public void TestReverseBitsCopySByte(sbyte input, sbyte expected)
        {
            Assert.Equal(expected, input.ReverseBitsCopy());
        }
    }
}
