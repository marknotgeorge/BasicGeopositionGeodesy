using MarkNotGeorge.BasicGeopositionExtensions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Windows.Devices.Geolocation;

namespace BGExtensionsTestApp
{
    [TestClass]
    public class UnitTests
    {
        public BasicGeoposition ArnosGrove;
        public BasicGeoposition FiveKilometresEast;
        private double errorPercent = 0.3 / 100;

        [TestInitialize]
        public void Initialize()
        {
            ArnosGrove = new BasicGeoposition()
            {
                Latitude = 51.6163,
                Longitude = 0.1335
            };

            FiveKilometresEast = new BasicGeoposition()
            {
                Latitude = 51.6163,
                Longitude = 0.2059
            };
        }

        [TestMethod]
        public void TestGetDistanceTo()
        {
            Initialize();

            var result = ArnosGrove.GetDistanceTo(FiveKilometresEast);

            var errorDelta = 5000 * errorPercent;

            Assert.AreEqual(5000, result, errorDelta);
        }

        [TestMethod]
        public void TestInitialBearing()
        {
            Initialize();

            var result = ArnosGrove.InitialBearing(FiveKilometresEast);

            var errorDelta = 90 * errorPercent;

            Assert.AreEqual(result, 90, errorDelta);
        }

        [TestMethod]
        public void TestFinalHeading()
        {
            Initialize();

            var result = ArnosGrove.FinalBearing(FiveKilometresEast);

            var errorDelta = 90.0284 * errorPercent;

            Assert.AreEqual(result, 90, errorDelta);
        }
    }
}