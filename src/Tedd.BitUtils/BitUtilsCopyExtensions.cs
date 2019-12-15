using System;
using System.Runtime.CompilerServices;

namespace Tedd
{
    public static class BitUtilsCopyExtensions
    {
        #region Copy

        #region Rol count

        // Note. RyuJIT will compile this to a single rotate CPU instruction (as of about .NET 4.6.1 and dotnet core 2.0).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 RolCopy(ref this Int32 value, int count) => (Int32)(((UInt32)value << count) | ((UInt32)value >> (32 - count)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 RolCopy(ref this UInt32 value, int count) => (UInt32)(((UInt64)value << count) | ((UInt64)value >> (32 - count)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 RolCopy(ref this Int64 value, int count) => (Int64)(((UInt64)value << count) | ((UInt64)value >> (64 - count)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 RolCopy(ref this UInt64 value, int count) => (UInt64)(((UInt64)value << count) | ((UInt64)value >> (64 - count)));

        #endregion

        #region Ror count

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 RorCopy(ref this Int32 value, int count) => (Int32)(((UInt32)value << (32 - count)) | ((UInt32)value >> count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 RorCopy(ref this UInt32 value, int count) => (UInt32)(((UInt32)value << (32 - count)) | ((UInt32)value >> count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 RorCopy(ref this Int64 value, int count) => (Int64)(((UInt64)value << (64 - count)) | ((UInt64)value >> count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 RorCopy(ref this UInt64 value, int count) => (UInt64)(((UInt64)value << (64 - count)) | ((UInt64)value >> count));

        #endregion

        #region Rol 1

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 RolCopy(ref this Int32 value) => (Int32)(((UInt32)value << 1) | ((UInt32)value >> 31));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 RolCopy(ref this UInt32 value) => (UInt32)(((UInt32)value << 1) | ((UInt32)value >> 31));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 RolCopy(ref this Int64 value) => (Int64)(((UInt64)value << 1) | ((UInt64)value >> 63));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 RolCopy(ref this UInt64 value) => (UInt64)(((UInt64)value << 1) | ((UInt64)value >> 63));

        #endregion

        #region Ror 1

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 RorCopy(ref this Int32 value) => (Int32)(((UInt32)value << 31) | ((UInt32)value >> 1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 RorCopy(ref this UInt32 value) => (UInt32)(((UInt32)value << 31) | ((UInt32)value >> 1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 RorCopy(ref this Int64 value) => (Int64)(((UInt64)value << 63) | ((UInt64)value >> 1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 RorCopy(ref this UInt64 value) => (UInt64)(((UInt64)value << 63) | ((UInt64)value >> 1));

        #endregion

        #region SetBit bool

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte SetBitCopy(ref this Byte value, int pos, bool state) => (state ? (Byte)(value | (1 << pos)) : (Byte)(value & ~(1 << pos)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 SetBitCopy(ref this Int16 value, int pos, bool state) => state ? (Int16)((UInt16)value | ((UInt16)1 << pos)) : (Int16)((UInt16)value & ~((UInt16)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 SetBitCopy(ref this UInt16 value, int pos, bool state) =>
            (state
                ? (UInt16)((UInt16)value | ((UInt16)1 << pos))
                : (UInt16)((UInt16)value & ~((UInt16)1 << pos)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 SetBitCopy(ref this Int32 value, int pos, bool state) =>
            (state
                ? (Int32)((UInt32)value | ((UInt32)1 << pos))
                : (Int32)((UInt32)value & ~((UInt32)1 << pos)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 SetBitCopy(ref this UInt32 value, int pos, bool state) =>
            (state
                ? (UInt32)((UInt32)value | ((UInt32)1 << pos))
                : (UInt32)(UInt32)(value & ~((UInt32)1 << pos)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 SetBitCopy(ref this Int64 value, int pos, bool state) =>
            (state
                ? (Int64)((UInt64)value | ((UInt64)1 << pos))
                : (Int64)((UInt64)value & ~((UInt64)1 << pos)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 SetBitCopy(ref this UInt64 value, int pos, bool state) =>
            (state
                ? (UInt64)((UInt64)value | ((UInt64)1 << pos))
                : (UInt64)((UInt64)value & ~((UInt64)1 << pos)));

        #endregion

        #region SetBit0

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte SetBit0Copy(ref this Byte value, int pos) => (Byte)(value & ~(1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 SetBit0Copy(ref this Int16 value, int pos) => (Int16)((UInt16)value & ~((UInt16)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 SetBit0Copy(ref this UInt16 value, int pos) => (UInt16)((UInt16)value & ~((UInt16)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 SetBit0Copy(ref this Int32 value, int pos) => (Int32)((UInt32)value & ~((UInt32)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 SetBit0Copy(ref this UInt32 value, int pos) => (UInt32)((UInt32)value & ~((UInt32)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 SetBit0Copy(ref this Int64 value, int pos) => (Int64)((Int64)value & ~((Int64)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 SetBit0Copy(ref this UInt64 value, int pos) => (UInt64)((UInt64)value & ~((UInt64)1 << pos));

        #endregion


        #region SetBit1

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte SetBit1Copy(ref this Byte value, int pos) => (Byte)(value | (1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 SetBit1Copy(ref this Int16 value, int pos) => (Int16)((UInt16)value | ((UInt16)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 SetBit1Copy(ref this UInt16 value, int pos) => (UInt16)((UInt16)value | ((UInt16)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 SetBit1Copy(ref this Int32 value, int pos) => (Int32)((UInt32)value | ((UInt32)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 SetBit1Copy(ref this UInt32 value, int pos) => (UInt32)((UInt32)value | ((UInt32)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 SetBit1Copy(ref this Int64 value, int pos) => (Int64)((UInt64)value | ((UInt64)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 SetBit1Copy(ref this UInt64 value, int pos) => (UInt64)((UInt64)value | ((UInt64)1 << pos));

        #endregion


        #region ReverseBits
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte ReverseBitsCopy(ref this Byte value) => BitUtilsExtensions.BitReverseLookup[value];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 ReverseBitsCopy(ref this Int16 value) => (Int16)((UInt16)BitUtilsExtensions.BitReverseLookup[(UInt16)value >> 8]
                                                                              | (UInt16)((UInt16)BitUtilsExtensions.BitReverseLookup[(UInt16)value & 0xFF] << (UInt16)8));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 ReverseBitsCopy(ref this UInt16 value) => (UInt16)((UInt16)BitUtilsExtensions.BitReverseLookup[(UInt16)value >> 8]
                                                                                | (UInt16)((UInt16)BitUtilsExtensions.BitReverseLookup[(UInt16)value & 0xFF] << (UInt16)8));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 ReverseBitsCopy(ref this Int32 value) => (Int32)((UInt32)BitUtilsExtensions.BitReverseLookup[(Int32)((UInt32)value >> 24)]
                                                                              | (UInt32)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt32)value >> 16) & 0xFF)] << 8
                                                                              | (UInt32)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt32)value >> 8) & 0xFF)] << 16
                                                                              | (UInt32)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt32)value & 0xFF))] << 24);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 ReverseBitsCopy(ref this UInt32 value) => (UInt32)((UInt32)BitUtilsExtensions.BitReverseLookup[(Int32)((UInt32)value >> 24)]
                                                                                | (UInt32)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt32)value >> 16) & 0xFF)] << 8
                                                                                | (UInt32)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt32)value >> 8) & 0xFF)] << 16
                                                                                | (UInt32)BitUtilsExtensions.BitReverseLookup[(Int32)((UInt32)value & 0xFF)] << 24);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 ReverseBitsCopy(ref this Int64 value) => (Int64)((UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)((UInt64)value >> 56)]
                                                                              | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 48) & 0xFF)] << 8
                                                                              | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 40) & 0xFF)] << 16
                                                                              | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 32) & 0xFF)] << 24
                                                                              | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 24) & 0xFF)] << 32
                                                                              | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 16) & 0xFF)] << 40
                                                                              | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 8) & 0xFF)] << 48
                                                                              | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)((UInt64)value & 0xFF)] << 56);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 ReverseBitsCopy(ref this UInt64 value) => (UInt64)((UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)((UInt64)value >> 56)]
                                                                                | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 48) & 0xFF)] << 8
                                                                                | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 40) & 0xFF)] << 16
                                                                                | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 32) & 0xFF)] << 24
                                                                                | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 24) & 0xFF)] << 32
                                                                                | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 16) & 0xFF)] << 40
                                                                                | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)(((UInt64)value >> 8) & 0xFF)] << 48
                                                                                | (UInt64)BitUtilsExtensions.BitReverseLookup[(Int32)((UInt64)value & 0xFF)] << 56);

        #endregion

        #endregion

    }
}