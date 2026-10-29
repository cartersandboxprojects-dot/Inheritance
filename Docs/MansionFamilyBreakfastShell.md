# Family breakfast room shell - 2026-10-29

## Checkpoint and scope

Safety guard passed: clean main at 4e8aab44b50000279e61fbba79ba136fdd012c62, equal to origin/main; staircase correction chat idle; Unity 6000.3.27f1 had saved Prototype_Sandbox open in EditMode, no compilation. Read MansionQualification, MansionEntryFoundation, MansionStairMasterSuiteShell and the superseding MansionStairCorrection before construction.

Only Prototype_Sandbox, the new owned Mansion_Family_Breakfast_Shell prefab/meta, and environment docs change. The prefab uses existing Entry_Limestone, Entry_Stone, Entry_Glazing_Provisional and twelve nested qualified FloorTile4A modules. No new vendor assets, materials, packages, scripts, gameplay changes, furnishings or decor. This is the Day 1 family breakfast/informal dining room, not the formal dining room. Design intent: family institution that happens to be a home. A modest rectangular single-height bay preserves escalation for formal dining/ballroom spaces.

## Location, dimensions and construction

Prefab: Assets/Inheritance/Environment/Sandbox/Mansion_Family_Breakfast_Shell.prefab. Scene root: Family Breakfast Room Shell, at (-30,0,-30), identity rotation, unit scale. Coordinates below are foundation-local metres: front -Z, rear +Z, east +X.

- Immediately behind the west half of the central block: X=-12..-4, Z=12..18, ground floor Y0. World bounds X=-42..-34, Z=-18..-12.
- Nominal 8 x 6 m bay, 48 square metres; 0.2 m walls contained within those bounds yield **7.6 x 5.6 m clear**, about 42.56 square metres. Clear ceiling **3.8 m**, removable ceiling deck top **Y4**.
- Two 4 m principal bays across, three 2 m tile rows deep. Existing 1 m/grid, vendor tile pivot and 4 m floor datum conventions retained. No upper room or wider wing is built. Removable deck permits future vertical continuation.
- Foundation top Y=-0.02, tile floor Y0; ceiling underside Y3.8. New floor/deck meets the existing rear edge at Z12 without extending over it. Side walls butt the south/north walls, rather than overlapping corner volumes.
- North wall includes a 2 x 1.7 m daylight aperture, sill Y1.1/head Y2.8, backed with existing opaque provisional glazing. This is a shell opening, not final fenestration or daylight qualification.

## Circulation and unbuilt connections

| Threshold | Local center | Clear opening | Relationship |
| --- | --- | --- | --- |
| Main south entry | (-6,0,12.1) | 2 x 2.8 m | Offset east; reached from foyer via west stair-side aisle at X=-6, without entering the stair footprint |
| North service | (-10,0,17.9) | 1.4 x 2.8 m | Reserved kitchen/pantry/service approach; no kitchen/service corridor or floor beyond the room built |
| West family sitting | (-11.9,0,16) | 1.6 x 2.8 m | Reserved nearby family living/sitting connection; adjacent room unbuilt |
| East family/private | (-4.1,0,14) | 1.4 x 2.8 m | Reserved private circulation; adjacent circulation unbuilt |

The east opening was initially at Z16. A clearance sweep found the existing provisional gallery pier beyond it. Moving only this new opening to Z14 clears that pier. Stair/gallery/supports/suite were not altered. The new room does not close the X+/-16 wing extension planes. Adjacent floors and routes beyond these future thresholds remain explicitly undesigned; doorways are reserved connection points, not complete usable destinations. No change to central foundation is required.

## Capacity and visual/spatial validation

Assumed table footprint **3.6 x 1.2 m**, centered (-8,0,15), long axis east/west. Ten illustrative places: four each long side at 0.9 m pitch, one each end. This comfortably accommodates Brecken, Elena, Charlie, Victoria and three additional family placeholders (seven), with capacity to investigate 8-10 later. Conservative occupied table/chair envelope **5 x 2.6 m**, leaving **1.3 m** at each end and **1.5 m** behind the long sides to inner walls. Actual chair pull-out and service choreography require future furnishing validation.

