## Sino.Extensions.AreaService

[![Build status](https://ci.appveyor.com/api/projects/status/k45f1cgyuo6cmshi?svg=true)](https://ci.appveyor.com/project/vip56/sino-extensions-areaservice)
[![NuGet](https://img.shields.io/nuget/v/Nuget.Core.svg?style=plastic)](https://www.nuget.org/packages/Sino.Extensions.AreaService)   

## 使用方法
```
Install-Package Sino.Extensions.AreaService
```

在`Startup`中进行配置，然后可直接通过IOC使用`IAreaService`接口。
```
services.AddArea();
```   

## 版本更新记录
* 2018.3.7 支持asp.net core 2.0 by y-z-f