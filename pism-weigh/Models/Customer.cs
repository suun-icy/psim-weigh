using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pism_weigh.Models
{
    /// <summary>
    /// 客户信息实体类
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// 客户 ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 客户类型（供应商/客户）
        /// </summary>
        public CustomerType Type { get; set; }
        
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactPerson { get; set; }
        
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public Customer()
        {
            Id = Guid.NewGuid().ToString("N");
            IsActive = true;
            CreateTime = DateTime.Now;
        }
    }
    
    /// <summary>
    /// 客户类型枚举
    /// </summary>
    public enum CustomerType
    {
        /// <summary>
        /// 供应商
        /// </summary>
        Supplier = 0,
        
        /// <summary>
        /// 客户
        /// </summary>
        Customer = 1,
        
        /// <summary>
        ///  both
        /// </summary>
        Both = 2
    }
    
    /// <summary>
    /// 货物类型实体类
    /// </summary>
    public class CargoType
    {
        /// <summary>
        /// 货物类型 ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public CargoType()
        {
            Id = Guid.NewGuid().ToString("N");
            Unit = "吨";
            IsActive = true;
        }
    }
}
