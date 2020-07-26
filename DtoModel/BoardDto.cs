using System;
using System.Collections.Generic;
using System.Text;

namespace DtoModel
{
    /// <summary>
    /// 看板
    /// </summary>
    public class BoardDto
    {
        /// <summary>
        /// Sn
        /// </summary>
        public int Sn { get; set; }

        /// <summary>
        /// 板名
        /// 在BoardInfo table中請參考Board column
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 看板型態
        /// </summary>
        public BoardType BoardType { get; set; }

        /// <summary>
        /// 板標
        /// 在BoardInfo table中請參考ChineseDes column
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 在線人數
        /// </summary>
        public int OnlineCount { get; set; }

        /// <summary>
        /// 板主名單
        /// </summary>
        public IEnumerable<UserDto> Moderators { get; set; }
    }
}
