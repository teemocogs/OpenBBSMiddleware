using DBModel;
using DBModel.SqlModels;
using DtoModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BILib
{
    /// <summary>
    /// 看板商業邏輯
    /// </summary>
    public class BoardBl
    {
        private MWDBContext _dbContext;
        private List<BoardDto> _data;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public BoardBl(MWDBContext context)
        {
            _dbContext = context;

            SetFakeData();
        }

        /// <summary>
        /// 取得看板清單
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="count">每頁數量</param>
        /// <returns>看板清單</returns>
        public IEnumerable<BoardDto> GetBoards()
        {
            return _dbContext.BoardInfo.ToDtos();
        }

        /// <summary>
        /// 取得指定看板
        /// </summary>
        /// <param name="name">看板名稱</param>
        /// <returns>看板</returns>
        public BoardDto GetBoard(string name)
        {
            return _dbContext.BoardInfo.SingleOrDefault(p => p.Board == name).ToDto();
        }

        /// <summary>
        /// 搜尋看板
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <returns>符合的看板清單</returns>
        public IEnumerable<BoardDto> SearchBoards(string keyword)
        {
            return _dbContext.BoardInfo.Where(p => p.Board.Contains(keyword)).ToDtos();
        }

        /// <summary>
        /// 取得熱門看板清單
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="count">每頁數量</param>
        /// <returns>熱門看板清單</returns>
        public IEnumerable<BoardDto> GetPopularBoards(int page, int count)
        {
            return _data.Skip((page - 1) * count).Take(count);
        }

        /// <summary>
        /// 取得我的最愛看板清單
        /// </summary>
        /// <param name="id">使用者帳號</param>
        /// <param name="page">頁數</param>
        /// <param name="count">每頁數量</param>
        /// <returns>我的最愛看板清單</returns>
        public IEnumerable<BoardDto> GetFavoriteBoards(string id, int page, int count)
        {
            return _data.Skip((page - 1) * count).Take(count);
        }

        /// <summary>
        /// 依分類取得看板清單
        /// </summary>
        /// <param name="name">分類名稱</param>
        /// <param name="page">頁數</param>
        /// <param name="count">每頁數量</param>
        /// <returns>看板清單</returns>
        public IEnumerable<BoardDto> GetCategoryBoards(string name, int page, int count)
        {
            return _data.Skip((page - 1) * count).Take(count);
        }

        /// <summary>
        /// 取得指定看板版主名單
        /// </summary>
        /// <param name="name">看板名稱</param>
        /// <returns>板主名單</returns>
        public string[] GetBoardModerators(string name)
        {
            var board = _dbContext.BoardInfo.SingleOrDefault(x => x.Board == name);
            if (board == null) return null;

            var result = board.ToDto().Moderators.Select(p => p.Id).ToArray();
            return result;
        }

        /// <summary>
        /// 建假資料
        /// </summary>
        private void SetFakeData()
        {
            _data = new List<BoardDto>();
            for (int i = 1; i <= 100; i++)
            {
                var moderators = new List<UserDto>();
                for (int j = 1; j <= 3; j++)
                {
                    moderators.Add(new UserDto
                    {
                        Sn = j,
                        Id = $"Ptter{j}",
                        NickName = $"NickName{j}",
                        Money = 100 * j
                    });
                }

                _data.Add(new BoardDto
                {
                    Sn = i,
                    Name = $"Test{i}",
                    BoardType = BoardType.Board,
                    Title = $"[Test{i}] 測試板{i}",
                    OnlineCount = 10 * i,
                    Moderators = moderators
                });
            }
        }
    }

    public static class BoardInfoExtension
    {
        public static BoardDto ToDto(this BoardInfo source)
        {
            if (source == null) return null;

            var moderatorSn = 0;
            var getModeratorSn = new Func<int>(() => moderatorSn += 1);

            return new BoardDto
            {
                Sn = 1,
                Name = source.Board,
                BoardType = BoardType.Board,
                Title = source.ChineseDes,
                OnlineCount = source.OnlineUser ?? 0,
                Moderators = source.Moderators == "(無)" ? Enumerable.Empty<UserDto>() : source.Moderators.Split('/').Select(id => new UserDto
                {
                    Sn = getModeratorSn(),
                    Id = id
                })
            };
        }

        public static IQueryable<BoardDto> ToDtos(this IQueryable<BoardInfo> source)
        {
            var results = source.Select(p => p.ToDto());

            return results;
        }
    }

    public static class BoardDtoExtension
    {
        public static List<BoardDto> SetSerialNumber(this IEnumerable<BoardDto> source)
        {
            var sn = 1;
            var list = source.ToList();

            list.ForEach(p => p.Sn = sn++);

            return list;
        }
    }
}
