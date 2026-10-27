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
relationships, per-character knowledge, opportunities, day, time block, and phase.
Player position, scene objects, transcript, and Ink execution position are excluded.

## Format and ownership

StateSave is a detached DTO with required Version, Day, Time, Phase, Facts,
Relationships, Knowledge, and Opportunities members. Version is currently 1;
unsupported versions fail rather than silently resetting or attempting migration.
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

No autosave, slot browser, migration system, cloud sync, or Ink-flow persistence
is included in this milestone.
