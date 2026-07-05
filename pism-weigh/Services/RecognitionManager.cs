using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using pism_weigh.Database;
using pism_weigh.Models;

namespace pism_weigh.Services
{
    /// <summary>
    /// 车牌识别管理器 — 统一处理识别结果的持久化存储
    /// (抓拍图片保存 + 数据库记录 + 车辆关联)
    /// </summary>
    public static class RecognitionManager
    {
        /// <summary>图片存储根目录 (Data/Recognition/)</summary>
        private static readonly string ImageRoot;

        static RecognitionManager()
        {
            ImageRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Recognition");
            if (!Directory.Exists(ImageRoot))
                Directory.CreateDirectory(ImageRoot);
        }

        /// <summary>
        /// 保存识别结果（含抓拍图片）到结构化存储
        /// </summary>
        /// <param name="plateNumber">识别到的车牌号</param>
        /// <param name="snapshot">抓拍原图 (可为null)</param>
        /// <param name="cameraName">摄像头名称</param>
        /// <param name="cameraType">摄像头类型</param>
        /// <param name="source">来源方式 Auto/Manual/Anpr</param>
        /// <param name="confidence">置信度 0-1</param>
        /// <returns>保存的记录</returns>
        public static PlateRecognitionRecord SaveRecognition(
            string plateNumber, Bitmap snapshot,
            string cameraName, string cameraType,
            string source = "Auto", double confidence = 0.85)
        {
            var record = new PlateRecognitionRecord
            {
                PlateNumber = plateNumber,
                Confidence = confidence,
                CameraName = cameraName ?? "N/A",
                CameraType = cameraType ?? "Generic",
                RecognizeTime = DateTime.Now,
                Source = source
            };

            // 保存抓拍图片
            if (snapshot != null)
            {
                record.ImagePath = SaveSnapshotImage(snapshot, record.Id);
            }

            // 自动关联车辆档案
            if (!string.IsNullOrWhiteSpace(plateNumber))
            {
                try
                {
                    var vehicle = DatabaseHelper.GetVehicleByPlate(plateNumber.Trim());
                    if (vehicle != null)
                        record.VehicleId = vehicle.Id;
                }
                catch { }
            }

            // 持久化数据库
            DatabaseHelper.SavePlateRecognitionRecord(record);
            return record;
        }

        /// <summary>
        /// 保存图片到 Data/Recognition/{yyyy-MM}/{id}_{timestamp}.jpg
        /// </summary>
        public static string SaveSnapshotImage(Bitmap image, string recordId)
        {
            if (image == null) return null;

            var dateDir = Path.Combine(ImageRoot, DateTime.Now.ToString("yyyy-MM"));
            if (!Directory.Exists(dateDir))
                Directory.CreateDirectory(dateDir);

            var fileName = string.Format("{0}_{1:HHmmssfff}.jpg", recordId, DateTime.Now);
            var filePath = Path.Combine(dateDir, fileName);

            try
            {
                image.Save(filePath, ImageFormat.Jpeg);
                return filePath;
            }
            catch
            {
                // JPEG 保存失败尝试 PNG
                try
                {
                    var pngPath = Path.ChangeExtension(filePath, ".png");
                    image.Save(pngPath, ImageFormat.Png);
                    return pngPath;
                }
                catch { return null; }
            }
        }

        /// <summary>
        /// 获取识别记录的图片存储根目录
        /// </summary>
        public static string GetImageRoot() { return ImageRoot; }
    }
}
