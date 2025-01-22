using TongBuilder.BlazorMaui.Client.Shared.Services;

namespace TongBuilder.BlazorMaui.Client.Web.Services
{
    public class FormFactor : IFormFactor
    {
        public string GetFormFactor()
        {
            return "Web";
        }

        public string GetPlatform()
        {
            return Environment.OSVersion.ToString();
        }
    }
}
