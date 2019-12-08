using System;
using Xunit;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class LeadingZeroCountTest
    {
        private Random _rnd = new Random();
        private const int Iterations = 1_000_000;

        [Fact]
        public void TestByte()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (Byte)_rnd.Next();
                var c = r.LeadingZeroCount();
                var str = Convert.ToString(r, 2);
                var len = str == "0" ? 0 : str.Length;
                var e = (sizeof(Byte) * 8) - len;

                Assert.Equal(e, c);
            }
        }
        [Fact]
        public void TestInt16()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (Int16)_rnd.Next();
                var c = r.LeadingZeroCount();
                var str = Convert.ToString(r, 2);
                var len = str == "0" ? 0 : str.Length;
                var e = (sizeof(Int16) * 8) - len;
                //if (e != c)
                //    throw new Exception("Mismatch");
                Assert.Equal(e, c);
            }
        }
        [Fact]
        public void TestUInt16()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (UInt16)_rnd.Next();
                var c = r.LeadingZeroCount();
                var str = Convert.ToString(r, 2);
                var len = str == "0" ? 0 : str.Length;
                var e = (sizeof(UInt16) * 8) - len;
                Assert.Equal(e, c);
            }
        }
        [Fact]
        public void TestInt32()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (Int32)_rnd.Next();
                var c = r.LeadingZeroCount();
                var str = Convert.ToString(r, 2);
                var len = str == "0" ? 0 : str.Length;
                var e = (sizeof(Int32) * 8) - len;
                Assert.Equal(e, c);
            }
        }

        [Fact]
        public void TestUInt32()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (UInt32)_rnd.Next();
                var c = r.LeadingZeroCount();
                var str = Convert.ToString(r, 2);
                var len = str == "0" ? 0 : str.Length;
                var e = (sizeof(UInt32) * 8) - len;
                Assert.Equal(e, c);
            }
        }

        [Fact]
        public void TestInt64()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (Int64)_rnd.Next() | ((Int64)_rnd.Next()) << 32;
                var c = r.LeadingZeroCount();
                var str = Convert.ToString(r, 2);
                var len = str == "0" ? 0 : str.Length;

                var e = (sizeof(Int64) * 8) - len;
                Assert.Equal(e, c);
            }
        }
        [Fact]
        public void TestUInt64()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (UInt64)_rnd.Next() | ((UInt64)_rnd.Next()) << 32;
                var c = r.LeadingZeroCount();
                var str = Convert.ToString((Int64)r, 2);
                var len = str == "0" ? 0 : str.Length;

                var e = (sizeof(UInt64) * 8) - len;
                Assert.Equal(e, c);
            }
        }
    }
}