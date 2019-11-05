using System;
using System.Collections.Generic;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class BitRotateTest
    {

        [Fact]
        public void TestInt32()
        {
            {
                Int32 v = 1;
                Int32 a = 1 << 1;
                v.Rol();
                Assert.Equal(a, v);
            }
            {
                Int32 v = 1;
                Int32 a = (Int32)1 << sizeof(Int32) * 8 - 1;
                v.Ror();
                Assert.Equal(a, v);
            }
            {
                Int32 v = 1;
                Int32 a = 1 << 2;
                v.Rol(2);
                Assert.Equal(a, v);
            }
            {
                Int32 v = 1;
                Int32 a = (Int32)1 << sizeof(Int32) * 8 - 2;
                v.Ror(2);
                Assert.Equal(a, v);
            }
        }

        [Fact]
        public void TestUInt32()
        {
            {
                UInt32 v = 1;
                UInt32 a = 1 << 1;
                v.Rol();
                Assert.Equal(a, v);
            }
            {
                UInt32 v = 1;
                UInt32 a = (UInt32)1 << sizeof(UInt32) * 8 - 1;
                v.Ror();
                Assert.Equal(a, v);
            }
            {
                UInt32 v = 1;
                UInt32 a = 1 << 2;
                v.Rol(2);
                Assert.Equal(a, v);
            }
            {
                UInt32 v = 1;
                UInt32 a = (UInt32)1 << sizeof(UInt32) * 8 - 2;
                v.Ror(2);
                Assert.Equal(a, v);
            }
        }

        [Fact]
        public void TestInt64()
        {
            {
                Int64 v = 1;
                Int64 a = 1 << 1;
                v.Rol();
                Assert.Equal(a, v);
            }
            {
                Int64 v = 1;
                Int64 a = (Int64)1 << (sizeof(Int64) * 8) - 1;
                v.Ror();
                Assert.Equal(a, v);
            }
            {
                Int64 v = 1;
                Int64 a = 1 << 2;
                v.Rol(2);
                Assert.Equal(a, v);
            }
            {
                Int64 v = 1;
                Int64 a = (Int64)1 << sizeof(Int64) * 8 - 2;
                v.Ror(2);
                Assert.Equal(a, v);
            }
        }    
        
        [Fact]
        public void TestUInt64()
        {
            {
                UInt64 v = 1;
                UInt64 a = 1 << 1;
                v.Rol();
                Assert.Equal(a, v);
            }
            {
                UInt64 v = 1;
                UInt64 a = (UInt64)1 << sizeof(UInt64) * 8 - 1;
                v.Ror();
                Assert.Equal(a, v);
            }
            {
                UInt64 v = 1;
                UInt64 a = 1 << 2;
                v.Rol(2);
                Assert.Equal(a, v);
            }
            {
                UInt64 v = 1;
                UInt64 a = (UInt64)1 << sizeof(UInt64) * 8 - 2;
                v.Ror(2);
                Assert.Equal(a, v);
            }
        }
    }
}
