# REAL DAY 1 / TRANCHE 1: MORNING LOGIC

## Implemented mechanics and exact flow

This is the first real Day 1 structure, distinct from the previous non-canonical
Sandbox systems day. It is logical/playable scaffolding, not final scene presentation.
The tldraw board `Inheritance — Day 1 Flow` (MlolT3EcT5nJtpLpbNl58) supplies the
structure. Its old “CEO call with Charlie” wording is superseded by the corrected
roles below. The board was read without edits.

Open Prototype_Sandbox and enter Play Mode. Its existing panel object now runs
DayOneMorningRunner; old SandboxDayContent/Runner and their tests remain as systems
proof sources, no longer the default panel. The frozen funeral POC is unchanged.

1. Day 1, Morning, 3/3: Brecken wakes in abstract Master Suite context.
2. Household staff delivers coffee for free. Marie reads the agenda for free.
   Coffee precedes agenda. Marie is a recurring aide/chief-of-staff-like character.
   Charlie is Brecken's older cousin and business mentor/challenger. CEO is Richard Hale.
   Agenda records the required later office obligation and Brecken's awareness.
3. First choice, Fixed(1), Morning 2/3:
   - Stay: coffee/newspaper, funeral coverage and stock-drop knowledge for Brecken,
     plus preparation/context state reserved for later Richard Hale/board content.
   - Leave room: brief Name[1] family conversation, +1 local rapport, shared seed
     knowledge. Optional small side-story seed; no major loss if missed.
4. Required Family Breakfast, Fixed(1), Morning 1/3. Participants: Brecken, Elena,
   Charlie, Victoria, Name[1], Name[2], Name[3]. Choose one minimal topic inside this
   single scene: offer Elena support or ask Charlie about business expectations.
   Both complete Breakfast, record the selected topic and grant Brecken/addressed
   character knowledge; +1 rapport with Elena or Charlie respectively.
   No route skips or repeats Breakfast, and both routes reach both final choices.
5. Final choice, Fixed(1), exhausts Morning:
   - Social Duty: one provisional funeral follow-up call with Adrian Kessler.
   - Personal Life: call provisional former-girlfriend/pre-inheritance-life contact.
   Both record outcome, +1 contact rapport and shared knowledge of the call.
6. Existing exact-exhaustion behavior automatically advances to Afternoon, 3/3,
   day still 1. Phase becomes day1.office.pending; Morning completion and office
   obligation persist. Stop here. No Afternoon interaction, travel, office or board
   content is implemented. ReadyForOffice requires authored phase, Afternoon,
   Morning completion, required office obligation and Breakfast completion together.

Capacity is 3 -> 2 -> 1 -> exhaustion -> Afternoon 3. Zero is not stored. GameState,
ActionCost, v3 persistence contract and existing transition tests are unchanged.
UI display, Save/Load/Reset and rejected/repeated actions consume no capacity.
No universal scheduler or family/business morality meter is introduced. Local rapport
belongs to an individual relationship; it is not a cross-family/business score.

## IDs and written state

New IDs use lowercase dot/underscore conventions. Existing Elena, Charlie, Victoria
retain their declared legacy character IDs. No saved ID is renamed.

All fact suffixes in this table use `day1.morning.` unless written in full:

| Facts | When written | Knowledge / relationships |
| --- | --- | --- |
| funeral_coverage, stock_drop | Objective starting premises on BOTH routes | Brecken learns only on Stay; Richard/Victoria do not automatically learn |
| coffee_delivered | Staff coffee interaction | No inferred character awareness |
| agenda_read; day1.office.required | Marie agenda | Brecken learns office obligation |
| stayed, newspaper_context | Stay | Brecken learns coverage, stock drop, context |
| left_room, family_seed | Leave | Brecken and provisional.family.name1 learn seed; Name[1] rapport +1 |
| breakfast_completed; breakfast_support OR breakfast_business | Required Breakfast topic | Brecken and Elena OR Charlie learn topic; addressed character rapport +1 |
| social_call OR personal_call; completed | Final call | Brecken and adrian.kessler OR provisional.contact.former_girlfriend learn call; contact rapport +1 |

