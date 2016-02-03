using MarkNotGeorge.BasicGeopositionExtensions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Windows.Devices.Geolocation;

namespace Tests
{
    [TestClass]
    public class BasicGeopositionExtensionsTests
    {
        [TestMethod]
        public void TestGetDistanceTo()
        {
            BasicGeoposition ArnosGrove = new BasicGeoposition()
            {
                Latitude = 51.6163,
                Longitude = 0.1335
            };

            BasicGeoposition FiveKilometresEast = new BasicGeoposition()
            {
                Latitude = 51.6163,
                Longitude = 0.2059
            };

            var result = ArnosGrove.GetDistanceTo(FiveKilometresEast);

            Assert.AreEqual(5, result);
        }
    }
}