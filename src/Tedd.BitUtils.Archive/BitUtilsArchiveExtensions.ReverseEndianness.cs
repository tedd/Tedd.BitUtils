using System;
using System.Runtime.CompilerServices;

namespace Tedd.BitUtils.Archive
{
    public static partial class BitUtilsArchiveExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseEndianness(ref UInt32 value)
        {
            var a = (value & 0x00FF00FFu);
            a = (a >> 8) | (a << (32 - 8));
            var b = (value & 0xFF00FF00u);
            b = (b << 8) | (b >> (32 - 8));
            value = a + b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseEndianness(ref UInt64 value)
        {
            var a = (UInt32)value;
            ReverseEndianness(ref a);
            var b = (UInt32)(value >> 32);
            ReverseEndianness(ref b);
            value = ((UInt64)a << 32) + (UInt64)b;
        }
    }
}
