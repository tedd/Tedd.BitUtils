using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Tedd;

/// <summary>Endianness swap extension methods.</summary>
/// <remarks>
/// Note on naming: the type name keeps the historical "Endinaness" spelling for source compatibility
/// with existing consumers; the methods it exposes are correctly spelled (<see cref="ReverseEndianness(ref short)"/> etc.).
/// </remarks>
[CLSCompliant(false)]
public static class BitUtilsEndinanessExtensions
{
    #region In-Place
    /// <summary>This is a no-op and exists only for consistency: a single byte has no endianness.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReverseEndianness(ref this sbyte value) { }

    /// <summary>This is a no-op and exists only for consistency: a single byte has no endianness.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReverseEndianness(ref this byte value) { }

    /// <summary>Reverses the byte order of the value (BSWAP).</summary>
    /// <param name="value">Value to modify in place.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReverseEndianness(ref this short value) => value = BinaryPrimitives.ReverseEndianness(value);
    /// <inheritdoc cref="ReverseEndianness(ref short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReverseEndianness(ref this ushort value) => value = BinaryPrimitives.ReverseEndianness(value);
    /// <inheritdoc cref="ReverseEndianness(ref short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReverseEndianness(ref this int value) => value = BinaryPrimitives.ReverseEndianness(value);
    /// <inheritdoc cref="ReverseEndianness(ref short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReverseEndianness(ref this uint value) => value = BinaryPrimitives.ReverseEndianness(value);
    /// <inheritdoc cref="ReverseEndianness(ref short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReverseEndianness(ref this long value) => value = BinaryPrimitives.ReverseEndianness(value);
    /// <inheritdoc cref="ReverseEndianness(ref short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReverseEndianness(ref this ulong value) => value = BinaryPrimitives.ReverseEndianness(value);
    #endregion

    #region Copy
    /// <summary>This is a no-op and exists only for consistency: a single byte has no endianness.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ReverseEndiannessCopy(ref this sbyte value) => value;

    /// <summary>This is a no-op and exists only for consistency: a single byte has no endianness.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReverseEndiannessCopy(ref this byte value) => value;

    /// <summary>Returns a copy of <paramref name="value"/> with the byte order reversed (BSWAP).</summary>
    /// <param name="value">Original value (not modified).</param>
    /// <returns>Byte-order-reversed copy.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReverseEndiannessCopy(ref this short value) => BinaryPrimitives.ReverseEndianness(value);
    /// <inheritdoc cref="ReverseEndiannessCopy(ref short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReverseEndiannessCopy(ref this ushort value) => BinaryPrimitives.ReverseEndianness(value);
    /// <inheritdoc cref="ReverseEndiannessCopy(ref short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReverseEndiannessCopy(ref this int value) => BinaryPrimitives.ReverseEndianness(value);
    /// <inheritdoc cref="ReverseEndiannessCopy(ref short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReverseEndiannessCopy(ref this uint value) => BinaryPrimitives.ReverseEndianness(value);
    /// <inheritdoc cref="ReverseEndiannessCopy(ref short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReverseEndiannessCopy(ref this long value) => BinaryPrimitives.ReverseEndianness(value);
    /// <inheritdoc cref="ReverseEndiannessCopy(ref short)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ReverseEndiannessCopy(ref this ulong value) => BinaryPrimitives.ReverseEndianness(value);
    #endregion
}
