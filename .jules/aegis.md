## 2024-05-18 - Tedd.BitUtilsExtensions.BitTricks Code Coverage Expansion
**Observation:** `BitUtilsExtensions.BitTricks.cs` exhibits low branch coverage (38.09%).
**Strategic Action:** Implement parameterized tests for untested boundary conditions.
**Execution:** Parameterized vectors utilizing `[Theory]` and `[InlineData]` covering maximum, minimum, 0, and typical bounds for primitives (SByte, Int16, Int32, Int64, and equivalents) were implemented for `BitTricksTest.cs`.
**Result:** Coverage delta was measured: Line rate for `BitUtilsExtensions.BitTricks.cs` increased to 1.0 (100%). Branch rate increased to 0.738 (73.8%). The remaining missing branches strictly correspond to the `Bmi1.IsSupported` / `Bmi2.IsSupported` conditional checks which require environment hardware to execute the secondary branch.
