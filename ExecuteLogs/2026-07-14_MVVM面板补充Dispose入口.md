## 计划清单
- [x] 查看 MVVM 示例当前的释放逻辑
- [x] 为面板补充可主动调用的 Dispose 入口
- [x] 复用统一释放逻辑并完成编译验证
- [x] 产出本次执行日志

## 最终完成
- 在 `Assets\Scripts\UI\Mvvm\CharacterMvvmDemoScene.cs` 中新增 `public void DisposePanel()`
- `OnDestroy()` 改为复用 `DisposePanel()`，避免释放逻辑分散
- `DisposePanel()` 会主动释放 `view`、`viewModel`，并清空引用
- 产出本次执行日志
