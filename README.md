# Tedd.BitUtils
Available as NuGet Package: https://www.nuget.org/packages/Tedd.BitUtils/

Bit manipulation extension methods for:<br />byte, short (Int16), ushort (UInt16), int (Int32), uint (UInt32), long (Int64) and ulong (UInt64).

Types can be manipulated in-place or by returning a modified copy.

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
* i.SetBit(n, bool);
* i.SetBit0(n);
* i.SetBit1(n);
* i.Rol();
* i.Rol(n);
* i.Ror();
* i.Ror(n);
* i.ReverseBits();
* i.ReverseEndianness();

### Copy
* i2 = i.SetBitCopy(n, bool);
* i2 = i.SetBit0Copy(n);
* i2 = i.SetBit1Copy(n);
* i2 = i.RolCopy();
* i2 = i.RolCopy(n);
* i2 = i.RorCopy();
* i2 = i.RorCopy(n);
* i2 = i.ReverseBitsCopy();
* i2 = i.ReverseEndiannessCopy();

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
## Performance
Ror() and Rol() are faster versions of Ror(1) and Ror(1). If you only need to roll one, use these.<br/>
The same goes for SetBit1(n) and SetBit0(n) which are faster versions of SetBit(n, bool) where branching is left out. If you don't need to pass true/false as parameter use the faster versions.

All methods are tagged for inline compile.

### Hardware intrinsics
For .Net Core 3.0 and above library uses hardware intrinsics for operations where available. This is possible because of new features supported in .Net Core 3.0 and aboce. For frameworks (.Net 4.x and .Net Standard) and processors (i.e. non-x86/x64 CPU) where this is not supported a slower software implementation is used.

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
| ReverseEndianness    |    N     |       N       |       N       |       N       |       Y       |
| ToBinaryString       |   N/A    |      N/A      |      N/A      |      N/A      |      N/A      |
| ToBinaryStringPadded |   N/A    |      N/A      |      N/A      |      N/A      |      N/A      |

Note that for Byte, Int16 and UInt16 the CPU uses 32-bit word size anyway so there is no speed gained in smaller datatypes. Decompile to MSIL shows extra assembly instructions handling conversions.
