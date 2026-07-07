---
name: project-ui-panel-workflow
description: Understand and follow this repository's active HotUpdate UI workflow for creating, opening, showing, hiding, and closing gameplay panels. Use this before adding or modifying runtime UI under `Assets/HotUpdate/Scripts/UI`.
---

# Project / UI Panel Workflow

This repository's active runtime UI flow is the **HotUpdate UI system** under `Assets/HotUpdate/Scripts/UI`, not the older `Assets/Scripts/UI` tree.

## Active runtime entry points

- `Launcher.InitData()` initializes the runtime UI manager with `await HA.UIManager.GetInstance().Init()`.
- `UIManager.Init()` loads the shared Canvas and EventSystem prefabs, marks them `DontDestroyOnLoad`, then caches the `Bot`, `Mid`, `Top`, and `System` layer anchors.

## Panel identity and prefab path rules

- Every runtime panel is addressed by its prefab path string constant in `GlobalDefine`.
- Each panel class must override `GetPanelName()` and return the same `GlobalDefine.*Panel` constant that callers use to open and close it.
- `UIManager.OpenPanel(...)` uses that string as both the Addressables/prefab lookup key and the pool key.

## Open/show lifecycle

1. Call `UIManager.GetInstance().OpenPanel(panelName, layer, param, action)`.
2. `UIManager` prevents duplicate concurrent loads with `_loadingPanels`.
3. If the panel already exists in `_panelDic`, it reuses the live instance.
4. Otherwise it pulls a GameObject from `UnityObjectPoolFactory.GetItem<GameObject>(panelName, ...)`, parents it to the target UI layer, resets local transform, and caches the `UIBasePanel`.
5. `UIManager` calls `panel.OnInit(param)`.
6. `UIManager` then awaits `panel.TryShowAsync(param, action)`.
7. If show succeeds and `_isBlockingWindow` is `true`, the panel is tracked in `_blockingWindows`.

## `UIBasePanel` lifecycle contract

- `OnInit(param)` delegates to `InitHandle(param)`.
- The default `InitHandle` sets the GameObject inactive first.
- `TryShowAsync(...)` runs `CanShowAsync(...)`, executes the optional callback, then activates the GameObject and calls `ShowHandle()`.
- `OnHide()` deactivates the GameObject and calls `HideHandle()`.
- `OnClose()` always runs `OnHide()` and then `CloseHandle()`.
- The default `ShowHandle()` / `CloseHandle()` logic manages cursor visibility and player input based on whether any blocking windows remain open.

## How to create a new runtime panel

1. Add the prefab path constant to `Assets/HotUpdate/Scripts/Utils/GlobalDefine/GlobalDefine.Prefab.cs`.
2. Create the panel prefab at the matching addressable path.
3. Add a `*PanelParam : OpenUIParam` class when the panel needs input data or flags.
4. Implement `*Panel : UIBasePanel` under `Assets/HotUpdate/Scripts/UI/...`.
5. Override `GetPanelName()` to return the exact `GlobalDefine.*Panel` constant.
6. In `InitHandle(...)`, call `base.InitHandle(param)` first, then decode params, initialize UI state, and register listeners.
7. In `CloseHandle()`, call `base.CloseHandle()` and unregister listeners / recycle transient child objects.
8. Open the panel only through `UIManager.GetInstance().OpenPanel(...)`.

## Blocking vs non-blocking windows

- Panels default to `_isBlockingWindow = true`.
- Set `_isBlockingWindow = false` for HUD-style or passive overlays that should not disable player input or participate in blocking-window cursor logic.
- Existing non-blocking examples include `MainPanel`, `InteractivePanel`, `LoadingPanel`, and `DamagePanel`.

## Close semantics: pooled vs destroyed

- Use `ClosePanel(panelName)` for normal reusable windows.
- Use `ClosePanelAndDestory(panelName)` only when the panel should bypass pool reuse and be destroyed outright after close.
- Standard panels like inventory, map, store, and loading use `ClosePanel(...)`.
- Ephemeral or sequence-heavy panels like `DialoguePanel` and `DamagePanel` use `ClosePanelAndDestory(...)`.

