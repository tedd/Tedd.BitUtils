using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tedd;

/// <summary>
/// Byte oriented variable length integer encodings, where small values take fewer bytes than large ones. All four
/// encodings here split the value into 7 bit groups, least significant group first, and set the high bit of every
/// byte but the last; they differ only in how the sign is carried.
/// </summary>
/// <remarks>
/// <para><b>Unsigned (ULEB128).</b> <see cref="WriteUnsigned"/>/<see cref="ReadUnsigned"/>. Plain 7 bit groups, no
/// sign. Values below 128 take one byte and a full 64 bit value takes ten. This is LEB128 as used by DWARF and
/// WebAssembly, and is byte for byte the same as the Protocol Buffers <c>uint32</c>/<c>uint64</c> wire encoding.</para>
/// <para><b>Signed (SLEB128).</b> <see cref="WriteSigned"/>/<see cref="ReadSigned"/>. Two's complement, sign extended
/// from bit 6 of the final byte. This is the standard signed LEB128 of DWARF and WebAssembly. Every value of the
/// width round trips, including the most negative one, and the size follows the magnitude, so -1 is one byte.</para>
/// <para><b>ZigZag.</b> <see cref="WriteZigZag"/>/<see cref="ReadZigZag"/>. The value is first folded onto an
/// unsigned one with <see cref="BitUtilsExtensions.ZigZagEncode(ref long)"/>, then written as ULEB128. This is what
/// Protocol Buffers calls <c>sint32</c>/<c>sint64</c>. It is <em>not</em> more compact than <see cref="WriteSigned"/>:
/// the two produce the same number of bytes for every single value, since folding costs exactly the one bit that
/// sign extension would have. Choose between them by what the other side speaks, not by size.</para>
/// <para><b>Sign-magnitude.</b> <see cref="WriteSignMagnitude"/>/<see cref="ReadSignMagnitude"/>. <b>Not an industry
/// standard</b> - this is the format Tedd.SpanUtils has written since 1.x, kept here so that data already on disk
/// stays readable. The first byte carries the sign in bit 6 (0x40) and six magnitude bits; later bytes carry seven
/// each. The one value it cannot express is the most negative one for the width, whose magnitude is one larger than
/// the most positive, so that value is written as the single byte 0x40 ("negative zero"), which is otherwise unused.
/// The sentinel is width dependent, so the <c>bits</c> argument has to match between writing and reading: -32768
/// written with <c>bits: 16</c> encodes as 0x40, while the same value written with <c>bits: 64</c> encodes as its
/// ordinary three byte form. For new formats use <see cref="WriteZigZag"/> instead, which is standard, has no
/// sentinel and is width independent.</para>
/// <para>The <c>bits</c> argument on the read methods bounds the value the caller is willing to accept (8 for a byte,
/// 16 for a short, and so on) and turns a value too large for that width into an <see cref="OverflowException"/>
/// rather than a silently truncated result. It also bounds how far a malformed input can be followed before the read
/// gives up.</para>
/// </remarks>
public static class VarInt
{
    /// <summary>Longest encoding any of these formats can produce or consume, in bytes, reached only by 64 bit values.</summary>
    public const int MaxLength = 10;

    #region Unsigned (ULEB128)
    /// <summary>Returns the number of bytes <see cref="WriteUnsigned"/> will use for <paramref name="value"/>.</summary>
    /// <param name="value">Value to measure.</param>
    /// <returns>The encoded length in bytes, 1 to 10.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MeasureUnsigned(ulong value) => (64 - BitOperations.LeadingZeroCount(value | 1UL) + 6) / 7;

    /// <summary>Writes <paramref name="value"/> as an unsigned LEB128 variable length integer.</summary>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="value">Value to write.</param>
    /// <returns>The number of bytes written, 1 to 10.</returns>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is too short.</exception>
    public static int WriteUnsigned(Span<byte> destination, ulong value)
    {
        if (!TryWriteUnsigned(destination, value, out var bytesWritten))
            throw new ArgumentException(TooShortMessage(destination.Length, MeasureUnsigned(value), value), nameof(destination));
        return bytesWritten;
    }

