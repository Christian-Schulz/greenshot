# .NET 9.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 9.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 9.0 upgrade.
3. Upgrade Greenshot.Base.csproj
4. Upgrade Greenshot.Editor.csproj 
5. Upgrade Greenshot.Test.csproj
6. Upgrade Greenshot.Plugin.Win10.csproj
7. Upgrade Greenshot.Plugin.Photobucket.csproj
8. Upgrade Greenshot.Plugin.Office.csproj
9. Upgrade Greenshot.Plugin.GooglePhotos.csproj
10. Upgrade Greenshot.Plugin.Flickr.csproj
11. Upgrade Greenshot.Plugin.Dropbox.csproj
12. Upgrade Greenshot.Plugin.Box.csproj
13. Upgrade Greenshot.Plugin.Jira.csproj
14. Upgrade Greenshot.Plugin.Imgur.csproj
15. Upgrade Greenshot.Plugin.ExternalCommand.csproj
16. Upgrade Greenshot.csproj
17. Run unit tests to validate upgrade in the projects listed below:
    - Greenshot.Test.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|
| Greenshot.Plugin.Confluence.csproj            | Excluded per user request   |

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                        | Current Version | New Version | Description                         |
|:------------------------------------|:---------------:|:-----------:|:------------------------------------|
| MicrosoftOfficeCore                 |   15.0.0        |  Remove     | No supported version found          |
| Unofficial.Microsoft.mshtml         |   7.0.3300      |  Remove     | No supported version found          |

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### Greenshot.Base modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot.Editor modifications  

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot.Test modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot.Plugin.Win10 modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot.Plugin.Photobucket modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot.Plugin.Office modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

NuGet packages changes:
- MicrosoftOfficeCore should be removed (*no supported version found*)
- Unofficial.Microsoft.mshtml should be removed (*no supported version found*)

#### Greenshot.Plugin.GooglePhotos modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot.Plugin.Flickr modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot.Plugin.Dropbox modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot.Plugin.Box modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot.Plugin.Jira modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot.Plugin.Imgur modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot.Plugin.ExternalCommand modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`

#### Greenshot modifications

Project properties changes:
- Target framework should be changed from `net472` to `net9.0-windows`