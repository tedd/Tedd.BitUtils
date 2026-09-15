using System.Runtime.CompilerServices;

namespace Tedd;

public static partial class BitUtilsExtensions
{
    // Pack copies "length" low bits of "value" into "packed" at bit range [offset-length, offset-1] (0 indexed, offset counted from LSB).
    // Unpack does the reverse: extracts "length" bits ending at bit "offset-1" and shifts them down to bit 0.
    // For SByte/Byte/Int16/UInt16 the field width (max 16) is always well within Int32's 32 bit shift range, so the
    // mask is computed directly. Int32/UInt32/Int64/UInt64 use a shift-right-from-all-ones formulation instead of
    // "(1 << length) - 1" so that length (or offset, for Unpack) equal to the full type width does not wrap around.

    #region Pack (in-place)
    /// <summary>Packs bits into an integer, similar to writing into a sub-range of bits.</summary>
    /// <param name="packed">Value to modify in place.</param>
    /// <param name="offset">Offset from the LSB (right) to the bit past the end of the field.</param>
    /// <param name="length">Width of the field in bits.</param>
    /// <param name="value">Value to insert; only the <paramref name="length"/> least significant bits are used.</param>
    /// <example>var i1 = 0b0000_1111_1100_0011; var i2 = 0b0000_0000_0000_0010; i1.Pack(5, 2, i2); // i1 == 0b0000_1111_1101_0011</example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pack(ref this sbyte packed, int offset, int length, sbyte value)
    {
        var mask = ((1 << length) - 1) << (offset - length);
        packed = (sbyte)((packed & ~mask) | ((value << (offset - length)) & mask));
    }
    /// <inheritdoc cref="Pack(ref sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pack(ref this byte packed, int offset, int length, byte value)
    {
        var mask = ((1 << length) - 1) << (offset - length);
        packed = (byte)((packed & ~mask) | ((value << (offset - length)) & mask));
    }
    /// <inheritdoc cref="Pack(ref sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pack(ref this short packed, int offset, int length, short value)
    {
        var mask = ((1 << length) - 1) << (offset - length);
        packed = (short)((packed & ~mask) | ((value << (offset - length)) & mask));
    }
    /// <inheritdoc cref="Pack(ref sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pack(ref this ushort packed, int offset, int length, ushort value)
    {
        var mask = ((1 << length) - 1) << (offset - length);
        packed = (ushort)((packed & ~mask) | ((value << (offset - length)) & mask));
    }
    /// <inheritdoc cref="Pack(ref sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pack(ref this int packed, int offset, int length, int value)
    {
        var mask = (int)((length <= 0 ? 0u : uint.MaxValue >> (32 - length)) << (offset - length));
        packed = (packed & ~mask) | ((value << (offset - length)) & mask);
    }
    /// <inheritdoc cref="Pack(ref sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pack(ref this uint packed, int offset, int length, uint value)
    {
        var mask = (length <= 0 ? 0u : uint.MaxValue >> (32 - length)) << (offset - length);
        packed = (packed & ~mask) | ((value << (offset - length)) & mask);
    }
    /// <inheritdoc cref="Pack(ref sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pack(ref this long packed, int offset, int length, long value)
    {
        var mask = (long)((length <= 0 ? 0ul : ulong.MaxValue >> (64 - length)) << (offset - length));
        packed = (packed & ~mask) | ((value << (offset - length)) & mask);
    }
    /// <inheritdoc cref="Pack(ref sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Pack(ref this ulong packed, int offset, int length, ulong value)
    {
        var mask = (length <= 0 ? 0ul : ulong.MaxValue >> (64 - length)) << (offset - length);
        packed = (packed & ~mask) | ((value << (offset - length)) & mask);
    }
    #endregion

