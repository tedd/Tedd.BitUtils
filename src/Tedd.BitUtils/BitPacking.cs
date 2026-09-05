using System;
using System.Runtime.CompilerServices;

namespace Tedd;

/// <summary>
/// Packs and unpacks bit fields of arbitrary width (0 to 64 bits) at arbitrary bit offsets in a span of bytes,
/// without regard for byte boundaries.
/// </summary>
/// <remarks>
/// <para>Bits are laid out most significant bit first: bit offset 0 is the <em>highest</em> bit (0x80) of byte 0,
/// offset 7 is the lowest bit (0x01) of byte 0, and offset 8 is the highest bit of byte 1. A field is written most
/// significant bit first too, so a 12 bit field written at offset 0 occupies all 8 bits of byte 0 and the top 4 bits
/// of byte 1. This is the bit order used by most binary wire formats (EBML, MPEG, JPEG, most network protocols); it
/// is the opposite of the one Deflate uses, which fills each byte from its low bit up.</para>
/// <para>Writes are read-modify-write: only the bits of the target field are touched, so the destination does not
/// need to be zeroed first and neighbouring fields are preserved. Only the <c>bitCount</c> least significant bits of
/// a value are stored; any higher bits are ignored rather than corrupting the next field.</para>
/// <para>Unlike the extension methods in <see cref="BitUtilsExtensions"/>, which never validate, these methods range
/// check their arguments and throw. Use the <c>Try</c> overloads to avoid exceptions on a buffer that is too short.</para>
/// <para>Bit offsets are <see cref="long"/> because a span can hold up to <see cref="int.MaxValue"/> bytes, which is
/// eight times more bits than an <see cref="int"/> offset could address.</para>
/// </remarks>
/// <example>
/// <code>
/// Span&lt;byte&gt; buffer = stackalloc byte[2];
/// BitPacking.WriteBits(buffer, 0, 3, 0b101);     // buffer[0] == 0b101_00000
/// BitPacking.WriteBits(buffer, 3, 5, 0b11011);   // buffer[0] == 0b101_11011
/// var field = BitPacking.ReadBits(buffer, 3, 5); // 0b11011
/// </code>
/// </example>
public static class BitPacking
{
    /// <summary>Widest field these methods can read or write in a single call, in bits.</summary>
    public const int MaxFieldBits = 64;

