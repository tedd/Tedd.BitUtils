using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Tedd;

public static partial class BitUtilsExtensions
{
    // Classic "Hacker's Delight" bit tricks, several backed directly by x86 BMI1/BMI2 instructions where available:
    //   ExtractLowestSetBit      value & -value          BLSI  (Bmi1)
    //   ResetLowestSetBit        value & (value - 1)      BLSR  (Bmi1)
    //   GetMaskUpToLowestSetBit  value ^ (value - 1)      BLSMSK (Bmi1)
    //   ZeroHighBits             keep only the low N bits BZHI  (Bmi2)
    //   ParallelBitExtract       gather masked bits       PEXT  (Bmi2)
    //   ParallelBitDeposit       scatter into masked bits PDEP  (Bmi2)
    // ExtractHighestSetBit and ZeroLowBits have no dedicated x86 instruction; they are plain arithmetic (using
    // LeadingZeroCount / a shifted mask respectively). PEXT/PDEP have no ARM64 equivalent, so a portable bit-by-bit
    // fallback is used there; every other operation here is a single masking expression on any platform.

    #region ExtractLowestSetBit
    /// <summary>Returns a copy of <paramref name="value"/> with only the lowest set bit kept (BLSI: <c>value &amp; -value</c>). Returns 0 for 0.</summary>
    /// <param name="value">Value to inspect.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ExtractLowestSetBit(ref this sbyte value) => (sbyte)(value & -value);
    /// <inheritdoc cref="ExtractLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ExtractLowestSetBit(ref this byte value) => (byte)(value & (byte)(0 - value));
    /// <inheritdoc cref="ExtractLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ExtractLowestSetBit(ref this short value) => (short)(value & -value);
    /// <inheritdoc cref="ExtractLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ExtractLowestSetBit(ref this ushort value) => (ushort)(value & (ushort)(0 - value));
    /// <inheritdoc cref="ExtractLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ExtractLowestSetBit(ref this int value) => Bmi1.IsSupported ? (int)Bmi1.ExtractLowestSetBit((uint)value) : value & -value;
    /// <inheritdoc cref="ExtractLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ExtractLowestSetBit(ref this uint value) => Bmi1.IsSupported ? Bmi1.ExtractLowestSetBit(value) : value & (0u - value);
    /// <inheritdoc cref="ExtractLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ExtractLowestSetBit(ref this long value) => Bmi1.X64.IsSupported ? (long)Bmi1.X64.ExtractLowestSetBit((ulong)value) : value & -value;
    /// <inheritdoc cref="ExtractLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ExtractLowestSetBit(ref this ulong value) => Bmi1.X64.IsSupported ? Bmi1.X64.ExtractLowestSetBit(value) : value & (0ul - value);
    #endregion

    #region ResetLowestSetBit
    /// <summary>Returns a copy of <paramref name="value"/> with the lowest set bit cleared (BLSR: <c>value &amp; (value - 1)</c>).</summary>
    /// <param name="value">Value to inspect.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ResetLowestSetBit(ref this sbyte value) => (sbyte)(value & (value - 1));
    /// <inheritdoc cref="ResetLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ResetLowestSetBit(ref this byte value) => (byte)(value & (value - 1));
    /// <inheritdoc cref="ResetLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ResetLowestSetBit(ref this short value) => (short)(value & (value - 1));
    /// <inheritdoc cref="ResetLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ResetLowestSetBit(ref this ushort value) => (ushort)(value & (value - 1));
    /// <inheritdoc cref="ResetLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ResetLowestSetBit(ref this int value) => Bmi1.IsSupported ? (int)Bmi1.ResetLowestSetBit((uint)value) : value & (value - 1);
    /// <inheritdoc cref="ResetLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ResetLowestSetBit(ref this uint value) => Bmi1.IsSupported ? Bmi1.ResetLowestSetBit(value) : value & (value - 1);
    /// <inheritdoc cref="ResetLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ResetLowestSetBit(ref this long value) => Bmi1.X64.IsSupported ? (long)Bmi1.X64.ResetLowestSetBit((ulong)value) : value & (value - 1);
    /// <inheritdoc cref="ResetLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ResetLowestSetBit(ref this ulong value) => Bmi1.X64.IsSupported ? Bmi1.X64.ResetLowestSetBit(value) : value & (value - 1);
    #endregion

