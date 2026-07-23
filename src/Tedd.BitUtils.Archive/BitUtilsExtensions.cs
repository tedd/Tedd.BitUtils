using System;
using System.Runtime.CompilerServices;

namespace Tedd.BitUtils.Archive
{
    public static class BitUtilsArchiveExtensions
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string CreateBitString(UInt64 value, int length)
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
        private static string CreateBitString(UInt64 value, int length)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLength(UInt64 value)
        {
            if (value == 0) return 1;
            int count = 64 - value.LeadingZeroCount();
            return count > 0 ? count : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLength(Byte value)
        {
            if (value == 0) return 1;
            int count = 8 - value.LeadingZeroCount();
            return count > 0 ? count : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLength(UInt16 value)
        {
            if (value == 0) return 1;
            int count = 16 - value.LeadingZeroCount();
            return count > 0 ? count : 1;
        }

        #region ToBitStringPadded
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this Byte value) => CreateBitString(value, sizeof(Byte) * 8);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this SByte value) => CreateBitString((Byte)value, sizeof(SByte) * 8);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this Int16 value) => CreateBitString((UInt16)value, sizeof(Int16) * 8);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this UInt16 value) => CreateBitString(value, sizeof(UInt16) * 8);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this Int32 value) => CreateBitString((UInt32)value, sizeof(Int32) * 8);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this UInt32 value) => CreateBitString(value, sizeof(UInt32) * 8);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this Int64 value) => CreateBitString((UInt64)value, sizeof(Int64) * 8);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this UInt64 value) => CreateBitString(value, sizeof(UInt64) * 8);
        #endregion

        #region ToBitString
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this Byte value) => CreateBitString(value, GetLength(value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this Int16 value) => CreateBitString((UInt16)value, GetLength((UInt16)value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this UInt16 value) => CreateBitString(value, GetLength(value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this Int32 value) => CreateBitString((UInt32)value, GetLength((UInt32)value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this UInt32 value) => CreateBitString(value, GetLength(value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this Int64 value) => CreateBitString((UInt64)value, GetLength((UInt64)value));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this UInt64 value) => CreateBitString(value, GetLength(value));
        #endregion
    }
}
