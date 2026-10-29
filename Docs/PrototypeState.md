# Funeral-night state and consequence prototype

`FuneralNightContent.CreateState()` starts in `FuneralNight`, day 0, Evening,
with `Charlie_FuneralNightBusiness` Available. Missing facts are false,
relationships zero, knowledge absent, and opportunities Unavailable.
Identifiers are case-sensitive. `GameState` remains independent of authored content.

## Generic API

`Condition.IsMet(state)` reads state without changing it. Factory operations:
- `Fact(id, expected = true)`
- `Knowledge(character, fact, expected = true)`
- `RelationshipAtMost(character, attribute, maximum)` (inclusive integer comparison)
- `Opportunity(id, expected)`
- `Time(expected)`
- `Phase(expected)`
- `All(params Condition[])`: short-circuit AND; an empty group is true.

`Effect.Apply(state)` operations:
- `SetFact(id, value)`
- `ModifyRelationship(character, attribute, delta)`
- `GrantKnowledge(character, fact)`
- `SetOpportunity(id, value)`
- `AdvanceTime()` (one block; Night rolls to Morning and increments Day)
- `SetPhase(id)`

`Consequence(condition, params Effect[])` copies its definitions and exposes
`TryApply(state)`: evaluate once, return false without effects if ineligible,
otherwise apply effects in declaration order and return true. Conditions/effects
are immutable delegate-backed definitions authored in C#, with no Unity dependency.
The original consequence milestone included no OR/NOT combinators, serialization, scheduler, history, automatic inference,
scene hookup, dialogue runner, or save/load are included.

All consequences use detached candidate/commit, including those without an ActionCost.
Invalid identifiers or relationship/day overflow throw without partial live mutation. Definitions must be
valid before execution; identifier validation is delegated to GameState operations.
There is no generic one-shot tracking. Content guards must prevent unwanted repeats.
Definition arrays are copied and null entries rejected. Definitions may be shared
between sessions; all mutable state stays in the supplied GameState.

## Authored proof

`FuneralNightConsequences.ChooseElena` requires FuneralNight, Evening, Charlie's
opportunity Available, and both existing event facts false. It sets
`BRECKEN_COMFORTED_ELENA` true, sets `BRECKEN_MET_CHARLIE_FUNERAL_NIGHT` false,
subtracts 1 from Elena/Resentment, marks `Charlie_FuneralNightBusiness` Missed,
advances to Night, and sets `AfterFuneralNight`. Victoria remains unaware.
The changed phase/time/opportunity/fact guards reject a repeat choice.

Later, the caller explicitly invokes `InformVictoria.TryApply(state)` when the
information should travel. Its conditions require the comfort fact, AfterFuneralNight,
and Victoria not yet knowing it; its sole effect grants that knowledge.
`VictoriaBranch.IsMet(state)` requires the comfort fact AND Victoria knowing that
same fact AND AfterFuneralNight. Other facts, characters' knowledge, opportunities,
and relationships cannot substitute for these requirements.

Example caller sequence:
1. Create a state with `FuneralNightContent.CreateState()`.
2. Call `FuneralNightConsequences.ChooseElena.TryApply(state)` for the player's choice.
3. VictoriaBranch is false until the later InformVictoria consequence is applied.
4. Apply InformVictoria at the authored information event; VictoriaBranch is now true.

Truth and awareness remain independent. Knowing a fact does not make it true,
and changing world truth never grants knowledge. Relationships are player-to-character
integer attributes; content owns their interpretation and bounds.

Run EditMode tests with Pipeline `test run`, mode `editor`, filter `Inheritance.Tests`.
GameStateTests covers storage separation/defaults/rollover; ConsequenceTests covers
all condition categories, AND composition, immediate effects, rejected choices,
repeat guards, propagation timing, each branch prerequisite, unrelated state, and
immutable definition arrays.

## Victoria playable extension (October 27, 2026)

