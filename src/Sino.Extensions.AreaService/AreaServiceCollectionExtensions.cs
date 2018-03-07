using Sino.Extensions.AreaService;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AreaServiceCollectionExtensions
    {
        /// <summary>
        /// 注册省市区服务
        /// </summary>
        public static IServiceCollection AddArea(this IServiceCollection services)
        {
            return services.AddSingleton<IAreaService>(new AreaService());
        }
    }
}
