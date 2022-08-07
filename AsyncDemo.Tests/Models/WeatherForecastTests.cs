using AsyncDemo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AsyncDemo.Tests.Models
{
    [TestClass]
    public class WeatherForecastTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            var weatherForecast = new WeatherForecast
            {
                Date = System.DateTime.Today
            };

            var tempF = weatherForecast.TemperatureF;
            var tempC = weatherForecast.TemperatureC;
            var sum = weatherForecast.Summary;
            // Act


            // Assert
            Assert.AreEqual(weatherForecast.Date.Date, System.DateTime.Today.Date);
            Assert.IsNotNull(tempF);
            Assert.IsNotNull(tempC);
            Assert.IsNotNull(sum);
            Assert.IsNotNull(weatherForecast);
        }
    }
}
