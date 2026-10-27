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
No OR/NOT combinators, serialization, scheduler, history, automatic inference,
scene hookup, dialogue runner, or save/load are included.

Effects are sequential, not transactional: invalid identifiers or relationship/day
overflow can throw, and earlier effects are not rolled back. Definitions must be
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
outcome table, and verification. The current suite has 32 passing cases.
