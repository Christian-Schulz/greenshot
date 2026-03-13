# .NET 10 Upgrade Plan for Greenshot

## Table of Contents
1. [Executive Summary](#executive-summary)
2. [Migration Strategy](#migration-strategy)
3. [Detailed Dependency Analysis](#detailed-dependency-analysis)
4. [Project-by-Project Migration Plans](#project-by-project-migration-plans)
5. [Package Update Reference](#package-update-reference)
6. [Breaking Changes Catalog](#breaking-changes-catalog)
7. [Risk Management](#risk-management)
8. [Testing & Validation Strategy](#testing--validation-strategy)
9. [Complexity & Effort Assessment](#complexity--effort-assessment)
10. [Source Control Strategy](#source-control-strategy)
11. [Success Criteria](#success-criteria)

---

## Executive Summary

### Overview
This plan outlines the upgrade of the **Greenshot** screenshot tool from .NET Framework 4.8/4.8.1 to **.NET 10.0** (Long Term Support). Greenshot is a Windows desktop application built with Windows Forms and WPF, featuring a plugin architecture with 8 plugins for various cloud services and Office integrations.

### Scope
- **Projects**: 12 projects (1 main app, 1 editor library, 1 base library, 1 build tasks, 8 plugins, 1 test project)
- **Total Issues**: 19,055 issues (13,866 mandatory, 5,189 potential)
- **Affected Files**: 321 files across all projects
- **Current Framework**: .NET Framework 4.8 (net480) and 4.8.1 (net481)
- **Target Framework**: .NET 10.0 with Windows-specific APIs (net10.0-windows)

### Key Challenges

#### 1. Windows Forms Migration (13,272 issues)
The largest impact area. Greenshot heavily uses Windows Forms for its UI, which requires:
- Migration from System.Windows.Forms (.NET Framework) to modern Windows Forms
- Addressing designer-related changes
- Updating legacy controls and event handling patterns

#### 2. GDI+ / System.Drawing Migration (4,954 issues)
As a screenshot tool, Greenshot extensively uses System.Drawing APIs:
- Many System.Drawing APIs are Windows-specific and require net10.0-windows TFM
- Image manipulation, graphics rendering, and bitmap operations need review
- Already using SixLabors.ImageSharp (modern alternative) but System.Drawing still prevalent

#### 3. BinaryFormatter Obsolescence (2 occurrences, user-flagged)
- **Location 1**: `Greenshot.Editor\FileFormat\V1\Legacy\LegacyFileHelper.cs:42`
- **Location 2**: `Greenshot.Test\Editor\FileFormat\V1\Legacy\LegacySerializationBinderTests.cs:46`
- **User Request**: Use NuGet package as temporary solution for later replacement
- **Recommended Package**: `System.Runtime.Serialization.Formatters` (compatibility shim) or migrate to System.Text.Json

#### 4. WPF Changes (337 issues)
While primarily Windows Forms, some WPF usage exists requiring updates for .NET 10 compatibility.

#### 5. Incompatible NuGet Packages (3 packages)
- **Dapplo.Jira.SvgWinForms** (1.0.41 → needs 1.1.47 or later)
- **MicrosoftOfficeCore** (15.0.0) - incompatible, needs removal or replacement
- **Unofficial.Microsoft.mshtml** (7.0.3300) - incompatible, needs alternative

#### 6. COM Interop for Office Integration (4 issues)
The Office plugin uses COM interop with Microsoft Office applications (Word, Excel, PowerPoint, OneNote, Outlook) which has behavioral changes in .NET 10.

### Migration Approach: **All-at-Once Strategy**

Given the tight interdependencies between projects and the plugin architecture, we'll use a coordinated upgrade approach:

1. **Phase 1: Foundation** - Update base libraries first (Greenshot.Base, Greenshot.BuildTasks)
2. **Phase 2: Editor & Dependencies** - Update Greenshot.Editor (depends on Base)
3. **Phase 3: Plugins** - Update all 8 plugins in parallel (all depend only on Base)
4. **Phase 4: Main Application** - Update Greenshot.csproj (depends on Base, Editor, BuildTasks)
5. **Phase 5: Testing** - Update test project and validate

### Timeline & Effort Estimate
- **Complexity**: High (19,055+ issues, heavy Windows Forms/GDI+ usage)
- **Estimated Effort**: 40-60 hours of development time
  - Phase 1: 8-12 hours (Base library migration)
  - Phase 2: 12-18 hours (Editor migration with GDI+ changes)
  - Phase 3: 8-12 hours (Plugin migrations, mostly parallel)
  - Phase 4: 8-12 hours (Main app migration)
  - Phase 5: 4-6 hours (Testing and validation)

### Risk Level: **Medium-High**
- **High**: Extensive Windows Forms and GDI+ usage
- **Medium**: Plugin architecture may have runtime loading issues
- **Medium**: COM Interop changes for Office integration
- **Low**: Most NuGet packages are compatible

### Success Criteria
1. ✅ All 12 projects compile successfully targeting net10.0-windows
2. ✅ All plugins load and function correctly
3. ✅ Screenshot capture, editing, and export functionality works
4. ✅ Office integration (Word, Excel, PowerPoint, Outlook) functions
5. ✅ Cloud service integrations (Box, Dropbox, Imgur, Jira, Confluence) work
6. ✅ Application starts, captures screenshots, and saves them
7. ✅ No BinaryFormatter usage in production code (test usage acceptable with warning)
8. ✅ All existing automated tests pass (if any)

### Next Steps
1. Review this plan and confirm approach
2. Create backup branch (already on `draft/next_dotnet_playground`)
3. Execute Phase 1: Foundation migration
4. Validate and iterate through remaining phases

## Migration Strategy

### Chosen Approach: All-at-Once Coordinated Update

#### Rationale
The Greenshot solution has a clear dependency hierarchy with a plugin architecture. All plugins depend on Greenshot.Base, and the main application depends on both Base and Editor. This structure makes an all-at-once approach optimal because:

1. **Tight Coupling**: Projects share common types and APIs from Greenshot.Base
2. **Plugin Architecture**: All plugins must be compatible with the same base library version
3. **Build System**: The solution uses MSBuild with shared Directory.Build.props and Directory.Build.targets
4. **No Mixed-Mode**: Can't have some projects on .NET Framework and others on .NET 10

#### Migration Sequence (Dependency-Ordered)

```
Level 0: Foundation Projects (no dependencies)
├── Greenshot.Base.csproj          [3,217 issues] 
└── Greenshot.BuildTasks.csproj    [2 issues]

Level 1: Dependent Libraries & Plugins
├── Greenshot.Editor.csproj        [8,467 issues] → depends on Base
├── Greenshot.Plugin.Box           [157 issues]   → depends on Base
├── Greenshot.Plugin.Confluence    [375 issues]   → depends on Base
├── Greenshot.Plugin.Dropbox       [141 issues]   → depends on Base
├── Greenshot.Plugin.ExternalCommand [508 issues] → depends on Base
├── Greenshot.Plugin.Imgur         [648 issues]   → depends on Base
├── Greenshot.Plugin.Jira          [774 issues]   → depends on Base
└── Greenshot.Plugin.Office        [40 issues]    → depends on Base

Level 2: Top-Level Applications
├── Greenshot.csproj               [4,277 issues] → depends on Base, Editor, BuildTasks
└── Greenshot.Test.csproj          [449 issues]   → depends on Editor
```

### Phase Breakdown

#### Phase 1: Foundation Migration (Level 0)
**Projects**: Greenshot.Base, Greenshot.BuildTasks

**Key Actions**:
1. Validate .NET 10 SDK installation
2. Validate global.json compatibility
3. Convert Greenshot.Base to net10.0-windows
4. Convert Greenshot.BuildTasks to net10.0-windows
5. Address System.Drawing and Windows Forms issues in Base
6. Address BinaryFormatter usage (1 occurrence in Base dependencies)
7. Resolve binary/source incompatibilities
8. Build and validate Base library

**Estimated Time**: 8-12 hours

#### Phase 2: Editor & UI Components (Level 1, Part 1)
**Projects**: Greenshot.Editor

**Key Actions**:
1. Convert Greenshot.Editor to net10.0-windows
2. Update System.Text.Json package (10.0.2 → 10.0.5)
3. Address BinaryFormatter in LegacyFileHelper.cs
4. Migrate GDI+ usage (2,235 issues in Editor)
5. Update Windows Forms controls and patterns (6,083 issues)
6. Address WPF incompatibilities (72 issues)
7. Build and validate Editor library

**Estimated Time**: 12-18 hours

#### Phase 3: Plugin Migration (Level 1, Part 2)
**Projects**: All 8 plugins (can be done in parallel)

**Strategy**: Migrate all plugins simultaneously since they have no inter-dependencies

**Common Actions per Plugin**:
1. Convert project to net10.0-windows
2. Update NuGet packages where needed
3. Address API incompatibilities
4. Validate plugin manifest and loading
5. Build and validate each plugin

**Plugin-Specific Considerations**:
- **Greenshot.Plugin.Jira**: Update Dapplo.Jira.SvgWinForms package, incompatible package issue
- **Greenshot.Plugin.Office**: Remove MicrosoftOfficeCore package (incompatible), address COM interop changes
- **Greenshot.Plugin.Confluence**: Address HTTP client and serialization changes

**Estimated Time**: 8-12 hours (parallel work)

#### Phase 4: Main Application Migration (Level 2, Part 1)
**Projects**: Greenshot (main application)

**Key Actions**:
1. Convert Greenshot.csproj to net10.0-windows
2. Address Windows Forms issues (3,392 issues)
3. Address GDI+ usage (798 issues)
4. Update COM interop for Office integration
5. Update Code Access Security (CAS) usage
6. Address Windows ACL usage
7. Update application startup and plugin loading
8. Build and validate main application
9. Test end-to-end functionality

**Estimated Time**: 8-12 hours

#### Phase 5: Testing & Validation (Level 2, Part 2)
**Projects**: Greenshot.Test

**Key Actions**:
1. Convert Greenshot.Test.csproj to net10.0-windows
2. Update test framework (xunit) if needed
3. Address BinaryFormatter in tests (LegacySerializationBinderTests.cs)
4. Run all automated tests
5. Perform manual testing of core features
6. Validate plugin loading and functionality
7. Performance testing and comparison

**Estimated Time**: 4-6 hours

### Build System Considerations

#### Critical Build Requirements
- **MUST use MSBuild**, not `dotnet build` (CodeTaskFactory usage in Directory.Build.targets)
- Full Git history required for Nerdbank.GitVersioning
- Environment variables for OAuth credentials (Box, Dropbox, Flickr, Imgur, Photobucket, Picasa)

#### Build Commands
```powershell
# Restore packages
msbuild src/Greenshot.sln /p:Configuration=Release /restore /t:PrepareForBuild

# Build solution
msbuild src/Greenshot.sln /p:Configuration=Release /t:Rebuild /v:normal
```

### Rollback Strategy
- **Branch**: Working directly on `draft/next_dotnet_playground` (dedicated upgrade branch)
- **Rollback**: Use git reset to previous commit if needed
- **Incremental Commits**: Commit after each phase for granular rollback points

### Dependencies on External Factors
- .NET 10 SDK must be installed
- MSBuild 17.0+ (from Visual Studio 2022 or MSBuild Tools)
- Windows OS (required for building Windows-specific projects)
- Full Git clone with history (for versioning)

## Detailed Dependency Analysis

### Project Dependency Hierarchy

```
Level 0: Foundation (No Dependencies)
├─ Greenshot.Base.csproj
│  ├─ Issues: 3,217 (1,782 mandatory, 1,435 potential)
│  ├─ Affected Files: 181 files
│  ├─ Main Technologies: Windows Forms, GDI+, WPF
│  └─ Used By: All other projects
│
└─ Greenshot.BuildTasks.csproj
   ├─ Issues: 2 (1 mandatory, 1 potential)
   ├─ Affected Files: Minimal
   └─ Used By: Greenshot (main app)

Level 1: Libraries & Plugins (Depend on Level 0)
├─ Greenshot.Editor.csproj
│  ├─ Issues: 8,467 (6,204 mandatory, 2,263 potential)
│  ├─ Affected Files: 172 files
│  ├─ Dependencies: Greenshot.Base
│  ├─ Main Technologies: Windows Forms (6,083), GDI+ (2,235), WPF (72)
│  ├─ Package Updates: System.Text.Json 10.0.2 → 10.0.5
│  ├─ ⚠️ BinaryFormatter Usage: LegacyFileHelper.cs:42
│  └─ Used By: Greenshot, Greenshot.Test
│
├─ Greenshot.Plugin.Box.csproj
│  ├─ Issues: 157 (154 mandatory, 3 potential)
│  ├─ Dependencies: Greenshot.Base
│  └─ Technologies: HTTP client, OAuth
│
├─ Greenshot.Plugin.Confluence.csproj
│  ├─ Issues: 375 (350 mandatory, 25 potential)
│  ├─ Dependencies: Greenshot.Base, Dapplo.Confluence
│  └─ Technologies: HTTP client, REST API
│
├─ Greenshot.Plugin.Dropbox.csproj
│  ├─ Issues: 141 (138 mandatory, 3 potential)
│  ├─ Dependencies: Greenshot.Base
│  └─ Technologies: HTTP client, OAuth
│
├─ Greenshot.Plugin.ExternalCommand.csproj
│  ├─ Issues: 508 (497 mandatory, 11 potential)
│  ├─ Dependencies: Greenshot.Base
│  └─ Technologies: Process launching, Windows Forms
│
├─ Greenshot.Plugin.Imgur.csproj
│  ├─ Issues: 648 (622 mandatory, 26 potential)
│  ├─ Dependencies: Greenshot.Base
│  └─ Technologies: HTTP client, OAuth
│
├─ Greenshot.Plugin.Jira.csproj
│  ├─ Issues: 774 (709 mandatory, 65 potential)
│  ├─ Dependencies: Greenshot.Base, Dapplo.Jira
│  ├─ ⚠️ Incompatible Package: Dapplo.Jira.SvgWinForms (2.0.7 → needs upgrade)
│  └─ Technologies: HTTP client, REST API
│
└─ Greenshot.Plugin.Office.csproj
   ├─ Issues: 40 (11 mandatory, 29 potential)
   ├─ Dependencies: Greenshot.Base, Office Interop assemblies
   ├─ ⚠️ Incompatible Packages:
   │  - MicrosoftOfficeCore 15.0.0 (REMOVE)
   │  - Unofficial.Microsoft.mshtml 7.0.3300 (REMOVE/REPLACE)
   └─ Technologies: COM Interop (Word, Excel, PowerPoint, Outlook, OneNote)

Level 2: Top-Level Applications
├─ Greenshot.csproj (Main Application)
│  ├─ Issues: 4,277 (3,397 mandatory, 880 potential)
│  ├─ Affected Files: 72 files
│  ├─ Dependencies: Greenshot.Base, Greenshot.Editor, Greenshot.BuildTasks
│  ├─ Technologies: Windows Forms (3,392), GDI+ (798), COM Interop (4), CAS (2)
│  └─ Used By: End users
│
└─ Greenshot.Test.csproj
   ├─ Issues: 449 (1 mandatory, 448 potential)
   ├─ Dependencies: Greenshot.Editor
   ├─ ⚠️ BinaryFormatter Usage: LegacySerializationBinderTests.cs:46
   └─ Technologies: xunit testing framework
```

### Critical Path Analysis

The critical path for this upgrade is:
1. **Greenshot.Base** → Must be done first (affects all other projects)
2. **Greenshot.Editor** → Second priority (affects main app and tests)
3. **Main App & Plugins** → Can proceed once Base & Editor are stable
4. **Testing** → Final validation

**Bottleneck**: Greenshot.Base is a blocking dependency for all projects. Any issues in Base will cascade to all other projects.

### Target Framework Mapping

All projects will be upgraded as follows:
- **Current**: `net480` (11 projects) or `net481` (1 project - Test)
- **Target**: `net10.0-windows` (all 12 projects)

**Rationale for net10.0-windows**:
- Windows Forms requires Windows-specific APIs
- WPF requires Windows-specific APIs
- System.Drawing (GDI+) requires Windows-specific APIs
- COM Interop for Office requires Windows platform

### SDK-Style Project Status
✅ **Good News**: All projects are already SDK-style format!
- No need for project file conversion
- Can directly update TargetFramework property
- Modern NuGet package management already in place

### Build System Migration

#### Current Build Process
```powershell
# Restore
msbuild src/Greenshot.sln /p:Configuration=Release /restore /t:PrepareForBuild

# Build
msbuild src/Greenshot.sln /p:Configuration=Release /t:Rebuild /v:normal
```

#### Post-Upgrade Build Process
**Same commands**, but with .NET 10 SDK present:
- MSBuild will automatically use .NET 10 SDK for building
- Same CI/CD workflow in `.github/workflows/release.yml`
- ⚠️ Update CI to ensure .NET 10 SDK is installed (currently uses .NET 7.x)

### Shared Configuration Files

#### Files That Will Be Updated
1. **src/Directory.Build.props**
   - Shared MSBuild properties
   - May need LangVersion updates
   - Already has WindowsTargetPlatformVersion settings

2. **src/Directory.Build.targets**
   - Token replacement for OAuth credentials
   - Should work as-is with .NET 10

3. **src/.editorconfig**
   - Code style settings
   - No changes needed

4. **src/version.json**
   - Nerdbank.GitVersioning config
   - No changes needed (version-agnostic)

#### Files That May Need Review
- **.github/workflows/release.yml** - Update .NET SDK version from 7.x to 10.x
- **installer/** configuration - Verify Inno Setup works with .NET 10 output

## Project-by-Project Migration Plans

---

### Phase 1: Foundation Projects

---

#### 1.1 Greenshot.Base.csproj

**Current Framework**: net480  
**Target Framework**: net10.0-windows  
**Issues**: 3,217 (1,782 mandatory, 1,435 potential)  
**Files Affected**: 181 files  

**Issue Breakdown**:
- Windows Forms: 1,671 issues
- GDI+ / System.Drawing: 1,380 issues
- BinaryFormatter/Remoting: 40 issues
- WPF: 35 issues
- Legacy Cryptography: 2 issues
- Legacy Controls: 2 issues

**Migration Steps**:
1. ✅ **Pre-validation**
   - Verify .NET 10 SDK installed
   - Check global.json compatibility

2. 🔧 **Update Project File**
   - Change `<TargetFramework>net480</TargetFramework>` to `<TargetFramework>net10.0-windows</TargetFramework>`

3. 🔧 **Address Code Issues**
   - Fix Windows Forms binary incompatibilities
   - Update System.Drawing usage patterns
   - Replace legacy cryptography APIs (SHA1 → SHA256, etc.)
   - Update BinaryFormatter dependencies (if any indirect usage)
   - Fix WPF resource handling

4. 🔨 **Build & Validate**
   - Run: `msbuild Greenshot.Base.csproj /t:Rebuild /p:Configuration=Release`
   - Address compilation errors iteratively
   - Verify no warnings related to obsolete APIs

**Key Files to Review** (high-issue files):
- Platform/Windows specific helpers
- Drawing and imaging utilities
- Windows Forms controls and helpers
- Configuration and serialization

**Estimated Effort**: 6-8 hours

---

#### 1.2 Greenshot.BuildTasks.csproj

**Current Framework**: net480  
**Target Framework**: net10.0-windows  
**Issues**: 2 (1 mandatory, 1 potential)  

**Issue Breakdown**:
- Project target framework: 1 issue
- Behavioral changes: 1 issue

**Migration Steps**:
1. 🔧 **Update Project File**
   - Change TargetFramework to net10.0-windows

2. 🔧 **Review MSBuild Task Code**
   - Verify custom MSBuild tasks work with .NET 10
   - Check Microsoft.Build.Utilities.Core compatibility

3. 🔨 **Build & Validate**
   - Build project
   - Test custom build tasks

**Estimated Effort**: 1-2 hours

---

### Phase 2: Editor Component

---

#### 2.1 Greenshot.Editor.csproj

**Current Framework**: net480  
**Target Framework**: net10.0-windows  
**Issues**: 8,467 (6,204 mandatory, 2,263 potential)  
**Files Affected**: 172 files  
**Dependencies**: Greenshot.Base

**Issue Breakdown**:
- Windows Forms: 6,083 issues (HIGHEST)
- GDI+ / System.Drawing: 2,235 issues (HIGH)
- WPF: 72 issues
- BinaryFormatter: 1 issue (LegacyFileHelper.cs)
- Legacy Controls: 4 issues

**Migration Steps**:
1. 🔧 **Update Project File**
   - Change TargetFramework to net10.0-windows
   - Update System.Text.Json: 10.0.2 → 10.0.5

2. 📦 **Add BinaryFormatter Support Package**
   - Add package: `System.Runtime.Serialization.Formatters`
   - Target file: `FileFormat\V1\Legacy\LegacyFileHelper.cs`

3. 🔧 **Address BinaryFormatter Usage**
   - Add `#pragma warning disable SYSLIB0011` around BinaryFormatter code
   - Add TODO comment: "Replace BinaryFormatter with System.Text.Json in future release"

4. 🔧 **Fix Windows Forms Issues**
   - Update Drawing/ToolStripProfessionalRenderer
   - Fix ImageEditor form controls
   - Update all editor forms (text, brightness, resize, etc.)
   - Review surface drawing operations

5. 🔧 **Fix System.Drawing Issues**
   - Update Image/Bitmap operations
   - Review Graphics rendering
   - Update ImageFormat usage (PNG, JPG, BMP, etc.)
   - Check PixelFormat operations

6. 🔧 **Fix WPF Issues**
   - Regenerate XAML designer code if present
   - Update WPF resource handling

7. 🔨 **Build & Validate**
   - Build project
   - Test image editor functionality manually
   - Verify drawing tools work (shapes, text, blur, etc.)

**High-Impact Files**:
- `FileFormat\V1\Legacy\LegacyFileHelper.cs` - BinaryFormatter usage
- `Destinations\EditorDestination.cs` - Image handling
- `FileFormatHandlers\GreenshotFileFormatHandler.cs` - Image properties
- Drawing surface implementations
- All Form files in Forms\

**Estimated Effort**: 12-18 hours

---

### Phase 3: Plugin Migrations

All plugins can be migrated in parallel since they only depend on Greenshot.Base.

---

#### 3.1 Greenshot.Plugin.Box.csproj

**Issues**: 157 (154 mandatory, 3 potential)  
**Dependencies**: Greenshot.Base, HTTP client libraries  

**Steps**:
1. Update TargetFramework to net10.0-windows
2. Fix API incompatibilities (mainly Windows Forms for settings UI)
3. Test OAuth authentication flow
4. Test file upload to Box

**Estimated Effort**: 1 hour

---

#### 3.2 Greenshot.Plugin.Confluence.csproj

**Issues**: 375 (350 mandatory, 25 potential)  
**Dependencies**: Greenshot.Base, Dapplo.Confluence  

**Steps**:
1. Update TargetFramework to net10.0-windows
2. Fix API incompatibilities (Windows Forms, HTTP client)
3. Review Dapplo.Confluence package compatibility
4. Test page upload to Confluence

**Estimated Effort**: 1-2 hours

---

#### 3.3 Greenshot.Plugin.Dropbox.csproj

**Issues**: 141 (138 mandatory, 3 potential)  
**Dependencies**: Greenshot.Base  

**Steps**:
1. Update TargetFramework to net10.0-windows
2. Fix API incompatibilities
3. Test OAuth authentication
4. Test file upload to Dropbox

**Estimated Effort**: 1 hour

---

#### 3.4 Greenshot.Plugin.ExternalCommand.csproj

**Issues**: 508 (497 mandatory, 11 potential)  
**Dependencies**: Greenshot.Base  

**Steps**:
1. Update TargetFramework to net10.0-windows
2. Fix Windows Forms incompatibilities (configuration UI)
3. Test process launching and command execution
4. Verify environment variable passing

**Estimated Effort**: 1-2 hours

---

#### 3.5 Greenshot.Plugin.Imgur.csproj

**Issues**: 648 (622 mandatory, 26 potential)  
**Dependencies**: Greenshot.Base  

**Steps**:
1. Update TargetFramework to net10.0-windows
2. Fix API incompatibilities (Windows Forms, HTTP)
3. Test OAuth authentication
4. Test image upload to Imgur

**Estimated Effort**: 1-2 hours

---

#### 3.6 Greenshot.Plugin.Jira.csproj ⚠️

**Issues**: 774 (709 mandatory, 65 potential)  
**Dependencies**: Greenshot.Base, Dapplo.Jira  
**⚠️ Package Issue**: Dapplo.Jira.SvgWinForms incompatible

**Steps**:
1. Update TargetFramework to net10.0-windows
2. **Fix Package Issue**:
   - Update or remove Dapplo.Jira.SvgWinForms package
   - Investigate version conflict (assessment suggests 1.1.47 but current is 2.0.7)
   - May need to update parent Dapplo.Jira package too
3. Fix API incompatibilities
4. Test Jira issue creation and attachment
5. Test SVG icon rendering (if SvgWinForms kept)

**Estimated Effort**: 2-3 hours

---

#### 3.7 Greenshot.Plugin.Office.csproj ⚠️⚠️

**Issues**: 40 (11 mandatory, 29 potential)  
**Dependencies**: Greenshot.Base, Office Interop assemblies  
**⚠️ Critical Package Issues**: 2 incompatible packages

**Steps**:
1. Update TargetFramework to net10.0-windows
2. **Remove Incompatible Packages**:
   - Remove `MicrosoftOfficeCore` (15.0.0) - functionality available via standard Interop
   - Remove `Unofficial.Microsoft.mshtml` (7.0.3300) or replace with HtmlAgilityPack
3. **Update COM Interop Code**:
   - Review Word destination
   - Review Excel destination
   - Review PowerPoint destination
   - Review Outlook email destination
   - Review OneNote destination
4. Test with Office applications installed
5. Test without Office (graceful degradation)

**High-Risk Areas**:
- COM object lifetime management
- Office application automation
- HTML parsing (mshtml replacement)

**Estimated Effort**: 3-4 hours

---

### Phase 4: Main Application

---

#### 4.1 Greenshot.csproj (Main Application)

**Current Framework**: net480  
**Target Framework**: net10.0-windows  
**Issues**: 4,277 (3,397 mandatory, 880 potential)  
**Files Affected**: 72 files  
**Dependencies**: Greenshot.Base, Greenshot.Editor, Greenshot.BuildTasks

**Issue Breakdown**:
- Windows Forms: 3,392 issues (HIGHEST)
- GDI+ / System.Drawing: 798 issues
- COM Interop: 4 issues
- Code Access Security: 2 issues
- Windows ACL: 1 issue
- Legacy Controls: 1 issue

**Migration Steps**:
1. 🔧 **Update Project File**
   - Change TargetFramework to net10.0-windows

2. 🔧 **Fix Main Application Issues**:
   - **Windows Forms**: Update MainForm, settings dialogs, tray icon
   - **GDI+**: Update screenshot capture, image processing
   - **COM Interop**: Review Office integration hooks
   - **CAS**: Remove Code Access Security checks (likely plugin trust)
   - **Windows ACL**: Update file permission code

3. 🔧 **Update Application Startup**:
   - Review GreenshotMain.cs (entry point)
   - Update plugin loading mechanism
   - Verify dependency injection if used

4. 🔧 **Update Configuration**:
   - Settings serialization
   - Configuration storage

5. 🔨 **Build & Validate**:
   - Build main application
   - Test startup
   - Test screenshot capture (screen, window, region)
   - Test editor integration
   - Test all plugins load correctly
   - Test tray icon and context menu
   - Test hotkeys

6. 🔧 **Update Build Outputs**:
   - Verify installer creation (Tools.InnoSetup)
   - Test portable package creation

**Critical Files**:
- `GreenshotMain.cs` - Application entry point
- `Forms\MainForm.cs` - Main window and tray icon
- `Helpers\CaptureHelper.cs` - Screenshot capture
- `Controls\*` - Custom controls
- `Destinations\*` - Export handlers
- `Configuration\*` - Settings management

**Estimated Effort**: 8-12 hours

---

### Phase 5: Testing & Validation

---

#### 5.1 Greenshot.Test.csproj

**Current Framework**: net481  
**Target Framework**: net10.0-windows  
**Issues**: 449 (1 mandatory, 448 potential)  
**Dependencies**: Greenshot.Editor

**Issue Breakdown**:
- Source incompatibilities: 448 issues
- BinaryFormatter test: 1 occurrence
- Project target framework: 1 issue

**Migration Steps**:
1. 🔧 **Update Project File**
   - Change TargetFramework to net10.0-windows

2. 📦 **Add BinaryFormatter Package**
   - Add `System.Runtime.Serialization.Formatters` package

3. 🔧 **Fix BinaryFormatter Test**
   - Update `LegacySerializationBinderTests.cs:46`
   - Add `#pragma warning disable SYSLIB0011`
   - Add comment: "Test for legacy format support - using compat package"

4. 🔧 **Fix Test Incompatibilities**
   - Update test assertions if needed
   - Fix any xunit compatibility issues

5. 🧪 **Run Tests**
   - Build test project
   - Run all tests via Test Explorer or command line
   - Document any failing tests
   - Fix test failures related to .NET 10 changes

**Estimated Effort**: 2-3 hours

---

#### 5.2 Manual Testing Checklist

After all projects build successfully, perform comprehensive manual testing:

**Core Functionality**:
- [ ] Application starts without errors
- [ ] Tray icon appears and context menu works
- [ ] Hotkeys work (PrintScreen, Alt+PrintScreen, etc.)

**Screenshot Capture**:
- [ ] Full screen capture
- [ ] Window capture
- [ ] Region selection capture
- [ ] Last region capture
- [ ] IE capture (if applicable)

**Editor Functionality**:
- [ ] Image editor opens with captured screenshot
- [ ] Drawing tools work (rectangle, ellipse, line, arrow, text)
- [ ] Highlighting and obfuscation tools work
- [ ] Resize, crop, rotate operations
- [ ] Undo/redo functionality
- [ ] Save and export

**Destinations/Export**:
- [ ] Save to file (PNG, JPG, BMP, TIFF)
- [ ] Copy to clipboard
- [ ] Print
- [ ] Email (if configured)
- [ ] External command execution

**Plugin Testing**:
- [ ] All plugins load without errors
- [ ] Box plugin: Upload to Box
- [ ] Confluence plugin: Upload to Confluence page
- [ ] Dropbox plugin: Upload to Dropbox
- [ ] External Command plugin: Execute commands
- [ ] Imgur plugin: Upload to Imgur
- [ ] Jira plugin: Attach to Jira issue
- [ ] Office plugin: Send to Word, Excel, PowerPoint, Outlook, OneNote

**Configuration**:
- [ ] Settings dialog opens
- [ ] Settings persist correctly
- [ ] Plugin configuration works
- [ ] Hotkey configuration
- [ ] Language selection

**Edge Cases**:
- [ ] Multi-monitor setup
- [ ] High DPI displays
- [ ] Different Windows themes
- [ ] Office not installed (Office plugin graceful degradation)

**Performance**:
- [ ] Startup time comparable to .NET Framework version
- [ ] Screenshot capture performance
- [ ] Memory usage monitoring

**Estimated Effort**: 3-4 hours

---

### Phase Completion Criteria

#### Phase 1 Complete When:
- ✅ Greenshot.Base builds without errors
- ✅ Greenshot.BuildTasks builds without errors
- ✅ No critical warnings in build output
- ✅ Base library loads in test harness

#### Phase 2 Complete When:
- ✅ Greenshot.Editor builds without errors
- ✅ BinaryFormatter package integrated
- ✅ Editor can be instantiated programmatically
- ✅ Basic drawing operations work

#### Phase 3 Complete When:
- ✅ All 8 plugins build without errors
- ✅ Package issues resolved (Jira, Office)
- ✅ Plugin manifests valid
- ✅ Plugins can be loaded by main app

#### Phase 4 Complete When:
- ✅ Greenshot.csproj (main app) builds without errors
- ✅ Application starts successfully
- ✅ All plugins load correctly
- ✅ Screenshot capture works
- ✅ Editor integration works

#### Phase 5 Complete When:
- ✅ Test project builds and runs
- ✅ All automated tests pass (or documented failures)
- ✅ Manual testing checklist 100% complete
- ✅ No regressions found

## Package Update Reference

### Packages Requiring Updates

#### 1. System.Text.Json
- **Current Version**: 10.0.2 (in Greenshot.Editor)
- **Recommended Version**: 10.0.5
- **Affected Projects**: Greenshot.Editor
- **Severity**: Potential (recommended update)
- **Action**: Update package reference in Greenshot.Editor.csproj
- **Rationale**: Version alignment with .NET 10.0

#### 2. Dapplo.Jira.SvgWinForms
- **Current Version**: 2.0.7
- **Recommended Version**: 1.1.47+ (or investigate why downgrade suggested)
- **Affected Projects**: Greenshot.Plugin.Jira
- **Severity**: Mandatory (incompatible)
- **Action**: Update or remove package, test Jira plugin functionality
- **Rationale**: Current version incompatible with .NET 10

### Packages Requiring Removal

#### 1. MicrosoftOfficeCore
- **Current Version**: 15.0.0
- **Affected Projects**: Greenshot.Plugin.Office
- **Severity**: Mandatory (incompatible)
- **Action**: Remove package reference, use Microsoft.Office.Interop.* assemblies directly
- **Rationale**: Legacy package incompatible with .NET 10, functionality available through standard Interop assemblies

#### 2. Unofficial.Microsoft.mshtml
- **Current Version**: 7.0.3300
- **Affected Projects**: Greenshot.Plugin.Office
- **Severity**: Mandatory (incompatible)
- **Action**: Remove or replace with modern HTML parsing (HtmlAgilityPack already referenced)
- **Investigation Needed**: Determine if mshtml usage can be replaced with HtmlAgilityPack
- **Rationale**: Unofficial package incompatible with .NET 10

### Compatible Packages (No Action Required)

All other packages are compatible with .NET 10:
- ✅ Dapplo.Confluence (1.0.41)
- ✅ Dapplo.HttpExtensions.JsonNet (2.0.11)
- ✅ Dapplo.HttpExtensions.WinForms (2.0.11)
- ✅ Dapplo.Jira (2.0.7)
- ✅ Dapplo.Windows.* (2.0.63) - Clipboard, Dpi, Gdi32, Icons, Kernel32, Multimedia
- ✅ HtmlAgilityPack (1.12.4)
- ✅ log4net (3.3.0)
- ✅ Microsoft.Build.Utilities.Core (18.3.3)
- ✅ Microsoft.IO.RecyclableMemoryStream (3.0.1)
- ✅ Microsoft.Office.Interop.* (15.0.x) - Excel, Outlook, PowerPoint, Word, OneNote
- ✅ Microsoft.SourceLink.GitHub (10.0.103)
- ✅ Microsoft.Toolkit.Uwp.Notifications (7.1.3)
- ✅ MSBuildTasks (1.5.0.235)
- ✅ Nerdbank.GitVersioning (3.9.50)
- ✅ SixLabors.ImageSharp (2.1.13)
- ✅ SixLabors.ImageSharp.Drawing (1.0.0)
- ✅ Svg (3.4.7)
- ✅ System.CommandLine (3.0.0-preview.1.26104.118)
- ✅ System.Reactive.Linq (6.1.0)
- ✅ System.Text.Json (10.0.5) - already at recommended version in some projects
- ✅ Tools.InnoSetup (6.7.1)
- ✅ xunit (2.4.2)
- ✅ xunit.runner.visualstudio (2.4.5)

### BinaryFormatter Mitigation Package

#### Option 1: System.Runtime.Serialization.Formatters (Compatibility Shim)
- **Package**: `System.Runtime.Serialization.Formatters`
- **Purpose**: Provides BinaryFormatter for .NET 10+ (with security warnings)
- **Pros**: Minimal code changes, maintains backward compatibility
- **Cons**: Still uses deprecated API, security warnings remain
- **Use Case**: Temporary solution as requested by user

#### Option 2: Modern Alternatives (Future Migration)
Consider these for future replacement:
- **System.Text.Json** - Already used in project, best for JSON serialization
- **protobuf-net** - For binary serialization with better performance
- **MessagePack** - Fast binary serialization

#### Affected Files
1. `Greenshot.Editor\FileFormat\V1\Legacy\LegacyFileHelper.cs:42` - Production code
2. `Greenshot.Test\Editor\FileFormat\V1\Legacy\LegacySerializationBinderTests.cs:46` - Test code

#### Recommendation
- Add `System.Runtime.Serialization.Formatters` package to Greenshot.Editor and Greenshot.Test
- Add `#pragma warning disable SYSLIB0011` to suppress BinaryFormatter obsolete warnings
- Add TODO comments for future migration to modern serialization

## Breaking Changes Catalog

### 1. Windows Forms API Changes (13,272 issues)

#### Category: WinForms
**Severity**: Mandatory  
**Impact**: High - Core UI functionality

**Description**: Windows Forms APIs for desktop applications are available in .NET 10 but require Windows-specific targeting. Many forms, controls, and event handling patterns need updates.

**Key Issues**:
- Binary incompatibilities in Form, Control, and component classes
- Event handler signature changes
- Designer code generation differences
- Layout and rendering behavior changes

**Resolution Strategy**:
- Target `net10.0-windows` framework (already SDK-style projects)
- Review and update control initialization patterns
- Test designer functionality for all forms
- Validate event subscriptions and handlers
- Focus on user-facing forms in Greenshot, Editor, and plugins

**Affected Projects**: All projects (most heavily: Greenshot.Editor: 6,083 issues, Greenshot: 3,392 issues, Greenshot.Base: 1,671 issues)

---

### 2. System.Drawing / GDI+ API Changes (4,954 issues)

#### Category: GdiDrawing
**Severity**: Mandatory  
**Impact**: High - Core screenshot functionality

**Description**: System.Drawing APIs (Image, Bitmap, Graphics, Brush, Pen, etc.) are Windows-specific in .NET 10. As a screenshot tool, Greenshot heavily relies on these APIs.

**Key Issues**:
- Image, Bitmap, Graphics classes marked as source incompatible
- PixelFormat, ImageFormat enumerations
- Image save/load operations
- Graphics rendering and manipulation
- Resolution properties (HorizontalResolution, VerticalResolution)

**Example Breaking Changes**:
```csharp
// Affected code pattern (from GreenshotFileFormatHandler.cs:117)
Log.InfoFormat("Resolution {0}x{1}", 
    surface.Image.HorizontalResolution, 
    surface.Image.VerticalResolution);

// System.Drawing APIs flagged:
- Image.Width, Image.Height
- Image.PixelFormat
- Image.HorizontalResolution, Image.VerticalResolution
- Image.Save(string, ImageFormat)
- Graphics rendering operations
```

**Resolution Strategy**:
- Ensure `net10.0-windows` targeting (System.Drawing.Common only works on Windows)
- Review all Image/Bitmap operations for compatibility
- Test image capture, manipulation, and export thoroughly
- Consider gradual migration to SixLabors.ImageSharp (already in use) for cross-platform future
- Keep System.Drawing for Windows-specific GDI+ operations

**Affected Projects**: Greenshot.Editor (2,235 issues), Greenshot.Base (1,380 issues), Greenshot (798 issues)

---

### 3. BinaryFormatter Obsolescence (42 issues, 2 critical)

#### Category: Remoting, Deprecated Remoting & Serialization
**Severity**: Mandatory  
**Impact**: Medium - Legacy file format support

**Description**: BinaryFormatter is obsolete and removed in .NET 10 due to security vulnerabilities. Greenshot uses it for legacy file format deserialization.

**Critical Occurrences**:
1. **Greenshot.Editor\FileFormat\V1\Legacy\LegacyFileHelper.cs:42**
   ```csharp
   BinaryFormatter binaryRead = new BinaryFormatter
   {
       Binder = new LegacySerializationBinder()
   };
   ```

2. **Greenshot.Test\Editor\FileFormat\V1\Legacy\LegacySerializationBinderTests.cs:46**
   ```csharp
   var binaryFormatter = new BinaryFormatter();
   ```

**Resolution Strategy (User-Requested Approach)**:
1. Add NuGet package `System.Runtime.Serialization.Formatters` to Greenshot.Editor and Greenshot.Test
2. Add suppression for obsolete warnings:
   ```csharp
   #pragma warning disable SYSLIB0011 // Type or member is obsolete
   // BinaryFormatter code
   #pragma warning restore SYSLIB0011
   ```
3. Add TODO comments indicating future migration to System.Text.Json or MessagePack
4. Document that legacy file format support is temporary

**Future Migration Path** (not in this upgrade):
- Migrate to System.Text.Json for new file formats
- Keep BinaryFormatter only for reading legacy .greenshot files
- Add deprecation warnings for legacy format

**Affected Projects**: Greenshot.Editor (production), Greenshot.Test (test code)

---

### 4. WPF API Changes (337 issues)

#### Category: Wpf
**Severity**: Mandatory  
**Impact**: Low-Medium - Some UI components use WPF

**Description**: While primarily Windows Forms, Greenshot uses some WPF components. WPF is available in .NET 10 but has some breaking changes.

**Key Issues**:
- XAML generated code (InternalTypeHelper)
- Markup extensions
- Resource handling
- Visual tree operations

**Resolution Strategy**:
- Target `net10.0-windows` (WPF requires Windows)
- Regenerate XAML designer files
- Review WPF control initialization
- Test any WPF-based dialogs or components

**Affected Projects**: Greenshot.Base (35 issues), Greenshot.Editor (72 issues), other projects with WPF usage

---

### 5. COM Interop Changes (4 issues)

#### Category: ComInterop
**Severity**: Mandatory  
**Impact**: Medium - Office plugin functionality

**Description**: COM interop for Microsoft Office automation has behavioral changes in .NET 10. The Office plugin integrates with Word, Excel, PowerPoint, OneNote, and Outlook.

**Key Issues**:
- COM object lifetime management changes
- Marshal behavior differences
- Type library interop updates

**Resolution Strategy**:
- Review Office plugin COM interop code
- Test with installed Office applications (Word, Excel, PowerPoint, Outlook)
- Ensure proper COM object disposal with `Marshal.ReleaseComObject`
- Validate Office Interop NuGet packages (Microsoft.Office.Interop.*)

**Affected Projects**: Greenshot.Plugin.Office, Greenshot (main app)

---

### 6. Code Access Security (CAS) Removal (2 issues)

#### Category: CodeAccessSecurity
**Severity**: Mandatory  
**Impact**: Low - Legacy security model

**Description**: Code Access Security (CAS) APIs removed in .NET 10. CAS was a .NET Framework security model that's no longer supported.

**Resolution Strategy**:
- Identify CAS usage in Greenshot main app
- Remove or replace with modern security patterns
- Likely used for plugin trust or permission checks

**Affected Projects**: Greenshot (2 issues)

---

### 7. Legacy Cryptography (2 issues)

#### Category: CryptographyLegacy
**Severity**: Mandatory  
**Impact**: Low - Encryption operations

**Description**: Some legacy cryptography APIs deprecated or changed in .NET 10.

**Resolution Strategy**:
- Review cryptography usage in Greenshot.Base
- Migrate to modern cryptography APIs
- Common updates: SHA1 → SHA256, DES → AES

**Affected Projects**: Greenshot.Base (2 issues)

---

### 8. Windows ACL Changes (1 issue)

#### Category: WindowsAcl
**Severity**: Mandatory  
**Impact**: Low - File permissions

**Description**: Windows Access Control List (ACL) APIs have changes in .NET 10.

**Resolution Strategy**:
- Review file permission code in Greenshot
- Update to modern ACL APIs
- Ensure proper Windows permission handling

**Affected Projects**: Greenshot (1 issue)

---

### 9. Legacy Windows Forms Controls (7 issues)

#### Category: WinFormsLegacyControls
**Severity**: Mandatory  
**Impact**: Low - Specific controls

**Description**: Some legacy Windows Forms controls deprecated or changed.

**Resolution Strategy**:
- Identify legacy controls (likely in Base and Editor)
- Replace with modern equivalents or custom controls
- Test control behavior and appearance

**Affected Projects**: Greenshot.Base (2 issues), Greenshot.Editor (4 issues), Greenshot (1 issue)

---

### Summary of Breaking Changes by Severity

| Category | Mandatory Issues | Potential Issues | Total |
|----------|-----------------|------------------|-------|
| Windows Forms | 11,607 | 1,665 | 13,272 |
| GDI+ / System.Drawing | 3,383 | 1,571 | 4,954 |
| WPF | 260 | 77 | 337 |
| BinaryFormatter/Remoting | 1 | 41 | 42 |
| Project Target Framework | 12 | 0 | 12 |
| NuGet Package Issues | 3 | 1 | 4 |
| COM Interop | 3 | 1 | 4 |
| Code Access Security | 2 | 0 | 2 |
| Legacy Cryptography | 1 | 1 | 2 |
| Windows ACL | 1 | 0 | 1 |
| Legacy Controls | 3 | 4 | 7 |
| **TOTAL** | **13,866** | **5,189** | **19,055** |

## Risk Management

### High-Risk Areas

#### 1. Windows Forms UI Rendering (Risk Level: HIGH)
**Issue**: 13,272 Windows Forms issues across all projects

**Risks**:
- Forms may not render correctly
- Controls may have layout issues
- Event handlers may not fire
- Designer-generated code may break
- High DPI scaling issues

**Mitigation**:
- Test each form individually after migration
- Use Windows Forms designer to validate forms
- Test on different DPI settings (100%, 125%, 150%, 200%)
- Keep .NET Framework version available for comparison
- Document any visual differences

**Rollback Plan**: Revert to previous git commit if critical rendering issues

---

#### 2. System.Drawing / Image Processing (Risk Level: HIGH)
**Issue**: 4,954 GDI+ issues - core screenshot functionality

**Risks**:
- Screenshot capture may fail
- Image quality degradation
- Memory leaks in image operations
- Performance regression
- Color space/format issues

**Mitigation**:
- Extensive testing of capture workflows
- Compare screenshots between .NET Framework and .NET 10 versions
- Profile memory usage during image operations
- Use existing SixLabors.ImageSharp as fallback for some operations
- Test all supported image formats (PNG, JPG, BMP, TIFF, GIF, ICO)

**Rollback Plan**: Revert if screenshot quality or capture reliability affected

---

#### 3. BinaryFormatter Security & Obsolescence (Risk Level: MEDIUM)
**Issue**: BinaryFormatter deprecated, 2 critical usages

**Risks**:
- Legacy file format (.greenshot files) may not open
- Security warnings in production code
- Future .NET versions may fully remove BinaryFormatter
- Compatibility shim package may not be maintained

**Mitigation (User-Requested Approach)**:
- Use `System.Runtime.Serialization.Formatters` package temporarily
- Suppress warnings with `#pragma warning disable SYSLIB0011`
- Add prominent TODO comments for future migration
- Document legacy format support status in release notes
- Consider adding "legacy format" warning to users
- Plan migration to System.Text.Json in future release

**Long-Term Plan**:
- Migrate to JSON-based format in next major version
- Provide converter tool for legacy files
- Deprecate legacy format in release notes

**Rollback Plan**: If legacy files cannot open, revert and reconsider approach

---

#### 4. Plugin Architecture & Loading (Risk Level: MEDIUM)
**Issue**: 8 plugins with dependency on Base library

**Risks**:
- Plugins may fail to load
- Plugin discovery mechanism may break
- Cross-plugin communication issues
- Plugin configuration may not persist

**Mitigation**:
- Test plugin loading mechanism early in Phase 4
- Verify plugin manifest format compatibility
- Test each plugin independently
- Test all plugins together
- Review plugin isolation/AppDomain usage (AppDomains removed in .NET Core)

**Rollback Plan**: Disable broken plugins individually, fix in hotfix

---

#### 5. COM Interop / Office Integration (Risk Level: MEDIUM)
**Issue**: Office plugin uses COM interop with 5 Office applications

**Risks**:
- COM marshaling behavior changes
- Memory leaks from improper COM cleanup
- Office automation may fail
- Version compatibility issues across Office versions

**Mitigation**:
- Remove incompatible packages (MicrosoftOfficeCore, Unofficial.Microsoft.mshtml)
- Review all `Marshal.ReleaseComObject` calls
- Test with Office 2016, 2019, 2021, Microsoft 365
- Test without Office installed (graceful degradation)
- Use try-catch for COM exceptions

**Rollback Plan**: Office plugin can be disabled if critical issues found

---

#### 6. Package Incompatibilities (Risk Level: MEDIUM)
**Issue**: 3 packages incompatible, must be updated or removed

**Risks**:
- Functionality loss from removed packages
- New package versions may have breaking API changes
- Package not available for .NET 10

**Affected Packages**:
1. **Dapplo.Jira.SvgWinForms** (Jira plugin)
   - Risk: SVG icon rendering may break
   - Mitigation: Test Jira plugin thoroughly, consider alternative SVG library

2. **MicrosoftOfficeCore** (Office plugin)
   - Risk: Office integration may break
   - Mitigation: Use standard Microsoft.Office.Interop.* assemblies

3. **Unofficial.Microsoft.mshtml** (Office plugin)
   - Risk: HTML parsing in Office plugin breaks
   - Mitigation: Replace with HtmlAgilityPack (already referenced)

**Rollback Plan**: Can disable specific plugins if package issues unresolvable

---

#### 7. Build System & CI/CD (Risk Level: LOW-MEDIUM)
**Issue**: Build system relies on MSBuild, CI uses .NET 7.x SDK

**Risks**:
- CI workflow may fail with .NET 10
- MSBuild compatibility issues
- Nerdbank.GitVersioning issues
- Inno Setup installer creation may fail

**Mitigation**:
- Update `.github/workflows/release.yml` to use .NET 10 SDK
- Test installer creation locally before pushing
- Verify Nerdbank.GitVersioning works with .NET 10
- Ensure MSBuild 17.0+ used

**Rollback Plan**: CI can be fixed independently of code changes

---

#### 8. Third-Party Dependencies (Risk Level: LOW)
**Issue**: 36 total NuGet packages

**Risks**:
- Transitive dependency conflicts
- Package runtime behavior changes
- Package security vulnerabilities

**Mitigation**:
- Most packages already compatible (33/36)
- Review package release notes for breaking changes
- Run dependency vulnerability scan post-upgrade
- Monitor package update notifications

**Rollback Plan**: Pin package versions if regression found

---

### Risk Mitigation Timeline

| Phase | Risk Level | Mitigation Strategy | Contingency |
|-------|-----------|---------------------|-------------|
| Phase 1 (Base) | HIGH | Small commits, incremental testing | Revert commit |
| Phase 2 (Editor) | HIGH | BinaryFormatter package, extensive testing | Revert commit |
| Phase 3 (Plugins) | MEDIUM | Parallel migration, independent testing | Disable plugin |
| Phase 4 (Main App) | MEDIUM-HIGH | End-to-end testing, comparison with old version | Revert commit |
| Phase 5 (Testing) | LOW | Automated + manual testing | Fix or document |

---

### Emergency Rollback Procedure

If critical issues discovered during any phase:

1. **Stop immediately** - Don't proceed to next phase
2. **Document the issue** - Capture error messages, screenshots, logs
3. **Assess impact** - Is it blocking? Can it be fixed quickly?
4. **Decision point**:
   - **Fix forward**: If fix is straightforward (< 2 hours), proceed
   - **Rollback**: If complex or blocking, revert changes
5. **Execute rollback**:
   ```powershell
   git reset --hard HEAD~1  # Revert last commit
   # Or identify specific commit to revert to
   git reset --hard <commit-hash>
   ```
6. **Replan**: Adjust plan based on learnings, then retry

---

### Success Indicators

Monitor these throughout migration:

✅ **Build Success Rate**: 100% of projects should build  
✅ **Warning Count**: Minimize warnings (exclude BinaryFormatter warnings)  
✅ **Runtime Startup**: Application starts in < 5 seconds  
✅ **Memory Baseline**: No significant memory increase vs .NET Framework  
✅ **Screenshot Quality**: Pixel-perfect or visually identical captures  
✅ **Plugin Load Rate**: 100% of plugins load successfully  
✅ **Manual Test Pass Rate**: 100% of manual test checklist items pass

## Testing & Validation Strategy

### Overview
Since Greenshot has **no automated test suite** (only 1 test project with minimal tests), validation relies heavily on **manual testing** and **build verification**.

---

### Testing Approach

#### Level 1: Build Validation (Per Phase)
**Goal**: Ensure code compiles without errors

**Process**:
1. Build individual project after migration
2. Review compilation errors and warnings
3. Fix errors iteratively
4. Document any suppressed warnings
5. Build dependent projects to verify integration

**Success Criteria**:
- Zero compilation errors
- Only expected warnings (BinaryFormatter obsolete warnings)
- Successful linking of dependencies

**Tools**:
```powershell
# Individual project build
msbuild <ProjectPath> /t:Rebuild /p:Configuration=Release /v:minimal

# Full solution build
msbuild src/Greenshot.sln /t:Rebuild /p:Configuration=Release /v:normal
```

---

#### Level 2: Automated Testing (Phase 5)
**Goal**: Run existing automated tests

**Process**:
1. Build Greenshot.Test.csproj
2. Run xunit tests via Test Explorer or CLI
3. Document test results
4. Investigate failures
5. Fix code or update tests as appropriate

**Expected Results**:
- Minimal test coverage (project has only basic tests)
- Legacy serialization tests may need updates
- Focus on tests that exist, don't create new ones in this upgrade

**Tools**:
```powershell
# Via Visual Studio Test Explorer
# Or via CLI if available
dotnet test Greenshot.Test\Greenshot.Test.csproj
```

---

#### Level 3: Functional Testing (Phase 4 & 5)
**Goal**: Verify all user-facing functionality works

**Test Environment**:
- Windows 10/11
- Multiple DPI settings (100%, 125%, 150%, 200%)
- With and without Office installed
- Multi-monitor setup (if available)

**Test Categories**:

##### 3.1 Application Lifecycle
- [ ] **Startup**: Application launches without errors
- [ ] **Tray Icon**: Icon appears in system tray
- [ ] **Context Menu**: Right-click menu functional
- [ ] **Settings**: Preferences dialog opens and saves
- [ ] **Shutdown**: Application closes cleanly
- [ ] **Restart**: Application restarts after configuration change

##### 3.2 Screenshot Capture (CRITICAL)
**This is the core functionality - test extensively**

- [ ] **Full Screen**: PrtScn key captures entire screen
- [ ] **Active Window**: Alt+PrtScn captures active window
- [ ] **Region**: Custom region selection with mouse
- [ ] **Last Region**: Repeat last region capture
- [ ] **Window from List**: Select window from list
- [ ] **Multiple Monitors**: Capture from each monitor
- [ ] **Capture Delay**: Timed capture works

**Validation**:
- Screenshot quality (no artifacts)
- Color accuracy
- Transparency handling (PNG)
- Correct resolution/DPI
- Cursor capture (if enabled)

##### 3.3 Image Editor (CRITICAL)
**Greenshot's primary differentiator**

Drawing Tools:
- [ ] Rectangle (filled/unfilled)
- [ ] Ellipse (filled/unfilled)
- [ ] Line
- [ ] Arrow
- [ ] Freehand drawing
- [ ] Text annotation
- [ ] Speech bubble

Effects:
- [ ] Highlight areas
- [ ] Obfuscate/blur
- [ ] Border effects
- [ ] Drop shadow

Operations:
- [ ] Crop
- [ ] Resize
- [ ] Rotate
- [ ] Undo/Redo (multiple levels)
- [ ] Zoom in/out

##### 3.4 Export Destinations
- [ ] **File**: Save as PNG, JPG, BMP, TIFF, GIF, ICO
- [ ] **Clipboard**: Copy to clipboard (image + metadata)
- [ ] **Printer**: Print dialog and printing
- [ ] **Email**: Attach to email (if client configured)
- [ ] **Office**: Word, Excel, PowerPoint, Outlook, OneNote
- [ ] **Cloud**: Box, Dropbox, Imgur
- [ ] **Issue Trackers**: Jira, Confluence
- [ ] **External Command**: Execute configured commands

##### 3.5 Configuration & Settings
- [ ] Capture settings (hotkeys, delay, cursor, etc.)
- [ ] Output settings (file format, quality, path)
- [ ] UI settings (language, theme)
- [ ] Plugin configuration (OAuth tokens, paths)
- [ ] Destination defaults
- [ ] Hotkey customization

##### 3.6 Plugin Functionality

**Box Plugin**:
- [ ] OAuth authentication
- [ ] Upload screenshot
- [ ] Create shareable link

**Confluence Plugin**:
- [ ] Server configuration
- [ ] Page selection
- [ ] Image attachment

**Dropbox Plugin**:
- [ ] OAuth authentication
- [ ] Upload screenshot
- [ ] Get shareable link

**ExternalCommand Plugin**:
- [ ] Command configuration
- [ ] Variable substitution
- [ ] Command execution
- [ ] Output handling

**Imgur Plugin**:
- [ ] Anonymous upload
- [ ] Authenticated upload
- [ ] Get image URL
- [ ] Open in browser

**Jira Plugin** ⚠️ (High risk due to package update):
- [ ] Server configuration
- [ ] Issue selection
- [ ] Image attachment
- [ ] SVG icon rendering (if Dapplo.Jira.SvgWinForms used)

**Office Plugin** ⚠️⚠️ (Highest risk due to COM interop & package removal):
- [ ] Word: Insert image into document
- [ ] Excel: Insert image into spreadsheet
- [ ] PowerPoint: Insert image into slide
- [ ] Outlook: Attach to email, insert inline
- [ ] OneNote: Insert into page
- [ ] Graceful failure when Office not installed

---

#### Level 4: Performance Testing
**Goal**: Ensure no performance regressions

**Metrics to Compare** (.NET Framework vs .NET 10):
- Application startup time
- Screenshot capture time (full screen, window, region)
- Image processing time (resize, effects)
- Memory usage (baseline, after 10 captures, after 100 captures)
- Plugin loading time
- Editor open time

**Tools**:
- Task Manager (memory monitoring)
- Stopwatch for timing critical operations
- Visual Studio Profiler (if regressions found)

**Acceptance Criteria**:
- Startup time: ≤ 10% slower than .NET Framework version
- Capture time: No noticeable difference
- Memory usage: ≤ 20% increase acceptable
- Plugin loading: ≤ 5 seconds total

---

#### Level 5: Compatibility Testing
**Goal**: Ensure compatibility across Windows versions and configurations

**Test Matrices**:

**Operating Systems**:
- [ ] Windows 10 (21H2, 22H2)
- [ ] Windows 11 (21H2, 22H2, 23H2)

**DPI Settings**:
- [ ] 100% (96 DPI)
- [ ] 125% (120 DPI)
- [ ] 150% (144 DPI)
- [ ] 200% (192 DPI)

**Monitor Configurations**:
- [ ] Single monitor
- [ ] Dual monitor (same resolution)
- [ ] Dual monitor (different resolutions)
- [ ] Dual monitor (different DPI)
- [ ] Triple+ monitor

**Office Versions** (for Office plugin):
- [ ] Office 2016
- [ ] Office 2019
- [ ] Office 2021
- [ ] Microsoft 365
- [ ] No Office (graceful degradation)

---

### Regression Testing

#### Critical User Workflows
Test these end-to-end scenarios:

**Workflow 1: Quick Screenshot to File**
1. Press PrtScn
2. Draw rectangle annotation
3. Save to file
4. Verify file created and correct

**Workflow 2: Screenshot to Clipboard**
1. Press PrtScn
2. Add text annotation
3. Copy to clipboard
4. Paste into another app (Paint, Word)
5. Verify image correct

**Workflow 3: Screenshot to Cloud**
1. Capture region
2. Add annotations
3. Upload to Imgur
4. Get shareable link
5. Verify image accessible

**Workflow 4: Screenshot to Email**
1. Capture window
2. Add effects (blur, highlight)
3. Send to Outlook
4. Verify email created with attachment

**Workflow 5: Multiple Screenshots Session**
1. Capture 10+ screenshots in session
2. Edit each one
3. Export to different destinations
4. Verify no memory leaks
5. Verify no performance degradation

---

### Issue Tracking During Testing

**For Each Issue Found**:
1. **Severity Classification**:
   - **Critical**: Crash, data loss, core feature broken → MUST fix before completion
   - **High**: Major feature broken, workaround exists → Fix before release
   - **Medium**: Minor feature broken, cosmetic issue → Fix or document
   - **Low**: Edge case, rarely used feature → Document for future

2. **Documentation**:
   - File issue in tracking system (or markdown doc)
   - Include repro steps
   - Note .NET Framework behavior for comparison
   - Assign to appropriate phase/project

3. **Resolution**:
   - Fix immediately (critical/high)
   - Schedule for fix (medium)
   - Document as known issue (low)
   - Revert if unfixable (critical with no workaround)

---

### Test Result Documentation

Create a test results document:
```
## .NET 10 Upgrade Test Results

### Build Validation
- [x] All projects build: YES/NO
- [x] Zero errors: YES/NO
- [x] Only expected warnings: YES/NO

### Automated Tests
- Total: X
- Passed: X
- Failed: X
- Skipped: X

### Manual Testing
- Core Features: X/Y passed
- Plugins: X/8 working
- Critical Workflows: X/5 successful

### Performance Comparison
| Metric | .NET Framework | .NET 10 | Change |
|--------|----------------|---------|--------|
| Startup Time | X ms | X ms | +/- X% |
| Capture Time | X ms | X ms | +/- X% |
| Memory (baseline) | X MB | X MB | +/- X% |

### Known Issues
1. [Issue description] - Severity: X - Status: X

### Conclusion
PASS / PASS WITH ISSUES / FAIL
```

---

### Validation Gates

**Gate 1** (After Phase 1): 
- Base library builds ✅
- Can proceed to Phase 2 ✅

**Gate 2** (After Phase 2):
- Editor builds ✅
- Editor can open programmatically ✅
- Can proceed to Phase 3 ✅

**Gate 3** (After Phase 3):
- All plugins build ✅
- Package issues resolved ✅
- Can proceed to Phase 4 ✅

**Gate 4** (After Phase 4):
- Main app builds ✅
- App starts without crash ✅
- Screenshot capture works ✅
- Can proceed to Phase 5 ✅

**Gate 5** (After Phase 5):
- All tests completed ✅
- No critical issues ✅
- **UPGRADE COMPLETE** ✅

If any gate fails, STOP and resolve before proceeding.

## Complexity & Effort Assessment

### Overall Complexity: **HIGH**

**Factors Contributing to High Complexity**:
1. **Issue Volume**: 19,055 total issues (13,866 mandatory)
2. **Windows Forms Dependency**: Extensive UI work (13,272 issues)
3. **GDI+ Usage**: Core functionality depends on System.Drawing (4,954 issues)
4. **Plugin Architecture**: 8 plugins requiring individual validation
5. **Limited Test Coverage**: Heavy reliance on manual testing
6. **Legacy Serialization**: BinaryFormatter usage requires package workaround

**Factors Reducing Complexity**:
1. ✅ **SDK-Style Projects**: All projects already modernized (no project file conversion)
2. ✅ **Package Compatibility**: 33/36 packages compatible
3. ✅ **Clear Dependency Tree**: Well-structured project dependencies
4. ✅ **Windows-Only**: No cross-platform concerns (net10.0-windows)
5. ✅ **Modern Alternatives Present**: SixLabors.ImageSharp already in use

---

### Effort Breakdown by Phase

#### Phase 1: Foundation (Greenshot.Base + BuildTasks)
**Complexity**: High  
**Effort**: 8-12 hours

**Breakdown**:
- Project file updates: 0.5 hours
- Windows Forms fixes: 3-4 hours
- System.Drawing fixes: 3-4 hours
- Legacy crypto/WPF fixes: 1 hour
- Build troubleshooting: 1-2 hours
- Validation: 1 hour

**Why High**: Base library used by all projects, must be stable before proceeding

---

#### Phase 2: Editor (Greenshot.Editor)
**Complexity**: Very High  
**Effort**: 12-18 hours

**Breakdown**:
- Project file updates: 0.5 hours
- BinaryFormatter package integration: 1 hour
- Windows Forms fixes (6,083 issues): 5-7 hours
- System.Drawing fixes (2,235 issues): 4-6 hours
- WPF fixes: 1 hour
- Designer file regeneration: 1 hour
- Build troubleshooting: 2-3 hours
- Functional testing: 2-3 hours

**Why Very High**: Largest issue count (8,467), critical editor functionality, complex drawing operations

---

#### Phase 3: Plugins (All 8)
**Complexity**: Medium (per plugin varies)  
**Effort**: 8-12 hours total (can be parallelized)

**Per-Plugin Breakdown**:

| Plugin | Issues | Complexity | Effort | Special Considerations |
|--------|--------|------------|--------|------------------------|
| Box | 157 | Low | 1 hour | HTTP/OAuth |
| Confluence | 375 | Low-Medium | 1-2 hours | REST API |
| Dropbox | 141 | Low | 1 hour | HTTP/OAuth |
| ExternalCommand | 508 | Medium | 1-2 hours | Process launching |
| Imgur | 648 | Medium | 1-2 hours | HTTP/OAuth |
| **Jira** ⚠️ | 774 | **Medium-High** | **2-3 hours** | **Package incompatibility** |
| **Office** ⚠️⚠️ | 40 | **High** | **3-4 hours** | **COM interop, 2 incompatible packages** |
| Test | 449 | Low | Covered in Phase 5 | Test project |

**Why Medium Overall**: Plugins are independent, issues mostly API updates, but Jira and Office have package complications

---

#### Phase 4: Main Application (Greenshot.csproj)
**Complexity**: High  
**Effort**: 8-12 hours

**Breakdown**:
- Project file updates: 0.5 hours
- Windows Forms fixes (3,392 issues): 4-5 hours
- System.Drawing fixes (798 issues): 2-3 hours
- COM interop fixes: 1 hour
- CAS removal: 0.5 hours
- Windows ACL updates: 0.5 hours
- Plugin loading mechanism: 1 hour
- Build troubleshooting: 1-2 hours
- Integration testing: 2-3 hours

**Why High**: Main application orchestrates everything, integration complexity, user-facing functionality

---

#### Phase 5: Testing & Validation
**Complexity**: Medium  
**Effort**: 4-6 hours

**Breakdown**:
- Test project migration: 1 hour
- Automated test execution: 0.5 hours
- Manual test checklist execution: 3-4 hours
- Performance comparison: 1 hour
- Documentation of results: 0.5 hours

---

### Total Effort Summary

| Phase | Complexity | Effort | Can Parallelize? |
|-------|-----------|---------|------------------|
| Phase 1: Foundation | High | 8-12 hours | No (blocking) |
| Phase 2: Editor | Very High | 12-18 hours | No (blocks Phase 4) |
| Phase 3: Plugins | Medium | 8-12 hours | Yes (all 8 plugins) |
| Phase 4: Main App | High | 8-12 hours | No (depends on 1-3) |
| Phase 5: Testing | Medium | 4-6 hours | No (final validation) |
| **TOTAL** | **High** | **40-60 hours** | - |

**With Parallelization** (if multiple developers):
- Minimum: ~32-42 hours (plugins done simultaneously)
- Realistic with 1 developer: 40-60 hours
- Team of 2-3: 20-30 hours elapsed time

---

### Risk-Adjusted Effort (Contingency)

**Add 20-30% contingency for**:
- Unforeseen API incompatibilities
- Complex Windows Forms designer issues
- COM interop troubleshooting
- Plugin integration problems
- Build system issues

**Risk-Adjusted Total**: 48-78 hours

---

### Complexity Factors by Category

#### High Complexity (🔴)
- Windows Forms migration (13,272 issues)
- System.Drawing migration (4,954 issues)
- Editor component (8,467 issues in one project)
- Office plugin (COM interop + package removal)

#### Medium Complexity (🟡)
- Plugin architecture updates
- BinaryFormatter workaround
- Jira plugin (package incompatibility)
- Build system adjustments

#### Low Complexity (🟢)
- BuildTasks project (2 issues)
- Most plugin migrations (standard API updates)
- Package updates (mostly compatible)
- SDK-style projects (already done)

---

### Skill Requirements

**Required Skills**:
- C# and .NET Framework → .NET Core/10 migration experience
- Windows Forms development (critical)
- GDI+ / System.Drawing expertise
- MSBuild and project file management
- NuGet package management
- Git version control

**Nice-to-Have Skills**:
- COM interop knowledge (Office plugin)
- OAuth/REST API integration (cloud plugins)
- WPF basics
- Image processing concepts
- Serialization patterns

---

### Effort Reduction Opportunities

**Could Reduce Effort By**:
1. **Defer Plugin Migration**: Migrate main app first, plugins later (saves 8-12 hours initially)
   - Risk: Incomplete testing, user feature loss

2. **Disable High-Risk Plugins**: Temporarily disable Office and Jira plugins (saves 5-7 hours)
   - Risk: User complaints about missing features

3. **Accept Some Breaking Changes**: Document known issues instead of fixing all (saves 10-20 hours)
   - Risk: Poor upgrade experience, user dissatisfaction

4. **Use Automated Tools**: Investigate .NET Upgrade Assistant for bulk fixes (could save 20-30%)
   - Risk: Tool may not handle complex scenarios, still needs validation

**Recommendation**: Complete full migration as planned. Greenshot is a mature, stable product with users expecting reliability. Cutting corners risks reputation.

---

### Timeline Estimate (Single Developer)

**Aggressive** (40 hours):
- Week 1: Phases 1-2 (20 hours)
- Week 2: Phases 3-5 (20 hours)
- **Total**: 2 weeks @ 20 hrs/week

**Moderate** (50 hours):
- Week 1: Phase 1 (10 hours)
- Week 2: Phase 2 (15 hours)
- Week 3: Phases 3-4 (15 hours)
- Week 4: Phase 5 (10 hours)
- **Total**: 4 weeks @ 12-15 hrs/week

**Conservative** (60+ hours):
- Spread over 6-8 weeks @ 8-10 hrs/week
- Allows time for thorough testing
- Buffer for unexpected issues

**Recommended**: Conservative approach for production software with users

## Source Control Strategy

### Branch Strategy

**Working Branch**: `draft/next_dotnet_playground`  
**Status**: Already on target branch, no pending changes  

**Approach**: Direct commits to dedicated upgrade branch (as requested by user)

---

### Commit Strategy

#### Commit After Each Phase
Create clear, descriptive commits at phase boundaries:

**Phase 1 Commit**:
```
chore: Upgrade Greenshot.Base and Greenshot.BuildTasks to .NET 10

- Updated TargetFramework to net10.0-windows
- Fixed Windows Forms API incompatibilities
- Updated System.Drawing usage patterns
- Replaced legacy cryptography APIs
- Resolved 3,219 compatibility issues

Projects: Greenshot.Base, Greenshot.BuildTasks
Status: Foundation projects build successfully
```

**Phase 2 Commit**:
```
chore: Upgrade Greenshot.Editor to .NET 10

- Updated TargetFramework to net10.0-windows
- Added System.Runtime.Serialization.Formatters for BinaryFormatter support
- Suppressed BinaryFormatter obsolete warnings (temporary, TODO: migrate)
- Updated System.Text.Json to 10.0.5
- Fixed 8,467 compatibility issues (Windows Forms, GDI+, WPF)
- Validated image editor functionality

Projects: Greenshot.Editor
Status: Editor builds and basic functionality tested
```

**Phase 3 Commit**:
```
chore: Upgrade all Greenshot plugins to .NET 10

- Updated 8 plugins to target net10.0-windows
- Fixed Jira plugin: Updated Dapplo.Jira.SvgWinForms package
- Fixed Office plugin: Removed incompatible packages (MicrosoftOfficeCore, mshtml)
- Resolved plugin API incompatibilities
- Fixed 3,141 compatibility issues across plugins

Plugins: Box, Confluence, Dropbox, ExternalCommand, Imgur, Jira, Office, Test
Status: All plugins build successfully
```

**Phase 4 Commit**:
```
chore: Upgrade Greenshot main application to .NET 10

- Updated TargetFramework to net10.0-windows
- Fixed Windows Forms incompatibilities in main app
- Updated System.Drawing usage in capture helpers
- Removed Code Access Security (CAS) usage
- Updated Windows ACL code
- Fixed COM interop for Office integration
- Resolved 4,277 compatibility issues
- Validated plugin loading mechanism

Projects: Greenshot
Status: Main application builds and starts successfully
```

**Phase 5 Commit**:
```
chore: Upgrade test project and validate .NET 10 migration

- Updated Greenshot.Test to net10.0-windows
- Added BinaryFormatter compatibility package to tests
- Fixed test incompatibilities
- Executed automated tests: X passed, Y failed
- Completed manual testing checklist
- Performance validated (startup, capture, memory)

Projects: Greenshot.Test
Status: Migration complete, all validation passed
```

**Final Commit** (CI/CD Update):
```
ci: Update GitHub Actions workflow for .NET 10

- Updated .NET SDK version from 7.x to 10.x in release.yml
- Validated installer creation with .NET 10
- Verified portable package creation

Status: CI/CD pipeline ready for .NET 10
```

---

### Commit Guidelines

**When to Commit**:
- ✅ After each phase completes successfully
- ✅ After fixing a major blocking issue
- ✅ Before attempting risky changes (create checkpoint)
- ✅ End of each work session

**When NOT to Commit**:
- ❌ Code doesn't compile
- ❌ Critical functionality broken
- ❌ Mid-phase with unstable state
- ❌ Experimental changes not validated

**Commit Message Format**:
```
type: Short description (72 chars max)

- Bullet point details
- What changed
- Why changed
- Issues resolved

Projects: ProjectA, ProjectB
Status: Current state
```

**Commit Types**:
- `chore:` - Upgrade work, project updates
- `fix:` - Bug fixes during migration
- `ci:` - CI/CD updates
- `docs:` - Documentation updates

---

### Branch Protection

**Current State**: Working on `draft/next_dotnet_playground` (dedicated branch)

**Recommendations**:
- Keep frequent commits for rollback points
- Tag significant milestones:
  - `upgrade-net10-phase1-complete`
  - `upgrade-net10-phase2-complete`
  - etc.
- Don't force push (preserve history)

---

### Merge Strategy (Post-Upgrade)

Once upgrade complete and validated:

**Option 1: Direct Merge to Main** (if `draft/next_dotnet_playground` was created for this upgrade)
```bash
git checkout main
git merge draft/next_dotnet_playground --no-ff
git tag v1.5.0-net10  # or appropriate version
git push origin main --tags
```

**Option 2: Pull Request** (recommended for review)
```bash
# Create PR: draft/next_dotnet_playground → main
# Review changes
# Merge with squash or merge commit
```

**Option 3: Keep Branch Long-Term** (if this is ongoing development branch)
```bash
# Continue development on draft/next_dotnet_playground
# Merge to main when ready for release
```

---

### Rollback Scenarios

#### Scenario 1: Phase Fails (Minor Issues)
**Action**: Fix forward
```bash
# Fix the issue
git add .
git commit -m "fix: Resolve [issue] in Phase X"
```

#### Scenario 2: Phase Fails (Major Blocking Issues)
**Action**: Rollback to previous phase
```bash
# Rollback last commit
git reset --hard HEAD~1

# Or rollback to specific commit
git reset --hard <commit-hash-before-phase>

# If already pushed
git revert <commit-hash>
```

#### Scenario 3: Complete Upgrade Failure
**Action**: Rollback entire upgrade
```bash
# Find commit before upgrade started
git log --oneline

# Reset to that commit
git reset --hard <commit-hash-before-upgrade>

# Or delete branch and start over
git checkout main
git branch -D draft/next_dotnet_playground
git checkout -b draft/next_dotnet_playground
```

---

### Git Workflow Best Practices

**Before Starting Each Phase**:
```bash
# Verify on correct branch
git branch

# Verify clean state
git status

# Pull latest changes (if team work)
git pull origin draft/next_dotnet_playground
```

**During Phase Work**:
```bash
# Stage changes incrementally
git add <file>

# Commit small logical units
git commit -m "fix: Update Form API in MainForm.cs"

# Push regularly (backup)
git push origin draft/next_dotnet_playground
```

**After Phase Completion**:
```bash
# Create phase completion commit (examples above)
git add .
git commit -m "chore: Upgrade Phase X complete"

# Tag milestone
git tag upgrade-net10-phaseX-complete

# Push with tags
git push origin draft/next_dotnet_playground --tags
```

---

### Stash Strategy (For Interrupted Work)

If you need to pause mid-phase:
```bash
# Save work in progress
git stash push -m "WIP: Phase X - partial Windows Forms fixes"

# Continue later
git stash list
git stash pop
```

---

### Conflict Resolution

**Unlikely in single-developer scenario**, but if working with team:

```bash
# Before starting each session
git pull origin draft/next_dotnet_playground

# If conflicts occur
# Resolve in IDE or editor
git add <resolved-files>
git commit -m "merge: Resolve conflicts in Phase X"
```

---

### History Preservation

**DO**:
- ✅ Keep all commits (detailed history useful for learning)
- ✅ Tag milestones
- ✅ Write descriptive commit messages
- ✅ Reference issue numbers if tracking externally

**DON'T**:
- ❌ Force push (unless absolutely necessary)
- ❌ Rewrite history on shared branch
- ❌ Squash commits until final merge to main
- ❌ Commit broken code

## Success Criteria

### Phase-Level Success Criteria

#### Phase 1: Foundation ✅
- [ ] Greenshot.Base.csproj builds with zero errors
- [ ] Greenshot.BuildTasks.csproj builds with zero errors
- [ ] Only expected warnings (BinaryFormatter if used)
- [ ] Base library can be referenced by other projects
- [ ] Custom build tasks execute successfully

#### Phase 2: Editor ✅
- [ ] Greenshot.Editor.csproj builds with zero errors
- [ ] System.Text.Json updated to 10.0.5
- [ ] BinaryFormatter package integrated
- [ ] Legacy file format can still be opened (backward compatibility)
- [ ] Image editor window opens without crash
- [ ] Basic drawing operations work (line, rectangle, text)

#### Phase 3: Plugins ✅
- [ ] All 8 plugin projects build with zero errors
- [ ] Dapplo.Jira.SvgWinForms package issue resolved
- [ ] MicrosoftOfficeCore package removed from Office plugin
- [ ] Unofficial.Microsoft.mshtml removed or replaced
- [ ] Each plugin's configuration UI opens
- [ ] No missing dependencies in plugin output folders

#### Phase 4: Main Application ✅
- [ ] Greenshot.csproj builds with zero errors
- [ ] Application launches without crashes
- [ ] Screenshot capture works (all modes)
- [ ] All 8 plugins load successfully
- [ ] Tray icon appears and menu functions
- [ ] Settings dialog works
- [ ] Can save screenshots to file

#### Phase 5: Testing & Validation ✅
- [ ] Greenshot.Test.csproj builds with zero errors
- [ ] All existing automated tests pass (or failures documented)
- [ ] Manual testing checklist 100% complete
- [ ] No critical or high-severity issues found
- [ ] Performance within acceptable range

---

### Overall Success Criteria

#### Must-Have (BLOCKING - Must Pass) 🔴

**Build Requirements**:
- [ ] All 12 projects build successfully with `msbuild src/Greenshot.sln /t:Rebuild /p:Configuration=Release`
- [ ] Zero compilation errors
- [ ] Only expected/suppressed warnings
- [ ] Installer creation succeeds (Tools.InnoSetup works)

**Core Functionality**:
- [ ] Application launches on Windows 10/11
- [ ] Screenshot capture works for all modes (full screen, window, region)
- [ ] Image editor opens and allows basic editing
- [ ] Screenshots can be saved to file (PNG, JPG)
- [ ] Screenshot can be copied to clipboard
- [ ] Tray icon and hotkeys work

**Plugin Loading**:
- [ ] All 8 plugins load without errors
- [ ] Plugin architecture intact
- [ ] No runtime exceptions during plugin initialization

**Critical Workflows**:
- [ ] Capture → Edit → Save to File
- [ ] Capture → Copy to Clipboard
- [ ] Capture → Upload to Cloud (at least 2 plugins working)

**Data Integrity**:
- [ ] Configuration settings persist across restarts
- [ ] Legacy .greenshot files can be opened (BinaryFormatter compatibility)
- [ ] No data loss in existing user configurations

---

#### Should-Have (HIGH PRIORITY - Should Pass) 🟡

**Plugin Functionality**:
- [ ] Box plugin: Upload and get shareable link
- [ ] Confluence plugin: Attach to Confluence page
- [ ] Dropbox plugin: Upload to Dropbox
- [ ] External Command plugin: Execute configured commands
- [ ] Imgur plugin: Upload and get URL
- [ ] Jira plugin: Attach to Jira issue (package fixed)
- [ ] Office plugin: Insert into Word, Excel, PowerPoint, Outlook

**Editor Features**:
- [ ] All drawing tools work (shapes, arrows, text, freehand)
- [ ] Effects work (highlight, obfuscate, blur)
- [ ] Operations work (crop, resize, rotate)
- [ ] Undo/redo functional
- [ ] All export formats work (PNG, JPG, BMP, TIFF, GIF, ICO)

**Configuration**:
- [ ] All settings categories accessible
- [ ] Hotkey customization works
- [ ] Plugin configuration works
- [ ] Language selection works (if multilingual)

**Performance**:
- [ ] Startup time ≤ 10% slower than .NET Framework
- [ ] Screenshot capture time comparable
- [ ] Memory usage ≤ 20% higher
- [ ] No memory leaks over extended use

**Compatibility**:
- [ ] Works on Windows 10 and Windows 11
- [ ] Works across different DPI settings
- [ ] Works with multi-monitor setups

---

#### Nice-to-Have (OPTIONAL - Can Document Issues) 🟢

**Polish**:
- [ ] No cosmetic UI differences from .NET Framework version
- [ ] All fonts render correctly
- [ ] Icons and images display properly at all DPI levels

**Edge Cases**:
- [ ] Works with Office 2016, 2019, 2021, Microsoft 365
- [ ] Graceful degradation when Office not installed
- [ ] Works with 3+ monitors
- [ ] Works with mixed DPI monitors

**Advanced Features**:
- [ ] Email destination works with various email clients
- [ ] Print functionality works with different printer types
- [ ] All cloud service authentications work

**Performance Optimization**:
- [ ] Startup time faster than .NET Framework (possible with .NET 10)
- [ ] Lower memory footprint (possible with GC improvements)
- [ ] Better high DPI performance

---

### Acceptance Testing Matrix

| Category | Feature | Must-Have | Should-Have | Nice-to-Have | Status |
|----------|---------|-----------|-------------|--------------|--------|
| **Build** | All projects compile | 🔴 | | | ⏳ |
| **Build** | Zero errors | 🔴 | | | ⏳ |
| **Startup** | App launches | 🔴 | | | ⏳ |
| **Capture** | Full screen | 🔴 | | | ⏳ |
| **Capture** | Window | 🔴 | | | ⏳ |
| **Capture** | Region | 🔴 | | | ⏳ |
| **Editor** | Opens | 🔴 | | | ⏳ |
| **Editor** | Basic drawing | 🔴 | | | ⏳ |
| **Editor** | All tools | | 🟡 | | ⏳ |
| **Export** | Save to file | 🔴 | | | ⏳ |
| **Export** | Clipboard | 🔴 | | | ⏳ |
| **Export** | All formats | | 🟡 | | ⏳ |
| **Plugins** | All load | 🔴 | | | ⏳ |
| **Plugins** | Box works | | 🟡 | | ⏳ |
| **Plugins** | Confluence works | | 🟡 | | ⏳ |
| **Plugins** | Dropbox works | | 🟡 | | ⏳ |
| **Plugins** | ExternalCommand works | | 🟡 | | ⏳ |
| **Plugins** | Imgur works | | 🟡 | | ⏳ |
| **Plugins** | Jira works | | 🟡 | | ⏳ |
| **Plugins** | Office works | | 🟡 | | ⏳ |
| **Config** | Settings persist | 🔴 | | | ⏳ |
| **Config** | Hotkeys work | 🔴 | | | ⏳ |
| **Legacy** | Old files open | 🔴 | | | ⏳ |
| **Perf** | Startup time OK | | 🟡 | | ⏳ |
| **Perf** | Memory OK | | 🟡 | | ⏳ |
| **Compat** | Win10/11 | 🔴 | | | ⏳ |
| **Compat** | Multi-DPI | | 🟡 | | ⏳ |

Legend: 🔴 Blocking | 🟡 Important | ⏳ Pending | ✅ Pass | ❌ Fail

---

### Definition of Done

The .NET 10 upgrade is considered **COMPLETE** when:

#### Technical Completion
1. ✅ All 12 projects target net10.0-windows
2. ✅ All projects build without errors using MSBuild
3. ✅ All Must-Have success criteria pass (🔴)
4. ✅ At least 80% of Should-Have criteria pass (🟡)
5. ✅ No critical or high-severity bugs in core workflows
6. ✅ All automated tests pass
7. ✅ BinaryFormatter compatibility package integrated
8. ✅ All incompatible packages removed or updated

#### Functional Completion
9. ✅ Application runs on Windows 10 and Windows 11
10. ✅ All 5 critical user workflows pass (see Testing section)
11. ✅ All 8 plugins either work or gracefully degrade
12. ✅ No data loss for user configurations or screenshots
13. ✅ Legacy file format support maintained

#### Quality Gates
14. ✅ Performance within acceptable ranges (≤10% slower startup, ≤20% memory increase)
15. ✅ No security warnings (except suppressed BinaryFormatter)
16. ✅ Build warning count documented and justified
17. ✅ All known issues documented

#### Documentation & CI/CD
18. ✅ CI/CD pipeline updated for .NET 10 SDK
19. ✅ Release notes updated with .NET 10 upgrade info
20. ✅ Known issues documented (if any)
21. ✅ Upgrade plan marked complete
22. ✅ Test results documented

---

### Sign-Off Checklist

**Technical Lead Sign-Off**:
- [ ] Code review completed
- [ ] All phases validated
- [ ] Architecture integrity maintained
- [ ] No technical debt introduced (beyond planned BinaryFormatter workaround)

**QA Sign-Off**:
- [ ] Manual testing complete
- [ ] All critical workflows validated
- [ ] Performance benchmarks acceptable
- [ ] No regressions in core functionality

**Product Owner Sign-Off**:
- [ ] Feature parity maintained
- [ ] User experience not degraded
- [ ] Release plan approved
- [ ] Communication plan ready

---

### Release Readiness

Before releasing .NET 10 version to users:

**Pre-Release Checklist**:
- [ ] Beta testing with select users (1-2 weeks)
- [ ] Collect feedback on performance and compatibility
- [ ] Fix any critical issues found
- [ ] Update documentation and help files
- [ ] Prepare release notes highlighting .NET 10
- [ ] Update download page with .NET 10 requirements
- [ ] Create rollback plan for users (offer .NET Framework version)

**Release Criteria**:
- [ ] No critical bugs reported in beta
- [ ] All must-have criteria met
- [ ] At least 90% should-have criteria met
- [ ] Positive feedback from beta testers
- [ ] CI/CD pipeline creates installer successfully
