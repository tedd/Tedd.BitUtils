using System.Buffers.Binary;
using BenchmarkDotNet.Attributes;

namespace Tedd.BitUtils.Benchmarks;

/// <summary>
/// Candidate implementations of the BitPacking core, kept here rather than in the library so the current shipping
/// implementation and the proposal can be timed against each other in one process. Whatever wins gets ported into
/// Tedd.BitUtils and this file keeps the losing form around as the control.
/// </summary>
internal static class Candidate
{
    // Word-at-a-time read. The shipping ReadBitsCore walks the field one byte per iteration (up to nine for a 64 bit
    // field that straddles a byte boundary); this loads a single big-endian 64 bit window and extracts the field with
    // one shift and one mask. The window only works when 8 whole bytes are addressable from byteIndex AND the field
    // ends within those 64 bits, so the byte loop stays as the fallback for buffer tails and for the widest fields at
    // a non-zero bit offset.
    internal static ulong ReadBits(ReadOnlySpan<byte> source, long bitOffset, int bitCount)
    {
        if (bitCount == 0) return 0;
        var byteIndex = (int)(bitOffset >> 3);
        var used = (int)(bitOffset & 7);
        if (used + bitCount <= 8)
            return (uint)(source[byteIndex] >> (8 - used - bitCount)) & ((1u << bitCount) - 1);
        if (used + bitCount <= 64 && byteIndex + 8 <= source.Length)
        {
            var word = BinaryPrimitives.ReadUInt64BigEndian(source.Slice(byteIndex, 8));
            var mask = bitCount == 64 ? ulong.MaxValue : (1UL << bitCount) - 1;
            return (word >> (64 - used - bitCount)) & mask;
        }
        return ScalarRead(source, bitOffset, bitCount);
    }

    // Word-at-a-time read-modify-write, same window rule as ReadBits. Bits outside the field are read back out of the
    // buffer and written unchanged, so storing the whole 64 bit window is not a wider write than the byte loop makes.
    internal static void WriteBits(Span<byte> destination, long bitOffset, int bitCount, ulong value)
    {
        if (bitCount == 0) return;
        var byteIndex = (int)(bitOffset >> 3);
        var used = (int)(bitOffset & 7);
        if (used + bitCount <= 8)
        {
            var narrowMask = (1 << bitCount) - 1;
            var narrowShift = 8 - used - bitCount;
            ref var target = ref destination[byteIndex];
            target = (byte)((target & ~(narrowMask << narrowShift)) | (((int)value & narrowMask) << narrowShift));
            return;
        }
        if (used + bitCount <= 64 && byteIndex + 8 <= destination.Length)
        {
            var window = destination.Slice(byteIndex, 8);
            var valueMask = bitCount == 64 ? ulong.MaxValue : (1UL << bitCount) - 1;
            var shift = 64 - used - bitCount;
            var fieldMask = valueMask << shift;
            var word = BinaryPrimitives.ReadUInt64BigEndian(window);
            word = (word & ~fieldMask) | ((value & valueMask) << shift);
            BinaryPrimitives.WriteUInt64BigEndian(window, word);
            return;
        }
        ScalarWrite(destination, bitOffset, bitCount, value);
    }

    // Verbatim copies of the shipping implementation, so the benchmark measures both forms in the same process and
    // the fallback path above is the exact code the library is already tested against.
    internal static ulong ScalarRead(ReadOnlySpan<byte> source, long bitOffset, int bitCount)
    {
        ulong result = 0;
        var remaining = bitCount;
        while (remaining > 0)
        {
            var byteIndex = (int)(bitOffset >> 3);
            var used = (int)(bitOffset & 7);
            var available = 8 - used;
            var take = available < remaining ? available : remaining;
            var shift = available - take;
            var chunk = (uint)(source[byteIndex] >> shift) & ((1u << take) - 1);
            result = (result << take) | chunk;
            remaining -= take;
            bitOffset += take;
        }
        return result;
    }

