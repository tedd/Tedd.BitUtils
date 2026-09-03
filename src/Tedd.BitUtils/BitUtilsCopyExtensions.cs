using System;
using System.Runtime.CompilerServices;

namespace Tedd;

/// <summary>
/// Non-mutating counterparts of the in-place operations in <see cref="BitUtilsExtensions"/>.
/// Every method leaves the original value untouched and returns the modified copy.
/// </summary>
/// <remarks>See <see cref="BitUtilsExtensions"/> for the conventions shared by all methods.</remarks>
[CLSCompliant(false)]
public static partial class BitUtilsCopyExtensions
{
    #region SetBitCopy(pos, state)
    /// <summary>Returns a copy of <paramref name="value"/> with bit <paramref name="pos"/> set to 1 when <paramref name="state"/> is <see langword="true"/>, otherwise cleared to 0. Branch free.</summary>
    /// <param name="value">Original value (not modified).</param>
    /// <param name="pos">Zero based bit position counted from the least significant bit.</param>
    /// <param name="state">New state of the bit.</param>
    /// <returns>Modified copy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte SetBitCopy(ref this sbyte value, int pos, bool state) => (sbyte)((value & ~(1 << pos)) | ((state ? 1 : 0) << pos));
    /// <inheritdoc cref="SetBitCopy(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte SetBitCopy(ref this byte value, int pos, bool state) => (byte)((value & ~(1 << pos)) | ((state ? 1 : 0) << pos));
    /// <inheritdoc cref="SetBitCopy(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short SetBitCopy(ref this short value, int pos, bool state) => (short)((value & ~(1 << pos)) | ((state ? 1 : 0) << pos));
    /// <inheritdoc cref="SetBitCopy(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort SetBitCopy(ref this ushort value, int pos, bool state) => (ushort)((value & ~(1 << pos)) | ((state ? 1 : 0) << pos));
    /// <inheritdoc cref="SetBitCopy(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SetBitCopy(ref this int value, int pos, bool state) => (value & ~(1 << pos)) | ((state ? 1 : 0) << pos);
    /// <inheritdoc cref="SetBitCopy(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint SetBitCopy(ref this uint value, int pos, bool state) => (value & ~(1u << pos)) | ((state ? 1u : 0u) << pos);
    /// <inheritdoc cref="SetBitCopy(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long SetBitCopy(ref this long value, int pos, bool state) => (value & ~(1L << pos)) | ((state ? 1L : 0L) << pos);
    /// <inheritdoc cref="SetBitCopy(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong SetBitCopy(ref this ulong value, int pos, bool state) => (value & ~(1UL << pos)) | ((state ? 1UL : 0UL) << pos);
    #endregion

    #region SetBit0Copy(pos)
    /// <summary>Returns a copy of <paramref name="value"/> with bit <paramref name="pos"/> cleared to 0.</summary>
    /// <param name="value">Original value (not modified).</param>
    /// <param name="pos">Zero based bit position counted from the least significant bit.</param>
    /// <returns>Modified copy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte SetBit0Copy(ref this sbyte value, int pos) => (sbyte)(value & ~(1 << pos));
    /// <inheritdoc cref="SetBit0Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte SetBit0Copy(ref this byte value, int pos) => (byte)(value & ~(1 << pos));
    /// <inheritdoc cref="SetBit0Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short SetBit0Copy(ref this short value, int pos) => (short)(value & ~(1 << pos));
    /// <inheritdoc cref="SetBit0Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort SetBit0Copy(ref this ushort value, int pos) => (ushort)(value & ~(1 << pos));
    /// <inheritdoc cref="SetBit0Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SetBit0Copy(ref this int value, int pos) => value & ~(1 << pos);
    /// <inheritdoc cref="SetBit0Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint SetBit0Copy(ref this uint value, int pos) => value & ~(1u << pos);
    /// <inheritdoc cref="SetBit0Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long SetBit0Copy(ref this long value, int pos) => value & ~(1L << pos);
    /// <inheritdoc cref="SetBit0Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong SetBit0Copy(ref this ulong value, int pos) => value & ~(1UL << pos);
    #endregion

    #region SetBit1Copy(pos)
    /// <summary>Returns a copy of <paramref name="value"/> with bit <paramref name="pos"/> set to 1.</summary>
    /// <param name="value">Original value (not modified).</param>
    /// <param name="pos">Zero based bit position counted from the least significant bit.</param>
    /// <returns>Modified copy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte SetBit1Copy(ref this sbyte value, int pos) => (sbyte)(value | (1 << pos));
    /// <inheritdoc cref="SetBit1Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte SetBit1Copy(ref this byte value, int pos) => (byte)(value | (1 << pos));
    /// <inheritdoc cref="SetBit1Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short SetBit1Copy(ref this short value, int pos) => (short)(value | (1 << pos));
    /// <inheritdoc cref="SetBit1Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort SetBit1Copy(ref this ushort value, int pos) => (ushort)(value | (1 << pos));
    /// <inheritdoc cref="SetBit1Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SetBit1Copy(ref this int value, int pos) => value | (1 << pos);
    /// <inheritdoc cref="SetBit1Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint SetBit1Copy(ref this uint value, int pos) => value | (1u << pos);
    /// <inheritdoc cref="SetBit1Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long SetBit1Copy(ref this long value, int pos) => value | (1L << pos);
    /// <inheritdoc cref="SetBit1Copy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong SetBit1Copy(ref this ulong value, int pos) => value | (1UL << pos);
    #endregion

    #region ToggleBitCopy(pos)
    /// <summary>Returns a copy of <paramref name="value"/> with bit <paramref name="pos"/> inverted.</summary>
    /// <param name="value">Original value (not modified).</param>
    /// <param name="pos">Zero based bit position counted from the least significant bit.</param>
    /// <returns>Modified copy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ToggleBitCopy(ref this sbyte value, int pos) => (sbyte)(value ^ (1 << pos));
    /// <inheritdoc cref="ToggleBitCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ToggleBitCopy(ref this byte value, int pos) => (byte)(value ^ (1 << pos));
    /// <inheritdoc cref="ToggleBitCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ToggleBitCopy(ref this short value, int pos) => (short)(value ^ (1 << pos));
    /// <inheritdoc cref="ToggleBitCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ToggleBitCopy(ref this ushort value, int pos) => (ushort)(value ^ (1 << pos));
    /// <inheritdoc cref="ToggleBitCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToggleBitCopy(ref this int value, int pos) => value ^ (1 << pos);
    /// <inheritdoc cref="ToggleBitCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToggleBitCopy(ref this uint value, int pos) => value ^ (1u << pos);
    /// <inheritdoc cref="ToggleBitCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToggleBitCopy(ref this long value, int pos) => value ^ (1L << pos);
    /// <inheritdoc cref="ToggleBitCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToggleBitCopy(ref this ulong value, int pos) => value ^ (1UL << pos);
    #endregion
}
