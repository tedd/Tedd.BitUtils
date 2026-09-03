using System;
using System.Runtime.CompilerServices;

namespace Tedd.BitUtils.Archive
{
    public static class BitUtilsArchiveExtensions
    {
        #region ToBitStringHelpers
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
            int count = 64 - LeadingZeroCount(value);
            return count > 0 ? count : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLength(UInt32 value)
        {
            if (value == 0) return 1;
            int count = 32 - LeadingZeroCount(value);
            return count > 0 ? count : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLength(Byte value)
        {
            if (value == 0) return 1;
            int count = 8 - LeadingZeroCount((UInt32)value);
            return count > 0 ? count : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLength(UInt16 value)
        {
            if (value == 0) return 1;
            int count = 16 - LeadingZeroCount((UInt32)value);
            return count > 0 ? count : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LeadingZeroCount(UInt32 value)
        {
#if NETCOREAPP3_0_OR_GREATER
            return System.Numerics.BitOperations.LeadingZeroCount(value);
#else
            if (value == 0) return 32;
            int count = 0;
            if ((value & 0xFFFF0000) == 0) { count += 16; value <<= 16; }
            if ((value & 0xFF000000) == 0) { count += 8; value <<= 8; }
            if ((value & 0xF0000000) == 0) { count += 4; value <<= 4; }
            if ((value & 0xC0000000) == 0) { count += 2; value <<= 2; }
            if ((value & 0x80000000) == 0) { count += 1; }
            return count;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LeadingZeroCount(UInt64 value)
        {
#if NETCOREAPP3_0_OR_GREATER
            return System.Numerics.BitOperations.LeadingZeroCount(value);
#else
            if (value == 0) return 64;
            int count = 0;
            if ((value & 0xFFFFFFFF00000000) == 0) { count += 32; value <<= 32; }
            if ((value & 0xFFFF000000000000) == 0) { count += 16; value <<= 16; }
            if ((value & 0xFF00000000000000) == 0) { count += 8; value <<= 8; }
            if ((value & 0xF000000000000000) == 0) { count += 4; value <<= 4; }
            if ((value & 0xC000000000000000) == 0) { count += 2; value <<= 2; }
            if ((value & 0x8000000000000000) == 0) { count += 1; }
            return count;
#endif
        }
        #endregion

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
