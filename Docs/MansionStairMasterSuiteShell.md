# Grand staircase and Brecken master-suite shell - 2026-10-29

## Scope and ownership

Sandbox-only architecture tranche on baseline `21096b447761a19615607d91df2bfcf5e6dd689b` (main), Unity 6000.3.27f1. Scene: `Assets/Scenes/Prototype/Prototype_Sandbox.unity`. New owned assembly: `Assets/Inheritance/Environment/Sandbox/Mansion_Stair_MasterSuite_Shell.prefab`. Its asset root is origin zero, identity rotation, unit scale; scene instance is (-30,0,-30), aligned with Mansion Entry Foundation. All coordinates below are assembly-local metres: front -Z, rear +Z, east/right +X as viewed from the front approach.

Read MansionEntryFoundation and MansionQualification before building. Retain 1 m placement grid, 4 m principal bays, Y=0/4/8 floor datums; stair and clearance subdimensions intentionally use smaller increments. Existing qualified vendor frontage/floors remain nested in the unchanged foundation asset. New walls/slabs/stair/solid guards use owned primitive construction and existing Entry_Stone / Entry_Limestone materials. No new vendor asset, material, package, runtime/editor source, furniture, fixtures, decor or lighting was added.

## Stair and circulation

- A broad return stair sits within the established foyer: flight treads X=-2.8..-0.4 (lower) and X=0.4..2.8 (upper), Z=1.6..4.4. Each flight is **2.4 m clear** between solid graybox guards.
- Lower flight climbs north, turns on a **5.6 x 1.6 m half landing at Y=2**, then upper flight returns south to Y=4. The landing's back guard reduces its clear turning depth to approximately **1.48 m**; a 0.8 m diameter body has turning space. This is a compact graybox return landing, not a final construction/code qualification.
- **22 equal risers**, 11 per flight, measured minimum/maximum **0.181818008 / 0.181818247 m**, total **4.000 m**. Each flight has ten **0.280 m goings** plus its arrival landing. Horizontal tread run is 2.8 m per flight.
- Upper arrival landing: **6 x 1.6 m**, X=-3..3, Z=0..1.6. A stairwell opening X=-3..3, Z=1.6..6 removes the overhead conflict. Solid guards protect stair sides, half landing and upper well edges; nominal guard height is 1.1 m. Sloping guard panels were adjusted for their projected vertical height. The half-landing undercroft is solid, preventing entry into its otherwise 1.8 m high underside.
- The original single central upper slab is disabled **only by a Sandbox scene override**. Four owned replacement slabs retain its exact original 24 x 12 m coverage except for the well. The source foundation prefab is unchanged; its disabled slab must stay paired with this replacement assembly. No vendor object is unpacked or edited.
- An approximately **9 m long x 4 m nominal bay-width hall** runs east/right from the arrival, X=3..12, Z=0..4. Wall faces and existing facade projections reduce the usable width locally (approximately 3.7-3.9 m). Its east continuation into the existing stub remains open; no terminal doorway/end wall fixes future circulation.
- The original first-floor transverse band **Z=6..10** remains clear through both wing stubs. The grand stair replaces the former unobstructed ground-floor axial route; side aisles remain beside it. Exactly two existing foyer guide walls remain unchanged. This is the intended stair insertion, not preservation of the former front-to-rear axial clearance claim.

## Suite sequence and approximate dimensions

| Space | Nominal dimensions | Local placement / intent |
| --- | --- | --- |
| First left suite entrance | **1.6 m clear x 2.8 m high** | North/left side of eastbound upper hall, center X=6, Z=4; first and only suite doorway along this hall |
| Outer receiving room | **8 x 8 m bay**, about **7.8 x 7.8 m clear** | X=4..12, Z=4..12; entry at X6, private exit offset east to X10 |
| Balcony doorway | **1.8 m wide x 2.8 m high** | East side of outer room, X=12, Z=7.1..8.9 |
| Bathroom shell | **4 x 4 m bay**, about **3.8 x 3.8 m clear** | X=4..8, Z=12..16; side entry from private transition at Z14 |
| Private transition | **4 x 4 m bay**, about **3.8 m clear width** | X=8..12, Z=12..16; two 1.4 x 2.8 m thresholds on X10 |
| Master bedroom | **8 x 8 m bay**, about **7.8 x 7.8 m clear** | X=4..12, Z=16..24, deepest room; empty shell |
| Balcony / covered terrace | **3 x 4 m**, about **2.8 x 3.8 m usable** | X=12..15, Z=6..10; uses the existing east stub slab and provisional cap above |

The suite's nominal main footprint is 8 x 20 m. Only its own rear deck extends beyond the old Z12 seed, to Z24; four removable 0.4 m square support piers below the extension meet the ground and slab underside. No ground-floor rooms or whole-floor layout were constructed beneath it. Walls meet the Y8 underside of the removable cap; suite clear ceiling height is 4 m. Doorways remain open architectural thresholds, without animated leaves or interaction.

Privacy sequence: hall -> receiving room -> offset private transition with side bathroom access -> rear bedroom. A ray from the hall at (6,5.65,2) toward the rear bedroom at (10,5.65,20) is blocked by the offset transition wall. Receiving-room access therefore does not expose a direct axial bedroom view. The bathroom is a side room, not a compulsory route through fixtures to reach the bedroom.

