using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace pism_weigh.Database
{
    /// <summary>
    /// 车辆统计数据
    /// </summary>
    public class VehicleStatItem
    {
        public string PlateNumber { get; set; }
        public string VehicleType { get; set; }
        public string OwnerName { get; set; }
        public int WeighCount { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalTare { get; set; }
        public decimal TotalNet { get; set; }
        public decimal AvgNet { get; set; }
        public decimal MaxNet { get; set; }
        public DateTime? FirstWeigh { get; set; }
        public DateTime? LastWeigh { get; set; }
        public int TotalPrints { get; set; }
    }
}
