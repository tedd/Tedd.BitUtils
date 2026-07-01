## 2024-05-18 - Dependency Modernization

**Observation:** The `Tedd.BitUtils.Tests` project had outdated dependencies for xunit, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk, and coverlet.collector. The `Tedd.BitUtils` project targets frameworks net8.0, net9.0, and net10.0 natively.

**Strategic Action:** Updated the dependencies in `Tedd.BitUtils.Tests.csproj`. Validated that `Tedd.BitUtils.csproj` correctly handles modern frameworks along with preserved legacy targets. Run `dotnet restore`, `dotnet build`, `dotnet test`, `dotnet pack`, and `dotnet format`.
