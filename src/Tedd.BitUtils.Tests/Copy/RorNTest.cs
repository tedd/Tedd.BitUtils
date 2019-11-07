using System;
using Xunit;
using Xunit.Abstractions;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class RorNCopyTest
    {
        private readonly ITestOutputHelper _output;

        public RorNCopyTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestInt32()
        {
            for (var i = 0; i < (sizeof(Int32) * 8); i++)
            {
                var b = sizeof(Int32) * 8 - 1;
                Int32 v = unchecked((Int32)(UInt32)1 << b);
                v = v.RorCopy(i);
                var a = (Int32)((UInt32)1 << (b - i));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(32, '0') + " == " + Convert.ToString(v, 2).PadLeft(32, '0'));
                Assert.Equal(a, v);
            }
        }

        [Fact]
        public void TestUInt32()
        {
            for (var i = 0; i < (sizeof(UInt32) * 8); i++)
            {
                var b = sizeof(UInt32) * 8 - 1;
                UInt32 v = unchecked((UInt32)(UInt32)1 << b);
                v = v.RorCopy(i);
                var a = (UInt32)((UInt32)1 << (b - i));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(32, '0') + " == " + Convert.ToString(v, 2).PadLeft(32, '0'));
                Assert.Equal(a, v);
            }
        }

        [Fact]
        public void TestInt64()
        {
            for (var i = 0; i < (sizeof(Int64) * 8); i++)
            {
                var b = sizeof(Int64) * 8 - 1;
                Int64 v = unchecked((Int64)(UInt64)1 << b);
                v = v.RorCopy(i);
                var a = (Int64)((UInt64)1 << (b - i));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(64, '0') + " == " + Convert.ToString(v, 2).PadLeft(64, '0'));
                Assert.Equal(a, v);
            }
        }

        [Fact]
        public void TestUInt64()
        {
            for (var i = 0; i < (sizeof(UInt64) * 8); i++)
            {
                var b = sizeof(UInt64) * 8 - 1;
                UInt64 v = unchecked((UInt64)(UInt64)1 << b);
                v = v.RorCopy(i);
                var a = (UInt64)((UInt64)1 << (b - i));
                _output?.WriteLine(Convert.ToString((Int64)a, 2).PadLeft(64, '0') + " == " + Convert.ToString((Int64)v, 2).PadLeft(64, '0'));
                Assert.Equal(a, v);
            }
        }
    }
}