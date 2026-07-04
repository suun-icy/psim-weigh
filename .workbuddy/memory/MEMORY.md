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
1. ✅ 脱机手动模式 — 已完成 2026-07-04
2. ⬜ 集成 DatabaseHelper（称重完成时本地保存）
3. ⬜ 集成 PrintService（button7 真正执行打印）
4. ⬜ 修复 button7 数据上传逻辑（字段映射错误）
5. ⬜ 修复停止位选项（去掉 "0"，增加 "1.5" 和 "2"）

## 架构要点
- 三个服务类未集成：ScaleService(289行)、PrintService(283行)、DatabaseHelper(529行)
- Form1 所有逻辑内联，UI 和业务高度耦合
- 服务器地址硬编码在代码中
