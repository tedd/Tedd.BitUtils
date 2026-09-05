using System;
using System.Runtime.CompilerServices;

namespace Tedd;

/// <summary>
/// Unpacks bit fields of arbitrary width sequentially from a span of bytes, keeping track of the current bit position.
/// The counterpart of <see cref="BitWriter"/>.
/// </summary>
/// <remarks>
/// <para>Bit order is that of <see cref="BitPacking"/>: most significant bit first.</para>
/// <para>This is a <c>ref struct</c>, so it lives on the stack and can wrap a <c>stackalloc</c> buffer. Because it is
/// a mutable struct, pass it by <c>ref</c> when handing it to another method: a copy would advance its own position
/// and leave the caller behind.</para>
/// </remarks>
/// <example>
/// <code>
/// ReadOnlySpan&lt;byte&gt; packet = [0b101_11011];
/// var reader = new BitReader(packet);
/// var first = reader.Read(3);  // 0b101
/// var second = reader.Read(5); // 0b11011
/// </code>
/// </example>
public ref struct BitReader
{
    private readonly ReadOnlySpan<byte> _source;
    private long _bitPosition;

    /// <summary>Creates a reader positioned at the first bit of <paramref name="source"/>.</summary>
    /// <param name="source">Buffer to unpack from.</param>
    public BitReader(ReadOnlySpan<byte> source)
    {
        _source = source;
        _bitPosition = 0;
    }

    /// <summary>Bits read so far, which is also the bit offset the next read starts at.</summary>
    public readonly long BitPosition => _bitPosition;

    /// <summary>Bytes touched so far, that is <see cref="BitPosition"/> rounded up to a whole byte.</summary>
    public readonly int BytesRead => (int)((_bitPosition + 7) >> 3);

    /// <summary>Bits still unread in the buffer.</summary>
    public readonly long BitsRemaining => ((long)_source.Length << 3) - _bitPosition;

    /// <summary><see langword="true"/> when <see cref="BitPosition"/> sits on a byte boundary.</summary>
    public readonly bool IsByteAligned => (_bitPosition & 7) == 0;

    /// <summary>The buffer this reader unpacks from.</summary>
    public readonly ReadOnlySpan<byte> Buffer => _source;

    /// <summary>Reads a <paramref name="bitCount"/> bit field and advances the position.</summary>
    /// <param name="bitCount">Width of the field, 0 to 64 bits. A width of 0 reads 0.</param>
    /// <returns>The field, shifted down so its low bit lands at bit 0.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitCount"/> is outside 0 to 64.</exception>
    /// <exception cref="InvalidOperationException">Fewer than <paramref name="bitCount"/> bits remain in the buffer.</exception>
    public ulong Read(int bitCount)
    {
        if (!TryRead(bitCount, out var value))
            throw new InvalidOperationException($"Cannot read {bitCount} bits: only {BitsRemaining} bits remain in the buffer.");
        return value;
    }

    /// <summary>Reads a field, returning <see langword="false"/> instead of throwing when too few bits remain.</summary>
    /// <param name="bitCount">Width of the field, 0 to 64 bits.</param>
    /// <param name="value">The field, shifted down so its low bit lands at bit 0; 0 when this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when the field was read and the position advanced; <see langword="false"/> when the position is left where it was.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitCount"/> is outside 0 to 64.</exception>
    public bool TryRead(int bitCount, out ulong value)
    {
        if ((uint)bitCount > BitPacking.MaxFieldBits)
            throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, $"Field width must be between 0 and {BitPacking.MaxFieldBits} bits.");
        if (bitCount > BitsRemaining) { value = 0; return false; }
        value = BitPacking.ReadBitsCore(_source, _bitPosition, bitCount);
        _bitPosition += bitCount;
        return true;
    }

    /// <summary>Reads a single bit and advances the position by one.</summary>
    /// <returns><see langword="true"/> when the bit is set.</returns>
    /// <exception cref="InvalidOperationException">The buffer is exhausted.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBit()
    {
        if (!TryReadBit(out var state)) throw new InvalidOperationException("Cannot read a bit: the buffer is exhausted.");
        return state;
    }

    /// <summary>Reads a single bit, returning <see langword="false"/> instead of throwing when the buffer is exhausted.</summary>
    /// <param name="state">The bit read; <see langword="false"/> when this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when a bit was read.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadBit(out bool state)
    {
        if (BitsRemaining < 1) { state = false; return false; }
        state = BitPacking.ReadBit(_source, _bitPosition);
        _bitPosition++;
        return true;
    }

    /// <summary>Discards bits up to the next byte boundary, so the next read starts on a whole byte.</summary>
    /// <returns>The number of bits skipped, 0 to 7.</returns>
    /// <exception cref="InvalidOperationException">The padding runs past the end of the buffer.</exception>
    public int AlignToByte()
    {
        var padding = (int)(-_bitPosition & 7);
        if (padding == 0) return 0;
        Skip(padding);
        return padding;
    }

    /// <summary>Moves the read position forward without returning the skipped bits.</summary>
    /// <param name="bitCount">Number of bits to skip. Must not be negative.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitCount"/> is negative.</exception>
    /// <exception cref="InvalidOperationException">Skipping would move the position past the end of the buffer.</exception>
    public void Skip(long bitCount)
    {
        if (bitCount < 0) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Bit count must not be negative.");
        if (bitCount > BitsRemaining) throw new InvalidOperationException($"Cannot skip {bitCount} bits: only {BitsRemaining} bits remain in the buffer.");
        _bitPosition += bitCount;
    }

    /// <summary>Returns the position to the first bit of the buffer.</summary>
    public void Reset() => _bitPosition = 0;
}
