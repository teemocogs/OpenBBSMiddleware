using System;
using System.Collections.Generic;
using System.Text;

namespace DtoModel
{
    /// <summary>
    /// 鄉民
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Sn
        /// </summary>
        public int Sn { get; set; }

        /// <summary>
        /// 使用者Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 暱稱
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// P幣
        /// </summary>
        public int Money { get; set; }
    }
}