The balcony has solid 1.1 m graybox perimeter guards and direct receiving-room access. It is a **provisional covered terrace on existing slab**, not a newly qualified cantilever or final facade composition. Its removable boundaries do not end the east wing at X16 or occupy a finalized wing program. Reconcile its location when the east wing is designed. No unqualified vendor railing kit was imported.

## Inspection findings and corrections

Reviewed eight final 1440 x 1080 captures: front/side cutaway, foyer stair, upper arrival/hall, receiving room, private transition, bedroom, balcony looking inward, and rear exterior. Interior views use **1.65 m eye height above their floor**, 68-degree field of view. The overview temporarily hides both caps; they are restored immediately. Cameras/render textures are destroyed, and scene lighting is unchanged.

Corrections during inspection:

- Replaced the overhead slab locally with a measured stair opening; retained the existing facade, first-floor cross corridor and wing stubs.
- Unified the rear suite cap with the existing Y8..8.16 cap datum and extended owned wall/lintel tops to meet its underside, eliminating ceiling gaps and a cap height step.
- Removed redundant doorway reveal blocks after identifying coplanar side faces and a gap in an attempted reveal adjustment. Final simple wall/lintel thresholds have continuous clean joins.
- Corrected sloping guard panel height for rotation, closed the low half-landing undercroft, and provided provisional piers rather than leaving the rear extension visually unsupported.
- Relocated the two earlier center-foyer scale rulers that occupied the new stair footprint to (-3.45,0,1) and (3.45,0,1). Added five instances of the existing 1.8 m adult ruler at the half landing, upper arrival, receiving room, bedroom and balcony. No new reference character assets were created.

Measured all 22 consecutive stair rises and ceiling clearances along the ascent: **minimum sampled route headroom 4.0 m**. Entry lintel clearance measures **2.8 m**. Downward rays hit Y4 in receiving/transition/bedroom/balcony and Y2 at the half landing. Six **1.8 m high / 0.8 m diameter capsule sweeps** found zero lateral blockers along the ground cross corridor, upper hall, suite entrance, private passage/bedroom, bathroom doorway and balcony doorway. No missing scripts or material slots in the new assembly. No pink materials, accidental slab overlaps, floating steps or unintended doorway gaps remained in the final reviewed views.

This verifies dimensions and inspection routes; it does not qualify a character controller stepping over individual risers, perform keyboard traversal or certify construction compliance. Furniture, daylight openings for the rear bedroom, exterior finishing and final architectural structure remain deferred with the shell/art pass.

## Deliberately provisional geometry

The suite cap, support piers, balcony terrace boundaries and rear wall are explicitly provisional. Existing X=-16/+16 wing extension planes remain open. The broader rear, western second floor, service/elevator cores, other suites, ground-floor rear program and story 3+ remain undesigned. The stair is one local circulation choice; no entire upper-floor footprint, central atrium redesign, office/boardroom or new wing has been imposed. Existing open outer deck edges elsewhere in the unfinished foundation remain unfinished; stairwell and new balcony edges receive guards in this tranche.

## Validation and protected files

Full unfiltered EditMode suite: **206/206 passed**, 0 failed, 0 skipped, 0 inconclusive; duration **2.12 seconds**. Recompile completed: failed=false, errors=[], compilationFailed=false. Current native Console ground truth after final render/recompile/tests: **0 errors / 0 warnings**. Three initial automation command-discovery requests used unsupported names; historical logs were preserved before clearing the Console and rerunning final verification. These were tooling requests, not project source/asset failures.

Ignored evidence: `Temp/StairSuite-view-0..7.png`, `StairSuite-inspection.json`, `StairSuite-measurements.json`, `StairSuite-full-tests.json`, `StairSuite-final-compile.json`, `StairSuite-final-console.json`, `StairSuite-historical-console.json`, `StairSuite-protected-before.json`, `StairSuite-protected-after.json`. Builders/inspection code exist only in ignored Temp, with no shipped helper.

All four pre/post **SHA-256 hashes match exactly**:

| Protected file | SHA-256 |
| --- | --- |
| Assets/Scenes/Game/Mansion.unity | 8FA6D5996D5FEAA8311D8A86A1D456F5B91CBA7792E2EC0DAD0EBC5955C3DF41 |
| Assets/Scenes/Game/Mansion.unity.meta | FC5E6D7FD3EBB3AEC1603D2E7BF7998A5774D252DABF9B349DCDBCEF44A75DB2 |
| Assets/Scenes/Prototype/Prototype_Mansion_POC.unity | AD62AFA9FCFE923C6F6637C2E0E22B708B7B4D6E39FDD0A256D8F85962460A4A |
| Assets/Scenes/Prototype/Prototype_Mansion_POC.unity.meta | 9AD9FC0E72FF0BB47DCDF9170A58EF1B413ADF3B14B28F7BD9358131481DC021 |

Git diff review excludes gameplay/backend, tests, other scenes, vendor assets, packages and project settings. Only Sandbox, the new owned prefab/meta and environment documentation are included. No additional LFS art payload is introduced.
