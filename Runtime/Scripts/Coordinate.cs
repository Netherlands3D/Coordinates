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
#if NEWTONSOFT
using Newtonsoft.Json;
#endif

namespace Netherlands3D.Coordinates
{
	[Serializable]
    public struct Coordinate
    {
        /// <summary>
        /// EPSG Code defining which Coordinate Reference System (CRS) the provided points relate to.
        /// </summary>
        /// <remarks>
        /// The CoordinateSystem is defined as an int and not as CoordinateSystem enum so that third-parties can
        /// add their own EPSG conversions that are not (yet) included in the enum.
        /// </remarks>
        ///
#if NEWTONSOFT
        [JsonProperty]
#endif
        [SerializeField]
        private int coordinateSystem;
        public int CoordinateSystem => coordinateSystem;
        
        /// <summary>
        /// Array representing all points for this coordinate.
        ///
        /// Since some coordinate only feature 2 points and some 3, and because coordinate system uses a different
        /// unit and meaning for a point; we have chosen to abstract this into an array with either 2 or 3 points.
        /// </summary>
        /// 
        [HideInInspector]
        public double extraLongitudeRotation;
        [HideInInspector]
        public double extraLattitudeRotation;
        public double[] Points;
        
        private CoordinateSystemOperation converter;

#if NEWTONSOFT
        [JsonIgnore]
#endif
        public double easting { 
            get {
                if (converter==null)
                {
                    converter = CoordinateSystems.operators[(CoordinateSystem)CoordinateSystem];
                }
                return Points[converter.EastingIndex()]; 
            }
            set {
                if (converter == null)
                {
                    converter = CoordinateSystems.operators[(CoordinateSystem)CoordinateSystem];
                }
                Points[converter.EastingIndex()] = value; 
            }
        }
#if NEWTONSOFT
        [JsonIgnore]
#endif
        public double northing
        {
            get {
                if (converter == null)
                {
                    converter = CoordinateSystems.operators[(CoordinateSystem)CoordinateSystem];
                }
                return Points[converter.NorthingIndex()]; }
            set {
                if (converter == null)
                {
                    converter = CoordinateSystems.operators[(CoordinateSystem)CoordinateSystem];
                }
                Points[converter.NorthingIndex()] = value; }
        }
#if NEWTONSOFT
        [JsonIgnore]
#endif
        public double height
        {
            get
            {
                if (Points.Length>2)
                {
                    return Points[2];
                }
                return 0;
            }
            set
            {
                if (Points.Length > 2)
                {
                    Points[2]=value;
                }
                
            }
        }

        public Coordinate(CoordinateSystem coordinateSystem, params double[] points)
        {
            converter = null;
            this.coordinateSystem = (int)coordinateSystem;
            Points = points;
            extraLongitudeRotation = 0;
            extraLattitudeRotation = 0;
        }
        
#if NEWTONSOFT
        [JsonConstructor]
#endif
        public Coordinate(int coordinateSystem, double[] Points, double extraLongitudeRotation, double extraLatitudeRotation)
        {
            converter = null;
            this.coordinateSystem = coordinateSystem;
            this.Points = Points;
            this.extraLongitudeRotation = extraLongitudeRotation;
            this.extraLattitudeRotation = extraLatitudeRotation;
        }
        
        public Coordinate(int coordinateSystem, params double[] points)
        {
            converter = null;
            this.coordinateSystem = coordinateSystem;
            Points = points;
            extraLongitudeRotation = 0;
            extraLattitudeRotation = 0;
        }

