## 2024-05-21 - ToBitString Allocation Optimization

**Observation:** The `ToBitStringPadded` and `ToBitString` extension methods were historically implemented using `Convert.ToString(value, 2).PadLeft(...)`. This creates an intermediate string resulting in elevated Garbage Collection pressure and degraded CPU cycle efficiency (O(N) time and space complexity due to string copies). The `Convert.ToString` allocates one string, and `PadLeft` allocates another.

**Strategic Action:** Optimized allocations by implementing a targeted branch using `#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER` to utilize `string.Create`. This allocates exactly once. Older framework versions fallback to a `char[]` constructor. Length determination for unpadded strings uses `LeadingZeroCount`.

Empirical Benchmark Results (Int32 target):
| Method                      | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|---------------------------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| Archive_ToBitStringPadded   | 70.42 ns | 0.958 ns | 0.800 ns |  1.00 |    0.02 | 0.0070 |     168 B |        1.00 |
| Optimized_ToBitStringPadded | 61.85 ns | 1.281 ns | 1.135 ns |  0.88 |    0.02 | 0.0037 |      88 B |        0.52 |

**Conclusion:** 48% reduction in memory allocations, translating to lowered GC pressure over the application lifecycle. Time complexity remains O(N), but physical overhead is structurally improved.
