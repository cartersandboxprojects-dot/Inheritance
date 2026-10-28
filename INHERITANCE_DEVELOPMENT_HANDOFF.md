# Inheritance Development Handoff

Persistent record of approved, implemented build milestones. Keep completed entries
and add new verified milestones newest first. Record scope, evidence, commit anchors,
and remaining limits; exclude exploratory chat. Git history supplies the exact
commit for each entry. See INHERITANCE_CURRENT_CHECKPOINT.md for restart state.

Convention mirrored from Wedge's WEDGE_DEVELOPMENT_HANDOFF.md and
WEDGE_CURRENT_CHECKPOINT.md, inspected October 27, 2026. This record documents
source/build provenance; it does not independently establish legal ownership.

## Environment structure (2026-10-28)

Started on clean main at 9f003ac; Unity 6000.3.27f1. Moved Prototype_Mansion to
Assets/Scenes/Prototype/Prototype_Mansion_POC.unity with unchanged scene contents
and original meta/GUID. Created independent Prototype_Sandbox with existing
playable graybox and a new 200m test ground/workspace; the original had no terrain.
Created Assets/Scenes/Game/Mansion.unity as geometry/camera/light shell, removing
prototype cast, player and runner from this copy only. Mansion replaces SampleScene
as the sole enabled production build scene; SampleScene itself remains untouched.

Docs/EnvironmentWorkflow.md establishes frozen POC, experimental Sandbox and clean
production lanes; GameState/content-driven day/time/story variation; third-party
qualification; and lightweight optional prefab promotion paths. No gameplay code,
packages or asset pipeline added. Current Ink scene-opening instructions migrated.

Verified all three scenes open with zero missing scripts/broken serialized references.
POC scene/meta Git hashes equal their original HEAD versions. Full EditMode suite:
60 passed, none failed/skipped/inconclusive. Live POC handlers exercised both choices
and Victoria responses. Save/reset/load restored AfterFuneralNight, preserving the
original save-slot bytes/existence. Frames advanced 982 -> 4661. No physical-key
movement or standalone build retest. Editor left on clean POC outside Play Mode,
background override off, compilation idle and current Console zero errors/warnings.
Pipeline retains an old October 27 invalid-command error in its history.

ProjectSettings and ShaderGraphSettings showed line-ending-only working-file rewrites;
normalized Git hashes equal HEAD, with no serialized-value changes. Only intended
scene/meta/docs/build-list changes belong in this tranche. This entry shares the
commit Establish prototype sandbox and production scenes; Git supplies its exact hash.

## Wall-alignment cleanup (2026-10-27)

Approved scope: smallest human-eye cleanup of Prototype_Mansion wall joins/ends.
Starting repository: clean main at 63a7050bfb9f7868a98828b19dd8cb67ff21d0c6.

Visual inspection of the live Unity Scene view showed north/south wall end tabs.
Those walls were 17 units long while the side-wall outer faces sit at x = +/-8.25.
Changed only North Wall and South Wall localScale.x from 17 to 16.5 through the
live Editor, retaining their positions, heights, thicknesses, colliders and layout.
Each end is shortened 0.25 units; exterior corners now meet flush by eye.
No redesign, materials, lighting, characters, camera or gameplay code changes.
The scene diff contains exactly those two scale edits.

Verification on Unity 6000.3.27f1:
- Before/after live Scene view inspected from the same perspective; end tabs removed.
- Unity compilation: up to date, zero errors and warnings.
- Full Inheritance.Tests EditMode suite: 60 passed, 0 failed/skipped/inconclusive.
- Live Play Mode advanced from frame 314 to 3263. Used actual public interaction
  handlers: Elena talk/choice, AfterFuneralNight transition, Victoria talk and
  correct family response. This check positioned Brecken through Pipeline; physical
  movement keys were not retested. No save-slot writes were performed.
- Console: zero errors/warnings. Editor returned outside Play Mode and temporary
  run-in-background override restored to off.
- An async test request did not yield a completed verdict; it was cancelled and
  replaced by the completed synchronous 60-test run above.
- Incidental Unity URP asset migration/line-ending rewrites excluded from this work.

This entry and the scene fix are committed together. Resolve their exact commit
with git log -- INHERITANCE_DEVELOPMENT_HANDOFF.md Assets/Prototype_Mansion.unity.
No architectural hardening or new story loop is included.

## Persistent narrative prototype milestone (2026-10-27)

