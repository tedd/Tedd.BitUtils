using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tedd;

public static partial class BitUtilsExtensions
{
    // PopCount, Parity, LeadingZeroCount, TrailingZeroCount, Log2 and BitLength.
    // All backed by System.Numerics.BitOperations, which the JIT recognises as POPCNT/LZCNT/TZCNT/BSR on x86-64 and
    // CNT/CLZ/RBIT on ARM64, with an automatic portable software fallback on CPUs without the matching instruction.
    // The internal *SoftwareFallback methods below are the portable algorithms exercised directly by the test suite;
    // shipped code never branches on them since BitOperations already does the equivalent CPU-capability check.

    #region Software fallbacks (exercised by tests via InternalsVisibleTo)
    private static ReadOnlySpan<byte> Log2DeBruijn =>
    [
        00, 09, 01, 10, 13, 21, 02, 29,
        11, 14, 16, 18, 22, 25, 03, 30,
        08, 12, 20, 28, 15, 17, 24, 07,
        19, 27, 23, 06, 26, 05, 04, 31,
    ];

    private static ReadOnlySpan<byte> TrailingZeroCountDeBruijn =>
    [
        00, 01, 28, 02, 29, 14, 24, 03,
        30, 22, 20, 15, 25, 17, 04, 08,
        31, 27, 13, 23, 21, 19, 16, 07,
        26, 12, 18, 06, 11, 05, 10, 09,
    ];

    /// <summary>Portable population count (SWAR).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int PopCountSoftwareFallback(uint value)
    {
        value -= (value >> 1) & 0x_55555555u;
        value = (value & 0x_33333333u) + ((value >> 2) & 0x_33333333u);
        value = (((value + (value >> 4)) & 0x_0F0F0F0Fu) * 0x_01010101u) >> 24;
        return (int)value;
    }

    /// <inheritdoc cref="PopCountSoftwareFallback(uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int PopCountSoftwareFallback(ulong value)
    {
        value -= (value >> 1) & 0x_55555555_55555555UL;
        value = (value & 0x_33333333_33333333UL) + ((value >> 2) & 0x_33333333_33333333UL);
        value = (((value + (value >> 4)) & 0x_0F0F0F0F_0F0F0F0FUL) * 0x_01010101_01010101UL) >> 56;
        return (int)value;
    }

    /// <summary>Portable floor(log2(value)) using a de Bruijn sequence. Returns 0 for 0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Log2SoftwareFallback(uint value)
    {
        // Fill all bits below the highest set bit, e.g. 00010010 becomes 00011111.
        value |= value >> 01;
        value |= value >> 02;
        value |= value >> 04;
        value |= value >> 08;
        value |= value >> 16;
        // (value * 0x07C4ACDD) >> 27 is always in [0, 31].
        return Log2DeBruijn[(int)((value * 0x07C4ACDDu) >> 27)];
    }

    /// <inheritdoc cref="Log2SoftwareFallback(uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Log2SoftwareFallback(ulong value)
    {
        uint hi = (uint)(value >> 32);
        return hi != 0 ? 32 + Log2SoftwareFallback(hi) : Log2SoftwareFallback((uint)value);
    }

    /// <summary>Portable leading zero count. Returns 32 for 0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LeadingZeroCountSoftwareFallback(uint value)
        => value == 0 ? 32 : 31 - Log2SoftwareFallback(value);

    /// <summary>Portable leading zero count. Returns 64 for 0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int LeadingZeroCountSoftwareFallback(ulong value)
    {
        uint hi = (uint)(value >> 32);
        return hi != 0 ? LeadingZeroCountSoftwareFallback(hi) : 32 + LeadingZeroCountSoftwareFallback((uint)value);
    }

    /// <summary>Portable trailing zero count using a de Bruijn sequence. Returns 32 for 0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int TrailingZeroCountSoftwareFallback(uint value)
        // Isolate the lowest set bit, then ((bit * 0x077CB531) >> 27) is always in [0, 31].
        => value == 0 ? 32 : TrailingZeroCountDeBruijn[(int)(((value & (0u - value)) * 0x077CB531u) >> 27)];

