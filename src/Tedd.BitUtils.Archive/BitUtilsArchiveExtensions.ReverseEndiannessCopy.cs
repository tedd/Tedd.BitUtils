using System;
using System.Runtime.CompilerServices;

namespace Tedd.BitUtils.Archive
{
    public static partial class BitUtilsArchiveExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 ReverseEndiannessCopy(ref UInt32 value)
        {
            var v = value;
            ReverseEndianness(ref v);
            return v;
        }
    }
}