The initial authored proof above described the Elena-only milestone. The current
prototype adds ChooseCharlie, InformVictoriaAboutCharlie, and VictoriaBusinessBranch.
Both choices share exclusive fact/time/phase/opportunity guards. Charlie records
BreckenMetCharlie, completes CharlieBusiness, adds 1 Elena resentment and subtracts
1 Charlie resentment, then advances to Night/AfterFuneralNight. Both Victoria
predicates and report consequences also require the opposite event false.
The scene's explicit information event is Brecken reporting his meeting when
talking to Victoria. See InkPrototype.md for complete current logic, controls,
outcome table, and verification. The original playable extension had 32 passing cases; see the authored time extension below for current coverage.

## Save/load extension (October 27, 2026)

GameState now exposes detached Capture/Restore snapshots. StatePersistence writes
versioned JSON and restores all backend state without Unity or Ink dependencies.
FuneralNightSaveValidation owns the current prototype's authored consistency rules.
See SaveLoad.md for the format, single-slot UI, error behavior, and verification.
## Authored time costs (October 28, 2026)

Interaction != time advancement. GameState owns TimeUnitsRemaining, initially 3,
and the constant TimeUnitsPerBlock = 3 for every coarse block. No calendar or minute clock.
ActionCost is immutable content: Fixed(0)/Free leaves time unchanged; Fixed(n)
spends n units, rejecting negative definitions and returning false for over-budget
actions. Exact exhaustion advances exactly one block and restores 3 units. No
overflow cost carries into another block. Capacity is always 1..3 in live state.
ConsumeRemainder explicitly ends the block regardless of its remaining capacity.
HardTransition explicitly forces the next block for sleep/travel/event boundaries.
Both reset capacity; Night -> Morning increments Day with checked overflow.
AdvanceTime remains the explicit low-level transition API and also resets capacity.
Narrative importance never implies a time cost.

Consequence(condition, cost, effects) checks guards and affordability, applies cost
and ordered effects to a detached candidate, then commits into the authoritative
GameState only on success. Rejection returns false; invalid effects/overflow throw
without changing live state. The quality checkpoint extends this atomic path to uncosted consequences too. Costs run before
effects. Content should declare one cost and avoid additional AdvanceTime effects
unless multiple transitions are intentional.

OpportunityDefinition(id, cost, condition, effects) provides CanStart/TryComplete:
requires Available plus authored conditions and budget, then applies cost/effects
and marks Completed atomically. Definitions are content, not save state. Rebuild
them from authored code after load. No automatic scheduling or expiry is implied;
existing explicit opportunity states/guards remain. Ordinary conversations can use
free consequences directly without becoming opportunities.

ChooseElena and ChooseCharlie now declare ActionCost.HardTransition instead of
an AdvanceTime effect. Both still move Evening -> Night/AfterFuneralNight exactly
once, preserving facts, relationships, Charlie opportunity, and Victoria knowledge
guards. Victoria reporting has no time effect. See ActionCostTests for budget,
atomic failure, rollover, opportunities, persistence, and both funeral branches.

Validation: full Unity 6000.3.27f1 EditMode suite passed 81/81 cases (60 existing +
21 new), zero failed/skipped/inconclusive. Script compilation completed with
failed=false, no errors; current Console had zero errors and warnings. No scenes,
art, vendor assets, packages, or project settings changed.

## Opportunity scheduling and phone proof (October 28, 2026)

GameState remains authoritative. OpportunityState retains numeric values 0..4 and
adds Accepted=5 and Expired=6. OpportunityDefinition is now in its own file. The
original constructor/CanStart/TryComplete API is preserved: a direct completion
from Available pays Cost. The extended constructor adds optional TimeBlock,
AcceptanceCost, and copied OpportunityCompetition definitions. Eligibility is
checked at offer, acceptance, and completion; completion of an accepted commitment
still requires the authored eligibility/block. Content can make eligibility broader
for commitments spanning blocks. Cost is the completion cost; AcceptanceCost is
independent, free by default. CanStart retains its legacy completion-budget meaning.

