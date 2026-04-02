# 地磅称重软件功能完善说明

## .NET Framework 4.5 兼容性确认

**所有代码已确认兼容 .NET Framework 4.5**，已完成以下调整：

### 兼容性修改清单

1. **字符串插值替换** - 将 C# 6.0 的 `$""` 语法改为 `+` 拼接
   - `DatabaseHelper.cs`: 连接字符串、错误消息、参数化查询
   - `ScaleService.cs`: 所有错误消息
   - `PrintService.cs`: 错误消息、打印内容、重量格式化

2. **属性初始化器调整** - 将自动属性初始化移至构造函数
   - `WeighRecord.cs`: Id、CreateTime、UpdateTime
   - `Customer.cs`: Id、IsActive、CreateTime
   - `CargoType.cs`: Id、Unit、IsActive
   - `PrintTemplate.cs`: Title

3. **异步支持** - `System.Threading.Tasks` 在 .NET 4.5 中可用（通过 Microsoft.Bcl.Async 或原生支持）

---

## 已完成的功能增强

### 1. 数据模型层 (Models)

#### WeighRecord.cs - 称重记录实体
- 完整的称重记录数据结构
- 支持车牌号、省份简称分离存储
- 毛重、皮重、净重管理
- 货物类型、发货/收货单位信息
- 司机信息管理（姓名、电话）
- 业务类型枚举（采购入库、销售出库、内部调拨、其他）
- 称重状态枚举（第一次称重、第二次称重、已完成、已取消）
- 操作员信息追踪
- 打印次数统计
- 上传状态标记
- 时间戳管理（创建时间、更新时间）

#### Customer.cs - 客户与货物类型
- 客户信息管理（供应商/客户）
- 联系人、电话、地址
- 货物类型管理
- 启用/禁用状态

### 2. 数据库层 (Database)

#### DatabaseHelper.cs - SQLite 数据库帮助类
- **本地数据存储**：使用 SQLite 轻量级数据库
- **自动初始化**：启动时自动创建数据库和表结构
- **称重记录 CRUD**：
  - SaveWeighRecord: 保存/更新称重记录
  - GetAllWeighRecords: 获取所有记录
  - GetWeighRecordsByPlate: 按车牌号查询
  - DeleteWeighRecord: 删除记录
- **索引优化**：车牌号、时间、状态索引
- **参数化查询**：防止 SQL 注入

### 3. 服务层 (Services)

#### ScaleService.cs - 地磅仪表串口服务
- **多协议支持**：
  - ASCII 格式解析（"=12.345", "GS,+12.345kg"）
  - STX/ETX格式（0x02...0x03）
  - 直接数字格式
- **事件驱动**：
  - WeightReceived: 重量数据回调
  - ConnectionStateChanged: 连接状态变化
  - ErrorOccurred: 错误信息
- **稳定重量检测**：异步等待重量稳定
- **指令发送**：支持向仪表发送控制指令
- **自动重连**：异常处理机制

#### PrintService.cs - 打印服务
- **多模板支持**：
  - Standard: 标准小票模板
  - A4: A4 纸完整单据
  - Receipt80: 80mm 热敏小票
- **打印功能**：
  - 直接打印
  - 打印预览
  - 打印设置对话框
- **自定义内容**：
  - 标题、边距、行高可配置
  - 表格线绘制
  - 签名区域
- **打印机管理**：获取可用打印机列表

### 4. 项目配置更新

#### pism-weigh.csproj
- 添加 System.Data.SQLite 引用
- 注册新增的模型和服务文件

---

## 建议继续完善的功能

### 1. UI 界面增强

#### 主窗口改进
```
- [ ] 仪表盘式重量显示（大字体、实时刷新）
- [ ] 重量曲线图表（显示重量变化趋势）
- [ ] 摄像头抓拍集成（称重时自动拍照）
- [ ] LED 大屏显示输出
- [ ] 语音播报功能（重量读数）
```

#### 数据管理窗口
```
- [ ] 称重记录列表视图（DataGridView）
- [ ] 高级搜索过滤（日期范围、车牌、业务类型）
- [ ] 数据导出（Excel、PDF）
- [ ] 记录详情查看/编辑
- [ ] 批量操作（批量删除、批量导出）
```

#### 基础数据管理
```
- [ ] 客户管理窗口（增删改查）
- [ ] 货物类型管理
- [ ] 车辆黑名单管理
- [ ] 常用车辆快速选择
```

### 2. 业务流程完善

#### 两次称重流程
```
- [ ] 第一次称重（毛重）自动保存
- [ ] 第二次称重（皮重）自动关联
- [ ] 自动计算净重
- [ ] 防作弊检测（重量异常波动）
```

#### 对称重模式
```
- [ ] 先毛后皮
- [ ] 先皮后毛
- [ ] 单次称重（已知皮重）
```

### 3. 数据同步与备份

```
- [ ] 离线数据缓存（网络断开时本地存储）
- [ ] 自动上传队列（网络恢复后批量上传）
- [ ] 数据备份/恢复功能
- [ ] 数据库定期清理（归档历史数据）
```

### 4. 权限与安全

```
- [ ] 角色权限管理（管理员、司磅员、查询员）
- [ ] 操作日志审计
- [ ] 数据修改审批流程
- [ ] 密码加密存储
```

### 5. 报表统计

```
- [ ] 日报表（每日称重汇总）
- [ ] 月报表（月度统计分析）
- [ ] 客户统计报表
- [ ] 货物类型统计
- [ ] 图表可视化（柱状图、饼图）
```

### 6. 硬件集成

```
- [ ] RFID 读卡器集成（自动识别车辆）
- [ ] 道闸控制（称重完成后自动抬杆）
- [ ] 红绿灯控制
- [ ] 红外对射检测（车辆完全上磅）
- [ ] 监控视频叠加（重量信息叠加到视频）
```

### 7. 系统配置

```
- [ ] 串口参数配置保存
- [ ] 打印机选择配置
- [ ] 服务器地址配置
- [ ] 称重参数配置（稳定阈值、超时时间）
- [ ] 界面主题切换
```

---

## 使用示例

### 初始化数据库
```csharp
// 在 Program.cs 或主窗口加载时调用
DatabaseHelper.Initialize();
```

### 使用串口服务读取重量
```csharp
var scaleService = new ScaleService();
scaleService.WeightReceived += weight => {
    // 更新 UI 显示重量
    labelWeight.Text = weight.ToString("F3") + " 吨";
};
scaleService.Connect("COM1", 9600);
```

### 保存称重记录
```csharp
var record = new WeighRecord
{
    PlateNumber = "皖A12345",
    GrossWeight = 50.5m,
    BusinessType = BusinessType.PurchaseIn,
    Status = WeighStatus.FirstWeigh,
    OperatorName = user.userName
};
DatabaseHelper.SaveWeighRecord(record);
```

### 打印称重单
```csharp
var printService = new PrintService();
printService.Print(record, PrintTemplate.Receipt80);
```

---

## 下一步建议

1. **首先**：在主窗口 Form1 中集成新的服务类
2. **然后**：添加数据管理界面（称重记录查询）
3. **接着**：完善两次称重业务流程
4. **最后**：添加报表统计功能

如需实现上述任何功能，请告知具体需求！
