using Sino.CommonService;

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
}