        public Coordinate(Vector3 unityPosition)
        {
            converter = CoordinateSystems.operators[CoordinateSystems.connectedCoordinateSystem];
            extraLattitudeRotation = 0;
            extraLongitudeRotation = 0;
            Vector3 unrotatedRelativePosition = Quaternion.Inverse(CoordinateSystems.connectedCRSToUnityUp) * unityPosition;

            if (CoordinateSystems.operators[CoordinateSystems.connectedCoordinateSystem].GetCoordinateSystemType() == CoordinateSystemType.Geocentric)
            {
                //unity.x = -deltaX;
                //unity.y = deltaZ;
                //unity.z = -deltaY;

                Points = new double[3] { -unrotatedRelativePosition.x, -unrotatedRelativePosition.z, unrotatedRelativePosition.y };
            }
            else
            {
                //cartesian X = unity X;
                //cartesian Y = unity Z;
                //cartesian Z = unity Y;

                Points = new double[3] { unrotatedRelativePosition.x, unrotatedRelativePosition.z, unrotatedRelativePosition.y };
            }
            coordinateSystem = (int)CoordinateSystems.connectedCoordinateSystem;
            Points = (CoordinateSystems.CoordinateAtUnityOrigin + new Coordinate(CoordinateSystem, Points)).Points;
        }

        public Coordinate(CoordinateSystem coordinateSystem)
        {
            converter = CoordinateSystems.operators[coordinateSystem];
            Points = new double[converter.AxisCount()];
            this.coordinateSystem = (int)coordinateSystem;
            extraLongitudeRotation = 0;
            extraLattitudeRotation = 0;
        }
        
        public static Coordinate operator +(Coordinate a, Coordinate b)
        {
            int maxcoordinatecount = a.Points.Length;
            int mincoordinatecount = b.Points.Length;
            Coordinate longestCoordainte = a;
            if (b.Points.Length>maxcoordinatecount)
            {
                maxcoordinatecount = b.Points.Length;
                mincoordinatecount = a.Points.Length;
                longestCoordainte = b;
            }
            double[] points = new double[maxcoordinatecount];
            for (int i = 0; i < mincoordinatecount; i++)
            {
                points[i] = a.Points[i] + b.Points[i];
            }
            for (int i = mincoordinatecount; i < maxcoordinatecount; i++)
            {
                points[i] = longestCoordainte.Points[i];
            }
            return new Coordinate(a.CoordinateSystem, points);
        }
        
        public static Coordinate operator -(Coordinate a, Coordinate b)
        {
            int maxcoordinatecount = a.Points.Length;
            int mincoordinatecount = b.Points.Length;
            Coordinate longestCoordainte = a;
            double remainMultiplier = 1;
            if (b.Points.Length > maxcoordinatecount)
            {
                maxcoordinatecount = b.Points.Length;
                mincoordinatecount = a.Points.Length;
                longestCoordainte = b;
                remainMultiplier = -1;
            }
            double[] points = new double[maxcoordinatecount];
            for (int i = 0; i < mincoordinatecount; i++)
            {
                points[i] = a.Points[i] - b.Points[i];
            }
            for (int i = mincoordinatecount; i < maxcoordinatecount; i++)
            {
                points[i] = longestCoordainte.Points[i]*remainMultiplier;
            }
            return new Coordinate(a.CoordinateSystem, points);
        }

        public bool IsValid()
        {
            CoordinateSystemOperation converter = CoordinateSystems.operators[(CoordinateSystem)this.CoordinateSystem];

            return converter.CoordinateIsValid(this);
        }

        public Coordinate Convert(CoordinateSystem targetCoordinateSystem)
        {

            if ((int)targetCoordinateSystem == this.CoordinateSystem)
            {
                return this;
            }

            if ((CoordinateSystem)this.CoordinateSystem == Coordinates.CoordinateSystem.Unity)
            {
                Vector3 vector3 = new Vector3((float)Points[0], (float)Points[1], (float)Points[2]);
                Coordinate coord= new Coordinate(vector3);
                return coord.Convert(targetCoordinateSystem);
            }

            CoordinateSystemOperation converter = CoordinateSystems.operators[(CoordinateSystem)this.CoordinateSystem];
            //if (converter.CoordinateIsValid(this)==false)
            //{
            //    Debug.LogWarning($"coordinate is not valid: epsg{this.CoordinateSystem} {Points.ToString()}");
            //}
            Coordinate result = converter.ConvertToWGS84LatLonH(this);
            converter = CoordinateSystems.operators[targetCoordinateSystem];
            result = converter.ConvertFromWGS84LatLonH(result);
            //if (converter.CoordinateIsValid(result) == false)
            //{
            //    Debug.LogWarning($"coordinate is not valid: epsg{result.CoordinateSystem} {result.Points.ToString()}");
            //}
            return result;
        }

