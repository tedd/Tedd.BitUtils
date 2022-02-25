# Tedd.BitUtils
Available as NuGet Package: https://www.nuget.org/packages/Tedd.BitUtils/

Bit manipulation extension methods for:<br />sbyte, byte, short (Int16), ushort (UInt16), int (Int32), uint (UInt32), long (Int64) and ulong (UInt64).
Types can be manipulated in-place or by returning a modified copy. In-place modification uses reference for maximum performance.

Library offers standard bit manipulation methods that are fast. Wherever possible accellerated things such as specialized CPU instructions (intrinsics) or compiler magic is used to accellerate execution. If .Net version or CPU you are using doesn't support hardware accelleration for a particular function then software implementation is used.

## Extension methods
Methods are implemented as extension methods. This should give you a list in editor when you type dot after a supported type.
Note: Ror and Rol is only implemented for 32-bit and 64-bit integers.

### Get info
* bool a = i.IsBitSet(n);
* int a = i.PopCount();
* int a = i.LeadingZeroCount();
* string a = i.ToBinaryString();
* string a = i.ToBinaryStringPadded();

### In-place
Operates directly on variable, avoiding copy operation.

* i.SetBit(n, bool);
* i.SetBit0(n);
* i.SetBit1(n);
* i.Rol();
* i.Rol(n);
* i.Ror();
* i.Ror(n);
* i.ReverseBits();
* i.ReverseEndianness();
* i.Pack(5, 2, i2);

### Copy
Result is copied to return value, keeping original variable unchaned.

* i2 = i.SetBitCopy(n, bool);
* i2 = i.SetBit0Copy(n);
* i2 = i.SetBit1Copy(n);
* i2 = i.RolCopy();
* i2 = i.RolCopy(n);
* i2 = i.RorCopy();
* i2 = i.RorCopy(n);
* i2 = i.ReverseBitsCopy();
* i2 = i.ReverseEndiannessCopy();
* i2 = i.PackCopy(5, 2, i3);
* i2 = i.Unpack(5, 2);

## Simple example of usage
```cs
var a = 0;
a.SetBit(0, true);
// a == 1
a.SetBit(1, true);
// a == 3
a.SetBit0(0);
// a == 2
// a.IsBitSet(0) == false
var b = a.SetBitCopy(0);
// b == 3
a = 1;
a.Rol();
// a == 3
```

## Pack / Unpack
Pack and unpack allows you to copy portions of an integer into portions of another integer.
```cs
var i1 = 0b0000_1111_1100_0011;
var i2 = 0b0000_0000_0000_0010;
// We copy the last 3 bits of i2 into fift position from right in i1.
i1.Pack(5, 2, i2);
// Arrows shows where the two lowest bits of i2 will be inserted into i1: 0b0000_1111_110 -> 0_0 <- 011
// i1 is now: 0b0000_1111_1101_0011
var i3 = i1.Unpack(5, 2);
// i3 is now: 0b0000_0000_0000_0010
```

## Performance
Ror() and Rol() are faster versions of Ror(1) and Ror(1). If you only need to roll one, use these.<br/>
The same goes for SetBit1(n) and SetBit0(n) which are faster versions of SetBit(n, bool) where branching is not required. If you don't need to pass true/false as parameter use the faster versions.

All methods are tagged for inline compile.

Note that for SByte, Byte, Int16 and UInt16 the CPU uses 32-bit word size anyway, so there is no speed gained in smaller datatypes. If you decompile to MSIL it will in fact show more instructions due to casting between types, but if you further look at the JIT generated ASM (final code executed) the code is identical for all datatypes.

### Hardware intrinsics
For .Net Core 3.0 and above library uses hardware intrinsics for operations where available. This is possible because of new features supported in .Net Core 3.0 and above. For frameworks (.Net 4.x and .Net Standard) and processors (i.e. non-x86/x64 CPU) where this is not supported a software implementation is used. The software implementation is naturally slower than the hardware accellerated implementation.

| Command              | .Net 4.x | .Net Standard | .Net Core 2.x | .Net Core 3.0 | .Net Core 3.1 |
| -------------------- |  :---:   |     :---:     |     :---:     |     :---:     |     :---:     |
| SetBit               |    Y     |       Y       |       Y       |       Y       |       Y       |
| SetBit0              |    Y     |       Y       |       Y       |       Y       |       Y       |
| SetBit1              |    Y     |       Y       |       Y       |       Y       |       Y       |
| Rol                  | RyuJit   |    RyuJit     |       Y       |       Y       |       Y       |
| Ror                  | RyuJit   |    RyuJit     |       Y       |       Y       |       Y       |
| ReverseBits          |    N     |       N       |       N       |       Y       |       Y       |
| PopCount             |    N     |       N       |       N       |       Y       |       Y       |
| LeadingZeroCount     |    N     |       N       |       N       |       Y       |       Y       |
| ReverseEndianness    |    N     |       N       |       N       |       N       |       YC      |
| ToBinaryString       |   N/A    |      N/A      |      N/A      |      N/A      |      N/A      |
| ToBinaryStringPadded |   N/A    |      N/A      |      N/A      |      N/A      |      N/A      |

*C = One copy operation.

