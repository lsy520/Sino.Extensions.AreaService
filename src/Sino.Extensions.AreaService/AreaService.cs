using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sino.CommonService;
using Sino.CommonService.HttpClientLibrary;
using Sino.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sino.Extensions.AreaService
{
    public class AreaService : IAreaService
    {
        private RegionList _areas { get; set; }
        private List<Region> _regions { get; set; }
        /// <summary>
        /// 特殊code
        /// </summary>
        private string SpecialCode { get; } = "6590";
        /// <summary>
        /// 阿勒泰
        /// </summary>
        private string AltayCode { get; } = "6543";
        public AreaService(IClientProvider iClientProvider)
        {
            var json = JsonFile();
            var regionData = JsonConvert.DeserializeObject<GetRegionDataResponse>(json);
            var areas = regionData?.Areas ?? new List<RegionList>();
            areas = areas.OrderBy(x => x.Version).ToList();
            var currentVersion = areas.LastOrDefault()?.Version ?? "V0_SN";
            if (iClientProvider != null)
            {
                //读取公共服务最新地址库
                var result = iClientProvider.GetRegionData(new GetRegionDataRequest
                {
                    Version = currentVersion
                }).Result;
                if (result.success && result.data != null)
                {
                    regionData = JsonConvert.DeserializeObject<GetRegionDataResponse>(result.data.ToString());
                    if (regionData?.Areas?.Count > 0)
                    {
                        var areasList = regionData.Areas.OrderBy(x => x.Version).ToList();
                        if (currentVersion != areasList.LastOrDefault()?.Version)
                        {
                            areas.AddRange(regionData.Areas);
                            var data = new GetRegionDataResponse
                            {
                                Areas = areas
                            };
                            JsonFile(false, data.ToJsonString());
                        }
                    }
                }
            }
            
            _areas = areas.OrderBy(x => x.Version).LastOrDefault();
            _regions = areas.SelectMany(x => x.Regions).Distinct().ToList();
            var provinces = _areas.Regions.Where(x => x.Level == Level.Province).Distinct().ToList();
            ProvinceCodes = provinces.Select(x => x.Version + x.Code).Distinct().ToList();
            ProvinceNames = provinces.Select(x => x.Name).Distinct().ToList();
        }

        private string JsonFile(bool isGet = true, string json = null)
        {
            string result = null;
            var jsonfile = AppDomain.CurrentDomain.BaseDirectory + "\\regions.json";
            if (File.Exists(jsonfile))
            {
                if (isGet)
                {
                    result = File.ReadAllText(jsonfile, Encoding.UTF8);//读取文件
                }
                else
                {
                    File.WriteAllText(jsonfile, json);//将内容写进jon文件中
                }
            }
            return result;
        }

        private string TakeCode(string code)
        {
            var codes = code.Split(new[] { "_SN" }, StringSplitOptions.None);
            var highVersion = codes.Length > 1;
            if (highVersion)
            {
                code = codes[1];
            }
            return code;
        }

        public List<string> ProvinceCodes { get; private set; }

        public List<string> ProvinceNames { get; private set; }

        public List<string> CityCodeInProvince(string provinceCode)
        {
            if (string.IsNullOrEmpty(provinceCode))
                throw new ArgumentNullException(nameof(provinceCode));
            provinceCode = TakeCode(provinceCode).Substring(0, 2);
            return _areas.Regions.Where(x => x.Level == Level.City && x.Code.StartsWith(provinceCode)).Select(x => x.Version + x.Code).Distinct().ToList();
        }

        public List<string> CityNameInProvince(string provinceName)
        {
            if (string.IsNullOrEmpty(provinceName))
                throw new ArgumentNullException(nameof(provinceName));

            var code = GetCode(provinceName, null, null, _areas.Version);
            code = TakeCode(code).Substring(0, 2);
            return _areas.Regions.Where(x => x.Level == Level.City && x.Code.StartsWith(code)).Select(x => x.Name).Distinct().ToList();
        }

        public List<string> CountyCodeInCity(string cityCode)
        {
            if (string.IsNullOrEmpty(cityCode))
                throw new ArgumentNullException(nameof(cityCode));
            cityCode = TakeCode(cityCode).Substring(0, 4);
            Func<Region, bool> predicate = x => x.Level == Level.County && x.Code.StartsWith(cityCode);
            if (cityCode == AltayCode)
            {
                predicate = x => x.Level == Level.County && (x.Code.StartsWith(cityCode) || x.Code.StartsWith(SpecialCode));
            }
            return _areas.Regions.Where(predicate).Select(x => x.Version + x.Code).Distinct().ToList();
        }

        public List<string> CountyNameInCity(string cityName)
        {
            if (string.IsNullOrEmpty(cityName))
                throw new ArgumentNullException(nameof(cityName));
            var code = GetCode(null, cityName, null, _areas.Version);
            code = TakeCode(code).Substring(0, 4);
            Func<Region, bool> predicate = x => x.Level == Level.County && x.Code.StartsWith(code);
            if (code == AltayCode)
            {
                predicate = x => x.Level == Level.County && (x.Code.StartsWith(code) || x.Code.StartsWith(SpecialCode));
            }
            return _areas.Regions.Where(predicate).Select(x => x.Name).Distinct().ToList();
        }

        public Area GetArea(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("编号格式错误");

            var region = _regions.FirstOrDefault(x => x.Version + x.Code == code);
            if (region == null)
                return null;

            var separator = "_SN";
            var codes = code.Split(new[] { separator }, StringSplitOptions.None);
            var highVersion = codes.Length > 1;
            var vesion = "";
            if (highVersion)
            {
                vesion = codes[0] + separator;
                code = codes[1];
            }
            var area = new Area
            {
                Level = region.Level
            };
            if (region.Level == Level.County)
            {
                area.CountyCode = region.Code;
                area.County = region.Name;
                var ciryCode = code.Substring(0, 4);
                if (ciryCode == SpecialCode)
                {
                    ciryCode = AltayCode;
                }
                if (highVersion)
                {
                    ciryCode += "00";
                }
                region = _regions.FirstOrDefault(x => x.Version + x.Code == vesion + ciryCode);
            }
            if (region?.Level == Level.City)
            {
                area.CityCode = region.Code;
                area.City = region.Name;
                var provinceCode = vesion + code.Substring(0, 2);
                if (highVersion)
                {
                    provinceCode += "0000";
                }
                region = _regions.FirstOrDefault(x => x.Version + x.Code == provinceCode);
            }
            if (region?.Level == Level.Province)
            {
                area.ProvinceCode = region.Code;
                area.Province = region.Name;
            }
            return area;
        }

        public string GetAreaName(string code, bool isFullName = true)
        {
            var area = GetArea(code);
            if (area == null)
                return null;

            if (isFullName)
                return $"{area.Province}{area.City}{area.County}";

            switch (area.Level)
            {
                case Level.Province:
                    return area.Province;
                case Level.City:
                    return area.City;
                case Level.County:
                    return area.County;
            }

            return null;
        }

        public string GetCode(string province, string city = null, string county = null, string version = null)
        {
            string code = null;

            var name = province;
            if (!string.IsNullOrEmpty(county))
            {
                name = county;
            }
            else if (!string.IsNullOrEmpty(city))
            {
                name = city;
            }

            Func<Region, bool> predicate = x => x.Name == name;
            if (version != null)
            {
                predicate = x => x.Name == name && x.Version == version;
            }
            var regions = _regions.Where(predicate);
            var hasCounty = regions.Any(x => x.Level == Level.County);
            if (hasCounty)
            {
                foreach (var item in regions)
                {
                    var area = GetArea(item.Version + item.Code);
                    if (item.Level == Level.County)
                    {
                        if (area?.Province == province && area?.City == city && area?.County == county)
                        {
                            code = item.Code;
                            break;
                        }
                    }
                }
            }
            else
            {
                code = regions.FirstOrDefault()?.Code;
            }

            return code;
        }
    }
}
