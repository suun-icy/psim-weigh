using System;

namespace pism_weigh.Models
{
    /// <summary>
    /// 车辆皮重记录
    /// </summary>
    public class TareRecord
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; }
        public decimal TareWeight { get; set; }
        public DateTime? WeighDate { get; set; }
        public string Source { get; set; }
        public string OperatorName { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
