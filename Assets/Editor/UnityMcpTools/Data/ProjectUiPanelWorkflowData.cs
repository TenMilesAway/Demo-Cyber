#nullable enable
using System.ComponentModel;

namespace AIGD
{
    public class ProjectUiPanelWorkflowData
    {
        [Description("Active runtime UI root used by the project.")]
        public string? ActiveUiRoot { get; set; }

        [Description("Older UI tree that exists in the repository but is not the active runtime flow.")]
        public string? LegacyUiRoot { get; set; }

        [Description("Entry point used to initialize the active UI manager at runtime.")]
        public string? InitializationEntryPoint { get; set; }

        [Description("Path to the active UI manager implementation.")]
        public string? UiManagerPath { get; set; }

        [Description("Path to the base panel lifecycle implementation.")]
        public string? UiBasePanelPath { get; set; }

        [Description("Path to the prefab-path constants used as panel identifiers.")]
        public string? GlobalDefinePrefabPath { get; set; }

        [Description("Canvas layer anchors used by UIManager.")]
        public string[]? LayerNames { get; set; }

        [Description("Ordered summary of the panel open and show flow.")]
        public string[]? OpenLifecycle { get; set; }

        [Description("Lifecycle responsibilities defined by UIBasePanel.")]
        public string[]? BasePanelLifecycle { get; set; }

        [Description("Checklist to follow when creating a new runtime panel.")]
        public string[]? CreationChecklist { get; set; }

        [Description("Rules that determine whether a panel participates in blocking-window behavior.")]
        public string[]? BlockingWindowRules { get; set; }

        [Description("Available close modes and when the project uses them.")]
        public string[]? CloseModes { get; set; }

        [Description("Representative open and close patterns already used in gameplay code.")]
        public string[]? CommonUsageExamples { get; set; }

        [Description("Cleanup rules for listeners and pooled child objects.")]
        public string[]? CleanupRules { get; set; }

        [Description("Key project files that illustrate the active UI workflow.")]
        public string[]? ExampleFiles { get; set; }
    }
}
