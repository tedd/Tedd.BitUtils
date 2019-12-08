using System;
using System.Linq;
using Xunit;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class PopCountTest
    {
        private Random _rnd = new Random();
        private const int Iterations = 1_000_000;

        [Fact]
        public void TestByte()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (Byte)_rnd.Next();
                var c = r.PopCount();
                var e = Convert.ToString(r, 2).Count(b => b == '1');
                Assert.Equal(e, c);
            }
        }

        [Fact]
        public void TestInt16()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (Int16)_rnd.Next();
                var c = r.PopCount();
                var e = Convert.ToString(r, 2).Count(b => b == '1');
                Assert.Equal(e, c);
            }
        }

        [Fact]
        public void TestUInt16()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (UInt16)_rnd.Next();
                var c = r.PopCount();
                var e = Convert.ToString(r, 2).Count(b => b == '1');
                Assert.Equal(e, c);
            }
        }

        [Fact]
        public void TestInt32()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (Int32)_rnd.Next();
                var c = r.PopCount();
                var e = Convert.ToString(r, 2).Count(b => b == '1');
                Assert.Equal(e, c);
            }
        }

        [Fact]
        public void TestUInt32()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (UInt32)_rnd.Next();
                var c = r.PopCount();
                var e = Convert.ToString(r, 2).Count(b => b == '1');
                Assert.Equal(e, c);
            }
        }
        [Fact]
        public void TestInt64()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (Int64)_rnd.Next();
                var c = r.PopCount();
                var e = Convert.ToString(r, 2).Count(b => b == '1');
                Assert.Equal(e, c);
            }
        }

        [Fact]
        public void TestUInt64()
        {
            for (var i = 0; i < Iterations; i++)
            {
                var r = (UInt64)_rnd.Next();
                var c = r.PopCount();
                var e = Convert.ToString((Int64)r, 2).Count(b => b == '1');
                Assert.Equal(e, c);
            }
        }
    }
}