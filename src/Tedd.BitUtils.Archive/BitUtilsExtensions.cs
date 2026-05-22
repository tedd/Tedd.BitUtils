using System;
using System.Runtime.CompilerServices;

namespace Tedd.BitUtils.Archive
{
    public static class BitUtilsArchiveExtensions
    {
        #region ToBitStringPadded
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this Byte value) => Convert.ToString(value, 2).PadLeft(sizeof(Byte) * 8, '0');
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this SByte value) => Convert.ToString(value, 2).PadLeft(sizeof(SByte) * 8, '0');
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this Int16 value) => Convert.ToString((UInt32)value & 0xFFFF, 2).PadLeft(sizeof(Int16) * 8, '0');

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this UInt16 value) => Convert.ToString((UInt32)value, 2).PadLeft(sizeof(UInt16) * 8, '0');
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this Int32 value) => Convert.ToString((Int32)value, 2).PadLeft(sizeof(Int32) * 8, '0');
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this UInt32 value) => Convert.ToString((Int32)value, 2).PadLeft(sizeof(UInt32) * 8, '0');
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this Int64 value) => Convert.ToString((Int64)value, 2).PadLeft(sizeof(Int64) * 8, '0');
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitStringPadded(ref this UInt64 value) => Convert.ToString((Int64)value, 2).PadLeft(sizeof(UInt64) * 8, '0');
        #endregion

        #region ToBitString
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this Byte value) => Convert.ToString(value, 2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this Int16 value) => Convert.ToString((UInt32)value & 0xFFFF, 2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this UInt16 value) => Convert.ToString((UInt32)value, 2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this Int32 value) => Convert.ToString((Int32)value, 2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this UInt32 value) => Convert.ToString((Int32)value, 2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this Int64 value) => Convert.ToString((Int64)value, 2);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToBitString(ref this UInt64 value) => Convert.ToString((Int64)value, 2);
        #endregion
    }
}
