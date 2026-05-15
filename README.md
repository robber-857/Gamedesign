# Museum

Unity 2D platformer project for the Gamedesign 32003 coursework.

## Project

Museum is a side-scrolling level project with a cover screen, level menu, checkpoint restart flow, hazards, moving platforms, switches, spring pads, and five playable levels.

Current build scenes are:

1. `Assets/Scenes/Cover.unity`
2. `Assets/Scenes/LevelMenu.unity`
3. `Assets/Scenes/Level_0.unity`
4. `Assets/Scenes/level_1.unity`
5. `Assets/Scenes/level_2.unity`
6. `Assets/Scenes/level_3.unity`
7. `Assets/Scenes/Level_4.unity`

Older prototype scenes such as `Game`, `Menu`, `Seconddev`, `Thirddev`, `LEVEL Y`, and `level_0_tutorial_scene` are no longer part of the build.

## Requirements

- Unity `6000.3.10f1`
- Universal Render Pipeline
- Unity 2D packages and Input System from `Packages/manifest.json`

Open the project from the repository root in Unity Hub.

## Controls

- Move: `A/D` or left/right input axis
- Jump: `Space`
- Menu: in-game `Menu` button

## Code Layout

- `Assets/Script/Gameplay/`: current gameplay controllers for levels 0-3, cover screen, menu, checkpoints, hazards, springs, audio, moving platforms, and player movement.
- `Assets/脚本/`: legacy gameplay scripts still used by Level 4 and some shared prefabs. These are kept because the active scene references them.
- `Assets/Resources/`: runtime-loaded cover image and spring bounce audio.

This branch intentionally avoids publishing Unity recovery files, obsolete scenes, and duplicate or unreferenced script changes.

## Build

Use `File > Build Profiles` in Unity and make sure the scenes listed above are enabled in the same order as `ProjectSettings/EditorBuildSettings.asset`.
