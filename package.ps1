#!/usr/bin/env pwsh
# Package MoveThisHere for local distribution.
# Run from the repository root: .\package.ps1

$ErrorActionPreference = "Stop"

$project = "MoveThisHere.csproj"
$modName = "MoveThisHere"
$distRoot = "dist"
$distMod = "$distRoot\$modName"

# Build Release
Write-Host "Building Release..." -ForegroundColor Cyan
dotnet build $project -c Release | Out-Host
if ($LASTEXITCODE -ne 0) {
    throw "Build failed."
}

# Clean and recreate dist folder
Write-Host "Packaging $modName..." -ForegroundColor Cyan
if (Test-Path $distMod) {
    Remove-Item -Recurse -Force $distMod
}
New-Item -ItemType Directory -Path "$distMod\anim\assets\haulingpoint" -Force | Out-Null
New-Item -ItemType Directory -Path "$distMod\locales" -Force | Out-Null

# Copy runtime content
Copy-Item -Path "bin\Release\$modName.dll" -Destination $distMod -Force
Copy-Item -Path "bin\Release\locales\*.po" -Destination "$distMod\locales" -Force
Copy-Item -Path "bin\Release\anim\assets\haulingpoint\*" -Destination "$distMod\anim\assets\haulingpoint" -Force

# Copy mod metadata
Copy-Item -Path "mod.yaml" -Destination $distMod -Force
Copy-Item -Path "mod_info.yaml" -Destination $distMod -Force

Write-Host "Packaged to $distMod" -ForegroundColor Green
Write-Host "Install by copying '$distMod' to '%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Local\'" -ForegroundColor Yellow
