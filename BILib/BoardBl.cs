using DBModel.SqlModels;
using DtoModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BILib
{
    /// <summary>
    /// 看板商業邏輯
    /// </summary>
    public class BoardBl
    {
        private readonly BoardInfoRepository _repository;
        private readonly ILogger<BoardBl> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public BoardBl(BoardInfoRepository repository, ILogger<BoardBl> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// 取得看板清單
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="count">每頁數量</param>
        /// <returns>看板清單</returns>
        public IEnumerable<BoardDto> GetBoards()
        {
            return _repository.GetAsNoTracking()
                              .ToDtos();
        }

        /// <summary>
        /// 取得指定看板
        /// </summary>
        /// <param name="name">看板名稱</param>
        /// <returns>看板</returns>
        public BoardDto GetBoard(string name)
        {
            return _repository.GetAsNoTracking()
                              .SingleOrDefault(p => p.Board == name)
                              .ToDto();
        }

        /// <summary>
        /// 搜尋看板
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <returns>符合的看板清單</returns>
        public IEnumerable<BoardDto> SearchBoards(string keyword)
        {
            return _repository.GetAsNoTracking()
                              .Where(p => p.Board.Contains(keyword))                              
                              .ToDtos();
        }

        /// <summary>
        /// 取得熱門看板清單
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="count">每頁數量</param>
        /// <returns>熱門看板清單</returns>
        public IEnumerable<BoardDto> GetPopularBoards(int page, int count)
        {
            return _repository.GetAsNoTracking()
                              //.Where(b => b)  // TODO: 熱門看板的條件
                              .Skip((page - 1) * count)
                              .Take(count)
                              .ToDtos();
        }

        /// <summary>
        /// 取得我的最愛看板清單
        /// </summary>
        /// <param name="userId">使用者帳號</param>
        /// <param name="page">頁數</param>
        /// <param name="count">每頁數量</param>
        /// <returns>我的最愛看板清單</returns>
        public IEnumerable<BoardDto> GetFavoriteBoards(string userId, int page, int count)
        {
            return _repository.GetAsNoTracking()
                              //.Where(b => b)  // TODO: 最愛看板的條件
                              .Skip((page - 1) * count)
                              .Take(count)
                              .ToDtos();
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
            return _repository.GetAsNoTracking()
                              //.Where(b => b)  // TODO: 分類名稱條件
                              .Skip((page - 1) * count)
                              .Take(count)
                              .ToDtos();
        }

        /// <summary>
        /// 取得指定看板版主名單
        /// </summary>
        /// <param name="name">看板名稱</param>
        /// <returns>板主名單</returns>
        public IEnumerable<string> GetBoardModerators(string name)
        {
            return _repository.GetAsNoTracking()
                              .SingleOrDefault(b => b.Board == name)?
                              .ToDto()
                              .Moderators?
                              .Select(p => p.Id);
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

        public static IQueryable<BoardDto> ToDtos(this IEnumerable<BoardInfo> source)
        {
            var results = source.AsQueryable()
                                .Select(p => p.ToDto());
            return results;
        }
    }

    public static class BoardDtoExtension
    {
        public static IEnumerable<BoardDto> SetSerialNumber(this IEnumerable<BoardDto> source)
        {
            int sn = 1;
            var list = source.ToList();
            list.ForEach(p => p.Sn = sn++);

            return list;
        }
    }
}
