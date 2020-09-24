using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DtoModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace middleware.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="body"></param>
        /// <response code="200">登入成功</response>
        /// <response code="401">登入失敗</response>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<Authentication>> Login(LoginPostBody body)
        {
            await Task.Delay(1);
            if (body.Password.Length > 8)
                return Unauthorized();

            var result = new Authentication()
            {
                Token = GenerateToken(body.UserName)
            };

            return Ok(result);
        }

        private string GenerateToken(string id)
        {
            const string TEMP_ISSUER_SIGNING_KEY = "TEMP_ISSUER_SIGNING_KEY";

            var claims = new Claim[] { new Claim("NameIdentifier", id) };
            var jwtSecurityToken = new JwtSecurityToken
            (
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(TEMP_ISSUER_SIGNING_KEY)),
                SecurityAlgorithms.HmacSha256)
            );
            var result = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return result;
        }
    }
}
