# Environment workflow

## Three lanes

- `Assets/Scenes/Prototype/Prototype_Mansion_POC.unity`: frozen regression/reference proof of concept. Renamed/moved from `Assets/Prototype_Mansion.unity`, preserving its GUID and original scene contents. Do not use it for ongoing construction; use it to reproduce the proven movement, Ink, consequence and save/load loop.
- `Assets/Scenes/Prototype/Prototype_Sandbox.unity`: disposable experimental integration/build area. Starts with an independent copy of the POC, including its playable cast and hookups, plus a 200 x 200 metre collider ground and an Assembly Workspace at x=30. The source scene had a graybox room, not a terrain asset. This ground supplies room for future buildings, vehicles, navigation, lighting, shaders and package trials. No new systems are installed.
- `Assets/Scenes/Game/Mansion.unity`: comparatively sterile production shell, initially retaining only graybox room geometry, camera and directional light. No prototype cast, player controller or funeral-night runner. This is the sole enabled build scene; open the POC directly to run the prototype. The existing SampleScene remains untouched as template material.

POC and Sandbox are development scenes, excluded from the production build list. The production shell is intentionally not a playable narrative milestone yet.

## One physical world, changing state

Reuse the production Mansion for day/time/story variation. Future GameState and content activation should control character presence, dialogue, access, props and lighting. Do not create Mansion_Day1, Mansion_Day2 or chapter copies. Separate scenes are justified for distinct locations or genuine physical/loading partitions. This tranche documents that direction; it adds no state activation system.

## Build in Sandbox, promote deliberately

Major third-party systems/assets must prove themselves in Sandbox before promotion to production. Check dependencies, project settings, render/input compatibility, references and performance. A scene boundary does not isolate project-wide package or shader/settings changes: evaluate risky packages in a disposable project/branch first, then integrate deliberately.

Work may assemble modular buildings and other reusable environment assemblies in Sandbox. When useful, save candidates under `Assets/Prefabs/Sandbox/` and approved environment prefabs under `Assets/Prefabs/Game/Environment/`; create these folders only when there is actual content. Promote via Unity's asset tools with GUIDs/dependencies preserved, then place the approved prefab in Mansion. Keep layout/pivots reusable to minimize rework. Do not copy entire experimental scenes into production. No prefab pipeline or new assets beyond the scene ground are required now.

Independent scene copies still share referenced scripts/materials/Ink assets. Freezing the POC means avoiding intentional scene edits and using Git milestone history when shared dependencies change; it is not an immutable copy of every dependency.
