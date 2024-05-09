using System.Globalization;
using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Core;
using TongBuilder.Contract.Models;

namespace TongBuilder.Contract.Extensions
{
    public static class CurrentUserExtensions
    {
        public static string? FindClaimValue(this ICurrentUser currentUser, string claimType)
        {
            return currentUser.FindClaim(claimType)?.Value;
        }

        public static T FindClaimValue<T>(this ICurrentUser currentUser, string claimType)
            where T : struct
        {
            var value = currentUser.FindClaimValue(claimType);
            if (value == null)
            {
                return default;
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }

        public static bool IsAdministrator(this ICurrentUser currentUser)
        {
            return Builtin.IsAdministrator(currentUser);
        }

        public static bool IsSystem(this ICurrentUser currentUser)
        {
            return Builtin.IsSystem(currentUser);
        }

        public static bool IsPublic(this ICurrentUser currentUser)
        {
            return Builtin.IsPublic(currentUser);
        }

        public static bool IsLoginUser(ICurrentUser currentUser)
        {
            return Builtin.IsLoginUser(currentUser);
        }

        public static IDisposable AsAdministrator(this ICurrentUser currentUser)
        {
            if (Builtin.Administrator != null)
            {
                return currentUser.Change(Builtin.Administrator);
            }
            return new DisposableAction(() =>
            {
            });
        }
    }
}
