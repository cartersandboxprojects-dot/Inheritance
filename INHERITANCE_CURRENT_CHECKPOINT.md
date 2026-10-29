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
