using Sino.CommonService;
using System.Collections.Generic;

namespace Sino.Extensions.AreaService
{
    public interface IAreaService
    {
        /// <summary>
        /// 省集合
        /// </summary>
        /// <param name="version">版本号</param>
        List<Region> Provinces(string version = null);

        /// <summary>
        /// 市集合
        /// </summary>
        /// <param name="provinceCode">省编号</param>
        List<Region> Citys(string provinceCode);

        /// <summary>
        /// 省集合
        /// </summary>
        /// <param name="cityCode">市编号</param>
        List<Region> Countys(string cityCode);

        /// <summary>
        /// 解析编号
        /// </summary>
        /// <param name="code">编号</param>
        AnalyzeCode AnalyzeCode(string code);

        /// <summary>
        /// 根据名称获取编号
        /// </summary>
        /// <param name="province">省名称</param>
        /// <param name="city">市名称</param>
        /// <param name="county">区名称</param>
        /// <param name="version">版本号</param>
        string GetCode(string province, string city = null, string county = null, string version = null);

        /// <summary>
        /// 根据编号获取名称
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="isFullName">是否显示全名，默认显示</param>
        string GetAreaName(string code, bool isFullName = true);

        /// <summary>
        /// 根据编号获取区域信息
        /// </summary>
        /// <param name="code">编号</param>
        Area GetArea(string code);
    }
}
