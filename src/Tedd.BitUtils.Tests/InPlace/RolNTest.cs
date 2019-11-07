using System;
using Xunit;
using Xunit.Abstractions;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class RolNTest
    {
        private readonly ITestOutputHelper _output;

        public RolNTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestInt32()
        {
            for (var i = 0; i < (sizeof(Int32) * 8) ; i++)
            {
                Int32 v = 1;
                v.Rol(i);
                var a = (Int32)((UInt32)1 << (i ));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(32, '0') + " == " + Convert.ToString(v, 2).PadLeft(32, '0'));
                Assert.Equal(a, v);
            }
        }

        [Fact]
        public void TestUInt32()
        {
            for (var i = 0; i < (sizeof(UInt32) * 8); i++)
            {
                UInt32 v = 1;
                v.Rol(i);
                var a = (UInt32)((UInt32)1 << (i));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(32, '0') + " == " + Convert.ToString(v, 2).PadLeft(32, '0'));
                Assert.Equal(a, v);
            }
        }

        [Fact]
        public void TestInt64()
        {
            for (var i = 0; i < (sizeof(Int64) * 8); i++)
            {
                Int64 v = 1;
                v.Rol(i);
                var a = (Int64)((UInt64)1 << (i));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(64, '0') + " == " + Convert.ToString(v, 2).PadLeft(64, '0'));
                Assert.Equal(a, v);
            }
        }

        [Fact]
        public void TestUInt64()
        {
            for (var i = 0; i < (sizeof(UInt64) * 8); i++)
            {
                UInt64 v = 1;
                v.Rol(i);
                var a = (UInt64)((UInt64)1 << (i));
                _output?.WriteLine(Convert.ToString((Int64)a, 2).PadLeft(64, '0') + " == " + Convert.ToString((Int64)v, 2).PadLeft(64, '0'));
                Assert.Equal(a, v);
            }
        }
    }
}