    /// <summary>Writes an unsigned LEB128 variable length integer, returning <see langword="false"/> instead of throwing when <paramref name="destination"/> is too short.</summary>
    /// <param name="destination">Buffer to write into. Left untouched when this returns <see langword="false"/>.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="bytesWritten">Number of bytes written; 0 when this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when the value was written.</returns>
    public static bool TryWriteUnsigned(Span<byte> destination, ulong value, out int bytesWritten)
    {
        bytesWritten = 0;
        var length = MeasureUnsigned(value);
        if (destination.Length < length) return false;
        var index = 0;
        while (value >= 0x80)
        {
            destination[index++] = (byte)(value | 0x80);
            value >>= 7;
        }
        destination[index] = (byte)value;
        bytesWritten = length;
        return true;
    }

    /// <summary>Reads an unsigned LEB128 variable length integer.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="bytesRead">Number of bytes consumed.</param>
    /// <param name="bits">Largest width the caller accepts, 1 to 64 bits.</param>
    /// <returns>The decoded value.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 1 to 64.</exception>
    /// <exception cref="OverflowException">The encoded value does not fit in <paramref name="bits"/> bits.</exception>
    /// <exception cref="EndOfStreamException">The encoding is cut short by the end of <paramref name="source"/>.</exception>
    public static ulong ReadUnsigned(ReadOnlySpan<byte> source, out int bytesRead, int bits = 64)
    {
        if (TryReadUnsigned(source, out var value, out bytesRead, out var overflow, bits)) return value;
        throw Failure(overflow, bits);
    }

    /// <summary>Reads an unsigned LEB128 variable length integer, returning <see langword="false"/> instead of throwing on a truncated or out of range encoding.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="value">The decoded value; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bytesRead">Number of bytes consumed; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bits">Largest width the caller accepts, 1 to 64 bits.</param>
    /// <returns><see langword="true"/> when a value was decoded.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 1 to 64.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadUnsigned(ReadOnlySpan<byte> source, out ulong value, out int bytesRead, int bits = 64) =>
        TryReadUnsigned(source, out value, out bytesRead, out _, bits);

    /// <summary>Reads an unsigned LEB128 variable length integer, reporting separately whether the failure was an out of range value or a truncated encoding.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="value">The decoded value; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bytesRead">Number of bytes consumed; 0 when this returns <see langword="false"/>.</param>
    /// <param name="overflow"><see langword="true"/> when the failure was a value too large for <paramref name="bits"/>, <see langword="false"/> when the encoding ran off the end of <paramref name="source"/>.</param>
    /// <param name="bits">Largest width the caller accepts, 1 to 64 bits.</param>
    /// <returns><see langword="true"/> when a value was decoded.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 1 to 64.</exception>
    public static bool TryReadUnsigned(ReadOnlySpan<byte> source, out ulong value, out int bytesRead, out bool overflow, int bits = 64)
    {
        ValidateBits(bits, 1);
        value = 0; bytesRead = 0; overflow = false;
        if (source.IsEmpty) return false;
        var maximum = bits == 64 ? ulong.MaxValue : (1UL << bits) - 1;
        var first = source[0];
        if (first < 0x80)
        {
            if (first > maximum) { overflow = true; return false; }
            value = first; bytesRead = 1; return true;
        }
        var maxBytes = (bits + 6) / 7;
        ulong result = 0;
        for (var index = 0; index < maxBytes; index++)
        {
            if (index >= source.Length) return false;
            var current = source[index];
            var payload = (ulong)(current & 0x7F);
            var shift = index * 7;
            if (payload > (maximum >> shift)) { overflow = true; return false; }
            result |= payload << shift;
            if ((current & 0x80) == 0) { value = result; bytesRead = index + 1; return true; }
        }
        overflow = true;
        return false;
    }
    #endregion

