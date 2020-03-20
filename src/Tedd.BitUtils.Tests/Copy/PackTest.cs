using System;
using Xunit;
using Xunit.Abstractions;

namespace Tedd.BitUtils.Tests.Copy
{
    public class PackTest
    {
        private readonly ITestOutputHelper _outputHelper;
        private Random _rnd = new Random();
        private const int Iterations = 1_000;

        public PackTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void TestByte()
        {
            Byte v1 = 0b1001_1001;
            Byte i1 = 0b0000_0010;
            Byte a1 = 0b1001_0001;
            var v1b = v1;
            var v1a = v1.PackCopy(5, 2, i1);
            Assert.Equal(v1b, v1);
            Assert.Equal(a1.ToBitStringPadded(), v1a.ToBitStringPadded());

            for (var i = 0; i < Iterations; i++)
            {
                var v = _rnd.NextByte();
                var sv = _rnd.NextByte();
                for (var offset = 1; offset < sizeof(Byte) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var ov = v;
                        v=v.PackCopy(offset, len, sv);

                        var ss = sv.ToBitStringPadded();
                        ss = ss.Substring(ss.Length - len, len);
                        var ovs = ov.ToBitStringPadded();
                        ovs = ovs.Substring(0, ovs.Length - offset)
                             + ss
                             + ovs.Substring(ovs.Length - offset + len);
                        Assert.Equal(v.ToBitStringPadded(), ovs);
                        //_outputHelper.WriteLine(ovs);

                    }
                }

            }
        }
        [Fact]
        public void TestSByte()
        {
            SByte v1 = 0b0001_1001;
            SByte i1 = 0b0000_0010;
            SByte a1 = 0b0001_0001;
            var v1b = v1;
            var v1a = v1.PackCopy(5, 2, i1);
            Assert.Equal(v1b, v1);
            Assert.Equal(a1.ToBitStringPadded(), v1a.ToBitStringPadded());

            for (var i = 0; i < Iterations; i++)
            {
                var v = _rnd.NextSByte();
                var sv = _rnd.NextSByte();
                for (var offset = 1; offset < sizeof(SByte) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var ov = v;
                        v=v.PackCopy(offset, len, sv);

                        var ss = sv.ToBitStringPadded();
                        ss = ss.Substring(ss.Length - len, len);
                        var ovs = ov.ToBitStringPadded();
                        ovs = ovs.Substring(0, ovs.Length - offset)
                             + ss
                             + ovs.Substring(ovs.Length - offset + len);
                        Assert.Equal(v.ToBitStringPadded(), ovs);
                        //_outputHelper.WriteLine(ovs);

                    }
                }

            }
        }

        [Fact]
        public void TestUInt16()
        {
            UInt16 v1 = 0b1001_1001;
            UInt16 i1 = 0b0000_0010;
            UInt16 a1 = 0b1001_0001;
            var v1b = v1;
            var v1a = v1.PackCopy(5, 2, i1);
            Assert.Equal(v1b, v1);
            Assert.Equal(a1.ToBitStringPadded(), v1a.ToBitStringPadded());

            for (var i = 0; i < Iterations; i++)
            {
                var v = _rnd.NextUInt16();
                var sv = _rnd.NextUInt16();
                for (var offset = 1; offset < sizeof(UInt16) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var ov = v;
                        v = v.PackCopy(offset, len, sv);

                        var ss = sv.ToBitStringPadded();
                        ss = ss.Substring(ss.Length - len, len);
                        var ovs = ov.ToBitStringPadded();
                        ovs = ovs.Substring(0, ovs.Length - offset)
                              + ss
                              + ovs.Substring(ovs.Length - offset + len);
                        Assert.Equal(v.ToBitStringPadded(), ovs);
                        //_outputHelper.WriteLine(ovs);

                    }
                }

            }
        }
        [Fact]
        public void TestInt16()
        {
            Int16 v1 = 0b1001_1001;
            Int16 i1 = 0b0000_0010;
            Int16 a1 = 0b1001_0001;
            var v1b = v1;
            var v1a = v1.PackCopy(5, 2, i1);
            Assert.Equal(v1b, v1);
            Assert.Equal(a1.ToBitStringPadded(), v1a.ToBitStringPadded());

            for (var i = 0; i < Iterations; i++)
            {
                var v = _rnd.NextInt16();
                var sv = _rnd.NextInt16();
                for (var offset = 1; offset < sizeof(Int16) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var ov = v;
                        v = v.PackCopy(offset, len, sv);

                        var ss = sv.ToBitStringPadded();
                        ss = ss.Substring(ss.Length - len, len);
                        var ovs = ov.ToBitStringPadded();
                        ovs = ovs.Substring(0, ovs.Length - offset)
                              + ss
                              + ovs.Substring(ovs.Length - offset + len);
                        Assert.Equal(v.ToBitStringPadded(), ovs);
                        //_outputHelper.WriteLine(ovs);

                    }
                }

            }
        }

        [Fact]
        public void TestUInt32()
        {
            UInt32 v1 = 0b1001_1001;
            UInt32 i1 = 0b0000_0010;
            UInt32 a1 = 0b1001_0001;
            var v1b = v1;
            var v1a = v1.PackCopy(5, 2, i1);
            Assert.Equal(v1b, v1);
            Assert.Equal(a1.ToBitStringPadded(), v1a.ToBitStringPadded());

            for (var i = 0; i < Iterations; i++)
            {
                var v = _rnd.NextUInt32();
                var sv = _rnd.NextUInt32();
                for (var offset = 1; offset < sizeof(UInt32) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var ov = v;
                        v = v.PackCopy(offset, len, sv);

                        var ss = sv.ToBitStringPadded();
                        ss = ss.Substring(ss.Length - len, len);
                        var ovs = ov.ToBitStringPadded();
                        ovs = ovs.Substring(0, ovs.Length - offset)
                              + ss
                              + ovs.Substring(ovs.Length - offset + len);
                        Assert.Equal(v.ToBitStringPadded(), ovs);
                        //_outputHelper.WriteLine(ovs);

                    }
                }

            }
        }
        [Fact]
        public void TestInt32()
        {
            Int32 v1 = 0b1001_1001;
            Int32 i1 = 0b0000_0010;
            Int32 a1 = 0b1001_0001;
            var v1b = v1;
            var v1a = v1.PackCopy(5, 2, i1);
            Assert.Equal(v1b, v1);
            Assert.Equal(a1.ToBitStringPadded(), v1a.ToBitStringPadded());

            for (var i = 0; i < Iterations; i++)
            {
                var v = _rnd.NextInt32();
                var sv = _rnd.NextInt32();
                for (var offset = 1; offset < sizeof(Int32) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var ov = v;
                        v = v.PackCopy(offset, len, sv);

                        var ss = sv.ToBitStringPadded();
                        ss = ss.Substring(ss.Length - len, len);
                        var ovs = ov.ToBitStringPadded();
                        ovs = ovs.Substring(0, ovs.Length - offset)
                              + ss
                              + ovs.Substring(ovs.Length - offset + len);
                        Assert.Equal(v.ToBitStringPadded(), ovs);
                        //_outputHelper.WriteLine(ovs);

                    }
                }

            }
        } 
        
        [Fact]
        public void TestUInt64()
        {
            UInt64 v1 = 0b1001_1001;
            UInt64 i1 = 0b0000_0010;
            UInt64 a1 = 0b1001_0001;
            var v1b = v1;
            var v1a = v1.PackCopy(5, 2, i1);
            Assert.Equal(v1b, v1);
            Assert.Equal(a1.ToBitStringPadded(), v1a.ToBitStringPadded());

            for (var i = 0; i < Iterations; i++)
            {
                var v = _rnd.NextUInt64();
                var sv = _rnd.NextUInt64();
                for (var offset = 1; offset < sizeof(UInt64) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var ov = v;
                        v = v.PackCopy(offset, len, sv);

                        var ss = sv.ToBitStringPadded();
                        ss = ss.Substring(ss.Length - len, len);
                        var ovs = ov.ToBitStringPadded();
                        ovs = ovs.Substring(0, ovs.Length - offset)
                              + ss
                              + ovs.Substring(ovs.Length - offset + len);
                        Assert.Equal(v.ToBitStringPadded(), ovs);
                        //_outputHelper.WriteLine(ovs);

                    }
                }

            }
        }
        [Fact]
        public void TestInt64()
        {
            Int64 v1 = 0b1001_1001;
            Int64 i1 = 0b0000_0010;
            Int64 a1 = 0b1001_0001;
            var v1b = v1;
            var v1a = v1.PackCopy(5, 2, i1);
            Assert.Equal(v1b, v1);
            Assert.Equal(a1.ToBitStringPadded(), v1a.ToBitStringPadded());

            for (var i = 0; i < Iterations; i++)
            {
                var v = _rnd.NextInt64();
                var sv = _rnd.NextInt64();
                for (var offset = 1; offset < sizeof(Int64) * 8; offset++)
                {
                    for (var len = 1; len < offset; len++)
                    {
                        var ov = v;
                        v = v.PackCopy(offset, len, sv);

                        var ss = sv.ToBitStringPadded();
                        ss = ss.Substring(ss.Length - len, len);
                        var ovs = ov.ToBitStringPadded();
                        ovs = ovs.Substring(0, ovs.Length - offset)
                              + ss
                              + ovs.Substring(ovs.Length - offset + len);
                        Assert.Equal(v.ToBitStringPadded(), ovs);
                        //_outputHelper.WriteLine(ovs);

                    }
                }

            }
        }

    }
}