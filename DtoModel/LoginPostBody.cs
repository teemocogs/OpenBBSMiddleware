using System.ComponentModel.DataAnnotations;

namespace DtoModel
{
    public class LoginPostBody
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
