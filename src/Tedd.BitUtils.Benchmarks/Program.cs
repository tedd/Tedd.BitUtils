using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Tedd.BitUtils.Benchmarks;

/// <summary>
/// Old (Tedd.BitUtils.Archive, a frozen snapshot of 1.0.7) vs new (Tedd.BitUtils 2.0.0) side by side.
/// Run a specific class with e.g. <c>dotnet run -c Release --filter *ReverseEndianness*</c>, or with no filter
/// BenchmarkSwitcher prompts interactively for which one(s) to run.
/// </summary>
/// <remarks>
/// Every call below is fully qualified (never a bare <c>value.Method()</c>) even though that reads more verbosely:
/// the old and new libraries expose extension methods with identical names and signatures, so an unqualified call
/// with both namespaces in scope would either be ambiguous or - worse - silently resolve to just one of them,
/// making "Archive_X" and "Optimized_X" secretly benchmark the same code.
/// </remarks>
public class Program
{
    public static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}

[MemoryDiagnoser]
public class ToBitStringBenchmarks
{
    private int _value = 123456789;

    [Benchmark(Baseline = true)]
    public string Archive_ToBitStringPadded() => Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.ToBitStringPadded(ref _value);

    [Benchmark]
    public string Optimized_ToBitStringPadded() => Tedd.BitUtilsExtensions.ToBitStringPadded(ref _value);

    [Benchmark]
    public string Archive_ToBitString() => Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.ToBitString(ref _value);

    [Benchmark]
    public string Optimized_ToBitString() => Tedd.BitUtilsExtensions.ToBitString(ref _value);
}

/// <summary>
/// Hypothesis: the 1.0.7 ReverseEndianness never actually took its intrinsic-backed fast path (see the comment in
/// Tedd.BitUtils.Archive/BitUtilsExtensions.cs) and always ran a manual shift-based software fallback instead.
/// Routing every call through BinaryPrimitives.ReverseEndianness (a single BSWAP on x86/x64) in 2.0.0 should measurably win.
/// </summary>
[MemoryDiagnoser]
public class ReverseEndiannessBenchmarks
{
    private int _valueInt32 = 123456789;
    private long _valueInt64 = 123456789012345L;

    [Benchmark(Baseline = true)]
    public int Archive_Int32() { var v = _valueInt32; Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.ReverseEndianness(ref v); return v; }

    [Benchmark]
    public int Optimized_Int32() { var v = _valueInt32; Tedd.BitUtilsEndinanessExtensions.ReverseEndianness(ref v); return v; }

    [Benchmark]
    public long Archive_Int64() { var v = _valueInt64; Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.ReverseEndianness(ref v); return v; }

    [Benchmark]
    public long Optimized_Int64() { var v = _valueInt64; Tedd.BitUtilsEndinanessExtensions.ReverseEndianness(ref v); return v; }
}

/// <summary>
/// Hypothesis: a branch-free SWAR bit swap (new, x86/x64 path) beats four dependent table lookups (old) for 32/64-bit
/// widths, since it trades a handful of ALU ops for zero memory accesses. For the 8-bit case both old and new use the
/// same 256-entry lookup table, so that comparison is expected to be a wash - included as a sanity check.
/// </summary>
[MemoryDiagnoser]
public class ReverseBitsBenchmarks
{
    private byte _valueByte = 0b1101_0010;
    private uint _valueUInt32 = 0xDEADBEEF;
    private ulong _valueUInt64 = 0xDEADBEEF_CAFEBABE;

    [Benchmark(Baseline = true)]
    public byte Archive_Byte() { var v = _valueByte; Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.ReverseBits(ref v); return v; }

    [Benchmark]
    public byte Optimized_Byte() { var v = _valueByte; Tedd.BitUtilsExtensions.ReverseBits(ref v); return v; }

    [Benchmark]
    public uint Archive_UInt32() { var v = _valueUInt32; Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.ReverseBits(ref v); return v; }

    [Benchmark]
    public uint Optimized_UInt32() { var v = _valueUInt32; Tedd.BitUtilsExtensions.ReverseBits(ref v); return v; }

    [Benchmark]
    public ulong Archive_UInt64() { var v = _valueUInt64; Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.ReverseBits(ref v); return v; }

