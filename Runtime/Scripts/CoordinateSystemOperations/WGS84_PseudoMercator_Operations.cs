using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netherlands3D.Coordinates
{
    class WGS84_PseudoMercator_Operations : CoordinateSystemOperation
    {

        public override string Code()
        {
            return "3857";
        }

        public override int NorthingIndex()
        {
            return 1;
        }
        public override int EastingIndex()
        {
            return 0;
        }
        public override int AxisCount()
        {
            return 2;
        }
        double semiMajorAxis = 6378137.000;

        public override Coordinate ConvertFromWGS84LatLonH(Coordinate coordinate)
        {
            //epsg method code 1024
            double lattitudeRad = coordinate.value1 * Math.PI / 180d;
            double longitudeRad = coordinate.value2 * Math.PI / 180d;
            double East = semiMajorAxis * longitudeRad;
            double North = semiMajorAxis * Math.Log(Math.Tan((0.25d * Math.PI) + (lattitudeRad * 0.5d)));
            Coordinate result = new Coordinate(CoordinateSystem.WGS84_PseudoMercator, East, North);
            return result;
        }

        public override Coordinate ConvertToWGS84LatLonH(Coordinate coordinate)
        {
            //epsg method code 1024 reverse
            var x = coordinate.value1;
            var y = coordinate.value2;
            x = (x * 180d) / 20037508.34d;
            y = (y * 180d) / 20037508.34d;
            y = (Math.Atan(Math.Exp(y * (Math.PI / 180d))) * 360d) / Math.PI - 90d;

            Coordinate result = new Coordinate(CoordinateSystem.WGS84_LatLonHeight, x, y, 0);
            result.extraLattitudeRotation = 0 + coordinate.extraLattitudeRotation;
            result.extraLongitudeRotation = x + coordinate.extraLongitudeRotation;
            return new Coordinate(CoordinateSystem.WGS84_LatLonHeight, y, x, 0);
        }

        public override bool CoordinateIsValid(Coordinate coordinate)
        {
            if (coordinate.PointsLength != 2)
            {
                return false;
            }
            if (coordinate.value1 < -20037508.34)
            {
                return false;
            }
            if (coordinate.value1 > 20037508.34)
            {
                return false;
            }
            if (coordinate.value2 < -20048966.1)
            {
                return false;
            }
            if (coordinate.value2 > 20048966.1)
            {
                return false;
            }

            return true;
        }

        public override CoordinateSystemGroup GetCoordinateSystemGroup()
        {
            return CoordinateSystemGroup.None;
        }

        public override CoordinateSystemType GetCoordinateSystemType()
        {
            return CoordinateSystemType.Projected;
        }

        public override Vector3WGS GlobalUpDirection(Coordinate coordinate)
        {
            Coordinate latlon = ConvertToWGS84LatLonH(coordinate);
            return new Vector3WGS(latlon.extraLongitudeRotation, latlon.extraLattitudeRotation, 0);
        }

        public override Vector3WGS LocalUpDirection(Coordinate coordinate)
        {
            return GlobalUpDirection(coordinate);
        }

        public override Vector3WGS Orientation()
        {
            return new Vector3WGS(0, 0, 0);
        }
    }
}
