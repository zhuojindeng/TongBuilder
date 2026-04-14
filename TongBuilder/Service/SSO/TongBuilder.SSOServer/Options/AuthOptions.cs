using System.Diagnostics.CodeAnalysis;

namespace TongBuilder.SSOServer.Options
{
    public class AuthOptions
    {
        /// <summary>
        /// accesstoken失效时间(分钟）
        /// </summary>
        public int AccessTokenLifetime { get; set; } = 60 * 24 * 7;

        /// <summary>
        /// Token 签发者
        /// </summary>
        [AllowNull]
        public string Issuer { get; set; }
    }
}
