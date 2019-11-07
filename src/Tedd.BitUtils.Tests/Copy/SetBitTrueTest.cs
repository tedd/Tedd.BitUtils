using System;
using Xunit;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class SetBitTrueCopyTest
    {
        [Fact]
        public void TestByteCopy()
        {
            for (var n = 0; n < sizeof(byte) * 8; n++)
            {
                byte a = (byte)(1 << n);
                byte i = unchecked((byte)0);
                
                Assert.Equal(a, i.SetBitCopy(n, true));
            }
        }
        [Fact]
        public void TestInt16Copy()
        {
            for (var n = 0; n < sizeof(Int16) * 8; n++)
            {
                Int16 a = (Int16)((Int16)1 << n);
                Int16 i = unchecked((Int16)0);
                
                Assert.Equal(a, i.SetBitCopy(n, true));
            }
        }
        [Fact]
        public void TestUInt16Copy()
        {
            for (var n = 0; n < sizeof(UInt16) * 8; n++)
            {
                UInt16 a = (UInt16)(1 << n);
                UInt16 i = unchecked((UInt16)0);
                
                Assert.Equal(a, i.SetBitCopy(n, true));
            }
        }
        [Fact]
        public void TestInt32Copy()
        {
            for (var n = 0; n < sizeof(Int32) * 8; n++)
            {
                Int32 a = (Int32)((Int32)1 << n);
                Int32 i = unchecked((Int32)0);
                
                Assert.Equal(a, i.SetBitCopy(n, true));
            }
        }
        [Fact]
        public void TestUInt32Copy()
        {
            for (var n = 0; n < sizeof(UInt32) * 8; n++)
            {
                UInt32 a = (UInt32)(1 << n);
                UInt32 i = unchecked((UInt32)0);
                
                Assert.Equal(a, i.SetBitCopy(n, true));
            }
        }
        [Fact]
        public void TestInt64Copy()
        {
            for (var n = 0; n < sizeof(Int64) * 8; n++)
            {
                Int64 a = (1L << n);
                Int64 i = 0L;
                
                Assert.Equal(a, i.SetBitCopy(n, true));
            }
        }
        [Fact]
        public void TestUInt64Copy()
        {
            for (var n = 0; n < sizeof(UInt64) * 8; n++)
            {
                UInt64 a = (UInt64)(1L << n);
                UInt64 i = unchecked((UInt64)0);
                
                Assert.Equal(a, i.SetBitCopy(n, true));
            }
        }
    }
}