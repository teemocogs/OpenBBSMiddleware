using BILib;
using DtoModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace middleware.Controllers
{
    /// <summary>
    /// 看板
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        /// <summary>
        /// 看板商業邏輯
        /// </summary>
        private readonly BoardBl _boardBl;
        

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="boardBl"></param>
        public BoardController(BoardBl boardBl)
        {
            _boardBl = boardBl;
        }

        /// <summary>
        /// 取得看板清單
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="count">每頁看板數</param>
        /// <returns>看板清單</returns>
        [HttpGet]
        public ActionResult<IEnumerable<BoardDto>> Get(int page = 1, int count = 20)
        {
            // 參數檢查
            var validationResults = new List<ValidationResult>();
            if (page <= 0)
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(page) }));
            }
            if (count <= 0)
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(count) }));
            }
            if (validationResults.Any())
            {
                return BadRequest(validationResults);
            }

            var result = _boardBl.GetBoards().Skip((page - 1) * count).Take(count).SetSerialNumber();
            if (!result.Any())
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>
        /// 取得指定看板
        /// </summary>
        /// <param name="name">看板名稱</param>
        /// <returns>看板</returns>
        [Route("{name}")]
        [HttpGet]
        public ActionResult<BoardDto> Get(string name)
        {
            // 參數檢查
            var validationResults = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(name))
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(name) }));
            }
            if (validationResults.Any())
            {
                return BadRequest(validationResults);
            }

            var result = _boardBl.GetBoard(name);
            if (result is null)
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>
        /// 搜尋看板
        /// </summary>
        /// <param name="keyword">看板名搜尋字</param>
        /// <returns>結果清單</returns>
        [Route("Search")]
        [HttpGet]
        public ActionResult<IEnumerable<BoardDto>> Search([FromQuery] string keyword)
        {
            // 參數檢查
            var validationResults = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(keyword))
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(keyword) }));
            }
            if (validationResults.Any())
            {
                return BadRequest(validationResults);
            }

            var result = _boardBl.SearchBoards(keyword).SetSerialNumber();
            if (!result.Any())
            {
                return NoContent();
            }

            return Ok(result);
        }

        /// <summary>
        /// 取得熱門看板清單
        /// </summary>
        /// <param name="page">頁數</param>
        /// <param name="count">每頁看板數</param>
        /// <returns>熱門看板清單</returns>
        [Route("Popular")]
        [HttpGet]
        public ActionResult<IEnumerable<BoardDto>> Popular(int page = 1, int count = 20)
        {
            // 參數檢查
            var validationResults = new List<ValidationResult>();
            if (page <= 0)
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(page) }));
            }
            if (count <= 0)
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(count) }));
            }
            if (validationResults.Any())
            {
                return BadRequest(validationResults);
            }

            return Ok(_boardBl.GetPopularBoards((page - 1) * count, count));
        }

        /// <summary>
        /// 取得我的最愛看板清單
        /// </summary>
        /// <param name="id">使用者帳號</param>
        /// <param name="page">頁數</param>
        /// <param name="count">每頁看板數</param>
        /// <returns>我的最愛看板清單</returns>
        [Route("Favorite/{id}")]
        [HttpGet]
        public ActionResult<IEnumerable<BoardDto>> Favorite(string id, int page = 1, int count = 20)
        {
            // 參數檢查
            var validationResults = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(id))
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(id) }));
            }
            if (page <= 0)
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(page) }));
            }
            if (count <= 0)
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(count) }));
            }
            if (validationResults.Any())
            {
                return BadRequest(validationResults);
            }

            return Ok(_boardBl.GetFavoriteBoards(id, page, count));
        }

        /// <summary>
        /// 依分類取得看板清單
        /// </summary>
        /// <param name="name">分類名稱</param>
        /// <param name="page">頁數</param>
        /// <param name="count">每頁看板數</param>
        /// <returns>看板清單</returns>
        [Route("Category/{name}")]
        [HttpGet]
        public ActionResult<IEnumerable<BoardDto>> Category(string name, int page = 1, int count = 20)
        {
            // 參數檢查
            var validationResults = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(name))
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(name) }));
            }
            if (page <= 0)
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(page) }));
            }
            if (count <= 0)
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(count) }));
            }
            if (validationResults.Any())
            {
                return BadRequest(validationResults);
            }

            return Ok(_boardBl.GetCategoryBoards(name, page, count));
        }

        /// <summary>
        /// 取得指定看板版主名單
        /// </summary>
        /// <param name="name">看板名稱</param>
        /// <returns>版主名單</returns>
        [Route("{name}/Moderators")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> Moderators(string name)
        {
            // 參數檢查
            var validationResults = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(name))
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(name) }));
            }
            if (validationResults.Any())
            {
                return BadRequest(validationResults);
            }

            var result = _boardBl.GetBoardModerators(name);
            if (result == null)
            {
                return NoContent();
            }
            return Ok(result);
        }
    }
}