    internal static void ScalarWrite(Span<byte> destination, long bitOffset, int bitCount, ulong value)
    {
        if (bitCount < 64) value &= (1UL << bitCount) - 1;
        var remaining = bitCount;
        while (remaining > 0)
        {
            var byteIndex = (int)(bitOffset >> 3);
            var used = (int)(bitOffset & 7);
            var free = 8 - used;
            var take = free < remaining ? free : remaining;
            var shift = free - take;
            var chunk = (int)((value >> (remaining - take)) & ((1UL << take) - 1));
            var keep = (byte)~(((1 << take) - 1) << shift);
            destination[byteIndex] = (byte)((destination[byteIndex] & keep) | (chunk << shift));
            remaining -= take;
            bitOffset += take;
        }
    }
}

/// <summary>
/// Hypothesis: BitPacking's core walks a field one byte per loop iteration, so cost scales with the number of bytes
/// the field touches (up to nine). A single big-endian 64 bit window load plus shift/mask should be flat in the field
/// width instead. Predicted: a large win for wide fields, shrinking towards neutral for fields of 8 bits or fewer,
/// where the existing loop already runs once or twice. Rejection: a repeatable regression on the narrow widths, since
/// those dominate real packet layouts.
/// </summary>
/// <remarks>
/// BitOffset is parameterised separately from BitCount because a field starting mid-byte touches one more byte than
/// the same field starting on a boundary - that is exactly the case the loop handles worst and the window handles
/// identically.
/// </remarks>
[MemoryDiagnoser]
public class BitPackingBenchmarks
{
    // 64 bytes: comfortably larger than the 8 byte window, so both the aligned and unaligned cases take the fast
    // path rather than measuring the buffer-tail fallback. Cache resident by construction.
    private readonly byte[] _buffer = new byte[64];

    [Params(1, 3, 8, 12, 24, 32, 57, 64)]
    public int BitCount { get; set; }

    [Params(0, 3)]
    public long BitOffset { get; set; }

    [GlobalSetup]
    public void Setup() => new Random(20260920).NextBytes(_buffer);

    [Benchmark(Baseline = true)]
    public ulong Scalar_Read() => Candidate.ScalarRead(_buffer, BitOffset, BitCount);

    [Benchmark]
    public ulong Window_Read() => Candidate.ReadBits(_buffer, BitOffset, BitCount);

    [Benchmark]
    public void Scalar_Write() => Candidate.ScalarWrite(_buffer, BitOffset, BitCount, 0xA5A5_5A5A_A5A5_5A5AUL);

    [Benchmark]
    public void Window_Write() => Candidate.WriteBits(_buffer, BitOffset, BitCount, 0xA5A5_5A5A_A5A5_5A5AUL);
}

/// <summary>
/// The complete-path control for <see cref="BitPackingBenchmarks"/>. A single field read is a microbenchmark; what a
/// caller actually does is pack or parse a whole packet through BitWriter/BitReader, which adds position tracking,
/// the range checks and the per-call validation on top of the core. A win on the core that disappears here is not a
/// win worth taking.
/// </summary>
[MemoryDiagnoser]
public class BitStreamBenchmarks
{
    private readonly byte[] _buffer = new byte[256];

    // A plausible mixed-width header: flags, small enums, medium ids, a couple of wide fields.
    private static readonly int[] Widths = [1, 1, 2, 4, 3, 5, 12, 7, 16, 9, 24, 11, 32, 13, 48, 6];

    [GlobalSetup]
    public void Setup() => new Random(20260920).NextBytes(_buffer);

    // The baseline: the same packet packed and parsed through the byte-at-a-time core, with the position tracking
    // BitWriter/BitReader do inlined here. Without this the shipping numbers below compare against nothing.
    [Benchmark(Baseline = true)]
    public int Scalar_Write_MixedPacket()
    {
        long position = 0;
        ulong value = 0x0123_4567_89AB_CDEF;
        foreach (var width in Widths)
        {
            Candidate.ScalarWrite(_buffer, position, width, value++);
            position += width;
        }
        return (int)((position + 7) >> 3);
    }

