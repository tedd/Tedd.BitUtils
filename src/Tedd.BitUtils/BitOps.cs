using System.Runtime.CompilerServices;
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
using System.Numerics;
#endif

namespace Tedd;

/// <summary>
/// The one place in this library that reaches for <c>System.Numerics.BitOperations</c>, so that supporting a
/// target framework without it needs a change here and nowhere else.
/// </summary>
/// <remarks>
/// <para>On .NET 6 and later every member forwards straight to <c>System.Numerics.BitOperations</c>, which the JIT
/// recognises and compiles to POPCNT/LZCNT/TZCNT/BSR/ROL/ROR on x86-64 and CNT/CLZ/RBIT on ARM64, itself falling back
/// to portable code on CPUs without the matching instruction. These wrappers are all aggressively inlined, so the
/// indirection costs nothing: the JIT sees the intrinsic at the original call site.</para>
/// <para>On .NET Standard 2.1 <c>System.Numerics.BitOperations</c> does not exist (it arrived in .NET Core 3.0), so
/// each member uses the portable software implementation instead. Those live next to the operations they belong to,
/// as <c>BitUtilsExtensions.*SoftwareFallback</c>, are compiled on every target framework, and are covered directly
/// by the test suite - so the .NET Standard 2.1 build runs algorithms that the .NET 8/10 test runs have verified
/// against the hardware backed results.</para>
/// </remarks>
internal static class BitOps
{
    /// <summary>Returns the number of bits set to 1.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(uint value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.PopCount(value);
#else
        BitUtilsExtensions.PopCountSoftwareFallback(value);
#endif

    /// <inheritdoc cref="PopCount(uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ulong value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.PopCount(value);
#else
        BitUtilsExtensions.PopCountSoftwareFallback(value);
#endif

    /// <summary>Returns the number of leading zero bits. Returns 32 for 0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(uint value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.LeadingZeroCount(value);
#else
        BitUtilsExtensions.LeadingZeroCountSoftwareFallback(value);
#endif

    /// <summary>Returns the number of leading zero bits. Returns 64 for 0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LeadingZeroCount(ulong value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.LeadingZeroCount(value);
#else
        BitUtilsExtensions.LeadingZeroCountSoftwareFallback(value);
#endif

    /// <summary>Returns the number of trailing zero bits. Returns 32 for 0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(uint value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.TrailingZeroCount(value);
#else
        BitUtilsExtensions.TrailingZeroCountSoftwareFallback(value);
#endif

    /// <summary>Returns the number of trailing zero bits. Returns 64 for 0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TrailingZeroCount(ulong value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.TrailingZeroCount(value);
#else
        BitUtilsExtensions.TrailingZeroCountSoftwareFallback(value);
#endif

    /// <summary>Returns floor(log2(value)). Returns 0 for 0.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(uint value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.Log2(value);
#else
        BitUtilsExtensions.Log2SoftwareFallback(value);
#endif

    /// <inheritdoc cref="Log2(uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(ulong value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.Log2(value);
#else
        BitUtilsExtensions.Log2SoftwareFallback(value);
#endif

    /// <summary>Rotates the bits left, wrapping the bits shifted out at the top back in at the bottom.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RotateLeft(uint value, int offset) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.RotateLeft(value, offset);
#else
        BitUtilsExtensions.RotateLeftSoftwareFallback(value, offset);
#endif

    /// <inheritdoc cref="RotateLeft(uint, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RotateLeft(ulong value, int offset) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.RotateLeft(value, offset);
#else
        BitUtilsExtensions.RotateLeftSoftwareFallback(value, offset);
#endif

    /// <summary>Rotates the bits right, wrapping the bits shifted out at the bottom back in at the top.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RotateRight(uint value, int offset) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.RotateRight(value, offset);
#else
        BitUtilsExtensions.RotateRightSoftwareFallback(value, offset);
#endif

    /// <inheritdoc cref="RotateRight(uint, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RotateRight(ulong value, int offset) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.RotateRight(value, offset);
#else
        BitUtilsExtensions.RotateRightSoftwareFallback(value, offset);
#endif

    /// <summary>Returns <see langword="true"/> when exactly one bit is set.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(uint value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.IsPow2(value);
#else
        BitUtilsExtensions.IsPow2SoftwareFallback(value);
#endif

    /// <inheritdoc cref="IsPow2(uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPow2(ulong value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.IsPow2(value);
#else
        BitUtilsExtensions.IsPow2SoftwareFallback(value);
#endif

    /// <summary>Rounds up to the nearest power of two. 0 stays 0, as does a value whose next power of two does not fit.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RoundUpToPowerOf2(uint value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.RoundUpToPowerOf2(value);
#else
        BitUtilsExtensions.RoundUpToPowerOf2SoftwareFallback(value);
#endif

    /// <inheritdoc cref="RoundUpToPowerOf2(uint)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RoundUpToPowerOf2(ulong value) =>
#if NET6_0_OR_GREATER && !BITUTILS_PORTABLE
        BitOperations.RoundUpToPowerOf2(value);
#else
        BitUtilsExtensions.RoundUpToPowerOf2SoftwareFallback(value);
#endif
}
