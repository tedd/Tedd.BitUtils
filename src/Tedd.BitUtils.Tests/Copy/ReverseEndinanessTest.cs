using System;
using System.Linq;
using Xunit;

namespace Tedd.BitUtils.Tests.Copy
{
    public class ReverseEndinanessTest
    {
        private Random rnd = new Random();
        private int testCount = 10_000;

        [Fact]
        public void TestSByte()
        {
            for (var c = 0; c < testCount; c++)
            {
                var n = rnd.NextSByte();
                var a = n;
                var r = n.ReverseEndiannessCopy();

                Assert.Equal(a, r);
            }
        }

        [Fact]
        public void TestByte()
        {
            for (var c = 0; c < testCount; c++)
            {
                var n = rnd.NextByte();
                var a = n;
                var r = n.ReverseEndiannessCopy();

                Assert.Equal(a, r);
            }
        }

        [Fact]
        public void TestInt16()
        {
            for (var c = 0; c < testCount; c++)
            {
                var n = rnd.NextInt16();
                var aa = BitConverter.GetBytes(n).Reverse().ToArray();
                var a = BitConverter.ToInt16(aa, 0);
                var r = n.ReverseEndiannessCopy();

                Assert.Equal(a, r);
            }
        }

        [Fact]
        public void TestUInt16()
        {
            for (var c = 0; c < testCount; c++)
            {
                var n = rnd.NextUInt16();
                var aa = BitConverter.GetBytes(n).Reverse().ToArray();
                var a = BitConverter.ToUInt16(aa, 0);
                var r = n.ReverseEndiannessCopy();

                Assert.Equal(a, r);
            }
        }
        [Fact]
        public void TestInt32()
        {
            for (var c = 0; c < testCount; c++)
            {
                var n = rnd.NextInt32();
                var aa = BitConverter.GetBytes(n).Reverse().ToArray();
                var a = BitConverter.ToInt32(aa, 0);
                var r = n.ReverseEndiannessCopy();

                Assert.Equal(a, r);
            }
        }

        [Fact]
        public void TestUInt32()
        {
            for (var c = 0; c < testCount; c++)
            {
                var n = rnd.NextUInt32();
                var aa = BitConverter.GetBytes(n).Reverse().ToArray();
                var a = BitConverter.ToUInt32(aa, 0);
                var r = n.ReverseEndiannessCopy();

                Assert.Equal(a, r);
            }
        }

        [Fact]
        public void TestInt64()
        {
            for (var c = 0; c < testCount; c++)
            {
                var n = rnd.NextInt64();
                var aa = BitConverter.GetBytes(n).Reverse().ToArray();
                var a = BitConverter.ToInt64(aa, 0);
                var r = n.ReverseEndiannessCopy();

                Assert.Equal(a, r);
            }
        }

        [Fact]
        public void TestUInt64()
        {
            for (var c = 0; c < testCount; c++)
            {
                var n = rnd.NextUInt64();
                var aa = BitConverter.GetBytes(n).Reverse().ToArray();
                var a = BitConverter.ToUInt64(aa, 0);
                var r = n.ReverseEndiannessCopy();

                Assert.Equal(a, r);
            }
        }


    }
}