Seven 1440 x 1080 views inspected: doorway approach, standing room view, seated-table perspective, center-room view, back toward central circulation, service-side view, and cap-hidden overview. Standing cameras use Y1.65; seated uses Y1.2. Temporary floor-only table outline and ten seat-footprint pads validate dimensions; all guides, inspection cameras and temporary occupied-envelope collider were destroyed before final scene save. No table, chairs or guides are shipped.

Proportions read as generous everyday family dining, with direct conversational sightlines and usable perimeter routes. Ceiling/wall and floor/wall joins show no obvious accidental gaps, coplanar surfaces or pivot/snapping problems. Existing floor-module grout seams remain visible. Main entry looks back into existing circulation and existing human-scale references. Caps restored after overview. Opaque glazing, plain unfinished shell walls and visible unfinished surroundings through reserved openings remain known shell limitations.

Eight 1.8 m tall/0.8 m diameter capsule sweeps return zero lateral blockers with the occupied envelope present: existing west aisle to entry, four perimeter segments, and three future thresholds. Floor rays hit Y0 within floating point tolerance; room ceiling Y3.80000019; entry lintel Y2.8. Zero missing scripts/material slots; zero temporary validation objects remain. These are camera/physics checks, not keyboard/controller traversal or final furnishing qualification.

## Deliberately deferred architecture

Grand staircase placement/form is accepted provisionally and unchanged. Its depth may later need to increase to improve foyer arrival space. Brecken's balcony remains provisional; future target is rear-facing or rear-offset access from the sitting/receiving room, subject to wing planning. No balcony correction here. Existing suite, gallery, future third-floor stair envelope, foundation, facade, all broader wings, kitchen/service complex, family sitting room, offices/boardroom, formal dining, ballroom and other suites remain untouched/unbuilt as applicable. Future upper program, permanent roof/exterior finishing and surrounding room placement are unresolved.

## Validation and evidence

Full unfiltered EditMode suite: **206/206 passed**, zero failed/skipped/inconclusive, duration 2.11 s. Recompile up_to_date, failed=false, compilationFailed=false, errors empty. Final native Console **0 errors / 0 warnings**; capture counters also zero errors/warnings. Early command-discovery requests used unsupported command syntax; diagnostics were preserved before clearing and rerunning final validation, with no project-code failures.

Ignored evidence: Temp/Breakfast-view-0..6.png, Breakfast-verification.json, Breakfast-full-tests.json, Breakfast-final-compile.json, Breakfast-final-console.json, Breakfast-historical-console.json, Breakfast-protected-before.json and Breakfast-protected-after.json. Builders are ignored Temp-only in-memory automation, not shipped editor helpers.

Pre/post SHA-256 hashes match exactly:

| File | SHA-256 |
| --- | --- |
| Assets/Scenes/Game/Mansion.unity | 8FA6D5996D5FEAA8311D8A86A1D456F5B91CBA7792E2EC0DAD0EBC5955C3DF41 |
| Assets/Scenes/Game/Mansion.unity.meta | FC5E6D7FD3EBB3AEC1603D2E7BF7998A5774D252DABF9B349DCDBCEF44A75DB2 |
| Assets/Scenes/Prototype/Prototype_Mansion_POC.unity | AD62AFA9FCFE923C6F6637C2E0E22B708B7B4D6E39FDD0A256D8F85962460A4A |
| Assets/Scenes/Prototype/Prototype_Mansion_POC.unity.meta | 9AD9FC0E72FF0BB47DCDF9170A58EF1B413ADF3B14B28F7BD9358131481DC021 |

Final scope review: scene adds one prefab instance and root only; unrelated Unity trailing-space serialization in the runner name normalized to baseline. No backend, settings, package, vendor, protected scene, original foundation, or stair/suite asset changes. Documentation: this report plus EnvironmentWorkflow and MansionEntryFoundation cross-references. Commit/push outcome is reported in the task handoff.
