using LabImage;
using LabSave;

namespace Tracking.ProcessModule.Models
{
    public class ProcessModel
    {
        public bool IsSaving { get; set; }

        public bool IsTracking { get; set; }

        public string SaveFileName { get; set; }

        public bool IsCalibration { get; set; }

        public bool IsCalibrateObjectMarked { get; set; }


        public ILabImage LabImage { get; set; }

        public ILabSave LabSave { get; set; }

        public int CurrentImageSizeX { get; set; }

        public int CurrentImageSizeY { get; set; }

        public Roi CurrentRoi { get; set; }
        public Roi CalibrateRoi { get; set; }

        public PosPoints? CalibrateStagePos { get; set; }
        public PosPoints? CurrentStagePos { get; set; }

        // somthing to prevent stage processing right away when cell is moving back 
        //becuase possible shake may make the processing not accurate after moving 
        // so put a small delay 

        public int CurrentLocalFlexNum { get; set; }
        public int StageAfterCenterDelay { get; set; }
    }
}
