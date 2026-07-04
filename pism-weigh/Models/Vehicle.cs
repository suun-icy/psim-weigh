using System;

namespace pism_weigh.Models
{
    /// <summary>
    /// 车辆档案实体
    /// </summary>
    public class Vehicle
    {
        /// <summary>唯一标识</summary>
        public string Id { get; set; }

        /// <summary>完整车牌号（如"豫A12345"）</summary>
        public string PlateNumber { get; set; }

        /// <summary>省份简称</summary>
        public string Province { get; set; }

        /// <summary>车牌号码部分</summary>
        public string PlateCode { get; set; }

        /// <summary>车辆类型（货车/挂车/罐车/自卸车等）</summary>
        public string VehicleType { get; set; }

        /// <summary>品牌型号</summary>
        public string BrandModel { get; set; }

        /// <summary>核定载重(吨)</summary>
        public decimal RatedLoad { get; set; }

        /// <summary>整备质量(吨)</summary>
        public decimal CurbWeight { get; set; }

        /// <summary>车主姓名</summary>
        public string OwnerName { get; set; }

        /// <summary>车主电话</summary>
        public string OwnerPhone { get; set; }

        /// <summary>所属单位</summary>
        public string OwnerUnit { get; set; }

        /// <summary>燃油类型</summary>
        public string FuelType { get; set; }

        /// <summary>排放标准</summary>
        public string EmissionStandard { get; set; }

        /// <summary>注册日期</summary>
        public DateTime? RegisteredDate { get; set; }

        /// <summary>状态（Active=正常, Frozen=冻结, Blacklisted=黑名单）</summary>
        public string Status { get; set; }

        /// <summary>照片路径</summary>
        public string PhotoPath { get; set; }

        /// <summary>备注</summary>
        public string Remark { get; set; }

        /// <summary>创建时间</summary>
        public DateTime CreateTime { get; set; }

        /// <summary>更新时间</summary>
        public DateTime UpdateTime { get; set; }

        public Vehicle()
        {
            Id = Guid.NewGuid().ToString("N");
            Status = "Active";
            CreateTime = DateTime.Now;
            UpdateTime = DateTime.Now;
        }
    }
}
