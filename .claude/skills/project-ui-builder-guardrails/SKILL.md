---
name: project-ui-builder-guardrails
description: Review this repository's reusable UI Builder guardrails before generating activity or panel-style UI. It focuses on preventing missing root panels, off-screen overflow, and overlapping functional modules.
---

# Project / UI Builder Guardrails

Use this skill when building panel-style UI so the generated hierarchy stays inside a single root container, respects safe visible bounds, and keeps separate modules from overlapping.

## The three failure patterns this skill prevents

1. **Missing overall container** — generated controls are created directly under the canvas or under unrelated nodes, so they drift outside the intended panel bounds.
2. **Content overflow** — long text blocks or stacked widgets extend below or beyond the visible frame because no safe-content area, padding, or scrolling strategy was reserved.
3. **Module overlap** — left/right or top/bottom functional blocks are positioned independently without dedicated zones, so they occupy the same space and visually collide.

## Root container rules

- Always start from one full, explicit root panel that owns the whole UI layout.
- All generated visual modules must be children of that root panel or of its named sub-containers — never free-floating siblings without a common boundary owner.
- Inside the root panel, create a content frame or safe-area container before placing real modules.
- Background, decorative elements, content blocks, and footer actions should all live under predictable named containers so later adjustments stay local.

## Safe bounds rules

- Reserve inner padding between the panel edge and live content instead of placing content directly against the panel border.
- Long text blocks must fit inside a bounded text region; if the wording implies long-form reading, place the content inside a Scroll View.
- After assigning font size, verify that text still fits its container and does not fall outside the safe content frame.
- Bottom buttons, owned-currency text, and status labels must each keep dedicated vertical space so they do not slide below the screen edge.

## Module separation rules

- Split the panel into named zones first, then place modules inside those zones; do not position unrelated modules by eyeballing global coordinates.
- For left/right compositions, create separate left and right containers before adding children.
- For top/middle/bottom compositions, create dedicated header, content, and footer containers before filling them.
- Treat each container as that module's territory: explanatory text, card grids, resource summaries, and action buttons should not cross container boundaries.

## Recommended build order

1. Create the root panel.
2. Create the inner safe-content container with padding.
3. Split the safe-content container into major layout zones such as left/right or header/content/footer.
4. Add one functional module per zone.
5. Add long text, grids, and buttons only after their parent zones exist.
6. Run a final overlap and overflow review before considering the UI complete.

## Validation checklist

- Every visible node belongs to the root panel hierarchy.
- No critical text, button, or module extends beyond the intended visible frame.
- Long-form content either fits comfortably or is wrapped in a Scroll View.
- Left/right and top/bottom modules occupy separate containers and do not overlap.
- Footer actions still have breathing room beneath the main content block.

## Typical hierarchy to prefer

- `RootPanel`
- `RootPanel/Background`
- `RootPanel/SafeContent`
- `RootPanel/SafeContent/LeftZone`
- `RootPanel/SafeContent/RightZone`
- `RootPanel/SafeContent/FooterZone`

## Companion usage

- Use this skill together with `project-ui-panel-workflow` when the UI also needs to obey the repository's HotUpdate runtime panel conventions.
- Consult this guardrail skill before generating a new activity panel, and consult `project-ui-panel-workflow` before wiring the prefab into the runtime open/show/close flow.

## How to Call

```bash
unity-mcp-cli run-tool project-ui-builder-guardrails --input '{}'
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
      "$ref": "#/$defs/AIGD.ProjectUiBuilderGuardrailsData"
    }
  },
  "$defs": {
    "System.String-1": {
      "type": "array",
      "items": {
        "type": "string"
      }
    },
    "AIGD.ProjectUiBuilderGuardrailsData": {
      "type": "object",
      "properties": {
        "Purpose": {
          "type": "string",
          "description": "Short statement of what this UI Builder guardrail skill is for."
        },
        "ProblemPatterns": {
          "$ref": "#/$defs/System.String-1",
          "description": "The three recurring UI construction failures this tool is designed to prevent."
        },
        "RootContainerRules": {
          "$ref": "#/$defs/System.String-1",
          "description": "Mandatory rules for creating a single root container and keeping child content inside it."
        },
        "SafeBoundsRules": {
          "$ref": "#/$defs/System.String-1",
          "description": "Rules for keeping text, panels, and modules inside a safe visible area without running off-screen."
        },
        "ModuleSeparationRules": {
          "$ref": "#/$defs/System.String-1",
          "description": "Rules for splitting the screen into non-overlapping functional zones so independent modules do not stack on top of each other."
        },
        "BuildSequence": {
          "$ref": "#/$defs/System.String-1",
          "description": "Recommended construction order for assembling a new activity-style UI safely."
        },
        "ValidationChecklist": {
          "$ref": "#/$defs/System.String-1",
          "description": "Checks to run before considering the generated UI layout acceptable."
        },
        "FailureSignals": {
          "$ref": "#/$defs/System.String-1",
          "description": "Warning signs that usually indicate the builder ignored container, bounds, or separation rules."
        },
        "RecommendedHierarchy": {
          "$ref": "#/$defs/System.String-1",
          "description": "Example layout hierarchy or zoning strategy that the builder should prefer for panel-style activity UIs."
        },
        "CompanionReferences": {
          "$ref": "#/$defs/System.String-1",
          "description": "Related project files or skills that should be consulted together with this guardrail tool."
        }
      }
    }
  },
  "required": [
    "result"
  ]
}
```

