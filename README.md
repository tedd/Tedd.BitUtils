# Tedd.BitUtils
Available as NuGet Package: https://www.nuget.org/packages/Tedd.BitUtils/

Bit manipulation extension methods for byte, short (Int16), ushort (UInt16), int (Int32), uint (UInt32), long (Int64) and ulong (UInt64).

Types can be manipulated in-place or by returning a modified copy. All methods are tagged for inline compile. Ror and Rol methods should be optimized by RyuJit to use CPU implementations.

Note that CPU most likely only supports bit manipulation on Int32 and Int64, so Byte and Int16 will be treated as Int32 internally. Result will be correct, but there is no speed benefit and possibly a small penalty for using smaller types than Int32.

## Extensions

i can be byte, short (Int16), ushort (UInt16), int (Int32), uint (UInt32), long (Int64) and ulong (UInt64).

Faster versions of Ror() and Rol() are hardcoded to shift 1. Faster versions of SetBit0(n) and SetBit1(n) skips the branch required for SetBit(n, true/false).

Note: Ror and Rol is only implemented for 32-bit and 64-bit integers.

### In-place
* i.SetBit(n, bool);
* i.SetBit0(n);
* i.SetBit1(n);
* i.Rol();
* i.Rol(n);
* i.Ror();
* i.Ror(n);
* i.ReverseBits();

### Copy
* bool = i.IsBitSet(n);
* i.SetBitCopy(n, bool);
* i.SetBit0Copy(n);
* i.SetBit1Copy(n);
* i.RolCopy();
* i.RolCopy(n);
* i.RorCopy();
* i.RorCopy(n);
* i.ReverseBits();

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