using System;
using System.Runtime.CompilerServices;

namespace Tedd.BitUtils.Archive
{
    public static partial class BitUtilsArchiveExtensions
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CreateBitString(UInt64 value, int length)
        {
            return string.Create(length, value, (span, v) =>
            {
                for (int i = span.Length - 1; i >= 0; i--)
                {
                    span[i] = (v & 1) == 1 ? '1' : '0';
                    v >>= 1;
                }
            });
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CreateBitString(UInt64 value, int length)
        {
            char[] chars = new char[length];
            for (int i = length - 1; i >= 0; i--)
            {
                chars[i] = (value & 1) == 1 ? '1' : '0';
                value >>= 1;
            }
            return new string(chars);
        }
#endif
    }
}
