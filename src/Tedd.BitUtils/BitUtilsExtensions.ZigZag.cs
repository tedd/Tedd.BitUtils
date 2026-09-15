using System.Runtime.CompilerServices;

namespace Tedd;

public static partial class BitUtilsExtensions
{
    // ZigZag maps signed values onto unsigned ones so that small magnitudes of either sign stay small:
    //   0 -> 0, -1 -> 1, 1 -> 2, -2 -> 3, 2 -> 4, ...
    // Encoding is "(value << 1) ^ (value >> (width - 1))": the arithmetic right shift yields all-ones for negative
    // values and all-zeros for non-negative ones, so negatives get their shifted bits inverted. Decoding reverses
    // that with "(value >> 1) ^ -(value & 1)". Both are branch free, total (every input maps to a distinct output
    // covering the full unsigned range) and never overflow, since C# shifts and the casts here are unchecked.
    // This is what makes a variable length encoding compact for negative numbers: without it, -1 as a two's
    // complement 64 bit pattern is 0xFFFFFFFFFFFFFFFF and costs the full 10 bytes as a VLQ. See VarInt.

    #region ZigZagEncode
    /// <summary>Maps a signed value onto an unsigned one so that small magnitudes of either sign encode to small numbers: 0, -1, 1, -2, 2 map to 0, 1, 2, 3, 4. Branch free.</summary>
    /// <param name="value">Value to encode.</param>
    /// <returns>The ZigZag encoded value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ZigZagEncode(ref this sbyte value) => (byte)((value << 1) ^ (value >> 7));
    /// <inheritdoc cref="ZigZagEncode(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ZigZagEncode(ref this short value) => (ushort)((value << 1) ^ (value >> 15));
    /// <inheritdoc cref="ZigZagEncode(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ZigZagEncode(ref this int value) => (uint)((value << 1) ^ (value >> 31));
    /// <inheritdoc cref="ZigZagEncode(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ZigZagEncode(ref this long value) => (ulong)((value << 1) ^ (value >> 63));
    #endregion

    #region ZigZagDecode
    /// <summary>Reverses <see cref="ZigZagEncode(ref sbyte)"/>, mapping an unsigned value back onto the signed one it came from. Branch free.</summary>
    /// <param name="value">Value to decode.</param>
    /// <returns>The original signed value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ZigZagDecode(ref this byte value) => (sbyte)((value >> 1) ^ -(value & 1));
    /// <inheritdoc cref="ZigZagDecode(ref byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ZigZagDecode(ref this ushort value) => (short)((value >> 1) ^ -(value & 1));
    /// <inheritdoc cref="ZigZagDecode(ref byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ZigZagDecode(ref this uint value) => (int)(value >> 1) ^ -(int)(value & 1);
    /// <inheritdoc cref="ZigZagDecode(ref byte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ZigZagDecode(ref this ulong value) => (long)(value >> 1) ^ -(long)(value & 1);
    #endregion
}
