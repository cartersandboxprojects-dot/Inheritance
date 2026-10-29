# Expandable mansion entry foundation — 2026-10-29

## Intent and scope

A very large central mansion with west/east wings and eventual 3–5 story architectural capacity. Future program includes bedroom/guest suites, kitchen/pantry/service circulation, family living/sitting rooms, formal dining, ballroom/entertaining, library, Brecken's office suite, family/business offices, staff/service spaces, bathrooms, storage/utilities and stair/elevator cores. These are deferred program, not rooms built in this tranche. Additional stories remain an architectural extension convention, not a structural-engineering certification.

Only Prototype_Sandbox receives a scene instance. The frozen POC and Production Mansion are untouched. No furnishings, grounds, final lighting, gameplay or package/settings changes are included.

## Built dimensions and coordinates

Owned asset: `Assets/Inheritance/Environment/Sandbox/Mansion_Entry_Foundation.prefab`, origin zero, unit scale. Sandbox instance: **(-30, 0, -30)**, identity rotation. Coordinates below are assembly-local metres. World west/east correspond to -X/+X; front approach is -Z; rear grounds is +Z.

- Central block: **24 m wide × 12 m deep**, X=-12..12, Z=0..12.
- West/east open stubs: **4 × 12 m each**, X=-16..-12 / 12..16. Combined seed footprint **32 × 12 m = 384 m² per floor**, excluding portico/steps. The stub depth was extended from the first 8 m sketch to keep the cross corridor clear.
- Two stories: finished floors Y=0 and 4; current wall top / next floor datum Y=8. Flat provisional cap top Y=8.16.
- Formal portico: 8 m wide, 3 m deep landing, centered X=0; three 150 mm approach steps reach the landing from the existing ground. Portico entablature spans Z=-3.1..0.1; steps extend to Z=-4.5.
- Center axis: X=0, through steps, portico, open double leaves, entry hall and open rear.
- Front bays at X=-8,-4,0,4,8 in the core and X=±14 in the stubs. Two-metre transition piers at X=±11 join the core to future wings. Both stories repeat the same vertical rhythm.

## Extension conventions

Use a **1 m placement grid**, **4 m principal bays**, **2 × 2 m floor tiles**, and **4 m floor-to-floor spacing**. Geometry subdimensions use 0.2 m/0.4 m conventions and millimetre-scale separation where needed. Preserve vendor pivots: FloorTile4A spans X=-2..0 and Z=0..2 relative to its origin; do not recenter it by bounds. Exterior doorway modules rotate 180 degrees to face -Z. Retain unit scale on vendor modules.

Owned exterior transition/return walls are **0.4 m thick**. Interior guide walls are **0.2 m thick**. Vendor arch walls have their authored ~0.15 m face depth and projecting frame/column geometry; do not assume their decorative outer bounds represent a uniform wall thickness. The owned upper deck is 0.2 m thick with underside Y=3.8, giving **3.8 m clear first-floor height**. Ground foundation top is Y=-0.02, separated from vendor floor planes at Y=0 to avoid coplanar surfaces.

Exactly **two interior walls** stand at X=±4, Z=2..6, Y=0..3.8. They guide an approximately **7.8 m clear entry hall**, preserve the axial view, and stop before the transverse circulation band. They define no finished rooms. The cross corridor reservation is Z=6..10, running through both wing stubs. Short exterior flank returns at X=±12 occupy Z=10..12 and stay out of that band. Space around/behind the hall remains available for future stair/elevator cores; no core location, stair or upper-floor opening is committed. Cut removable deck sections when that design is established.

Extend west/east at **X=-16/+16** using the same Y=0/4 floor datums and bay rhythm. No end walls, terminal corners, end pediments, stairs or roof hips close these planes. Rear Z=12 is intentionally open. Continue floor/deck/face modules rather than demolishing a finished end elevation.

For a third story remove the clearly named **PROVISIONAL removable cap - next floor datum 8m** group. Place the next deck with top Y=8 and repeat walls from that datum. Later floor datums are Y=12 and 16 for four/five stories. The cap contains its own front top trim; do not carry that trim into an intermediate floor without review. The portico has a separately named removable entablature. No pitched roof, attic, permanent parapet or fixed vertical core constrains expansion.

## Ownership and vendor limits

The owned assembly retains **124 nested vendor prefab instances**: 96 floor tiles, 14 arched wall modules and 14 exterior frames. All are already in the qualified Mansion Pack 1.1 subset; no extra vendor assets, archive imports or LFS payloads were needed. Vendor prefabs/materials/importers are unchanged.

