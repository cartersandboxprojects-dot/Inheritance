# Inheritance Development Handoff

Persistent record of approved, implemented build milestones. Keep completed entries
and add new verified milestones newest first. Record scope, evidence, commit anchors,
and remaining limits; exclude exploratory chat. Git history supplies the exact
commit for each entry. See INHERITANCE_CURRENT_CHECKPOINT.md for restart state.

Convention mirrored from Wedge's WEDGE_DEVELOPMENT_HANDOFF.md and
WEDGE_CURRENT_CHECKPOINT.md, inspected October 27, 2026. This record documents
source/build provenance; it does not independently establish legal ownership.

## Backend gameplay quality checkpoint (2026-10-28)

Started clean on main/origin/main at d7d7dbe3ce85819d001327ba72ea620981e67d8f
(opportunity/phone scheduling, 119 tests), following 6289c49 (authored time costs).
This entry shares the commit titled `Checkpoint backend gameplay systems`; Git
history supplies its exact hash. No new gameplay subsystem or scene integration.

Reviewed state ownership, conditions/effects/consequences, authored costs and
rollover, opportunities/competition/expiry, delayed phone delivery/read, independent
knowledge, relationships, phase and v3 save/load with v1/v2 migration. Ownership
remains coherent: GameState stores progress; immutable definitions own authored
rules; callers evaluate consequences explicitly; persistence owns structural
validation and delegates story invariants to content callbacks.

Fixed one demonstrated defect: uncosted Consequence could partially mutate live
state when a later effect threw. Both new regression cases failed before the fix.
All consequences now apply ordered effects to detached candidates and commit only
on success, matching the existing costed path. No API, schema or cost-policy change.
Strengthened existing tests for copied condition arrays, positive reaction setup,
and v2 partial-capacity migration instead of adding redundant cases.

Full unfiltered Unity 6000.3.27f1 EditMode suite: 121/121 passed, none failed, skipped
or inconclusive. Compilation succeeded, errors=[], compilationFailed=false. Current
Console: zero errors/warnings; historical Pipeline buffer is distinguished from
current Editor ground truth. No extra gameplay walkthrough/player-build validation.
Only backend Consequence code, existing backend tests and these checkpoint/relevant
Docs files changed. Production/POC/Sandbox scenes, art, vendors, packages and project
settings are unchanged.

Current milestone: 3-unit authored within-block time; opportunity acceptance and
completion costs/competition/defer/expiry; Queued/Delivered/Read phone state;
explicit delayed authored consequences; schema v3 with legacy empty-inbox migration.
See INHERITANCE_CURRENT_CHECKPOINT.md for restart state and detailed future risks.
Next debt: stable-ID/reference validation and rename/migration policy, followed by
content invariants and mutation-boundary hardening as production authoring grows.
Directional competition, acceptance across block windows, missed message windows,
guarded reads and one-shot consequences remain deliberate authored responsibilities.
No broad ID refactor or generalized authoring framework was introduced.
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
engine-independent. Effects apply in order; the backend quality checkpoint above
extends detached atomic application to every consequence.

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
- Atomic consequence application is now implemented; retain regression coverage.
- V1/v2 migration to v3 is implemented; plan ID/content migrations as schemas evolve.
- Other platforms/player builds remain unverified.

The next story expansion remains unimplemented. A second small authored loop is a
possible next step, not an approved specification in this log.


## Stable authored identifiers (October 29, 2026)

Stable-ID hardening is implemented; the earlier “next hardening” notes are historical.
See Docs/StableIdentifiers.md for the full inventory, conventions, mode behavior,
alias/removal policy, runtime boundary, and deferred work. Current strings and story
semantics are retained. Category-aware authored catalogs validate actual definition
references; existing uppercase/mixed IDs are explicit legacy spellings. New IDs use
lowercase dot/underscore segments, maximum 128 ASCII characters.

