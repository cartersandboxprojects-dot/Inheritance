# PROVISIONAL SANDBOX DAY

## Purpose and content status

Systems validation only. Every new line, event, task, message and character beat in
this tranche is AI-generated **PROVISIONAL / NON-CANONICAL** test content. Implementing
it does not accept it as Inheritance story. Elena, Charlie and Victoria are existing
character IDs used in a non-canonical systems exercise; their canonical arcs are not
changed. Personal Carter should refine or replace all motivations, tasks, wording,
phone timing, competition consequences and reactions before any production promotion.

Open `Assets/Scenes/Prototype/Prototype_Sandbox.unity` and enter Play Mode. Use the
scrollable panel at the upper left. Movement/proximity is not required for this
bounded exercise. The previous funeral runner is disabled only in Sandbox; the
frozen POC still retains its original runner. Mansion remains unchanged.

## Exact flow / two valid branches

Fresh state: day 0, Morning, 3/3 units, phase `sandbox.day.exercise`, no prior funeral
facts/knowledge. Talk Elena about sample boxes and Charlie about sample figures in
either order. Both are free, one-shot conversations; both unlock the competing offers.

| Action | Family path | Business path |
| --- | --- | --- |
| Morning commitment | Help Elena sort sample boxes, Fixed(2) | Check sample figures with Charlie, Fixed(3) |
| Immediate time | Morning, 1 unit remains | Exact exhaustion advances to Afternoon, 3 units |
| Competition | Charlie opportunity Missed | Elena opportunity Deferred |
| Knowledge | Elena knows the family task occurred | Charlie knows the business task occurred |
| Phone | Elena message queued, unreadable in Morning | Charlie message queued then delivered at post-action Afternoon evaluation |
| Morning boundary | Explicit Finish morning spends remaining unit; Elena message delivers | Already Afternoon |
| Afternoon | Read phone for free, then Finish afternoon (ConsumeRemainder) | Read phone for free, then Finish afternoon (ConsumeRemainder) |
| Evening | Free Victoria report recognizes helping Elena and missing Charlie | Free Victoria report recognizes helping Charlie and deferring Elena |
| End | Finish day consumes evening remainder, reaches Night and records completion | Same |

Reading can also happen in Evening: delivered records stay readable. A queued
message is delivered only at the explicit post-action/load evaluation boundary
while its chosen task is Completed, its outcome fact is true and it is Afternoon.
There is no real-time timer or automatic scheduler. The unchosen message is Absent.
A completed competitor prevents reoffering the deferred family task. No automatic
next-day continuation is authored; Night completion and Restart bound this exercise.

Victoria requires Evening, the correct Completed/Missed-or-Deferred opportunity
pair, true outcome fact, chosen character's knowledge, and the matching phone
message Read. The report explicitly grants Victoria knowledge and records a one-shot
reaction. World truth does not automatically grant awareness. End day requires that
report. Early/unread/repeated reactions fail without cost. Free interaction never
advances time; only authored commitment or explicit boundary costs spend capacity.

## Characters and stable IDs

Characters: existing legacy IDs `Elena`, `Charlie`, `Victoria` (no new characters).
All new IDs use the `sandbox.day.` namespace:

- Phase: `sandbox.day.exercise`.
- Facts: `greeted`, `briefed`, `family_helped`, `business_helped`, `reacted`, `finished`.
- Opportunities: `family_elena`, `business_charlie`.
- Messages: `phone_elena` (Elena / family opportunity), `phone_charlie` (Charlie / business opportunity).
- Bindings: `greet`, `brief`, `finish_morning`, `finish_afternoon`, `family_reaction`, `business_reaction`, `end`.

Each suffix above is prefixed by `sandbox.day.`. The source catalogs derive actual
references from the conditions/effects. Every catalog entry uses the existing
`ProvisionalReason` mechanism with an explicit non-canonical notice, including
bindings and the character usages. All references resolve in Strict and Runtime;
no validation rule was weakened. Expected provisional diagnostics are informational
catalog warnings, not Unity Console warnings. The notice also appears in source,
scene object name, UI header/transcript and every generated phone/dialogue line.

## Save/load

The panel has Save/Load and Restart controls. Its separate slot is
`Application.persistentDataPath/provisional-sandbox-day-v3.json`; it never overwrites
the funeral slot. Schema stays v3. Existing StatePersistence and the day catalog
validate before replacing the active state or writing a slot. Time/capacity, phase,
facts, knowledge, opportunity states and queued/delivered/read messages persist.
Definitions are rebuilt from code. Load evaluates delivery at the authored boundary
and replaces transient transcript text with a continuation prompt. Failed loads keep
the active session. Restart creates a fresh day and leaves the saved slot available.
This prototype does not add a save-history validator or universal scheduler.

## Implementation boundary

`SandboxDayContent` is immutable backend content. `SandboxDayRunner` is the small
Sandbox-only MonoBehaviour adapter and scrollable immediate-mode UI; its buttons
call public handlers also used for live verification. It refuses to run outside
the Sandbox scene. No new architecture, packages, inputs, assets or canonical Ink
content are needed. The old POC dependency scripts are unchanged.

## Verification - October 29, 2026

- Unity 6000.3.27f1 / Unity 6.3 LTS. Full unfiltered EditMode suite: **171/171 passed**
  (167 baseline + four cases), 0 failed/skipped/inconclusive. Focused cases exercise
  both full-day branches, disk save/load at different capacities/message states,
  delayed delivery, evening reading, one-shot reactions/end, knowledge/truth and
  competitor prerequisites, and all three catalog validation modes.
- Final compilation: failed=false, errors=[], compilationFailed=false.
- Both live Play Mode UI-handler walkthroughs passed: family 19 assertions, business
  18 assertions. Each saved after a meaningful choice, reset/reloaded, compared the
  entire persisted snapshot, continued through its distinct phone/reaction, and
  completed at Night. Business also verified corrupt-load preservation and recovery.
  Physical keys were not simulated; the actual panel handlers were invoked.
- Final rendered panel inspected for readability. A temporary editor background-run
  override enabled continuing frames for the final walkthrough; it was restored.
- Current Unity Console: 0 errors / 0 warnings. Historical Pipeline buffer entries
  are distinct from current Console counts. No Player build was performed.
- Ignored evidence: `Temp/SandboxDay-full-tests.json`, `SandboxDay-final-compile.json`,
  `SandboxDay-live-family.json`, `SandboxDay-live-business.json`, screenshots and
  final Console record. Verification-created save slots were removed.
- Protected Production and POC scene files and their meta files match the starting
  commit byte-for-byte; no changes to art/vendor/packages/project settings.

Personal Carter's next step is editorial: choose the real day placement, motivations,
commitments, dialogue, costs, delivery window, deferred-task meaning and later
reaction. Keep those decisions provisional until deliberately accepted. This loop
provides testable structure, not a canonical story proposal.

## Default Sandbox panel superseded

The real Day 1 Morning tranche now owns the default Prototype_Sandbox panel.
The systems-day content/runner and tests remain intact for regression purposes.
This guide describes the earlier systems proof. See DayOneMorning.md for the
current default interaction flow; its save slot is separate.
