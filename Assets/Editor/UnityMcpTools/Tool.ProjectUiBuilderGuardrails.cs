#nullable enable
using AIGD;
using System.ComponentModel;
using com.IvanMurzak.McpPlugin;
using com.IvanMurzak.ReflectorNet.Utils;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    [AiToolType]
    public partial class Tool_ProjectUiBuilderGuardrails
    {
        public const string ProjectUiBuilderGuardrailsToolId = "project-ui-builder-guardrails";

        [AiTool
        (
            ProjectUiBuilderGuardrailsToolId,
            Title = "Project / UI Builder Guardrails",
            ReadOnlyHint = true,
            IdempotentHint = true
        )]
        [AiSkillDescription("Review this repository's reusable UI Builder guardrails before generating activity or panel-style UI. It focuses on preventing missing root panels, off-screen overflow, and overlapping functional modules.")]
        [AiSkillBody("Use this skill when building panel-style UI so the generated hierarchy stays inside a single root container, respects safe visible bounds, and keeps separate modules from overlapping.\n\n" +
            "## The three failure patterns this skill prevents\n\n" +
            "1. **Missing overall container** — generated controls are created directly under the canvas or under unrelated nodes, so they drift outside the intended panel bounds.\n" +
            "2. **Content overflow** — long text blocks or stacked widgets extend below or beyond the visible frame because no safe-content area, padding, or scrolling strategy was reserved.\n" +
            "3. **Module overlap** — left/right or top/bottom functional blocks are positioned independently without dedicated zones, so they occupy the same space and visually collide.\n\n" +
            "## Root container rules\n\n" +
            "- Always start from one full, explicit root panel that owns the whole UI layout.\n" +
            "- All generated visual modules must be children of that root panel or of its named sub-containers — never free-floating siblings without a common boundary owner.\n" +
            "- Inside the root panel, create a content frame or safe-area container before placing real modules.\n" +
            "- Background, decorative elements, content blocks, and footer actions should all live under predictable named containers so later adjustments stay local.\n\n" +
            "## Safe bounds rules\n\n" +
            "- Reserve inner padding between the panel edge and live content instead of placing content directly against the panel border.\n" +
            "- Long text blocks must fit inside a bounded text region; if the wording implies long-form reading, place the content inside a Scroll View.\n" +
            "- After assigning font size, verify that text still fits its container and does not fall outside the safe content frame.\n" +
            "- Bottom buttons, owned-currency text, and status labels must each keep dedicated vertical space so they do not slide below the screen edge.\n\n" +
            "## Module separation rules\n\n" +
            "- Split the panel into named zones first, then place modules inside those zones; do not position unrelated modules by eyeballing global coordinates.\n" +
            "- For left/right compositions, create separate left and right containers before adding children.\n" +
            "- For top/middle/bottom compositions, create dedicated header, content, and footer containers before filling them.\n" +
            "- Treat each container as that module's territory: explanatory text, card grids, resource summaries, and action buttons should not cross container boundaries.\n\n" +
            "## Recommended build order\n\n" +
            "1. Create the root panel.\n" +
            "2. Create the inner safe-content container with padding.\n" +
            "3. Split the safe-content container into major layout zones such as left/right or header/content/footer.\n" +
            "4. Add one functional module per zone.\n" +
            "5. Add long text, grids, and buttons only after their parent zones exist.\n" +
            "6. Run a final overlap and overflow review before considering the UI complete.\n\n" +
            "## Validation checklist\n\n" +
            "- Every visible node belongs to the root panel hierarchy.\n" +
            "- No critical text, button, or module extends beyond the intended visible frame.\n" +
            "- Long-form content either fits comfortably or is wrapped in a Scroll View.\n" +
            "- Left/right and top/bottom modules occupy separate containers and do not overlap.\n" +
            "- Footer actions still have breathing room beneath the main content block.\n\n" +
            "## Typical hierarchy to prefer\n\n" +
            "- `RootPanel`\n" +
            "- `RootPanel/Background`\n" +
            "- `RootPanel/SafeContent`\n" +
            "- `RootPanel/SafeContent/LeftZone`\n" +
            "- `RootPanel/SafeContent/RightZone`\n" +
            "- `RootPanel/SafeContent/FooterZone`\n\n" +
            "## Companion usage\n\n" +
            "- Use this skill together with `project-ui-panel-workflow` when the UI also needs to obey the repository's HotUpdate runtime panel conventions.\n" +
            "- Consult this guardrail skill before generating a new activity panel, and consult `project-ui-panel-workflow` before wiring the prefab into the runtime open/show/close flow.")]
        [Description("Return reusable UI Builder guardrails that prevent missing root containers, content overflow, and overlapping modules during panel generation.")]
        public ProjectUiBuilderGuardrailsData Get()
        {
            return MainThread.Instance.Run(() => new ProjectUiBuilderGuardrailsData
            {
                Purpose = "Provide reusable layout guardrails for panel-style UI generation so the builder creates a bounded root panel, keeps content inside a safe visible area, and separates major modules into non-overlapping zones.",
                ProblemPatterns = new[]
                {
                    "Missing overall container: generated UI elements are not owned by one explicit root panel, so they escape the intended panel bounds.",
                    "Content overflow: long text or stacked widgets extend below or beyond the safe visible frame because no bounded content region or scrolling strategy was reserved.",
                    "Module overlap: independent functional blocks share the same physical area because the layout was not partitioned into dedicated zones before content was placed."
                },
                RootContainerRules = new[]
                {
                    "Always create one explicit RootPanel first and treat it as the only top-level owner of the generated UI.",
                    "Create a SafeContent container inside RootPanel before adding real modules, so padding and visible bounds can be managed centrally.",
                    "Place every functional block under a named sub-container such as Header, LeftZone, RightZone, ContentZone, or FooterZone instead of attaching everything directly to RootPanel.",
                    "Do not leave generated controls as free-floating siblings under Canvas or under unrelated hierarchy nodes."
                },
                SafeBoundsRules = new[]
                {
                    "Reserve inner padding between the outer panel frame and live content to create a safe display area.",
                    "Bound long text inside dedicated text regions; when the content is likely to exceed the available height, use a Scroll View instead of letting the text extend downward.",
                    "After assigning font size and final wording, verify that the text still fits inside its container and remains fully visible.",
                    "Keep separate vertical space for footer actions, state text, and owned-currency labels so they do not slide outside the bottom edge."
                },
                ModuleSeparationRules = new[]
                {
                    "Partition the panel into major layout zones before creating module content.",
                    "Use dedicated left/right containers for side-by-side modules such as an explanation area and an interaction area.",
                    "Use dedicated header/content/footer containers for vertically stacked modules such as banners, card grids, and bottom actions.",
                    "Do not solve layout by placing unrelated modules with absolute positions inside the same shared area."
                },
                BuildSequence = new[]
                {
                    "Create RootPanel and its background/frame.",
                    "Create SafeContent with padding inside RootPanel.",
                    "Split SafeContent into the major structural zones required by the design.",
                    "Create one functional module per zone and keep each module inside its own container.",
                    "Only after the zoning is stable, populate detailed widgets such as long-form text, card grids, resource displays, and buttons.",
                    "Perform a final overflow and overlap review before treating the generated UI as acceptable."
                },
                ValidationChecklist = new[]
                {
                    "Every visible widget is a descendant of RootPanel.",
                    "No important content extends beyond the intended panel or screen-safe bounds.",
                    "Long text is either fully visible inside its region or wrapped in a Scroll View.",
                    "Left/right and top/bottom modules occupy separate containers and do not intersect visually.",
                    "Buttons, owned-currency labels, and state text preserve clear spacing instead of touching or covering nearby modules."
                },
                FailureSignals = new[]
                {
                    "Controls appear outside the decorative background or outside the intended panel frame.",
                    "Text blocks are clipped, pushed below the screen, or require shrinking the font just to stay visible.",
                    "The bottom action area overlaps the main content or owned-currency text.",
                    "Two modules that should be separate share the same anchors or world space and visually cover each other."
                },
                RecommendedHierarchy = new[]
                {
                    "RootPanel",
                    "RootPanel/Background",
                    "RootPanel/SafeContent",
                    "RootPanel/SafeContent/HeaderZone",
                    "RootPanel/SafeContent/LeftZone",
                    "RootPanel/SafeContent/RightZone",
                    "RootPanel/SafeContent/FooterZone"
                },
                CompanionReferences = new[]
                {
                    ".claude/skills/project-ui-panel-workflow/SKILL.md",
                    "Assets/Editor/UnityMcpTools/Tool.ProjectUiPanelWorkflow.cs",
                    "Assets/Editor/UnityMcpTools/Data/ProjectUiPanelWorkflowData.cs"
                }
            });
        }
    }
}
