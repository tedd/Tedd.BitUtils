using System;
using Xunit;
using Xunit.Abstractions;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class Ror1CopyTest
    {
        private readonly ITestOutputHelper _output;

        public Ror1CopyTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestInt32()
        {
            var b = sizeof(Int32) * 8 - 1;
            var start = unchecked((Int32)(UInt32)1 << b);
            Int32 v = start;
            for (var i = 0; i < (sizeof(Int32) * 8) - 1; i++)
            {
                v = v.RorCopy();
                var a = (Int32)((UInt32)1 << (b - 1 - i));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(32, '0') + " == " + Convert.ToString(v, 2).PadLeft(32, '0'));
                Assert.Equal(a, v);
            }

            v = v.RorCopy();
            _output?.WriteLine(Convert.ToString(start, 2).PadLeft(32, '0') + " == " + Convert.ToString((Int32)((UInt32)v), 2).PadLeft(32, '0'));
            Assert.Equal((Int32)start, (Int32)(UInt32)v);
        }
        [Fact]
        public void TestUInt32()
        {
            var b = sizeof(UInt32) * 8 - 1;
            var start = unchecked((UInt32)(UInt32)1 << b);
            UInt32 v = start;
            for (var i = 0; i < (sizeof(UInt32) * 8) - 1; i++)
            {
                v = v.RorCopy();
                var a = (UInt32)((UInt32)1 << (b - 1 - i));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(32, '0') + " == " + Convert.ToString(v, 2).PadLeft(32, '0'));
                Assert.Equal(a, v);
            }

            v = v.RorCopy();
            _output?.WriteLine(Convert.ToString(start, 2).PadLeft(32, '0') + " == " + Convert.ToString((UInt32)((UInt32)v), 2).PadLeft(32, '0'));
            Assert.Equal((UInt32)start, (UInt32)(UInt32)v);
        }

        [Fact]
        public void TestInt64()
        {
            var b = sizeof(Int64) * 8 - 1;
            var start = unchecked((Int64)(UInt64)1 << b);
            Int64 v = start;
            for (var i = 0; i < (sizeof(Int64) * 8) - 1; i++)
            {
                v = v.RorCopy();
                var a = (Int64)((UInt64)1 << (b - 1 - i));
                _output?.WriteLine(Convert.ToString(a, 2).PadLeft(64, '0') + " == " + Convert.ToString(v, 2).PadLeft(64, '0'));
                Assert.Equal(a, v);
            }

            v = v.RorCopy();
            _output?.WriteLine(Convert.ToString(start, 2).PadLeft(64, '0') + " == " + Convert.ToString((Int64)((UInt64)v), 2).PadLeft(64, '0'));
            Assert.Equal((Int64)start, (Int64)(UInt64)v);
        }
        [Fact]
        public void TestUInt64()
        {
            var b = sizeof(UInt64) * 8 - 1;
            var start = unchecked((UInt64)(UInt64)1 << b);
            UInt64 v = start;
            for (var i = 0; i < (sizeof(UInt64) * 8) - 1; i++)
            {
                v = v.RorCopy();
                var a = (UInt64)((UInt64)1 << (b - 1 - i));
                _output?.WriteLine(Convert.ToString((Int64)a, 2).PadLeft(64, '0') + " == " + Convert.ToString((Int64)v, 2).PadLeft(64, '0'));
                Assert.Equal(a, v);
            }

            v = v.RorCopy();
            _output?.WriteLine(Convert.ToString((Int64)start, 2).PadLeft(64, '0') + " == " + Convert.ToString((Int64)((UInt64)v), 2).PadLeft(64, '0'));
            Assert.Equal((UInt64)start, (UInt64)(UInt64)v);
        }

    }
}