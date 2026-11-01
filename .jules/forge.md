## 2026-11-01 - Dependency Modernization

**Observation:** The `Tedd.BitUtils.Tests` project had outdated dependencies. The xUnit framework shift (v2 to v3) breaks existing test runners, so `xunit.runner.visualstudio` should be kept at v2 (e.g. 2.9.3/2.8.2/2.x) to avoid breaking test execution, as noted in the memory: "When modernizing dependencies, avoid upgrading xUnit from v2 (e.g., 2.9.3) to v3, as the framework shift breaks existing test runners and execution behaviors without extensive rewriting." I've upgraded `coverlet.collector`, `coverlet.msbuild` and `Microsoft.NET.Test.Sdk` to their latest versions. All tests passed.

**Strategic Action:** Upgraded `coverlet.collector`, `coverlet.msbuild`, and `Microsoft.NET.Test.Sdk` to their latest versions in `Tedd.BitUtils.Tests.csproj`. Did not upgrade `xunit.runner.visualstudio` from 2.8.2 to 4.0.1 due to v3 breaking changes.
