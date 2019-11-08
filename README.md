# Tedd.BitUtils
Bit manipulation extension methods for Byte, short (Int16), ushort (UInt16), int (Int32), uint (UInt32), long (Int64) and ulong (UInt64).

Types can be manipulated in-place or by returning a modified copy. All methods are tagged for inline compile. Ror and Rol methods should be optimized by RyuJit to use CPU implementations.

Faster versions of Ror() and Rol() are hardcoded to shift 1.
Faster versions of SetBit0(n) and SetBit1(n) skips the branch required for SetBit(n, true/false).

Note that CPU most likely only supports bit manipulation on Int32 and Int64, so Byte and Int16 will be treated as Int32 internally. Result will be correct, but there is no speed benefit and possibly a small penalty for using smaller types than Int32.

Ror and Rol is only implemented for 32-bit and 64-bit integers.

## Quick examples
```cs
var a = 0;
a.SetBit(0, true);
// a == 1
a.SetBit(1, true);
// a == 3
a.SetBit0(0);
// a == 2
// a.IsBitSet(0) == false
var b = a.SetBit(0);
// b == 3
a = 1;
a.Rol();
// a == 3
```