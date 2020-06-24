using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sino.CommonService;
using Sino.Extensions.AreaService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SinoAreaService.UnitTest
{
    public class AreaServiceTest
    {
        private IAreaService _area;

        public AreaServiceTest()
        {
            _area = new AreaService(null);
        }

        [Fact]
        public void TestGetProvinceCodeByName()
        {
            var code = _area.GetCode("北京");
            Assert.Equal(code, "01");

            code = _area.GetCode("山西");
            Assert.Equal(code, "04");

            code = _area.GetCode("吉林");
            Assert.Equal(code, "07");

            code = _area.GetCode("上海");
            Assert.Equal(code, "09");

            code = _area.GetCode("湖南");
            Assert.Equal(code, "18");

            code = _area.GetCode("海南");
            Assert.Equal(code, "21");

            code = _area.GetCode("陕西");
            Assert.Equal(code, "27");

            code = _area.GetCode("新疆");
            Assert.Equal(code, "31");

            code = _area.GetCode("香港");
            Assert.Equal(code, "33");

            code = _area.GetCode("澳门");
            Assert.Equal(code, "34");
        }

        [Fact]
        public void TestGetCityCodeByName()
        {
            var code = _area.GetCode("澳门", "离岛");
            Assert.Equal(code, "3402");

            code = _area.GetCode("香港", "新界");
            Assert.Equal(code, "3303");

            code = _area.GetCode("宁夏", "固原");
            Assert.Equal(code, "3004");

            code = _area.GetCode("甘肃", "天水");
            Assert.Equal(code, "2805");

            code = _area.GetCode("甘肃", "嘉峪关");
            Assert.Equal(code, "2802");

            code = _area.GetCode("云南", "曲靖");
            Assert.Equal(code, "2502");

            code = _area.GetCode("云南", "保山");
            Assert.Equal(code, "2504");

            code = _area.GetCode("四川", "自贡");
            Assert.Equal(code, "2302");

            code = _area.GetCode("四川", "泸州");
            Assert.Equal(code, "2304");

            code = _area.GetCode("广西", "桂林");
            Assert.Equal(code, "2003");

            code = _area.GetCode("广东", "韶关");
            Assert.Equal(code, "1902");

            code = _area.GetCode("广东", "汕头");
            Assert.Equal(code, "1905");

            code = _area.GetCode("河南", "郑州");
            Assert.Equal(code, "1601");

            code = _area.GetCode("河南", "安阳");
            Assert.Equal(code, "1605");

            code = _area.GetCode("福建", "厦门");
            Assert.Equal(code, "1302");

            code = _area.GetCode("福建", "泉州");
            Assert.Equal(code, "1305");

            code = _area.GetCode("浙江", "杭州");
            Assert.Equal(code, "1101");

            code = _area.GetCode("浙江", "温州");
            Assert.Equal(code, "1103");

            code = _area.GetCode("江苏", "南京");
            Assert.Equal(code, "1001");

            code = _area.GetCode("江苏", "镇江");
            Assert.Equal(code, "1011");

            code = _area.GetCode("黑龙江", "哈尔滨");
            Assert.Equal(code, "0801");

            code = _area.GetCode("吉林", "吉林");
            Assert.Equal(code, "0702");

            code = _area.GetCode("辽宁", "沈阳");
            Assert.Equal(code, "0601");

            code = _area.GetCode("内蒙古", "包头");
            Assert.Equal(code, "0502");

            code = _area.GetCode("河北", "石家庄");
            Assert.Equal(code, "0301");

            code = _area.GetCode("河北", "唐山");
            Assert.Equal(code, "0302");

            code = _area.GetCode("河北", "邯郸");
            Assert.Equal(code, "0304");

            code = _area.GetCode("北京", "北京");
            Assert.Equal(code, "0101");

            code = _area.GetCode("安徽省");
            Assert.Equal(code, "12");

            code = _area.GetCode("安徽");
            Assert.Equal(code, "12");

            code = _area.GetCode("安徽省", "芜湖市");
            Assert.Equal(code, "1202");

            code = _area.GetCode("安徽", "芜湖市");
            Assert.Equal(code, "1202");

            code = _area.GetCode("安徽省", "芜湖");
            Assert.Equal(code, "1202");

            code = _area.GetCode("安徽省", "芜湖市", "镜湖区");
            Assert.Equal(code, "120202");

            code = _area.GetCode("安徽", "芜湖市", "镜湖区");
            Assert.Equal(code, "120202");

            code = _area.GetCode("安徽", "芜湖", "镜湖区");
            Assert.Equal(code, "120202");

            code = _area.GetCode("安徽省", "芜湖", "镜湖区");
            Assert.Equal(code, "120202");
        }

        [Fact]
        public void TestGetCountyCodeByCode()
        {
            var code = _area.GetCode("北京", "北京", "丰台区");
            Assert.Equal(code, "010106");

            code = _area.GetCode("北京", "北京辖县", "密云县");
            Assert.Equal(code, "010201");

            code = _area.GetCode("河北", "石家庄", "市区");
            Assert.Equal(code, "030101");

            code = _area.GetCode("河北", "唐山", "滦县");
            Assert.Equal(code, "030208");

            code = _area.GetCode("山西", "太原", "市区");
            Assert.Equal(code, "040101");

            code = _area.GetCode("山西", "太原", "晋源区");
            Assert.Equal(code, "040107");

            code = _area.GetCode("山西", "太原", "古交市");
            Assert.Equal(code, "040111");
        }

        [Fact]
        public void TestGetAreaNameByCode()
        {
            var name = _area.GetAreaName("040111");
            Assert.Equal(name, "山西太原古交市");

            name = _area.GetAreaName("040107");
            Assert.Equal(name, "山西太原晋源区");

            name = _area.GetAreaName("040101");
            Assert.Equal(name, "山西太原市区");

            name = _area.GetAreaName("030208");
            Assert.Equal(name, "河北唐山滦县");

            name = _area.GetAreaName("030101");
            Assert.Equal(name, "河北石家庄市区");

            name = _area.GetAreaName("0101");
            Assert.Equal(name, "北京北京");

            name = _area.GetAreaName("1011");
            Assert.Equal(name, "江苏镇江");

            name = _area.GetAreaName("10");
            Assert.Equal(name, "江苏");

            name = _area.GetAreaName("030101", false);
            Assert.Equal(name, "市区");

            name = _area.GetAreaName("1011", false);
            Assert.Equal(name, "镇江");
        }

        [Fact]
        public void TestGetCityCodeByProvinceCode()
        {

        }

        [Fact]
        public void TestProvinceEqualCode()
        {
            for (int i = 0; i < _area.ProvinceCodes.Count; i++)
            {
                var code = _area.ProvinceCodes[i];
                var name = _area.ProvinceNames[i];

                var destCode = _area.GetCode(name);

                Assert.Equal(code, destCode);
            }
        }
    }
}