    #region GetMaskUpToLowestSetBit
    /// <summary>Returns a mask with every bit up to and including the lowest set bit of <paramref name="value"/> set to 1 (BLSMSK: <c>value ^ (value - 1)</c>). Returns all-ones for 0.</summary>
    /// <param name="value">Value to inspect.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte GetMaskUpToLowestSetBit(ref this sbyte value) => (sbyte)(value ^ (value - 1));
    /// <inheritdoc cref="GetMaskUpToLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetMaskUpToLowestSetBit(ref this byte value) => (byte)(value ^ (value - 1));
    /// <inheritdoc cref="GetMaskUpToLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short GetMaskUpToLowestSetBit(ref this short value) => (short)(value ^ (value - 1));
    /// <inheritdoc cref="GetMaskUpToLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort GetMaskUpToLowestSetBit(ref this ushort value) => (ushort)(value ^ (value - 1));
    /// <inheritdoc cref="GetMaskUpToLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMaskUpToLowestSetBit(ref this int value) => Bmi1.IsSupported ? (int)Bmi1.GetMaskUpToLowestSetBit((uint)value) : value ^ (value - 1);
    /// <inheritdoc cref="GetMaskUpToLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetMaskUpToLowestSetBit(ref this uint value) => Bmi1.IsSupported ? Bmi1.GetMaskUpToLowestSetBit(value) : value ^ (value - 1);
    /// <inheritdoc cref="GetMaskUpToLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetMaskUpToLowestSetBit(ref this long value) => Bmi1.X64.IsSupported ? (long)Bmi1.X64.GetMaskUpToLowestSetBit((ulong)value) : value ^ (value - 1);
    /// <inheritdoc cref="GetMaskUpToLowestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong GetMaskUpToLowestSetBit(ref this ulong value) => Bmi1.X64.IsSupported ? Bmi1.X64.GetMaskUpToLowestSetBit(value) : value ^ (value - 1);
    #endregion

    #region ExtractHighestSetBit
    /// <summary>Returns a copy of <paramref name="value"/> with only the highest set bit kept. Returns 0 for 0.</summary>
    /// <param name="value">Value to inspect. Signed values are treated as their two's complement bit pattern.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ExtractHighestSetBit(ref this sbyte value) { byte b = (byte)value; return (sbyte)(b == 0 ? 0 : 1 << (31 - System.Numerics.BitOperations.LeadingZeroCount(b))); }
    /// <inheritdoc cref="ExtractHighestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ExtractHighestSetBit(ref this byte value) => value == 0 ? (byte)0 : (byte)(1 << (31 - System.Numerics.BitOperations.LeadingZeroCount(value)));
    /// <inheritdoc cref="ExtractHighestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ExtractHighestSetBit(ref this short value) { ushort u = (ushort)value; return (short)(u == 0 ? 0 : 1 << (31 - System.Numerics.BitOperations.LeadingZeroCount(u))); }
    /// <inheritdoc cref="ExtractHighestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ExtractHighestSetBit(ref this ushort value) => value == 0 ? (ushort)0 : (ushort)(1 << (31 - System.Numerics.BitOperations.LeadingZeroCount(value)));
    /// <inheritdoc cref="ExtractHighestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ExtractHighestSetBit(ref this int value) => value == 0 ? 0 : (int)(1u << (31 - System.Numerics.BitOperations.LeadingZeroCount((uint)value)));
    /// <inheritdoc cref="ExtractHighestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ExtractHighestSetBit(ref this uint value) => value == 0 ? 0u : 1u << (31 - System.Numerics.BitOperations.LeadingZeroCount(value));
    /// <inheritdoc cref="ExtractHighestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ExtractHighestSetBit(ref this long value) => value == 0 ? 0L : (long)(1ul << (63 - System.Numerics.BitOperations.LeadingZeroCount((ulong)value)));
    /// <inheritdoc cref="ExtractHighestSetBit(ref sbyte)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ExtractHighestSetBit(ref this ulong value) => value == 0 ? 0ul : 1ul << (63 - System.Numerics.BitOperations.LeadingZeroCount(value));
    #endregion

