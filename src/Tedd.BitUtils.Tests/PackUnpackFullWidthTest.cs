using System;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    /// <summary>
    /// The original Pack/Unpack for Int32/UInt32/Int64/UInt64 computed their mask as "(1 &lt;&lt; length) - 1"
    /// (or "(1 &lt;&lt; offset) - 1" for Unpack). When length/offset equals the full width of the type, that shift
    /// count equals the type's own bit width, which C# defines as a no-op shift (count is taken modulo the width),
    /// silently producing a mask of 0 instead of all-ones. These tests cover exactly that boundary, which the
    /// original loop-bounded tests (offset up to width-1) never reached.
    /// </summary>
    public class PackUnpackFullWidthTest
    {
        [Fact]
        public void Pack_Int32_FullWidthReplacesEntireValue()
        {
            var packed = 0x12345678;
            var value = unchecked((int)0xAABBCCDD);
            packed.Pack(32, 32, value);
            Assert.Equal(value, packed);
        }

        [Fact]
        public void Pack_UInt32_FullWidthReplacesEntireValue()
        {
            var packed = 0x12345678u;
            var value = 0xAABBCCDDu;
            packed.Pack(32, 32, value);
            Assert.Equal(value, packed);
        }

        [Fact]
        public void Pack_Int64_FullWidthReplacesEntireValue()
        {
            var packed = 0x1234567890ABCDEFL;
            var value = unchecked((long)0xAABBCCDDEEFF0011UL);
            packed.Pack(64, 64, value);
            Assert.Equal(value, packed);
        }

        [Fact]
        public void Pack_UInt64_FullWidthReplacesEntireValue()
        {
            var packed = 0x1234567890ABCDEFuL;
            var value = 0xAABBCCDDEEFF0011uL;
            packed.Pack(64, 64, value);
            Assert.Equal(value, packed);
        }

        [Fact]
        public void Unpack_Int32_FullWidthReturnsWholeValue()
        {
            var value = unchecked((int)0xAABBCCDD);
            Assert.Equal(value, value.Unpack(32, 32));
        }

        [Fact]
        public void Unpack_UInt32_FullWidthReturnsWholeValue()
        {
            var value = 0xAABBCCDDu;
            Assert.Equal(value, value.Unpack(32, 32));
        }

        [Fact]
        public void Unpack_Int64_FullWidthReturnsWholeValue()
        {
            var value = unchecked((long)0xAABBCCDDEEFF0011UL);
            Assert.Equal(value, value.Unpack(64, 64));
        }

        [Fact]
        public void Unpack_UInt64_FullWidthReturnsWholeValue()
        {
            var value = 0xAABBCCDDEEFF0011uL;
            Assert.Equal(value, value.Unpack(64, 64));
        }

        [Fact]
        public void PackThenUnpack_Int32_FullWidth_RoundTrips()
        {
            var packed = 0;
            var value = unchecked((int)0xDEADBEEF);
            packed.Pack(32, 32, value);
            Assert.Equal(value, packed.Unpack(32, 32));
        }

        [Fact]
        public void Pack_UInt32_HighHalfOnly()
        {
            // A 16-bit field ending at bit 32 (the top half) is a realistic case near the boundary this bug affected.
            var packed = 0x0000FFFFu;
            var value = 0xBEEFu;
            packed.Pack(32, 16, value);
            Assert.Equal(0xBEEFFFFFu, packed);
        }
    }
}
