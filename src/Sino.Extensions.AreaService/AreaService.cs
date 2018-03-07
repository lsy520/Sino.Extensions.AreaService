using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sino.Extensions.AreaService
{
    public class AreaService : IAreaService
    {
        private Dictionary<string, Area> _areas;

        public AreaService()
        {
            _areas = JsonConvert.DeserializeObject<Dictionary<string, Area>>(Resource.Res);

            var provinces = _areas.Where(x => x.Key.Length == 2).ToList();
            ProvinceCodes = provinces.Select(x => x.Key).ToList();
            ProvinceNames = provinces.Select(x => x.Value.Province).ToList();
        }

        public List<string> ProvinceCodes { get; private set; }

        public List<string> ProvinceNames { get; private set; }

        public List<string> CityCodeInProvince(string provinceCode)
        {
            if (string.IsNullOrEmpty(provinceCode))
                throw new ArgumentNullException(nameof(provinceCode));
            return _areas.Where(x => x.Value.ProvinceCode == provinceCode).Select(x => x.Value.CityCode).Distinct().ToList();
        }

        public List<string> CityNameInProvince(string provinceName)
        {
            if (string.IsNullOrEmpty(provinceName))
                throw new ArgumentNullException(nameof(provinceName));
            return _areas.Where(x => x.Value.Province == provinceName).Select(x => x.Value.City).Distinct().ToList();
        }

        public List<string> CountyCodeInCity(string cityCode)
        {
            if (string.IsNullOrEmpty(cityCode))
                throw new ArgumentNullException(nameof(cityCode));
            return _areas.Where(x => x.Value.CityCode == cityCode).Select(x => x.Value.CountyCode).Distinct().ToList();
        }

        public List<string> CountyNameInCity(string cityName)
        {
            if (string.IsNullOrEmpty(cityName))
                throw new ArgumentNullException(nameof(cityName));
            return _areas.Where(x => x.Value.City == cityName).Select(x => x.Value.County).Distinct().ToList();
        }

        public Area GetArea(string code)
        {
            if (string.IsNullOrEmpty(code) || (code.Length != 2 && code.Length != 4 && code.Length != 6))
                throw new ArgumentException("编号格式错误");

            if (!_areas.ContainsKey(code))
                return null;

            var area = _areas[code];

            return area;
        }

        public string GetAreaName(string code, bool isFullName = true)
        {
            var area = GetArea(code);

            if (area == null)
                return null;

            if (isFullName)
                return $"{area.Province}{area.City}{area.County}";

            switch(code.Length)
            {
                case 2:
                    return area.Province;
                case 4:
                    return area.City;
                case 6:
                    return area.County;
            }

            return null;
        }

        public string GetCode(string province, string city = null, string county = null)
        {
            int count = 2;

            if (string.IsNullOrEmpty(province))
                throw new ArgumentNullException("省必须设置");
            if (string.IsNullOrEmpty(city))
                count-=2;
            if (string.IsNullOrEmpty(county))
                count--;

            string code = null;
            switch(count)
            {
                case 1:
                    {
                        code = _areas.Where(x => x.Value.Province == province && x.Value.City == city).Select(x => x.Key).FirstOrDefault();
                    }
                    break;
                case 2:
                    {
                        if (city.EndsWith("市"))
                        {
                            city = city.Substring(0, city.Length - 1);
                        }
                        code = _areas.Where(x => x.Value.Province == province && x.Value.City == city && x.Value.County == county).Select(x => x.Key).FirstOrDefault();
                    }
                    break;
                default:
                    {
                        province = province.Replace("省", "").Replace("市", "").Replace("特区", "").Replace("自治区", "");
                        code = _areas.Where(x => x.Value.Province == province).Select(x => x.Key).FirstOrDefault();
                    }
                    break;
            }

            return code;
        }
    }
}
