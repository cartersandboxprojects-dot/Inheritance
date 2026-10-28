# Mansion Pack qualification - 2026-10-28

Baseline: `5aa44e8cc4d967f416cf2c59bffd35703ecc0b7e`, main. Unity 6000.3.27f1, URP 17.3.0. This qualifies a small realtime architectural subset for Sandbox experimentation; it is not production-mansion assembly or a whole-pack performance certification.

## Pre-existing NatureManufacture content

`C:/Users/josep/Inheritance/Assets/NatureManufacture Assets/` contains 1,050 files totaling 1,901,380,168 bytes (1.90 GB decimal), plus its root `.meta` outside that directory. It was already untracked at the start.

Compared against cached **Advanced Foliage Pack 2.0, version 3.1.2**, from `NatureManufacture/3D ModelsEnvironments/Advanced Foliage Pack 20.unitypackage` (SHA-256 `323547b50055850e524476042d551dcd7ba897ad5d3cca817097e0abd85757e9`): no missing archive paths, all 506 asset payloads identical, all 545 metadata GUIDs preserved. The 506 differing metadata files include Unity-added `AssetOrigin` records identifying product 103151, package 3.1.2, upload 880618. Unity Package Manager reports the package In Project.

This is a complete base-archive import, not evidence of a truncated payload import. It does not establish application of the optional URP support archive or render qualification. Keep it untouched and untracked for a separate Foliage tranche. Do not delete or include it in the Mansion commit. Existing Foliage FBX tangent/binormal warnings were present before Mansion work.

## Source, import, and storage

Imported only **Greenworks Mansion Pack 1.1** from:

`C:/Users/josep/AppData/Roaming/Unity/Asset Store-5.x/Greenworks/3D ModelsEnvironmentsUrban/Mansion Pack.unitypackage`

Original archive SHA-256: `d4ae5ad40d50d6817ae293c18e54b091dad200a04404b1765c1204200e5b1d0b`.

Imported to its original `Assets/Mansion/` paths, then applied its bundled `Assets/Mansion/Assets/Mansion-URP.unitypackage` overlay. This is the documented vendor render-pipeline setup, not a project-wide material conversion. The original and overlay hashes and restore instructions are in `AssetSources.json`. No Town, Furniture, Foliage support, HDRP, or unrelated integrations were imported.

Full archives stay in the Asset Store cache, with optional private cloud/external backup. Existing `.gitignore` excludes `.unitypackage` and associated metadata. Vendor paths/GUIDs remain identifiable. Owned changes live under `Assets/Inheritance/Environment/Sandbox/` and are saved in the outer assembly, not the vendor originals.

The full inspection import occupied about 1.16 GB under Assets. After inspection, unused newly imported Mansion files were moved to ignored `Temp/MansionUnselected/Assets/Mansion/`, preserving their paths. Temp is disposable; the original cache archive plus recorded overlay are the durable restore source. The final Assets tree contains only the selected dependency closure. `MansionSelectedFiles.txt` lists the 89 vendor files including root/folder metadata. A normal checkout needs Git LFS and the existing package manifest, not any vendor cache or NatureManufacture content, to resolve the selected assembly.

## Construction and prefab findings

The documentation specifies a 1-unit world grid and integer local placement. Module suffixes describe dimensions/styles; for example FloorTile4 covers four square units (2 x 2 m). Root transforms are generally origin zero with unit scale, but mesh geometry can be offset from that pivot. Use the matching module origin rather than centering objects by renderer bounds.

The base archive contains 645 prefabs and 363 FBX files; the URP import exposes 646 prefabs. Categories include exterior walls/corners, wall bases/floorlines/rooflines, roofs, window frames/windows, balconies and fences; interior walls, corner fillers, door linings, ceilings, floors, columns and stairs; plus furniture and garden decorations. Door and window pieces commonly nest at local zero under matching walls. The docs distinguish full corner modules from small fillers and explain when doorway thresholds should be disabled to avoid overlap with tiles.

All 646 loaded prefabs were audited for hierarchy, renderer bounds, scale, colliders, LOD groups and missing scripts. No missing scripts were found. The selected exterior wall/corner modules are 4 m high; interior linings are 3.8 m high. Floors and ceilings are single-sided planes, not solid slabs. Selected architectural pieces have colliders but no LOD groups. Pack-wide collider and LOD coverage varies; do not infer either from a prefab name or assume decorative trim blocks movement. The static base-archive inventory contains 35 LODGroup declarations.

All three demos opened without missing scripts. DemoScene contains many assembled examples and camera animation data; a direct first-camera capture at edit-time produced sky, so that camera is not a useful static overview without positioning/animation. DemoScene2 rendered a complete terrain-backed mansion. Presentation rendered the modular component catalogue. Their many punctual lights exceed the project's 2048 shadow-atlas budget: Unity reduced shadow resolutions and removed shadow maps in the demos. Those demos and lights are excluded from the selected subset. No project lighting settings were changed to accommodate them.

