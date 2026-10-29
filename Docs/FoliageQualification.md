# Advanced Foliage qualification — 2026-10-28

Baseline: `d4043938372a3826cafc64f33d0cfe9bff7caf37`, main. Editor: Unity 6000.3.27f1; locked URP 17.3.0. Scope: a small realtime Sandbox subset. Furniture, Town, POC, production, and final mansion grounds were not developed in this tranche.

## Source and support

The Asset Store package is **Advanced Foliage Pack 2.0**, publisher NatureManufacture, cached **version 3.1.2** (product 103151, upload 880618). The existing import was reconfirmed against the base archive: all 506 asset payloads identical, no missing archive paths, all 545 metadata GUIDs preserved. Initial vendor directory: 1,050 files / 1,901,380,168 bytes. Unity-added metadata differs from archive metadata without changing identity.

Base archive: `C:/Users/josep/AppData/Roaming/Unity/Asset Store-5.x/NatureManufacture/3D ModelsEnvironments/Advanced Foliage Pack 20.unitypackage`.

SHA-256: `323547b50055850e524476042d551dcd7ba897ad5d3cca817097e0abd85757e9`.

Applied only the bundled `Advanced Foliage Pack/HD and URP Support Packs Foliage/URP 17.3 Unity 6.3 Advanced Foliage Pack.unitypackage` under the vendor root. SHA-256: `cd40ccafdbb02d3609ad2b9297245b040b24a5d69e91fbfb66a40566cb971e06`. Its 295 paths all remain inside `Assets/NatureManufacture Assets/`. It replaces materials, prefabs, models, shaders and wind support; it is not a project-wide converter. No package manifest or global render setting change was needed. The included diffusion-profile instructions explicitly target HDRP and do not apply to this URP project.

HDRP support, optional Post Process Stack and Vegetation Studio packages were not imported. Static archive inspection found one path in Post Process Stack support and 124 in Vegetation Studio support, with no C# source in either support archive. These are integration payloads, not proof their external systems are installed or compatible. Exact optional archive hashes are in AssetSources.json.

## Pack inspection

Vendor roots cover Advanced Grass, Crooked Roots, Trunks, Unity grass (mesh detail prefabs and billboard textures), Ground Textures, Demo, foliage/object shaders, snow textures and NatureManufacture Wind. This pack has no leafy tree or formal hedge prefabs: its woody content is roots and cut trunks. A tree/hedge qualification cannot be inferred from these assets.

After overlay, the loaded inventory contains **114 prefabs**, all without missing scripts; 60 have an LODGroup, 54 do not. Six prefabs have one collider each; 108 have none. These counts include snow variants and the overlay's extra trunk prefabs. Full prefab renderer bounds, shadow flags, LOD thresholds and material shader names are in FoliagePackAudit.txt.

Of 71 loaded materials, 26 use URP/Lit, 18 use the custom URP Foliage/Foliage Snow shaders, seven use custom URP Top Cover, one uses URP Terrain/Lit, 18 retain legacy Advanced Grass Specular/Snow, and one retains Standard. All report supported shaders on this editor, but that flag alone does not prove URP render compatibility. Only the selected URP subset was visually qualified. In particular, do not promote the Unity grass legacy materials based on this sample.

The custom URP shaders include the existing URP/Core package libraries and provide forward, GBuffer, shadow, depth and motion passes. The selected shaders need no separately installed shader editor. No DLL, custom Editor script, automatic project-setting mutation, or additional runtime system was found in the imported base/URP support. Optional integration packages remain separate.

`NM_Wind.cs` is the sole vendor C# source. ExecuteInEditMode plus Start/Update/OnValidate write global `WIND_SETTINGS_*` textures, direction/speed, turbulence, gust scales, four optional spherical WindZone positions and radii. It converts km/h using 0.2777. One selected controller supplies 3DNoise.psd and GustNoise.psd; observed global direction/speed was (0,0,1,5.55), matching its 20 km/h preset. Multiple active controllers can overwrite each other's globals. It has no OnDisable cleanup, so global values can remain until replaced; treat wind as shared scene state. No spherical touch zones were needed or qualified.

