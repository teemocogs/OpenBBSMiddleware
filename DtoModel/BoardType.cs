using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DtoModel
{
    /// <summary>
    /// 看板型態
    /// </summary>
    public enum BoardType
    {
        /// <summary>
        /// 看板
        /// </summary>
        [Description("看板")]
        Board = 1,

        /// <summary>
        /// 目錄
        /// </summary>
        [Description("目錄")]
        Folder = 2,

        /// <summary>
        /// 分隔線
        /// </summary>
        [Description("分隔線")]
        Divider = 3
    }
}
