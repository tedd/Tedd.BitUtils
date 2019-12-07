using System;
using System.Runtime.CompilerServices;

namespace Tedd
{
    public static class BitUtilsExtensions
    {
        #region In-place
        #region Rol count
        // Note. RyuJIT will compile this to a single rotate CPU instruction (as of about .NET 4.6.1 and dotnet core 2.0).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this Int32 value, int count) => value = (Int32)(((UInt32)value << count) | ((UInt32)value >> (32 - count)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this UInt32 value, int count) => value = (UInt32)(((UInt32)value << count) | ((UInt32)value >> (32 - count)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this Int64 value, int count) => value = (Int64)(((UInt64)value << count) | ((UInt64)value >> (64 - count)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this UInt64 value, int count) => value = (UInt64)(((UInt64)value << count) | ((UInt64)value >> (64 - count)));
        #endregion
        #region Ror count
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this Int32 value, int count) => value = (Int32)(((UInt32)value << (32 - count)) | ((UInt32)value >> count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this UInt32 value, int count) => value = (UInt32)(((UInt32)value << (32 - count)) | ((UInt32)value >> count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this Int64 value, int count) => value = (Int64)(((UInt64)value << (64 - count)) | ((UInt64)value >> count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this UInt64 value, int count) => value = (UInt64)(((UInt64)value << (64 - count)) | ((UInt64)value >> count));
        #endregion

        #region Rol 1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this Int32 value) => value = (Int32)(((UInt32)value << 1) | ((UInt32)value >> (32 - 1)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this UInt32 value) => value = (UInt32)(((UInt32)value << 1) | ((UInt32)value >> (32 - 1)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this Int64 value) => value = (Int64)(((UInt64)value << 1) | ((UInt64)value >> (64 - 1)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this UInt64 value) => value = (UInt64)(((UInt64)value << 1) | ((UInt64)value >> (64 - 1)));
        #endregion

        #region Ror 1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this Int32 value) => value = (Int32)(((UInt32)value << (32 - 1)) | ((UInt32)value >> 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this UInt32 value) => value = (UInt32)(((UInt32)value << (32 - 1)) | ((UInt32)value >> 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this Int64 value) => value = (Int64)(((UInt64)value << (64 - 1)) | ((UInt64)value >> 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this UInt64 value) => value = (UInt64)(((UInt64)value << (64 - 1)) | ((UInt64)value >> 1));
        #endregion

        #region SetBit bool
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this Byte value, int pos, bool state) => value = (state ? (Byte)(value | (1 << pos)) : (Byte)(value & ~(1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this Int16 value, int pos, bool state) => value = (state ? (Int16)((UInt16)value | ((UInt16)1 << pos)) : (Int16)((UInt16)value & ~((UInt16)1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this UInt16 value, int pos, bool state) => value = (state ? (UInt16)((UInt16)value | ((UInt16)1 << pos)) : (UInt16)((UInt16)value & ~((UInt16)1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this Int32 value, int pos, bool state) => value = (state ? (Int32)((UInt32)value | ((UInt32)1 << pos)) : (Int32)unchecked((UInt32)value & ~((UInt32)1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this UInt32 value, int pos, bool state) => value = (state ? (UInt32)((UInt32)value | ((UInt32)1 << pos)) : (UInt32)(UInt32)((UInt32)value & ~((UInt32)1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this Int64 value, int pos, bool state) => value = (state ? (Int64)((UInt64)value | ((UInt64)1 << pos)) : (Int64)unchecked((UInt64)value & ~((UInt64)1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this UInt64 value, int pos, bool state) => value = (state ? (UInt64)((UInt64)value | ((UInt64)1 << pos)) : (UInt64)((UInt64)value & ~((UInt64)1 << pos)));
        #endregion

        #region SetBit0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this Byte value, int pos) => value = (Byte)(value & ~(1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this Int16 value, int pos) => value = (Int16)((UInt16)value & ~((UInt16)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this UInt16 value, int pos) => value = (UInt16)((UInt16)value & ~((UInt16)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this Int32 value, int pos) => value = (Int32)((UInt32)value & ~((UInt32)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this UInt32 value, int pos) => value = (UInt32)((UInt32)value & ~((UInt32)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this Int64 value, int pos) => value = (Int64)((Int64)value & ~((Int64)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this UInt64 value, int pos) => value = (UInt64)((UInt64)value & ~((UInt64)1 << pos));
        #endregion


        #region SetBit1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this Byte value, int pos) => value = (Byte)(value | (1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this Int16 value, int pos) => value = (Int16)((UInt16)value | ((UInt16)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this UInt16 value, int pos) => value = (UInt16)((UInt16)value | ((UInt16)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this Int32 value, int pos) => value = (Int32)((UInt32)value | ((UInt32)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this UInt32 value, int pos) => value = (UInt32)((UInt32)value | ((UInt32)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this Int64 value, int pos) => value = (Int64)((Int64)value | ((Int64)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this UInt64 value, int pos) => value = (UInt64)((UInt64)value | ((UInt64)1 << pos));
        #endregion
        #endregion

        #region IsBitSet
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this Byte value, int pos) => (Byte)((Byte)value & ((Byte)1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this Int16 value, int pos) => (Int16)((UInt16)value & ((UInt16)1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this UInt16 value, int pos) => (UInt16)((UInt16)value & ((UInt16)1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this Int32 value, int pos) => (Int32)((UInt32)value & ((UInt32)1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this UInt32 value, int pos) => (UInt32)((UInt32)value & ((UInt32)1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this Int64 value, int pos) => (Int64)((UInt64)value & ((UInt64)1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this UInt64 value, int pos) => (UInt64)((value & ((UInt64)1 << pos))) != 0;
        #endregion


        internal static readonly byte[] BitReverseLookup =
        {
            0x00, 0x80, 0x40, 0xc0, 0x20, 0xa0, 0x60, 0xe0,
            0x10, 0x90, 0x50, 0xd0, 0x30, 0xb0, 0x70, 0xf0,
            0x08, 0x88, 0x48, 0xc8, 0x28, 0xa8, 0x68, 0xe8,
            0x18, 0x98, 0x58, 0xd8, 0x38, 0xb8, 0x78, 0xf8,
            0x04, 0x84, 0x44, 0xc4, 0x24, 0xa4, 0x64, 0xe4,
            0x14, 0x94, 0x54, 0xd4, 0x34, 0xb4, 0x74, 0xf4,
            0x0c, 0x8c, 0x4c, 0xcc, 0x2c, 0xac, 0x6c, 0xec,
            0x1c, 0x9c, 0x5c, 0xdc, 0x3c, 0xbc, 0x7c, 0xfc,
            0x02, 0x82, 0x42, 0xc2, 0x22, 0xa2, 0x62, 0xe2,
            0x12, 0x92, 0x52, 0xd2, 0x32, 0xb2, 0x72, 0xf2,
            0x0a, 0x8a, 0x4a, 0xca, 0x2a, 0xaa, 0x6a, 0xea,
            0x1a, 0x9a, 0x5a, 0xda, 0x3a, 0xba, 0x7a, 0xfa,
            0x06, 0x86, 0x46, 0xc6, 0x26, 0xa6, 0x66, 0xe6,
            0x16, 0x96, 0x56, 0xd6, 0x36, 0xb6, 0x76, 0xf6,
            0x0e, 0x8e, 0x4e, 0xce, 0x2e, 0xae, 0x6e, 0xee,
            0x1e, 0x9e, 0x5e, 0xde, 0x3e, 0xbe, 0x7e, 0xfe,
            0x01, 0x81, 0x41, 0xc1, 0x21, 0xa1, 0x61, 0xe1,
            0x11, 0x91, 0x51, 0xd1, 0x31, 0xb1, 0x71, 0xf1,
            0x09, 0x89, 0x49, 0xc9, 0x29, 0xa9, 0x69, 0xe9,
            0x19, 0x99, 0x59, 0xd9, 0x39, 0xb9, 0x79, 0xf9,
            0x05, 0x85, 0x45, 0xc5, 0x25, 0xa5, 0x65, 0xe5,
            0x15, 0x95, 0x55, 0xd5, 0x35, 0xb5, 0x75, 0xf5,
            0x0d, 0x8d, 0x4d, 0xcd, 0x2d, 0xad, 0x6d, 0xed,
            0x1d, 0x9d, 0x5d, 0xdd, 0x3d, 0xbd, 0x7d, 0xfd,
            0x03, 0x83, 0x43, 0xc3, 0x23, 0xa3, 0x63, 0xe3,
            0x13, 0x93, 0x53, 0xd3, 0x33, 0xb3, 0x73, 0xf3,
            0x0b, 0x8b, 0x4b, 0xcb, 0x2b, 0xab, 0x6b, 0xeb,
            0x1b, 0x9b, 0x5b, 0xdb, 0x3b, 0xbb, 0x7b, 0xfb,
            0x07, 0x87, 0x47, 0xc7, 0x27, 0xa7, 0x67, 0xe7,
            0x17, 0x97, 0x57, 0xd7, 0x37, 0xb7, 0x77, 0xf7,
            0x0f, 0x8f, 0x4f, 0xcf, 0x2f, 0xaf, 0x6f, 0xef,
            0x1f, 0x9f, 0x5f, 0xdf, 0x3f, 0xbf, 0x7f, 0xff
        };

        #region ReverseBits
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseBits(ref this Byte value) => value = BitReverseLookup[value];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseBits(ref this Int16 value) => value = (Int16)((UInt16)BitReverseLookup[(UInt16)value >> 8] 
                                                                              | (UInt16)((UInt16)BitReverseLookup[(UInt16)value & 0xFF] << (UInt16)8));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseBits(ref this UInt16 value) => value = (UInt16)((UInt16)BitReverseLookup[(UInt16)value >> 8] 
                                                                                | (UInt16)((UInt16)BitReverseLookup[(UInt16)value & 0xFF] << (UInt16)8));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseBits(ref this Int32 value) => value = (Int32)((UInt32)BitReverseLookup[(UInt32)value >> 24]
                                                                              | (UInt32)BitReverseLookup[((UInt32)value >> 16) & 0xFF] << 8
                                                                              | (UInt32)BitReverseLookup[((UInt32)value >> 8) & 0xFF] << 16
                                                                              | (UInt32)BitReverseLookup[((UInt32)value & 0xFF)] << 24);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseBits(ref this UInt32 value) => value = (UInt32)((UInt32)BitReverseLookup[(UInt32)value >> 24]
                                                                                | (UInt32)BitReverseLookup[((UInt32)value >> 16) & 0xFF] << 8
                                                                                | (UInt32)BitReverseLookup[((UInt32)value >> 8) & 0xFF] << 16
                                                                                | (UInt32)BitReverseLookup[(UInt32)value & 0xFF] << 24);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseBits(ref this Int64 value) => value = (Int64)((UInt64)BitReverseLookup[(UInt64)value >> 56]
                                                                              | (UInt64)BitReverseLookup[((UInt64)value >> 48) & 0xFF] << 8
                                                                              | (UInt64)BitReverseLookup[((UInt64)value >> 40) & 0xFF] << 16
                                                                              | (UInt64)BitReverseLookup[((UInt64)value >> 32) & 0xFF] << 24
                                                                              | (UInt64)BitReverseLookup[((UInt64)value >> 24) & 0xFF] << 32
                                                                              | (UInt64)BitReverseLookup[((UInt64)value >> 16) & 0xFF] << 40
                                                                              | (UInt64)BitReverseLookup[((UInt64)value >> 8) & 0xFF] << 48
                                                                              | (UInt64)BitReverseLookup[(UInt64)value & 0xFF] << 56);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseBits(ref this UInt64 value) => value = (UInt64)((UInt64)BitReverseLookup[(UInt64)value >> 56]
                                                                                | (UInt64)BitReverseLookup[((UInt64)value >> 48) & 0xFF] << 8
                                                                                | (UInt64)BitReverseLookup[((UInt64)value >> 40) & 0xFF] << 16
                                                                                | (UInt64)BitReverseLookup[((UInt64)value >> 32) & 0xFF] << 24
                                                                                | (UInt64)BitReverseLookup[((UInt64)value >> 24) & 0xFF] << 32
                                                                                | (UInt64)BitReverseLookup[((UInt64)value >> 16) & 0xFF] << 40
                                                                                | (UInt64)BitReverseLookup[((UInt64)value >> 8) & 0xFF] << 48
                                                                                | (UInt64)BitReverseLookup[(UInt64)value & 0xFF] << 56);

        #endregion

        // Broken for now
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static void Pack(ref this Int32 packed, int offset, int length, uint value)
        //{
        //    // Clear excess data in value and move it into position
        //    value &= (~0U >> sizeof(Int32) - length)<< offset;

        //    //if (offset + length > sizeof(Int32))
        //    //    throw new Exception($"Bit out of bounds"); // + ": Offset " + offset + " + length " + length + " > 32

        //    // Clear bit mask
        //    var clearMask = (Int32)~((~0U >> (sizeof(Int32) - length)) << offset);

        //    // Clear space for value
        //    packed &= clearMask;
        //    // Apply value
        //    packed |= (Int32)value;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static uint Unpack(ref this uint packed, int offset, int length)
        //{
        //    var v = packed >> offset;
        //    v = v & (((uint)1 << length) - 1);
        //    return v;
        //}


        //public static bool IsSet(this byte[] array, int pos)
        //{
        //    var mask = 1 << (7 - (pos % 8));
        //    return (array[pos / 8] & mask) != 0;
        //}
    }
}