Phases: `day1.morning.wake`, `day1.morning.first_choice`, `day1.morning.breakfast`,
`day1.morning.final_choice`, `day1.office.pending`. Phase progression provides
one-shot/ordering guards; time alone never creates the story beat.

New characters: `brecken`, `marie`, `richard.hale`, `adrian.kessler`,
`provisional.family.name1`, `provisional.family.name2`, `provisional.family.name3`,
`provisional.contact.former_girlfriend`. Display names for unresolved family remain
Name[1..3]. Attribute: `relationship.rapport`.

Binding IDs use `day1.morning.action.` plus `deliver_coffee`, `read_agenda`, `stay`,
`leave`, `breakfast_support`, `breakfast_business`, `social_duty`, `personal_life`.
The eligibility binding is `day1.morning.ready_for_office`.
No opportunities/messages are needed for this bounded flow; calls are completed
authored actions rather than fabricated incoming phone records.

## Canonical basis versus provisional story content

Established basis: the named character roles above, required Breakfast, selected
Morning structure/costs, separate world truth/knowledge, and persistent consequences.
The funeral coverage and stock drop are supplied narrative premises; no specific
stock price, company performance, article quotation, family history or motives are invented.

Provisional/non-canonical: all minimal wording, Breakfast topic/outcome variations,
rapport amounts/interpretation, Name[1..3] identities, optional family side-story seed,
Adrian's use as this call recipient, and the former girlfriend's identity/content.
She is explicitly not equated with Alex Wynn. No second social contact is invented.
New facts/variations, rapport, authored action bindings and unresolved characters
carry existing ProvisionalReason metadata. All references resolve in Development,
Strict and Runtime; strict/runtime validation is not relaxed. Metadata diagnostics
are informational content warnings, not Unity Console warnings.

## Save/load

Separate slot: Application.persistentDataPath/day1-morning-v3.json. Existing funeral
and systems-day slots are untouched. StatePersistence v3 persists stage, day/block,
capacity, both choices, world premises, knowledge, family seed, Breakfast topic,
relationships, final outcomes and office obligation. Definitions are rebuilt from code.
Both save and load use the runtime catalog plus DayOneMorningContent.Validate.
This bounded history validator replays recorded one-shot actions from a fresh
Morning and compares the whole snapshot. Skips, contradictory choices, wrong
capacity/day/phase, fabricated knowledge or relationships and unknown IDs reject.
Load replaces the live state only after successful validation. Reset retains the
slot for continuation. Transcript is transient; persisted stage/outcomes drive UI.
The exact replay validator is intentionally scoped to this tranche; next content
must deliberately extend/replace its accepted histories and slot policy.

## Unresolved Personal-writing needs

Write Marie's agenda in her recurring character voice; staff/coffee lines; newspaper
wording without inventing prices; Name[1..3] identities and family connection;
the optional family seed; richer Breakfast exchanges for all participants;
calibrated relationship changes; Adrian's funeral follow-up; former girlfriend
identity, motives and history. These are writing needs, not accepted new canon.

## Next tranche / Afternoon

Implement required office arrival and board sequence through authored state/obligation
guards plus Afternoon phase. Decide preparation payoff from Brecken's newspaper
knowledge; implement discretionary office interactions, meeting choices, costs,
and subsequent callbacks later. This tranche adds no automatic office entry,
board meeting, universal scheduler, architecture, manual traversal or art assets.
After Day 1, progression should be state-driven rather than calendar-driven.

## Verification

Verification results are recorded in the current checkpoint after the full EditMode
suite, compile, Console check and live Sandbox handler walkthroughs complete.

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
