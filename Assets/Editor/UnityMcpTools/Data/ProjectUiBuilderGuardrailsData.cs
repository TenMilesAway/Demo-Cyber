#nullable enable
using System.ComponentModel;

namespace AIGD
{
    public class ProjectUiBuilderGuardrailsData
    {
        [Description("Short statement of what this UI Builder guardrail skill is for.")]
        public string? Purpose { get; set; }

        [Description("The three recurring UI construction failures this tool is designed to prevent.")]
        public string[]? ProblemPatterns { get; set; }

        [Description("Mandatory rules for creating a single root container and keeping child content inside it.")]
        public string[]? RootContainerRules { get; set; }

        [Description("Rules for keeping text, panels, and modules inside a safe visible area without running off-screen.")]
        public string[]? SafeBoundsRules { get; set; }

        [Description("Rules for splitting the screen into non-overlapping functional zones so independent modules do not stack on top of each other.")]
        public string[]? ModuleSeparationRules { get; set; }

        [Description("Recommended construction order for assembling a new activity-style UI safely.")]
        public string[]? BuildSequence { get; set; }

        [Description("Checks to run before considering the generated UI layout acceptable.")]
        public string[]? ValidationChecklist { get; set; }

        [Description("Warning signs that usually indicate the builder ignored container, bounds, or separation rules.")]
        public string[]? FailureSignals { get; set; }

        [Description("Example layout hierarchy or zoning strategy that the builder should prefer for panel-style activity UIs.")]
        public string[]? RecommendedHierarchy { get; set; }

        [Description("Related project files or skills that should be consulted together with this guardrail tool.")]
        public string[]? CompanionReferences { get; set; }
    }
}
