using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;

namespace Tedd;

/// <summary>
/// A prefix-varint length field: one to four bytes holding a value of up to 30 bits, with the byte count in the top
/// two bits of the first byte and the payload big endian in the rest.
/// </summary>
/// <remarks>
/// <para>The two most significant bits of the first byte are the length selector (0 to 3, meaning 1 to 4 bytes) and
/// every remaining bit is payload. So values up to 0x3F take one byte, up to 0x3FFF two, up to 0x3FFFFF three and up
/// to <see cref="MaxValue"/> four.</para>
/// <para>Compared with <see cref="VarInt"/> this trades range for density at the sizes that matter for a length
/// prefix: it holds 30 bits in four bytes where LEB128 holds 28, and a decoder learns the total length from the
/// first byte (see <see cref="GetLength"/>) instead of scanning for a continuation bit. It cannot express anything
/// above 30 bits at all. Because the length lives in the high bits and the payload is big endian, encoded values
/// sort in the same order as the numbers they represent.</para>
/// <para>This is the same idea as the QUIC variable-length integer of RFC 9000, but not the same encoding: QUIC
/// selects 1, 2, 4 or 8 bytes (6, 14, 30 or 62 bit values) where this selects 1, 2, 3 or 4 (6, 14, 22 or 30 bits).
/// It is unrelated to Bitcoin's CompactSize, which is little endian and uses 0xFD/0xFE/0xFF escape bytes. This is
/// the format Tedd.SpanUtils writes for its length prefixes.</para>
/// </remarks>
public static class SizePrefix
{
    /// <summary>Largest value that can be encoded, 30 bits.</summary>
    public const uint MaxValue = 0x3FFFFFFFU;

    /// <summary>Longest encoding, in bytes.</summary>
    public const int MaxLength = 4;

    /// <summary>Returns the number of bytes <see cref="Write"/> will use for <paramref name="value"/>.</summary>
    /// <param name="value">Value to measure.</param>
    /// <returns>The encoded length in bytes, 1 to 4.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is above <see cref="MaxValue"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Measure(uint value)
    {
        if (value <= 0x3FU) return 1;
        if (value <= 0x3FFFU) return 2;
        if (value <= 0x3FFFFFU) return 3;
        if (value <= MaxValue) return 4;
        throw new ArgumentOutOfRangeException(nameof(value), value, "Size exceeds the 30-bit format.");
    }

    /// <summary>Returns the total length in bytes of the encoding that starts with <paramref name="firstByte"/>.</summary>
    /// <param name="firstByte">First byte of an encoding.</param>
    /// <returns>The encoded length in bytes, 1 to 4.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLength(byte firstByte) => (firstByte >> 6) + 1;

    /// <summary>Writes <paramref name="value"/> as a size prefix, using the shortest encoding.</summary>
    /// <param name="destination">Buffer to write into.</param>
    /// <param name="value">Value to write.</param>
    /// <returns>The number of bytes written, 1 to 4.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is above <see cref="MaxValue"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is too short.</exception>
    public static int Write(Span<byte> destination, uint value)
    {
        var length = Measure(value);
        if (destination.Length < length)
            throw new ArgumentException($"Writing {value} needs {length} bytes, but only {destination.Length} are available.", nameof(destination));
        WriteCore(destination, value, length);
        return length;
    }

    /// <summary>Writes a size prefix, returning <see langword="false"/> instead of throwing when the value is out of range or <paramref name="destination"/> is too short.</summary>
    /// <param name="destination">Buffer to write into. Left untouched when this returns <see langword="false"/>.</param>
    /// <param name="value">Value to write.</param>
    /// <param name="bytesWritten">Number of bytes written; 0 when this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when the value was written.</returns>
    public static bool TryWrite(Span<byte> destination, uint value, out int bytesWritten)
    {
        bytesWritten = 0;
        if (value > MaxValue) return false;
        var length = Measure(value);
        if (destination.Length < length) return false;
        WriteCore(destination, value, length);
        bytesWritten = length;
        return true;
    }

    /// <summary>Reads a size prefix from the start of <paramref name="source"/>.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="bytesRead">Number of bytes consumed, 1 to 4.</param>
    /// <returns>The decoded value.</returns>
    /// <exception cref="EndOfStreamException">The encoding is cut short by the end of <paramref name="source"/>.</exception>
    public static uint Read(ReadOnlySpan<byte> source, out int bytesRead)
    {
        if (TryRead(source, out var value, out bytesRead)) return value;
        throw new EndOfStreamException("Incomplete size prefix.");
    }

    /// <summary>Reads a size prefix, returning <see langword="false"/> instead of throwing on a truncated encoding.</summary>
    /// <param name="source">Buffer to read from.</param>
    /// <param name="value">The decoded value; 0 when this returns <see langword="false"/>.</param>
    /// <param name="bytesRead">Number of bytes consumed; 0 when this returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> when a value was decoded.</returns>
    public static bool TryRead(ReadOnlySpan<byte> source, out uint value, out int bytesRead)
    {
        value = 0; bytesRead = 0;
        if (source.IsEmpty) return false;
        var length = GetLength(source[0]);
        if (source.Length < length) return false;
        value = length switch
        {
            1 => source[0],
            2 => BinaryPrimitives.ReadUInt16BigEndian(source) & 0x3FFFU,
            3 => ((uint)source[0] << 16 | (uint)source[1] << 8 | source[2]) & 0x3FFFFFU,
            _ => BinaryPrimitives.ReadUInt32BigEndian(source) & MaxValue
        };
        bytesRead = length;
        return true;
    }

    private static void WriteCore(Span<byte> destination, uint value, int length)
    {
        switch (length)
        {
            case 1:
                destination[0] = (byte)value;
                break;
            case 2:
                BinaryPrimitives.WriteUInt16BigEndian(destination, (ushort)(value | 0x4000U));
                break;
            case 3:
                destination[0] = (byte)((value >> 16) | 0x80U);
                destination[1] = (byte)(value >> 8);
                destination[2] = (byte)value;
                break;
            default:
                BinaryPrimitives.WriteUInt32BigEndian(destination, value | 0xC0000000U);
                break;
        }
    }
}