Verified Git anchor: 63a7050bfb9f7868a98828b19dd8cb67ff21d0c6.
Actual commit subject: "Add validated backend save/load persistence to mansion prototype".
The supplied handoff called it "Add prototype save load persistence"; retain that
as the supplied milestone label, not the literal Git subject.

Evidence: repository source, ProjectSettings/ProjectVersion.txt, package lock,
Docs/PrototypeState.md, Docs/InkPrototype.md, Docs/SaveLoad.md and Joe's approved
October 27 development summary. Historical manual verification below is reported
from that milestone, distinguished from the cleanup verification above.

### Environment and playable graybox

Unity 6.3 LTS, exact Editor 6000.3.27f1, Universal Render Pipeline (URP 17.3.0).
Prototype_Mansion supports Brecken movement, proximity interaction, a Talk UI and
stationary labeled Elena, Charlie and Victoria. The supplied milestone reports
separate Git clones verified on primary and secondary laptops; that cross-machine
claim was not re-executed during this cleanup.

### State and consequence boundaries

GameState preserves distinct conceptual categories: objective world facts and
historical event tags; character knowledge/awareness; flexible character-specific
relationship attributes; character-specific state; opportunity state; story
progression; and time blocks/day. Current concrete storage is facts, relationships,
knowledge, opportunities, phase, day and time. Character-specific meaning is authored
through identifiers/content, not a separate universal stat bundle.
World truth never automatically grants character knowledge.

Generic Condition / Effect / Consequence definitions query and modify backend
state. Content owns story meaning and guarded eligibility; GameState remains
engine-independent. Effects apply in order, with no transactional rollback yet.

### Ink and funeral-night branch

Official Ink Unity Integration 2.0.0 (locked revision
73cc147969899710843f473fdb06cea0e4d50fed) is provisionally adopted.
Ink controls conversation flow and queries; the backend owns authoritative state
and applies guarded consequences. Ink is not a parallel state authority.

On funeral night, Elena needs emotional support and Charlie requests business
attention. Brecken can choose only one; repeated/opposite applications are rejected.
Choosing Elena sets BRECKEN_COMFORTED_ELENA, misses Charlie's opportunity, decreases
Elena resentment, advances Evening to Night and phase to AfterFuneralNight.
Choosing Charlie sets BRECKEN_MET_CHARLIE_FUNERAL_NIGHT, completes his opportunity,
increases Elena resentment, decreases Charlie resentment and advances the same
phase/time. Current numeric deltas are authored prototype values.

Victoria initially reports an unresolved decision. Talking to her after choosing
is an explicit authored information transfer. Her later response requires both
exclusive world truth and legitimate knowledge, plus the later story phase:
- Family: "I see you have chosen family over business."
- Business: "Your mind is on business instead of family."
Choice alone does not inform her. Queries do not grant knowledge.

### Versioned JSON save/load

Schema v1 saves/restores backend facts, relationships, knowledge, opportunities,
phase, day and time rather than scene snapshots or Ink execution position.
Single-slot Save/Load validates before replacing the active state; malformed,
unsupported-version and contradictory saves are rejected. Failed Load preserves
the active session. Load does not replay consequences. No autosave, slot browser,
cloud sync or migration framework is included.

### Established verification

Milestone records report 60 passing EditMode tests covering generic state,
conditions/effects, exclusive branches, legitimate knowledge, Ink integration,
round trips, validation, schema handling and safe file behavior. An isolation test
was corrected to remove contradictory prerequisites; gameplay behavior was unchanged.
Architecture review found no gameplay/architecture defects at this scale.

Live handler-based verification covered both branches, early Victoria, save/load
before/after reporting, Play Mode exit/re-entry, invalid load preservation and repeat
rejection. Joe additionally manually played both branches, visited Victoria before
choosing and confirmed smooth movement/interaction. The recorded Editor compilation
and final Console had zero project-caused warnings/errors. See Docs/SaveLoad.md for
precise historical verification methods and platform limits.

### Future hardening, not current blockers

- Generalize Elena-specific dialogue bridge names when further interactions justify it.
- Revisit string identifiers as content expands to reduce typo-driven errors.
- Consider transactional consequence application before complex multi-system effects.
- Add save migration as the schema evolves; other platforms/player builds remain unverified.

The next story expansion remains unimplemented. A second small authored loop is a
possible next step, not an approved specification in this log.
