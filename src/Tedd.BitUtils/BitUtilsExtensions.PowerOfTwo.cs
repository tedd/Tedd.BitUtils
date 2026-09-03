using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tedd;

public static partial class BitUtilsExtensions
{
    #region Software fallback (exercised by tests via InternalsVisibleTo)
    /// <summary>Portable round up to power of two. 0 stays 0; values whose next power of two does not fit give 0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint RoundUpToPowerOf2SoftwareFallback(uint value)
    {
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        return value + 1;
    }

    /// <inheritdoc cref="RoundUpToPowerOf2SoftwareFallback(uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong RoundUpToPowerOf2SoftwareFallback(ulong value)
    {
        --value;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        value |= value >> 32;
        return value + 1;
    }
    #endregion

    #region IsPowerOfTwo
    /// <summary>Returns <see langword="true"/> when exactly one bit is set, i.e. the value is a positive power of two. 0 and negative values give <see langword="false"/>.</summary>
    /// <param name="value">Value to inspect.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPowerOfTwo(ref this sbyte value) => value > 0 && BitOperations.IsPow2((byte)value);
    /// <inheritdoc cref="IsPowerOfTwo(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPowerOfTwo(ref this byte value) => BitOperations.IsPow2(value);
    /// <inheritdoc cref="IsPowerOfTwo(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPowerOfTwo(ref this short value) => value > 0 && BitOperations.IsPow2((ushort)value);
    /// <inheritdoc cref="IsPowerOfTwo(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPowerOfTwo(ref this ushort value) => BitOperations.IsPow2(value);
    /// <inheritdoc cref="IsPowerOfTwo(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPowerOfTwo(ref this int value) => value > 0 && BitOperations.IsPow2((uint)value);
    /// <inheritdoc cref="IsPowerOfTwo(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPowerOfTwo(ref this uint value) => BitOperations.IsPow2(value);
    /// <inheritdoc cref="IsPowerOfTwo(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPowerOfTwo(ref this long value) => value > 0 && BitOperations.IsPow2((ulong)value);
    /// <inheritdoc cref="IsPowerOfTwo(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPowerOfTwo(ref this ulong value) => BitOperations.IsPow2(value);
    #endregion

    #region RoundUpToPowerOf2
    /// <summary>
    /// Rounds the value up to the smallest power of two that is greater than or equal to it, in place.
    /// 0 stays 0. When the result does not fit in the type (for example 0x81 for <see cref="byte"/>) the value becomes 0.
    /// Signed types are treated as their unsigned bit pattern.
    /// </summary>
    /// <param name="value">Value to modify in place.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RoundUpToPowerOf2(ref this sbyte value) => value = (sbyte)BitOperations.RoundUpToPowerOf2((byte)value);
    /// <inheritdoc cref="RoundUpToPowerOf2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RoundUpToPowerOf2(ref this byte value) => value = (byte)BitOperations.RoundUpToPowerOf2(value);
    /// <inheritdoc cref="RoundUpToPowerOf2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RoundUpToPowerOf2(ref this short value) => value = (short)BitOperations.RoundUpToPowerOf2((ushort)value);
    /// <inheritdoc cref="RoundUpToPowerOf2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RoundUpToPowerOf2(ref this ushort value) => value = (ushort)BitOperations.RoundUpToPowerOf2(value);
    /// <inheritdoc cref="RoundUpToPowerOf2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RoundUpToPowerOf2(ref this int value) => value = (int)BitOperations.RoundUpToPowerOf2((uint)value);
    /// <inheritdoc cref="RoundUpToPowerOf2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RoundUpToPowerOf2(ref this uint value) => value = BitOperations.RoundUpToPowerOf2(value);
    /// <inheritdoc cref="RoundUpToPowerOf2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RoundUpToPowerOf2(ref this long value) => value = (long)BitOperations.RoundUpToPowerOf2((ulong)value);
    /// <inheritdoc cref="RoundUpToPowerOf2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RoundUpToPowerOf2(ref this ulong value) => value = BitOperations.RoundUpToPowerOf2(value);
    #endregion
}

public static partial class BitUtilsCopyExtensions
{
    #region RoundUpToPowerOf2Copy
    /// <summary>
    /// Returns the smallest power of two that is greater than or equal to <paramref name="value"/>.
    /// 0 gives 0. When the result does not fit in the type (for example 0x81 for <see cref="byte"/>) 0 is returned.
    /// Signed types are treated as their unsigned bit pattern.
    /// </summary>
    /// <param name="value">Original value (not modified).</param>
    /// <returns>Rounded copy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte RoundUpToPowerOf2Copy(ref this sbyte value) { var v = value; v.RoundUpToPowerOf2(); return v; }
    /// <inheritdoc cref="RoundUpToPowerOf2Copy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte RoundUpToPowerOf2Copy(ref this byte value) { var v = value; v.RoundUpToPowerOf2(); return v; }
    /// <inheritdoc cref="RoundUpToPowerOf2Copy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short RoundUpToPowerOf2Copy(ref this short value) { var v = value; v.RoundUpToPowerOf2(); return v; }
    /// <inheritdoc cref="RoundUpToPowerOf2Copy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort RoundUpToPowerOf2Copy(ref this ushort value) { var v = value; v.RoundUpToPowerOf2(); return v; }
    /// <inheritdoc cref="RoundUpToPowerOf2Copy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundUpToPowerOf2Copy(ref this int value) { var v = value; v.RoundUpToPowerOf2(); return v; }
    /// <inheritdoc cref="RoundUpToPowerOf2Copy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RoundUpToPowerOf2Copy(ref this uint value) { var v = value; v.RoundUpToPowerOf2(); return v; }
    /// <inheritdoc cref="RoundUpToPowerOf2Copy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long RoundUpToPowerOf2Copy(ref this long value) { var v = value; v.RoundUpToPowerOf2(); return v; }
    /// <inheritdoc cref="RoundUpToPowerOf2Copy(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RoundUpToPowerOf2Copy(ref this ulong value) { var v = value; v.RoundUpToPowerOf2(); return v; }
    #endregion
}