    #region Signed (SLEB128)
    /// <summary>Returns the number of bytes <see cref="WriteSigned"/> will use for <paramref name="value"/>.</summary>
    /// <param name="value">Value to measure.</param>
    /// <returns>The encoded length in bytes, 1 to 10.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MeasureSigned(long value)
    {
        // Bits needed for the magnitude relative to the sign, plus one for the sign itself, rounded up to 7 bit
        // groups. Complementing a negative value gives the same count for -n as for n - 1, which is what two's
        // complement sign extension needs: -64 fits in 7 bits, -65 does not.
        var magnitude = (ulong)(value < 0 ? ~value : value);
        return (64 - BitOperations.LeadingZeroCount(magnitude) + 1 + 6) / 7;
    }

    /// <summary>Writes <paramref name="value"/> as a signed LEB128 variable length integer (two's complement, sign extended).</summary>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="value">Value to write.</param>
    /// <returns>The number of bytes written, 1 to 10.</returns>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is too short.</exception>
    public static int WriteSigned(Span<byte> destination, long value)
    {
        if (!TryWriteSigned(destination, value, out var bytesWritten))
            throw new ArgumentException(TooShortMessage(destination.Length, MeasureSigned(value), value), nameof(destination));
        return bytesWritten;
    }

    /// <summary>Writes a signed LEB128 variable length integer, returning <see langword="false"/> instead of throwing when <paramref name="destination"/> is too short.</summary>
    /// <param name="destination">Buffer to write into. Left untouched when this returns <see langword="false"/>.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="bytesWritten">Number of bytes written; 0 when this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when the value was written.</returns>
    public static bool TryWriteSigned(Span<byte> destination, long value, out int bytesWritten)
    {
        bytesWritten = 0;
        var length = MeasureSigned(value);
        if (destination.Length < length) return false;
        var index = 0;
        while (true)
        {
            var current = (byte)(value & 0x7F);
            value >>= 7;        // arithmetic, so the sign floods in from the left
            // Done once what is left is pure sign extension of the byte just emitted.
            if ((value == 0 && (current & 0x40) == 0) || (value == -1 && (current & 0x40) != 0))
            {
                destination[index++] = current;
                break;
            }
            destination[index++] = (byte)(current | 0x80);
        }
        bytesWritten = index;
        return true;
    }

    /// <summary>Reads a signed LEB128 variable length integer (two's complement, sign extended).</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="bytesRead">Number of bytes consumed.</param>
    /// <param name="bits">Largest width the caller accepts, 2 to 64 bits.</param>
    /// <returns>The decoded value.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 2 to 64.</exception>
    /// <exception cref="OverflowException">The encoded value does not fit in <paramref name="bits"/> bits.</exception>
    /// <exception cref="EndOfStreamException">The encoding is cut short by the end of <paramref name="source"/>.</exception>
    public static long ReadSigned(ReadOnlySpan<byte> source, out int bytesRead, int bits = 64)
    {
        if (TryReadSigned(source, out var value, out bytesRead, out var overflow, bits)) return value;
        throw Failure(overflow, bits);
    }

    /// <summary>Reads a signed LEB128 variable length integer, returning <see langword="false"/> instead of throwing on a truncated or out of range encoding.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="value">The decoded value; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bytesRead">Number of bytes consumed; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bits">Largest width the caller accepts, 2 to 64 bits.</param>
    /// <returns><see langword="true"/> when a value was decoded.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 2 to 64.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadSigned(ReadOnlySpan<byte> source, out long value, out int bytesRead, int bits = 64) =>
        TryReadSigned(source, out value, out bytesRead, out _, bits);