The two vendor demos were inspected as serialized scenes and their dependencies/terrain data were loaded for audit; they were not used as qualification render scenes. Overview contains approximately 6,040 prefab instances and three reflection probes. Demo with Unity Grass contains a 100 x 150 x 100 metre terrain, heightmap resolution 257, 12 terrain layers, zero terrain tree instances and nine detail prototypes: seven instanced VertexLit mesh prototypes, one Grass texture and one GrassBillboard texture. Terrain painting, billboard transitions and legacy detail-shader rendering remain unqualified in URP. Neither demo, its terrain, lighting data nor reflection maps is in the selected subset.

## Owned Sandbox patch

`Assets/Inheritance/Environment/Sandbox/Foliage_Qualification_Patch.prefab` is an owned outer assembly of nested vendor prefabs placed at (37,0,2) in `Prototype_Sandbox` only:

- Prefab_trunk_01: cut trunk, four LODs, MeshCollider, LOD0 3,527 triangles.
- prefab_crooked_root_01: four LODs, no collider, LOD0 856 triangles.
- prefab_SM_forest_heather_01: small flowering shrub, one LOD, 16 triangles.
- prefab_SM_grass_bush_01: two LODs, 29/13 triangles.
- prefab_SM_clover_01_flowery: two LODs, 74/16 triangles.
- prefab_SM_fern_01_0: three LODs, 144/59/20 triangles.
- Prefab_Wind: one wind controller with its two noise textures.

All six mesh families have UV2 data; this does not certify UV packing or baked-light quality. Whole-sample LOD0 geometry is 4,646 triangles before shadows/other passes. Unit scales are retained. The existing ground surface is y=-0.5; per-child height overrides put renderer bounds minima at -0.51 to avoid floating placement. Root geometry is irregular and raised portions remain visible above its ground-contact points. Ground cover is deliberately tiny relative to the four-metre Mansion modules, not scaled into hedges. This is a spaced inspection patch, not landscaping.

Reviewed multiple front, rear and close render views beside the Mansion sample, including time-separated wind renders. No pink materials, broken active shader references or gross wind deformation appeared. Wind visibly changes the grass/fern silhouette. The existing directional light disables shadows; temporary captures enabled soft shadows and restored its setting without saving that lighting change. Trunk, vegetation and architecture cast plausible shadows together. This is a small-sample visual check, not a moving-camera, dense-field, GPU timing, standalone build or navigation-bake certification.

Only the trunk blocks physics. Root/shrub/grass/fern/clover do not block a character; do not assume root collision from appearance. No navigation surface, obstacle, carving or terrain collider policy was added. Before promotion, test the intended walking route, trunk collision detail, wind bounds, alpha overdraw, shadow distance, LOD popping and density on target hardware. Leaves/cards cost more than triangle counts imply, especially in shadow/depth passes. No project shadow budget was enlarged. Baked lighting remains unqualified.

**Art direction:** the photogrammetry-like mossy stump has more surface detail/contrast than the Mansion sample's simpler architecture. Technical interoperability passes for this subset; a coherent production look still needs a style/material review. Use owned material variants for future artistic tuning. No artistic reskin was attempted here.

## Selective source-control decision

Accept a selected, GUID-preserving closure, not the full 1.90 GB import. Unity recursive dependencies were checked with unselected content physically outside Assets, then supplemented with parent metadata and vendor notices. Serialized GUID scanning resolves all remaining links against selected Assets or the locked URP package (Lit shader and AssetVersion editor metadata). Shader include paths resolve inside locked packages. Normal checkout needs **Git LFS**, not the vendor cache.

Two narrow, recorded normalization exceptions are required after the vendor overlay:

