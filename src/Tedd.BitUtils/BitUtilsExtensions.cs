using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(false)]

namespace Tedd;

/// <summary>
/// Bit manipulation extension methods for <see cref="sbyte"/>, <see cref="byte"/>, <see cref="short"/>, <see cref="ushort"/>,
/// <see cref="int"/>, <see cref="uint"/>, <see cref="long"/> and <see cref="ulong"/>.
/// </summary>
/// <remarks>
/// <para>Methods in this class either modify the value in place (<c>ref this</c>) or return information about the value.
/// Variants that leave the original untouched and return a modified copy live in <see cref="BitUtilsCopyExtensions"/>.</para>
/// <para>Every public method is a thin wrapper that the JIT inlines into the caller. On .NET 6 and later the implementations
/// use <see cref="System.Numerics.BitOperations"/> and hardware intrinsics (POPCNT, LZCNT, TZCNT, BMI1, BMI2, BSWAP, ARM RBIT)
/// with automatic software fallbacks. On .NET Standard 2.0 (.NET Framework, Mono, Unity) portable software implementations are used.</para>
/// <para>Bit positions are zero based and counted from the least significant bit. Signed types are treated as their
/// two's complement bit pattern. No argument validation is performed: a bit position, count, offset or length outside the
/// bit width of the type gives an unspecified (but never throwing) result.</para>
/// </remarks>
[CLSCompliant(false)]
public static partial class BitUtilsExtensions
{
    #region SetBit(pos, state)
    /// <summary>Sets bit <paramref name="pos"/> to 1 when <paramref name="state"/> is <see langword="true"/>, otherwise clears it to 0. Branch free.</summary>
    /// <param name="value">Value to modify in place.</param>
    /// <param name="pos">Zero based bit position counted from the least significant bit.</param>
    /// <param name="state">New state of the bit.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(ref this sbyte value, int pos, bool state) => value = (sbyte)((value & ~(1 << pos)) | ((state ? 1 : 0) << pos));
    /// <inheritdoc cref="SetBit(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(ref this byte value, int pos, bool state) => value = (byte)((value & ~(1 << pos)) | ((state ? 1 : 0) << pos));
    /// <inheritdoc cref="SetBit(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(ref this short value, int pos, bool state) => value = (short)((value & ~(1 << pos)) | ((state ? 1 : 0) << pos));
    /// <inheritdoc cref="SetBit(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(ref this ushort value, int pos, bool state) => value = (ushort)((value & ~(1 << pos)) | ((state ? 1 : 0) << pos));
    /// <inheritdoc cref="SetBit(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(ref this int value, int pos, bool state) => value = (value & ~(1 << pos)) | ((state ? 1 : 0) << pos);
    /// <inheritdoc cref="SetBit(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(ref this uint value, int pos, bool state) => value = (value & ~(1u << pos)) | ((state ? 1u : 0u) << pos);
    /// <inheritdoc cref="SetBit(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(ref this long value, int pos, bool state) => value = (value & ~(1L << pos)) | ((state ? 1L : 0L) << pos);
    /// <inheritdoc cref="SetBit(ref sbyte, int, bool)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(ref this ulong value, int pos, bool state) => value = (value & ~(1UL << pos)) | ((state ? 1UL : 0UL) << pos);
    #endregion

    #region SetBit0(pos)
    /// <summary>Clears bit <paramref name="pos"/> to 0. Faster than <c>SetBit(pos, false)</c>.</summary>
    /// <param name="value">Value to modify in place.</param>
    /// <param name="pos">Zero based bit position counted from the least significant bit.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit0(ref this sbyte value, int pos) => value = (sbyte)(value & ~(1 << pos));
    /// <inheritdoc cref="SetBit0(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit0(ref this byte value, int pos) => value = (byte)(value & ~(1 << pos));
    /// <inheritdoc cref="SetBit0(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit0(ref this short value, int pos) => value = (short)(value & ~(1 << pos));
    /// <inheritdoc cref="SetBit0(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit0(ref this ushort value, int pos) => value = (ushort)(value & ~(1 << pos));
    /// <inheritdoc cref="SetBit0(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit0(ref this int value, int pos) => value &= ~(1 << pos);
    /// <inheritdoc cref="SetBit0(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit0(ref this uint value, int pos) => value &= ~(1u << pos);
    /// <inheritdoc cref="SetBit0(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit0(ref this long value, int pos) => value &= ~(1L << pos);
    /// <inheritdoc cref="SetBit0(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit0(ref this ulong value, int pos) => value &= ~(1UL << pos);
    #endregion

