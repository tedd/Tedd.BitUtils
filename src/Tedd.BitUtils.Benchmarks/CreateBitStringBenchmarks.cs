using BenchmarkDotNet.Attributes;
using System;
using Tedd;

namespace Tedd.BitUtils.Benchmarks
{
    [MemoryDiagnoser]
    public class CreateBitStringBenchmarks
    {
        private ulong[] _values;
        private string[] _results;

        [Params(1024)]
        public int Count { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _values = new ulong[Count];
            _results = new string[Count];
            var r = new Random(42);
            for (int i = 0; i < Count; i++)
                _values[i] = (ulong)r.NextInt64();
        }

        [Benchmark(Baseline = true)]
        public void Baseline_CreateBitString()
        {
            for (int i = 0; i < Count; i++)
            {
                ulong v = _values[i];
                _results[i] = Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.ToBitStringPadded(ref v);
            }
        }

        [Benchmark]
        public void Optimized_CreateBitString()
        {
            for (int i = 0; i < Count; i++)
            {
                ulong v = _values[i];
                _results[i] = Tedd.BitUtilsExtensions.ToBitStringPadded(ref v);
            }
        }
    }
}