    [Benchmark]
    public ulong Scalar_Read_MixedPacket()
    {
        long position = 0;
        ulong sink = 0;
        foreach (var width in Widths)
        {
            sink ^= Candidate.ScalarRead(_buffer, position, width);
            position += width;
        }
        return sink;
    }

    [Benchmark]
    public int Write_MixedPacket()
    {
        var writer = new BitWriter(_buffer);
        ulong value = 0x0123_4567_89AB_CDEF;
        foreach (var width in Widths) writer.Write(value++, width);
        return writer.BytesWritten;
    }

    [Benchmark]
    public ulong Read_MixedPacket()
    {
        var reader = new BitReader(_buffer);
        ulong sink = 0;
        foreach (var width in Widths) sink ^= reader.Read(width);
        return sink;
    }
}

/// <summary>
/// Hypothesis: EbmlVInt.TryRead finds the encoding length with a data-dependent loop that shifts a marker bit right
/// one position at a time (one to eight iterations), then assembles the big-endian payload one byte at a time. The
/// length is exactly what LeadingZeroCount of the first byte reports, which is a single LZCNT on this CPU, and the
/// payload is a big-endian load. Predicted: a win that grows with encoding length. Rejection: a regression on the one
/// byte encoding, which is the overwhelmingly common case in a real Matroska stream.
/// </summary>
[MemoryDiagnoser]
public class EbmlVIntBenchmarks
{
    private readonly byte[] _buffer = new byte[16];

    // 1, 2, 4 and 8 byte encodings.
    [Params(0UL, 200UL, 100_000UL, 1UL << 50)]
    public ulong Value { get; set; }

    [GlobalSetup]
    public void Setup() => EbmlVInt.Write(_buffer, Value);

    [Benchmark(Baseline = true)]
    public int Scan_Read()
    {
        // Verbatim copy of the shipping length scan and payload assembly.
        ReadOnlySpan<byte> source = _buffer;
        var first = source[0];
        var marker = 0x80;
        var length = 1;
        while ((first & marker) == 0) { marker >>= 1; length++; }
        var payload = (ulong)(first & (marker - 1));
        for (var index = 1; index < length; index++) payload = (payload << 8) | source[index];
        return (int)payload + length;
    }

    [Benchmark]
    public int Lzcnt_Read()
    {
        ReadOnlySpan<byte> source = _buffer;
        var first = source[0];
        // A byte widened to uint has 24 leading zeros before its own bits, so subtracting them leaves the position of
        // the marker within the byte; length is that position plus one.
        var length = System.Numerics.BitOperations.LeadingZeroCount(first) - 24 + 1;
        var marker = 0x80 >> (length - 1);
        var payload = (ulong)(first & (marker - 1));
        for (var index = 1; index < length; index++) payload = (payload << 8) | source[index];
        return (int)payload + length;
    }

    // The shipped implementation, measured through its public entry point rather than a copy of it.
    [Benchmark]
    public int Shipped_Read()
    {
        var v = EbmlVInt.Read(_buffer);
        return (int)v.Value + v.Length;
    }

    [Benchmark]
    public int LzcntWindow_Read()
    {
        ReadOnlySpan<byte> source = _buffer;
        var first = source[0];
        var length = System.Numerics.BitOperations.LeadingZeroCount(first) - 24 + 1;
        // One big-endian load covers every legal encoding (at most eight bytes), then the unused low bytes are
        // shifted off and the length marker masked away.
        var word = BinaryPrimitives.ReadUInt64BigEndian(source.Slice(0, 8));
        var payload = (word >> ((8 - length) << 3)) & ((1UL << (7 * length)) - 1);
        return (int)payload + length;
    }
}
