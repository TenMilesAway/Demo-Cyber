### 配置表流程
首先确保 Luban - MiniTemplate - gen.bat 的配置正常，output 指向项目中的 DataTable 文件夹
```
1. 配置表，如新建表，还需在 __tables__.xlsx 中声明
2. 运行 gen.bat，生成表
3. 为每张表建一个 Entity 对象类及 Manager 管理（Entity 的变量不能为属性，必须为字段）
4. 在 DataTableComponent 的 Awake 方法中调用 Manager 的 Init 方法对表进行初始化
5. 将新增道具的 id 在 GlobalDefine.Props 脚本中注册
6. 后续使用时直接通过对应的 Manager.GetData(int id) 来查出数据
```
