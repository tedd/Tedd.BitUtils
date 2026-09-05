using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tedd;

/// <summary>
/// An EBML variable-length integer (a "VINT" in RFC 8794), together with its original wire representation.
/// </summary>
/// <remarks>
/// <para>Where <see cref="VarInt"/> marks the end of a value with a continuation bit on every byte, EBML puts the
/// whole length in the first byte: the number of leading zero bits before the first set bit is the number of
/// <em>extra</em> bytes that follow. So <c>1xxxxxxx</c> is a one byte value with 7 payload bits, <c>01xxxxxx xxxxxxxx</c>
/// is two bytes with 14, and so on up to eight bytes with 56. The payload is big endian, which means encoded values
/// sort in the same order as the numbers they represent, and a decoder knows the full length after one byte.</para>
/// <para>A payload of all ones is reserved: EBML uses it as the "unknown size" marker (see <see cref="IsUnknown"/>),
/// which is why <see cref="MaxValue"/> is one below the 56 bit maximum.</para>
/// <para>This is the encoding used by Matroska, WebM and MKV for element IDs and sizes. RFC 8794 calls the leading
/// marker VINT_MARKER and the payload VINT_DATA, which are <see cref="EncodedValue"/> and <see cref="Value"/> here.</para>
/// </remarks>
public readonly struct EbmlVInt : IEquatable<EbmlVInt>
{
    /// <summary>Largest value that can be encoded: 56 payload bits, less the all-ones pattern reserved for the unknown-size marker.</summary>
    public const ulong MaxValue = 0x00FFFFFFFFFFFFFEUL;

    /// <summary>Longest encoding, in bytes.</summary>
    public const int MaxLength = 8;

    /// <summary>Length in bytes of the encoding this value was read from, 1 to 8.</summary>
    public int Length { get; }

    /// <summary>The raw big endian bytes of the encoding, length marker included, as an integer.</summary>
    public ulong EncodedValue { get; }

    /// <summary>The decoded payload, with the length marker stripped.</summary>
    public ulong Value { get; }

    /// <summary>Length in bytes of the shortest encoding of <see cref="Value"/>, which is below <see cref="Length"/> when the value was written padded out.</summary>
    public int Size { get; }

    /// <summary><see langword="true"/> when every payload bit is one: the EBML unknown-size marker.</summary>
    public bool IsUnknown => Length > 0 && Value == (1UL << (Length * 7)) - 1;

    /// <summary>Creates a value from an already decoded encoding.</summary>
    /// <param name="length">Length in bytes of the encoding, 1 to 8.</param>
    /// <param name="encoded">The raw bytes of the encoding, length marker included.</param>
    /// <param name="value">The decoded payload.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is outside 1 to 8, or <paramref name="value"/> has more payload bits than <paramref name="length"/> bytes can hold.</exception>
    public EbmlVInt(int length, ulong encoded, ulong value)
    {
        if ((uint)(length - 1) >= MaxLength) throw new ArgumentOutOfRangeException(nameof(length), length, "Length must be between 1 and 8 bytes.");
        var capacity = (1UL << (length * 7)) - 1;
        if (value > capacity) throw new ArgumentOutOfRangeException(nameof(value), value, $"Value has more payload bits than {length} bytes can hold.");
        Length = length;
        EncodedValue = encoded;
        Value = value;
        // The all-ones payload is the unknown-size marker rather than a number, so GetSize would reject it.
        Size = value == capacity ? length : GetSize(value);
    }

    /// <summary>Returns the length in bytes of the shortest encoding of <paramref name="value"/>, 1 to 8.</summary>
    /// <param name="value">Value to measure.</param>
    /// <returns>The encoded length in bytes.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is above <see cref="MaxValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetSize(ulong value)
    {
        if (value > MaxValue) throw new ArgumentOutOfRangeException(nameof(value), value, "EBML data integers have at most 56 payload bits, with the all-one value reserved.");
        // Measured on value + 1, so a value that is all ones for its width rolls up into the next size rather than
        // colliding with that width unknown-size marker.
        return (64 - BitOperations.LeadingZeroCount(value + 1) + 6) / 7;
    }

    /// <summary>Reads an EBML variable-length integer from the start of <paramref name="source"/>.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="maxLength">Longest encoding to accept, 1 to 8 bytes.</param>
    /// <returns>The decoded value, including its wire length.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is outside 1 to 8.</exception>
    /// <exception cref="InvalidDataException">The encoding is malformed, longer than <paramref name="maxLength"/>, or cut short by the end of <paramref name="source"/>.</exception>
    public static EbmlVInt Read(ReadOnlySpan<byte> source, int maxLength = MaxLength)
    {
        if (TryRead(source, out var value, maxLength)) return value;
        throw new InvalidDataException("Invalid or incomplete EBML variable-length integer.");
    }

    /// <summary>Reads an EBML variable-length integer, returning <see langword="false"/> instead of throwing on a malformed or truncated encoding.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="value">The decoded value; <see langword="default"/> when this returns <see langword="false"/>.</param>
    /// <param name="maxLength">Longest encoding to accept, 1 to 8 bytes.</param>
    /// <returns><see langword="true"/> when a value was decoded.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is outside 1 to 8.</exception>
    public static bool TryRead(ReadOnlySpan<byte> source, out EbmlVInt value, int maxLength = MaxLength)
    {
        if ((uint)(maxLength - 1) >= MaxLength) throw new ArgumentOutOfRangeException(nameof(maxLength), maxLength, "Maximum length must be between 1 and 8 bytes.");
        value = default;
        if (source.IsEmpty || source[0] == 0) return false;    // a zero first byte would mean more than eight length bits
        var first = source[0];
        var marker = 0x80;
        var length = 1;
        while ((first & marker) == 0) { marker >>= 1; length++; }
        if (length > maxLength || source.Length < length) return false;
        ulong raw = first;
        var payload = (ulong)(first & (marker - 1));
        for (var index = 1; index < length; index++)
        {
            raw = (raw << 8) | source[index];
            payload = (payload << 8) | source[index];
        }
        value = new EbmlVInt(length, raw, payload);
        return true;
    }

    /// <summary>Writes <paramref name="value"/> as an EBML variable-length integer, using the shortest encoding.</summary>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="value">Value to write.</param>
    /// <returns>The number of bytes written, 1 to 8.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is above <see cref="MaxValue"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is too short.</exception>
    public static int Write(Span<byte> destination, ulong value)
    {
        var size = GetSize(value);
        if (destination.Length < size)
            throw new ArgumentException($"Writing {value} needs {size} bytes, but only {destination.Length} are available.", nameof(destination));
        var encoded = value | (1UL << (7 * size));      // the length marker: bit (7 * size) counted from the LSB
        for (var index = size - 1; index >= 0; index--)
        {
            destination[index] = (byte)encoded;
            encoded >>= 8;
        }
        return size;
    }

    /// <summary>Writes an EBML variable-length integer, returning <see langword="false"/> instead of throwing when the value is out of range or <paramref name="destination"/> is too short.</summary>
    /// <param name="destination">Buffer to write into. Left untouched when this returns <see langword="false"/>.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="bytesWritten">Number of bytes written; 0 when this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when the value was written.</returns>
    public static bool TryWrite(Span<byte> destination, ulong value, out int bytesWritten)
    {
        bytesWritten = 0;
        if (value > MaxValue || destination.Length < GetSize(value)) return false;
        bytesWritten = Write(destination, value);
        return true;
    }

    /// <inheritdoc/>
    public bool Equals(EbmlVInt other) => Length == other.Length && EncodedValue == other.EncodedValue && Value == other.Value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is EbmlVInt other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Length, EncodedValue, Value);

    /// <summary>Compares two values for equality.</summary>
    /// <param name="left">First value.</param>
    /// <param name="right">Second value.</param>
    /// <returns><see langword="true"/> when both describe the same encoding of the same value.</returns>
    public static bool operator ==(EbmlVInt left, EbmlVInt right) => left.Equals(right);

    /// <summary>Compares two values for inequality.</summary>
    /// <param name="left">First value.</param>
    /// <param name="right">Second value.</param>
    /// <returns><see langword="true"/> when the values differ.</returns>
    public static bool operator !=(EbmlVInt left, EbmlVInt right) => !left.Equals(right);

    /// <inheritdoc/>
    public override string ToString() => $"EbmlVInt, value = {Value}, length = {Length}, encoded = {EncodedValue:X}";
}
