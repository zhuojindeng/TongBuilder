using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Models;

namespace TongBuilder.Application.Services
{
    public class SystemConfigService : ISystemConfigService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCopyright()
        {
            return GetSystemConfig().Copyright;
        }
        /// <summary>
        /// 获取系统配置      
        /// </summary>
        /// <returns></returns>
        public SystemConfig GetSystemConfig()
        {

            return new SystemConfig
            {

                Copyright = DateTime.Now.Year + " TongBuilder",
                SystemName = "TongBuilder",
                SystemDescription = "TongBuilder,基于Blazor的低代码，跨平台，开箱即用的快速开发框架。"

            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSystemName()
        {
            return GetSystemConfig().SystemName;
        }
    }
}
