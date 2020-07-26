using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BILib;
using DBModel.SqlModels;
using DtoModel;
using Microsoft.AspNetCore.Mvc;

namespace middleware.Controllers
{
    /// <summary>
    /// 使用者
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Database context
        /// </summary>
        private readonly MWDBContext _context;

        /// <summary>
        /// 使用者商業邏輯
        /// </summary>
        private readonly UserBl _userBl;

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="context">Database context</param>
        public UserController(MWDBContext context)
        {
            _context = context;
            _userBl = new UserBl(_context);
        }

        /// <summary>
        /// 取得指定使用者資訊
        /// </summary>
        /// <param name="id">帳號</param>
        /// <returns>使用者資訊</returns>
        [Route("{id}")]
        [HttpGet]
        public ActionResult<UserDto> Get(string id)
        {
            // 參數檢查
            var validationResults = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(id))
            {
                validationResults.Add(new ValidationResult($"參數不合法", new[] { nameof(id) }));
            }
            if (validationResults.Any())
            {
                return BadRequest(validationResults);
            }

            var result = _userBl.GetUser(id);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