TryMakeAvailable offers Unavailable/Deferred opportunities only when eligible.
TryAccept locks the commitment; TryComplete accepts Available or Accepted and is
one-shot. Both validate eligibility, budget and competitors before working on a
detached candidate, then commit cost, competition, effects and own outcome together.
Accepted/Completed competitors reject an action. Nonterminal competitors become
Missed or Deferred as authored; terminal results are not overwritten. Author mutual
competition on both definitions for mutual exclusion. GameState setters remain
low-level primitives; callers should use definitions for player actions.
TryDefer is a free explicit action on Available. TryExpire uses a caller-authored
predicate for Available/Deferred/Accepted. There is no automatic expiry tick.
Missed/Completed/Expired never reopen through TryMakeAvailable.

PhoneMessage is an immutable view of backend state: Id, SenderId, optional
RelatedOpportunityId and Queued/Delivered/Read status (Absent means no record).
MessageDefinition owns stable IDs, text, delivery eligibility and optional onRead
Consequence; definitions are rebuilt from code after load. TryQueue is idempotent,
TryDeliver checks the queued record metadata and authored predicate before the
one-time transition. Neither rendering Text nor GetMessage has effects. TryRead
is explicit, requires Delivered, and atomically applies an optional guarded
consequence before marking Read. A rejected/throwing consequence leaves everything
unchanged. Reading is free unless content explicitly authors a cost in onRead.
Condition.Message composes with existing opportunity, fact, time and knowledge
conditions; an exact Delivered check ceases matching once Read, by design.

SchedulingProofContent is test-only authored content with no scene connection:
- Phase SchedulingProof, initially Afternoon. Both commitments become Available.
- proof.family.visit_elena: free acceptance, ConsumeRemainder completion. Acceptance
  or direct completion marks proof.business.charlie_call Missed. Completion records
  proof.family.visited and grants Elena knowledge of that visit, ending Afternoon.
- proof.business.charlie_call: free acceptance, ConsumeRemainder completion. It
  defers the family visit; completed/accepted business prevents reoffering it.
- proof.message.elena_thanks is queued initially, linked to the family visit, sender
  Elena: "Thank you for spending the afternoon with me." It delivers only in Evening
  in the proof phase with the family visit Completed and the visit fact true.
- LaterReaction requires Evening/proof phase, family Completed, business Missed,
  the message Read, visit fact true and Elena knowing it. This represents Elena's
  later response after the player has read her thanks. React sets proof.elena.reacted
  once using an explicit fact guard. The sample read itself has no world effects.

Caller sequence: CreateState -> Family.TryAccept -> Family.TryComplete -> Evaluate
-> Thanks.TryRead -> React.TryApply. Direct TryComplete also works. Evaluate offers
eligible commitments and attempts queue/delivery at explicit action/load boundaries.
It is safe to repeat. No message delivers merely because time changes; callers
must evaluate at the authored boundary. The sample Evening window is intentional:
other content can author a wider condition if delivery should survive that window.

Delayed consequences remain authored persistent state-read/write chains, NOT a
universal pending-event simulator. There is no universal scheduler, automatic event
propagation, conversation-to-opportunity conversion or scene/visual integration.
FuneralNightContent, funeral consequences, Ink and all scenes remain unchanged.

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

At d7d7dbe the 119-test baseline had one atomicity gap in uncosted Consequence.
The checkpoint unifies candidate/commit for every consequence, with two regressions
proved failing before the fix. Full unfiltered EditMode suite: 121/121 passed,
zero failed/skipped/inconclusive; compilation passed and current Console zero
warnings/errors. Existing array-isolation, reaction-prerequisite and partial-v2
migration tests were strengthened. No gameplay systems or protected assets changed.

GameState setters/Effect.Apply are low-level primitives, not guarded player actions.
Definitions own transition rules; structural save validation does not prove authored
history. Current dialogue mutations use consequences. No truth-to-knowledge leakage
was found. Directional competition, completion windows after acceptance, delivery
windows, read guards and one-shot guards remain content responsibilities. Next debt
is stable-ID uniqueness/reference validation, saved-ID rename/alias policy and
content-specific invariants. C# definitions remain sufficient for this proof; add
validation/tooling as content expands. See the current checkpoint for detailed risks.


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