## Rendering, UVs, scripts, dependencies

After the vendor URP overlay, 126 loaded materials use URP/Lit and one uses URP/Simple Lit; all report supported shaders in Unity 6.3. No custom shader conversion or new package was needed for the sample. The selected closure references the existing URP package, seven vendor materials, seven FBX files, seven vendor prefabs, and fifteen PNG textures. Some extra material/texture references originate in FBX importer remaps; those serialized dependencies are retained.

The selected meshes have no UV2 channel and secondary-UV generation is disabled in their importers, despite static/GI flags on prefabs. Realtime rendering is qualified; baked lighting is not. Before production baking, choose and test generated or authored UV2 via a recorded importer override. High-detail statues enable generated UVs and are expensive to import (Aquarius alone took 158 seconds); they and their large textures are excluded.

The only base-archive C# file is `OpenCloseDoor.cs`, a runtime trigger/Animator script used by eight audited door prefabs. It assumes a Player tag, a valid Animator/open parameter, and sufficient audio clips when an AudioSource is assigned; it does not validate those assumptions. No editor scripts or DLLs were present in the base archive. The minimal open doorway requires none of that script, animation, audio or trigger behavior, so those files are excluded. Animated-door integration remains a separate check.

## Owned sample and verification

`Assets/Inheritance/Environment/Sandbox/Mansion_Qualification_Corner.prefab` is an owned assembly retaining nested vendor prefab links. It is placed at (30, 0, 0) in `Prototype_Sandbox` only:

- one exterior corner and matching interior corner;
- one 4 m arched doorway wall with exterior frame and interior lining;
- three adjoining 2 x 2 m floor tiles and one partial ceiling;
- a small owned plinth meeting the Sandbox ground below the sample.

The interior doorway lining needs a 180-degree orientation relative to the exterior wall. That override and disabling its duplicate floor threshold are recorded inside the owned assembly. No vendor prefab was edited by hand. Open ends and the partial ceiling are deliberate inspection access, not a completed room.

Reviewed four camera angles, including close interior and exterior views, then re-rendered after reducing the dependency set and running tests. No pink materials, accidental gaps at the selected joins, floating structural pieces, duplicate threshold z-fighting, or scale mismatches were visible. Floor/wall/ceiling seams align on the local grid. This is visual sample qualification, not a claim about every pack module.

Physics checks after assembly: downward rays hit each of the three FloorTile4A colliders at y=0; an exterior wall ray is blocked; the ceiling is hit at y=3.8; a 2 m capsule with radius 0.5 m crosses the open doorway without collision. Physical keyboard traversal and standalone builds were not run.

Unity compiled successfully. After excluding demo content, Console verification returned **0 errors, 0 warnings**; this clean state persisted through the selected sample render and the **60/60 passing EditMode tests** (0 failed/skipped). Temporary editor automation was removed and Unity recompiled afterward. A transient file-sharing exception in the temporary command helper was corrected by atomic command-file publication; it is not a shipped script or asset error. Full demo shadow warnings and pre-existing Foliage warnings are recorded above rather than treated as a clean full-pack result.

The frozen `Prototype_Mansion_POC`, production `Assets/Scenes/Game/Mansion.unity`, package manifest, build scenes and gameplay scripts are unchanged.

## Repository impact before commit

The selected source assets add 94 files, 57,301,765 working-tree bytes (about 57.3 MB), including folder metadata and the owned assembly. Together with the source manifest, selected-file list and this report, the tranche adds 97 files and modifies only Sandbox and EnvironmentWorkflow.

**22 new LFS objects**: all seven selected `.fbx` and fifteen `.png` files, totaling **57,149,280 bytes**. Exact paths appear in `MansionSelectedFiles.txt`; filter this list for `.fbx`/`.png`. Largest selected file: `DoorExt1A.png`, 17,860,769 bytes; its normal map is 13,902,859 bytes. Existing LFS patterns already cover these. Mansion has no TIFF; TIFF coverage remains deferred until a TIFF-using pack is qualified, then add vendor-scoped rules. YAML prefabs/materials/scenes and `.meta` files remain normal Git text. No binary `.asset`, demo lighting, nested archives, statue assets or unrelated vendor files are included.

The 94 new source files contribute approximately 155 KB of uncompressed Git blobs including LFS pointers; with docs and the updated scene the ordinary Git payload remains below 0.3 MB before compression. LFS upload/storage increment is about 57.15 MB before server deduplication. This is reasonable for the selected-LFS strategy and avoids committing the full 1.16 GB inspection import or the pre-existing 1.90 GB Foliage tree. Git/LFS integrity and push results are reported with the final commit in the handoff.
