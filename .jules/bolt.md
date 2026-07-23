## 2024-05-21 - ToBitString Allocation Optimization

**Observation:** The `ToBitStringPadded` and `ToBitString` extension methods were historically implemented using `Convert.ToString(value, 2).PadLeft(...)`. This creates an intermediate string resulting in elevated Garbage Collection pressure and degraded CPU cycle efficiency (O(N) time and space complexity due to string copies). The `Convert.ToString` allocates one string, and `PadLeft` allocates another.

**Strategic Action:** Optimized allocations by implementing a targeted branch using `#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER` to utilize `string.Create`. This allocates exactly once. Older framework versions fallback to a `char[]` constructor. Length determination for unpadded strings uses `LeadingZeroCount`.

Empirical Benchmark Results (Int32 target):
| Method                      | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| Archive_ToBitStringPadded   | 70.42 ns | 0.958 ns | 0.800 ns |  1.00 |    0.02 | 0.0070 |     168 B |        1.00 |
| Optimized_ToBitStringPadded | 61.85 ns | 1.281 ns | 1.135 ns |  0.88 |    0.02 | 0.0037 |      88 B |        0.52 |

**Conclusion:** 48% reduction in memory allocations, translating to lowered GC pressure over the application lifecycle. Time complexity remains O(N), but physical overhead is structurally improved.

## 2024-07-23 - CreateBitString Loop Optimization

**Observation:** The `CreateBitString` loop in `Tedd.BitUtilsExtensions` processes bit sequences iteratively and uses a conditional branch to determine the character value for binary '1' or '0': `span[i] = (v & 1) == 1 ? '1' : '0';`. The BenchmarkDotNet diagnostics revealed a baseline execution latency of ~76 ns for `Optimized_ToBitString` and ~74.5 ns for `Optimized_ToBitStringPadded`. This branch behavior contributes heavily to cpu-pipeline stalling within the loop.

**Strategic Action:** Replaced the conditional branching with direct bitwise arithmetic logic: `span[i] = (char)('0' + (v & 1));` (and equivalently for the char[] array fallback). This adjustment eliminated the branch point, mapping bit states implicitly to their ASCII offsets, reducing execution time to ~53.8 ns for `Optimized_ToBitString` and ~58.4 ns for `Optimized_ToBitStringPadded`.
