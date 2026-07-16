using System;
using System.Linq;
using Xunit;

namespace Tedd.BitUtils.Tests.Copy
{
    public class ReverseBitTest
    {
        [Theory]
        [InlineData((byte)0)]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData(127)]
        [InlineData(255)]
        public void TestByte(byte r)
        {
            var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(byte) * 8, '0').Reverse().ToArray());
            var o = r;
            var n = r.ReverseBitsCopy();
            Assert.Equal(o, r);
            var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(byte) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((Int16)0)]
        [InlineData((Int16)1)]
        [InlineData((Int16)(-1))]
        [InlineData(Int16.MinValue)]
        [InlineData(Int16.MaxValue)]
        public void TestInt16(Int16 r)
        {
            var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(Int16) * 8, '0').Reverse().ToArray());
            var o = r;
            var n = r.ReverseBitsCopy();
            Assert.Equal(o, r);
            var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(Int16) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((UInt16)0)]
        [InlineData((UInt16)1)]
        [InlineData((UInt16)2)]
        [InlineData(32768)]
        [InlineData(UInt16.MaxValue)]
        public void TestUInt16(UInt16 r)
        {
            var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(UInt16) * 8, '0').Reverse().ToArray());
            var o = r;
            var n = r.ReverseBitsCopy();
            Assert.Equal(o, r);
            var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(UInt16) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(Int32.MinValue)]
        [InlineData(Int32.MaxValue)]
        public void TestInt32(Int32 r)
        {
            var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(Int32) * 8, '0').Reverse().ToArray());
            var o = r;
            var n = r.ReverseBitsCopy();
            Assert.Equal(o, r);
            var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(Int32) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0u)]
        [InlineData(1u)]
        [InlineData(2u)]
        [InlineData(0x80000000u)]
        [InlineData(UInt32.MaxValue)]
        public void TestUInt32(UInt32 r)
        {
            var expected = new string(Convert.ToString((Int32)r, 2).PadLeft(sizeof(UInt32) * 8, '0').Reverse().ToArray());
            var o = r;
            var n = r.ReverseBitsCopy();
            Assert.Equal(o, r);
            var actual = new string(Convert.ToString((Int32)n, 2).PadLeft(sizeof(UInt32) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0L)]
        [InlineData(1L)]
        [InlineData(-1L)]
        [InlineData(Int64.MinValue)]
        [InlineData(Int64.MaxValue)]
        public void TestInt64(Int64 r)
        {
            var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(Int64) * 8, '0').Reverse().ToArray());
            var o = r;
            var n = r.ReverseBitsCopy();
            Assert.Equal(o, r);
            var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(Int64) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0ul)]
        [InlineData(1ul)]
        [InlineData(2ul)]
        [InlineData(0x8000000000000000ul)]
        [InlineData(UInt64.MaxValue)]
        public void TestUInt64(UInt64 r)
        {
            var expected = new string(Convert.ToString((long)r, 2).PadLeft(sizeof(UInt64) * 8, '0').Reverse().ToArray());
            var o = r;
            var n = r.ReverseBitsCopy();
            Assert.Equal(o, r);
            var actual = new string(Convert.ToString((long)n, 2).PadLeft(sizeof(UInt64) * 8, '0').ToArray());
            Assert.Equal(expected, actual);
        }
    }
}
