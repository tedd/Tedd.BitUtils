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
            int count = 64 - LeadingZeroCountSoftwareFallback(value);
            return count > 0 ? count : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLength(UInt32 value)
        {
            if (value == 0) return 1;
            int count = 32 - LeadingZeroCountSoftwareFallback(value);
            return count > 0 ? count : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLength(Byte value)
        {
            if (value == 0) return 1;
            int count = 8 - LeadingZeroCountSoftwareFallback(value);
            return count > 0 ? count : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLength(UInt16 value)
        {
            if (value == 0) return 1;
            int count = 16 - LeadingZeroCountSoftwareFallback(value);
            return count > 0 ? count : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LeadingZeroCountSoftwareFallback(UInt64 value)
        {
            if (value == 0) return 64;
            var upper = (UInt32)(value >> 32);
            var n = upper > 0
                ? Log2SoftwareFallback(upper) + 32
                : Log2SoftwareFallback((UInt32)value);
            return 63 - n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LeadingZeroCountSoftwareFallback(UInt32 value)
        {
            if (value == 0) return 32;
            return 31 - Log2SoftwareFallback(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LeadingZeroCountSoftwareFallback(UInt16 value)
        {
            if (value == 0) return 16;
            return 15 - Log2SoftwareFallback((UInt32)value & 0xFFFF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LeadingZeroCountSoftwareFallback(Byte value)
        {
            if (value == 0) return 8;
            return 7 - Log2SoftwareFallback((UInt32)value & 0xFF);
        }

#if !BEFORENETCOREAPP3
        private static ReadOnlySpan<byte> Log2DeBruijn => new byte[32]
#else
        private static byte[] Log2DeBruijn =
#endif
        {
            00, 09, 01, 10, 13, 21, 02, 29,
            11, 14, 16, 18, 22, 25, 03, 30,
            08, 12, 20, 28, 15, 17, 24, 07,
            19, 27, 23, 06, 26, 05, 04, 31
        };

        private static int Log2SoftwareFallback(uint value)
        {
            value |= value >> 01;
            value |= value >> 02;
            value |= value >> 04;
            value |= value >> 08;
            value |= value >> 16;
#if !BEFORENETCOREAPP3
            return System.Runtime.CompilerServices.Unsafe.AddByteOffset(
                ref System.Runtime.InteropServices.MemoryMarshal.GetReference(Log2DeBruijn),
                (IntPtr)(int)((value * 0x07C4ACDDu) >> 27));
#else
            return Log2DeBruijn[(value * 0x07C4ACDDu) >> 27];
#endif
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
