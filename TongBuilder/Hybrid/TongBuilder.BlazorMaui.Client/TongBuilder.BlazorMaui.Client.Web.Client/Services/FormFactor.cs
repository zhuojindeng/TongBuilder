using TongBuilder.BlazorMaui.Client.Shared.Services;

namespace TongBuilder.BlazorMaui.Client.Web.Client.Services
{
    public class FormFactor : IFormFactor
    {
        public string GetFormFactor()
        {
            return "WebAssembly";
        }

        public string GetPlatform()
        {
            return Environment.OSVersion.ToString();
        }
    }
}
