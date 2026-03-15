
## [2026-03-15 13:45] TASK-001: Verify prerequisites

Status: Complete

**Verified**:
- .NET 10.0 SDK installed and available
- No global.json file to conflict with upgrade
- MSBuild 18.4.0 available (exceeds requirement of 17.0+)

All prerequisites met for .NET 10.0 upgrade.


## [2026-03-15 13:47] TASK-002: Atomic upgrade of all 12 projects to .NET 10.0

**Files Modified**:
- src/Directory.Build.props: Updated TargetFramework from net480 to net10.0-windows (affects 11 projects)
- src/Greenshot.Test/Greenshot.Test.csproj: Updated TargetFramework from net481 to net10.0-windows

All 12 projects now target net10.0-windows.


## [2026-03-15 13:48] TASK-002: Atomic upgrade of all 12 projects to .NET 10.0

**Files Modified**:
- src/Greenshot.Editor/Greenshot.Editor.csproj: Updated System.Text.Json 10.0.2→10.0.5, added System.Runtime.Serialization.Formatters 10.0.5
- src/Greenshot.Test/Greenshot.Test.csproj: Added System.Runtime.Serialization.Formatters 10.0.5
- src/Greenshot.Plugin.Office/Greenshot.Plugin.Office.csproj: Removed incompatible packages (MicrosoftOfficeCore, Unofficial.Microsoft.mshtml) and their embedding targets

All package updates complete per plan.


## [2026-03-15 13:49] TASK-002: Atomic upgrade of all 12 projects to .NET 10.0

**Files Modified**:
- src/Greenshot.Editor/FileFormat/V1/Legacy/LegacyFileHelper.cs: Added #pragma warning disable/restore SYSLIB0011 around BinaryFormatter usage with TODO comment
- src/Greenshot.Test/Editor/FileFormat/V1/Legacy/LegacySerializationBinderTests.cs: Added #pragma warning disable/restore SYSLIB0011 around BinaryFormatter usage with TODO comment

BinaryFormatter obsolete warnings suppressed with documentation per user request.


## [2026-03-15 13:50] TASK-002: Atomic upgrade of all 12 projects to .NET 10.0

**Files Modified**:
- src/Directory.Build.props: Updated RuntimeIdentifiers from win10-x64;win10-x86 to win-x64;win-x86 for .NET 10 compatibility

**Verified**:
- NuGet package restore successful for all 12 projects
- 2 warnings about System.Text.Json trimming (informational only, not blocking)

All dependencies restored successfully.


## [2026-03-15 13:53] TASK-002: Atomic upgrade of all 12 projects to .NET 10.0

Status: Paused

**Verified**:
- Initial build attempt reveals compilation errors in Greenshot.Base
- RuntimeIdentifiers updated from win10-x64/x86 to win-x64/x86

**Code Changes**:
- Fixed ScreenCaptureMode ambiguity in CoreConfiguration.cs

**Errors Fixed**:
- Screen CaptureMode ambiguous reference (qualified with full namespace)
- RuntimeIdentifier compatibility (updated to modern RID format)

**Blocking Issues Found**:
1. **System.Runtime.Remoting removed** - COMWrapper.cs uses RealProxy, IRemotingTypeInfo, IMessage (no .NET 10 equivalent)
2. **System.Runtime.InteropServices.CustomMarshalers removed** - IDispatch.cs uses TypeToTypeInfoMarshaler (no .NET 10 equivalent)
3. **BuildTasks loading issue** - MSBuild task instantiation failing

These are significant breaking changes requiring refactoring. COMWrapper uses .NET Remoting for COM proxy which was removed in .NET Core/.NET 10. Files affected:
- Greenshot.Base/Interop/COMWrapper.cs (uses Remoting)
- Greenshot.Base/Interop/IDispatch.cs (uses CustomMarshalers)
- Greenshot.Base/Core/WindowDetails.cs (uses COMWrapper)

Partial - Build blocked by removed APIs


## [2026-03-15 14:11] TASK-002: Atomic upgrade of all 12 projects to .NET 10.0

**Files Modified**:
- src/Greenshot.Base/Interop/COMWrapper.cs: Completely refactored - removed .NET Remoting (RealProxy) dependency, modernized for .NET 10 using direct RCW
- src/Greenshot.Base/Interop/COMWrapper.cs.old: Original file backed up
- src/Greenshot.Base/Interop/IDispatch.cs.old: Original file backed up (no longer needed without wrapping)
- src/Greenshot.Base/Controls/GreenshotForm.cs: Added DesignerSerializationVisibility attribute to LanguageKey property
- src/Greenshot.Base/Controls/QualityDialog.cs: Added DesignerSerializationVisibility attribute to Settings property  
- src/Greenshot.Base/Controls/HotkeyControl.cs: Added DesignerSerializationVisibility attributes to Hotkey and HotkeyModifiers properties
- src/Greenshot.Editor/FileFormat/V1/GreenshotFileV1.cs: Removed incorrect System.ServiceModel.Security using
- src/Greenshot.Editor/FileFormat/V1/Legacy/LegacySerializationBinder.cs: Removed incorrect System.ServiceModel.Security using

