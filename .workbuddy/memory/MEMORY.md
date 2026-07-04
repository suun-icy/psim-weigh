# psim-weigh 项目记忆

## 项目概况
- 地磅称重管理桌面软件，.NET Framework 4.7.2 + WinForms (C# 5.0)
- SQLite 3 本地数据库，Newtonsoft.Json 13.0.4
- 一期完成度: 33.3% (6/18)

## 开发约束
1. 不修改已有业务逻辑
2. 不修改数据库字段名
3. 不修改串口通信协议
4. 新功能模块化设计
5. 每次一个功能点
6. 兼容 .NET 4.7.2 / C# 5.0 / WinForms / Windows 7+
7. 不得引入 .NET Core/.NET 5+ 依赖
8. 优先用 Interface 解耦
9. 保留手动模式，不强制依赖硬件

## 一期优先开发顺序
1. ✅ 脱机手动模式 — 2026-07-04
2. ✅ 集成 DatabaseHelper — 2026-07-04
3. ✅ 集成 PrintService — 2026-07-04
4. ✅ 修复停止位选项 — 2026-07-04
5. ✅ 参数配置持久化 + 自动连接 — 2026-07-04
6. ✅ 称重记录查询与汇总分析 — 2026-07-04
7. ✅ 主界面信息输入补充 + 历史快速填充 — 2026-07-04
8. ✅ 主界面UI布局优化与美化 — 2026-07-04
9. ✅ 系统设置界面 — 2026-07-04

## 新增文件
- `Models/AppConfig.cs` — JSON 配置文件持久化
- `QueryForm.cs` + `QueryForm.Designer.cs` — 查询分析窗口

## 架构要点
- ScaleService 和 PrintService 已被部分引用但未充分使用
- DatabaseHelper 已在 Form1.cs 多处使用（SaveWeighRecord / GetLatestOpenRecordByPlate / SaveRawWeightLog）
- Program.cs 启动时调用 DatabaseHelper.Initialize()
- Form1.cs 已实现完整的先毛后皮/先皮后毛两阶段称重流程
- 服务器地址仍硬编码在代码中
