// Copyright © 2016 Mark Johnson Original Javascript © 2002-15 Chris Veness.
// 
// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using Windows.Devices.Geolocation;

namespace MarkNotGeorge.BasicGeopositionGeodesy
{
    public static class BasicGeopositionGeodesy
    {
        private static double earthRadius = 6371000;

        private static double ToRadians(this double angle)
        {
            return (Math.PI / 180) * angle;
        }

        private static double ToDegrees(this double angle)
        {
            return angle * (180 / Math.PI);
        }

        /// <summary>
        /// Returns the great-circle distance from this BasicGeoposition to the supplied
        /// BasicGeoposition, using the Haversine formula.
        /// </summary>
        /// <param name="toPoint">The destination BasicGeocoordinate.</param>
        /// <returns>The distance, in metres.</returns>
        public static double DistanceTo(this BasicGeoposition thisPoint, BasicGeoposition toPoint)
        {
            var lat1 = thisPoint.Latitude.ToRadians();
            var lat2 = toPoint.Latitude.ToRadians();
            var lon1 = thisPoint.Longitude.ToRadians(); var lon2 = toPoint.Longitude.ToRadians();

            var dLat = lat2 - lat1;
            var dLon = lon2 - lon1;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = earthRadius * c;

            return d;
        }

        /// <summary>
        /// Returns the (initial) bearing from this BasicGeoposition to the supplied
        /// BasicGeoposition, in degrees see http://williams.best.vwh.net/avform.htm#Crs
        /// </summary>
        /// <param name="toPoint">The destination BasicGeoposition</param>
        /// <returns>Initial bearing, in degrees clockwise from True North</returns>
        public static double InitialBearing(this BasicGeoposition thisPoint, BasicGeoposition toPoint)
        {
            var lat1 = thisPoint.Latitude.ToRadians();
            var lat2 = toPoint.Latitude.ToRadians();
            var dLon = (toPoint.Longitude - thisPoint.Longitude).ToRadians();

            var y = Math.Sin(dLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) -
                    Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);

            var brng = Math.Atan2(y, x);

            return (brng.ToDegrees() + 360) % 360;
        }

        /// <summary>
        /// Returns final bearing arriving at supplied destination BasicGeoposition from this
        /// BasicGeoposition; the final bearing will differ from the initial bearing by varying
        /// degrees according to distance and latitude.
        /// </summary>
        /// <param name="toPosition">The destination BasicGeoposition</param>
        /// <returns>Final bearing, in degrees clockwise from True North</returns>
        public static double FinalBearing(this BasicGeoposition thisPoint, BasicGeoposition toPoint)
        {
            var initialBearing = toPoint.InitialBearing(thisPoint);

            return (initialBearing + 180) % 360;
        }

        /// <summary>
        /// Returns the midpoint between this Position and the supplied Position. see
        /// http://mathforum.org/library/drmath/view/51822.html for derivation
        /// </summary>
        /// <param name="toPosition">The destination Position</param>
        /// <returns>Midpoint between this Position and the supplied Position</returns>
        public static BasicGeoposition MidpointTo(this BasicGeoposition thisPoint, BasicGeoposition toPoint)
        {
            var lat1 = thisPoint.Latitude.ToRadians();
            var lon1 = thisPoint.Longitude.ToRadians();
            var lat2 = toPoint.Latitude.ToRadians();
            var dLon = (toPoint.Longitude - thisPoint.Longitude).ToRadians();

            var Bx = Math.Cos(lat2) * Math.Cos(dLon);
            var By = Math.Cos(lat2) * Math.Sin(dLon);

            var lat3 = Math.Atan2(Math.Sin(lat1) + Math.Sin(lat2),
                    Math.Sqrt((Math.Cos(lat1) + Bx) * (Math.Cos(lat1) + Bx) + By * By));
            var midLon = lon1 + Math.Atan2(By, Math.Cos(lat1) + Bx);
            var lon3 = (midLon + 3 * Math.PI) % (2 * Math.PI) - Math.PI;  // normalise to -180..+180º

            return new BasicGeoposition()
            {
                Latitude = lat3.ToDegrees(),
                Longitude = lon3.ToDegrees()
            };
        }
    }
}