    #region ZeroHighBits
    /// <summary>Returns a copy of <paramref name="value"/> with every bit at position <paramref name="index"/> and above cleared, keeping only the low <paramref name="index"/> bits (BZHI).</summary>
    /// <param name="value">Value to inspect.</param>
    /// <param name="index">Number of low bits to keep. Values at or above the bit width of the type keep the value unchanged; values at or below 0 give 0.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ZeroHighBits(ref this sbyte value, int index) => (sbyte)(value & ZeroHighBitsMask32(index, 8));
    /// <inheritdoc cref="ZeroHighBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ZeroHighBits(ref this byte value, int index) => (byte)(value & ZeroHighBitsMask32(index, 8));
    /// <inheritdoc cref="ZeroHighBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ZeroHighBits(ref this short value, int index) => (short)(value & ZeroHighBitsMask32(index, 16));
    /// <inheritdoc cref="ZeroHighBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ZeroHighBits(ref this ushort value, int index) => (ushort)(value & ZeroHighBitsMask32(index, 16));
    /// <inheritdoc cref="ZeroHighBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ZeroHighBits(ref this int value, int index) => Bmi2.IsSupported ? (int)Bmi2.ZeroHighBits((uint)value, (uint)index) : (int)((uint)value & ZeroHighBitsMask32(index, 32));
    /// <inheritdoc cref="ZeroHighBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ZeroHighBits(ref this uint value, int index) => Bmi2.IsSupported ? Bmi2.ZeroHighBits(value, (uint)index) : value & ZeroHighBitsMask32(index, 32);
    /// <inheritdoc cref="ZeroHighBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ZeroHighBits(ref this long value, int index) => Bmi2.X64.IsSupported ? (long)Bmi2.X64.ZeroHighBits((ulong)value, (ulong)index) : (long)((ulong)value & ZeroHighBitsMask64(index, 64));
    /// <inheritdoc cref="ZeroHighBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ZeroHighBits(ref this ulong value, int index) => Bmi2.X64.IsSupported ? Bmi2.X64.ZeroHighBits(value, (ulong)index) : value & ZeroHighBitsMask64(index, 64);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ZeroHighBitsMask32(int index, int width) => index <= 0 ? 0u : index >= width ? uint.MaxValue : (1u << index) - 1;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ZeroHighBitsMask64(int index, int width) => index <= 0 ? 0ul : index >= width ? ulong.MaxValue : (1ul << index) - 1;
    #endregion

    #region ZeroLowBits
    /// <summary>Returns a copy of <paramref name="value"/> with every bit below position <paramref name="index"/> cleared, keeping only the bits at <paramref name="index"/> and above.</summary>
    /// <param name="value">Value to inspect.</param>
    /// <param name="index">Number of low bits to clear. Values at or above the bit width of the type give 0; values at or below 0 keep the value unchanged.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ZeroLowBits(ref this sbyte value, int index) => (sbyte)(value & ~ZeroHighBitsMask32(index, 8));
    /// <inheritdoc cref="ZeroLowBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ZeroLowBits(ref this byte value, int index) => (byte)(value & ~ZeroHighBitsMask32(index, 8));
    /// <inheritdoc cref="ZeroLowBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ZeroLowBits(ref this short value, int index) => (short)(value & ~ZeroHighBitsMask32(index, 16));
    /// <inheritdoc cref="ZeroLowBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ZeroLowBits(ref this ushort value, int index) => (ushort)(value & ~ZeroHighBitsMask32(index, 16));
    /// <inheritdoc cref="ZeroLowBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ZeroLowBits(ref this int value, int index) => (int)((uint)value & ~ZeroHighBitsMask32(index, 32));
    /// <inheritdoc cref="ZeroLowBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ZeroLowBits(ref this uint value, int index) => value & ~ZeroHighBitsMask32(index, 32);
    /// <inheritdoc cref="ZeroLowBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ZeroLowBits(ref this long value, int index) => (long)((ulong)value & ~ZeroHighBitsMask64(index, 64));
    /// <inheritdoc cref="ZeroLowBits(ref sbyte, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ZeroLowBits(ref this ulong value, int index) => value & ~ZeroHighBitsMask64(index, 64);
    #endregion

    #region ParallelBitExtract (PEXT)
    /// <summary>
    /// Gathers the bits of <paramref name="value"/> selected by the 1 bits of <paramref name="mask"/> and packs them
    /// contiguously into the low bits of the result, in mask-bit order (PEXT).
    /// </summary>
    /// <param name="value">Value to gather bits from.</param>
    /// <param name="mask">Selects which bits of <paramref name="value"/> are gathered.</param>
    /// <example>ParallelBitExtract(0b_1101_1010u, 0b_0000_1111u) == 0b1010</example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ParallelBitExtract(ref this uint value, uint mask) => Bmi2.IsSupported ? Bmi2.ParallelBitExtract(value, mask) : ParallelBitExtractSoftwareFallback(value, mask);
    /// <inheritdoc cref="ParallelBitExtract(ref uint, uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParallelBitExtract(ref this int value, int mask) { var v = (uint)value; return (int)v.ParallelBitExtract((uint)mask); }
    /// <inheritdoc cref="ParallelBitExtract(ref uint, uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ParallelBitExtract(ref this ulong value, ulong mask) => Bmi2.X64.IsSupported ? Bmi2.X64.ParallelBitExtract(value, mask) : ParallelBitExtractSoftwareFallback(value, mask);
    /// <inheritdoc cref="ParallelBitExtract(ref uint, uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ParallelBitExtract(ref this long value, long mask) { var v = (ulong)value; return (long)v.ParallelBitExtract((ulong)mask); }

