# SmartCardReader

Read a `SmartCard` using `PowerShell`

## Build Instructions

Build using

```
dotnet publish --configuration Release
```

Install by copying contents of `bin/Release/netstandard2.0/publish` into a new directory on the [PSModulePath](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_psmodulepath)

## Command Set

The module provides the following commands.

```
Open-SmartCardReader

Close-SmartCardReader [-Reader] <Reader>

Get-SmartCardReader [-Reader] <Reader>

Connect-SmartCardReader [-Reader] <Reader> [-Name] <string>

Disconnect-SmartCardReader [-Reader] <Reader>

Invoke-SmartCardReader [-Reader] <Reader> [-Command] <byte[]>
```

## Test

Test with [test.ps1](test.ps1).
