using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class UnpackTest
    {
        private Random _rnd = new Random();
        private const int Iterations = 1_000;

        [Fact]
        public void TestByte()
        {
            Byte r = 0b1001_1001;
            Assert.Equal(0b11, r.Unpack(5, 2));

            for (var i = 0; i < Iterations; i++)
            {
                r = _rnd.NextByte();
                for (var offset = 1; offset < sizeof(Byte)*8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var n = (UInt32)r.Unpack(offset, len);
                        var nsp = n.ToBitStringPadded();
                        var ns = nsp.Substring(nsp.Length - len, len);

                        var ur = (UInt64)r;
                        var srsp = ur.ToBitStringPadded();
                        var srs = srsp.Substring(srsp.Length - offset , len);
                        Assert.NotEqual(0, srs.Length);
                        Assert.Equal(srs, ns);

                    }
                }

            }
        }

        [Fact]
        public void TestSByte()
        {
            SByte r = 0b0101_1001;
            Assert.Equal(0b11, r.ExtractBits(5, 2));

            for (var i = 0; i < Iterations; i++)
            {
                r = _rnd.NextSByte();
                for (var offset = 1; offset < sizeof(SByte) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var n = (UInt32)r.ExtractBits(offset, len);
                        var nsp = n.ToBitStringPadded();
                        var ns = nsp.Substring(nsp.Length - len, len);

                        var ur = (UInt64)r;
                        var srsp = ur.ToBitStringPadded();
                        var srs = srsp.Substring(srsp.Length - offset, len);
                        Assert.NotEqual(0, srs.Length);
                        Assert.Equal(srs, ns);

                    }
                }

            }
        }

        [Fact]
        public void TestUInt16()
        {
            UInt16 r = 0b1001_1001;
            Assert.Equal(0b11, r.Unpack(5, 2));

            for (var i = 0; i < Iterations; i++)
            {
                r = _rnd.NextUInt16();
                for (var offset = 1; offset < sizeof(UInt16) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var n = (UInt32)r.Unpack(offset, len);
                        var nsp = n.ToBitStringPadded();
                        var ns = nsp.Substring(nsp.Length - len, len);

                        var ur = (UInt64)r;
                        var srsp = ur.ToBitStringPadded();
                        var srs = srsp.Substring(srsp.Length - offset, len);
                        Assert.NotEqual(0, srs.Length);
                        Assert.Equal(srs, ns);

                    }
                }

            }
        }

        [Fact]
        public void TestInt16()
        {
            Int16 r = 0b1001_1001;
            Assert.Equal(0b11, r.Unpack(5, 2));

            for (var i = 0; i < Iterations; i++)
            {
                r = _rnd.NextInt16();
                for (var offset = 1; offset < sizeof(Int16) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var n = (UInt32)r.Unpack(offset, len);
                        var nsp = n.ToBitStringPadded();
                        var ns = nsp.Substring(nsp.Length - len, len);

                        var ur = (UInt64)r;
                        var srsp = ur.ToBitStringPadded();
                        var srs = srsp.Substring(srsp.Length - offset, len);
                        Assert.NotEqual(0, srs.Length);
                        Assert.Equal(srs, ns);

                    }
                }

            }
        }

        [Fact]
        public void TestUInt32()
        {
            UInt32 r = 0b1001_1001;
            Assert.Equal((UInt32)0b11, r.Unpack(5, 2));

            for (var i = 0; i < Iterations; i++)
            {
                r = _rnd.NextUInt32();
                for (var offset = 1; offset < sizeof(UInt32) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var n = (UInt32)r.Unpack(offset, len);
                        var nsp = n.ToBitStringPadded();
                        var ns = nsp.Substring(nsp.Length - len, len);

                        var ur = (UInt64)r;
                        var srsp = ur.ToBitStringPadded();
                        var srs = srsp.Substring(srsp.Length - offset, len);
                        Assert.NotEqual(0, srs.Length);
                        Assert.Equal(srs, ns);

                    }
                }

            }
        }

        [Fact]
        public void TestInt32()
        {
            Int32 r = 0b1001_1001;
            Assert.Equal(0b11, r.Unpack(5, 2));

            for (var i = 0; i < Iterations; i++)
            {
                r = _rnd.NextInt32();
                for (var offset = 1; offset < sizeof(Int32) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var n = (UInt32)r.Unpack(offset, len);
                        var nsp = n.ToBitStringPadded();
                        var ns = nsp.Substring(nsp.Length - len, len);

                        var ur = (UInt64)r;
                        var srsp = ur.ToBitStringPadded();
                        var srs = srsp.Substring(srsp.Length - offset, len);
                        Assert.NotEqual(0, srs.Length);
                        Assert.Equal(srs, ns);

                    }
                }

            }
        }

        [Fact]
        public void TestUInt64()
        {
            UInt64 r = 0b1001_1001;
            Assert.Equal((UInt64)0b11, r.Unpack(5, 2));

            for (var i = 0; i < Iterations; i++)
            {
                r = _rnd.NextUInt64();
                for (var offset = 1; offset < sizeof(UInt64) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var n = (UInt64)r.Unpack(offset, len);
                        var nsp = n.ToBitStringPadded();
                        var ns = nsp.Substring(nsp.Length - len, len);

                        var ur = (UInt64)r;
                        var srsp = ur.ToBitStringPadded();
                        var srs = srsp.Substring(srsp.Length - offset, len);
                        Assert.NotEqual(0, srs.Length);
                        Assert.Equal(srs, ns);

                    }
                }

            }
        }

        [Fact]
        public void TestInt64()
        {
            Int64 r = 0b1001_1001;
            Assert.Equal(0b11, r.Unpack(5, 2));

            for (var i = 0; i < Iterations; i++)
            {
                r = _rnd.NextInt64();
                for (var offset = 1; offset < sizeof(Int64) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var n = (UInt64)r.Unpack(offset, len);
                        var nsp = n.ToBitStringPadded();
                        var ns = nsp.Substring(nsp.Length - len, len);

                        var ur = (UInt64)r;
                        var srsp = ur.ToBitStringPadded();
                        var srs = srsp.Substring(srsp.Length - offset, len);
                        Assert.NotEqual(0, srs.Length);
                        Assert.Equal(srs, ns);

                    }
                }

            }
        }
    }
}