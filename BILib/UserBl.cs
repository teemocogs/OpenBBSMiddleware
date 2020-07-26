using DBModel.SqlModels;
using DtoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BILib
{
    /// <summary>
    /// 使用者商業邏輯
    /// </summary>
    public class UserBl
    {
        private MWDBContext _dbContext;
        private List<UserDto> _data;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public UserBl(MWDBContext context)
        {
            _dbContext = context;

            SetFakeData();
        }

        /// <summary>
        /// 取得指定使用者
        /// </summary>
        /// <param name="id">帳號</param>
        /// <returns>使用者資訊</returns>
        public UserDto GetUser(string id)
        {
            return _data.SingleOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 建假資料
        /// </summary>
        private void SetFakeData()
        {
            _data = new List<UserDto>();
            for (int i = 1; i <= 100; i++)
            {
                _data.Add(new UserDto
                {
                    Sn = i,
                    Id = $"Ptter{i}",
                    NickName = $"NickName{i}",
                    Money = 100 * i
                });
            }
        }


        public bool CheckUserAvailable(string pttUserID, string pttPassword)
        {
            WSS wss = new WSS();
            return wss.CheckUserAvailable(pttUserID, pttPassword);
        }
    }
}
