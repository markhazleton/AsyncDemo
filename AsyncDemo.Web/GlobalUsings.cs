﻿global using AsyncDemo.Models;
global using AsyncDemo.Services;
global using AsyncDemo.Web.Extensions;
global using AsyncDemo.Web.Models;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using OpenWeatherMapClient.Interfaces;
global using OpenWeatherMapClient.Models;
global using OpenWeatherMapClient.WeatherService;
global using Polly;
global using Polly.Retry;
global using System.Diagnostics;
global using System.Globalization;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Reflection;
