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
