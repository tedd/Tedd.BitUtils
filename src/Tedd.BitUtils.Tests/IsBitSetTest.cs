using System;
using System.Collections.Generic;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class IsBitSetTest
    {
        [Fact]
        public void TestByte()
        {
            for (var n = 0; n < sizeof(byte) * 8; n++)
            {
                byte i = (byte)(1 << n);
                Assert.True(i.IsBitSet(n), n.ToString());
            }
        }
        [Fact]
        public void TestInt16()
        {
            for (var n = 0; n < sizeof(Int32) * 8; n++)
            {
                Int32 i = (Int32)1 << n;
                Assert.True(i.IsBitSet(n), n.ToString());
            }
        }

        [Fact]
        public void TestUInt16()
        {
            for (var n = 0; n < sizeof(UInt16) * 8; n++)
            {
                UInt16 i = unchecked((UInt16)((Int32)1 << n));
                Assert.True(i.IsBitSet(n), n.ToString());
            }
        }

        [Fact]
        public void TestInt32()
        {
            for (var n = 0; n < sizeof(Int32) * 8; n++)
            {
                Int32 i = (Int32)1 << n;
                Assert.True(i.IsBitSet(n), n.ToString());
            }
        }

        [Fact]
        public void TestUInt32()
        {
            for (var n = 0; n < sizeof(UInt32) * 8; n++)
            {
                UInt32 i = (UInt32)1 << n;
                Assert.True(i.IsBitSet(n), n.ToString());
            }
        }
        [Fact]
        public void TestInt64()
        {
            for (var n = 0; n < sizeof(Int64) * 8; n++)
            {
                Int64 i = (Int64)1 << n;
                Assert.True(i.IsBitSet(n), n.ToString());
            }
        }

        [Fact]
        public void TestUInt64()
        {
            for (var n = 0; n < sizeof(UInt64) * 8; n++)
            {
                UInt64 i = (UInt64)1 << n;
                Assert.True(i.IsBitSet(n), n.ToString());
            }
        }
    }
}
