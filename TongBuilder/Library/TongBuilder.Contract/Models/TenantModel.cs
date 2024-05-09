using System.Diagnostics.CodeAnalysis;

namespace TongBuilder.Contract.Models
{
    public class TenantModel
    {
        [AllowNull]
        public string Code { get; set; }

        [AllowNull]
        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}
