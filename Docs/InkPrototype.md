# Funeral-night Ink prototype

Verified October 27, 2026 with Unity 6000.3.27f1.

## Playable flow

Open Assets/Prototype_Mansion.unity and enter Play Mode. Move Brecken with
WASD/arrows. Approach a stationary labeled cylinder (Elena, Charlie, Victoria);
within 2.5 world units press E or click Talk. Choose family with Elena or
business with Charlie. Close the conversation, approach Victoria, and talk.
Esc/Close dismisses dialogue. Restart prototype clears session state so the
other outcome can be exercised without leaving Play Mode. Leaving a choice
conversation without choosing spends nothing. Use Save/Load for the backend state slot; see SaveLoad.md.

Victoria is a third cylinder at (0,1,4), cloned from Elena with the same
stationary collider/rendering and screen-space label setup. The scene runner
references Brecken and all three NPCs.

## Architecture and authored information event

Ink controls flow; ElenaInkConversation binds pure backend queries with
lookaheadSafe=true and guarded consequence requests with lookaheadSafe=false.
No Ink variable is authoritative. The bridge retains its original class name
but now selects elena, charlie, or victoria knots from the existing story asset.
InkPrototypeRunner owns fresh prototype session state and minimal proximity UI.

Speaking to Victoria is an explicit authored report: the UI states that Brecken
tells her who he spent the evening with. The runner invokes InformVictoria and
InformVictoriaAboutCharlie; only the eligible true, exclusive event is granted.
Neither choosing nor querying automatically grants knowledge. There is no
generic propagation or belief engine.

Victoria's exact predicates:
- Family: BRECKEN_COMFORTED_ELENA true, BRECKEN_MET_CHARLIE_FUNERAL_NIGHT false,
  Victoria knows BRECKEN_COMFORTED_ELENA, phase AfterFuneralNight.
- Business: BRECKEN_MET_CHARLIE_FUNERAL_NIGHT true, BRECKEN_COMFORTED_ELENA false,
  Victoria knows BRECKEN_MET_CHARLIE_FUNERAL_NIGHT, phase AfterFuneralNight.
- Otherwise: "I have not heard what you decided tonight."

The eligible lines are "I see you have chosen family over business." and
"Your mind is on business instead of family."

## Backend outcomes

Both choices require FuneralNight, Evening, CharlieBusiness Available, and both
event facts false. Both advance one block to Night, day remains 0, and set phase
AfterFuneralNight. These guards reject repeats and the opposite choice.

| Choice | Comforted Elena | Met Charlie | Elena resentment | Charlie resentment | CharlieBusiness |
| --- | --- | --- | --- | --- | --- |
| Elena/family | true | false | -1 | unchanged (0) | Missed |
| Charlie/business | false | true | +1 | -1 | Completed |

Relationship deltas are prototype authored values. GameState remains generic;
FuneralNightConsequences owns all effects and eligibility. Each information
consequence requires the matching exclusive truth, later phase, and absent
knowledge, and grants only Victoria's awareness of that event.

## Package and scene provenance

No package changes. Official com.inkle.ink-unity-integration 2.0.0 remains locked
to 73cc147969899710843f473fdb06cea0e4d50fed. Existing Ink settings and defines remain.

Directional Light's UniversalAdditionalLightData remains the verified URP 17.3
default inspector attachment. CoreEditorUtils.GetAdditionalData adds it through
Undo.AddComponent; UniversalAdditionalLightData defines the serialized defaults.
This change does not alter lighting.

## Verification

- Unity CLI recompile passed with no errors or warnings; forced Ink import passed.
- Pipeline EditMode suite: 32 passed, 0 failed/skipped/inconclusive (20 existing
  cases plus 12 focused cases). Covers both backend choices/effects/repeats,
  truth/knowledge/phase requirements, explicit propagation, contradictory truth,
  actual compiled Ink branches, opposite dialogue eligibility, and leaving.
- Live Play Mode through the same public handlers used by UI: early Victoria
  fallback, rejected out-of-range talk, Elena choice then family verification,
  restart, Charlie choice then business verification. Knowledge absent before
  report and present after. Repeat and opposite backend requests rejected.
- Game advanced from frame 2402 to 13884; inspected Game View screenshots showed
  both lines, outcome readout, and all three labels. Verification positioned
  Brecken via Pipeline and invoked interaction/choice handlers; physical key
  presses were not simulated.
- Captures kept outside Assets; temporary capture assets removed.
- Editor left outside Play Mode; background override restored.
