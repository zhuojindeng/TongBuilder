using TongBuilder.Contract.Models;

namespace TongBuilder.Contract.Contracts
{
    public interface ISystemConfigService
    {
        string GetCopyright();

        string GetSystemName();

        SystemConfig GetSystemConfig();
    }
}
