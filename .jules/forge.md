## 2025-03-05 - Framework Modernization

**Observation:** `Tedd.BitUtils` supports `net8.0` and `net10.0` but misses explicit targeting for `net9.0`. The library has no NuGet dependencies to upgrade.

**Strategic Action:** Added `net9.0` to `<TargetFrameworks>` across `Tedd.BitUtils`, `Tedd.BitUtils.Tests`, `Tedd.BitUtils.Benchmarks`, and `Tedd.BitUtils.Archive` to improve platform alignment and multi-targeting capabilities.
