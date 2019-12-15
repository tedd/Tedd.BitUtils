using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tedd.BitUtils
{
    public static class EndinanessExtensions
    {
        #region In-Place
        /// <summary>
        /// This is a no-op and added only for consistency.
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseEndianness(ref this SByte value) { }

        /// <summary>
        /// This is a no-op and added only for consistency.
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseEndianness(ref this Byte value) { }

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseEndianness(ref this Int16 value) =>
            // Don't need to AND with 0xFF00 or 0x00FF since the final
            // cast back to ushort will clear out all bits above [ 15 .. 00 ].
            // This is normally implemented via "movzx eax, ax" on the return.
            // Alternatively, the compiler could elide the movzx instruction
            // entirely if it knows the caller is only going to access "ax"
            // instead of "eax" / "rax" when the function returns.
            value = (Int16)(((UInt16)value >> 8) + ((UInt16)value << 8));

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseEndianness(ref this UInt16 value) =>
            // Don't need to AND with 0xFF00 or 0x00FF since the final
            // cast back to ushort will clear out all bits above [ 15 .. 00 ].
            // This is normally implemented via "movzx eax, ax" on the return.
            // Alternatively, the compiler could elide the movzx instruction
            // entirely if it knows the caller is only going to access "ax"
            // instead of "eax" / "rax" when the function returns.
            value = (UInt16)((UInt16)(value >> 8) | (UInt16)(value << 8));

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseEndianness(ref this Int32 value)
        {
            // This takes advantage of the fact that the JIT can detect
            // ROL32 / ROR32 patterns and output the correct intrinsic.
            //
            // Input: value = [ ww xx yy zz ]
            //
            // First line generates : [ ww xx yy zz ]
            //                      & [ 00 FF 00 FF ]
            //                      = [ 00 xx 00 zz ]
            //             ROR32(8) = [ zz 00 xx 00 ]
            //
            // Second line generates: [ ww xx yy zz ]
            //                      & [ FF 00 FF 00 ]
            //                      = [ ww 00 yy 00 ]
            //             ROL32(8) = [ 00 yy 00 ww ]
            //
            //                (sum) = [ zz yy xx ww ]
            //
            // Testing shows that throughput increases if the AND
            // is performed before the ROL / ROR.

            var a = ((UInt32)value & 0x00FF00FFu);
            a.Ror(8);
            var b = ((UInt32)value & 0xFF00FF00u);
            b.Rol(8);
            value = (Int32)(a    // xx zz
                            + b); // ww yy
        }


        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseEndianness(ref this UInt32 value)
        {
            // This takes advantage of the fact that the JIT can detect
            // ROL32 / ROR32 patterns and output the correct intrinsic.
            //
            // Input: value = [ ww xx yy zz ]
            //
            // First line generates : [ ww xx yy zz ]
            //                      & [ 00 FF 00 FF ]
            //                      = [ 00 xx 00 zz ]
            //             ROR32(8) = [ zz 00 xx 00 ]
            //
            // Second line generates: [ ww xx yy zz ]
            //                      & [ FF 00 FF 00 ]
            //                      = [ ww 00 yy 00 ]
            //             ROL32(8) = [ 00 yy 00 ww ]
            //
            //                (sum) = [ zz yy xx ww ]
            //
            // Testing shows that throughput increases if the AND
            // is performed before the ROL / ROR.

            var a = (value & 0x00FF00FFu);
            a.Ror(8);
            var b = (value & 0xFF00FF00u);
            b.Rol(8);
            value = a // xx zz
                   + b; // ww yy
        }


        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseEndianness(ref this Int64 value)
        {
            // Operations on 32-bit values have higher throughput than
            // operations on 64-bit values, so decompose.
            var a = (UInt32)value;
            a.ReverseEndianness();
            var b = (UInt32)(value >> 32);
            b.ReverseEndianness();
            value = (Int64)(((UInt64)a << 32) + (UInt64)b);
        }

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseEndianness(ref this UInt64 value)
        {
            // Operations on 32-bit values have higher throughput than
            // operations on 64-bit values, so decompose.
            var a = (UInt32)value;
            a.ReverseEndianness();
            var b = (UInt32)(value >> 32);
            b.ReverseEndianness();
            value = ((UInt64)a << 32) + (UInt64)b;
        }
#endregion


#region Copy
        /// <summary>
        /// This is a no-op and added only for consistency.
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SByte ReverseEndiannessCopy(ref this SByte value) => value;

        /// <summary>
        /// This is a no-op and added only for consistency.
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Byte ReverseEndiannessCopy(ref this Byte value) => value;

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 ReverseEndiannessCopy(ref this Int16 value) =>
            // Don't need to AND with 0xFF00 or 0x00FF since the final
            // cast back to ushort will clear out all bits above [ 15 .. 00 ].
            // This is normally implemented via "movzx eax, ax" on the return.
            // Alternatively, the compiler could elide the movzx instruction
            // entirely if it knows the caller is only going to access "ax"
            // instead of "eax" / "rax" when the function returns.
            (Int16)(((UInt16)value >> 8) + ((UInt16)value << 8));

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 ReverseEndiannessCopy(ref this UInt16 value) =>
            // Don't need to AND with 0xFF00 or 0x00FF since the final
            // cast back to ushort will clear out all bits above [ 15 .. 00 ].
            // This is normally implemented via "movzx eax, ax" on the return.
            // Alternatively, the compiler could elide the movzx instruction
            // entirely if it knows the caller is only going to access "ax"
            // instead of "eax" / "rax" when the function returns.
            (UInt16)((value >> 8) + (value << 8));

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 ReverseEndiannessCopy(ref this Int32 value)
        {
            // This takes advantage of the fact that the JIT can detect
            // ROL32 / ROR32 patterns and output the correct intrinsic.
            //
            // Input: value = [ ww xx yy zz ]
            //
            // First line generates : [ ww xx yy zz ]
            //                      & [ 00 FF 00 FF ]
            //                      = [ 00 xx 00 zz ]
            //             ROR32(8) = [ zz 00 xx 00 ]
            //
            // Second line generates: [ ww xx yy zz ]
            //                      & [ FF 00 FF 00 ]
            //                      = [ ww 00 yy 00 ]
            //             ROL32(8) = [ 00 yy 00 ww ]
            //
            //                (sum) = [ zz yy xx ww ]
            //
            // Testing shows that throughput increases if the AND
            // is performed before the ROL / ROR.
            var a = ((UInt32)value & 0x00FF00FFu);
            a.Ror(8);
            var b = ((UInt32)value & 0xFF00FF00u);
            b.Rol(8);
            return (Int32)(a    // xx zz
                            + b); // ww yy
        }


        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 ReverseEndiannessCopy(ref this UInt32 value)
        {
            // This takes advantage of the fact that the JIT can detect
            // ROL32 / ROR32 patterns and output the correct intrinsic.
            //
            // Input: value = [ ww xx yy zz ]
            //
            // First line generates : [ ww xx yy zz ]
            //                      & [ 00 FF 00 FF ]
            //                      = [ 00 xx 00 zz ]
            //             ROR32(8) = [ zz 00 xx 00 ]
            //
            // Second line generates: [ ww xx yy zz ]
            //                      & [ FF 00 FF 00 ]
            //                      = [ ww 00 yy 00 ]
            //             ROL32(8) = [ 00 yy 00 ww ]
            //
            //                (sum) = [ zz yy xx ww ]
            //
            // Testing shows that throughput increases if the AND
            // is performed before the ROL / ROR.

            var a = (value & 0x00FF00FFu);
            a.Ror(8);
            var b = (value & 0xFF00FF00u);
            b.Rol(8);
            return a // xx zz
                   + b; // ww yy
        }


        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 ReverseEndiannessCopy(ref this Int64 value)
        {
            // Operations on 32-bit values have higher throughput than
            // operations on 64-bit values, so decompose.
            var a = (UInt32)value;
            a.ReverseEndianness();
            var b = (UInt32)(value >> 32);
            b.ReverseEndianness();
            return (Int64)(((UInt64)a << 32) + (UInt64)b);
        }

        /// <summary>
        /// Reverses a primitive value - performs an endianness swap
        /// </summary> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 ReverseEndiannessCopy(ref this UInt64 value)
        {
            // Operations on 32-bit values have higher throughput than
            // operations on 64-bit values, so decompose.
            var a = (UInt32)value;
            a.ReverseEndianness();
            var b = (UInt32)(value >> 32);
            b.ReverseEndianness();
            return ((UInt64)a << 32) + (UInt64)b;
        }

#endregion
    }
}
