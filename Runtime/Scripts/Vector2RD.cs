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
    /// Vector2 width Double values to represent RD-coordinates (X,Y)
    /// </summary>
    [System.Serializable]
    public struct Vector2RD
    {
        public double x;
        public double y;

        public Vector2RD(double X, double Y)
        {
            x = X;
            y = Y;
        }

        public bool IsInThousands
        {
            get
            {
                Debug.Log($"x:{x} y:{y}");
                return x % 1000 == 0 && y % 1000 == 0;
            }
        }
    }
}
