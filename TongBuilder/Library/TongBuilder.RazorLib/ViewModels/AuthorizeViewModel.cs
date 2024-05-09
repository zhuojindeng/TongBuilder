using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TongBuilder.RazorLib.ViewModels
{
    public class AuthorizeViewModel
    {
        [AllowNull]
        [Display(Name = "Application")]
        public string ApplicationName { get; set; }

        [AllowNull]

        [Display(Name = "Scope")]
        public string Scope { get; set; }
    }
}
