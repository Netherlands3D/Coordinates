﻿/*
*  Copyright (C) X Gemeente
*                X Amsterdam
*                X Economic Services Departments
*
*  Licensed under the EUPL, Version 1.2 or later (the "License");
*  You may not use this work except in compliance with the License.
*  You may obtain a copy of the License at:
*
*    https://github.com/Amsterdam/3DAmsterdam/blob/master/LICENSE.txt
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" basis,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
*  implied. See the License for the specific language governing
*  permissions and limitations under the License.
*/

using System;
using UnityEngine;

namespace Netherlands3D.Coordinates
{
    /// <summary>
    /// Convert coordinates between Unity, ECEF (EPSG4936) and RD(EPSG7415)
    /// <!-- accuracy: WGS84 to RD  X <0.01m, Y <0.02m H <0.03m, tested in Amsterdam with PCNapTrans-->
    /// <!-- accuracy: RD to WGS84  X <0.01m, Y <0.02m H <0.03m, tested in Amsterdam with PCNapTrans-->
    /// </summary>
    public static class EPSG4936
    {
        public static Vector3ECEF relativeCenter;

        //setup coefficients for ecef-calculation ETRD89
        private static double semimajorAxis = 6378137;
        private static double flattening = 0.003352810681183637418;
        private static double eccentricity = 0.0818191910428;

        public static Vector3WGS ToWGS84(Vector3ECEF ecefCoordinate)
        {
            double eta = Math.Pow(eccentricity, 2) / (1 - Math.Pow(eccentricity, 2));
            double b = semimajorAxis * (1 - flattening);
            double p = Math.Sqrt(Math.Pow(ecefCoordinate.X, 2) + Math.Pow(ecefCoordinate.Y, 2));
            double q = Math.Atan2((ecefCoordinate.Z * semimajorAxis), p * b);

            double lattitude = Math.Atan2((ecefCoordinate.Z + eta * b * Math.Pow(Math.Sin(q), 3)), p - Math.Pow(eccentricity, 2) * semimajorAxis * Math.Pow(Math.Cos(q), 3));
            double longitude = Math.Atan2(ecefCoordinate.Y, ecefCoordinate.X);
            double primeVerticalRadius = semimajorAxis / (Math.Sqrt(1 - (Math.Pow(eccentricity, 2) * Math.Pow(Math.Sin(lattitude), 2))));
            double height = (p / Math.Cos(lattitude)) - primeVerticalRadius;
            Vector3WGS result = new Vector3WGS(longitude * 180 / Math.PI, lattitude * 180 / Math.PI, height);

            return result;
        }

        public static Vector3 ToUnity(Vector3ECEF ecef)
        {
            Vector3 result = new Vector3();
            float deltaX = (float)(ecef.X - relativeCenter.X);
            float deltaY = (float)(ecef.Y - relativeCenter.Y);
            float deltaZ = (float)(ecef.Z - relativeCenter.Z);

            result.x = -deltaX;
            result.y = deltaZ;
            result.z = -deltaY;

            //check Rotation
            result = RotationToUp() * result;

            return result;
        }

        public static Quaternion RotationToUp()
        {

            //calculate lattitude and longitude for the origin
            Vector3WGS wgslocation = ToWGS84(relativeCenter);





            //rotate around the up-axis (=counterclockwise) with an angle of the lattitude
            //rotate -90 degrees around the up-axis, to make sure east is in the X-direction;
            Quaternion rotationToEast = Quaternion.AngleAxis((float)wgslocation.lon - 90, Vector3.up);
            Quaternion rotationToFlat = Quaternion.AngleAxis(90 - (float)wgslocation.lat, Vector3.right);
            Quaternion result = rotationToFlat * rotationToEast;

            return result;




        }

        public static Coordinate ConvertTo(Coordinate coordinate, int targetCrs)
        {
            if (coordinate.CoordinateSystem != 4936)
            {
                throw new ArgumentOutOfRangeException(
                    $"Invalid coordinate received, this class cannot convert CRS ${coordinate.CoordinateSystem}"
                );
            }

            var vector3Ecef = new Vector3ECEF(coordinate.value1, coordinate.value2, coordinate.value3);

            if(targetCrs == (int)CoordinateSystem.WGS84_LatLonHeight)
            {
                return coordinate.Convert(CoordinateSystem.WGS84_LatLonHeight);
                //var result = ToWGS84(vector3Ecef);
                //return new Coordinate(targetCrs, result.lon, result.lat, result.h);
            }
                        
            return coordinate.Convert((CoordinateSystem)targetCrs);

            throw new ArgumentOutOfRangeException(
                $"Conversion between CRS ${coordinate.CoordinateSystem} and ${targetCrs} is not yet supported"
            );
        }

        public static Vector3ECEF ToVector3ECEF(this Coordinate coordinate)
        {
            return new Vector3ECEF(
                coordinate.value1,
                coordinate.value2,
                coordinate.value3
            );
        }
    }
}
