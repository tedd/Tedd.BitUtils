using Xunit;

namespace Tedd.BitUtils.Tests;

/// <summary>
/// ZeroHighBits documents what an index outside 0..width does: at or above the width the value is kept, at or below
/// zero the result is 0. On .NET 6 and later the 32 and 64 bit overloads hand the index straight to BZHI, which reads
/// only the low 8 bits of it - so an index of -1 or 256 means something entirely different to the instruction than it
/// does to the portable mask. These tests hold every overload and every build to the one documented answer.
/// </summary>
/// <remarks>
/// The same assertions run against the intrinsic paths and, under <c>-p:ForcePortable=true</c>, against the portable
/// ones. That is the point: the two shipped builds of this package have to agree, otherwise the same call gives one
/// answer on .NET 8 and another on the .NET Standard 2.1 asset a Unity or Xamarin consumer resolves to.
/// </remarks>
public class ZeroHighBitsContractTest
{
    // Indices far outside any type's width, on both sides. 256 and -256 matter specifically because they are the
    // values whose low 8 bits are zero, which is what BZHI would read as "keep no bits".
    public static TheoryData<int> OutOfRangeIndices => [-1, -7, -8, -32, -64, -255, -256, -257, 65, 128, 255, 256, 257, 264, int.MaxValue, int.MinValue];

    [Theory]
    [MemberData(nameof(OutOfRangeIndices))]
    public void NegativeIndexGivesZero_AndIndexAboveWidthKeepsTheValue_ForEveryWidth(int index)
    {
        var expectKeep = index > 0;   // index >= width for every width tested here, since |index| > 64

        sbyte i8 = -1;
        byte u8 = 0xFF;
        short i16 = -1;
        ushort u16 = 0xFFFF;
        var i32 = -1;
        var u32 = 0xFFFF_FFFFu;
        var i64 = -1L;
        var u64 = 0xFFFF_FFFF_FFFF_FFFFUL;

        Assert.Equal(expectKeep ? (sbyte)-1 : (sbyte)0, i8.ZeroHighBits(index));
        Assert.Equal(expectKeep ? (byte)0xFF : (byte)0, u8.ZeroHighBits(index));
        Assert.Equal(expectKeep ? (short)-1 : (short)0, i16.ZeroHighBits(index));
        Assert.Equal(expectKeep ? (ushort)0xFFFF : (ushort)0, u16.ZeroHighBits(index));
        Assert.Equal(expectKeep ? -1 : 0, i32.ZeroHighBits(index));
        Assert.Equal(expectKeep ? 0xFFFF_FFFFu : 0u, u32.ZeroHighBits(index));
        Assert.Equal(expectKeep ? -1L : 0L, i64.ZeroHighBits(index));
        Assert.Equal(expectKeep ? 0xFFFF_FFFF_FFFF_FFFFUL : 0UL, u64.ZeroHighBits(index));
    }

    [Fact]
    public void EveryWidthAgreesWithEveryOtherWidth_OnTheSameOutOfRangeIndex()
    {
        // The narrow overloads have no intrinsic path at all, so they were always right. This pins the wide ones to
        // them rather than to a separately computed expectation.
        foreach (var index in new[] { -1, -256, 256, 264, int.MinValue, int.MaxValue })
        {
            byte narrow = 0xFF;
            var wide = 0xFFu;
            var wide64 = 0xFFUL;

            var fromNarrow = narrow.ZeroHighBits(index);
            Assert.Equal(fromNarrow, (byte)wide.ZeroHighBits(index));
            Assert.Equal(fromNarrow, (byte)wide64.ZeroHighBits(index));
        }
    }

    [Fact]
    public void InRangeIndices_KeepExactlyTheLowIndexBits()
    {
        // The behaviour that was already correct, kept here so a fix to the out-of-range case cannot silently break
        // the ordinary one.
        for (var index = 0; index <= 32; index++)
        {
            var value = 0xFFFF_FFFFu;
            var expected = index == 0 ? 0u : index >= 32 ? 0xFFFF_FFFFu : (1u << index) - 1;
            Assert.Equal(expected, value.ZeroHighBits(index));
        }

        for (var index = 0; index <= 64; index++)
        {
            var value = 0xFFFF_FFFF_FFFF_FFFFUL;
            var expected = index == 0 ? 0UL : index >= 64 ? 0xFFFF_FFFF_FFFF_FFFFUL : (1UL << index) - 1;
            Assert.Equal(expected, value.ZeroHighBits(index));
        }
    }

    [Fact]
    public void ZeroHighBitsAndZeroLowBits_StayComplementary_AtOutOfRangeIndices()
    {
        // ZeroLowBits has no intrinsic path, so this invariant is exactly where a BZHI-only divergence surfaces.
        foreach (var index in new[] { -1, 0, 1, 31, 32, 33, 256, int.MaxValue, int.MinValue })
        {
            var value = 0xDEAD_BEEFu;
            Assert.Equal(value, value.ZeroHighBits(index) | value.ZeroLowBits(index));
        }
    }
}
