# Funeral-night state prototype

`FuneralNightContent.CreateState()` creates a new session in `FuneralNight`, day 0,
`TimeBlock.Night`, with `Charlie_FuneralNightBusiness` available. Missing facts are
false, relationships are zero, knowledge is absent, and opportunities are unavailable.
Identifiers are case-sensitive. There is no static mutable state or scene dependency.

`GameState` separates world facts (bool), player-to-character relationship attributes
(int, keyed by character and attribute), character awareness (character/fact pairs),
opportunities (enum), broad time blocks with a relative day counter, and story phase
(content string). Resentment is the prototype relationship attribute; its range and
story effects belong to content, not the storage layer.

Prototype identifiers: `BRECKEN_COMFORTED_ELENA`,
`BRECKEN_MET_CHARLIE_FUNERAL_NIGHT`, `Elena`, `Victoria`, `Resentment`,
`Charlie_FuneralNightBusiness`, `FuneralNight`, `AfterFuneralNight`.

Opportunity states: Unavailable, Available, Missed, Deferred, Completed.
Time blocks: Morning, Afternoon, Evening, Night. `AdvanceTime()` advances one
block; Night wraps to Morning and increments Day. Phase changes do not advance time.

Writing truth never grants knowledge. `GrantKnowledge` explicitly records awareness
of an event, not a belief about its truth. No inference or propagation is performed.
Time does not automatically close opportunities, and relationships do not write facts.
The future scene/consequence caller must apply those effects explicitly.

Example sequence: set the comfort fact, modify Elena/Resentment, advance time,
explicitly grant Victoria knowledge when the story calls for it, then query phase,
truth, and knowledge separately when selecting a later scene branch.

Run `Inheritance.Tests.GameStateTests` in Unity Test Runner (EditMode), or Pipeline
`test run` with mode `editor` and filter `Inheritance.Tests.GameStateTests`.
These tests cover independence, defaults, day rollover, and the funeral-night sequence.

This is an in-memory model only. Scene hookup, save/load, automatic consequences,
dialogue eligibility evaluation, and future narrative systems are outside this step.