**Code Changes**:
- **Major refactoring**: Replaced .NET Remoting-based COMWrapper with modern RCW-based implementation
- Removed RealProxy, IRemotingTypeInfo, IMessage dependencies (not available in .NET 10)
- Removed IDispatch CustomMarshalers dependency (not available in .NET 10)
- Simplified COM interop to use modern .NET's built-in COM support
- Added Windows Forms designer serialization attributes for .NET 10 compatibility

**Errors Fixed**:
- System.Runtime.Remoting namespace removal (refactored COM wrapper)
- System.Runtime.InteropServices.CustomMarshalers removal (removed IDispatch wrapping)
- WFO1000 Windows Forms designer serialization errors (added DesignerSerializationVisibility attributes)
- System.ServiceModel.Security namespace errors (corrected using statements)

**Build Status**:
- Greenshot.Base now builds successfully with only obsolete API warnings
- Continuing with full solution build

Continuing action 9 - fixing compilation errors.


## [2026-03-15 15:31] TASK-002: Atomic upgrade of all 12 projects to .NET 10.0

**Files Modified**:
- src/Greenshot.Base/Interop/COMWrapper.cs: Refactored from .NET Remoting to modern RCW
- src/Greenshot.Base/Controls/GreenshotForm.cs, QualityDialog.cs, HotkeyControl.cs: Added DesignerSerializationVisibility attributes
- src/Greenshot.Editor/: Added DesignerSerializationVisibility attributes to 14+ control properties across Surface.cs and other controls
- src/Greenshot.Editor/FileFormat/V1/: Removed System.ServiceModel.Security using statements
- src/Greenshot.Editor/Forms/TextObfuscationForm.cs: Fixed ObserveOn to use SynchronizationContext
- src/Greenshot.Plugin.Office/OfficeInterop/MsoEnums.cs: Created local enum definitions for MsoTriState and MsoScaleFrom
- src/Greenshot.Plugin.Office/OfficeExport/: Updated all exporters to use dynamic for COM interop, removed mshtml HTML editing
- src/Greenshot.Plugin.Jira/: Added System.Runtime.Caching package, fixed ObserveOn in JiraForm
- src/Greenshot.BuildTasks/Greenshot.BuildTasks.csproj: Changed SDK to Microsoft.NET.Sdk, added CopyLocalLockFileAssemblies
- src/Greenshot/Greenshot.csproj: Added TargetFramework override to net10.0-windows10.0.26100.0, updated BuildTasks path
- src/Greenshot/Forms/MainForm.cs: Added using aliases for MethodInvoker and ScreenCaptureMode
- src/Greenshot/Helpers/CaptureHelper.cs: Added ScreenCaptureMode using alias
- src/Greenshot/Helpers/PluginHelper.cs: Added MethodInvoker using alias
- src/Greenshot/Helpers/ResourceMutex.cs: Fixed Mutex constructor for .NET 10 API changes
- src/Greenshot/Native/WindowsGraphicsCaptureInterop.cs: Updated to use GraphicsCaptureItem.As<> instead of GetActivationFactory
- src/Greenshot/Forms/ToolStripMenuSelectList.cs: Added DesignerSerializationVisibility attribute
- src/Greenshot.Test/: Removed System.Web.UI.WebControls using

**Files Created**:
- src/Greenshot.Plugin.Office/OfficeInterop/MsoEnums.cs: Local definitions for Office COM enums

**Files Backed Up**:
- src/Greenshot.Base/Interop/COMWrapper.cs.old
- src/Greenshot.Base/Interop/IDispatch.cs.old

**Code Changes**:
- Major COM interop refactoring - removed .NET Remoting dependencies
- Fixed 25+ Windows Forms designer serialization compatibility issues
- Migrated Windows Runtime API calls to .NET 10 patterns
- Fixed Office plugin COM interop using dynamic for type safety
- Disabled Outlook HTML inline editing (mshtml not available in .NET 10)
- Fixed Reactive Extensions scheduler usage

**Build Status**:
- ✅ All 12 projects build successfully with 0 errors
- Warnings present (obsolete APIs, SDK recommendations) - non-blocking

**Verified**:
- Complete solution builds: 0 errors
- All projects targeting net10.0-windows
- All dependencies resolved

Success - All 12 projects upgraded to .NET 10 and building successfully.

