using System;
using System.Collections.Generic;

namespace DBModel.SqlModels
{
    public partial class ActionLog
    {
        public int No { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string ClientIp { get; set; }
        public string Description { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