    /// <summary>Returns the number of whole bytes needed to hold <paramref name="bitCount"/> bits, rounding up.</summary>
    /// <param name="bitCount">Number of bits. Must not be negative.</param>
    /// <returns>The byte count.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitCount"/> is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ByteCount(long bitCount)
    {
        if (bitCount < 0) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, "Bit count must not be negative.");
        return (bitCount + 7) >> 3;
    }

    /// <summary>Writes the <paramref name="bitCount"/> least significant bits of <paramref name="value"/> into <paramref name="destination"/>, most significant bit first, starting at <paramref name="bitOffset"/>.</summary>
    /// <param name="destination">Buffer to write into. Bits outside the target field are left untouched.</param>
    /// <param name="bitOffset">Bit position of the field, counted most significant bit first from the start of the buffer.</param>
    /// <param name="bitCount">Width of the field, 0 to 64 bits. A width of 0 writes nothing.</param>
    /// <param name="value">Value to write; only its <paramref name="bitCount"/> least significant bits are used.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitOffset"/> is negative, or <paramref name="bitCount"/> is outside 0 to 64.</exception>
    /// <exception cref="ArgumentException">The field does not fit in <paramref name="destination"/>.</exception>
    public static void WriteBits(Span<byte> destination, long bitOffset, int bitCount, ulong value)
    {
        ValidateRange(bitOffset, bitCount);
        if (!FitsIn(destination.Length, bitOffset, bitCount))
            throw new ArgumentException($"A {bitCount} bit field at bit offset {bitOffset} does not fit in {destination.Length} bytes.", nameof(destination));
        WriteBitsCore(destination, bitOffset, bitCount, value);
    }

    /// <summary>Writes a bit field, returning <see langword="false"/> instead of throwing when it does not fit in <paramref name="destination"/>.</summary>
    /// <param name="destination">Buffer to write into. Left untouched when this returns <see langword="false"/>.</param>
    /// <param name="bitOffset">Bit position of the field, counted most significant bit first from the start of the buffer.</param>
    /// <param name="bitCount">Width of the field, 0 to 64 bits.</param>
    /// <param name="value">Value to write; only its <paramref name="bitCount"/> least significant bits are used.</param>
    /// <returns><see langword="true"/> when the field was written.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitOffset"/> is negative, or <paramref name="bitCount"/> is outside 0 to 64.</exception>
    public static bool TryWriteBits(Span<byte> destination, long bitOffset, int bitCount, ulong value)
    {
        ValidateRange(bitOffset, bitCount);
        if (!FitsIn(destination.Length, bitOffset, bitCount)) return false;
        WriteBitsCore(destination, bitOffset, bitCount, value);
        return true;
    }

    /// <summary>Reads a <paramref name="bitCount"/> bit field from <paramref name="source"/>, most significant bit first, starting at <paramref name="bitOffset"/>.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="bitOffset">Bit position of the field, counted most significant bit first from the start of the buffer.</param>
    /// <param name="bitCount">Width of the field, 0 to 64 bits. A width of 0 reads 0.</param>
    /// <returns>The field, shifted down so its low bit lands at bit 0.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitOffset"/> is negative, or <paramref name="bitCount"/> is outside 0 to 64.</exception>
    /// <exception cref="ArgumentException">The field does not fit in <paramref name="source"/>.</exception>
    public static ulong ReadBits(ReadOnlySpan<byte> source, long bitOffset, int bitCount)
    {
        ValidateRange(bitOffset, bitCount);
        if (!FitsIn(source.Length, bitOffset, bitCount))
            throw new ArgumentException($"A {bitCount} bit field at bit offset {bitOffset} does not fit in {source.Length} bytes.", nameof(source));
        return ReadBitsCore(source, bitOffset, bitCount);
    }

    /// <summary>Reads a bit field, returning <see langword="false"/> instead of throwing when it does not fit in <paramref name="source"/>.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="bitOffset">Bit position of the field, counted most significant bit first from the start of the buffer.</param>
    /// <param name="bitCount">Width of the field, 0 to 64 bits.</param>
    /// <param name="value">The field, shifted down so its low bit lands at bit 0; 0 when this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when the field was read.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitOffset"/> is negative, or <paramref name="bitCount"/> is outside 0 to 64.</exception>
    public static bool TryReadBits(ReadOnlySpan<byte> source, long bitOffset, int bitCount, out ulong value)
    {
        ValidateRange(bitOffset, bitCount);
        if (!FitsIn(source.Length, bitOffset, bitCount)) { value = 0; return false; }
        value = ReadBitsCore(source, bitOffset, bitCount);
        return true;
    }

    /// <summary>Writes a single bit at <paramref name="bitOffset"/>, leaving every other bit untouched.</summary>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="bitOffset">Bit position, counted most significant bit first from the start of the buffer.</param>
    /// <param name="state">New state of the bit.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitOffset"/> is negative or past the end of <paramref name="destination"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBit(Span<byte> destination, long bitOffset, bool state)
    {
        if (bitOffset < 0 || bitOffset >= (long)destination.Length << 3)
            throw new ArgumentOutOfRangeException(nameof(bitOffset), bitOffset, "Bit offset is outside the buffer.");
        var mask = 0x80 >> (int)(bitOffset & 7);
        ref var target = ref destination[(int)(bitOffset >> 3)];
        target = (byte)(state ? target | mask : target & ~mask);
    }

    /// <summary>Reads a single bit at <paramref name="bitOffset"/>.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="bitOffset">Bit position, counted most significant bit first from the start of the buffer.</param>
    /// <returns><see langword="true"/> when the bit is set.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitOffset"/> is negative or past the end of <paramref name="source"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ReadBit(ReadOnlySpan<byte> source, long bitOffset)
    {
        if (bitOffset < 0 || bitOffset >= (long)source.Length << 3)
            throw new ArgumentOutOfRangeException(nameof(bitOffset), bitOffset, "Bit offset is outside the buffer.");
        return (source[(int)(bitOffset >> 3)] & (0x80 >> (int)(bitOffset & 7))) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateRange(long bitOffset, int bitCount)
    {
        if (bitOffset < 0) throw new ArgumentOutOfRangeException(nameof(bitOffset), bitOffset, "Bit offset must not be negative.");
        if ((uint)bitCount > MaxFieldBits) throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, $"Field width must be between 0 and {MaxFieldBits} bits.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool FitsIn(int byteLength, long bitOffset, int bitCount) => bitOffset + bitCount <= (long)byteLength << 3;

    // Splits the field at byte boundaries and writes it one chunk at a time, high chunk first. At most nine
    // iterations (a 64 bit field straddling a byte boundary), and one or two for the field widths that dominate.
    internal static void WriteBitsCore(Span<byte> destination, long bitOffset, int bitCount, ulong value)
    {
        if (bitCount < MaxFieldBits) value &= (1UL << bitCount) - 1;
        var remaining = bitCount;
        while (remaining > 0)
        {
            var byteIndex = (int)(bitOffset >> 3);
            var used = (int)(bitOffset & 7);        // bits of this byte already consumed, counted from its MSB
            var free = 8 - used;
            var take = free < remaining ? free : remaining;
            var shift = free - take;                // where the chunk sits within the byte
            var chunk = (int)((value >> (remaining - take)) & ((1UL << take) - 1));
            var keep = (byte)~(((1 << take) - 1) << shift);
            destination[byteIndex] = (byte)((destination[byteIndex] & keep) | (chunk << shift));
            remaining -= take;
            bitOffset += take;
        }
    }

    internal static ulong ReadBitsCore(ReadOnlySpan<byte> source, long bitOffset, int bitCount)
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
            result = (result << take) | (ulong)((source[byteIndex] >> shift) & ((1 << take) - 1));
            remaining -= take;
            bitOffset += take;
        }
        return result;
    }
}
