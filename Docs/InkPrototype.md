# Ink dialogue prototype

Verified October 27, 2026 with Unity 6000.3.27f1.

## Architecture

Ink controls conversation flow and reads caller-owned GameState. The explicit
comfort_elena request delegates exclusively to the existing guarded
FuneralNightConsequences.ChooseElena.TryApply operation. It does not write raw
world state or maintain a parallel authoritative state. The mutation callback
is bound with lookaheadSafe=false; only pure queries are lookahead-safe.
The existing bridge and backend were preserved without redesign.

InkPrototypeRunner is a temporary graybox OnGUI harness. Each Play session
creates fresh prototype state. Future integration should inject the real
session owner and replace this UI; save/load is outside this prototype.

## Package and scene

Official com.inkle.ink-unity-integration 2.0.0 is installed through the upm Git
branch, locked to 73cc147969899710843f473fdb06cea0e4d50fed. InkSettings and
INK_RUNTIME/INK_EDITOR defines are included. Prototype_Mansion references the
imported InkFile through its runner.

The Directional Light's UniversalAdditionalLightData is retained as expected
URP Editor data, verified against installed URP 17.3.0 source:

- Project GraphicsSettings and QualitySettings select Universal pipeline assets.
- Editor/Lighting/UniversalRenderPipelineSerializedLight.cs constructs its
  additional data through CoreEditorUtils.GetAdditionalData.
- Core RP Editor/CoreEditorUtils.cs adds missing data with Undo.AddComponent.
- Runtime/UniversalAdditionalLightData.cs defines exactly the serialized defaults:
  pipeline shadow bias enabled, high additional-light shadow tier (2), no custom
  shadow layers, cookie size (1,1), offset (0,0), pipeline soft-shadow quality (0),
  default rendering/shadow masks (1), and current serialized version 4.

This is the normal light-inspector attachment path, not an Ink requirement or
an authored lighting adjustment. Original Light and Transform fields are unchanged.
Persisting the component avoids repeating the automatic attachment on inspection.

## Verification after cleanup

- Removed Assets/Temp/ink-before.png, ink-after.png, their metadata, and the empty
  Assets/Temp folder and folder metadata through AssetDatabase.DeleteAsset.
- Forced Ink story reimport succeeded and produced compiled story JSON.
- Script recompile reported up_to_date, with no compilation failure.
- Live runner started with one choice and comfort fact false; selecting the same
  handler used by the button completed the story and set comfort true.
- Backend effects: Night, AfterFuneralNight, Elena resentment -1, Charlie's
  opportunity Missed, Victoria unaware. Repeating TryApply was rejected and
  resentment remained -1.
- Returning conversation recognized the existing comfort fact.
- Ineligible conversation returned the alternate response; fact, phase, time,
  day, relationship, opportunity and Victoria knowledge remained unchanged.
- Existing EditMode suite: 20 passed, zero failed/skipped/inconclusive.

Tooling history: the earlier screenshot destination error and command-discovery
errors were identified and cleared. An initial asynchronous test attempt overlapped
Play Mode, causing test-runner errors; it was cancelled and rerun successfully
outside Play Mode. These were verification orchestration errors, not game errors.
Final clean-console cycle passed: 20/20 EditMode tests, forced Ink reimport,
and live Play Mode choice. Final Editor console: zero errors and zero warnings;
compilationFailed=false. Editor left outside Play Mode.