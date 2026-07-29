# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test

```bash
# Build the mod DLL
dotnet build MoveThisHere.csproj -c Release

# Build with debug symbols
dotnet build MoveThisHere.csproj -c Debug

# Clean build artifacts
dotnet clean
```

The project targets .NET Framework 4.8 and compiles to `MoveThisHere.dll`. It requires the game's managed DLLs as references (`Assembly-CSharp.dll`, `0Harmony.dll`, `UnityEngine.dll`, `UnityEngine.CoreModule.dll`). These are resolved via hardcoded paths in the `.csproj` — update the `HintPath` elements if your ONI installation is elsewhere.

There are no unit tests in this project — testing is done by loading the mod in-game.

## Project Structure

```
MoveThisHere.csproj    — .NET Framework 4.8 class library; references ONI game DLLs
MoveThisHere.sln       — Visual Studio solution file
MoveThisHere_Patch.cs  — Entry point + all Harmony patches + utility helpers
HaulingPointConfig.cs  — Building definition (size, materials, layers, storage config)
HaulingPoint.cs        — Building behavior (storage logic, user menu, auto-deconstruct, custom FilteredStorage)
STRINGS.cs             — LocString definitions for UI text
locales/en.po          — English translations
locales/zh.po          — Chinese translations
anim/                  — Sprites and animation data for the building
```

## Architecture

### Entry Point & Patches (`MoveThisHere_Patch.cs`)

`MoveThisHere_Patch` extends `UserMod2` (Klei's base mod class). Patches are nested classes with `[HarmonyPatch]` attributes:

| Patch | Game class | Purpose |
|---|---|---|
| `Localization_Initialize_Patch` | `Localization.Initialize` | Load `.po` localization files from `locales/` dir |
| `GeneratedBuildings_LoadGeneratedBuildings_Patch` | `GeneratedBuildings.LoadGeneratedBuildings` | Inject HaulingPoint into the build menu (Base > Storage) |
| `ProductInfoScreen_SetMaterials_Patch` | `ProductInfoScreen.SetMaterials` | Hide material selector since building costs nothing |
| `ResourceRemainingDisplayScreen_Patch` | `ResourceRemainingDisplayScreen.GetString` | Show "No resources required" instead of resource count |
| `BuildingDef_Instantiate_Patch` | `BuildingDef.Instantiate` | Force Diamond as the building's primary element; skip resource cost |

`Utils` class provides `AddBuildingStrings()` and `AddPlan()` helpers for registering new buildings.

### Building Config (`HaulingPointConfig.cs`)

`HaulingPointConfig` extends `IBuildingConfig`. Key properties:
- 1×1 tile, placeable anywhere (`BuildLocationRule.Anywhere`), on the `Canvases` layer (renders above buildings, clicks through to it)
- Uses `haulingpoint_kanim` for animation
- Indestructible (Invincible, flood-proof, overheat-proof)
- Stores only liquids + gases (`STORAGEFILTERS.LIQUIDS` + `STORAGEFILTERS.GASES`)
- Removes vanilla `Reconstructable` and `Deconstructable`; adds custom `DeconstructableHaulingPoint`
- Max capacity defaults to 20,000 kg

### Building Logic (`HaulingPoint.cs`)

Three classes in one file:

1. **`HaulingPoint`** — Core behavior component attached to the building:
   - Implements `ISim1000ms`, `ISingleSliderControl`
   - Serializes: `allowManualPumpingStationFetching`, `userMaxCapacity`, `willSelfDestruct`, `willSpill`
   - Custom capacity slider (grams precision, rounds to kg above 100 kg)
   - Right-click menu: Auto-Bottle, Auto-Drop (self-destruct at 99% full), Auto-Spill toggles
   - `Sim1000ms` checks if storage ≥ 99% and triggers auto-deconstruct when enabled
   - Forces Diamond as the primary element on spawn

2. **`DeconstructableHaulingPoint`** — Replaces vanilla `Deconstructable`:
   - Instant deconstruction via "Remove" button (no duplicant required)
   - No building material refund
   - Drops contents according to `willSpill` setting (`Storage.DropAll(spill, spill)`)

3. **`FilteredStorageHaulingPoint`** — Fork of vanilla `FilteredStorage`:
   - Custom forbidden tags handling (needed because vanilla's `forbiddenTags` field was private)
   - Controls fetch chores, meters, and storage filtering

### Localization

- `STRINGS.cs` defines all `LocString` constants in code
- `.po` files in `locales/` are loaded at runtime based on game locale
- English (`en.po`) and Chinese (`zh.po`) are bundled; others fall back to the `STRINGS.cs` defaults
- The localization patch registers `STRINGS` and loads matching `.po` files

## Key Conventions

- Namespace: `MoveThisHere`
- Harmony patches use nested class pattern inside the main mod class
- Building ID string: `"HaulingPoint"` (constant in `HaulingPointConfig.Id`)
- Animated assets use Klei's `.bytes` format (built with Spriter or similar tools)
- All user-facing strings use `LocString` with `FormatAsLink()` for in-game hyperlinks
