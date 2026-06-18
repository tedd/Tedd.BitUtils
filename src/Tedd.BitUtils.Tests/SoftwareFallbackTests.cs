using System;
using System.Reflection;
using Xunit;

namespace Tedd.BitUtils.Tests
{
    public class SoftwareFallbackTests
    {
        private static int InvokePopCntSoftwareFallback(uint value)
        {
            var method = typeof(BitUtilsExtensions).GetMethod("PopCntSoftwareFallback", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(uint) }, null);
            return (int)method.Invoke(null, new object[] { value });
        }

        private static int InvokePopCntSoftwareFallback(ulong value)
        {
            var method = typeof(BitUtilsExtensions).GetMethod("PopCntSoftwareFallback", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(ulong) }, null);
            return (int)method.Invoke(null, new object[] { value });
        }

        private static int InvokeLzCntSoftwareFallback(Byte value)
        {
            var method = typeof(BitUtilsExtensions).GetMethod("LzCntSoftwareFallback", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(Byte) }, null);
            return (int)method.Invoke(null, new object[] { value });
        }

        private static int InvokeLzCntSoftwareFallback(UInt16 value)
        {
            var method = typeof(BitUtilsExtensions).GetMethod("LzCntSoftwareFallback", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(UInt16) }, null);
            return (int)method.Invoke(null, new object[] { value });
        }

        private static int InvokeLzCntSoftwareFallback(UInt32 value)
        {
            var method = typeof(BitUtilsExtensions).GetMethod("LzCntSoftwareFallback", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(UInt32) }, null);
            return (int)method.Invoke(null, new object[] { value });
        }

        private static int InvokeLzCntSoftwareFallback(UInt64 value)
        {
            var method = typeof(BitUtilsExtensions).GetMethod("LzCntSoftwareFallback", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(UInt64) }, null);
            return (int)method.Invoke(null, new object[] { value });
        }

        [Theory]
        [InlineData(0u, 0)]
        [InlineData(1u, 1)]
        [InlineData(0xFFFFFFFFu, 32)]
        [InlineData(0x0F0F0F0Fu, 16)]
        [InlineData(0x55555555u, 16)]
        public void PopCntSoftwareFallback_UInt32_Matches(uint input, int expected)
        {
            Assert.Equal(expected, InvokePopCntSoftwareFallback(input));
        }

        [Theory]
        [InlineData(0ul, 0)]
        [InlineData(1ul, 1)]
        [InlineData(0xFFFFFFFFFFFFFFFFul, 64)]
        [InlineData(0x0F0F0F0F0F0F0F0Ful, 32)]
        [InlineData(0x5555555555555555ul, 32)]
        public void PopCntSoftwareFallback_UInt64_Matches(ulong input, int expected)
        {
            Assert.Equal(expected, InvokePopCntSoftwareFallback(input));
        }

        [Theory]
        [InlineData(0, 8)]
        [InlineData(1, 7)]
        [InlineData(0xFF, 0)]
        [InlineData(0x80, 0)]
        [InlineData(0x40, 1)]
        public void LzCntSoftwareFallback_Byte_Matches(byte input, int expected)
        {
            Assert.Equal(expected, InvokeLzCntSoftwareFallback(input));
        }

        [Theory]
        [InlineData(0, 16)]
        [InlineData(1, 15)]
        [InlineData(0xFFFF, 0)]
        [InlineData(0x8000, 0)]
        [InlineData(0x4000, 1)]
        public void LzCntSoftwareFallback_UInt16_Matches(ushort input, int expected)
        {
            Assert.Equal(expected, InvokeLzCntSoftwareFallback(input));
        }

        [Theory]
        [InlineData(0u, 32)]
        [InlineData(1u, 31)]
        [InlineData(0xFFFFFFFFu, 0)]
        [InlineData(0x80000000u, 0)]
        [InlineData(0x40000000u, 1)]
        public void LzCntSoftwareFallback_UInt32_Matches(uint input, int expected)
        {
            Assert.Equal(expected, InvokeLzCntSoftwareFallback(input));
        }

        [Theory]
        [InlineData(0ul, 64)]
        [InlineData(1ul, 63)]
        [InlineData(0xFFFFFFFFFFFFFFFFul, 0)]
        [InlineData(0x8000000000000000ul, 0)]
        [InlineData(0x4000000000000000ul, 1)]
        [InlineData(0x00000000FFFFFFFFul, 32)]
        [InlineData(0x0000000080000000ul, 32)]
        public void LzCntSoftwareFallback_UInt64_Matches(ulong input, int expected)
        {
            Assert.Equal(expected, InvokeLzCntSoftwareFallback(input));
        }
    }
}
