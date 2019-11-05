using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class SetBit0Test
    {
        [Fact]
        public void TestByte()
        {
            for (var n = 0; n < sizeof(byte) * 8; n++)
            {
                byte a = (byte)~(1 << n);
                byte i = byte.MaxValue;
                i.SetBit0(n);
                Assert.Equal(a, i);
            }
        }
        [Fact]
        public void TestInt16()
        {
            for (var n = 0; n < sizeof(Int16) * 8; n++)
            {
                Int16 a = (Int16)~((Int16)1 << n);
                Int16 i = Int16.MinValue;
                i.SetBit0(n);
                Assert.Equal(a, i);
            }
        }
        [Fact]
        public void TestUInt16()
        {
            for (var n = 0; n < sizeof(UInt16) * 8; n++)
            {
                UInt16 a = (UInt16)~(1 << n);
                UInt16 i = UInt16.MaxValue;
                i.SetBit0(n);
                Assert.Equal(a, i);
            }
        }
    }
}