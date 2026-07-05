using System;
using System.Drawing;

namespace pism_weigh.Interfaces
{
    /// <summary>
    /// 摄像头服务接口 — 统一抽象海康/通用/USB摄像头
    /// 业务层通过此接口调用，不依赖具体SDK
    /// </summary>
    public interface ICameraService
    {
        /// <summary>帧捕获完成事件（Bitmap 为完整帧）</summary>
        event Action<Bitmap> FrameCaptured;

        /// <summary>连接摄像头</summary>
        bool Connect(Models.CameraConfig config);

        /// <summary>断开连接</summary>
        void Disconnect();

        /// <summary>抓拍当前帧</summary>
        Bitmap CaptureSnapshot();

        /// <summary>是否已连接</summary>
        bool IsConnected { get; }

        /// <summary>服务名称（用于UI显示）</summary>
        string ServiceName { get; }
    }

    /// <summary>
    /// 车牌识别服务接口 — 从图片中识别车牌号
    /// </summary>
    public interface ILPRService
    {
        /// <summary>识别成功事件</summary>
        event Action<string> PlateRecognized;

        /// <summary>识别单张图片中的车牌号</summary>
        string Recognize(Bitmap image);

        /// <summary>引擎是否可用</summary>
        bool IsAvailable { get; }

        /// <summary>引擎名称（OCR/ANPR）</summary>
        string EngineName { get; }
    }
}
