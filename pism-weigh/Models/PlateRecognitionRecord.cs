using System;

namespace pism_weigh.Models
{
    /// <summary>
    /// 车牌识别记录 — 结构化存储每次识别的结果与图片
    /// </summary>
    public class PlateRecognitionRecord
    {
        /// <summary>唯一标识</summary>
        public string Id { get; set; }

        /// <summary>识别到的车牌号</summary>
        public string PlateNumber { get; set; }

        /// <summary>识别置信度 (0.0-1.0)</summary>
        public double Confidence { get; set; }

        /// <summary>摄像头名称</summary>
        public string CameraName { get; set; }

        /// <summary>摄像头类型 (Generic/ONVIF/Hikvision)</summary>
        public string CameraType { get; set; }

        /// <summary>抓拍原图存储路径</summary>
        public string ImagePath { get; set; }

        /// <summary>关联的车辆档案ID (匹配成功时)</summary>
        public string VehicleId { get; set; }

        /// <summary>识别时间</summary>
        public DateTime RecognizeTime { get; set; }

        /// <summary>来源方式 (Manual=手动输入, Auto=自动抓拍, Anpr=海康ANPR)</summary>
        public string Source { get; set; }

        /// <summary>备注</summary>
        public string Remark { get; set; }

        /// <summary>创建时间</summary>
        public DateTime CreateTime { get; set; }

        public PlateRecognitionRecord()
        {
            Id = Guid.NewGuid().ToString("N");
            RecognizeTime = DateTime.Now;
            CreateTime = DateTime.Now;
            Source = "Auto";
            Confidence = 0.0;
        }
    }
}
