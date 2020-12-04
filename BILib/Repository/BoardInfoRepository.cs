using DBModel.SqlModels;
using System;
using System.Linq;

namespace BILib
{
    public class BoardInfoRepository : GenericRepository<BoardInfo>
    {
        public BoardInfoRepository(MWDBContext context) : base(context)
        {
            SetFakeData();
        }

        private void SetFakeData()
        {
#if !DEBUG
    return;
#endif
            if (GetAsNoTracking().Any()) return;

            for (int i = 1; i <= 100; i++)
            {
                var newItem = new BoardInfo
                {
                    Board = $"Test{i}",
                    ChineseDes = $"[Test{i}] 測試板{i}",
                    IsOpen = true,
                    OnlineUser = 10 * i,
                    Moderators = $"Ptter{i}",
                    LastUpdateTime = DateTime.UtcNow
                };

                Insert(newItem);
            }
            Save();
        }
    }
}
