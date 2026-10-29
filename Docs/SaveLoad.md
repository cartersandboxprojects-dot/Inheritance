# Save/load milestone

Implemented October 27, 2026 on Unity 6000.3.27f1.

## Scope and controls

Save and Load buttons in the existing mansion prototype UI operate one local slot:
Application.persistentDataPath/funeral-night-v1.json. On this laptop that resolves to
C:/Users/josep/AppData/LocalLow/DefaultCompany/Inheritance/funeral-night-v1.json.

Save overwrites the slot. Restart resets the session without deleting the slot.
A new Play session starts fresh; Load explicitly restores the slot.
Successful Load closes the active Ink conversation and clears its transcript.
Approach an NPC to start fresh Ink flow against the loaded backend.
Failed Load preserves the previous state, conversation, and transcript and displays
an error. Save errors also display in the existing UI.

Only backend state persists: facts (including explicitly false entries),
relationships, per-character knowledge, opportunities, phone messages, day, time block, remaining capacity, and phase.
Player position, scene objects, transcript, and Ink execution position are excluded.

## Format and ownership

StateSave is a detached DTO with required Version, Day, Time, Phase, Facts,
Relationships, Knowledge, Opportunities and (for v3) Messages members. Version is now 3 and requires
TimeUnitsRemaining (1..3). Missing/null/out-of-range capacity fails on versions 2 and 3.
Version 1 saves migrate to a full block (3 units), because they recorded no partial
capacity. All other state and authored validation are preserved. Future versions fail.
The existing funeral-night-v1.json slot filename is retained to find older saves;
the JSON Version member determines schema. New writes always use version 3.
Dictionary/set entries use arrays of explicit records. Capture orders records
ordinally for deterministic output and never shares live mutable collections.

GameState and StatePersistence remain pure .NET in the existing engine-free
Inheritance.State assembly. No package or assembly dependency changes were needed.
DataContractJsonSerializer handles JSON escaping/parsing and required members.
A separate root-boundary check rejects concatenated objects/trailing data and
limits nesting to 64; saves are limited to 1 MiB.

Restore constructs a new state and rejects missing/null collections and records,
blank identifiers, duplicate keys, invalid enum values, and negative days.
FuneralNightSaveValidation separately checks current authored invariants: exclusive
choice facts, phase/time/day, matching Charlie opportunity and relationship effects,
and Victoria knowledge consistent with the selected event. Validation occurs
before Save and before the runner replaces its current state on Load.

Writes flush a unique temporary file beside the destination, then move it into a
new slot or replace an existing slot. A failed write/replace retains the previous
slot and cleans up the temporary file. This was verified on this Windows laptop;
other platforms and player builds were not tested.

## Verification

- Unity compilation passed with zero warnings/errors; final Console also had zero warnings/errors.
- Full EditMode suite: 60/60 passed, zero failed/skipped/inconclusive, including the 32 existing cases and 28 persistence
  cases. Covers generic category round trips, false facts/independent knowledge,
  detached snapshots, both authored branches before/after reporting, replay guards,
  initial-state eligibility, malformed/missing/oversized/trailing JSON, unsupported
  versions, duplicate/invalid records, contradictions, file create/overwrite,
  locked-file failure, and preservation of the previous slot.
- Live Prototype_Mansion Play Mode through the actual public handlers used by UI:
  early Victoria fallback; Elena and Charlie choices; Save while dialogue is open;
  restart then Load; no knowledge invented before reporting; correct Victoria line
  after reporting; Save/Load after reporting; repeats/opposite choices rejected.
- Both saved outcomes were also loaded across Play Mode exit/re-entry and retained
  knowledge and the matching Victoria reaction.
- Live corrupt, future-version, missing-file, and contradictory loads were rejected
  while preserving state and active dialogue; a valid slot loaded after failures.
- Full Game View capture inspected: Save/Load buttons, status text, business
  outcome, and Victoria's business line were visible. Camera-only capture omits
  OnGUI; ScreenCapture was used for UI inspection.
