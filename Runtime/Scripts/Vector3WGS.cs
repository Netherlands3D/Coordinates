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

using UnityEngine;

namespace Netherlands3D.Coordinates
{
    /// <summary>
    /// Vector3 width Double values to represent WGS84-coordinates (Lon,Lat,H)
    /// </summary>
    public struct Vector3WGS
    {
        public double lat;
        public double lon;
        public double h;
        public Vector3WGS(double Lon, double Lat, double H)
        {
            lat = Lat;
            lon = Lon;
            h = H;
        }
        public static Vector3WGS operator +(Vector3WGS a, Vector3WGS b )
        {
            return new Vector3WGS(a.lon + b.lon, a.lat + b.lat, a.h + b.h);
        }
        public static Vector3WGS operator -(Vector3WGS a, Vector3WGS b)
        {
            return new Vector3WGS(a.lon - b.lon, a.lat - b.lat, a.h - b.h);
        }

        public  Quaternion UnityQuaterion()
        {
           return Quaternion.AngleAxis((float)lon, Vector3.up)* Quaternion.AngleAxis((float)(lat), Vector3.right) ;
        }
    }
}
