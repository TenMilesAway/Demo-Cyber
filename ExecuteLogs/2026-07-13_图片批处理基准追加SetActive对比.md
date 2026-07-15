## 计划清单
- [x] 理解 Image Batch Benchmark 现有实现与对比项
- [x] 追加 `GameObject.SetActive(false)` 基准入口与结果展示
- [x] 调整重置逻辑，确保三组测试可以重复执行
- [x] 完成编译验证并整理交付

## 最终完成
- 在 `Assets\Scripts\UI\Benchmark\ImageBatchBenchmark.cs` 中新增 `SetActive(false)` 基准模式
- 新增第三个测试按钮与结果文本，支持直接对比 `enabled`、`Color.a = 0`、`GameObject.SetActive(false)`
- 在重置阶段补上 `gameObject.SetActive(true)`，确保关闭对象后的后续轮次与其他测试项都能正常恢复
- 产出本次执行日志
