## 计划清单
- [x] 阅读现有 Unity MCP Tool 与 Skill 的组织方式
- [x] 将缺少整体容器、内容溢出边界、模块互相重叠抽象成通用 UI Builder 规则
- [x] 新增通用 UI Builder Unity Tool 与结构化返回模型
- [x] 生成对应的 `.claude` Skill 文件并检查可调用结果
- [x] 产出本次执行日志

## 最终完成
- 新增 `Assets/Editor/UnityMcpTools/Tool.ProjectUiBuilderGuardrails.cs`，提供只读的 `project-ui-builder-guardrails` Unity Tool
- 新增 `Assets/Editor/UnityMcpTools/Data/ProjectUiBuilderGuardrailsData.cs`，结构化返回根容器、安全边界、模块分区、构建顺序与校验清单
- 生成 `.claude/skills/project-ui-builder-guardrails/SKILL.md`，供后续 UI 搭建前直接调用
- 通过 MCP 调用新工具，确认输出结构与预期规则一致
- 产出本次执行日志