1. The six selected FBX importers use **Calculate Mikk** tangents. Vendor import-tangent settings produced missing tangent/binormal warnings. Mesh payloads and GUIDs are unchanged.
2. Unity's serializer removed obsolete texture slots absent from the six selected materials' current shaders. Four foliage materials contained dead `<noninit>` and `_WindNoise` references to assets missing even from the full vendor import. Keeping those serialized links would undermine a clean-reference claim. Active shader properties were preserved. FoliageMaterialNormalization.txt records every removed slot; FoliageReimportNormalization.cs.txt reproduces the operation and importer settings. This is explicit pipeline cleanup, not artistic material editing.

The resulting closure has **103 vendor files including metadata, 227,115,099 bytes**: six FBX, seven prefabs (including wind), six materials, 14 PNG, four TGA, two PSD, two shaders, one C# source, vendor notice and metadata. Existing .gitattributes covers every selected binary; no TIFF or broad binary-asset rule is needed. **26 selected LFS objects total 225,403,273 bytes** before server deduplication. Shader source and YAML remain ordinary Git. The remaining roughly 1.71 MB of vendor data is mostly shader source and metadata, before Git compression. The full staged tranche changes 115 files (112 additions, three modifications), with approximately 1.96 MB of ordinary Git blobs plus the LFS payload. Vendor shader/metadata whitespace is preserved; the owned documentation passes the staged whitespace check. Exact selected paths, sizes and SHA-256 values are in FoliageSelectedFiles.txt and FoliageSelectedManifest.json.

The complete inspected import is preserved at ignored `Temp/FoliageUnselected/Assets/NatureManufacture Assets/`; the additional snow dependency made unused by material cleanup is preserved in `Temp/FoliagePruned/`. Temp is disposable and is not the durable restore source. Original base/support archives and recorded hashes supply reproducibility. No full vendor archive, optional integration, demo, terrain asset, lightmap or unused legacy shader is staged.

## Recreate on the other laptop

For the committed sample: clone main with Git LFS installed, run `git lfs pull`, open in Unity 6000.3.27f1, let the locked packages restore, then open Prototype_Sandbox. No Asset Store import is necessary. Do not overwrite selected assets with the base built-in-renderer import alone.

For licensed full-pack reinspection: retrieve the exact base archive above from the Asset Store cache or private backup, verify SHA-256, import at original paths, then import the exact bundled URP overlay and verify its hash. Keep the existing URP 17.3.0 lock; skip HDRP and optional integrations. Copy FoliageReimportNormalization.cs.txt temporarily into Assets/Editor as a .cs file and run Tools > Inheritance > Normalize Selected Foliage, then remove the temporary script. Preserve original .meta GUIDs. Use the selected manifest to retain only the committed closure if returning to the qualified minimal state. For exact committed normalization bytes, restore the selected files from Git rather than regenerating YAML on a different editor version. Repeat dependency, shader, visual, LFS and regression checks for every expansion.

## Verification

- Final normalized selected setup compiled; **60/60 EditMode tests passed, 0 failed/skipped**. NUnit evidence: FoliageEditModeTests.xml.
- Console after that run: **0 errors, 0 warnings**, four informational test-run messages. Historical full-import tangent warnings and temporary helper reload/file-sharing errors were excluded only after correcting the selected import settings and completing a clean rerun; they are not shipped runtime code.
- Temporary automation was removed before commit; Unity recompiled and the final Editor Console was inspected again: 0 errors, 0 warnings, 0 informational entries.
- POC bytes equal baseline HEAD exactly; SHA-256 `ad62afa9fcfe923c6f6637c2e0e22b708b7b4d6e39fdd0a256d8f85962460a4a`.
- Production Mansion bytes equal baseline HEAD exactly; SHA-256 `8fa6d5996d5feaa8311d8a86a1d456f5b91cba7792e2ec0dad0ebc5955c3df41`.
- Package manifests, build scenes, gameplay source, Mansion vendor subset and global project settings have no semantic diff. Sandbox scene diff adds only the owned foliage prefab instance and scene-root entry.

Promote only selected assets after resolving art direction and the production lighting/navigation/density checks above. Trees, formal hedges, full terrain integration, snow variants and optional systems need their own validation. Furniture and Town remain deferred.
