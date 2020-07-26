using System;
using System.Collections.Generic;

namespace DBModel.SqlModels
{
    public partial class Verification
    {
        public int No { get; set; }
        public string Pttid { get; set; }
        public int VerifyType { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateDateIp { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ModifyDateIp { get; set; }
        public DateTime? AvailableDate { get; set; }
        public string Base5 { get; set; }
    }
}
