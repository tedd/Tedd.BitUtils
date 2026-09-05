using System;
using System.Runtime.CompilerServices;

namespace Tedd;

/// <summary>
/// Packs bit fields of arbitrary width sequentially into a span of bytes, keeping track of the current bit position.
/// </summary>
/// <remarks>
/// <para>Bit order and read-modify-write semantics are those of <see cref="BitPacking"/>: most significant bit first,
/// and only the bits actually written are touched, so the destination needs no zeroing beforehand.</para>
/// <para>This is a <c>ref struct</c>, so it lives on the stack and can wrap a <c>stackalloc</c> buffer. Because it is
/// a mutable struct, pass it by <c>ref</c> when handing it to another method: a copy would advance its own position
/// and leave the caller behind.</para>
/// </remarks>
/// <example>
/// <code>
/// Span&lt;byte&gt; buffer = stackalloc byte[4];
/// var writer = new BitWriter(buffer);
/// writer.Write(0b101, 3);
/// writer.Write(0b11011, 5);
/// var packet = buffer[..writer.BytesWritten]; // one byte: 0b101_11011
/// </code>
/// </example>
public ref struct BitWriter
{
    private readonly Span<byte> _destination;
    private long _bitPosition;

    /// <summary>Creates a writer positioned at the first bit of <paramref name="destination"/>.</summary>
    /// <param name="destination">Buffer to pack into.</param>
    public BitWriter(Span<byte> destination)
    {
        _destination = destination;
        _bitPosition = 0;
    }

    /// <summary>Bits written so far, which is also the bit offset the next write starts at.</summary>
    public readonly long BitPosition => _bitPosition;

    /// <summary>Bytes touched so far, that is <see cref="BitPosition"/> rounded up to a whole byte.</summary>
    public readonly int BytesWritten => (int)((_bitPosition + 7) >> 3);

    /// <summary>Bits still available in the buffer.</summary>
    public readonly long BitsRemaining => ((long)_destination.Length << 3) - _bitPosition;

    /// <summary><see langword="true"/> when <see cref="BitPosition"/> sits on a byte boundary.</summary>
    public readonly bool IsByteAligned => (_bitPosition & 7) == 0;

    /// <summary>The buffer this writer packs into.</summary>
    public readonly Span<byte> Buffer => _destination;

    /// <summary>The bytes written so far, that is <see cref="Buffer"/> truncated to <see cref="BytesWritten"/>.</summary>
    public readonly Span<byte> WrittenSpan => _destination.Slice(0, BytesWritten);

    /// <summary>Writes the <paramref name="bitCount"/> least significant bits of <paramref name="value"/> and advances the position.</summary>
    /// <param name="value">Value to write; only its <paramref name="bitCount"/> least significant bits are used.</param>
    /// <param name="bitCount">Width of the field, 0 to 64 bits. A width of 0 writes nothing.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitCount"/> is outside 0 to 64.</exception>
    /// <exception cref="InvalidOperationException">The field does not fit in the remaining buffer.</exception>
    public void Write(ulong value, int bitCount)
    {
        if (!TryWrite(value, bitCount))
            throw new InvalidOperationException($"Cannot write {bitCount} bits: only {BitsRemaining} bits remain in the buffer.");
    }

    /// <summary>Writes a field, returning <see langword="false"/> instead of throwing when it does not fit in the remaining buffer.</summary>
    /// <param name="value">Value to write; only its <paramref name="bitCount"/> least significant bits are used.</param>
    /// <param name="bitCount">Width of the field, 0 to 64 bits.</param>
    /// <returns><see langword="true"/> when the field was written and the position advanced; <see langword="false"/> when the buffer is left untouched.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitCount"/> is outside 0 to 64.</exception>
    public bool TryWrite(ulong value, int bitCount)
    {
        if ((uint)bitCount > BitPacking.MaxFieldBits)
            throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, $"Field width must be between 0 and {BitPacking.MaxFieldBits} bits.");
        if (bitCount > BitsRemaining) return false;
        BitPacking.WriteBitsCore(_destination, _bitPosition, bitCount, value);
        _bitPosition += bitCount;
        return true;
    }

    /// <summary>Writes a single bit and advances the position by one.</summary>
    /// <param name="state">Bit to write.</param>
    /// <exception cref="InvalidOperationException">The buffer is full.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBit(bool state)
    {
        if (!TryWriteBit(state)) throw new InvalidOperationException("Cannot write a bit: the buffer is full.");
    }

    /// <summary>Writes a single bit, returning <see langword="false"/> instead of throwing when the buffer is full.</summary>
    /// <param name="state">Bit to write.</param>
    /// <returns><see langword="true"/> when the bit was written.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryWriteBit(bool state)
    {
        if (BitsRemaining < 1) return false;
        BitPacking.WriteBit(_destination, _bitPosition, state);
        _bitPosition++;
        return true;
    }

    /// <summary>Pads with zero bits up to the next byte boundary, so the next write starts on a whole byte.</summary>
    /// <returns>The number of padding bits written, 0 to 7.</returns>
    /// <exception cref="InvalidOperationException">The padding does not fit in the remaining buffer.</exception>
    public int AlignToByte()
    {
        var padding = (int)(-_bitPosition & 7);
        if (padding == 0) return 0;
        Write(0, padding);
        return padding;
    }

    /// <summary>Moves the write position without touching the buffer, leaving the skipped bits as they are.</summary>
    /// <param name="bitCount">Number of bits to skip. Must not be negative.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitCount"/> is negative.</exception>
    /// <exception cref="InvalidOperationException">Skipping would move the position past the end of the buffer.</exception>
    public void Skip(long bitCount)
    {
        if (bitCount < 0) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Bit count must not be negative.");
        if (bitCount > BitsRemaining) throw new InvalidOperationException($"Cannot skip {bitCount} bits: only {BitsRemaining} bits remain in the buffer.");
        _bitPosition += bitCount;
    }

    /// <summary>Returns the position to the first bit of the buffer. The buffer contents are left as they are.</summary>
    public void Reset() => _bitPosition = 0;
}
