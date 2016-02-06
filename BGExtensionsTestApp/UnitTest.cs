using MarkNotGeorge.BasicGeopositionGeodesy;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Windows.Devices.Geolocation;

namespace BGGeodesyTestApp
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
        public void TestDistanceTo()
        {
            Initialize();

            var result = ArnosGrove.DistanceTo(FiveKilometresEast);

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

        [TestMethod]
        public void TestMidpointTo()
        {
            Initialize();

            double expectedLatitude = 51.6163;
            double expectedLongitude = 0.1697;

            var latitudeDelta = expectedLatitude * errorPercent;
            var longitudeDelta = expectedLongitude * errorPercent;

            var result = ArnosGrove.MidpointTo(FiveKilometresEast);

            Assert.AreEqual(result.Latitude, expectedLatitude, latitudeDelta);
            Assert.AreEqual(result.Longitude, expectedLongitude, longitudeDelta);
        }
    }
}