    /// <summary>Portable trailing zero count. Returns 64 for 0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int TrailingZeroCountSoftwareFallback(ulong value)
    {
        uint lo = (uint)value;
        return lo != 0 ? TrailingZeroCountSoftwareFallback(lo) : 32 + TrailingZeroCountSoftwareFallback((uint)(value >> 32));
    }

    /// <summary>Portable parity (number of set bits modulo 2) by xor folding.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ParitySoftwareFallback(uint value)
    {
        value ^= value >> 16;
        value ^= value >> 8;
        value ^= value >> 4;
        return (0x6996 >> (int)(value & 0xF)) & 1;
    }

    /// <inheritdoc cref="ParitySoftwareFallback(uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int ParitySoftwareFallback(ulong value)
        => ParitySoftwareFallback((uint)(value ^ (value >> 32)));
    #endregion

    #region PopCount
    /// <summary>Returns the number of bits set to 1 (population count, POPCNT).</summary>
    /// <param name="value">Value to inspect. Signed values are counted as their two's complement bit pattern.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ref this sbyte value) => BitOperations.PopCount((byte)value);
    /// <inheritdoc cref="PopCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ref this byte value) => BitOperations.PopCount(value);
    /// <inheritdoc cref="PopCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ref this short value) => BitOperations.PopCount((ushort)value);
    /// <inheritdoc cref="PopCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ref this ushort value) => BitOperations.PopCount(value);
    /// <inheritdoc cref="PopCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ref this int value) => BitOperations.PopCount((uint)value);
    /// <inheritdoc cref="PopCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ref this uint value) => BitOperations.PopCount(value);
    /// <inheritdoc cref="PopCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ref this long value) => BitOperations.PopCount((ulong)value);
    /// <inheritdoc cref="PopCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ref this ulong value) => BitOperations.PopCount(value);
    #endregion

    #region Parity
    /// <summary>Returns the parity of the value: 1 when an odd number of bits are set, 0 when an even number of bits are set.</summary>
    /// <param name="value">Value to inspect. Signed values are treated as their two's complement bit pattern.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Parity(ref this sbyte value) => BitOperations.PopCount((byte)value) & 1;
    /// <inheritdoc cref="Parity(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Parity(ref this byte value) => BitOperations.PopCount(value) & 1;
    /// <inheritdoc cref="Parity(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Parity(ref this short value) => BitOperations.PopCount((ushort)value) & 1;
    /// <inheritdoc cref="Parity(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Parity(ref this ushort value) => BitOperations.PopCount(value) & 1;
    /// <inheritdoc cref="Parity(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Parity(ref this int value) => BitOperations.PopCount((uint)value) & 1;
    /// <inheritdoc cref="Parity(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Parity(ref this uint value) => BitOperations.PopCount(value) & 1;
    /// <inheritdoc cref="Parity(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Parity(ref this long value) => BitOperations.PopCount((ulong)value) & 1;
    /// <inheritdoc cref="Parity(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Parity(ref this ulong value) => BitOperations.PopCount(value) & 1;
    #endregion

    #region LeadingZeroCount
    /// <summary>Returns the number of consecutive 0 bits starting from the most significant bit (LZCNT). Returns the bit width of the type for 0.</summary>
    /// <param name="value">Value to inspect. Signed values are treated as their two's complement bit pattern, so negative values return 0.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ref this sbyte value) => BitOperations.LeadingZeroCount((byte)value) - 24;
    /// <inheritdoc cref="LeadingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ref this byte value) => BitOperations.LeadingZeroCount(value) - 24;
    /// <inheritdoc cref="LeadingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ref this short value) => BitOperations.LeadingZeroCount((ushort)value) - 16;
    /// <inheritdoc cref="LeadingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ref this ushort value) => BitOperations.LeadingZeroCount(value) - 16;
    /// <inheritdoc cref="LeadingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ref this int value) => BitOperations.LeadingZeroCount((uint)value);
    /// <inheritdoc cref="LeadingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ref this uint value) => BitOperations.LeadingZeroCount(value);
    /// <inheritdoc cref="LeadingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ref this long value) => BitOperations.LeadingZeroCount((ulong)value);
    /// <inheritdoc cref="LeadingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ref this ulong value) => BitOperations.LeadingZeroCount(value);
    #endregion

