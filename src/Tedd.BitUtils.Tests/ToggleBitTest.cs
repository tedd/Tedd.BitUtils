using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class ToggleBitTest
    {
        [Fact]
        public void TestByte()
        {
            for (var n = 0; n < sizeof(byte) * 8; n++)
            {
                byte i = 0;
                i.ToggleBit(n);
                Assert.Equal((byte)(1 << n), i);
                i.ToggleBit(n);
                Assert.Equal((byte)0, i);
            }
        }

        [Fact]
        public void TestSByte()
        {
            for (var n = 0; n < sizeof(sbyte) * 8; n++)
            {
                sbyte i = 0;
                i.ToggleBit(n);
                Assert.Equal(unchecked((sbyte)(1 << n)), i);
                i.ToggleBit(n);
                Assert.Equal((sbyte)0, i);
            }
        }

        [Fact]
        public void TestInt16()
        {
            for (var n = 0; n < sizeof(short) * 8; n++)
            {
                short i = 0;
                i.ToggleBit(n);
                Assert.Equal(unchecked((short)(1 << n)), i);
                i.ToggleBit(n);
                Assert.Equal((short)0, i);
            }
        }

        [Fact]
        public void TestUInt16()
        {
            for (var n = 0; n < sizeof(ushort) * 8; n++)
            {
                ushort i = 0;
                i.ToggleBit(n);
                Assert.Equal((ushort)(1 << n), i);
                i.ToggleBit(n);
                Assert.Equal((ushort)0, i);
            }
        }

        [Fact]
        public void TestInt32()
        {
            for (var n = 0; n < sizeof(int) * 8; n++)
            {
                int i = 0;
                i.ToggleBit(n);
                Assert.Equal(1 << n, i);
                i.ToggleBit(n);
                Assert.Equal(0, i);
            }
        }

        [Fact]
        public void TestUInt32()
        {
            for (var n = 0; n < sizeof(uint) * 8; n++)
            {
                uint i = 0;
                i.ToggleBit(n);
                Assert.Equal(1u << n, i);
                i.ToggleBit(n);
                Assert.Equal(0u, i);
            }
        }

        [Fact]
        public void TestInt64()
        {
            for (var n = 0; n < sizeof(long) * 8; n++)
            {
                long i = 0;
                i.ToggleBit(n);
                Assert.Equal(1L << n, i);
                i.ToggleBit(n);
                Assert.Equal(0L, i);
            }
        }

        [Fact]
        public void TestUInt64()
        {
            for (var n = 0; n < sizeof(ulong) * 8; n++)
            {
                ulong i = 0;
                i.ToggleBit(n);
                Assert.Equal(1ul << n, i);
                i.ToggleBit(n);
                Assert.Equal(0ul, i);
            }
        }
    }

    public class ToggleBitCopyTest
    {
        [Fact]
        public void TestByte()
        {
            for (var n = 0; n < sizeof(byte) * 8; n++)
            {
                byte i = 0;
                var r = i.ToggleBitCopy(n);
                Assert.Equal((byte)0, i); // original untouched
                Assert.Equal((byte)(1 << n), r);
            }
        }

        [Fact]
        public void TestInt32()
        {
            for (var n = 0; n < sizeof(int) * 8; n++)
            {
                int i = 0;
                var r = i.ToggleBitCopy(n);
                Assert.Equal(0, i);
                Assert.Equal(1 << n, r);
            }
        }

        [Fact]
        public void TestUInt64()
        {
            for (var n = 0; n < sizeof(ulong) * 8; n++)
            {
                ulong i = 0;
                var r = i.ToggleBitCopy(n);
                Assert.Equal(0ul, i);
                Assert.Equal(1ul << n, r);
            }
        }
    }
}
