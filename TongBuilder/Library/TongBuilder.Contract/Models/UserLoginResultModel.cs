using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace TongBuilder.Contract.Models
{
    public class UserLoginResultModel
    {
        public bool Succeeded { get; set; }

        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

        public string? OpenId { get; set; }

        /// <summary>
        /// 用户所属租户列表
        /// </summary>
        public List<TenantModel> Tenants { get; set; } = new List<TenantModel>();

        public string ErrorMsg
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach(var error in Errors)
                {
                    stringBuilder.Append(error.Description);
                    stringBuilder.Append(" ");
                }
                return stringBuilder.ToString();
            }
        }
    }
}
