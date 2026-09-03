# Tedd.BitUtils
Available as NuGet Package: https://www.nuget.org/packages/Tedd.BitUtils/

Fast bit manipulation extension methods for `sbyte`, `byte`, `short` (`Int16`), `ushort` (`UInt16`), `int` (`Int32`), `uint` (`UInt32`), `long` (`Int64`) and `ulong` (`UInt64`).

Every operation is available in two forms: **in-place** (modifies the variable via a `ref` extension method, avoiding a copy) and **copy** (returns a modified copy, leaving the original untouched, method name suffixed `Copy`).

Targets **.NET 6, .NET 8 and .NET 10**. Every operation is backed by [`System.Numerics.BitOperations`](https://learn.microsoft.com/dotnet/api/system.numerics.bitoperations) and hardware intrinsics (POPCNT, LZCNT, TZCNT, BMI1, BMI2, BSWAP, ARM64 RBIT) where the CPU supports them, with an automatic runtime fallback where it doesn't - you never need to branch on this yourself. All methods are tagged for inline compilation.

## Extension methods
Methods are implemented as extension methods, so your editor will list them when you type `.` after a supported type. Bit positions are zero based, counted from the least significant bit.

### Get info
* `bool a = i.IsBitSet(n);`
* `int a = i.PopCount();` — number of set bits
* `int a = i.Parity();` — 1 if an odd number of bits are set, else 0
* `int a = i.LeadingZeroCount();`
* `int a = i.TrailingZeroCount();`
* `int a = i.Log2();` — floor(log2(i)), i.e. the position of the highest set bit
* `int a = i.BitLength();` — number of bits needed to represent i, i.e. `Log2() + 1`
* `bool a = i.IsPowerOfTwo();`
* `string a = i.ToBitString();`
* `string a = i.ToBitStringPadded();`

### In-place
Operates directly on the variable, avoiding a copy.

* `i.SetBit(n, bool);`
* `i.SetBit0(n);` / `i.SetBit1(n);` — faster than `SetBit` when the state is a compile-time constant
* `i.ToggleBit(n);`
* `i.Rol();` / `i.Rol(n);` — rotate left one position, or `n` positions
* `i.Ror();` / `i.Ror(n);`
* `i.ReverseBits();`
* `i.ReverseEndianness();`
* `i.RoundUpToPowerOf2();`
* `i.Pack(offset, length, i2);`
* `i.ExtractLowestSetBit();` — isolate the lowest set bit (`i & -i`)
* `i.ResetLowestSetBit();` — clear the lowest set bit (`i & (i - 1)`)
* `i.GetMaskUpToLowestSetBit();` — mask covering every bit up to and including the lowest set bit
* `i.ExtractHighestSetBit();`
* `i.ZeroHighBits(index);` — keep only the low `index` bits
* `i.ZeroLowBits(index);` — clear the low `index` bits
* `i.ParallelBitExtract(mask);` — gather the bits selected by `mask` into consecutive low bits (PEXT)
* `i.ParallelBitDeposit(mask);` — scatter consecutive low bits into the positions selected by `mask` (PDEP)

### Copy
Result is returned as a new value; the original variable is unchanged. Every in-place method above has a `...Copy` counterpart, e.g.:

* `i2 = i.SetBitCopy(n, bool);`
* `i2 = i.RolCopy(n);`
* `i2 = i.ReverseBitsCopy();`
* `i2 = i.RoundUpToPowerOf2Copy();`
* `i2 = i.PackCopy(offset, length, i3);`
* `i2 = i.Unpack(offset, length);` — the only naturally non-mutating "get" operation, so it has no in-place form

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
var b = a.SetBitCopy(0, true);
// b == 3
a = 1;
a.Rol();
// a == 2
```

## Pack / Unpack
Pack and Unpack copy a range of bits between two integers, similar to `Substring` but for bits. `offset` counts from the LSB (right) to the bit past the end of the field; `length` is the field's width.
```cs
var i1 = 0b0000_1111_1100_0011;
var i2 = 0b0000_0000_0000_0010;
// Copies the 2 lowest bits of i2 into bit positions 3 and 4 of i1 (offset 5, length 2: field is [offset-length, offset-1]).
i1.Pack(5, 2, i2);
// i1 is now: 0b0000_1111_1101_0011
var i3 = i1.Unpack(5, 2);
// i3 is now: 0b0000_0000_0000_0010
```

## Performance
`Rol()`/`Ror()` (no count) are faster than `Rol(1)`/`Ror(1)`, since no count needs to be masked to the type's bit width. Likewise `SetBit0(n)`/`SetBit1(n)` are faster than `SetBit(n, bool)` when the state is known at the call site, since no branch is needed.

Note that for `sbyte`, `byte`, `short` and `ushort` the CPU operates at 32-bit word size regardless, so there's no speed to be gained from the smaller datatypes themselves - the JIT-generated assembly for these operations ends up identical across the narrower integer types.

### Hardware intrinsics
| Operation                              | Backing intrinsic (when supported)     |
| --------------------------------------- | --------------------------------------- |
| `Rol`, `Ror`                            | `BitOperations.RotateLeft`/`RotateRight` (ROL/ROR) |
| `PopCount`, `Parity`                    | `BitOperations.PopCount` (POPCNT)       |
| `LeadingZeroCount`, `BitLength`         | `BitOperations.LeadingZeroCount` (LZCNT) |
| `TrailingZeroCount`                     | `BitOperations.TrailingZeroCount` (TZCNT) |
| `Log2`                                  | `BitOperations.Log2`                    |
| `IsPowerOfTwo`, `RoundUpToPowerOf2`     | `BitOperations.IsPow2`/`RoundUpToPowerOf2` |
| `ReverseEndianness`                     | `BinaryPrimitives.ReverseEndianness` (BSWAP) |
| `ReverseBits`                           | ARM64 `RBIT`; branch-free SWAR bit-swap on x86/x64 |
| `ExtractLowestSetBit`/`ResetLowestSetBit`/`GetMaskUpToLowestSetBit` | BMI1 (`BLSI`/`BLSR`/`BLSMSK`) |
| `ZeroHighBits`                          | BMI2 (`BZHI`)                           |
| `ParallelBitExtract`/`ParallelBitDeposit` | BMI2 (`PEXT`/`PDEP`); portable bit-by-bit fallback elsewhere |

Every entry above falls back automatically to a portable software implementation on CPUs or platforms without the matching instruction (e.g. ARM without RBIT for `ReverseBits`, or x86 without BMI2 for `ParallelBitExtract`/`ParallelBitDeposit`) - there's no configuration or feature flag involved.

## Benchmarks
`src/Tedd.BitUtils.Benchmarks` uses [BenchmarkDotNet](https://benchmarkdotnet.org/) to compare this version against a frozen snapshot of the pre-2.0 implementation (`src/Tedd.BitUtils.Archive`), operation by operation. Run it with:
```
cd src/Tedd.BitUtils.Benchmarks
dotnet run -c Release
```
or target one comparison directly, e.g. `dotnet run -c Release --filter *ReverseBits*`.

## Changelog

### 2.0.0
* **Breaking:** now targets .NET 6, .NET 8 and .NET 10 only. .NET Framework and .NET Standard consumers should stay on the 1.x line.
* All operations now go through `System.Numerics.BitOperations` and hardware intrinsics directly, rather than each method doing its own `IsSupported` check.
* New operations: `ToggleBit`, `Parity`, `TrailingZeroCount`, `Log2`, `BitLength`, `IsPowerOfTwo`, `RoundUpToPowerOf2`, `ExtractLowestSetBit`, `ResetLowestSetBit`, `GetMaskUpToLowestSetBit`, `ExtractHighestSetBit`, `ZeroHighBits`, `ZeroLowBits`, `ParallelBitExtract`, `ParallelBitDeposit`.
* `Rol`/`Ror` are now implemented for `sbyte`/`byte`/`short`/`ushort` too (previously 32/64-bit only), and every operation now has an `sbyte` overload.
* **Fixed:** `ReverseEndianness` never actually took its fast path in 1.x, due to a compile constant that was never defined anywhere in the project - it silently ran the manual software fallback on every call, on every target framework, even where `BinaryPrimitives.ReverseEndianness` (a single BSWAP) was available. It now always uses the intrinsic-backed path.
* **Fixed:** `Pack`/`Unpack` for `int`/`uint`/`long`/`ulong` computed their mask as `(1 << length) - 1` (or `(1 << offset) - 1` for `Unpack`); when `length`/`offset` equalled the full width of the type, that shift count wrapped to a no-op and silently produced a mask of 0 instead of all-ones.
* `ReverseBits` on x86/x64 now uses a branch-free SWAR bit-swap instead of four table lookups.

## License
MIT - see [LICENSE](LICENSE).
