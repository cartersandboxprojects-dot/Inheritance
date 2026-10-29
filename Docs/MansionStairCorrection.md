# Grand staircase correction - 2026-10-29

## Checkpoint and scope

Safety guard passed before edits: repository C:/Users/josep/Inheritance on main was clean, HEAD and origin/main both d63f474a73c2df9f4d58dce67ac160bb5c2d6923. Previous architecture chat was completed/idle. Live Unity had the clean Prototype_Sandbox scene open in EditMode, with no compile running. This correction supersedes the return-stair/cross-band claims in MansionStairMasterSuiteShell.md; its suite dimensions and privacy arrangement remain current.

Only the existing owned Mansion_Stair_MasterSuite_Shell.prefab and environment documentation change. The Sandbox instance uses the same origin (-30,0,-30). Foundation/vendor assets and Production/POC remain unchanged. No dining/breakfast room, furniture, decor, office, boardroom, third-floor stair, new asset, gameplay, package, or settings changes.

## Corrected geometry

Coordinates are assembly-local metres, front -Z and rear +Z.

- Single centered ceremonial flight, rising north/away from the entrance, **4 m clear width**, X=-2..2. Symmetrical solid graybox banisters on both sides use existing Entry_Stone material and primitive construction. No ornamental railing asset was imported.
- Base at **Z6**, at the end of the established Z2..6 foyer guide walls, versus the former Z1.6 start. Entrance at Z0 now leads through **6 m of open arrival space** before the first riser. Foyer width remains 7.8 m clear between guide walls. Existing human references remain to the sides of the approach.
- **4 m total rise**, **22 equal 181.818 mm risers**, **21 x 300 mm goings**, **6.3 m horizontal run**, upper arrival at **Z12.3/Y4**. Measured rise range 0.181818008..0.181818247 m. No half landing or split/return flight remains in the first-to-second-floor stair.
- Gallery slab **7.7 x 3.7 m**, X=-3.9..3.8, Z12.3..16. Its 1.1 m solid perimeter guards reduce clear dimensions to about **7.46 x 3.58 m**. Rear support piers and the Y8 cap are explicitly provisional. The small rear extension is required for the corrected stair arrival, not an additional room.
- Opening X=-2.15..2.15, Z6..12.3; side deck strips continue to the gallery. The main flight has no overhead Y4 slab. Measured current center-route headroom is **at least 4.0 m**; foyer ceiling remains 3.8 m.
- The east side passage between the well guard and suite west wall is about **1.69 m clear**, X2.21..3.9. From the gallery, walk south past the reserved future up-stair position, then turn east into the existing **9 x 4 m nominal hall** at Z0..4. Brecken's entrance stays at X6/Z4, **first door on the left when walking east**; clear opening remains 1.6 x 2.8 m.
- Only the projecting west end of the hall's suite-entry wall was shortened, from X3.1 to X4, to reconnect this passage. Receiving room, bath, private transition, bedroom, balcony, doors and privacy sequence are otherwise unchanged.
- The former unbroken ground cross band at Z6..10 is superseded: the new central flight occupies its center. Ground movement uses the side aisles and the open foyer before the stair. No wing junction is closed.

## Future reverse-direction third-floor stair

No third-floor flight is built. Reserve a centered flight approximately X=-2..2, **starting at the rear gallery near Z16/Y4**, rising **south toward Z9.7/Y8**, opposite the current northbound main flight. The rear gallery gives turning/departure space, while the east side passage carries the route past this future stair to the hall. Keep that passage and the first-left suite doorway.

This rearward departure offset matters: placing the future flight's first riser directly at Z12.3 would collide with the current ascent. With the Z16 departure, a comparable 6.3 m run and a 0.2 m stair structure give an illustrative underside near Y6.04 at the present arrival, approximately **2.04 m above Y4**. The future detail still needs actual tread/landing/guard/headroom verification before construction. The rear guard is deliberately removable for the future departure; existing gallery/foundation caps at Y8 are removable to form the future upper opening. No permanent wall, suite, column, or room occupies the future flight interior. Current edge protection stays installed until that flight is built. The portion beneath its future low end will require enclosure/route control as part of that later pass.

## Visual and spatial checks

Reviewed 1440 x 1080 captures at **1.65 m above the floor** from just inside the doors (Z0.8), middle foyer (Z3.5), stair base (Z5.4), upper gallery (Y5.65/Z14.5), east passage and eastbound hall, plus a cap-hidden overview. Caps were restored and temporary cameras destroyed. The supplied reference image itself was unavailable in the cached conversation; the explicit written single-flight/symmetrical/arrival-space requirements governed this correction.

Entrance view now has a centered broad stair destination beyond substantial visible tiled arrival space. Restrained stone mass and paired guards maintain inherited authority without ornamental additions. The upper view shows the foyer below and continuous side circulation. Open rear/wing geometry, opaque glazing and unfinished interior vendor surfaces remain visible shell limitations.

Corrections found during verification: shortened the hall wall projection that initially blocked the side route, and removed a 100 mm overlap between the new gallery cap/deck and the existing suite extension. Final stair treads, guards and deck joins show no obvious accidental gaps or coplanar faces in the inspected views. Final 1.8 m high / 0.8 m diameter capsule sweeps find zero lateral blockers across foyer approach, gallery-to-passage, passage-to-hall, eastbound hall and suite entrance. All 22 rises are measured; zero missing scripts/material slots.

This is graybox geometry and camera/physics validation; player-controller stair traversal was not tested.

## Validation and protected files

Full unfiltered EditMode suite: **206/206 passed**, zero failed/skipped/inconclusive. Script recompile: **up_to_date**; no changed project scripts. Native Console after checks: **0 errors, 0 warnings**. One initial temporary builder request used eval_file instead of the required script-run mode and returned compiler diagnostics without changing project code/assets; corrected invocation succeeded. Evidence stays in ignored Temp/StairCorrection-* files.

Production and frozen POC scene/meta SHA-256 values match the previous completed checkpoint:

| File | SHA-256 |
| --- | --- |
| Assets/Scenes/Game/Mansion.unity | 8FA6D5996D5FEAA8311D8A86A1D456F5B91CBA7792E2EC0DAD0EBC5955C3DF41 |
| Assets/Scenes/Game/Mansion.unity.meta | FC5E6D7FD3EBB3AEC1603D2E7BF7998A5774D252DABF9B349DCDBCEF44A75DB2 |
| Assets/Scenes/Prototype/Prototype_Mansion_POC.unity | AD62AFA9FCFE923C6F6637C2E0E22B708B7B4D6E39FDD0A256D8F85962460A4A |
| Assets/Scenes/Prototype/Prototype_Mansion_POC.unity.meta | 9AD9FC0E72FF0BB47DCDF9170A58EF1B413ADF3B14B28F7BD9358131481DC021 |

Final Git scope review excludes backend, vendor, settings, packages, all other scenes and foundation prefab. Unity's unrelated trailing-space serialization in the Sandbox runner name was normalized back to baseline, leaving the scene content unchanged; the existing scene instance receives the prefab correction automatically.