    /// <summary>Reads a signed LEB128 variable length integer, reporting separately whether the failure was an out of range value or a truncated encoding.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="value">The decoded value; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bytesRead">Number of bytes consumed; 0 when this returns <see langword="false"/>.</param>
    /// <param name="overflow"><see langword="true"/> when the failure was a value too large for <paramref name="bits"/>, <see langword="false"/> when the encoding ran off the end of <paramref name="source"/>.</param>
    /// <param name="bits">Largest width the caller accepts, 2 to 64 bits.</param>
    /// <returns><see langword="true"/> when a value was decoded.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 2 to 64.</exception>
    public static bool TryReadSigned(ReadOnlySpan<byte> source, out long value, out int bytesRead, out bool overflow, int bits = 64)
    {
        ValidateBits(bits, 2);
        value = 0; bytesRead = 0; overflow = false;
        // A b bit value needs at most ceil(b / 7) bytes, since SLEB128 carries the sign inside those 7 bit groups.
        var maxBytes = (bits + 6) / 7;
        long result = 0;
        var shift = 0;
        for (var index = 0; index < maxBytes; index++)
        {
            if (index >= source.Length) return false;
            var current = source[index];
            var payload = (ulong)(current & 0x7F);
            // The last byte of a 64 bit value contributes a single bit, so its remaining six must already be the
            // sign extension of that bit: 0x00 or 0x7F. Anything else carries bits that do not fit and is malformed.
            if (shift == 63 && payload != 0 && payload != 0x7F) { overflow = true; return false; }
            result |= (long)(payload << shift);
            shift += 7;
            if ((current & 0x80) != 0) continue;
            if (shift < 64 && (current & 0x40) != 0) result |= -1L << shift;
            if (!FitsInBits(result, bits)) { overflow = true; return false; }
            value = result;
            bytesRead = index + 1;
            return true;
        }
        overflow = true;
        return false;
    }
    #endregion

    #region ZigZag
    /// <summary>Returns the number of bytes <see cref="WriteZigZag"/> will use for <paramref name="value"/>.</summary>
    /// <param name="value">Value to measure.</param>
    /// <returns>The encoded length in bytes, 1 to 10.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MeasureZigZag(long value) => MeasureUnsigned(value.ZigZagEncode());

    /// <summary>Writes <paramref name="value"/> ZigZag folded, then as an unsigned LEB128 variable length integer.</summary>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="value">Value to write.</param>
    /// <returns>The number of bytes written, 1 to 10.</returns>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is too short.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteZigZag(Span<byte> destination, long value) => WriteUnsigned(destination, value.ZigZagEncode());

    /// <summary>Writes a ZigZag folded variable length integer, returning <see langword="false"/> instead of throwing when <paramref name="destination"/> is too short.</summary>
    /// <param name="destination">Buffer to write into. Left untouched when this returns <see langword="false"/>.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="bytesWritten">Number of bytes written; 0 when this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when the value was written.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryWriteZigZag(Span<byte> destination, long value, out int bytesWritten) =>
        TryWriteUnsigned(destination, value.ZigZagEncode(), out bytesWritten);

    /// <summary>Reads a ZigZag folded variable length integer.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="bytesRead">Number of bytes consumed.</param>
    /// <param name="bits">Width of the signed value, 1 to 64 bits. ZigZag folds an n bit signed value into n unsigned bits, so this is the same n used when writing.</param>
    /// <returns>The decoded value.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 1 to 64.</exception>
    /// <exception cref="OverflowException">The encoded value does not fit in <paramref name="bits"/> bits.</exception>
    /// <exception cref="EndOfStreamException">The encoding is cut short by the end of <paramref name="source"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReadZigZag(ReadOnlySpan<byte> source, out int bytesRead, int bits = 64)
    {
        var encoded = ReadUnsigned(source, out bytesRead, bits);
        return encoded.ZigZagDecode();
    }

    /// <summary>Reads a ZigZag folded variable length integer, returning <see langword="false"/> instead of throwing on a truncated or out of range encoding.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="value">The decoded value; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bytesRead">Number of bytes consumed; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bits">Width of the signed value, 1 to 64 bits.</param>
    /// <returns><see langword="true"/> when a value was decoded.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 1 to 64.</exception>
    public static bool TryReadZigZag(ReadOnlySpan<byte> source, out long value, out int bytesRead, int bits = 64)
    {
        if (!TryReadUnsigned(source, out var encoded, out bytesRead, bits)) { value = 0; return false; }
        value = encoded.ZigZagDecode();
        return true;
    }
    #endregion