Straight infills, decks, caps, guide walls, portico and provisional open door leaves are owned geometry. Five owned materials include simple stone/limestone, opaque glazing and door placeholders, plus a copy of WallExtA with culling disabled. The exterior vendor wall is one-sided: the owned two-sided copy fixes invisible wall backs without editing the original. This remains exterior surface texture seen from inside, not final interior wall finishing. Frame backs are unfinished. Glazing is an opaque rectangular backing masked by the exterior arch; its interior silhouette and the open double leaves are provisional. Doors have no animation, audio or interaction. Door leaves are 2.8 m tall and together imply an approximately 2 m opening; the vendor surround is roughly 4 m wide and 3.9 m high. The 8 m portico supplies estate-scale breadth without scaling vendor meshes.

Floor tiles remain single-sided vendor planes; the upper deck/cap are owned solid primitives. Selected vendor modules have no LOD groups and their meshes lack qualified baked-lighting UV2; realtime inspection is the existing qualification boundary. Broad material/art-direction matching and baked lighting are deferred.

## Verification

Reviewed six final views: centered front, oblique front, open rear, entry hall, transverse hall/wing junction, and upper interior. Confirmed symmetry, aligned story/floor seams, grounded steps/columns, continuous tiled ground floor and deliberate open junctions. Corrected a potential coplanar foundation/floor overlap and invisible vendor wall backs. No pink materials, floating structural modules, unintended overlapping thresholds or visible accidental floor seams were found in these views. Opaque glazing, unfinished backs/upper floors and provisional caps are visible limitations, not finished art.

Physics casts with a **2 m tall, 0.9 m wide capsule** found no assembly blockers from front to rear on X=0 or west to east at Z=8. Floor rays hit Y≈0 in the hall and both stubs and Y=4 upstairs. Exactly two guide walls, zero missing scripts, zero missing material slots. This is automated clearance checking and camera inspection; keyboard traversal, staircase design and standalone build were not performed.

Unity 6000.3.27f1 script recompile reports **up_to_date**, compilationFailed=false. Console ground truth: **0 errors, 0 warnings**. Existing EditMode suite: **121 passed / 121, 0 failed, 0 skipped** (Temp/pipeline_test_status.json). Six camera captures and the clearance report are in ignored Temp/Entry-view-0..5.png and Temp/Entry-verification.txt.

Sandbox diff adds only one prefab instance/root (58 lines); all prior scene objects remain intact. `git diff --exit-code HEAD` confirms POC, Production Mansion, Assets/Mansion, Packages and ProjectSettings have no content changes. URPProjectSettings briefly appeared modified due solely to line endings; its empty diff was verified and normalization cleared the flag without a staged change. No ambiguous serialization remains.

## Human scale validation - 2026-10-29

### Reusable owned references

- `Assets/Inheritance/Environment/Sandbox/Reference/HumanScaleReference_Adult.prefab`: measured renderer height **1.800 m**.
- `Assets/Inheritance/Environment/Sandbox/Reference/HumanScaleReference_Short.prefab`: measured renderer height **1.680 m**.
- Shared owned `HumanScaleReference_Neutral.mat`, using the existing URP/Lit shader. Each prefab has 16 built-in sphere/capsule/cylinder/cube parts with readable adult proportions. The shorter reference uses proportionally smaller geometry, not a scaled root. Both roots are position/rotation zero and scale one; feet meet local Y=0; local +Z is forward. Drag either prefab into a scene at the intended finished-floor elevation.
- These are neutral provisional mannequins, not gender/casting or final character decisions. No external model, new framework/package, texture, rig, animation, runtime script or LFS object is required. No colliders were retained: the references are visual rulers and do not introduce movement blockers. Capsule casts below are independent measurement tools, not character-controller qualification.

### Sandbox placements

One new scene root, `Human Scale Validation - provisional`, at **(-30,0,-30)**, separate from the unchanged foundation prefab. Assembly-local positions in metres:

| Reference | Height | Local position | Purpose |
| --- | ---: | --- | --- |
| Portico landing | 1.80 | (0,0,-2) | Columns, entablature and entrance breadth |
| Front door inside | 1.80 | (0,0,0.65) | Open leaves and arch |
| Center foyer | 1.80 | (0,0,4) | Room width/depth and ceiling |
| West guide wall | 1.80 | (-3.5,0,4) | Wall/head relationship |
| East guide wall | 1.80 | (3.5,0,4) | Opposite wall/clear hall width |
| West wing opening | 1.80 | (-12,0,8) | Transverse circulation/extension junction |
| Foyer comparison | 1.68 | (1,0,4) | Second adult height beside center figure |

All seven renderer bounds start at world Y=0 and match their declared heights within 1 mm. Existing Sandbox objects remain unchanged; only the new group/prefab instances and the scene root-list addition are serialized.

### Findings and recommendation

**The foundation reads appropriately large for a wealthy estate entry seed. No architectural adjustment is recommended from this measurement pass, and none was made.**