Development mode allows missing canonical targets only on entries explicitly marked
provisional with a reason; strict/tests and runtime reject unresolved references.
Duplicates, malformed IDs, contradictory equalities, invalid enums and invalid
migration rules remain errors in every mode. No Sandbox/scene/UI changes.

Save schema remains v3. The actual runner supplies a catalog for save/load; all
saved ID categories are checked before state replacement. One-hop reviewed aliases
demonstrate character and opportunity renames, collision rejection, and removal by
replacement. Explicitly archived historical facts retain truth/knowledge safely;
other unmapped removals/unknown IDs reject load. Generic registry-free serialization
and GameState cloning remain available for engine tests and actions, not as a
production validation boundary. The funeral-specific story validator is retained.

Full unfiltered Unity 6000.3.27f1 EditMode suite: **167/167 passed**, 0 failed,
skipped or inconclusive (121 baseline + 46 new). Compile completed with failed=false,
errors=[], compilationFailed=false; current Console 0 errors/0 warnings. Existing
Elena/Charlie/Victoria, time-cost and delayed message/reaction regressions pass.
Evidence: ignored Temp/StableIds-full-tests.json and final compile/Console records.
No live gameplay walkthrough/player build was added. Only backend code, its save/load
caller, tests and docs changed; scenes/art/vendor/packages/settings remain unchanged.
Commit title: `Harden authored content identifiers` (resolve hash via Git history).

## PROVISIONAL SANDBOX DAY (October 29, 2026)

A complete non-canonical systems day now runs only in Prototype_Sandbox. Every new
beat is provisional AI-generated test content, not accepted story. Morning free
Elena/Charlie conversations unlock a 2-unit family task versus a 3-unit business
call; competition, explicit time boundaries, delayed phone delivery, a guarded
Victoria reaction and separate v3 save/load slot carry both paths to Night.
All new catalog entries carry ProvisionalReason and resolve strictly. No backend
architecture or canonical Ink was changed. See Docs/ProvisionalSandboxDay.md for
exact flow, IDs, Personal Carter rewrite responsibilities and verification.
Full EditMode suite: 171/171 passed (167 baseline + 4). Both live Sandbox UI-handler
paths passed with save/reload continuation. Compilation passed; current Console
0 errors/0 warnings. Production Mansion and frozen POC scenes remain unchanged.


## Real Day 1 Morning implementation (October 29, 2026)

Day 1 Tranche 1 now replaces the default Sandbox systems-day panel with the real
Morning structure: abstract wake/coffee, free Marie agenda, Stay/newspaper versus
Leave/family seed (1), required Breakfast with a small topic variation (1), then
Social Duty versus Personal Life call (1). Existing exhaustion automatically enters
Afternoon 3/3; this tranche stops at day1.office.pending. No Afternoon content.
GameState, ActionCost and v3 save behavior are unchanged. Separate Morning slot
uses strict/runtime catalog checks and bounded authored-history validation.
Established roles/structure are distinguished from provisional writing, contacts,
family placeholders and relationship variations in Docs/DayOneMorning.md, which
contains exact flow, IDs, Personal-writing needs and the next-tranche boundary.
This is logical/playable scaffolding, not final scene presentation. Production
Mansion and frozen POC, art/vendor/packages/project settings remain untouched.
Earlier milestone sections and their test counts are historical.

## October 29, 2026 — Day 1 Tranche 2 Afternoon complete

Current stopping point supersedes the Morning-only endpoint above. Abstract
Prototype_Sandbox now continues: authored office obligation at Afternoon 3/3 ->
exactly one secretary / CEO Richard Hale / Acquisitions VP visit, cost 1 -> required
Board Meeting, cost 2 -> automatic Evening 3/3, day1.evening.pending. No Evening
content or environment construction. Charlie remains cousin/mentor, never CEO.

