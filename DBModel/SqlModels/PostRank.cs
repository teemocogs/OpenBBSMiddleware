using System;
using System.Collections.Generic;

namespace DBModel.SqlModels
{
    public partial class PostRank
    {
        public Guid No { get; set; }
        public string Board { get; set; }
        public string Aid { get; set; }
        public string Pttid { get; set; }
        public int Rank { get; set; }
    }
}
