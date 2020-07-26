using System;
using System.Collections.Generic;

namespace DBModel.SqlModels
{
    public partial class BoardInfo
    {
        public string Board { get; set; }
        public int? OnlineUser { get; set; }
        public string ChineseDes { get; set; }
        public string Moderators { get; set; }
        public bool? IsOpen { get; set; }
        public DateTime? LastUpdateTime { get; set; }
    }
}
