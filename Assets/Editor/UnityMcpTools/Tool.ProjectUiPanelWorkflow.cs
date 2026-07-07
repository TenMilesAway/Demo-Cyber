#nullable enable
using AIGD;
using System.ComponentModel;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.ReflectorNet.Utils;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    [AiToolType]
    public partial class Tool_ProjectUiPanelWorkflow
    {
        public const string ProjectUiPanelWorkflowToolId = "project-ui-panel-workflow";

        [AiTool
        (
            ProjectUiPanelWorkflowToolId,
            Title = "Project / UI Panel Workflow",
            ReadOnlyHint = true,
            IdempotentHint = true
        )]
        [AiSkillDescription("Understand and follow this repository's active HotUpdate UI workflow for creating, opening, showing, hiding, and closing gameplay panels. Use this before adding or modifying runtime UI under `Assets/HotUpdate/Scripts/UI`.")]
        [AiSkillBody("This repository's active runtime UI flow is the **HotUpdate UI system** under `Assets/HotUpdate/Scripts/UI`, not the older `Assets/Scripts/UI` tree.\n\n" +
            "## Active runtime entry points\n\n" +
            "- `Launcher.InitData()` initializes the runtime UI manager with `await HA.UIManager.GetInstance().Init()`.\n" +
            "- `UIManager.Init()` loads the shared Canvas and EventSystem prefabs, marks them `DontDestroyOnLoad`, then caches the `Bot`, `Mid`, `Top`, and `System` layer anchors.\n\n" +
            "## Panel identity and prefab path rules\n\n" +
            "- Every runtime panel is addressed by its prefab path string constant in `GlobalDefine`.\n" +
            "- Each panel class must override `GetPanelName()` and return the same `GlobalDefine.*Panel` constant that callers use to open and close it.\n" +
            "- `UIManager.OpenPanel(...)` uses that string as both the Addressables/prefab lookup key and the pool key.\n\n" +
            "## Open/show lifecycle\n\n" +
            "1. Call `UIManager.GetInstance().OpenPanel(panelName, layer, param, action)`.\n" +
            "2. `UIManager` prevents duplicate concurrent loads with `_loadingPanels`.\n" +
            "3. If the panel already exists in `_panelDic`, it reuses the live instance.\n" +
            "4. Otherwise it pulls a GameObject from `UnityObjectPoolFactory.GetItem<GameObject>(panelName, ...)`, parents it to the target UI layer, resets local transform, and caches the `UIBasePanel`.\n" +
            "5. `UIManager` calls `panel.OnInit(param)`.\n" +
            "6. `UIManager` then awaits `panel.TryShowAsync(param, action)`.\n" +
            "7. If show succeeds and `_isBlockingWindow` is `true`, the panel is tracked in `_blockingWindows`.\n\n" +
            "## `UIBasePanel` lifecycle contract\n\n" +
            "- `OnInit(param)` delegates to `InitHandle(param)`.\n" +
            "- The default `InitHandle` sets the GameObject inactive first.\n" +
            "- `TryShowAsync(...)` runs `CanShowAsync(...)`, executes the optional callback, then activates the GameObject and calls `ShowHandle()`.\n" +
            "- `OnHide()` deactivates the GameObject and calls `HideHandle()`.\n" +
            "- `OnClose()` always runs `OnHide()` and then `CloseHandle()`.\n" +
            "- The default `ShowHandle()` / `CloseHandle()` logic manages cursor visibility and player input based on whether any blocking windows remain open.\n\n" +
            "## How to create a new runtime panel\n\n" +
            "1. Add the prefab path constant to `Assets/HotUpdate/Scripts/Utils/GlobalDefine/GlobalDefine.Prefab.cs`.\n" +
            "2. Create the panel prefab at the matching addressable path.\n" +
            "3. Add a `*PanelParam : OpenUIParam` class when the panel needs input data or flags.\n" +
            "4. Implement `*Panel : UIBasePanel` under `Assets/HotUpdate/Scripts/UI/...`.\n" +
            "5. Override `GetPanelName()` to return the exact `GlobalDefine.*Panel` constant.\n" +
            "6. In `InitHandle(...)`, call `base.InitHandle(param)` first, then decode params, initialize UI state, and register listeners.\n" +
            "7. In `CloseHandle()`, call `base.CloseHandle()` and unregister listeners / recycle transient child objects.\n" +
            "8. Open the panel only through `UIManager.GetInstance().OpenPanel(...)`.\n\n" +
            "## Blocking vs non-blocking windows\n\n" +
            "- Panels default to `_isBlockingWindow = true`.\n" +
            "- Set `_isBlockingWindow = false` for HUD-style or passive overlays that should not disable player input or participate in blocking-window cursor logic.\n" +
            "- Existing non-blocking examples include `MainPanel`, `InteractivePanel`, `LoadingPanel`, and `DamagePanel`.\n\n" +
            "## Close semantics: pooled vs destroyed\n\n" +
            "- Use `ClosePanel(panelName)` for normal reusable windows.\n" +
            "- Use `ClosePanelAndDestory(panelName)` only when the panel should bypass pool reuse and be destroyed outright after close.\n" +
            "- Standard panels like inventory, map, store, and loading use `ClosePanel(...)`.\n" +
            "- Ephemeral or sequence-heavy panels like `DialoguePanel` and `DamagePanel` use `ClosePanelAndDestory(...)`.\n\n" +
            "## Common open/close patterns in gameplay code\n\n" +
            "- `FsmStateSpawn` opens `MainPanel` after loading player data and closes it on leave.\n" +
            "- `MainPanel` opens `InventoryPanel` and `PropertyPanel` together.\n" +
            "- `InventoryPanel` closes companion windows such as `ItemDetailInfoPanel`, `TreasurePanel`, `PropertyPanel`, `EquipmentTipPanel`, and `ConvertPanel` based on its param flags.\n" +
            "- `InventoryDataManager` opens `ItemDetailInfoPanel` on pointer enter and closes it on pointer exit.\n" +
            "- `InteractiveDataManager` opens `InteractivePanel` with the `action` callback to populate content before the first visible frame.\n" +
            "- `MapPanel` opens `LoadingPanel` with a `LoadingPanelParam`, then closes itself.\n\n" +
            "## Listener and pooled-child cleanup rules\n\n" +
            "- Always unregister UI and game-event listeners in `CloseHandle()`.\n" +
            "- If the panel spawned pooled child elements, recycle them before closing or as part of the close flow.\n" +
            "- Follow the existing pattern of `AddListeners()` / `RemoveListeners()` plus explicit pooled-object cleanup.")]
        [Description("Return the repository's active runtime UI workflow and conventions for panel creation, opening, showing, hiding, and closing.")]
        public ProjectUiPanelWorkflowData Get()
        {
            return MainThread.Instance.Run(() => new ProjectUiPanelWorkflowData
            {
                ActiveUiRoot = "Assets/HotUpdate/Scripts/UI",
                LegacyUiRoot = "Assets/Scripts/UI",
                InitializationEntryPoint = "Launcher.InitData -> await HA.UIManager.GetInstance().Init()",
                UiManagerPath = "Assets/HotUpdate/Scripts/UI/UIManager.cs",
                UiBasePanelPath = "Assets/HotUpdate/Scripts/UI/UIBasePanel.cs",
                GlobalDefinePrefabPath = "Assets/HotUpdate/Scripts/Utils/GlobalDefine/GlobalDefine.Prefab.cs",
                LayerNames = new[] { "Bot", "Mid", "Top", "System" },
                OpenLifecycle = new[]
                {
                    "Call UIManager.GetInstance().OpenPanel(panelName, layer, param, action).",
                    "UIManager blocks duplicate concurrent loads with _loadingPanels.",
                    "If the panel is already open in _panelDic, reuse the existing instance.",
                    "Otherwise load or reuse the prefab through UnityObjectPoolFactory and parent it under the target UI layer.",
                    "UIManager calls panel.OnInit(param).",
                    "UIManager awaits panel.TryShowAsync(param, action).",
                    "Blocking panels are tracked in _blockingWindows after a successful show."
                },
                BasePanelLifecycle = new[]
                {
                    "OnInit(param) delegates to InitHandle(param).",
                    "The default InitHandle disables the GameObject first.",
                    "TryShowAsync calls CanShowAsync, then the optional callback, then activates the GameObject and runs ShowHandle.",
                    "OnHide disables the GameObject and runs HideHandle.",
                    "OnClose always runs OnHide and then CloseHandle.",
                    "The default ShowHandle and CloseHandle manage cursor visibility and player input according to blocking-window state."
                },
                CreationChecklist = new[]
                {
                    "Add the prefab path constant to GlobalDefine.Prefab.cs.",
                    "Create the prefab at the matching addressable path.",
                    "Create a *PanelParam : OpenUIParam type when the panel needs input data or flags.",
                    "Implement *Panel : UIBasePanel under Assets/HotUpdate/Scripts/UI.",
                    "Override GetPanelName() to return the exact GlobalDefine.*Panel constant.",
                    "Call base.InitHandle(param) first inside InitHandle, then decode params and register listeners.",
                    "Call base.CloseHandle() inside CloseHandle, then unregister listeners and recycle transient child objects.",
                    "Open the panel only through UIManager.GetInstance().OpenPanel(...)."
                },
                BlockingWindowRules = new[]
                {
                    "Panels default to _isBlockingWindow = true.",
                    "Set _isBlockingWindow = false for HUD-style or passive overlays.",
                    "Existing non-blocking examples include MainPanel, InteractivePanel, LoadingPanel, and DamagePanel."
                },
                CloseModes = new[]
                {
                    "ClosePanel(panelName) closes the panel and returns it to UnityObjectPoolFactory for reuse.",
                    "ClosePanelAndDestory(panelName) closes the panel and destroys the GameObject instead of pooling it.",
                    "Standard panels such as inventory, map, store, and loading use ClosePanel.",
                    "Ephemeral panels such as DialoguePanel and DamagePanel use ClosePanelAndDestory."
                },
                CommonUsageExamples = new[]
                {
                    "FsmStateSpawn opens MainPanel after loading player data and closes it on state leave.",
                    "MainPanel opens InventoryPanel and PropertyPanel together.",
                    "InventoryPanel owns the close logic for ItemDetailInfoPanel, TreasurePanel, PropertyPanel, EquipmentTipPanel, and ConvertPanel based on param flags.",
                    "InventoryDataManager opens ItemDetailInfoPanel on pointer enter and closes it on pointer exit.",
                    "InteractiveDataManager opens InteractivePanel with the action callback so it can populate content before first show.",
                    "MapPanel opens LoadingPanel with LoadingPanelParam and then closes itself."
                },
                CleanupRules = new[]
                {
                    "Always unregister UI and game-event listeners in CloseHandle().",
                    "Recycle pooled child objects such as item cells, dialogue options, and drag images during close or teardown.",
                    "Follow the AddListeners / RemoveListeners pattern used by existing panels."
                },
                ExampleFiles = new[]
                {
                    "Assets/HotUpdate/Scripts/Launcher/Launcher.cs",
                    "Assets/HotUpdate/Scripts/UI/UIManager.cs",
                    "Assets/HotUpdate/Scripts/UI/UIBasePanel.cs",
                    "Assets/HotUpdate/Scripts/Utils/GlobalDefine/GlobalDefine.Prefab.cs",
                    "Assets/HotUpdate/Scripts/UI/Main/MainPanel.cs",
                    "Assets/HotUpdate/Scripts/UI/Inventory/InventoryPanel.cs",
                    "Assets/HotUpdate/Scripts/UI/Interactive/DialoguePanel.cs",
                    "Assets/HotUpdate/Scripts/UI/Others/LoadingPanel.cs",
                    "Assets/HotUpdate/Scripts/DataManager/InteractiveDataManager.cs",
                    "Assets/HotUpdate/Scripts/DataManager/InventoryDataManager.cs"
                }
            });
        }
    }
}
