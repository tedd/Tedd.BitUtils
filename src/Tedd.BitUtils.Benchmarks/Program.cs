using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using Tedd.BitUtils.Archive;
using Tedd;

namespace Tedd.BitUtils.Benchmarks
{
    [MemoryDiagnoser]
    public class ToBitStringBenchmarks
    {
        private int _value = 123456789;

        [Benchmark(Baseline = true)]
        public string Archive_ToBitStringPadded()
        {
            return Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.ToBitStringPadded(ref _value);
        }

        [Benchmark]
        public string Optimized_ToBitStringPadded()
        {
            return Tedd.BitUtilsExtensions.ToBitStringPadded(ref _value);
        }

        [Benchmark]
        public string Archive_ToBitString()
        {
            return Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.ToBitString(ref _value);
        }

        [Benchmark]
        public string Optimized_ToBitString()
        {
            return Tedd.BitUtilsExtensions.ToBitString(ref _value);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ToBitStringBenchmarks>();
        }
    }
}