    #region Sign-magnitude (Tedd.SpanUtils legacy format)
    /// <summary>Returns the number of bytes <see cref="WriteSignMagnitude"/> will use for <paramref name="value"/>.</summary>
    /// <param name="value">Value to measure.</param>
    /// <param name="bits">Width of the signed value, 2 to 64 bits. Selects which value is the most negative one and so encodes as the single byte sentinel.</param>
    /// <returns>The encoded length in bytes, 1 to 10.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 2 to 64, or <paramref name="value"/> does not fit in <paramref name="bits"/> bits.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MeasureSignMagnitude(long value, int bits = 64)
    {
        var minimum = SignMagnitudeMinimum(bits);
        ValidateSignedRange(value, bits, minimum);
        if (value == minimum) return 1;
        var magnitude = (ulong)(value < 0 ? -value : value);
        // One bit more than the magnitude needs, for the sign, rounded up to whole 7 bit groups. The first byte
        // holds six payload bits and every later byte seven, which is what "(bitlength + 1 + 6) / 7" comes out as.
        return (65 - BitOperations.LeadingZeroCount(magnitude | 1UL) + 6) / 7;
    }

    /// <summary>Writes <paramref name="value"/> in the non-standard sign-magnitude variable length format used by Tedd.SpanUtils. Prefer <see cref="WriteZigZag"/> for new formats.</summary>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="bits">Width of the signed value, 2 to 64 bits. Must match the width used when reading.</param>
    /// <returns>The number of bytes written, 1 to 10.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 2 to 64, or <paramref name="value"/> does not fit in <paramref name="bits"/> bits.</exception>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is too short.</exception>
    public static int WriteSignMagnitude(Span<byte> destination, long value, int bits = 64)
    {
        if (!TryWriteSignMagnitude(destination, value, out var bytesWritten, bits))
            throw new ArgumentException(TooShortMessage(destination.Length, MeasureSignMagnitude(value, bits), value), nameof(destination));
        return bytesWritten;
    }

    /// <summary>Writes a sign-magnitude variable length integer, returning <see langword="false"/> instead of throwing when <paramref name="destination"/> is too short.</summary>
    /// <param name="destination">Buffer to write into. Left untouched when this returns <see langword="false"/>.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="bytesWritten">Number of bytes written; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bits">Width of the signed value, 2 to 64 bits. Must match the width used when reading.</param>
    /// <returns><see langword="true"/> when the value was written.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 2 to 64, or <paramref name="value"/> does not fit in <paramref name="bits"/> bits.</exception>
    public static bool TryWriteSignMagnitude(Span<byte> destination, long value, out int bytesWritten, int bits = 64)
    {
        bytesWritten = 0;
        var minimum = SignMagnitudeMinimum(bits);
        ValidateSignedRange(value, bits, minimum);
        if (destination.IsEmpty) return false;
        if (value == minimum)
        {
            destination[0] = 0x40;
            bytesWritten = 1;
            return true;
        }
        var length = MeasureSignMagnitude(value, bits);
        if (destination.Length < length) return false;
        var negative = value < 0;
        var magnitude = (ulong)(negative ? -value : value);
        destination[0] = (byte)((magnitude & 0x3F) | (negative ? 0x40UL : 0UL));
        magnitude >>= 6;
        var index = 0;
        while (magnitude != 0)
        {
            destination[index++] |= 0x80;
            destination[index] = (byte)(magnitude & 0x7F);
            magnitude >>= 7;
        }
        bytesWritten = length;
        return true;
    }

    /// <summary>Reads a sign-magnitude variable length integer in the non-standard format used by Tedd.SpanUtils.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="bytesRead">Number of bytes consumed.</param>
    /// <param name="bits">Width of the signed value, 2 to 64 bits. Must match the width used when writing.</param>
    /// <returns>The decoded value.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 2 to 64.</exception>
    /// <exception cref="OverflowException">The encoded value does not fit in <paramref name="bits"/> bits.</exception>
    /// <exception cref="EndOfStreamException">The encoding is cut short by the end of <paramref name="source"/>.</exception>
    public static long ReadSignMagnitude(ReadOnlySpan<byte> source, out int bytesRead, int bits = 64)
    {
        if (TryReadSignMagnitude(source, out var value, out bytesRead, out var overflow, bits)) return value;
        throw Failure(overflow, bits);
    }