- Player placement and interaction/choice/save/load handlers were invoked through
  Pipeline. Physical button clicks/key movement were not simulated for this milestone.
- Original save-slot state was restored after verification. Editor left ready,
  outside Play Mode, with its background override restored. Temporary verification
  files/captures remain under ignored Temp, outside Assets.
- ShaderGraphSettings.asset was confirmed content-identical, restored before
  implementation, and normalized again after Unity regenerated the same LF noise.
  No actual shader settings changed.

No autosave, slot browser, general migration framework, cloud sync, or Ink-flow persistence
is included in this milestone.

Authored-time validation (October 28, 2026): full suite 81/81 passed, including
partial-capacity round trip, invalid/missing/null capacity rejection, version 1
compatibility, and existing Elena/Charlie/Victoria save/load regression cases.
Compilation succeeded; current Console had zero errors/warnings.

## Scheduling/phone schema v3 (October 28, 2026)

CurrentVersion is 3. Capture stores the ordered Messages array with stable message
Id, Sender, optional RelatedOpportunity, and Status (1 Queued, 2 Delivered, 3 Read).
Opportunities retain their original numeric values with Accepted=5 and Expired=6
appended. Every scheduling result/queue/read marker lives in GameState, is included
in detached snapshots, and survives atomic candidate commits and save/load.
Definitions, text, predicates, costs and competition rules remain content, not saves.

V1/v2 migrate to an empty inbox; v1 still gets 3 time units, v2 still requires valid
capacity. V3 requires a non-null Messages array, even if empty. It rejects null or
duplicate records, blank IDs/senders/optional relation IDs, invalid statuses (including
Absent in a stored record), missing required record members and invalid opportunity
enum values. Generic restore validates structure, not an authored content registry
or inferred story history. Message actions also require metadata matching their
definition. Content-specific cross-field invariants can use the existing validation
callback; funeral-night validation is unchanged.

After loading, rebuild definitions and explicitly evaluate eligible delivery where
the authored caller requires it. Delivered/read records reject requeue/redelivery;
queued records can deliver later. Loading and rendering never deliver implicitly.
There is no serialized universal event queue or pending-event simulator.

Scheduling validation: full Unity 6000.3.27f1 EditMode suite passed 119/119 cases
(81 baseline + 38 new), zero failed/skipped/inconclusive. Compilation completed
with failed=false and no errors; current Console had zero errors/warnings. Tests
cover eligibility/block guards, acceptance/completion costs, atomic failures,
competition, defer/expiry, delayed one-time delivery, free/guarded/failed reads,
reaction prerequisites, queued/accepted/delivered/read save-load continuation,
v1/v2 migration and invalid phone/opportunity records. Existing authored-time and
Elena/Charlie/Victoria regression tests passed. No runtime surface was needed;
Production Mansion, frozen POC, Sandbox, art, vendor, package and project settings
content were untouched. This tranche was verified through backend/EditMode tests,
not an additional live gameplay walkthrough.

## Backend quality checkpoint (October 28, 2026)

Schema remains v3; no format change. Full unfiltered EditMode suite: 121/121 passed,
zero failures/skips/inconclusive. Compilation passed and current Console was clear.
V2 migration coverage now starts with one remaining unit to prove capacity survives;
v1 restores three units. Both introduce an empty inbox. Existing JSON tests cover
missing v2 capacity and missing v3 Messages/required message members.

Restore validates structural states allowed by low-level GameState operations.
It has no content registry, so unknown IDs, directional competition conflicts or
impossible authored histories require the existing validation callback. Metadata
mismatch is rejected by message actions before delivery/read. Do not broaden the
funeral-specific validator into a global story rule or infer knowledge from truth.
Next hardening should validate stable IDs/references and define saved-ID aliases
and content migrations before renaming authored IDs. No broad ID refactor here.
All consequences now commit atomically, including uncosted effects; failed effects
cannot leak partial state into a later save. See INHERITANCE_CURRENT_CHECKPOINT.md
and the handoff entry titled Backend gameplay quality checkpoint for the full audit.


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
