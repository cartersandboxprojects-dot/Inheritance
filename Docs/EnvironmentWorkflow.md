# Environment workflow

## Three lanes

- `Assets/Scenes/Prototype/Prototype_Mansion_POC.unity`: frozen regression/reference proof of concept. Renamed/moved from `Assets/Prototype_Mansion.unity`, preserving its GUID and original scene contents. Do not use it for ongoing construction; use it to reproduce the proven movement, Ink, consequence and save/load loop.
- `Assets/Scenes/Prototype/Prototype_Sandbox.unity`: disposable experimental integration/build area. Starts with an independent copy of the POC, including its playable cast and hookups, plus a 200 x 200 metre collider ground and an Assembly Workspace at x=30. The source scene had a graybox room, not a terrain asset. This ground supplies room for future buildings, vehicles, navigation, lighting, shaders and package trials. No new systems are installed.
- `Assets/Scenes/Game/Mansion.unity`: comparatively sterile production shell, initially retaining only graybox room geometry, camera and directional light. No prototype cast, player controller or funeral-night runner. This is the sole enabled build scene; open the POC directly to run the prototype. The existing SampleScene remains untouched as template material.

POC and Sandbox are development scenes, excluded from the production build list. The production shell is intentionally not a playable narrative milestone yet.

## One physical world, changing state

Reuse the production Mansion for day/time/story variation. Future GameState and content activation should control character presence, dialogue, access, props and lighting. Do not create Mansion_Day1, Mansion_Day2 or chapter copies. Separate scenes are justified for distinct locations or genuine physical/loading partitions. This tranche documents that direction; it adds no state activation system.

## Build in Sandbox, promote deliberately

Major third-party systems/assets must prove themselves in Sandbox before promotion to production. Check dependencies, project settings, render/input compatibility, references and performance. A scene boundary does not isolate project-wide package or shader/settings changes: evaluate risky packages in a disposable project/branch first, then integrate deliberately.

Work may assemble modular buildings and other reusable environment assemblies in Sandbox. When useful, save candidates under `Assets/Prefabs/Sandbox/` and approved environment prefabs under `Assets/Prefabs/Game/Environment/`; create these folders only when there is actual content. Promote via Unity's asset tools with GUIDs/dependencies preserved, then place the approved prefab in Mansion. Keep layout/pivots reusable to minimize rework. Do not copy entire experimental scenes into production. No prefab pipeline or new assets beyond the scene ground are required now.

Independent scene copies still share referenced scripts/materials/Ink assets. Freezing the POC means avoiding intentional scene edits and using Git milestone history when shared dependencies change; it is not an immutable copy of every dependency.

## Historical asset ingestion preflight - 2026-10-28 (superseded by qualification below)

Repository HEAD verified: `5aa44e8cc4d967f416cf2c59bffd35703ecc0b7e`.
Editor: Unity `6000.3.27f1`; project URP dependency: `17.3.0`.
The starting working tree was NOT clean: untracked `Assets/NatureManufacture Assets/` and its root meta already existed. These were preserved; this inspection did not import or remove vendor assets.

### Lightweight ownership and promotion rules

- Preserve vendor paths and GUIDs; do not rearrange vendor originals simply to standardize folder names. Record each vendor root and source version.
- Keep Inheritance-owned modified materials and prefab variants/assemblies under `Assets/Inheritance/Environment/Sandbox/`. Keep promoted equivalents under `Assets/Inheritance/Environment/Production/`. Create folders only when needed. This ownership convention supersedes the earlier generic prefab folder suggestion for new environment work.
- Experiment only in `Assets/Scenes/Prototype/Prototype_Sandbox.unity`. Do not edit the frozen POC or production scene during qualification.
- Use narrowly scoped vendor URP support or owned material copies. Never blanket-convert project materials. Review bundled scripts, shaders, settings and dependency conflicts before enabling them; a scene boundary does not isolate those changes.
- Promote only after checking render correctness, missing references/scripts, dimensions/pivots, colliders, LODs and dependencies in Unity. Record sample names and test results. No pack below is yet qualified for production.

### Confirmed local sources

All four sources are legacy Asset Store `.unitypackage` archives, not UPM packages. Metadata versions below were read directly from their gzip Asset Store headers; they are cached versions, not a claim about latest store releases or current account entitlement.
Cache root: `C:/Users/josep/AppData/Roaming/Unity/Asset Store-5.x/`.

| Pack | Cached version | Cache-relative archive | Compressed bytes | Expanded archive file bytes |
| --- | --- | --- | ---: | ---: |
| Mansion Pack | 1.1 | `Greenworks/3D ModelsEnvironmentsUrban/Mansion Pack.unitypackage` | 1095715372 | 1176020477 |
| Advanced Foliage Pack 2.0 | 3.1.2 | `NatureManufacture/3D ModelsEnvironments/Advanced Foliage Pack 20.unitypackage` | 1615028931 | 1910037420 |
| Town Constructor 3 | 1.0 | `Purple Jump/3D ModelsEnvironmentsUrban/Town Constructor 3.unitypackage` | 668535470 | 2418382407 |
| Furniture Mega Pack - Free | 1.0 | `dlgames/3D ModelsPropsFurniture/Furniture Mega Pack - Free.unitypackage` | 1807679361 | 1835981875 |

Expanded totals include archive metadata/previews and embedded support archives, exclude generated Library data, and are not an exact Git/LFS upload estimate. Only these four archives were found under this cache root; other cache locations/account downloads were not exhaustively searched.

