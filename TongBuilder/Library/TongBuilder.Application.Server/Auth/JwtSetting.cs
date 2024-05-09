using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TongBuilder.Application.Server.Auth
{
    public class JwtSetting
    {
        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 令牌密码
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        ///  过期时间
        /// </summary>
        public long ExpireSeconds { get; set; }

        /// <summary>
        /// 访问令牌过期时间
        /// </summary>
        public int AccessTokenExpirationMinutes { get; set; }

        /// <summary>
        /// cookie过期时间
        /// </summary>
        public int CokkieExpirationMinutes { get; set; }

        /// <summary>
        /// 刷新令牌过期时间
        /// </summary>
        public int RefreshTokenExpirationMinutes { get; set; }


        /// <summary>
        /// 签名
        /// </summary>
        public SigningCredentials Credentials
        {
            get
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
                return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            }
        }
    }
}