    #region Unpack
    /// <summary>Extracts a sub-range of bits, similar to Substring() but for bits, shifted down so the field's low bit lands at bit 0.</summary>
    /// <param name="value">Value to extract from.</param>
    /// <param name="offset">Offset from the LSB (right) to the bit past the end of the field.</param>
    /// <param name="length">Width of the field in bits.</param>
    /// <returns>The extracted bits, shifted down to start at bit 0.</returns>
    /// <example>var value = 0b00000000_10011001; value.Unpack(5, 2) == 0b11</example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Unpack(ref this sbyte value, int offset, int length) => (sbyte)(((byte)value & (byte)((1 << offset) - 1)) >> (offset - length));
    /// <inheritdoc cref="Unpack(ref sbyte, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Unpack(ref this byte value, int offset, int length) => (byte)((value & ((1 << offset) - 1)) >> (offset - length));
    /// <inheritdoc cref="Unpack(ref sbyte, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Unpack(ref this short value, int offset, int length) => (short)(((ushort)value & (ushort)((1 << offset) - 1)) >> (offset - length));
    /// <inheritdoc cref="Unpack(ref sbyte, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Unpack(ref this ushort value, int offset, int length) => (ushort)((value & ((1 << offset) - 1)) >> (offset - length));
    /// <inheritdoc cref="Unpack(ref sbyte, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Unpack(ref this int value, int offset, int length) => (int)(((uint)value & (offset <= 0 ? 0u : uint.MaxValue >> (32 - offset))) >> (offset - length));
    /// <inheritdoc cref="Unpack(ref sbyte, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Unpack(ref this uint value, int offset, int length) => (value & (offset <= 0 ? 0u : uint.MaxValue >> (32 - offset))) >> (offset - length);
    /// <inheritdoc cref="Unpack(ref sbyte, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Unpack(ref this long value, int offset, int length) => (long)(((ulong)value & (offset <= 0 ? 0ul : ulong.MaxValue >> (64 - offset))) >> (offset - length));
    /// <inheritdoc cref="Unpack(ref sbyte, int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Unpack(ref this ulong value, int offset, int length) => (value & (offset <= 0 ? 0ul : ulong.MaxValue >> (64 - offset))) >> (offset - length);
    #endregion
}

public static partial class BitUtilsCopyExtensions
{
    #region PackCopy
    /// <summary>Returns a copy of <paramref name="packed"/> with bits packed in, similar to writing into a sub-range of bits.</summary>
    /// <param name="packed">Original value (not modified).</param>
    /// <param name="offset">Offset from the LSB (right) to the bit past the end of the field.</param>
    /// <param name="length">Width of the field in bits.</param>
    /// <param name="value">Value to insert; only the <paramref name="length"/> least significant bits are used.</param>
    /// <returns>Modified copy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte PackCopy(this sbyte packed, int offset, int length, sbyte value) { packed.Pack(offset, length, value); return packed; }
    /// <inheritdoc cref="PackCopy(sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte PackCopy(this byte packed, int offset, int length, byte value) { packed.Pack(offset, length, value); return packed; }
    /// <inheritdoc cref="PackCopy(sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short PackCopy(this short packed, int offset, int length, short value) { packed.Pack(offset, length, value); return packed; }
    /// <inheritdoc cref="PackCopy(sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort PackCopy(this ushort packed, int offset, int length, ushort value) { packed.Pack(offset, length, value); return packed; }
    /// <inheritdoc cref="PackCopy(sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PackCopy(this int packed, int offset, int length, int value) { packed.Pack(offset, length, value); return packed; }
    /// <inheritdoc cref="PackCopy(sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint PackCopy(this uint packed, int offset, int length, uint value) { packed.Pack(offset, length, value); return packed; }
    /// <inheritdoc cref="PackCopy(sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long PackCopy(this long packed, int offset, int length, long value) { packed.Pack(offset, length, value); return packed; }
    /// <inheritdoc cref="PackCopy(sbyte, int, int, sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong PackCopy(this ulong packed, int offset, int length, ulong value) { packed.Pack(offset, length, value); return packed; }
    #endregion
}