    #region SetBit1(pos)
    /// <summary>Sets bit <paramref name="pos"/> to 1. Faster than <c>SetBit(pos, true)</c>.</summary>
    /// <param name="value">Value to modify in place.</param>
    /// <param name="pos">Zero based bit position counted from the least significant bit.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit1(ref this sbyte value, int pos) => value = (sbyte)((byte)value | (1 << pos));
    /// <inheritdoc cref="SetBit1(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit1(ref this byte value, int pos) => value = (byte)(value | (1 << pos));
    /// <inheritdoc cref="SetBit1(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit1(ref this short value, int pos) => value = (short)((ushort)value | (1 << pos));
    /// <inheritdoc cref="SetBit1(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit1(ref this ushort value, int pos) => value = (ushort)(value | (1 << pos));
    /// <inheritdoc cref="SetBit1(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit1(ref this int value, int pos) => value |= 1 << pos;
    /// <inheritdoc cref="SetBit1(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit1(ref this uint value, int pos) => value |= 1u << pos;
    /// <inheritdoc cref="SetBit1(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit1(ref this long value, int pos) => value |= 1L << pos;
    /// <inheritdoc cref="SetBit1(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit1(ref this ulong value, int pos) => value |= 1UL << pos;
    #endregion

    #region ToggleBit(pos)
    /// <summary>Inverts bit <paramref name="pos"/> (0 becomes 1, 1 becomes 0).</summary>
    /// <param name="value">Value to modify in place.</param>
    /// <param name="pos">Zero based bit position counted from the least significant bit.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleBit(ref this sbyte value, int pos) => value = (sbyte)(value ^ (1 << pos));
    /// <inheritdoc cref="ToggleBit(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleBit(ref this byte value, int pos) => value = (byte)(value ^ (1 << pos));
    /// <inheritdoc cref="ToggleBit(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleBit(ref this short value, int pos) => value = (short)(value ^ (1 << pos));
    /// <inheritdoc cref="ToggleBit(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleBit(ref this ushort value, int pos) => value = (ushort)(value ^ (1 << pos));
    /// <inheritdoc cref="ToggleBit(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleBit(ref this int value, int pos) => value ^= 1 << pos;
    /// <inheritdoc cref="ToggleBit(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleBit(ref this uint value, int pos) => value ^= 1u << pos;
    /// <inheritdoc cref="ToggleBit(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleBit(ref this long value, int pos) => value ^= 1L << pos;
    /// <inheritdoc cref="ToggleBit(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ToggleBit(ref this ulong value, int pos) => value ^= 1UL << pos;
    #endregion

    #region IsBitSet(pos)
    /// <summary>Returns <see langword="true"/> when bit <paramref name="pos"/> is 1.</summary>
    /// <param name="value">Value to inspect.</param>
    /// <param name="pos">Zero based bit position counted from the least significant bit.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(ref this sbyte value, int pos) => (value & (1 << pos)) != 0;
    /// <inheritdoc cref="IsBitSet(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(ref this byte value, int pos) => (value & (1 << pos)) != 0;
    /// <inheritdoc cref="IsBitSet(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(ref this short value, int pos) => (value & (1 << pos)) != 0;
    /// <inheritdoc cref="IsBitSet(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(ref this ushort value, int pos) => (value & (1 << pos)) != 0;
    /// <inheritdoc cref="IsBitSet(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(ref this int value, int pos) => (value & (1 << pos)) != 0;
    /// <inheritdoc cref="IsBitSet(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(ref this uint value, int pos) => (value & (1u << pos)) != 0;
    /// <inheritdoc cref="IsBitSet(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(ref this long value, int pos) => (value & (1L << pos)) != 0;
    /// <inheritdoc cref="IsBitSet(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(ref this ulong value, int pos) => (value & (1UL << pos)) != 0;
    #endregion
}
