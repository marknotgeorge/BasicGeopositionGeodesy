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

        [TestMethod]
        public void TestDestination()
        {
            Initialize();
            var latitudeDelta = FiveKilometresEast.Latitude * errorPercent;
            var longitudeDelta = FiveKilometresEast.Longitude * errorPercent;

            var result = ArnosGrove.Destination(90, 5000);

            Assert.AreEqual(result.Latitude, FiveKilometresEast.Latitude, latitudeDelta);
            Assert.AreEqual(result.Longitude, FiveKilometresEast.Longitude, longitudeDelta);
        }

        [TestMethod]
        public void TestIntersection()
        {
            double expectedLat = 50.9076;
            double expectedLon = 4.5086;

            BasicGeoposition result;

            BasicGeoposition firstPosition = new BasicGeoposition()
            {
                Latitude = 51.8853,
                Longitude = 0.2545
            };

            BasicGeoposition secondPosition = new BasicGeoposition()
            {
                Latitude = 49.0034,
                Longitude = 2.5735
            };

            double firstBearing = 108.55;
            double secondBearing = 32.44;

            var latitudeDelta = expectedLat * errorPercent;
            var longitudeDelta = expectedLon * errorPercent;

            var testValue = firstPosition.Intersection(firstBearing, secondPosition, secondBearing);

            if (testValue != null)
            {
                result = (BasicGeoposition)testValue;

                Assert.AreEqual(expectedLat, result.Latitude, latitudeDelta);
                Assert.AreEqual(expectedLon, result.Longitude, longitudeDelta);
            }
        }
    }
}