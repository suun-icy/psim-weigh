using System;

namespace pism_weigh.Models
{
    /// <summary>
    /// 车辆进出场记录
    /// </summary>
    public class VehicleLog
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; }
        public string Direction { get; set; }
        public DateTime LogTime { get; set; }
        public string RelatedWeighId { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal TareWeight { get; set; }
        public string OperatorName { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
