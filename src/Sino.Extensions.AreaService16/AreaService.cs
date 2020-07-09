using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Sino.CommonService;
using Sino.CommonService.HttpClientLibrary;
using Sino.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

namespace Sino.Extensions.AreaService
{
    public class AreaService : IAreaService
    {
        private List<RegionList> areas { get; set; }
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
            string path = PlatformServices.Default.Application.ApplicationBasePath + "\\Regions";
            string json = null;
            Stream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);
                using (var rr = new ResourceReader(fs))
                {
                    var iter = rr.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        json = iter.Value.ToString();
                        break;
                    }
                }
            }
            catch (Exception) { }
            if (fs != null)
                fs.Dispose();
            if (json == null || json == "[]")
            {
                json = Regions.Areas;
            }

            var regionData = new GetRegionDataResponse();
            if (json != null)
            {
                regionData = JsonConvert.DeserializeObject<GetRegionDataResponse>(json);
            }
            areas = regionData.Areas ?? new List<RegionList>();
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
                            if (areas?.Count > 0)
                            {
                                fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                                using (var rw = new ResourceWriter(fs))
                                {
                                    rw.AddResource("Areas", data.ToJsonString());
                                }
                                if (fs != null)
                                    fs.Dispose();
                            }
                        }
                    }
                }
            }

            _areas = areas.OrderBy(x => x.Version).LastOrDefault();
            _regions = areas.SelectMany(x => x.Regions).Distinct().ToList();
        }

        public AnalyzeCode AnalyzeCode(string code)
        {
            var separator = "_SN";
            var codes = code.Split(new[] { separator }, StringSplitOptions.None);
            var highVersion = codes.Length > 1;
            string version = null;
            if (highVersion)
            {
                version = codes[0] + separator;
                code = codes[1];
            }

            var region = new AnalyzeCode
            {
                NumberCode = code,
                Version = version
            };
            return region;
        }

        public List<Region> Provinces(string version = null)
        {
            var regions = _areas.Regions;
            if (version != _areas.Version)
            {
                regions = areas.Where(x => x.Version == version)?.FirstOrDefault()?.Regions;
            }
            var provinces = regions?.Where(x => x.Level == Level.Province)?.Distinct()?.ToList();
            if (provinces?.Count > 0)
            {
                provinces.ForEach((r) =>
                {
                    r.Code = r.Version + r.Code;
                });
            }
            return provinces;
        }

        public List<Region> Citys(string provinceCode)
        {
            if (string.IsNullOrEmpty(provinceCode))
                throw new SinoException("省编号不可为空");

            var ac = AnalyzeCode(provinceCode);
            provinceCode = ac.NumberCode;
            if (provinceCode.Length < 2)
                throw new SinoException("省编号有误");

            provinceCode = provinceCode.Substring(0, 2);
            var citys = areas.Where(x => x.Version == ac.Version)?.FirstOrDefault()?.Regions?
                .Where(x => x.Level == Level.City && x.Code.StartsWith(provinceCode))?.Distinct()?.ToList();
            if (citys?.Count > 0)
            {
                citys.ForEach((r) =>
                {
                    r.Code = r.Version + r.Code;
                });
            }
            return citys;
        }

        public List<Region> Countys(string cityCode)
        {
            if (string.IsNullOrEmpty(cityCode))
                throw new SinoException("市编号不可为空");

            var ac = AnalyzeCode(cityCode);
            cityCode = ac.NumberCode;

            if (cityCode.Length < 4)
                throw new SinoException("市编号有误");

            cityCode = cityCode.Substring(0, 4);
            Func<Region, bool> predicate = x => x.Level == Level.County && x.Code.StartsWith(cityCode);
            if (cityCode == AltayCode)
            {
                predicate = x => x.Level == Level.County && (x.Code.StartsWith(cityCode) || x.Code.StartsWith(SpecialCode));
            }

            var countys = areas.Where(x => x.Version == ac.Version)?.FirstOrDefault()?.Regions?
                .Where(predicate)?.Distinct()?.ToList();
            if (countys?.Count > 0)
            {
                countys.ForEach((r) =>
                {
                    r.Code = r.Version + r.Code;
                });
            }
            return countys;
        }

        public Area GetArea(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new SinoException("编号不可为空");

            var region = _regions.FirstOrDefault(x => x.Version + x.Code == code);
            if (region == null)
                return null;

            var r = AnalyzeCode(code);
            var highVersion = !string.IsNullOrEmpty(r.Version);
            var area = new Area
            {
                Level = region.Level
            };
            if (region.Level == Level.County)
            {
                var ciryCode = code.Substring(0, 4);
                if (ciryCode == SpecialCode)
                {
                    ciryCode = AltayCode;
                }
                if (highVersion)
                {
                    ciryCode += "00";
                    area.CountyCode = r.Version;
                }
                area.CountyCode += region.Code;
                area.County = region.Name;
                region = _regions.FirstOrDefault(x => x.Version + x.Code == r.Version + ciryCode);
            }
            if (region?.Level == Level.City)
            {
                var provinceCode = r.Version + code.Substring(0, 2);
                if (highVersion)
                {
                    provinceCode += "0000";
                    area.CityCode = r.Version;
                }
                area.CityCode += region.Code;
                area.City = region.Name;
                region = _regions.FirstOrDefault(x => x.Version + x.Code == provinceCode);
            }
            if (region?.Level == Level.Province)
            {
                if (highVersion)
                {
                    area.ProvinceCode = r.Version;
                }
                area.ProvinceCode += region.Code;
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
            var level = Level.Province;
            if (!string.IsNullOrEmpty(county))
            {
                name = county;
                level = Level.County;
            }
            else if (!string.IsNullOrEmpty(city))
            {
                name = city;
                level = Level.City;
            }

            var regions = _regions.Where(x => x.Name == name && x.Level == level && x.Version == version);
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
                            code = item.Version + item.Code;
                            break;
                        }
                    }
                }
            }
            else
            {
                code = regions.FirstOrDefault()?.Version + regions.FirstOrDefault()?.Code;
            }

            return code;
        }
    }
}
