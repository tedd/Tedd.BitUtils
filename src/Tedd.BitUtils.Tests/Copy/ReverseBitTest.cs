using System;
using System.Linq;
using Xunit;

namespace Tedd.BitUtils.Tests.Copy
{
    public class ReverseBitTest
    {
        private Random rnd = new Random();
        [Fact]
        public void TestByte()
        {
            for (var i = 0; i < 100; i++)
            {
                var r = (byte)rnd.Next();
                var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(byte) * 8, '0').Reverse().ToArray());
                var o = r;
                var n = r.ReverseBitsCopy();
                Assert.Equal(o,r);
                var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(byte) * 8, '0').ToArray());
                Assert.Equal(expected, actual);
            }
        }
        [Fact]
        public void TestInt16()
        {
            for (var i = 0; i < 100; i++)
            {
                var r = (Int16)rnd.Next();
                var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(Int16) * 8, '0').Reverse().ToArray());
                var o = r;
                var n = r.ReverseBitsCopy();
                Assert.Equal(o, r);
                var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(Int16) * 8, '0').ToArray());
                Assert.Equal(expected, actual);
            }
        }
        [Fact]
        public void TestUInt16()
        {
            for (var i = 0; i < 100; i++)
            {
                var r = (UInt16)rnd.Next();
                var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(UInt16) * 8, '0').Reverse().ToArray());
                var o = r;
                var n = r.ReverseBitsCopy();
                Assert.Equal(o, r);
                var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(UInt16) * 8, '0').ToArray());
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestInt32()
        {
            for (var i = 0; i < 100; i++)
            {
                var r = (Int32)rnd.Next();
                var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(Int32) * 8, '0').Reverse().ToArray());
                var o = r;
                var n = r.ReverseBitsCopy();
                Assert.Equal(o, r);
                var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(Int32) * 8, '0').ToArray());
                Assert.Equal(expected, actual);
            }
        }
        [Fact]
        public void TestUInt32()
        {
            for (var i = 0; i < 100; i++)
            {
                var r = (UInt32)rnd.Next();
                var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(UInt32) * 8, '0').Reverse().ToArray());
                var o = r;
                var n = r.ReverseBitsCopy();
                Assert.Equal(o, r);
                var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(UInt32) * 8, '0').ToArray());
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestInt64()
        {
            for (var i = 0; i < 100; i++)
            {
                var r = (Int64)rnd.Next() | (Int64)rnd.Next()<<32;
                var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(Int64) * 8, '0').Reverse().ToArray());
                var o = r;
                var n = r.ReverseBitsCopy();
                Assert.Equal(o, r);
                var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(Int64) * 8, '0').ToArray());
                Assert.Equal(expected, actual);
            }
        }
        [Fact]
        public void TestUInt64()
        {
            for (var i = 0; i < 100; i++)
            {
                var r = (Int64)rnd.Next() | (Int64)rnd.Next() << 32;
                var expected = new string(Convert.ToString(r, 2).PadLeft(sizeof(UInt64) * 8, '0').Reverse().ToArray());
                var o = r;
                var n = r.ReverseBitsCopy();
                Assert.Equal(o, r);
                var actual = new string(Convert.ToString(n, 2).PadLeft(sizeof(UInt64) * 8, '0').ToArray());
                Assert.Equal(expected, actual);
            }
        }
    }
}