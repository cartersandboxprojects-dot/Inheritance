# Development-to-Personal Work handoff

Repository: `C:/Users/josep/Inheritance`. Inspected baseline: `2a08b17ad3060c0daf83654077f87db58f35cc75` (main), October 29, 2026. Documentation only; no story acceptance or implementation change.

## Authority and first inspection

Read `Docs/ProvisionalSandboxDay.md`, `Docs/PrototypeState.md` (including its latest extensions), `Docs/StableIdentifiers.md`, then `Docs/MansionEntryFoundation.md` and `Docs/EnvironmentWorkflow.md`. Inspect the source definitions below before summarizing behavior. Current repository code/assets establish implemented behavior; docs explain intent and recorded verification. Development chat is not a source of truth. Milestone docs contain historical sections/test counts; later extensions supersede earlier limitations. Personal's summary is descriptive and story-facing, not implementation-authoritative.

## Practical source map

All paths are repository-relative.

| Files | Inspect for |
| --- | --- |
| `Docs/PrototypeState.md` | Architecture, authored costs, opportunities, phone, truth/knowledge and consequence milestones. |
| `Assets/Scripts/State/GameState.cs` | Authoritative facts/tags, relationships, independent knowledge, phase, day/block and three-unit capacity. |
| `Assets/Scripts/State/Consequence.cs` | Condition/Effect/Consequence definitions; composed guards and atomic state changes, explicit caller evaluation. |
| `Docs/StableIdentifiers.md` | Current ID conventions, validation scopes/modes, legacy spellings and reviewed saved-ID rename/removal policy. |
| `Assets/Scripts/State/ContentCatalog.cs`; `Assets/Scripts/State/AuthoredContentCatalogs.cs` | Actual catalog/reference validation, migration rules and funeral/scheduling scopes; Sandbox owns its catalog separately. |
| `Assets/Scripts/State/ActionCost.cs` | Free/Fixed costs, ConsumeRemainder and HardTransition; execution/capacity handling is in GameState. |
| `Assets/Scripts/State/OpportunityDefinition.cs` | Eligibility, optional block windows, acceptance/completion costs, directional competition, defer/explicit expiry. |
| `Assets/Scripts/State/PhoneMessage.cs` | MessageDefinition plus queued/delivered/read records, guarded delivery and explicit reads. |
| `Assets/Scripts/State/FuneralNightContent.cs`; `Assets/Scripts/State/FuneralNightConsequences.cs` | Existing authored vocabulary and Elena/Charlie choices, explicit information transfer and Victoria branch predicates. |
| `Assets/Scripts/State/SchedulingProofContent.cs` | Test-only competing commitments, delayed thanks and state/knowledge/read-gated later reaction; no scene hookup. |
| `Docs/SaveLoad.md`; `Assets/Scripts/State/StatePersistence.cs`; `Assets/Scripts/State/FuneralNightSaveValidation.cs` | v3 snapshots, v1/v2 migration, catalog-aware loading and funeral-specific invariants; definitions/text are rebuilt, not saved. |
| `Docs/InkPrototype.md`; `Assets/Scripts/Dialogue/InkPrototypeRunner.cs`; `Assets/Scripts/Dialogue/ElenaInkConversation.cs` | Existing scene/Ink adapter contracts, guarded actions and save/load caller boundary. |
| `Docs/ProvisionalSandboxDay.md`; `Assets/Scripts/State/SandboxDayContent.cs`; `Assets/Scripts/Dialogue/SandboxDayRunner.cs` | Exact provisional flow/IDs/costs, catalog, explicit evaluation, Sandbox UI and separate save slot. |
| `Assets/Tests/Editor/GameStateTests.cs`; `Assets/Tests/Editor/ConsequenceTests.cs`; `Assets/Tests/Editor/ActionCostTests.cs` | Executable evidence for state separation, guards, atomic consequences and authored time budgets. |
| `Assets/Tests/Editor/SchedulingTests.cs`; `Assets/Tests/Editor/PersistenceTests.cs`; `Assets/Tests/Editor/ContentIdentifierTests.cs`; `Assets/Tests/Editor/SandboxDayTests.cs` | Executable competition, delayed delivery/reaction, persistence/migration/ID validation and both complete provisional paths. |
| `Docs/EnvironmentWorkflow.md` | Scene ownership, one physical world with changing state, Sandbox qualification and deliberate production promotion. |
| `Docs/MansionEntryFoundation.md` | Exact built dimensions/axes, expansion conventions, provisional architecture and human-scale measurements/limits. |
| `Docs/MansionQualification.md` | Selected vendor module/pivot/collider/render qualification and limits; not a finished mansion certification. |
| `Assets/Inheritance/Environment/Sandbox/Mansion_Entry_Foundation.prefab` | Current owned modular entry assembly; its Sandbox placement is documented in the foundation guide. |
| `Assets/Inheritance/Environment/Sandbox/Reference/HumanScaleReference_Adult.prefab`; `Assets/Inheritance/Environment/Sandbox/Reference/HumanScaleReference_Short.prefab` | 1.800 m/1.680 m neutral visual rulers, not gameplay characters. |

## Implementation boundaries