## Common open/close patterns in gameplay code

- `FsmStateSpawn` opens `MainPanel` after loading player data and closes it on leave.
- `MainPanel` opens `InventoryPanel` and `PropertyPanel` together.
- `InventoryPanel` closes companion windows such as `ItemDetailInfoPanel`, `TreasurePanel`, `PropertyPanel`, `EquipmentTipPanel`, and `ConvertPanel` based on its param flags.
- `InventoryDataManager` opens `ItemDetailInfoPanel` on pointer enter and closes it on pointer exit.
- `InteractiveDataManager` opens `InteractivePanel` with the `action` callback to populate content before the first visible frame.
- `MapPanel` opens `LoadingPanel` with a `LoadingPanelParam`, then closes itself.

## Listener and pooled-child cleanup rules

- Always unregister UI and game-event listeners in `CloseHandle()`.
- If the panel spawned pooled child elements, recycle them before closing or as part of the close flow.
- Follow the existing pattern of `AddListeners()` / `RemoveListeners()` plus explicit pooled-object cleanup.

## How to Call

```bash
unity-mcp-cli run-tool project-ui-panel-workflow --input '{}'
```


### Troubleshooting

If `unity-mcp-cli` is not found, either install it globally (`npm install -g unity-mcp-cli`) or use `npx unity-mcp-cli` instead.
Read the /unity-initial-setup skill for detailed installation instructions.

## Input

This tool takes no input parameters.

### Input JSON Schema

```json
{
  "type": "object",
  "additionalProperties": false
}
```

## Output

### Output JSON Schema

```json
{
  "type": "object",
  "properties": {
    "result": {
      "$ref": "#/$defs/AIGD.ProjectUiPanelWorkflowData"
    }
  },
  "$defs": {
    "System.String-1": {
      "type": "array",
      "items": {
        "type": "string"
      }
    },
    "AIGD.ProjectUiPanelWorkflowData": {
      "type": "object",
      "properties": {
        "ActiveUiRoot": {
          "type": "string",
          "description": "Active runtime UI root used by the project."
        },
        "LegacyUiRoot": {
          "type": "string",
          "description": "Older UI tree that exists in the repository but is not the active runtime flow."
        },
        "InitializationEntryPoint": {
          "type": "string",
          "description": "Entry point used to initialize the active UI manager at runtime."
        },
        "UiManagerPath": {
          "type": "string",
          "description": "Path to the active UI manager implementation."
        },
        "UiBasePanelPath": {
          "type": "string",
          "description": "Path to the base panel lifecycle implementation."
        },
        "GlobalDefinePrefabPath": {
          "type": "string",
          "description": "Path to the prefab-path constants used as panel identifiers."
        },
        "LayerNames": {
          "$ref": "#/$defs/System.String-1",
          "description": "Canvas layer anchors used by UIManager."
        },
        "OpenLifecycle": {
          "$ref": "#/$defs/System.String-1",
          "description": "Ordered summary of the panel open and show flow."
        },
        "BasePanelLifecycle": {
          "$ref": "#/$defs/System.String-1",
          "description": "Lifecycle responsibilities defined by UIBasePanel."
        },
        "CreationChecklist": {
          "$ref": "#/$defs/System.String-1",
          "description": "Checklist to follow when creating a new runtime panel."
        },
        "BlockingWindowRules": {
          "$ref": "#/$defs/System.String-1",
          "description": "Rules that determine whether a panel participates in blocking-window behavior."
        },
        "CloseModes": {
          "$ref": "#/$defs/System.String-1",
          "description": "Available close modes and when the project uses them."
        },
        "CommonUsageExamples": {
          "$ref": "#/$defs/System.String-1",
          "description": "Representative open and close patterns already used in gameplay code."
        },
        "CleanupRules": {
          "$ref": "#/$defs/System.String-1",
          "description": "Cleanup rules for listeners and pooled child objects."
        },
        "ExampleFiles": {
          "$ref": "#/$defs/System.String-1",
          "description": "Key project files that illustrate the active UI workflow."
        }
      }
    }
  },
  "required": [
    "result"
  ]
}
```