### Preliminary findings (archive/static inspection only)

- **Mansion:** vendor root `Assets/Mansion/`; 645 prefab entries, 363 FBX entries, three scene entries, two nested support packages, documentation PDF, one C# entry, animation/controller assets and audio. Archive authored with Unity 2021.3.45f2. Actual architectural module taxonomy, interior coverage, DemoScene usefulness, shader support and script behavior still require inspection; counts alone do not establish modularity.
- **Foliage:** vendor root `Assets/NatureManufacture Assets/`; cached 3.1.2 authored with Unity 6000.3.9f1. Existing local tree: 1050 files, 1901380168 bytes, including 112 prefabs, 61 FBXs, 70 materials, ten shaders, five shader includes, terrain layers and two scenes. Includes `URP 17.3 Unity 6.3 Advanced Foliage Pack.unitypackage` and an HDRP counterpart. Optional Post Process Stack and Vegetation Studio prefab packages are also embedded. `NM_Wind.cs` is an ExecuteInEditMode component that updates shader wind settings. Presence of matching URP support is confirmed; application of that support and resulting materials are unverified. Do not install optional integrations without need.
- **Town:** vendor root `Assets/Town Constructor 3/`; authored with Unity 2018.3.0f2. Archive contains 526 FBX entries, 288 prefab entries, three scenes, 79 C# entries and a substantial `PostProcessing/` tree with shaders/compute shaders. This old bundled system needs conflict/API review before import. Package metadata advertises modular parts, premade buildings and low-resolution doubles, but actual LODGroup configuration is unverified. Material conversion requirements remain unverified. Includes 44 TIFF entries.
- **Furniture:** vendor root `Assets/Furniture Mega Pack/`; authored with Unity 6000.0.23f1. Archive contains 511 prefab entries, 395 FBX entries, 155 materials, 459 PNG entries and one scene. No C# or DLL entries were found in the archive pathname inventory. Style range, as-is prefab cleanliness, scale, colliders, material slots and variant suitability remain unverified.

### Historical repository stop decision

STOP before committing vendor content: sources total 5186959134 bytes compressed, with 7340422179 bytes of expanded archive files, before Unity Library generation. Committing all four wholesale needs an explicit storage/budget decision. Existing LFS handles FBX, PNG, TGA, PSD, EXR, audio and unitypackage formats, but not `.tif`/`.tiff`. No LFS rules were changed. Binary `.asset` content must be assessed individually; do not blanket-LFS YAML assets.

Recommendation: retain complete original archives in private artifact storage, with a manifest of asset ID, version, archive SHA-256, provenance and retrieval instructions. Qualify in a disposable Unity project first; commit only selected usable prefabs and their complete dependencies with preserved GUID/meta files through Git LFS. Add narrow TIFF coverage if those textures are selected, and verify hosted LFS storage/bandwidth capacity before pushing. Keep unnecessary demo lightmaps, embedded render-pipeline archives and legacy PostProcessing code out of the selected runtime subset. Do not ignore required dependencies: a fresh checkout must reconstruct successfully. A private separately versioned vendor content repository is an alternative if full pack availability is required.

No new Sandbox sample layout, material conversion, Unity visual/compile qualification or EditMode test run was performed in this paused tranche. Prior 60/60 results are historical and were not reverified. No scenes, project settings, package manifest or vendor files were changed by this inspection. Editor Console status was not independently verified. No commit or push was performed.

## Accepted storage convention - Mansion qualification (2026-10-28)

- Raw vendor `.unitypackage` archives stay outside normal Git history in the Unity Asset Store cache, with optional private external/cloud backup. Do not stage imported copies of embedded support archives.
- Keep required selected vendor source under its original `Assets/Mansion/` paths, preserving GUIDs and `.meta` files. Vendor originals are not edited by hand; applying the vendor's matching URP support overlay is an explicit, recorded exception. Keep original archive hashes so the overlay is reproducible.
- Inheritance-owned assemblies, prefab variants and material overrides belong under `Assets/Inheritance/Environment/Sandbox/`; production promotion is separate.
- Commit the selected sample's recursive dependencies and parent folder metadata, not the entire import. A fresh LFS-enabled checkout must load the sample without the archive/cache. Keep demo scenes, unused meshes/textures, generated demo lighting and nested support archives out of this tranche unless required by that dependency closure.
- Required FBX, PNG and WAV binaries are already covered by `.gitattributes`; retain YAML scenes, prefabs, materials and metadata as normal Git text. Inspect binary `.asset` files individually. Mansion 1.1 has no TIFF files. Do not add speculative TIFF rules now: add vendor-scoped `*.tif`/`*.tiff` rules when a TIFF-using pack is actually qualified.
- Record package name, publisher, exact cached version, local/Asset Store source and archive SHA-256 in `Docs/AssetSources.json`. SHA-256 identifies bytes, not entitlement; only licensed collaborators may restore proprietary archives.
- Pre-existing NatureManufacture content remains untouched and untracked; do not stage it as part of Mansion qualification.


Mansion qualification is complete for the selected realtime Sandbox subset. See [MansionQualification.md](MansionQualification.md) for findings, limits, tests and repository impact; [AssetSources.json](AssetSources.json) records exact archive provenance and [MansionSelectedFiles.txt](MansionSelectedFiles.txt) records selected vendor files. Future additions must repeat dependency and LFS impact review. Foliage, Town and Furniture remain unqualified.
