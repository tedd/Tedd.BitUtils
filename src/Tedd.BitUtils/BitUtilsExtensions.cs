using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
        // Tedd.MoreRandom should support Byte, Int16, Int64 and stuff + static copy of random-class

#if !BEFORENETCOREAPP3
        // https://github.com/dotnet/roslyn/pull/24621
        internal static ReadOnlySpan<byte> BitReverseLookup => new byte[256]
#else
        internal static byte[] BitReverseLookup =
#endif
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
        public static void ReverseBits(ref this Int32 value) => value = (Int32)((UInt32)BitReverseLookup[(Int32)((UInt32)value >> 24)]
                                                                              | (UInt32)BitReverseLookup[(Int32)(((UInt32)value >> 16) & 0xFF)] << 8
                                                                              | (UInt32)BitReverseLookup[(Int32)(((UInt32)value >> 8) & 0xFF)] << 16
                                                                              | (UInt32)BitReverseLookup[(Int32)(((UInt32)value & 0xFF))] << 24);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseBits(ref this UInt32 value) => value = (UInt32)((UInt32)BitReverseLookup[(Int32)((UInt32)value >> 24)]
                                                                                | (UInt32)BitReverseLookup[(Int32)(((UInt32)value >> 16) & 0xFF)] << 8
                                                                                | (UInt32)BitReverseLookup[(Int32)(((UInt32)value >> 8) & 0xFF)] << 16
                                                                                | (UInt32)BitReverseLookup[(Int32)((UInt32)value & 0xFF)] << 24);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseBits(ref this Int64 value) => value = (Int64)((UInt64)BitReverseLookup[(Int32)((UInt64)value >> 56)]
                                                                              | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 48) & 0xFF)] << 8
                                                                              | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 40) & 0xFF)] << 16
                                                                              | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 32) & 0xFF)] << 24
                                                                              | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 24) & 0xFF)] << 32
                                                                              | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 16) & 0xFF)] << 40
                                                                              | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 8) & 0xFF)] << 48
                                                                              | (UInt64)BitReverseLookup[(Int32)((UInt64)value & 0xFF)] << 56);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseBits(ref this UInt64 value) => value = (UInt64)((UInt64)BitReverseLookup[(Int32)((UInt64)value >> 56)]
                                                                                | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 48) & 0xFF)] << 8
                                                                                | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 40) & 0xFF)] << 16
                                                                                | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 32) & 0xFF)] << 24
                                                                                | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 24) & 0xFF)] << 32
                                                                                | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 16) & 0xFF)] << 40
                                                                                | (UInt64)BitReverseLookup[(Int32)(((UInt64)value >> 8) & 0xFF)] << 48
                                                                                | (UInt64)BitReverseLookup[(Int32)((UInt64)value & 0xFF)] << 56);

        #endregion

        #region PopCont
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int PopCntSoftwareFallback(uint value)
        {
            const uint c1 = 0x_55555555u;
            const uint c2 = 0x_33333333u;
            const uint c3 = 0x_0F0F0F0Fu;
            const uint c4 = 0x_01010101u;

            value -= (value >> 1) & c1;
            value = (value & c2) + ((value >> 2) & c2);
            value = (((value + (value >> 4)) & c3) * c4) >> 24;

            return (int)value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int PopCntSoftwareFallback(ulong value)
        {
            const ulong c1 = 0x_55555555_55555555ul;
            const ulong c2 = 0x_33333333_33333333ul;
            const ulong c3 = 0x_0F0F0F0F_0F0F0F0Ful;
            const ulong c4 = 0x_01010101_01010101ul;

            value -= (value >> 1) & c1;
            value = (value & c2) + ((value >> 2) & c2);
            value = (((value + (value >> 4)) & c3) * c4) >> 56;

            return (int)value;
        }

#if !BEFORENETCOREAPP3
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this Byte value) => System.Runtime.Intrinsics.X86.Popcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Popcnt.PopCount((UInt32)value) : PopCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this Int16 value) => System.Runtime.Intrinsics.X86.Popcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Popcnt.PopCount((UInt32)value & 0xFFFF) : PopCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this UInt16 value) => System.Runtime.Intrinsics.X86.Popcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Popcnt.PopCount((UInt32)value) : PopCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this Int32 value) => System.Runtime.Intrinsics.X86.Popcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Popcnt.PopCount((UInt32)value) : PopCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this UInt32 value) => System.Runtime.Intrinsics.X86.Popcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Popcnt.PopCount((UInt32)value) : PopCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this Int64 value) => System.Runtime.Intrinsics.X86.Popcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Popcnt.X64.PopCount((UInt64)value) : PopCntSoftwareFallback((UInt64)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this UInt64 value) => System.Runtime.Intrinsics.X86.Popcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Popcnt.X64.PopCount((UInt64)value) : PopCntSoftwareFallback((UInt64)value);
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this Byte value) => PopCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this Int16 value) => PopCntSoftwareFallback((UInt32)value & 0xFFFF);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this UInt16 value) => PopCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this Int32 value) => PopCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this UInt32 value) => PopCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this Int64 value) => PopCntSoftwareFallback((UInt64)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PopCount(ref this UInt64 value) => PopCntSoftwareFallback((UInt64)value);
#endif

        #endregion


        #region LZCNT
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int LzCntSoftwareFallback(Byte value)
        {
            // Unguarded fallback contract is 0->31
            if (value == 0)
                return 8;

            return 7 - Log2SoftwareFallback((UInt32)value & 0xFF);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int LzCntSoftwareFallback(UInt16 value)
        {
            // Unguarded fallback contract is 0->31
            if (value == 0)
                return 16;

            return 15 - Log2SoftwareFallback((UInt32)value & 0xFFFF);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int LzCntSoftwareFallback(UInt32 value)
        {
            // Unguarded fallback contract is 0->31
            if (value == 0)
                return 32;

            return 31 - Log2SoftwareFallback(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int LzCntSoftwareFallback(UInt64 value)
        {
            // Unguarded fallback contract is 0->63
            if (value == 0)
                return 64;

            var n = Log2SoftwareFallback((UInt32)(value >> 32));
            if (n > 0)
                n += 32;
            else
                n = Log2SoftwareFallback((UInt32)value);

            return 63 - n;
        }

#if !BEFORENETCOREAPP3

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this Byte value) => System.Runtime.Intrinsics.X86.Lzcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Lzcnt.LeadingZeroCount((UInt32)value) - 24 : LzCntSoftwareFallback(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this Int16 value) => System.Runtime.Intrinsics.X86.Lzcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Lzcnt.LeadingZeroCount((UInt32)value & 0xFFFF) - 16 : LzCntSoftwareFallback((UInt16)value) ;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this UInt16 value) => System.Runtime.Intrinsics.X86.Lzcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Lzcnt.LeadingZeroCount((UInt32)value) - 16 : LzCntSoftwareFallback((UInt16)value) ;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this Int32 value) => System.Runtime.Intrinsics.X86.Lzcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Lzcnt.LeadingZeroCount((UInt32)value) : LzCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this UInt32 value) => System.Runtime.Intrinsics.X86.Lzcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Lzcnt.LeadingZeroCount((UInt32)value) : LzCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this Int64 value) => System.Runtime.Intrinsics.X86.Lzcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Lzcnt.X64.LeadingZeroCount((UInt64)value) : LzCntSoftwareFallback((UInt64)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this UInt64 value) => System.Runtime.Intrinsics.X86.Lzcnt.IsSupported ? (Int32)System.Runtime.Intrinsics.X86.Lzcnt.X64.LeadingZeroCount((UInt64)value) : LzCntSoftwareFallback((UInt64)value);
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this Byte value) => LzCntSoftwareFallback(value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this Int16 value) => LzCntSoftwareFallback((UInt16)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this UInt16 value) => LzCntSoftwareFallback((UInt16)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this Int32 value) => LzCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this UInt32 value) => LzCntSoftwareFallback((UInt32)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this Int64 value) => LzCntSoftwareFallback((UInt64)value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 LeadingZeroCount(ref this UInt64 value) => LzCntSoftwareFallback((UInt64)value);
#endif

        #endregion
        #region Log2

#if !BEFORENETCOREAPP3
        // https://github.com/dotnet/roslyn/pull/24621
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
            // No AggressiveInlining due to large method size
            // Has conventional contract 0->0 (Log(0) is undefined)

            // Fill trailing zeros with ones, eg 00010010 becomes 00011111
            value |= value >> 01;
            value |= value >> 02;
            value |= value >> 04;
            value |= value >> 08;
            value |= value >> 16;

#if !BEFORENETCOREAPP3
            // uint.MaxValue >> 27 is always in range [0 - 31] so we use Unsafe.AddByteOffset to avoid bounds check
            return Unsafe.AddByteOffset(
                // Using deBruijn sequence, k=2, n=5 (2^5=32) : 0b_0000_0111_1100_0100_1010_1100_1101_1101u
                ref MemoryMarshal.GetReference(Log2DeBruijn),
                // uint|long -> IntPtr cast on 32-bit platforms does expensive overflow checks not needed here
                (IntPtr)(int)((value * 0x07C4ACDDu) >> 27));
#else
            return Log2DeBruijn[(value * 0x07C4ACDDu) >> 27];
#endif
        }
        #endregion

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
        #endregion|
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
        #endregion|

        //#region FromBitString

        //public static Byte BinaryStringToByte(string @string) => Convert.ToByte(@string, 2);
        //public static Int32 BinaryStringToInt32(string @string) => Convert.ToInt32(@string, 2);

        //#endregion
        #region Pack
        #region In-place
        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack(ref this SByte packed, int offset, int length, SByte value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            packed = (SByte)((packed & ~mask) | ((value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack(ref this Byte packed, int offset, int length, Byte value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            packed = (byte)((packed & ~mask) | ((value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack(ref this UInt16 packed, int offset, int length, UInt16 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            packed = (UInt16)((packed & ~mask) | ((value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack(ref this Int16 packed, int offset, int length, Int16 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            packed = (Int16)((packed & ~mask) | ((value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack(ref this UInt32 packed, int offset, int length, UInt32 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            packed = (UInt32)(((Int32)packed & ~mask) | (((Int32)value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack(ref this Int32 packed, int offset, int length, Int32 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            packed = (Int32)((packed & ~mask) | ((value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack(ref this UInt64 packed, int offset, int length, UInt64 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int64)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            packed = (UInt64)(((Int64)packed & ~mask) | (((Int64)value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack(ref this Int64 packed, int offset, int length, Int64 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int64)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            packed = (Int64)((packed & ~mask) | ((value << (offset - length)) & mask));
        }
        #endregion
        #region Copy
        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        /// <returns>Modified copy.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SByte PackCopy(this SByte packed, int offset, int length, SByte value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            return (SByte)((packed & ~mask) | ((value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        /// <returns>Modified copy.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte PackCopy(this Byte packed, int offset, int length, Byte value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            return (byte)((packed & ~mask) | ((value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        /// <returns>Modified copy.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 PackCopy(this UInt16 packed, int offset, int length, UInt16 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            return (UInt16)((packed & ~mask) | ((value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        /// <returns>Modified copy.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 PackCopy(this Int16 packed, int offset, int length, Int16 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            return (Int16)((packed & ~mask) | ((value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        /// <returns>Modified copy.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 PackCopy(this UInt32 packed, int offset, int length, UInt32 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            return (UInt32)(((Int32)packed & ~mask) | (((Int32)value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        /// <returns>Modified copy.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 PackCopy(this Int32 packed, int offset, int length, Int32 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int32)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            return (Int32)((packed & ~mask) | ((value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        /// <returns>Modified copy.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 PackCopy(this UInt64 packed, int offset, int length, UInt64 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int64)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            return (UInt64)(((Int64)packed & ~mask) | (((Int64)value << (offset - length)) & mask));
        }

        /// <summary>
        /// Packs bits into integer.
        /// </summary>
        /// <param name="packed"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <param name="value">Value to insert, only <para>length</para> least significant bits will be used.</param>
        /// <returns>Modified copy.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 PackCopy(this Int64 packed, int offset, int length, Int64 value)
        {
            // mask value by length and shift up to correct position
            var mask = (((Int64)1 << (length)) - 1) << (offset - length);
            // (clear by reverse mask) OR with (shift value up, apply mask)
            return (Int64)((packed & ~mask) | ((value << (offset - length)) & mask));
        }
        #endregion
        #endregion

        #region Unpack
        /// <summary>
        /// Extract a subset of bits. Works similarly to SubString(), except on bits and offset is from right side.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <returns>Extracted bits shifted down.</returns>
        /// <example>var value = 0b00000000_10011001;
        /// value.ExtractBits(5, 2) == 0b11</example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SByte Unpack(ref this SByte value, int offset, int length) => (SByte)(((UInt16)value & (UInt16)(((Int16)1 << offset) - 1)) >> (offset - length));
        /// <summary>
        /// Extract a subset of bits. Works similarly to SubString(), except on bits and offset is from right side.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <returns>Extracted bits shifted down.</returns>
        /// <example>var value = 0b00000000_10011001;
        /// value.ExtractBits(5, 2) == 0b11</example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte Unpack(ref this Byte value, int offset, int length) => (Byte)(((UInt16)value & (UInt16)(((Int16)1 << offset) - 1)) >> (offset - length));
        /// <summary>
        /// Extract a subset of bits. Works similarly to SubString(), except on bits and offset is from right side.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <returns>Extracted bits shifted down.</returns>
        /// <example>var value = 0b00000000_10011001;
        /// value.ExtractBits(5, 2) == 0b11</example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 Unpack(ref this UInt16 value, int offset, int length) => (UInt16)(((UInt16)value & (UInt16)(((Int16)1 << offset) - 1)) >> (offset - length));
        /// <summary>
        /// Extract a subset of bits. Works similarly to SubString(), except on bits and offset is from right side.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <returns>Extracted bits shifted down.</returns>
        /// <example>var value = 0b00000000_10011001;
        /// value.ExtractBits(5, 2) == 0b11</example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 Unpack(ref this Int16 value, int offset, int length) => (Int16)(((UInt16)value & (UInt16)(((Int16)1 << offset) - 1)) >> (offset - length));
        /// <summary>
        /// Extract a subset of bits. Works similarly to SubString(), except on bits and offset is from right side.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <returns>Extracted bits shifted down.</returns>
        /// <example>var value = 0b00000000_10011001;
        /// value.ExtractBits(5, 2) == 0b11</example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 Unpack(ref this UInt32 value, int offset, int length) => (UInt32)(((UInt32)value & (UInt32)(((Int32)1 << offset) - 1)) >> (offset - length));
        /// <summary>
        /// Extract a subset of bits. Works similarly to SubString(), except on bits and offset is from right side.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <returns>Extracted bits shifted down.</returns>
        /// <example>var value = 0b00000000_10011001;
        /// value.ExtractBits(5, 2) == 0b11</example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 Unpack(ref this Int32 value, int offset, int length) => (Int32)(((UInt32)value & (UInt32)(((Int32)1 << offset) - 1)) >> (offset - length));
        /// <summary>
        /// Extract a subset of bits. Works similarly to SubString(), except on bits and offset is from right side.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <returns>Extracted bits shifted down.</returns>
        /// <example>var value = 0b00000000_10011001;
        /// value.ExtractBits(5, 2) == 0b11</example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 Unpack(ref this UInt64 value, int offset, int length) => (UInt64)(((UInt64)value & (UInt64)(((Int64)1 << offset) - 1)) >> (offset - length));
        /// <summary>
        /// Extract a subset of bits. Works similarly to SubString(), except on bits and offset is from right side.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset">Offset from LSB (right).</param>
        /// <param name="length">Length of bits.</param>
        /// <returns>Extracted bits shifted down.</returns>
        /// <example>var value = 0b00000000_10011001;
        /// value.ExtractBits(5, 2) == 0b11</example>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 Unpack(ref this Int64 value, int offset, int length) => (Int64)(((UInt64)value & (UInt64)(((Int64)1 << offset) - 1)) >> (offset - length));
        #endregion
    }
}