    /// <summary>Reads a sign-magnitude variable length integer, returning <see langword="false"/> instead of throwing on a truncated or out of range encoding.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="value">The decoded value; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bytesRead">Number of bytes consumed; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bits">Width of the signed value, 2 to 64 bits.</param>
    /// <returns><see langword="true"/> when a value was decoded.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 2 to 64.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryReadSignMagnitude(ReadOnlySpan<byte> source, out long value, out int bytesRead, int bits = 64) =>
        TryReadSignMagnitude(source, out value, out bytesRead, out _, bits);

    /// <summary>Reads a sign-magnitude variable length integer, reporting separately whether the failure was an out of range value or a truncated encoding.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="value">The decoded value; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bytesRead">Number of bytes consumed; 0 when this returns <see langword="false"/>.</param>
    /// <param name="overflow"><see langword="true"/> when the failure was a value too large for <paramref name="bits"/>, <see langword="false"/> when the encoding ran off the end of <paramref name="source"/>.</param>
    /// <param name="bits">Width of the signed value, 2 to 64 bits.</param>
    /// <returns><see langword="true"/> when a value was decoded.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 2 to 64.</exception>
    public static bool TryReadSignMagnitude(ReadOnlySpan<byte> source, out long value, out int bytesRead, out bool overflow, int bits = 64)
    {
        ValidateBits(bits, 2);
        value = 0; bytesRead = 0; overflow = false;
        if (source.IsEmpty) return false;
        var first = source[0];
        if (first == 0x40) { value = SignMagnitudeMinimum(bits); bytesRead = 1; return true; }
        var maximum = (1UL << (bits - 1)) - 1;
        var negative = (first & 0x40) != 0;
        var result = (ulong)(first & 0x3F);
        if (first < 0x80)
        {
            if (result > maximum) { overflow = true; return false; }
            value = negative ? -(long)result : (long)result;
            bytesRead = 1;
            return true;
        }
        var maxBytes = (bits + 6) / 7;
        for (var index = 1; index < maxBytes; index++)
        {
            if (index >= source.Length) return false;
            var current = source[index];
            var shift = index * 7 - 1;      // the first byte carries six payload bits, every later one seven
            var payload = (ulong)(current & 0x7F);
            if (payload > (maximum >> shift)) { overflow = true; return false; }
            result |= payload << shift;
            if ((current & 0x80) == 0)
            {
                value = negative ? -(long)result : (long)result;
                bytesRead = index + 1;
                return true;
            }
        }
        overflow = true;
        return false;
    }

    /// <summary>The most negative value a <paramref name="bits"/> wide signed integer can hold, which <see cref="WriteSignMagnitude"/> encodes as the single byte sentinel 0x40.</summary>
    /// <param name="bits">Width of the signed value, 2 to 64 bits.</param>
    /// <returns>The most negative value of that width.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is outside 2 to 64.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long SignMagnitudeMinimum(int bits)
    {
        ValidateBits(bits, 2);
        return bits == 64 ? long.MinValue : -(1L << (bits - 1));
    }
    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool FitsInBits(long value, int bits) => bits == 64 || (value >= -(1L << (bits - 1)) && value <= (1L << (bits - 1)) - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateBits(int bits, int minimumBits)
    {
        if (bits < minimumBits || bits > 64)
            throw new ArgumentOutOfRangeException(nameof(bits), bits, $"Width must be between {minimumBits} and 64 bits.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateSignedRange(long value, int bits, long minimum)
    {
        if (bits == 64) return;
        if (value < minimum || value > -minimum - 1)
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Value does not fit in a {bits} bit signed integer.");
    }

    private static string TooShortMessage(int available, int required, long value) =>
        $"Writing {value} needs {required} bytes, but only {available} are available.";

    private static string TooShortMessage(int available, int required, ulong value) =>
        $"Writing {value} needs {required} bytes, but only {available} are available.";

    private static Exception Failure(bool overflow, int bits) => overflow
        ? new OverflowException($"The variable length integer does not fit in {bits} bits.")
        : new EndOfStreamException("Incomplete variable length integer.");
}
