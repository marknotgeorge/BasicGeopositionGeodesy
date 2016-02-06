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

        /// <summary>
        /// Returns the destination point from this point having travelled the given distance on the
        /// given initial bearing (bearing may vary before destination is reached)
        /// </summary>
        /// <param name="brng">Initial bearing in degrees from True North</param>
        /// <param name="dist">Distance in metres</param>
        /// <returns></returns>
        public static BasicGeoposition Destination(this BasicGeoposition thisPoint, double bearing, double distance)
        {
            // Convert distance to angular distance in radians
            double angularDistance = distance / earthRadius;

            var lat1 = thisPoint.Latitude.ToRadians();
            var lon1 = thisPoint.Longitude.ToRadians();

            var lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(angularDistance) +
                Math.Cos(lat1) * Math.Sin(angularDistance) * Math.Cos(bearing.ToRadians()));

            var midLon = lon1 + Math.Atan2(Math.Sin(bearing.ToRadians()) * Math.Sin(angularDistance) * Math.Cos(lat1),
                               Math.Cos(angularDistance) - Math.Sin(lat1) * Math.Sin(lat2));

            var lon2 = (midLon + 3 * Math.PI) % (2 * Math.PI) - Math.PI;

            return new BasicGeoposition()
            {
                Latitude = lat2.ToDegrees(),
                Longitude = lon2.ToDegrees()
            };
        }

        /// <summary>
        /// Returns the point of intersection of two paths defined by Position and bearing
        /// 
        /// see http://williams.best.vwh.net/avform.htm#Intersection
        /// </summary>
        /// <param name="firstBearing">Initial bearing from first point in degrees</param>
        /// <param name="secondPoint">Second Position</param>
        /// <param name="secondBearing">Initial bearing from second point in degrees</param>
        /// <returns></returns>
        public static BasicGeoposition? Intersection(this BasicGeoposition firstPoint, double firstBearing,
            BasicGeoposition secondPoint, double secondBearing)
        {
            // see http://williams.best.vwh.net/avform.htm#Intersection

            var lat1 = firstPoint.Latitude.ToRadians();
            var lon1 = firstPoint.Longitude.ToRadians();
            var lat2 = secondPoint.Latitude.ToRadians();
            var lon2 = secondPoint.Longitude.ToRadians();
            var theta13 = firstBearing.ToRadians();
            var theta23 = secondBearing.ToRadians();
            var deltaLat = lat2 - lat1;
            var deltaLon = lon2 - lon1;

            var delta12 = 2 * Math.Asin(Math.Sqrt(Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2)
                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2)));
            if (delta12 == 0)
                return null;

            // initial/final bearings between points
            var theta1 = Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(delta12)) / (Math.Sin(delta12) * Math.Cos(lat1)));
            if (double.IsNaN(theta1)) theta1 = 0; // protect against rounding
            var theta2 = Math.Acos((Math.Sin(lat1) - Math.Sin(lat2) * Math.Cos(delta12)) / (Math.Sin(delta12) * Math.Cos(lat2)));

            var theta12 = Math.Sin(lon2 - lon1) > 0 ? theta1 : 2 * Math.PI - theta1;
            var theta21 = Math.Sin(lon2 - lon1) > 0 ? 2 * Math.PI - theta2 : theta2;

            var alpha1 = (theta13 - theta12 + Math.PI) % (2 * Math.PI) - Math.PI; // angle 2-1-3
            var alpha2 = (theta21 - theta23 + Math.PI) % (2 * Math.PI) - Math.PI; // angle 1-2-3

            if (Math.Sin(alpha1) == 0 && Math.Sin(alpha2) == 0) return null; // infinite intersections
            if (Math.Sin(alpha1) * Math.Sin(alpha2) < 0) return null;      // ambiguous intersection

            //α1 = Math.abs(α1);
            //α2 = Math.abs(α2);
            // ... Ed Williams takes abs of α1/α2, but seems to break calculation?

            var alpha3 = Math.Acos(-Math.Cos(alpha1) * Math.Cos(alpha2) + Math.Sin(alpha1) * Math.Sin(alpha2) * Math.Cos(delta12));
            var delta13 = Math.Atan2(Math.Sin(delta12) * Math.Sin(alpha1) * Math.Sin(alpha2), Math.Cos(alpha2) + Math.Cos(alpha1) * Math.Cos(alpha3));
            var lat3 = Math.Asin(Math.Sin(lat1) * Math.Cos(delta13) + Math.Cos(lat1) * Math.Sin(delta13) * Math.Cos(theta13));
            var deltaLon13 = Math.Atan2(Math.Sin(theta13) * Math.Sin(delta13) * Math.Cos(lat1), Math.Cos(delta13) - Math.Sin(lat1) * Math.Sin(lat3));
            var lon3 = lon1 + deltaLon13;

            return new BasicGeoposition()
            {
                Latitude = lat3.ToDegrees(),
                Longitude = lon3.ToDegrees()
            };
        }
    }
}