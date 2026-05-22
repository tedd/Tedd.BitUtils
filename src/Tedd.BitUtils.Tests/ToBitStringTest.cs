using System;
using Xunit;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class ToBitStringTest
    {
        [Fact]
        public void TestByte()
        {
            var values = new Byte[] { 0, 1, 2, 127, Byte.MaxValue };

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var actual = value.ToBitString();
                var expected = Convert.ToString(value, 2);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestInt16()
        {
            var values = new Int16[] { 0, 1, -1, Int16.MinValue, Int16.MaxValue };

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var actual = value.ToBitString();
                var expected = Convert.ToString((UInt32)value & 0xFFFF, 2);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestUInt16()
        {
            var values = new UInt16[] { 0, 1, 2, 32768, UInt16.MaxValue };

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var actual = value.ToBitString();
                var expected = Convert.ToString((UInt32)value, 2);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestInt32()
        {
            var values = new Int32[] { 0, 1, -1, Int32.MinValue, Int32.MaxValue };

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var actual = value.ToBitString();
                var expected = Convert.ToString(value, 2);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestUInt32()
        {
            var values = new UInt32[] { 0, 1, 2, 0x80000000, UInt32.MaxValue };

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var actual = value.ToBitString();
                var expected = Convert.ToString((Int32)value, 2);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestInt64()
        {
            var values = new Int64[] { 0, 1, -1, Int64.MinValue, Int64.MaxValue };

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var actual = value.ToBitString();
                var expected = Convert.ToString(value, 2);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestUInt64()
        {
            var values = new UInt64[] { 0, 1, 2, 0x8000000000000000, UInt64.MaxValue };

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var actual = value.ToBitString();
                var expected = Convert.ToString((Int64)value, 2);
                Assert.Equal(expected, actual);
            }
        }
    }
}
