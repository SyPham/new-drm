using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMR_API.DTO
{
    public class BottomFactoryDto
    {

    }
    public class AddDispatchParams
    {
        public int MixingInfoID { get; set; }
        public int LineID { get; set; }
        public int BuildingID { get; set; }
        public string Option { get; set; }
    }
    public class GenerateSubpackageParams
    {
        public int MixingInfoID { get; set; }
        public int BuildingID { get; set; }
        public double AmountOfChemical { get; set; }
        public int Can { get; set; }
    }
    public class ScanQRCodeParams
    {
        public string PartNO { get; set; }
        public int GlueID { get; set; }
        public string GlueName { get; set; }
        public int BuildingID { get; set; }
        public DateTime EstimatedStartTime { get; set; }
        public DateTime EstimatedFinishTime { get; set; }
    }

}