        public Quaternion RotationToLocalGravityUp()
        {

            if (this.CoordinateSystem == (int)CoordinateSystems.connectedCoordinateSystem)
            {
                return CoordinateSystems.connectedCRSToUnityUp;
            }

            //return RotationToUnityUP();
            CoordinateSystemOperation myConverter = CoordinateSystems.operators[(CoordinateSystem)this.CoordinateSystem];
            CoordinateSystemOperation connectedConverter = CoordinateSystems.operators[CoordinateSystems.connectedCoordinateSystem];

            Vector3WGS orientationDifference = connectedConverter.Orientation() - myConverter.Orientation();

            Coordinate inConnectedCrs = this.Convert(CoordinateSystems.connectedCoordinateSystem);
            
            Vector3WGS extraRotation = new Vector3WGS(inConnectedCrs.extraLongitudeRotation, inConnectedCrs.extraLattitudeRotation, 0);
            
            orientationDifference += extraRotation;

            //calculate the exrtaRotation in the connected coordainteSystem at the UnityOrigin
            Coordinate pointAtOrigin = CoordinateSystems.CoordinateAtUnityOrigin.Convert(Coordinates.CoordinateSystem.WGS84_LatLon);
            Vector3WGS ExtraRotationAtOrigin = new Vector3WGS(-pointAtOrigin.extraLongitudeRotation, -pointAtOrigin.extraLattitudeRotation, 0);
           // ExtraRotationAtOrigin = new Vector3WGS(0, -pointAtOrigin.extraLattitudeRotation, 0);
           // orientationDifference += ExtraRotationAtOrigin;


            Quaternion rotationToEast = Quaternion.AngleAxis((float)orientationDifference.lon, Vector3.up);
            if (myConverter.GetCoordinateSystemType() == CoordinateSystemType.Geocentric)
            {
                //rotate -90 degrees around the up-axis, to make sure east is in the X-direction;
                rotationToEast = rotationToEast * Quaternion.AngleAxis(-90, Vector3.up);
            }
            /// Now we calculate the difference in lattitude between de localUP at the coordinate and the orientation of the coordinateSystem  
            Quaternion rotationToFlat = Quaternion.AngleAxis(-(float)orientationDifference.lat, Vector3.right);

            /// when we apply both rotations, we get the rotation required to get the coordinateSystem pointing Up and North at the Unity-Origin
            Quaternion result = Quaternion.AngleAxis(-(float)ExtraRotationAtOrigin.lon, Vector3.up) *CoordinateSystems.connectedCRSToUnityUp* rotationToFlat * rotationToEast ;

            return result;

        }
        
        public Vector3 ToUnity()
        {
            
            Coordinate connectionCoordinate = CoordinateSystems.CoordinateAtUnityOrigin;
            //transform current coordinate to connectioncoordinate;

            Coordinate inConnecedCRS = this.Convert(CoordinateSystems.connectedCoordinateSystem);
            
            //get position relative to origin
            Coordinate difference = inConnecedCRS - connectionCoordinate;
            Vector3 relativePosition = new Vector3((float)difference.Points[0], (float)difference.Points[1], (float)difference.Points[2]);
            
            //move axes to unity-equivlent axes
            if (CoordinateSystems.operators[CoordinateSystems.connectedCoordinateSystem].GetCoordinateSystemType() == CoordinateSystemType.Geocentric)
            {
                //unity.x = -deltaX;
                //unity.y = deltaZ;
                //unity.z = -deltaY;

                relativePosition = new Vector3(-relativePosition.x, relativePosition.z, -relativePosition.y);
            }
            else
            {
                //cartesian X = unity X;
                //cartesian Y = unity Z;
                //cartesian Z = unity Y;

                relativePosition = new Vector3(relativePosition.x, relativePosition.z, relativePosition.y);
            }
            //apply rotation from connectedCoordinateSystem to Unity

            Vector3 rotatedRelativePosition = CoordinateSystems.connectedCRSToUnityUp * relativePosition;
            
            return rotatedRelativePosition;
        }
    }
}