Secretary is light/delayable; Hale has newspaper-prepared/basic variants; VP opens
an unresolved multi-day opportunity. Board records provisional informed/listening/
concern first impressions, a distinct prepared-Hale payoff and explicit individual
knowledge/rapport. No universal competence meter. Save schema v3 unchanged; new
separate Afternoon slot validates complete Morning/Afternoon authored histories.
Failed loads preserve active state. See Docs/DayOneAfternoon.md for full ID inventory,
canonical/provisional distinction, writing needs and next-tranche scope.

Verification: 189/189 full EditMode tests (178+11), zero failures/skips/inconclusive;
312 live handler checks across 12 combinations, each including 2/3 save/reset/reload
and failed-load preservation; automatic Evening 3/3 and saved endpoint verified.
Clean compilation and final current Console 0 errors / 0 warnings. Initial test
runner attempt from pre-existing Play Mode failed; clean stopped-editor rerun passed.
Office/board/Evening UI visually inspected. Reset to Morning, Play Mode stopped and
verification slot restored/removed. Production Mansion and frozen POC SHA-256 match
Morning baseline, including metadata; Sandbox scene/meta unchanged too. No art,
vendor, package or project-setting changes. Evidence: Temp/DayOneAfternoon-*.
Next work: Evening logic only when requested; Personal writing and production
presentation remain unresolved. Commit title: Implement Day 1 afternoon logic.

## October 29, 2026 — Day 1 Tranche 3 Evening complete

This is the current stopping point; earlier tranche boundaries are historical.
Implemented only backend/content and abstract existing Sandbox runner controls.
Flow: Evening 3/3 / day1.evening.pending -> free invitation opening from Morning
Personal Life or Social Duty -> take (2, Completed) OR free defer (Deferred) ->
Mansion final Elena / Charlie / minor-relative choice (1) -> explicit remainder
wind-down when needed -> Night 3/3 -> free Victoria close -> day1.completed and
tutorial.day1.complete. No Day 2 content or freeform architecture implemented.

Victoria uses her witnessed board impression and explicit report of the final
Mansion conversation only. No private-call, newspaper or attendance omniscience,
no family/business meter. Unchosen Elena/Charlie interactions remain delayable.
All exact dialogue, minor-relative/former-girlfriend identities, club guest details
and reaction/rapport tuning remain provisional; established roles are preserved.
Personal writing still owns those decisions and future delayed-interaction content.
See Docs/DayOneEvening.md for exact flow, IDs, policy and implementation boundaries.

Save schema v3 unchanged. Separate Evening slot preserves old slots. Full bounded
history replay rejects invented/contradictory state; failed loads preserve session.
Verification: 206/206 full EditMode tests (189 baseline +17 focused cases), zero
failures/skips/inconclusive; all 12 live handler combinations pass 288 checks.
Every route exercises invitation save/load, post-decision mid-Evening reset/reload,
corrupt-load preservation, completion and completed-state reload. Actual invitation,
Mansion and completion panels visually inspected. Clean compilation; final current
Console ground truth 0 errors / 0 warnings (historical Pipeline buffer is separate).
Runner reset to Morning; Play Mode stopped; verification save restored/removed.
One initial new test assertion incorrectly expected provisional metadata to be a
Strict error; corrected to existing warning policy, then full suite passed.

Production Mansion SHA-256:
8FA6D5996D5FEAA8311D8A86A1D456F5B91CBA7792E2EC0DAD0EBC5955C3DF41
Frozen POC SHA-256:
AD62AFA9FCFE923C6F6637C2E0E22B708B7B4D6E39FDD0A256D8F85962460A4A
Both scenes and their metadata match f5fd6d65f3c127f8570fa3ffa137ed0e99083cec;
Sandbox scene/meta match too. No art/vendor/package/project-setting changes.
Evidence in ignored Temp: DayOneEvening-full-tests.json, -live.json,
-final-compile.json, -final-console.json, -protected.json; invitation/Mansion/
completion screenshots and walk-evening.cs. No Player build performed.
Commit title: Implement Day 1 evening logic.
