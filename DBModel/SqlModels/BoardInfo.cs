using System;
using System.ComponentModel.DataAnnotations;

namespace DBModel.SqlModels
{
    public partial class BoardInfo
    {
        [Key]
        public int Id { get; set; }
        public string Board { get; set; }
        public int? OnlineUser { get; set; }
        public string ChineseDes { get; set; }
        public string Moderators { get; set; }
        public bool? IsOpen { get; set; }
        public DateTime? LastUpdateTime { get; set; }
    }
}
