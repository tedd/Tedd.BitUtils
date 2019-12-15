using System;
using Xunit;
using Xunit.Abstractions;

namespace Tedd.BitUtils.Tests.InPlace
{
    public class PackTest
    {
        private readonly ITestOutputHelper _output;
        private Random _rnd = new Random();
        public PackTest(ITestOutputHelper output)
        {
            _output = output;
        }


        //[Fact]
        //public void TestInt32()
        //{
        //    var max = 1000;
        //    for (var offset = 0; offset < sizeof(Int32) * 8; offset++)
        //    {
        //        for (var len = 0; len < sizeof(Int32) * 8; len++)
        //        {
        //            // for (var r = 0; r < 1000; r++)
        //            {
        //                var a = GetRandomInt32();
        //                var cm1 = ((~0U << offset) >> (offset + len)) << len;
        //                //_output?.WriteLine(Convert.ToString((Int32)cm1, 2).PadLeft(sizeof(Int32) * 8, '0') + "  & " + Convert.ToString((Int32)cm2, 2).PadLeft(sizeof(Int32) * 8, '0'));
        //                // Clear space for val
        //                a &= ~(Int32)cm1;

        //                var val = (UInt32)GetRandomInt32();
        //                var valResult = val | (~0U >> sizeof(Int32) * 8 - len);
        //                var v = a;
        //                a |= (Int32)(valResult << offset);
        //                v.Pack(offset, len, (UInt32)val);

        //                _output?.WriteLine(Convert.ToString((Int32)a, 2).PadLeft(sizeof(Int32) * 8, '0') + " == " + Convert.ToString((Int32)v, 2).PadLeft(sizeof(Int32)*8, '0'));

        //                if (--max < 0)
        //                    return;
        //            }
        //        }
        //    }
        //}
    }
}