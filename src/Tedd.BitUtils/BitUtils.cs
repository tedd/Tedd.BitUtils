using System;
using System.Runtime.CompilerServices;

namespace Tedd
{
    public static class BitUtilsExtensions
    {
        // Note. RyuJIT will compile this to a single rotate CPU instruction (as of about .NET 4.6.1 and dotnet core 2.0).


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this Int32 value, int count) => value = (value << count) | (value >> (32 - count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this UInt32 value, int count) => value = (value << count) | (value >> (32 - count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this Int64 value, int count) => value = (value << count) | (value >> (64 - count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this UInt64 value, int count) => value = (value << count) | (value >> (64 - count));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this Int32 value, int count) => value = (value << (32 - count)) | (value >> count);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this UInt32 value, int count) => value = (value << (32 - count)) | (value >> count);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this Int64 value, int count) => value = (value << (64 - count)) | (value >> count);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this UInt64 value, int count) => value = (value << (64 - count)) | (value >> count);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this Int32 value) => value = (value << 1) | (value >> (32 - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this UInt32 value) => value = (value << 1) | (value >> (32 - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this Int64 value) => value = (value << 1) | (value >> (64 - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rol(ref this UInt64 value) => value = (value << 1) | (value >> (64 - 1));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this Int32 value) => value = (value << (32 - 1)) | (value >> 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this UInt32 value) => value = (value << (32 - 1)) | (value >> 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this Int64 value) => value = (value << (64 - 1)) | (value >> 1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Ror(ref this UInt64 value) => value = (value << (64 - 1)) | (value >> 1);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this Byte value, int pos) => (value & (1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this Int16 value, int pos) => (value & (1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this UInt16 value, int pos) => (value & (1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this Int32 value, int pos) => (value & (1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this UInt32 value, int pos) => (value & (1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this Int64 value, int pos) => (value & ((Int64)1 << pos)) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBitSet(ref this UInt64 value, int pos) => (value & ((UInt64)1 << pos)) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this Byte value, int pos, bool state) => value = (state ? (Byte)((int)value | (int)(1 << pos)) : (Byte)((int)value & ~(int)(1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this Int16 value, int pos, bool state) => value = (state ? (Int16)((Int16)value | (Int16)(1 << pos)) : (Int16)((Int16)value & ~(Int16)(1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this UInt16 value, int pos, bool state) => value = (state ? (UInt16)((UInt16)value | (UInt16)(1 << pos)) : (UInt16)((UInt16)value & ~(UInt16)(1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this Int32 value, int pos, bool state) => value = (state ? (Int32)((Int32)value | ((Int32)1 << pos)) : (Int32)((Int32)value & ~(Int32)(1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this UInt32 value, int pos, bool state) => value = (state ? (UInt32)((UInt32)value | ((UInt32)1 << pos)) : (UInt32)((UInt32)value & ~(UInt32)(1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this Int64 value, int pos, bool state) => value = (state ? (Int64)((Int64)value | ((Int64)1 << pos)) : (Int64)((Int64)value & ~(Int64)(1 << pos)));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit(ref this UInt64 value, int pos, bool state) => value = (state ? (UInt64)((UInt64)value | ((UInt64)1 << pos)) : (UInt64)((UInt64)value & ~(UInt64)(1 << pos)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this Byte value, int pos) => value = (Byte)((int)value | (int)(1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this Int16 value, int pos) => value = (Int16)((Int16)value | (Int16)(1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this UInt16 value, int pos) => value = (UInt16)((UInt16)value | (UInt16)(1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this Int32 value, int pos) => value = (Int32)((Int32)value | ((Int32)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this UInt32 value, int pos) => value = (UInt32)((UInt32)value | ((UInt32)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this Int64 value, int pos) => value = (Int64)((Int64)value | ((Int64)1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit1(ref this UInt64 value, int pos) => value = (UInt64)((UInt64)value | ((UInt64)1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this Byte value, int pos) => value = (Byte)((int)value & ~(int)(1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this Int16 value, int pos) => value = (Int16)((Int16)value & ~(Int16)(1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this UInt16 value, int pos) => value = (UInt16)((UInt16)value & ~(UInt16)(1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this Int32 value, int pos) => value = (Int32)((Int32)value & ~(Int32)(1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this UInt32 value, int pos) => value = (UInt32)((UInt32)value & ~(UInt32)(1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this Int64 value, int pos) => value = (Int64)((Int64)value & ~(Int64)(1 << pos));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetBit0(ref this UInt64 value, int pos) => value = (UInt64)((UInt64)value & ~(UInt64)(1 << pos));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Pack(ref this uint packed, int offset, int length, uint value)
        {
            // Ensure value only contains length bits
            value &= uint.MaxValue >> 32 - length;

            if (offset + length > 32)
                throw new Exception($"Bit out of bounds"); // + ": Offset " + offset + " + length " + length + " > 32
            var clearMask = (uint)~((uint.MaxValue >> (32 - length)) << offset);

            packed &= clearMask;
            packed |= (uint)(value << offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Unpack(ref this uint packed, int offset, int length)
        {
            var v = packed >> offset;
            v = v & (((uint)1 << length) - 1);
            return v;
        }
    }
}
