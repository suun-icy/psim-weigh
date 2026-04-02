# 地磅称重软件功能完善说明

## 已完成的功能

### 1. 数据模型层 (Models)

#### WeighRecord.cs - 称重记录实体
- 基础信息：车牌号、毛重、皮重、净重、货物类型等
- 业务信息：发货单位、收货单位、司机信息、业务类型
- 状态管理：称重状态（第一次/第二次/已完成/已取消）
- **新增记录类别**：
  - `Valid` (有效记录) - 正常称重完成的记录
  - `Temporary` (临时修改) - 被修改过需要审核的记录
  - `Invalid` (废弃记录) - 无效或错误的记录
  - `PendingReview` (待审核) - 修改后等待审核确认
- 审核信息：审核人 ID、姓名、审核时间
- 修改历史：JSON 格式存储修改记录

#### ModifyHistory.cs - 修改历史记录
- `ModifyHistoryItem` - 单次修改记录项
  - 修改时间、修改人信息
  - 修改字段名、原值、新值
  - 修改原因
- `WeighRecordModifyHistory` - 完整修改历史管理
  - JSON 序列化/反序列化（兼容.NET 4.7.2）
  - 添加修改记录方法

### 2. 数据库层 (Database)

#### DatabaseHelper.cs - SQLite 数据库操作
**数据库表结构更新**：
- 新增字段：Category、ModifyHistory、ReviewerId、ReviewerName、ReviewTime

**新增查询方法**：
- `GetWeighRecordsByCategory()` - 按记录类别查询
- `GetWeighRecordsByDateRange()` - 按时间范围查询
- `GetPendingReviewRecords()` - 获取待审核记录

**新增操作方法**：
- `ReviewWeighRecord()` - 审核记录（通过/拒绝）
- `MarkAsTemporary()` - 标记为临时修改
- `MarkAsInvalid()` - 标记为废弃
- 所有查询方法均支持新增字段的读取

### 3. 项目配置

#### pism-weigh.csproj
- 框架升级：从 .NET Framework 4.5 升级到 **4.7.2**
- 新增文件引用：Models/ModifyHistory.cs

## 使用示例

### 1. 保存称重记录
```csharp
var record = new WeighRecord
{
    PlateNumber = "京 A12345",
    GrossWeight = 50.5m,
    TareWeight = 15.2m,
    NetWeight = 35.3m,
    BusinessType = BusinessType.PurchaseIn,
    Status = WeighStatus.Completed,
    Category = RecordCategory.Valid, // 默认为有效记录
    OperatorId = "user001",
    OperatorName = "张三"
};
DatabaseHelper.SaveWeighRecord(record);
```

### 2. 修改记录并标记为待审核
```csharp
// 创建修改历史
var history = WeighRecordModifyHistory.FromJsonString(record.ModifyHistory);
history.AddItem(new ModifyHistoryItem
{
    ModifierId = currentUser.Id,
    ModifierName = currentUser.Name,
    FieldName = "NetWeight",
    OldValue = record.NetWeight.ToString(),
    NewValue = "36.0",
    Reason = "称重设备校准后修正"
});

// 更新记录
record.NetWeight = 36.0m;
record.ModifyHistory = history.ToJsonString();
record.Category = RecordCategory.PendingReview; // 标记为待审核
DatabaseHelper.SaveWeighRecord(record);
```

### 3. 审核记录
```csharp
// 审核通过
DatabaseHelper.ReviewWeighRecord(recordId, reviewerId, reviewerName, true);

// 审核拒绝（标记为废弃）
DatabaseHelper.ReviewWeighRecord(recordId, reviewerId, reviewerName, false);
```

### 4. 查询记录
```csharp
// 查询所有有效记录
var validRecords = DatabaseHelper.GetWeighRecordsByCategory(RecordCategory.Valid);

// 查询待审核记录
var pendingRecords = DatabaseHelper.GetPendingReviewRecords();

// 按时间范围查询
var records = DatabaseHelper.GetWeighRecordsByDateRange(
    DateTime.Now.AddDays(-30), 
    DateTime.Now
);

// 组合查询示例：查询某时间段的有效记录
var allRecords = DatabaseHelper.GetAllWeighRecords();
var filtered = allRecords
    .Where(r => r.Category == RecordCategory.Valid && 
                r.CreateTime >= startDate && 
                r.CreateTime <= endDate)
    .ToList();
```

### 5. 查看修改历史
```csharp
var record = GetRecordById(recordId);
var history = WeighRecordModifyHistory.FromJsonString(record.ModifyHistory);

foreach (var item in history.Items)
{
    Console.WriteLine(string.Format("时间：{0}, 修改人：{1}, 字段：{2}, 原值：{3}, 新值：{4}, 原因：{5}",
        item.ModifyTime,
        item.ModifierName,
        item.FieldName,
        item.OldValue,
        item.NewValue,
        item.Reason));
}
```

## 工作流程建议

### 正常称重流程
1. 第一次称重（毛重）→ 保存记录，状态=FirstWeigh，类别=Valid
2. 第二次称重（皮重）→ 更新记录，状态=Completed，类别=Valid
3. 打印磅单 → 更新 PrintCount

### 修改审核流程
1. 操作员申请修改 → 记录修改历史，类别=PendingReview
2. 管理员审核：
   - 通过 → 类别=Valid
   - 拒绝 → 类别=Invalid
3. 紧急修改 → 类别=Temporary（先修改后补审）

### 废弃记录流程
1. 发现错误记录 → 标记为 Invalid
2. 可选择性保留用于审计追踪
3. 查询时默认过滤掉 Invalid 记录

## 后续可扩展功能

### UI 界面
- [ ] 历史记录查询窗口（支持多条件筛选）
- [ ] 待审核记录列表及审核界面
- [ ] 修改历史查看对话框
- [ ] 记录类别标识（不同颜色区分）

### 报表统计
- [ ] 日报/月报/年报
- [ ] 按类别统计（有效/废弃记录占比）
- [ ] 修改记录审计报告
- [ ] 操作员工作量统计

### 权限管理
- [ ] 修改权限控制
- [ ] 审核权限控制
- [ ] 废弃记录权限控制
- [ ] 操作日志记录

### 数据导出
- [ ] Excel 导出（支持按类别筛选）
- [ ] PDF 磅单导出
- [ ] 数据备份与恢复

### 高级功能
- [ ] 自动审核规则（小额修改免审）
- [ ] 修改预警（频繁修改提示）
- [ ] 数据同步到服务器
- [ ] 移动端审核支持

## 注意事项

1. **兼容性**：所有代码已确保兼容 .NET Framework 4.7.2 和 C# 5.0
2. **数据库迁移**：现有数据库会自动添加新字段，无需手动迁移
3. **JSON 解析**：使用自定义简单解析器，不依赖第三方库
4. **数据安全**：建议定期备份数据库文件（Data/weigh.db）
