# Tedd.BitUtils

[![NuGet](https://img.shields.io/nuget/v/Tedd.BitUtils.svg)](https://www.nuget.org/packages/Tedd.BitUtils/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Tedd.BitUtils.svg)](https://www.nuget.org/packages/Tedd.BitUtils/)
[![.NET](https://github.com/tedd/Tedd.BitUtils/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tedd/Tedd.BitUtils/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/github/license/tedd/Tedd.BitUtils)](LICENSE)

Fast bit manipulation extension methods for `sbyte`, `byte`, `short` (`Int16`), `ushort` (`UInt16`), `int` (`Int32`), `uint` (`UInt32`), `long` (`Int64`) and `ulong` (`UInt64`).

Every operation is available in two forms: **in-place** (modifies the variable via a `ref` extension method, avoiding a copy) and **copy** (returns a modified copy, leaving the original untouched, method name suffixed `Copy`).

On top of the per-integer operations the package also has a **bit packing** layer that works over `Span<byte>`: arbitrary width bit fields (`BitPacking`, `BitWriter`, `BitReader`) and the byte oriented variable length integer encodings built on them (`VarInt`, `EbmlVInt`, `SizePrefix`). See [Bit packing](#bit-packing) below.

Targets **.NET Standard 2.1, .NET 6, .NET 8 and .NET 10**. On .NET 6 and later every operation is backed by [`System.Numerics.BitOperations`](https://learn.microsoft.com/dotnet/api/system.numerics.bitoperations) and hardware intrinsics (POPCNT, LZCNT, TZCNT, BMI1, BMI2, BSWAP, ARM64 RBIT) where the CPU supports them, with an automatic runtime fallback where it doesn't - you never need to branch on this yourself. All methods are tagged for inline compilation.

The .NET Standard 2.1 target (.NET Core 3.x, Mono, Xamarin, Unity 2021+) has neither of those APIs available, so it compiles to portable software implementations instead - same results, same API, no configuration. Note that .NET Framework does **not** support .NET Standard 2.1; .NET Framework consumers should stay on the 1.x line. No package dependencies on any target.

## Extension methods
Methods are implemented as extension methods, so your editor will list them when you type `.` after a supported type. Bit positions are zero based, counted from the least significant bit.

### Get info
* `bool a = i.IsBitSet(n);`
* `int a = i.PopCount();` — number of set bits
* `int a = i.Parity();` — 1 if an odd number of bits are set, else 0
* `uint a = i.ZigZagEncode();` / `int a = u.ZigZagDecode();` — fold a signed value onto an unsigned one so small magnitudes of either sign stay small (0, -1, 1, -2 map to 0, 1, 2, 3)
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

## Bit packing
Everything above operates on a single integer. These types operate on a `Span<byte>`, for building and parsing binary formats. They validate their arguments and throw, and every operation also has a `Try...` form that reports failure instead. All of them are allocation free.

### Arbitrary width bit fields
`BitPacking` reads and writes fields of 0 to 64 bits at any bit offset, ignoring byte boundaries. Bits are laid out **most significant bit first** (offset 0 is the `0x80` bit of byte 0), which is the order used by EBML, MPEG, JPEG and most network protocols. Writes are read-modify-write, so the buffer needs no zeroing and neighbouring fields survive:
```cs
Span<byte> buffer = stackalloc byte[2];
BitPacking.WriteBits(buffer, 0, 3, 0b101);     // buffer[0] == 0b101_00000
BitPacking.WriteBits(buffer, 3, 5, 0b11011);   // buffer[0] == 0b101_11011
var field = BitPacking.ReadBits(buffer, 3, 5); // 0b11011
```
`BitWriter` and `BitReader` are `ref struct`s that do the same thing sequentially, tracking the bit position for you:
```cs
Span<byte> buffer = stackalloc byte[4];
var writer = new BitWriter(buffer);
writer.Write(0b101, 3);
writer.Write(0xABC, 12);
writer.AlignToByte();                  // pad to the next byte boundary
var packet = writer.WrittenSpan;       // just the bytes touched

var reader = new BitReader(packet);
var first = reader.Read(3);            // 0b101
var second = reader.Read(12);          // 0xABC
```

### Variable length integers
`VarInt` holds four byte oriented encodings where small values take fewer bytes. All split the value into 7 bit groups, least significant first, with a continuation bit in the high bit of every byte but the last; they differ only in how the sign is carried.

| Method pair | Format | Also known as |
| --- | --- | --- |
| `WriteUnsigned` / `ReadUnsigned` | ULEB128 | LEB128 (DWARF, WebAssembly), Protocol Buffers `uint32`/`uint64` |
| `WriteSigned` / `ReadSigned` | SLEB128, two's complement sign extended | signed LEB128 (DWARF, WebAssembly) |
| `WriteZigZag` / `ReadZigZag` | ZigZag folded, then ULEB128 | Protocol Buffers `sint32`/`sint64` |
| `WriteSignMagnitude` / `ReadSignMagnitude` | sign in bit 6 of the first byte | **non-standard**, the format Tedd.SpanUtils writes |

`WriteSigned` and `WriteZigZag` produce the same number of bytes for every value - folding costs exactly the one bit that sign extension would have - so choose between them by what the other side speaks, not by size. `WriteSignMagnitude` exists to keep data already written by Tedd.SpanUtils readable; prefer ZigZag for new formats, since the sign-magnitude form has no encoding for the most negative value of a width and works around it with a sentinel that depends on that width.

```cs
Span<byte> buffer = stackalloc byte[VarInt.MaxLength];
var length = VarInt.WriteZigZag(buffer, -300);
var value = VarInt.ReadZigZag(buffer, out var bytesRead);   // -300
var needed = VarInt.MeasureZigZag(-300);                    // bytes it will take, without writing
```
The `bits` argument on the read methods bounds what the caller will accept (8 for a byte, 16 for a short, ...), turning an out of range value into an `OverflowException` rather than a silently truncated result:
```cs
VarInt.ReadUnsigned(buffer, out _, bits: 16);   // throws OverflowException above 65535
```

### Other encodings
`EbmlVInt` is the EBML variable-length integer of RFC 8794, used by Matroska, WebM and MKV. The length lives in the position of the first set bit of the first byte and the payload is big endian, so a decoder knows the full length after one byte and encoded values sort in the same order as the numbers. An all-ones payload is the reserved "unknown size" marker (`IsUnknown`).
```cs
Span<byte> buffer = stackalloc byte[EbmlVInt.MaxLength];
EbmlVInt.Write(buffer, 127);            // 0x40 0x7F
var v = EbmlVInt.Read(buffer);
// v.Value == 127, v.Length == 2 (bytes on the wire), v.Size == 2 (bytes the shortest form needs)
```
`SizePrefix` is a prefix-varint length field: the top two bits of the first byte give the byte count (1 to 4) and the rest is big endian payload, holding up to 30 bits. It packs more into four bytes than LEB128 does (30 bits against 28) and the decoder learns the length from the first byte, at the cost of no range beyond 30 bits. Same idea as the QUIC varint of RFC 9000, but not the same encoding (QUIC selects 1/2/4/8 bytes); unrelated to Bitcoin's CompactSize.
```cs
Span<byte> buffer = stackalloc byte[SizePrefix.MaxLength];
var length = SizePrefix.Write(buffer, 16384);          // 0x80 0x40 0x00
var size = SizePrefix.Read(buffer, out var bytesRead); // 16384
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

### .NET Standard 2.1
`System.Numerics.BitOperations` arrived in .NET Core 3.0 and `System.Runtime.Intrinsics` is .NET Core 3.0+ too, so neither exists on .NET Standard 2.1. Every use of them sits behind a `#if NET6_0_OR_GREATER`, with the portable algorithm as the shared fallback; `BitOps` is the single place the whole library reaches for `BitOperations`, so there is one file to audit rather than ninety-odd call sites.

Those portable paths are not shipped untested. A .NET Standard 2.1 target cannot host a test project, and a normal test run on .NET 8/10 would only ever execute the intrinsic-backed branches, so the library can be compiled down the portable branches on a modern target instead:
```
cd src
dotnet test Tedd.BitUtils.Tests/Tedd.BitUtils.Tests.csproj -c Release -p:ForcePortable=true
```
CI runs the full suite both ways on every push, and publishing is gated on the portable run passing.

## Benchmarks
`src/Tedd.BitUtils.Benchmarks` uses [BenchmarkDotNet](https://benchmarkdotnet.org/) to compare this version against a frozen snapshot of the pre-2.0 implementation (`src/Tedd.BitUtils.Archive`), operation by operation. Run it with:
```
cd src/Tedd.BitUtils.Benchmarks
dotnet run -c Release
```
or target one comparison directly, e.g. `dotnet run -c Release --filter *ReverseBits*`.

## Changelog

### 2.1.0
* New target: **.NET Standard 2.1** (.NET Core 3.x, Mono, Xamarin, Unity 2021+), alongside net6.0/net8.0/net10.0. Everything that needs `System.Numerics.BitOperations` or `System.Runtime.Intrinsics` is behind a `#if`, so modern targets keep the intrinsic-backed paths unchanged and .NET Standard 2.1 gets portable implementations. Still no package dependencies. (.NET Framework does not support .NET Standard 2.1 and stays on the 1.x line.)
* New **bit packing** layer over `Span<byte>`: `BitPacking` (arbitrary width bit fields at any bit offset, MSB first), plus the `BitWriter`/`BitReader` `ref struct`s for sequential packing.
* New **variable length integer** encodings in `VarInt`: ULEB128, SLEB128, ZigZag (Protocol Buffers `sint`) and the non-standard sign-magnitude format Tedd.SpanUtils writes.
* New `EbmlVInt` (RFC 8794 VINT, as used by Matroska/WebM) and `SizePrefix` (prefix-varint length field, 30 bits in up to 4 bytes).
* New `ZigZagEncode`/`ZigZagDecode` extension methods for `sbyte`/`short`/`int`/`long` and their unsigned counterparts.

### 2.0.0
* **Breaking:** now targets .NET 6, .NET 8 and .NET 10 only. .NET Framework and .NET Standard consumers should stay on the 1.x line.
* All operations now go through `System.Numerics.BitOperations` and hardware intrinsics directly, rather than each method doing its own `IsSupported` check.
* New operations: `ToggleBit`, `Parity`, `TrailingZeroCount`, `Log2`, `BitLength`, `IsPowerOfTwo`, `RoundUpToPowerOf2`, `ExtractLowestSetBit`, `ResetLowestSetBit`, `GetMaskUpToLowestSetBit`, `ExtractHighestSetBit`, `ZeroHighBits`, `ZeroLowBits`, `ParallelBitExtract`, `ParallelBitDeposit`.
* `Rol`/`Ror` are now implemented for `sbyte`/`byte`/`short`/`ushort` too (previously 32/64-bit only), and every operation now has an `sbyte` overload.
* **Fixed:** `ReverseEndianness` never actually took its fast path in 1.x, due to a compile constant that was never defined anywhere in the project - it silently ran the manual software fallback on every call, on every target framework, even where `BinaryPrimitives.ReverseEndianness` (a single BSWAP) was available. It now always uses the intrinsic-backed path.
* **Fixed:** `Pack`/`Unpack` for `int`/`uint`/`long`/`ulong` computed their mask as `(1 << length) - 1` (or `(1 << offset) - 1` for `Unpack`); when `length`/`offset` equalled the full width of the type, that shift count wrapped to a no-op and silently produced a mask of 0 instead of all-ones.
* `ReverseBits` on x86/x64 now uses a branch-free SWAR bit-swap instead of four table lookups (measured ~1.5x faster for `uint`, ~3.5x for `ulong` - see `Tedd.BitUtils.Benchmarks`).

## License
GNU Lesser General Public License v3.0 (LGPL-3.0) - see [LICENSE](LICENSE).