- `Assets/Scenes/Prototype/Prototype_Mansion_POC.unity` is the frozen regression/reference scene. Shared dependencies can evolve; freezing the scene does not freeze every dependency.
- `Assets/Scenes/Prototype/Prototype_Sandbox.unity` is the experimental/story-systems environment, containing the entry seed and provisional playable day.
- `Assets/Scenes/Game/Mansion.unity` remains Production: a shell, not fully built story space. Sandbox architecture has not been promoted into it.
- The playable Sandbox Day is explicitly **PROVISIONAL / NON-CANONICAL**, proving mechanics through a panel rather than completed rooms, character performances or event choreography.
- Personal may propose story, character, spatial and pacing changes. Development/repo remains authoritative for implemented IDs, systems and assets; proposals require deliberate implementation/promotion.

## Design doctrine and implemented evidence

- **State explains why a beat is ready.** Inspect Condition.Fact/Phase/Knowledge/Opportunity/Message in `Consequence.cs` and the composed guards in `SandboxDayContent.cs`; time alone is insufficient for its phone/reaction beats.
- **Time primarily supplies availability, pacing, scarcity and cost.** `GameState.cs`, `ActionCost.cs` and `OpportunityDefinition.cs` separate budgets/windows from state eligibility. Interaction does not automatically spend time: Sandbox Greet/Brief/reactions and ordinary phone reads are free; commitments and explicit block boundaries declare costs.
- **Different player paces must remain possible.** Phase/fact/knowledge predicates can stay valid across day rollover, with appropriate authored availability windows. An early-phase event may occur several in-game days later if its state still makes sense. This is writing guidance supported by separate phase/day/state primitives, not a completed multi-day story system. The bounded Sandbox sample uses Morning/Afternoon/Evening guards and ends at Night; it does not implement a Day-5 continuation or automatic rescheduling. Personal should challenge narrow windows rather than assume they are canonical causality.
- **World truth and character knowledge are distinct.** `GameState.cs` stores them independently; funeral consequences explicitly inform Victoria, and Sandbox reactions require the chosen character's knowledge before explicitly granting Victoria knowledge. No automatic truth-to-awareness propagation.
- **Delayed consequences usually read persistent state written earlier.** Inspect SchedulingProofContent and SandboxDayContent.Evaluate/reaction guards, plus PhoneMessage.TryDeliver/TryRead. Callers explicitly evaluate delivery at action/load boundaries. This is not a universal pending-event scheduler, automatic expiry tick or automatic propagation system. One-shot guards and viable delivery/completion windows remain authored responsibilities.

## Mansion spatial context: current conventions versus provisional design

Use `Docs/MansionEntryFoundation.md` as the detailed spatial source and inspect the prefab/scene for actual geometry. Current retained foundation conventions are a 1 m placement grid, 4 m principal bays, 2 x 2 m floor tiles, unit module scale/vendor pivots and 4 m floor spacing. The built core is 24 x 12 m; open 4 x 12 m wing stubs give a 32 x 12 m seed. Floors are Y=0/4, next datum Y=8; the Sandbox assembly sits at (-30,0,-30).

The central portico/entry axis runs front (-Z) to rear (+Z); two guide walls leave 7.8 m clear hall width and reserve transverse circulation at Z=6..10. Extend wings at local X=-16/+16; rear Z=12 remains open. Remove the named provisional cap for vertical extension at Y=8, then Y=12/16 for eventual four/five-story capacity. Stair/elevator cores, upper openings and final room allocation are uncommitted; the larger estate program is deferred, not built.

Treat the measured foundation, module conventions and protected scene boundaries as the current implementation baseline, not an irrevocable architectural story lock. No completed mansion plan is locked. Cap/trim, glazing, door leaves, finishes, roof, wing lengths, vertical cores, furnishings and room program remain provisional/deferred; spatial revisions should be proposed to Development. Human-scale evidence uses 1.80/1.68 m rulers, 3.8 m overhead and 1.86 m clear between provisional door leaves. Recorded camera/capsule checks support the entry's generous scale; they do not qualify final controllers, stairs, animated doors or a furnished estate.

## Personal Work Deliverable

Inspect the referenced repository files directly, then produce a concise AI-readable summary for Personal Project Chat covering:

- What current systems support and how choices/consequences become persistent facts, relationships, opportunities and phase changes.
- How authored costs, competing commitments and availability create time/opportunity pressure without charging every interaction.
- How truth, individual knowledge, messages and explicit later evaluation support delayed reactions.
- How the central mansion entry is structured, measured and intended to expand, distinguishing built space from deferred program.
- Which Sandbox Day elements are mechanical placeholders: sample boxes/figures, panel conversations, branch competition, phone wording/windows, Victoria report and explicit block-ending buttons.
- What Personal Carter may rewrite for real Day 1: motivations, dialogue, commitments, room use, scene flow/choreography, side stories, nonlinear timing, costs, missed/deferred meanings and later reactions. Propose required implementation changes explicitly; do not present them as already built or edit canonical implementation to fit the summary.

Keep the summary descriptive and story-facing. Separate implemented capability, non-canonical demonstration and proposed story decisions; do not turn this handoff into acceptance of the provisional story.


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
