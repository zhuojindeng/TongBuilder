using System.Diagnostics.CodeAnalysis;

namespace TongBuilder.Contract.Models
{
    public class ErrorModel
    {
        [AllowNull]
        public string Code { get; set; }

        public string? Description { get; set; }
    }
}
