using System.Collections.Generic;

namespace Sino.Extensions.AreaService
{
    public interface IAreaService
    {
        /// <summary>
        /// 省编号
        /// </summary>
        List<string> ProvinceCodes { get; }

        /// <summary>
        /// 省名称
        /// </summary>
        List<string> ProvinceNames { get; }

        /// <summary>
        /// 根据名称获取编号
        /// </summary>
        /// <param name="province">省名称</param>
        /// <param name="city">市名称</param>
        /// <param name="county">区名称</param>
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

        /// <summary>
        /// 获取省编号下市编号列表
        /// </summary>
        /// <param name="provinceCode">省编号</param>
        List<string> CityCodeInProvince(string provinceCode);

        /// <summary>
        /// 获取市编号下区编号列表
        /// </summary>
        /// <param name="cityCode">市编号</param>
        List<string> CountyCodeInCity(string cityCode);

        /// <summary>
        /// 获取省名称下市名称列表
        /// </summary>
        /// <param name="provinceName">省名称</param>
        List<string> CityNameInProvince(string provinceName);

        /// <summary>
        /// 获取市名称下区名称列表
        /// </summary>
        /// <param name="cityName">市名称</param>
        List<string> CountyNameInCity(string cityName);
    }
}
