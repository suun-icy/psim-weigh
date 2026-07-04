using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pism_weigh.Models
{
    /// <summary>
    /// 称重记录实体类 - 用于本地数据库存储
    /// </summary>
    public class WeighRecord
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 车牌号（完整）
        /// </summary>
        public string PlateNumber { get; set; }
        
        /// <summary>
        /// 省份简称
        /// </summary>
        public string Province { get; set; }
        
        /// <summary>
        /// 车牌号码部分
        /// </summary>
        public string PlateCode { get; set; }
        
        /// <summary>
        /// 毛重（吨）
        /// </summary>
        public decimal GrossWeight { get; set; }
        
        /// <summary>
        /// 皮重（吨）
        /// </summary>
        public decimal TareWeight { get; set; }
        
        /// <summary>
        /// 净重（吨）
        /// </summary>
        public decimal NetWeight { get; set; }
        
        /// <summary>
        /// 货物类型
        /// </summary>
        public string CargoType { get; set; }
        
        /// <summary>
        /// 发货单位
        /// </summary>
        public string Sender { get; set; }
        
        /// <summary>
        /// 收货单位
        /// </summary>
        public string Receiver { get; set; }
        
        /// <summary>
        /// 司机姓名
        /// </summary>
        public string DriverName { get; set; }
        
        /// <summary>
        /// 司机电话
        /// </summary>
        public string DriverPhone { get; set; }
        
        /// <summary>
        /// 业务类型（进厂/出厂）
        /// </summary>
        public BusinessType BusinessType { get; set; }
        
        /// <summary>
        /// 称重状态
        /// </summary>
        public WeighStatus Status { get; set; }
        
        /// <summary>
        /// 第一次称重时间（毛重）
        /// </summary>
        public DateTime? FirstWeighTime { get; set; }
        
        /// <summary>
        /// 第二次称重时间（皮重）
        /// </summary>
        public DateTime? SecondWeighTime { get; set; }
        
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime CompleteTime { get; set; }
        
        /// <summary>
        /// 操作员 ID
        /// </summary>
        public string OperatorId { get; set; }
        
        /// <summary>
        /// 操作员姓名
        /// </summary>
        public string OperatorName { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// 打印次数
        /// </summary>
        public int PrintCount { get; set; }
        
        /// <summary>
        /// 是否已上传服务器
        /// </summary>
        public bool IsUploaded { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        
        /// <summary>
        /// 记录类别（有效/临时修改/废弃/待审核）
        /// </summary>
        public RecordCategory Category { get; set; }
        
        /// <summary>
        /// 修改历史 JSON
        /// </summary>
        public string ModifyHistory { get; set; }
        
        /// <summary>
        /// 审核人 ID
        /// </summary>
        public string ReviewerId { get; set; }
        
        /// <summary>
        /// 审核人姓名
        /// </summary>
        public string ReviewerName { get; set; }
        
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ReviewTime { get; set; }
        
        /// <summary>
        /// 关联车辆档案 ID（Vehicles 表外键）
        /// </summary>
        public string VehicleId { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public WeighRecord()
        {
            Id = Guid.NewGuid().ToString("N");
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
            Category = RecordCategory.Valid;
            PrintCount = 0;
            IsUploaded = false;
        }
    }
    
    /// <summary>
    /// 业务类型枚举
    /// </summary>
    public enum BusinessType
    {
        /// <summary>
        /// 采购入库（进厂）
        /// </summary>
        PurchaseIn = 0,
        
        /// <summary>
        /// 销售出库（出厂）
        /// </summary>
        SalesOut = 1,
        
        /// <summary>
        /// 内部调拨
        /// </summary>
        Transfer = 2,
        
        /// <summary>
        /// 其他
        /// </summary>
        Other = 3
    }
    
    /// <summary>
    /// 称重状态枚举
    /// </summary>
    public enum WeighStatus
    {
        /// <summary>
        /// 第一次称重（毛重）
        /// </summary>
        FirstWeigh = 0,
        
        /// <summary>
        /// 第二次称重（皮重）
        /// </summary>
        SecondWeigh = 1,
        
        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 2,
        
        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled = 3
    }
    
    /// <summary>
    /// 记录类别枚举 - 用于区分记录的有效性
    /// </summary>
    public enum RecordCategory
    {
        /// <summary>
        /// 有效记录 - 正常称重完成的记录
        /// </summary>
        Valid = 0,
        
        /// <summary>
        /// 临时修改 - 被修改过的记录，需要审核
        /// </summary>
        Temporary = 1,
        
        /// <summary>
        /// 废弃记录 - 无效或错误的记录
        /// </summary>
        Invalid = 2,
        
        /// <summary>
        /// 待审核 - 修改后等待审核确认
        /// </summary>
        PendingReview = 3
    }
}
