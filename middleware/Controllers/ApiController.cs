using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBModel;
using DBModel.SqlModels;
using Microsoft.AspNetCore.Mvc;

namespace middleware.Controllers
{
    /// <summary>
    /// 暫時的Controller
    /// </summary>
    [Route("api/")]
    [ApiController]
    public class ApiController : Controller
    {
        NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private readonly MWDBContext _context;
        private readonly BILib.BoardHelper _boardHelper;

        public ApiController(MWDBContext context)
        {
            _context = context;
            _boardHelper = new BILib.BoardHelper(_context);
        }

        /// <summary>
        /// 取得看板的文章清單
        /// </summary>
        /// <remarks>依時間倒序排列</remarks>
        /// <param name="board">看板名稱</param>
        /// <param name="page">分頁</param>
        /// <returns>文章清單</returns>
        [HttpGet]
        [Route("Article/{board}")]
        public ActionResult<APagePosts> GetNewPostList(string board, int page = 1)
        {
            var result = _boardHelper.GetNewPostListByBoardName(board, page);
            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>
        /// 取得指定文章
        /// </summary>
        /// <param name="board">看板名稱</param>
        /// <param name="file">文章檔案名稱</param>
        /// <returns>文章</returns>
        [HttpGet]
        [Route("Article/{board}/{file}")]
        public ActionResult<OnePost> GetPost(string board, string file)
        {
            return Ok(_boardHelper.GetPost(board, file));
        }

        /// <summary>
        /// 取得文章評價數
        /// </summary>
        /// <param name="board">看板名稱</param>
        /// <param name="aid">文章AID</param>
        /// <returns>文章的評價</returns>
        [HttpGet]
        [Route("Rank/{board}/{aid}")]
        public ActionResult<RankByPost> GetRankByPost(string board, string aid)
        {
            return Ok(_boardHelper.GetRank(board, aid));
        }

        /// <summary>
        /// 給文章評價
        /// </summary>
        /// <param name="board">看板名稱</param>
        /// <param name="aid">文章AID</param>
        /// <param name="pttId">PttID</param>
        /// <param name="rank">評價數(-1 ~ 1)</param>
        /// <returns>文章的評價</returns>
        [HttpPost]
        [Route("Rank/{board}/{aid}")]
        public ActionResult<PostRank> SetRankByPost(string board, string aid, string pttId, int rank)
        {
            if ((rank > 1) || (rank < -1))
                rank = 0;
            return Ok(_boardHelper.SetRank(board, aid, pttId, rank));
        }

    }
}