    [Benchmark]
    public ulong Optimized_UInt64() { var v = _valueUInt64; Tedd.BitUtilsExtensions.ReverseBits(ref v); return v; }
}

/// <summary>
/// Hypothesis: no change. Both old and new resolve to the same POPCNT instruction on this CPU (old: an explicit
/// X86.Popcnt.IsSupported check per call; new: System.Numerics.BitOperations.PopCount, which does the same check
/// internally). This benchmark exists to confirm the refactor to BitOperations did not regress performance.
/// </summary>
[MemoryDiagnoser]
public class PopCountBenchmarks
{
    private uint _valueUInt32 = 0xDEADBEEF;
    private ulong _valueUInt64 = 0xDEADBEEF_CAFEBABE;

    [Benchmark(Baseline = true)]
    public int Archive_UInt32() { var v = _valueUInt32; return Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.PopCount(ref v); }

    [Benchmark]
    public int Optimized_UInt32() { var v = _valueUInt32; return Tedd.BitUtilsExtensions.PopCount(ref v); }

    [Benchmark]
    public int Archive_UInt64() { var v = _valueUInt64; return Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.PopCount(ref v); }

    [Benchmark]
    public int Optimized_UInt64() { var v = _valueUInt64; return Tedd.BitUtilsExtensions.PopCount(ref v); }
}

/// <summary>Hypothesis: no change, for the same reason as PopCountBenchmarks (old used X86.Lzcnt directly; new uses BitOperations.LeadingZeroCount, backed by the same instruction).</summary>
[MemoryDiagnoser]
public class LeadingZeroCountBenchmarks
{
    private uint _valueUInt32 = 0xDEADBEEF;
    private ulong _valueUInt64 = 0xDEADBEEF_CAFEBABE;

    [Benchmark(Baseline = true)]
    public int Archive_UInt32() { var v = _valueUInt32; return Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.LeadingZeroCount(ref v); }

    [Benchmark]
    public int Optimized_UInt32() { var v = _valueUInt32; return Tedd.BitUtilsExtensions.LeadingZeroCount(ref v); }

    [Benchmark]
    public int Archive_UInt64() { var v = _valueUInt64; return Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.LeadingZeroCount(ref v); }

    [Benchmark]
    public int Optimized_UInt64() { var v = _valueUInt64; return Tedd.BitUtilsExtensions.LeadingZeroCount(ref v); }
}

/// <summary>Hypothesis: no change. Both old (manual shift pair) and new (BitOperations.RotateLeft) are patterns RyuJIT recognizes and compiles to a single ROL instruction.</summary>
[MemoryDiagnoser]
public class RolBenchmarks
{
    private uint _valueUInt32 = 0xDEADBEEF;
    private ulong _valueUInt64 = 0xDEADBEEF_CAFEBABE;

    [Benchmark(Baseline = true)]
    public uint Archive_UInt32() { var v = _valueUInt32; Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.Rol(ref v, 7); return v; }

    [Benchmark]
    public uint Optimized_UInt32() { var v = _valueUInt32; Tedd.BitUtilsExtensions.Rol(ref v, 7); return v; }

    [Benchmark]
    public ulong Archive_UInt64() { var v = _valueUInt64; Tedd.BitUtils.Archive.BitUtilsArchiveExtensions.Rol(ref v, 23); return v; }

    [Benchmark]
    public ulong Optimized_UInt64() { var v = _valueUInt64; Tedd.BitUtilsExtensions.Rol(ref v, 23); return v; }
}

/// <summary>
/// New in 2.0.0, no old implementation to compare against: BMI2 PEXT versus the portable bit-by-bit software
/// fallback that runs when BMI2 is unavailable (see BitUtilsExtensions.BitTricks.cs). Quantifies what the hardware
/// intrinsic buys over the fallback that ships for correctness on non-x86 hardware.
/// </summary>
[MemoryDiagnoser]
public class ParallelBitExtractBenchmarks
{
    private uint _value = 0xDEADBEEF;
    private uint _mask = 0x0F0F0F0F;

    [Benchmark(Baseline = true)]
    public uint Intrinsic() { var v = _value; return Tedd.BitUtilsExtensions.ParallelBitExtract(ref v, _mask); }

    [Benchmark]
    public uint SoftwareFallback() => Tedd.BitUtilsExtensions.ParallelBitExtractSoftwareFallback(_value, _mask);
}