- Upward physics rays at the portico, center hall and beside both guide walls hit the overhead underside at **3.800 m**: **2.000 m** above the 1.80 m head, or **2.120 m** above the 1.68 m head under the same deck. No head/ceiling problem. The ceiling feels generous for a single-height formal hall; this is not a double-height atrium.
- The open double-door leaves measure **2.800 m** high and **1.860 m clear between their inner faces**, rather than the approximately 2 m nominal opening inferred from leaf placement. Their tops are 1.00 m above the adult head. The taller arched surround remains visibly grand. The 2.8 m leaf height is not a lintel/arch-clearance measurement. A **1.80 m tall / 0.80 m wide** capsule sweep from local (0,0.05,-2) forward 14 m found no foundation blockers. This does not qualify animated doors or a final controller.
- The guide-wall inner faces measure **7.800 m** apart. Their 4 m length guides the entrance without defining a closed foyer. The core continues **12 m** front-to-rear with cross circulation reserved at Z=6..10. Figures occupy a modest portion of the floor, not a quarter of the foyer. The hall feels broad, with substantial room around people; open rear/sides make unfinished space feel more expansive than a completed furnished room may.
- **4 m bays** read as substantial architectural modules: 2.22 adult heights in spacing, about seven proxy shoulder widths. They do not read as person-sized cubicles. Retain this rhythm.
- **24 x 12 m core / 32 m seed frontage** reads about right as a large central entry block awaiting wings. It is not evidence that this footprint alone can hold the complete estate program. The current two-story silhouette, provisional cap, missing wings, opaque glazing and unfinished interior frame backs still limit the final mansion impression. No footprint reduction/enlargement is justified by the figures.
- Three 150 mm approach steps remain plausible at human scale. No stair traversal or final stair design was attempted.

### Views, checks and effort boundary

Reviewed six 1440 x 1080 captures in ignored `Temp/HumanScale-view-0..5.png`: full frontal frontage, oblique portico, foyer looking rearward, foyer looking toward entrance/guide wall, west wing opening, and close entrance/arch. Views 2, 3 and 4 use **1.65 m camera height**, 60-degree field of view. Other views supplement rather than substitute for eye-level judgement. Temporary cameras were destroyed; lighting and scene cameras were not changed. Measurements are in ignored `Temp/HumanScale-verification.txt` and `Temp/HumanScale-details.txt`.

Unity 6000.3.27f1 recompile returned **up_to_date**, failed=false, compilationFailed=false, no compile errors. Full current EditMode suite: **121/121 passed, 0 failed, 0 skipped, 0 inconclusive**, reported duration **1.82 seconds** (`Temp/pipeline_test_status.json`). No missing scripts or material slots in the new references. Final native Console counts: **0 errors / 0 warnings**, with matching empty capture counters (`Temp/HumanScale-final-console.json`). Four command-discovery attempts during this task incorrectly used tag names as command names and logged automation errors; their messages were preserved in `Temp/HumanScale-command-errors.json`, then cleared before asset/compile re-verification. They were not project code/asset failures; historical captured errors from prior tranches are also distinguishable by timestamp in that file.

Protected scene Git blob hashes match the pre-tranche HEAD (`612f39f`):

- POC: `eb6175ee6782c5bd8d46c7b1ccefc86bccc9fdf9`.
- Production Mansion: `e0ec7480b289f1311c7b517ed1fff59d3181845d`.

`git diff --exit-code HEAD --` also confirms no changes to the foundation prefab, vendor Mansion assets, Packages or ProjectSettings. Original Sandbox serialized objects compare identically; its existing root-list block only gains the new validation group. Documentation changes are this report and EnvironmentWorkflow.

Fabrication required a short in-memory Unity builder, two prefab saves, seven placements, six rendered views and measurement/verification. The builder lives in ignored Temp, with no persistent editor helper. This is a materially smaller asset/dependency task than the prior Mansion Pack qualification/assembly (which involved package overlay, mesh/material/pivot inspection and selected vendor dependency management). It does not show that fabricating finished character art or architecture would be cheaper than purchasing it. No comparable prior elapsed time or fresh quota measurement is available, so no time ratio or usage delta is claimed. The ~94% five-hour / ~77% weekly figures are user-supplied pre-task reference values only.

## Grand staircase / master-suite addition - 2026-10-29

See [MansionStairMasterSuiteShell.md](MansionStairMasterSuiteShell.md) for the current Sandbox stair opening, return staircase, east/right upper hall, first-left receiving-room entrance, bathroom/transition, rear bedroom and provisional balcony terrace. The foundation asset remains unchanged; Sandbox disables its single central upper slab and pairs it with the new owned replacement-deck/stair/suite assembly. The earlier no-stair and unobstructed axial-route observations above describe the pre-stair foundation. Ground cross circulation at Z6..10 is retained. Two foyer scale rulers moved to the side approach; the historical reference-position table above is superseded for those two placements. Current full suite is 206/206 passing; current Console has 0 errors / 0 warnings.