    #region TrailingZeroCount
    /// <summary>Returns the number of consecutive 0 bits starting from the least significant bit (TZCNT). Returns the bit width of the type for 0.</summary>
    /// <param name="value">Value to inspect. Signed values are treated as their two's complement bit pattern.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(ref this sbyte value) => BitOperations.TrailingZeroCount((byte)value | 0x100u);
    /// <inheritdoc cref="TrailingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(ref this byte value) => BitOperations.TrailingZeroCount(value | 0x100u);
    /// <inheritdoc cref="TrailingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(ref this short value) => BitOperations.TrailingZeroCount((ushort)value | 0x10000u);
    /// <inheritdoc cref="TrailingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(ref this ushort value) => BitOperations.TrailingZeroCount(value | 0x10000u);
    /// <inheritdoc cref="TrailingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(ref this int value) => BitOperations.TrailingZeroCount((uint)value);
    /// <inheritdoc cref="TrailingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(ref this uint value) => BitOperations.TrailingZeroCount(value);
    /// <inheritdoc cref="TrailingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(ref this long value) => BitOperations.TrailingZeroCount((ulong)value);
    /// <inheritdoc cref="TrailingZeroCount(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(ref this ulong value) => BitOperations.TrailingZeroCount(value);
    #endregion

    #region Log2
    /// <summary>Returns the integer base 2 logarithm, i.e. the position of the highest set bit (floor(log2(value))). Returns 0 for 0 (mathematically undefined).</summary>
    /// <param name="value">Value to inspect. Signed values are treated as their two's complement bit pattern, so negative values return the highest bit position.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(ref this sbyte value) => BitOperations.Log2((byte)value);
    /// <inheritdoc cref="Log2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(ref this byte value) => BitOperations.Log2(value);
    /// <inheritdoc cref="Log2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(ref this short value) => BitOperations.Log2((ushort)value);
    /// <inheritdoc cref="Log2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(ref this ushort value) => BitOperations.Log2(value);
    /// <inheritdoc cref="Log2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(ref this int value) => BitOperations.Log2((uint)value);
    /// <inheritdoc cref="Log2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(ref this uint value) => BitOperations.Log2(value);
    /// <inheritdoc cref="Log2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(ref this long value) => BitOperations.Log2((ulong)value);
    /// <inheritdoc cref="Log2(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(ref this ulong value) => BitOperations.Log2(value);
    #endregion

    #region BitLength
    /// <summary>Returns the number of bits needed to represent the value, i.e. the position of the highest set bit plus one. Returns 0 for 0.</summary>
    /// <param name="value">Value to inspect. Signed values are treated as their two's complement bit pattern, so negative values return the full bit width.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BitLength(ref this sbyte value) => 32 - BitOperations.LeadingZeroCount((byte)value);
    /// <inheritdoc cref="BitLength(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BitLength(ref this byte value) => 32 - BitOperations.LeadingZeroCount(value);
    /// <inheritdoc cref="BitLength(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BitLength(ref this short value) => 32 - BitOperations.LeadingZeroCount((ushort)value);
    /// <inheritdoc cref="BitLength(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BitLength(ref this ushort value) => 32 - BitOperations.LeadingZeroCount(value);
    /// <inheritdoc cref="BitLength(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BitLength(ref this int value) => 32 - BitOperations.LeadingZeroCount((uint)value);
    /// <inheritdoc cref="BitLength(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BitLength(ref this uint value) => 32 - BitOperations.LeadingZeroCount(value);
    /// <inheritdoc cref="BitLength(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BitLength(ref this long value) => 64 - BitOperations.LeadingZeroCount((ulong)value);
    /// <inheritdoc cref="BitLength(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BitLength(ref this ulong value) => 64 - BitOperations.LeadingZeroCount(value);
    #endregion
}
