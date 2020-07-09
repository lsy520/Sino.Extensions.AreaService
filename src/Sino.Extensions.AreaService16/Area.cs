using Sino.CommonService;
using System.Collections.Generic;

namespace Sino.Extensions.AreaService
{
    /// <summary>
    /// 区域信息
    /// </summary>
    public class Area
    {
        /// <summary>
        /// 省名称
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市名称
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 区名称
        /// </summary>
        public string County { get; set; }
        /// <summary>
        /// 省编号
        /// </summary>
        public string ProvinceCode { get; set; }
        /// <summary>
        /// 市编号
        /// </summary>
        public string CityCode { get; set; }
        /// <summary>
        /// 区编号
        /// </summary>
        public string CountyCode { get; set; }
        /// <summary>
        /// 区域级别
        /// </summary>
        public Level Level { get; set; }
    }

    /// <summary>
    /// 解析编号返回值
    /// </summary>
    public class AnalyzeCode
    {
        /// <summary>
        /// 数字编号
        /// </summary>
        public string NumberCode { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
    }

    /// <summary>
    /// 区域集合
    /// </summary>
    public class AreaItem
    {
        /// <summary>
        /// 编号集合
        /// </summary>
        public List<string> Codes { get; set; }
        /// <summary>
        /// 名称集合
        /// </summary>
        public List<string> Names { get; set; }
    }
}
