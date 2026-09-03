using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tedd;

public static partial class BitUtilsExtensions
{
    #region Helpers
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string CreateBitString(ulong value, int length) => string.Create(length, value, static (span, v) =>
    {
        for (var i = span.Length - 1; i >= 0; i--)
        {
            span[i] = (char)('0' + (int)(v & 1));
            v >>= 1;
        }
    });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetLength(ulong value) => value == 0 ? 1 : 64 - BitOperations.LeadingZeroCount(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetLength(uint value) => value == 0 ? 1 : 32 - BitOperations.LeadingZeroCount(value);
    // Note: value is widened to uint before counting, so LeadingZeroCount is always computed over 32 bits
    // regardless of the source type's width - subtract from 32 here too, not from 16/8.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetLength(ushort value) => value == 0 ? 1 : 32 - BitOperations.LeadingZeroCount((uint)value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetLength(byte value) => value == 0 ? 1 : 32 - BitOperations.LeadingZeroCount((uint)value);
    #endregion

    #region ToBitStringPadded
    /// <summary>Returns a bit string representing the number, padded with leading zeros to the full width of the type. Time complexity O(N), space complexity O(N).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitStringPadded(ref this sbyte value) => CreateBitString((byte)value, sizeof(sbyte) * 8);
    /// <inheritdoc cref="ToBitStringPadded(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitStringPadded(ref this byte value) => CreateBitString(value, sizeof(byte) * 8);
    /// <inheritdoc cref="ToBitStringPadded(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitStringPadded(ref this short value) => CreateBitString((ushort)value, sizeof(short) * 8);
    /// <inheritdoc cref="ToBitStringPadded(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitStringPadded(ref this ushort value) => CreateBitString(value, sizeof(ushort) * 8);
    /// <inheritdoc cref="ToBitStringPadded(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitStringPadded(ref this int value) => CreateBitString((uint)value, sizeof(int) * 8);
    /// <inheritdoc cref="ToBitStringPadded(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitStringPadded(ref this uint value) => CreateBitString(value, sizeof(uint) * 8);
    /// <inheritdoc cref="ToBitStringPadded(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitStringPadded(ref this long value) => CreateBitString((ulong)value, sizeof(long) * 8);
    /// <inheritdoc cref="ToBitStringPadded(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitStringPadded(ref this ulong value) => CreateBitString(value, sizeof(ulong) * 8);
    #endregion

    #region ToBitString
    /// <summary>Returns a bit string representing the number without leading zero padding (a single "0" for a value of 0). Time complexity O(N), space complexity O(N).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitString(ref this sbyte value) => CreateBitString((byte)value, GetLength((byte)value));
    /// <inheritdoc cref="ToBitString(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitString(ref this byte value) => CreateBitString(value, GetLength(value));
    /// <inheritdoc cref="ToBitString(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitString(ref this short value) => CreateBitString((ushort)value, GetLength((ushort)value));
    /// <inheritdoc cref="ToBitString(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitString(ref this ushort value) => CreateBitString(value, GetLength(value));
    /// <inheritdoc cref="ToBitString(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitString(ref this int value) => CreateBitString((uint)value, GetLength((uint)value));
    /// <inheritdoc cref="ToBitString(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitString(ref this uint value) => CreateBitString(value, GetLength(value));
    /// <inheritdoc cref="ToBitString(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitString(ref this long value) => CreateBitString((ulong)value, GetLength((ulong)value));
    /// <inheritdoc cref="ToBitString(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToBitString(ref this ulong value) => CreateBitString(value, GetLength(value));
    #endregion
}
