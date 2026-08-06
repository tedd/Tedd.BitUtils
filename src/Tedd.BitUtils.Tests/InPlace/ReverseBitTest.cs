using System;
using System.Linq;
using Xunit;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class ReverseBitTest
    {
        [Theory]
        [InlineData((byte)0)]
        [InlineData((byte)1)]
        [InlineData(byte.MaxValue)]
        [InlineData((byte)0xAA)]
        [InlineData((byte)0x55)]
        public void TestByte(byte r)
        {
            var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(byte) * 8, '0').Reverse().ToArray());
            r.ReverseBits();
            var actual = new string(Convert.ToString(r, 2).PadLeft(sizeof(byte) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((Int16)0)]
        [InlineData((Int16)1)]
        [InlineData((Int16)(-1))]
        [InlineData(Int16.MinValue)]
        [InlineData(Int16.MaxValue)]
        [InlineData(unchecked((Int16)0xAAAA))]
        [InlineData((Int16)0x5555)]
        public void TestInt16(Int16 r)
        {
            var expected = new string(Convert.ToString((uint)r & 0xFFFF, 2).PadLeft(sizeof(Int16) * 8, '0').Reverse().ToArray());
            r.ReverseBits();
            var actual = new string(Convert.ToString((uint)r & 0xFFFF, 2).PadLeft(sizeof(Int16) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((UInt16)0)]
        [InlineData((UInt16)1)]
        [InlineData(UInt16.MaxValue)]
        [InlineData((UInt16)0xAAAA)]
        [InlineData((UInt16)0x5555)]
        public void TestUInt16(UInt16 r)
        {
            var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(UInt16) * 8, '0').Reverse().ToArray());
            r.ReverseBits();
            var actual = new string(Convert.ToString(r, 2).PadLeft(sizeof(UInt16) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((Int32)0)]
        [InlineData((Int32)1)]
        [InlineData((Int32)(-1))]
        [InlineData(Int32.MinValue)]
        [InlineData(Int32.MaxValue)]
        [InlineData(unchecked((Int32)0xAAAAAAAA))]
        [InlineData(0x55555555)]
        public void TestInt32(Int32 r)
        {
            var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(Int32) * 8, '0').Reverse().ToArray());
            r.ReverseBits();
            var actual = new string(Convert.ToString(r, 2).PadLeft(sizeof(Int32) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((UInt32)0)]
        [InlineData((UInt32)1)]
        [InlineData(UInt32.MaxValue)]
        [InlineData(0xAAAAAAAA)]
        [InlineData(0x55555555)]
        public void TestUInt32(UInt32 r)
        {
            var expected = new string(Convert.ToString((long)r, 2).PadLeft(sizeof(UInt32) * 8, '0').Reverse().ToArray());
            r.ReverseBits();
            var actual = new string(Convert.ToString((long)r, 2).PadLeft(sizeof(UInt32) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((Int64)0)]
        [InlineData((Int64)1)]
        [InlineData((Int64)(-1))]
        [InlineData(Int64.MinValue)]
        [InlineData(Int64.MaxValue)]
        [InlineData(unchecked((Int64)0xAAAAAAAAAAAAAAAA))]
        [InlineData(0x5555555555555555)]
        public void TestInt64(Int64 r)
        {
            var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(Int64) * 8, '0').Reverse().ToArray());
            r.ReverseBits();
            var actual = new string(Convert.ToString(r, 2).PadLeft(sizeof(Int64) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((UInt64)0)]
        [InlineData((UInt64)1)]
        [InlineData(UInt64.MaxValue)]
        [InlineData(0xAAAAAAAAAAAAAAAA)]
        [InlineData(0x5555555555555555)]
        public void TestUInt64(UInt64 r)
        {
            var expected = new string(Convert.ToString((long)r, 2).PadLeft(sizeof(UInt64) * 8, '0').Reverse().ToArray());
            r.ReverseBits();
            var actual = new string(Convert.ToString((long)r, 2).PadLeft(sizeof(UInt64) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }
    }
}