    /// <summary>Portable PEXT: walks the set bits of <paramref name="mask"/> from low to high, packing the corresponding bits of <paramref name="value"/> into consecutive output bits.</summary>
    internal static uint ParallelBitExtractSoftwareFallback(uint value, uint mask)
    {
        uint result = 0;
        for (int outBit = 0; mask != 0; outBit++)
        {
            uint lowest = mask & (0u - mask); // lowest set bit of remaining mask
            if ((value & lowest) != 0)
                result |= 1u << outBit;
            mask &= mask - 1; // clear lowest set bit
        }
        return result;
    }

    /// <inheritdoc cref="ParallelBitExtractSoftwareFallback(uint, uint)"/>
    internal static ulong ParallelBitExtractSoftwareFallback(ulong value, ulong mask)
    {
        ulong result = 0;
        for (int outBit = 0; mask != 0; outBit++)
        {
            ulong lowest = mask & (0ul - mask);
            if ((value & lowest) != 0)
                result |= 1ul << outBit;
            mask &= mask - 1;
        }
        return result;
    }
    #endregion

    #region ParallelBitDeposit (PDEP)
    /// <summary>
    /// Scatters consecutive low bits of <paramref name="value"/> into the positions selected by the 1 bits of
    /// <paramref name="mask"/>, in mask-bit order; every other bit of the result is 0 (PDEP). The inverse of <see cref="ParallelBitExtract(ref uint, uint)"/>.
    /// </summary>
    /// <param name="value">Value whose low bits are scattered.</param>
    /// <param name="mask">Selects which bit positions receive bits from <paramref name="value"/>.</param>
    /// <example>ParallelBitDeposit(0b1010u, 0b_0000_1111u) == 0b_0000_1010u</example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ParallelBitDeposit(ref this uint value, uint mask) => Bmi2.IsSupported ? Bmi2.ParallelBitDeposit(value, mask) : ParallelBitDepositSoftwareFallback(value, mask);
    /// <inheritdoc cref="ParallelBitDeposit(ref uint, uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParallelBitDeposit(ref this int value, int mask) { var v = (uint)value; return (int)v.ParallelBitDeposit((uint)mask); }
    /// <inheritdoc cref="ParallelBitDeposit(ref uint, uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ParallelBitDeposit(ref this ulong value, ulong mask) => Bmi2.X64.IsSupported ? Bmi2.X64.ParallelBitDeposit(value, mask) : ParallelBitDepositSoftwareFallback(value, mask);
    /// <inheritdoc cref="ParallelBitDeposit(ref uint, uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ParallelBitDeposit(ref this long value, long mask) { var v = (ulong)value; return (long)v.ParallelBitDeposit((ulong)mask); }

    /// <summary>Portable PDEP: walks the set bits of <paramref name="mask"/> from low to high, scattering consecutive input bits of <paramref name="value"/> into each one.</summary>
    internal static uint ParallelBitDepositSoftwareFallback(uint value, uint mask)
    {
        uint result = 0;
        for (int inBit = 0; mask != 0; inBit++)
        {
            uint lowest = mask & (0u - mask);
            if ((value & (1u << inBit)) != 0)
                result |= lowest;
            mask &= mask - 1;
        }
        return result;
    }

    /// <inheritdoc cref="ParallelBitDepositSoftwareFallback(uint, uint)"/>
    internal static ulong ParallelBitDepositSoftwareFallback(ulong value, ulong mask)
    {
        ulong result = 0;
        for (int inBit = 0; mask != 0; inBit++)
        {
            ulong lowest = mask & (0ul - mask);
            if ((value & (1ul << inBit)) != 0)
                result |= lowest;
            mask &= mask - 1;
        }
        return result;
    }
    #endregion
}
