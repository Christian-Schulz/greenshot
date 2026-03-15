# Greenshot .NET 10.0 Upgrade Tasks

## Overview

This document tracks the execution of the Greenshot screenshot tool upgrade from .NET Framework 4.8/4.8.1 to .NET 10.0. All 12 projects will be upgraded simultaneously in a single atomic operation.

**Progress**: 1/3 tasks complete (33%) ![0%](https://progress-bar.xyz/33)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-03-15 12:45)*
**References**: Plan §Phase 0, Plan §Build Requirements & Environment

- [✓] (1) Verify .NET 10.0 SDK installed on system
- [✓] (2) .NET 10.0 SDK available (Verify)
- [✓] (3) Check global.json compatibility with .NET 10.0 (if file exists at repository root or src/)
- [✓] (4) global.json compatible or updated (Verify)
- [✓] (5) Verify MSBuild 17.0+ available (from Visual Studio 2022 or MSBuild Tools)
- [✓] (6) MSBuild version meets requirements (Verify)

---

### [▶] TASK-002: Atomic upgrade of all 12 projects to .NET 10.0
**References**: Plan §Phase 1-4, Plan §Project-by-Project Migration Plans, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [✓] (1) Update TargetFramework to net10.0-windows in all 12 project files per Plan §Project-by-Project Migration Plans (Greenshot.Base, Greenshot.BuildTasks, Greenshot.Editor, Greenshot.Plugin.Box, Greenshot.Plugin.Confluence, Greenshot.Plugin.Dropbox, Greenshot.Plugin.ExternalCommand, Greenshot.Plugin.Imgur, Greenshot.Plugin.Jira, Greenshot.Plugin.Office, Greenshot, Greenshot.Test)
- [✓] (2) All 12 project files updated to net10.0-windows (Verify)
- [✓] (3) Update package references per Plan §Package Update Reference: System.Text.Json 10.0.5 in Greenshot.Editor; add System.Runtime.Serialization.Formatters to Greenshot.Editor and Greenshot.Test; update or remove Dapplo.Jira.SvgWinForms in Greenshot.Plugin.Jira; remove MicrosoftOfficeCore and Unofficial.Microsoft.mshtml from Greenshot.Plugin.Office
- [✓] (4) All package references updated (Verify)
- [✓] (5) Add `#pragma warning disable SYSLIB0011` around BinaryFormatter usage in Greenshot.Editor\FileFormat\V1\Legacy\LegacyFileHelper.cs:42 and Greenshot.Test\Editor\FileFormat\V1\Legacy\LegacySerializationBinderTests.cs:46 with TODO comments indicating future migration to System.Text.Json
- [✓] (6) BinaryFormatter warnings suppressed with documentation (Verify)
- [✓] (7) Restore NuGet packages: `msbuild src/Greenshot.sln /p:Configuration=Release /restore /t:PrepareForBuild`
- [✓] (8) All dependencies restored successfully (Verify)
- [✓] (9) Build solution to identify compilation errors: `msbuild src/Greenshot.sln /p:Configuration=Release /t:Rebuild /v:normal`
- [✓] (10) Fix all compilation errors found per Plan §Breaking Changes Catalog (focus areas: §1 Windows Forms 13,272 issues, §2 System.Drawing/GDI+ 4,954 issues, §4 WPF 337 issues, §5 COM Interop 4 issues, §6 Code Access Security 2 issues, §7 Legacy Cryptography 2 issues, §8 Windows ACL 1 issue, §9 Legacy Controls 7 issues)
- [▶] (11) Rebuild solution to verify fixes: `msbuild src/Greenshot.sln /p:Configuration=Release /t:Rebuild /v:normal`
- [ ] (12) All 12 projects build with 0 errors (Verify)
- [ ] (13) Commit changes with message: "TASK-002: Atomic upgrade of all 12 projects to .NET 10.0"

---

### [ ] TASK-003: Run full test suite and validate upgrade
**References**: Plan §Phase 5, Plan §Testing & Validation Strategy

- [ ] (1) Run tests in Greenshot.Test project
- [ ] (2) Fix any test failures (reference Plan §Breaking Changes Catalog §3 for BinaryFormatter, Plan §Phase 5 for test-specific issues)
- [ ] (3) Re-run tests after fixes
- [ ] (4) All tests pass with 0 failures (Verify)
- [ ] (5) Commit test fixes with message: "TASK-003: Complete .NET 10.0 upgrade testing and validation"

---










