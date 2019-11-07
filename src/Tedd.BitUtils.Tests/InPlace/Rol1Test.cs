using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class Rol1Test
    {
        private readonly ITestOutputHelper _output;

        public Rol1Test(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestInt32()
        {
            Int32 v = 1;
            for (var i = 0; i < (sizeof(Int32) * 8) - 1; i++)
            {
                v.Rol();
                var a = (Int32)((UInt32)1 << (i + 1));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(32, '0') + " == " + Convert.ToString(v, 2).PadLeft(32, '0'));
                Assert.Equal(a, v);
            }

            v.Rol();
            _output?.WriteLine(Convert.ToString(1, 2).PadLeft(32, '0') + " == " + Convert.ToString((Int32)((UInt32)v), 2).PadLeft(32, '0'));
            Assert.Equal((Int32)1, (Int32)(UInt32)v);
        }

        [Fact]
        public void TestUInt32()
        {
            UInt32 v = 1;
            for (var i = 0; i < (sizeof(UInt32) * 8) - 1; i++)
            {
                v.Rol();
                var a = (UInt32)((UInt32)1 << (i + 1));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(32, '0') + " == " + Convert.ToString(v, 2).PadLeft(32, '0'));
                Assert.Equal(a, v);
            }

            v.Rol();
            _output?.WriteLine(Convert.ToString(1, 2).PadLeft(32, '0') + " == " + Convert.ToString((UInt32)((UInt32)v), 2).PadLeft(32, '0'));
            Assert.Equal((UInt32)1, (UInt32)(UInt32)v);
        }

        [Fact]
        public void TestInt64()
        {
            Int64 v = 1;
            for (var i = 0; i < (sizeof(Int64) * 8) - 1; i++)
            {
                v.Rol();
                var a = (Int64)((UInt64)1 << (i + 1));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(64, '0') + " == " + Convert.ToString(v, 2).PadLeft(64, '0'));
                Assert.Equal(a, v);
            }

            v.Rol();
            _output?.WriteLine(Convert.ToString(1, 2).PadLeft(64, '0') + " == " + Convert.ToString((Int64)((UInt64)v), 2).PadLeft(64, '0'));
            Assert.Equal((Int64)1, (Int64)(UInt64)v);
        }

        [Fact]
        public void TestUInt64()
        {
            UInt64 v = 1;
            for (var i = 0; i < (sizeof(UInt64) * 8) - 1; i++)
            {
                v.Rol();
                var a = (UInt64)((UInt64)1 << (i + 1));
                _output?.WriteLine(Convert.ToString((Int64)a, 2).PadLeft(64, '0') + " == " + Convert.ToString((Int64)v, 2).PadLeft(64, '0'));
                Assert.Equal(a, v);
            }

            v.Rol();
            _output?.WriteLine(Convert.ToString(1, 2).PadLeft(64, '0') + " == " + Convert.ToString((Int64)((UInt64)v), 2).PadLeft(64, '0'));
            Assert.Equal((UInt64)1, (UInt64)(UInt64)v);
        }

    }
}
