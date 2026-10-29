# Day 1 Tranche 2: Afternoon Logic

## Implemented flow

Prototype_Sandbox retains its existing DayOneMorningRunner panel and all Morning
handlers. It now continues from day1.office.pending when Morning is complete,
Afternoon is active and day1.office.required exists. The clock alone cannot start it.
Office arrival is a free abstract label; exactly one office visit costs Fixed(1),
leaving Afternoon 2/3 and phase day1.board.pending. No travel or geometry is added.

- Secretary: light rapport +1 and shared optional encounter knowledge. Identity and
  precise role provisional. All routes record secretary_delayable; skipping creates
  no penalty, expiry or permanently missed opportunity. No later encounter authored yet.
- Richard Hale: always eligible within this office sequence. Newspaper awareness
  selects hale_prepared; otherwise hale_basic provides an introduction to succession
  and operational continuity. Neither route grants newspaper facts to Hale or Victoria.
- Acquisitions VP: provisional identity and target; opens the existing opportunity
  architecture as Available, without acceptance/completion, deadline or implicit expiry.
  It remains available across day changes. Its reserved followup phase has no entry
  in this tranche; subsequent content must author the actual business resolution.

Required Board Meeting includes Brecken, Charlie, Richard Hale and Victoria. It
addresses succession uncertainty, market reaction, investor confidence, authority
and immediate continuity; it does not settle the board conflict. Two provisional
handling options belong to the SAME meeting and each costs Fixed(2):

- Listen, acknowledge limits and ask questions: informed first impression if Brecken
  has newspaper context, a Hale briefing, or Morning Charlie/business knowledge;
  otherwise constructive listening first impression. Newspaper-prepared Hale also records board_hale_prepared as a distinct preparation payoff. Lack of newspaper is no failure.
- Dismiss legitimate concerns and demand trust: concerned first impression despite
  preparation. No permanent victory/defeat or universal competence score.

Hale and Victoria receive local rapport +1/-1 and awareness of the observed first
impression; Charlie and Brecken also know the impression. They do not automatically
learn Brecken's private preparation or world premises. Effects remain explicit.

The meeting requires office obligation, Morning completion, completed office visit,
board-pending phase and Afternoon. It clears the office obligation, records board
completion/handling/impression, and sets day1.evening.pending. Fixed(2) exhausts the
remaining capacity using unchanged GameState behavior: automatic Evening 3/3, Day 1.
No persistent zero state and no Evening interaction. Repeats/skips reject without cost.

## Stable IDs and persisted state

Facts below use day1.afternoon. unless fully qualified:

| ID | Meaning |
| --- | --- |
| office_selected | Exclusive discretionary visit complete |
| secretary_seed | Optional office-life encounter occurred |
| secretary_delayable | Encounter can be continued/delayed, no miss penalty |
| hale_met; hale_prepared OR hale_basic | CEO briefing and preparation variation |
| acquisitions_met | Provisional VP introduction |
| board_completed; board_measured OR board_dismissive | Required meeting and handling |
| board_informed OR board_listening OR board_concern | Observed first impression |
| board_hale_prepared | Measured handling with newspaper-prepared Hale payoff |
| day1.office.required | Morning obligation, cleared on board completion |

Phases: existing day1.office.pending -> day1.board.pending -> day1.evening.pending.
Reserved later phase: business.acquisition.followup (no route to it implemented).
Opportunity: business.acquisition.provisional, Available only when VP chosen.
New characters: provisional.office.secretary, provisional.office.acquisitions_vp.
Existing brecken, richard.hale, Charlie and Victoria IDs are reused.
Existing relationship.rapport is reused. Binding IDs use day1.afternoon.action.
with secretary, hale_prepared, hale_basic, acquisitions and board variant suffixes.

## Save/load and validation

Schema remains v3. Active Sandbox slot: day1-afternoon-v3.json under
Application.persistentDataPath. Existing day1-morning-v3.json remains untouched.
The full Morning/Afternoon history can be saved at any stage, including office
selection at 2/3 and the final Evening endpoint. The old Morning-only validator
still accepts exactly its previous histories; its replay is reused by the bounded
Afternoon validator. No generic validator or strict catalog rule is relaxed.

Both save and load use the combined runtime catalog and replay every one-shot
choice from a fresh Morning, comparing the entire serialized snapshot. Wrong
capacity, contradictory paths, fabricated knowledge, relationships, resolved
opportunities or outcomes reject. Load assigns live state only after success;
failed loads preserve the active session. Reset does not remove the slot.

## Canonical structure and Personal-writing needs

Established roles: CEO Richard Hale; Charlie is Brecken's older cousin, experienced
business mentor and later internal challenger. The required participants, sequence,
costs and stopping boundary are supplied structure. No new family/business canon.
All concise dialogue, reactions, rapport tuning and outcome facts are provisional
through existing metadata. Secretary/VP identities, roles, acquisition target and
later followup are explicitly unresolved. No extra named board member is invented.

Personal writing still needs office encounter dialogue, secretary identity/role,
VP identity/target/business terms, nuanced Hale preparation exchange, differentiated
board voices and calibrated first impressions/callbacks. Evening commitments and
all production presentation remain future work. This adds no office, executive
floor, boardroom, travel, model, animation, furniture, cinematic or art dressing.

## Verification

See current checkpoint for final full-suite, live handler, Console and protected
scene evidence. The focused tests cover three routes with/without newspaper,
capacity boundaries, mandatory/exclusive order, measured/dismissive handling,
Charlie context, strict metadata/history rejection, v3 continuation and persistent
unresolved acquisition opening. Runtime verification must use the actual panel
handlers in Sandbox Play Mode, including failed-load preservation.

Verification completed October 29, 2026, Unity 6000.3.27f1:
- Full unfiltered EditMode suite: 189/189 passed (178 baseline + 11 focused cases),
  zero failed/skipped/inconclusive. Clean compilation; no project warnings/errors.
- Actual active Sandbox panel handlers: all 3 routes x both newspaper states x
  both board responses, 26 checks each, 312 total. Each tested 2/3 save/reset/reload,
  corrupt-load preservation, Evening 3/3 completion and endpoint round trip.
- Office, board and Evening panels visually inspected. Runner reset to fresh
  Morning; verification slot removed/restored and Play Mode stopped.
- Current Console ground truth: zero errors and zero warnings. An initial test
  request made while the editor was already in Play Mode failed at the test-runner
  level; editor restart and verified stopped-state rerun produced the full pass.
- Production Mansion SHA-256:
  8FA6D5996D5FEAA8311D8A86A1D456F5B91CBA7792E2EC0DAD0EBC5955C3DF41
- Frozen POC SHA-256:
  AD62AFA9FCFE923C6F6637C2E0E22B708B7B4D6E39FDD0A256D8F85962460A4A
- Both scenes and metadata match f634e13f86ce1e6dedb18b08596a595be1e2ecfd.
  Sandbox scene and metadata also unchanged; only its existing runner source changed.
  No art/vendor/package/project-setting changes. Verification-created background-run
  setting/configuration restored; generated config backed up under ignored Temp.
- Evidence under ignored Temp: DayOneAfternoon-full-tests.json, -live.json,
  -final-compile.json, -final-console.json, -protected.json; office/board/evening PNGs
  and walk-afternoon.cs. No Player build performed.
