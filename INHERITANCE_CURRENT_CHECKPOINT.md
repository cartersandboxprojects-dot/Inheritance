# Inheritance current checkpoint

Stopping point: October 28, 2026. Backend quality/integration checkpoint on Unity
6000.3.27f1 (Unity 6.3 LTS). No new gameplay systems or scene integration.

## Current backend milestone

- GameState owns facts, relationships, independent character knowledge, opportunity
  progress, phone records, day/block, remaining capacity and story phase.
- Authored ActionCost: 3 units per block; Free/Fixed(n), ConsumeRemainder and
  HardTransition. Over-budget actions reject; exact exhaustion advances one block;
  Night rollover increments the day. Costs run before ordered effects.
- Opportunities support authored eligibility/block, independent acceptance/completion
  costs, competition, defer and explicit expiry. Accepted/Completed competitors block
  actions. No implicit scheduler or expiry tick.
- Phone records are Queued/Delivered/Read, with backend ID, sender and optional
  opportunity relation. Rendering is read-only; delivery and reads are explicit.
- Delayed consequences are authored checks of persistent state. The scheduling proof
  covers family visit versus business, delayed Elena thanks and a guarded later
  reaction. Knowledge never follows automatically from global truth.
- Save schema v3 persists capacity, opportunities and messages. V1 restores full
  capacity; v2 preserves partial capacity; both migrate with an empty inbox. Generic
  restore validates structure; authored invariants use the validation callback.

## Checkpoint result and verification

PASS after one narrow backend correction: uncosted Consequence previously mutated
live state sequentially. All consequences now use detached candidate/commit,
matching costed actions, opportunities and message reads. Exceptions leave live
state unchanged. Guard and cost semantics, effect order and public APIs are retained.

Two regression cases reproduced the defect before the fix (overflow and invalid
identifier after multi-category effects, including Night rollover). Existing tests
now verify Condition.All array isolation, positive later-reaction setup before
removing one prerequisite, and v2 migration with partial capacity.

Full unfiltered EditMode suite: **121/121 passed**, 0 failed/skipped/inconclusive
(119 baseline plus 2 regression cases). Script compilation completed with
failed=false, errors=[], compilationFailed=false. Final current Console ground
truth: 0 errors, 0 warnings. Pipeline retains historical import/helper logs; these
are not current Console entries. Evidence: ignored Temp/Checkpoint-full-tests.json
and Temp/Checkpoint-final-console.json. No additional live gameplay walkthrough or
player build was performed.

## Boundaries and next hardening

- Stable-ID hardening is the next technical-debt task: validate authored ID uniqueness
  and references (opportunities, competition targets, message IDs/senders/relations,
  facts, characters, relationship attributes, phase and Ink bindings). Detect renamed
  or unknown saved IDs and define aliases/migrations before changing ID spelling.
  No broad string-ID refactor was done here.
- Public GameState setters and Effect.Apply remain low-level authoring/bootstrap
  primitives. Player actions must use guarded definitions. Current dialogue callers
  use consequences; restrict mutation access as authoring expands.
- Generic restore cannot establish story history or mutual-exclusion invariants
  without definitions. Add content-specific validation alongside production content;
  do not infer knowledge from truth. Message metadata mismatch blocks delivery/read.
- Competition is directional: author both directions for mutual exclusion. Accepted
  commitments still check completion eligibility/block. Acceptance can spend the
  final unit and leave completion outside its window; author a broader window or
  explicit expiry deliberately. Queued messages with a narrow delivery window can
  remain queued if evaluation misses it. A guarded onRead can intentionally block
  reading. These policies need content validation, not a universal scheduler.
- One-shot consequences require authored persistent guards. Costs plus AdvanceTime
  effects can deliberately advance twice; avoid accidental double transitions.
- Candidate cloning is coherent at this scale; profile before optimizing. Authoring
  remains immutable C# definitions with copied arrays, no scene dependencies. Add
  authoring validation/tools when content volume justifies them.

## Commit anchors and environment

- Authored time costs: 6289c49 (Add authored time cost system).
- Opportunity/phone scheduling: d7d7dbe3ce85819d001327ba72ea620981e67d8f.
- This checkpoint: commit titled `Checkpoint backend gameplay systems`; resolve its
  exact hash with Git history (the commit contains this document).
- Narrative/save foundation: 63a7050bfb9f7868a98828b19dd8cb67ff21d0c6.

Production Assets/Scenes/Game/Mansion.unity remains a non-playable shell. Frozen
Assets/Scenes/Prototype/Prototype_Mansion_POC.unity remains the playable regression
reference; Prototype_Sandbox is the experiment lane. All three scenes, art, vendor
assets, packages and project settings are untouched by this checkpoint. Existing
Mansion/foliage qualification and storage policies remain in Docs/EnvironmentWorkflow.md.
See INHERITANCE_DEVELOPMENT_HANDOFF.md, Docs/PrototypeState.md and Docs/SaveLoad.md.


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

Verification (October 29, 2026, Unity 6000.3.27f1): full unfiltered EditMode suite
178/178 passed (171 baseline + 7 focused cases), zero failed/skipped/inconclusive.
Compile completed with failed=false, errors=[], compilationFailed=false. Final
current Console ground truth: 0 errors, 0 warnings. Historical Pipeline buffer
entries are not current Console errors. No Player build was performed.

Actual Sandbox Play Mode UI handlers passed all four first-choice/final-choice
combinations, 24 checks each (96 total), covering both Breakfast topics. Every route
saved/reset/reloaded identical full snapshots at Morning 2/3 and 1/3, then saved and
loaded the final Afternoon 3/3 endpoint. Corrupt load retained live state; validated
Save recovered the slot. Verification-created slots were removed, and the runner
was reset to a fresh Morning for the user. No physical WASD traversal was needed.
The actual Game panel was visually inspected; an opaque panel prevents proxy-label
overlap. The temporary background-run override was restored. Its newly generated
default configuration was backed up in ignored Temp and removed after provenance
and hash checks, leaving no project-settings changes.

Ignored evidence: Temp/DayOneMorning-full-tests.json, DayOneMorning-live.json,
DayOneMorning-final-compile.json, DayOneMorning-final-console.json and walk-morning.cs.
Protected scene SHA-256 values are identical before/after implementation:
- Production Mansion: 8FA6D5996D5FEAA8311D8A86A1D456F5B91CBA7792E2EC0DAD0EBC5955C3DF41
- Frozen POC: AD62AFA9FCFE923C6F6637C2E0E22B708B7B4D6E39FDD0A256D8F85962460A4A
Scene/meta Git comparisons also match the starting commit. Sandbox scene diff is
only the existing panel object's name, script GUID and class binding; no geometry.
No art/vendor/package/project-setting changes. Existing capacity/save source and
transition tests are unchanged. Commit title: Implement Day 1 morning logic.

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
