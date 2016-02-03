using MarkNotGeorge.BasicGeopositionExtensions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Windows.Devices.Geolocation;

namespace BGExtensionsTestApp
{
    [TestClass]
    public class UnitTests
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

            var errorDelta = 5000 * 0.003;

            Assert.AreEqual(5000, result, errorDelta);
        }
    }
}