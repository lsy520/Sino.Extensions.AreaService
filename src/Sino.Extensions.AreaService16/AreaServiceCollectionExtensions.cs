using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Sino.CommonService;
using Sino.CommonService.HttpClientLibrary;
using Sino.Extensions.AreaService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AreaServiceCollectionExtensions
    {
        /// <summary>
        /// 注册省市区服务（如需实时更新则先要注册公共服务）
        /// </summary>
        public static IServiceCollection AddArea(this IServiceCollection services)
        {
            var service = services.BuildServiceProvider();
            var clientProvider = service.GetService<IClientProvider>();
            return services.AddSingleton<IAreaService>(new AreaService(clientProvider));
        }
    }
}
