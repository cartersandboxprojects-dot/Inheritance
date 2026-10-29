# Day 1 Tranche 3: Evening Logic

## Implemented flow and capacity

Morning and Afternoon are unchanged. Board completion enters Evening 3/3 at
`day1.evening.pending`. Begin Evening is free, requires the completed board and
Morning contact outcome, and makes exactly one conditional invitation Available.
Personal call opens the former-girlfriend encounter; Social Duty opens Adrian's
private-club drinks. All locations are abstract panel labels.

Take completes the opportunity for Fixed(2): Evening 3 -> 1, then Mansion.
Defer is free: opportunity becomes Deferred, Evening remains 3, then Mansion.
Deferral is the authored skip/decline-for-tonight behavior; no Declined/Missed
state, deadline, scheduler, implicit expiry or future resolution is invented.
A deferred opportunity persists unresolved; no reopening route is authored yet.
Unlocking never forces attendance. The other invitation remains Unavailable.

At Mansion, choose exactly one Fixed(1) conversation:
- Elena: connection/reassurance after Brecken's return and their parents' deaths.
- Charlie: helpful mentor, business advice/review; no villain reveal or moral penalty.
- Minor relative: light optional texture, provisional identity, no skip penalty.

Each chosen character gains rapport +1 and shares knowledge of that conversation
with Brecken. Unchosen Elena/Charlie interactions get explicit delayable facts,
not permanent missed outcomes; subsequent continuation needs authored writing.

With attendance, final choice consumes the last Evening unit and advances to
Night 3/3. Without attendance, final choice spends one unit (3 -> 2), then the
existing ConsumeRemainder cost explicitly winds down unused Evening capacity to
Night 3/3. This compound final handler works on a detached candidate and commits
atomically. Wind-down is tutorial pacing, not a second interaction. No persistent
zero state, cost-engine change or accidental double block advance.

Victoria then closes for Free at Night, on Day 1. Completion waits for her scene;
no clock tick alone completes the tutorial. Brecken explicitly reports his final
Mansion conversation, granting Victoria knowledge of that choice only at close.
Her reaction uses only her independently held board-impression knowledge:
- known concern: caution about witnessed dismissiveness;
- known informed/listening: acknowledge constructive board response;
- neither: no claim about board behavior.
She refers to the reported final choice and warns that family, business,
obligations and attention shape how he is judged. No universal balance meter.
Private newspaper, calls, breakfast topic, office preparation and external
attendance are never inferred or granted to her. Objective truth is separate.

Close records `day1.completed` and phase `tutorial.day1.complete`, retaining
Day 1 / Night 3/3. Tutorial handlers stop. This is an explicit terminal onboarding
state, not Day 2 gameplay. No post-Day-1 freeform progression is implemented.

## Stable IDs and state

Unless qualified, facts/phases below have prefix `day1.evening.`.

| Category | IDs |
| --- | --- |
| Phases | external, mansion, victoria; existing day1.evening.pending; tutorial.day1.complete |
| Entry/decision facts | entered, external_taken OR external_deferred |
| Attended route facts | personal_taken OR social_taken (neither on deferral) |
| Exclusive final-choice facts | elena, charlie, relative |
| Delayed interactions | elena_delayable, charlie_delayable when unchosen |
| Exclusive Victoria outcome facts | victoria_board_caution, victoria_board_acknowledged, victoria_unbriefed |
| Completion | day1.completed |
| Opportunities | day1.invitation.personal, day1.invitation.adrian |
| Character | provisional.family.evening_relative |
| Bindings | day1.evening.action.enter_personal/social, defer_personal/social, wind_down, final_0/1/2, close_0/1/2_victoria_board_caution/acknowledged/unbriefed |

Existing FormerGirlfriend and Adrian IDs are reused. Former girlfriend remains
explicitly distinct from Alex Wynn. No guest, politician or venue identity added.
Existing relationship.rapport and shared Morning/Afternoon catalog are reused.

## Canonical structure versus provisional writing

Supplied sequence, costs, established character roles and relationship concerns
are the implementation contract. All exact encounter dialogue, invitation details,
rapport tuning, reaction wording and story outcome labels remain provisional via
existing catalog metadata. Minor relative, former girlfriend identity and club
guest mix/politician remain unresolved. Personal writing must supply nuanced
Elena/Charlie scenes, identity decisions, invitation handling/future deferral
consequences, and calibrated Victoria callbacks. No canon invented to fill gaps.

No architecture, club/date/office/boardroom/travel scene, models, animation,
furniture, cinematics, art, vendor, package or project-setting changes. Production
Mansion, frozen POC and Sandbox scene files/metas are protected and unchanged.

## Save/load and validation

Schema remains v3. Runner uses a separate `day1-evening-v3.json` slot, leaving
Morning/Afternoon slots intact. Earlier valid Morning/Afternoon histories are
accepted by the extended validator. Existing old slots are not automatically
selected; their files can be loaded through the Evening validator API.

The bounded replay reuses unchanged Morning replay and extracted Afternoon
replay, then replays Evening entry, take/defer, exclusive final choice and close.
Entire serialized snapshots must match, including time, knowledge, rapport,
opportunities, delayable facts and completion. Fabricated history/state reject.
No generic validation relaxed. Load assigns the live runner only after validation;
failed loads preserve the active session. Reset does not delete saves.

## Verification

Final results and protected-file evidence are recorded in the current checkpoint.
Focused tests exercise both contacts x take/defer x all three final choices,
round trips at each stage, completion guards, known/unknown Victoria facts,
forged state/capacity rejection and provisional metadata in every validation mode.
Live verification uses actual active Sandbox runner handlers, including file
save/reset/reload and corrupt-load preservation. No physical traversal required.

Final verification, Unity 6000.3.27f1, October 29, 2026: 206/206 full EditMode tests
(189+17), 12 live combinations / 288 checks; compilation and current Console both
zero warnings/errors. Mid-Evening save/reset/reload and failed-load preservation
passed in every live route. Invitation, Mansion and completed panels visually
verified. Protected scenes and metadata match Afternoon baseline; see checkpoint
for hashes. Runner reset; Play Mode stopped; save slot restored/removed.
