using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tedd;

public static partial class BitUtilsExtensions
{
    // 32 and 64 bit rotates are System.Numerics.BitOperations.RotateLeft/RotateRight, which the JIT compiles to a
    // single ROL/ROR instruction on x86/x64 and ARM64. 8 and 16 bit rotates are composed from shifts on the zero
    // extended value; the count is reduced modulo the bit width first.

    #region Core
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte RotateLeftCore8(byte value, int count)
    {
        count &= 7;
        return (byte)((value << count) | (value >> (8 - count)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte RotateRightCore8(byte value, int count)
    {
        count &= 7;
        return (byte)((value >> count) | (value << (8 - count)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ushort RotateLeftCore16(ushort value, int count)
    {
        count &= 15;
        return (ushort)((value << count) | (value >> (16 - count)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ushort RotateRightCore16(ushort value, int count)
    {
        count &= 15;
        return (ushort)((value >> count) | (value << (16 - count)));
    }
    #endregion

    #region Rol(count)
    /// <summary>Rotates the bits left (towards the most significant bit) by <paramref name="count"/> positions. Bits shifted out at the top re-enter at the bottom.</summary>
    /// <param name="value">Value to modify in place.</param>
    /// <param name="count">Number of positions to rotate. Taken modulo the bit width of the type, so negative counts rotate right.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this sbyte value, int count) => value = (sbyte)RotateLeftCore8((byte)value, count);
    /// <inheritdoc cref="Rol(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this byte value, int count) => value = RotateLeftCore8(value, count);
    /// <inheritdoc cref="Rol(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this short value, int count) => value = (short)RotateLeftCore16((ushort)value, count);
    /// <inheritdoc cref="Rol(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this ushort value, int count) => value = RotateLeftCore16(value, count);
    /// <inheritdoc cref="Rol(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this int value, int count) => value = (int)BitOperations.RotateLeft((uint)value, count);
    /// <inheritdoc cref="Rol(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this uint value, int count) => value = BitOperations.RotateLeft(value, count);
    /// <inheritdoc cref="Rol(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this long value, int count) => value = (long)BitOperations.RotateLeft((ulong)value, count);
    /// <inheritdoc cref="Rol(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this ulong value, int count) => value = BitOperations.RotateLeft(value, count);
    #endregion

    #region Ror(count)
    /// <summary>Rotates the bits right (towards the least significant bit) by <paramref name="count"/> positions. Bits shifted out at the bottom re-enter at the top.</summary>
    /// <param name="value">Value to modify in place.</param>
    /// <param name="count">Number of positions to rotate. Taken modulo the bit width of the type, so negative counts rotate left.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this sbyte value, int count) => value = (sbyte)RotateRightCore8((byte)value, count);
    /// <inheritdoc cref="Ror(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this byte value, int count) => value = RotateRightCore8(value, count);
    /// <inheritdoc cref="Ror(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this short value, int count) => value = (short)RotateRightCore16((ushort)value, count);
    /// <inheritdoc cref="Ror(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this ushort value, int count) => value = RotateRightCore16(value, count);
    /// <inheritdoc cref="Ror(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this int value, int count) => value = (int)BitOperations.RotateRight((uint)value, count);
    /// <inheritdoc cref="Ror(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this uint value, int count) => value = BitOperations.RotateRight(value, count);
    /// <inheritdoc cref="Ror(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this long value, int count) => value = (long)BitOperations.RotateRight((ulong)value, count);
    /// <inheritdoc cref="Ror(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this ulong value, int count) => value = BitOperations.RotateRight(value, count);
    #endregion

    #region Rol()
    /// <summary>Rotates the bits left by one position. Slightly cheaper than <c>Rol(1)</c> because no count needs to be masked.</summary>
    /// <param name="value">Value to modify in place.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this sbyte value) => value = (sbyte)((byte)(value << 1) | ((byte)value >> 7));
    /// <inheritdoc cref="Rol(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this byte value) => value = (byte)((value << 1) | (value >> 7));
    /// <inheritdoc cref="Rol(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this short value) => value = (short)((ushort)(value << 1) | ((ushort)value >> 15));
    /// <inheritdoc cref="Rol(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this ushort value) => value = (ushort)((value << 1) | (value >> 15));
    /// <inheritdoc cref="Rol(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this int value) => value = (int)BitOperations.RotateLeft((uint)value, 1);
    /// <inheritdoc cref="Rol(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this uint value) => value = BitOperations.RotateLeft(value, 1);
    /// <inheritdoc cref="Rol(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this long value) => value = (long)BitOperations.RotateLeft((ulong)value, 1);
    /// <inheritdoc cref="Rol(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Rol(ref this ulong value) => value = BitOperations.RotateLeft(value, 1);
    #endregion

    #region Ror()
    /// <summary>Rotates the bits right by one position. Slightly cheaper than <c>Ror(1)</c> because no count needs to be masked.</summary>
    /// <param name="value">Value to modify in place.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this sbyte value) => value = (sbyte)(((byte)value >> 1) | (byte)(value << 7));
    /// <inheritdoc cref="Ror(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this byte value) => value = (byte)((value >> 1) | (value << 7));
    /// <inheritdoc cref="Ror(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this short value) => value = (short)(((ushort)value >> 1) | (ushort)(value << 15));
    /// <inheritdoc cref="Ror(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this ushort value) => value = (ushort)((value >> 1) | (value << 15));
    /// <inheritdoc cref="Ror(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this int value) => value = (int)BitOperations.RotateRight((uint)value, 1);
    /// <inheritdoc cref="Ror(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this uint value) => value = BitOperations.RotateRight(value, 1);
    /// <inheritdoc cref="Ror(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this long value) => value = (long)BitOperations.RotateRight((ulong)value, 1);
    /// <inheritdoc cref="Ror(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Ror(ref this ulong value) => value = BitOperations.RotateRight(value, 1);
    #endregion
}

public static partial class BitUtilsCopyExtensions
{
    #region RolCopy(count)
    /// <summary>Returns a copy of <paramref name="value"/> rotated left by <paramref name="count"/> positions.</summary>
    /// <param name="value">Original value (not modified).</param>
    /// <param name="count">Number of positions to rotate. Taken modulo the bit width of the type, so negative counts rotate right.</param>
    /// <returns>Rotated copy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte RolCopy(ref this sbyte value, int count) { var v = value; v.Rol(count); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte RolCopy(ref this byte value, int count) { var v = value; v.Rol(count); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short RolCopy(ref this short value, int count) { var v = value; v.Rol(count); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort RolCopy(ref this ushort value, int count) { var v = value; v.Rol(count); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RolCopy(ref this int value, int count) { var v = value; v.Rol(count); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RolCopy(ref this uint value, int count) { var v = value; v.Rol(count); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long RolCopy(ref this long value, int count) { var v = value; v.Rol(count); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RolCopy(ref this ulong value, int count) { var v = value; v.Rol(count); return v; }
    #endregion

    #region RorCopy(count)
    /// <summary>Returns a copy of <paramref name="value"/> rotated right by <paramref name="count"/> positions.</summary>
    /// <param name="value">Original value (not modified).</param>
    /// <param name="count">Number of positions to rotate. Taken modulo the bit width of the type, so negative counts rotate left.</param>
    /// <returns>Rotated copy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte RorCopy(ref this sbyte value, int count) { var v = value; v.Ror(count); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte RorCopy(ref this byte value, int count) { var v = value; v.Ror(count); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short RorCopy(ref this short value, int count) { var v = value; v.Ror(count); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort RorCopy(ref this ushort value, int count) { var v = value; v.Ror(count); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RorCopy(ref this int value, int count) { var v = value; v.Ror(count); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RorCopy(ref this uint value, int count) { var v = value; v.Ror(count); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long RorCopy(ref this long value, int count) { var v = value; v.Ror(count); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RorCopy(ref this ulong value, int count) { var v = value; v.Ror(count); return v; }
    #endregion

    #region RolCopy()
    /// <summary>Returns a copy of <paramref name="value"/> rotated left by one position.</summary>
    /// <param name="value">Original value (not modified).</param>
    /// <returns>Rotated copy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte RolCopy(ref this sbyte value) { var v = value; v.Rol(); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte RolCopy(ref this byte value) { var v = value; v.Rol(); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short RolCopy(ref this short value) { var v = value; v.Rol(); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort RolCopy(ref this ushort value) { var v = value; v.Rol(); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RolCopy(ref this int value) { var v = value; v.Rol(); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RolCopy(ref this uint value) { var v = value; v.Rol(); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long RolCopy(ref this long value) { var v = value; v.Rol(); return v; }
    /// <inheritdoc cref="RolCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RolCopy(ref this ulong value) { var v = value; v.Rol(); return v; }
    #endregion

    #region RorCopy()
    /// <summary>Returns a copy of <paramref name="value"/> rotated right by one position.</summary>
    /// <param name="value">Original value (not modified).</param>
    /// <returns>Rotated copy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte RorCopy(ref this sbyte value) { var v = value; v.Ror(); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte RorCopy(ref this byte value) { var v = value; v.Ror(); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short RorCopy(ref this short value) { var v = value; v.Ror(); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort RorCopy(ref this ushort value) { var v = value; v.Ror(); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RorCopy(ref this int value) { var v = value; v.Ror(); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RorCopy(ref this uint value) { var v = value; v.Ror(); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long RorCopy(ref this long value) { var v = value; v.Ror(); return v; }
    /// <inheritdoc cref="RorCopy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RorCopy(ref this ulong value) { var v = value; v.Ror(); return v; }
    #endregion
}
