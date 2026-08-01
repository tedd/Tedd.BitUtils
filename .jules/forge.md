## 2024-06-01 - Dependency and Framework Drift

**Observation:** `Tedd.BitUtils` supports `net4.6.2;netstandard2.1;netcoreapp3.1;net5.0;net6.0`. `netcoreapp3.1`, `net5.0`, and `net6.0` are EOL. `Tedd.BitUtils.Tests` targets `net6.0` and contains outdated packages (`coverlet.collector`, `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`). `Tedd.BitUtils.Benchmarks` targets `net6.0`. `Tedd.BitUtils.Net4Tests` uses `packages.config` and targets `net4.8`.

**Strategic Action:**
Modernized Target Frameworks to retain existing backward-compatible ones while adding modern counterparts to prevent downstream consumers from downgrading or failing due to EOL frameworks. `Tedd.BitUtils` and `Archive` target `net4.6.2;netstandard2.0;netstandard2.1;netcoreapp3.1;net5.0;net6.0;net8.0;net9.0;net10.0`. `Tests` targets `net8.0;net10.0` to work effectively on modern environments, and test dependencies were updated to `coverlet.collector` (6.0.2), `Microsoft.NET.Test.Sdk` (17.11.1), `xunit` (2.9.2), and `xunit.runner.visualstudio` (2.8.2). `Benchmarks` targets `net8.0`.
## 2024-10-24 - Testing Framework and Tooling Update

**Observation:** Test project `Tedd.BitUtils.Tests` dependencies `coverlet.collector`, `Microsoft.NET.Test.Sdk`, `xunit`, and `xunit.runner.visualstudio` have newer stable versions available.

**Strategic Action:**
Updated NuGet packages in `Tedd.BitUtils.Tests` to their latest stable versions (`coverlet.collector` to 10.0.1, `Microsoft.NET.Test.Sdk` to 18.8.1, `xunit` to 2.9.3, and `xunit.runner.visualstudio` to 3.1.5) to align with modern testing platforms (`net8.0`, `net10.0`). Validated project compilation and test execution on all targets.
