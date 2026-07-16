using System;
using Xunit;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class ToBitStringTest
    {
        [Theory]
        [InlineData((byte)0)]
        [InlineData((byte)1)]
        [InlineData((byte)2)]
        [InlineData(127)]
        [InlineData(255)]
        public void TestByte(byte value)
        {
            var actual = value.ToBitString();
            var expected = Convert.ToString(value, 2);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((Int16)0)]
        [InlineData((Int16)1)]
        [InlineData((Int16)(-1))]
        [InlineData(Int16.MinValue)]
        [InlineData(Int16.MaxValue)]
        public void TestInt16(Int16 value)
        {
            var actual = value.ToBitString();
            var expected = Convert.ToString((UInt32)value & 0xFFFF, 2);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((UInt16)0)]
        [InlineData((UInt16)1)]
        [InlineData((UInt16)2)]
        [InlineData(32768)]
        [InlineData(UInt16.MaxValue)]
        public void TestUInt16(UInt16 value)
        {
            var actual = value.ToBitString();
            var expected = Convert.ToString((UInt32)value, 2);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(Int32.MinValue)]
        [InlineData(Int32.MaxValue)]
        public void TestInt32(Int32 value)
        {
            var actual = value.ToBitString();
            var expected = Convert.ToString(value, 2);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0u)]
        [InlineData(1u)]
        [InlineData(2u)]
        [InlineData(0x80000000u)]
        [InlineData(UInt32.MaxValue)]
        public void TestUInt32(UInt32 value)
        {
            var actual = value.ToBitString();
            var expected = Convert.ToString((Int32)value, 2);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0L)]
        [InlineData(1L)]
        [InlineData(-1L)]
        [InlineData(Int64.MinValue)]
        [InlineData(Int64.MaxValue)]
        public void TestInt64(Int64 value)
        {
            var actual = value.ToBitString();
            var expected = Convert.ToString(value, 2);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(0ul)]
        [InlineData(1ul)]
        [InlineData(2ul)]
        [InlineData(0x8000000000000000ul)]
        [InlineData(UInt64.MaxValue)]
        public void TestUInt64(UInt64 value)
        {
            var actual = value.ToBitString();
            var expected = Convert.ToString((long)value, 2);
            Assert.Equal(expected, actual);
        }
    }
}
