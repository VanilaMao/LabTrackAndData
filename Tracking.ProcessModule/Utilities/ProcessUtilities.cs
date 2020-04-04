using System;
using LabImage;
using LabSave;
using Tracking.ProcessModule.Models;

namespace Tracking.ProcessModule.Utilities
{
    public static class ProcessUtilities
    {
        public static void SaveFrame(this ProcessModel model, 
            Roi roi, 
            ushort[] data, 
            double calibrateFactor, 
            int localFlexNum)
        {
            if (model.LabSave == null)
            {
                return;
            }
            var motionPosX = model.CurrentStagePos?.X ?? 0;
            var motionPosY = model.CurrentStagePos?.Y ?? 0;
            int wormX = 0, wormY = 0;
            if (roi != null)
            {
                wormX = roi.CenterX;
                wormY =  roi.CenterY;
            }
            var frame = new Frame()
            {
                Time = (DateTime.Now - model.LabSave.StartTime).TotalMilliseconds,
                MotionPosX = motionPosX,
                MotionPosY = motionPosY,
                FrameHeight = model.CurrentImageSizeX,
                FrameWidth = model.CurrentImageSizeY,
                Data = data,
                ImageDistanceMappingToMotionDistance = calibrateFactor,
                ImotionX = (int)(motionPosX / calibrateFactor + wormX),
                ImotionY = (int)(motionPosY / calibrateFactor + wormY),
                IthisPosMotionX = (int)motionPosX,
                IcenterX = roi?.CenterX ?? 0,
                IlocalFlex = localFlexNum
            };
            model.LabSave.AddOneFrame(frame);
        }

        public static void Clear(this ProcessModel model)
        {
            model.CurrentImageSizeX = 0;
            model.CurrentImageSizeY = 0;
            model.CurrentLocalFlexNum = 0;
            model.CurrentStagePos = null;
            model.CurrentRoi = null;
            model.StageAfterCenterDelay = 0;
        }

        public static void Clear(this ProcessResult result)
        {
            result.Counts = 0;
            result.TimeLeft = TimeSpan.Zero;
        }
    }
}
