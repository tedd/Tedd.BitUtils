## 2024-06-01 - Dependency and Framework Drift

**Observation:** `Tedd.BitUtils` supports `net4.6.2;netstandard2.1;netcoreapp3.1;net5.0;net6.0`. `netcoreapp3.1`, `net5.0`, and `net6.0` are EOL. `Tedd.BitUtils.Tests` targets `net6.0` and contains outdated packages (`coverlet.collector`, `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`). `Tedd.BitUtils.Benchmarks` targets `net6.0`. `Tedd.BitUtils.Net4Tests` uses `packages.config` and targets `net4.8`.

**Strategic Action:**
Modernized Target Frameworks to retain existing backward-compatible ones while adding modern counterparts to prevent downstream consumers from downgrading or failing due to EOL frameworks. `Tedd.BitUtils` and `Archive` target `net4.6.2;netstandard2.0;netstandard2.1;netcoreapp3.1;net5.0;net6.0;net8.0;net9.0;net10.0`. `Tests` targets `net8.0;net10.0` to work effectively on modern environments, and test dependencies were updated to `coverlet.collector` (6.0.2), `Microsoft.NET.Test.Sdk` (17.11.1), `xunit` (2.9.2), and `xunit.runner.visualstudio` (2.8.2). `Benchmarks` targets `net8.0`.
## 2024-05-18 - Dependency Modernization Protocol - Tedd.BitUtils

**Observation:** The primary project `Tedd.BitUtils` has no dependencies or drift to update. The test project `Tedd.BitUtils.Tests` had minor outdated packages (`xunit` packages and `Microsoft.NET.Test.Sdk`). These were updated. The `Tedd.BitUtils.Net4Tests` project appears to be a legacy testing project that is using `packages.config`, making it incompatible with modern `dotnet` commands. No critical drift was found that requires complex compatibility partitioning or multi-targeting beyond what exists.

**Strategic Action:** Updated the testing packages in `Tedd.BitUtils.Tests`. Verified build and tests.
