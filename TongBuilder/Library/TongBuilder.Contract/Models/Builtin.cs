using System.Security.Claims;
using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Enums;

namespace TongBuilder.Contract.Models
{
    /// <summary>
    /// 系统内置用户
    /// </summary>
    public sealed class Builtin
    {
        public const string SystemCode = "System";
        public const string PublicCode = "Public";
        public const string AdministratorCode = "Administrator";
        public const string LoginUserCode = "LoginUser";

        /// <summary>
        /// 获取系统用户信息。系统用户能够执行系统内的任何操作
        /// </summary>
        public static ClaimsPrincipal? System { get; internal set; }

        /// <summary>
        /// 获取公共用户信息。公共用户不能执行任何实质性的操作
        /// </summary>
        public static ClaimsPrincipal? Public { get; internal set; }

        /// <summary>
        /// 获取管理员用户信息。管理员用户能够执行系统内所有操作
        /// </summary>
        public static ClaimsPrincipal? Administrator { get; internal set; }

        /// <summary>
        /// 获取用户登陆时的环境用户。登陆环境用户没有任何权限。
        /// </summary>
        public static ClaimsPrincipal? LoginUser { get; internal set; }

        /// <summary>
        /// 确定指定的用户是否是管理员
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsAdministrator(ICurrentUser user)
        {
            if (null != user &&
                (user.UserType == IdentityUserType.SystemAdmin ||
                user.AccountType == AccountType.SystemAdmin))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 确定指定的用户是否是系统用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsSystem(ICurrentUser user)
        {
            if (null != user && user.AccountCode?.ToUpper() == SystemCode.ToUpper() && user.AccountType == AccountType.SystemAdmin)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 确定指定的用户是否是公共用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsPublic(ICurrentUser user)
        {
            if (null != user && user.AccountCode?.ToUpper() == PublicCode.ToUpper())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 确定指定的用户是否是登录环境用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsLoginUser(ICurrentUser user)
        {
            if (null != user && user.AccountCode?.ToUpper() == LoginUserCode.ToUpper() && user.AccountType == AccountType.SystemAdmin)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 确认指定的用户是否是不需认证用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsNoNeedAuthenticate(ICurrentUser user)
        {
            return IsLoginUser(user) || IsSystem(user) || IsPublic(user);
        }
    }
}
