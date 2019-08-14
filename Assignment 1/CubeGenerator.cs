/* 
UVic CSC 305, 2019 Spring
Assignment 01
Name: Alwien Dippenaar
UVic ID: V00849850
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assignment01
{
    public class CubeGenerator
    {
        string name;
        Vector3[] indexArray;

        public CubeGenerator()
        {
            /* Index points for the visible points of the cube */
            name = "CubeGenerator";
            indexArray = new [] {
                                new Vector3(.3f, .23f, 1f),
                                new Vector3(.3f, .77f, 1f),
                                new Vector3(.5f, .2f, 1f),
                                new Vector3(.5f, .8f, 1f),
                                new Vector3(.7f, .77f, 1f),
                                new Vector3(.7f, .23f, 1f),
                                };
        }

        public Texture2D GenBarycentricVis(int width, int height)
        {
            Texture2D cubeResult = new Texture2D(width, height);
          
            Vector3 origin = new Vector3(0, 0, -1);
            /* Loops to run through every pixel on view */
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    float i = y / (float)height;
                    float j = x / (float)width;
                    Vector3 direction = new Vector3(j, i, 1);
                    Vector3 baryCent;
                    float t;
                    for (int z = 0; z < indexArray.Length - 2; z++)
                    {
                        if (z == 3)
                        {
                            IntersectTriangle(origin, direction, indexArray[z - 1], indexArray[z + 1], indexArray[z + 2], out t, out baryCent);
                        }
                        else
                        {
                            IntersectTriangle(origin, direction, indexArray[z], indexArray[z + 1], indexArray[z + 2], out t, out baryCent);
                        }

                        if (baryCent.x > 0 && baryCent.x < 1 && baryCent.y > 0 && baryCent.y < 1 && baryCent.z > 0 && baryCent.z < 1)
                        {
                            float distSum = baryCent.x + baryCent.y + baryCent.z;
                            /* Normalize the result of the barycentric coordinates */
                            baryCent.x = baryCent.x / distSum;
                            baryCent.y = baryCent.y / distSum;
                            baryCent.z = baryCent.z / distSum;
                            cubeResult.SetPixel(x, y, new Color(baryCent.x, baryCent.y, baryCent.z));
                            break;
                        }
                    }
                }
            }
            cubeResult.Apply();
            return cubeResult;
        }

        public Texture2D GenUVMapping(int width, int height, Texture2D inputTexture)
        {
            Texture2D cubeResult = new Texture2D(width, height);
            
            Vector3 origin = new Vector3(0, 0, -1);
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    
                    float i = y / (float)height;
                    float j = x / (float)width;
                    
                    Vector3 direction = new Vector3(j, i, 1);
                    Vector3 baryCent;
                    float t;
                    for (int z = 0; z < indexArray.Length - 2; z++)
                    {
                        if (z == 3)
                        {
                            IntersectTriangle(origin, direction, indexArray[z - 1], indexArray[z + 1], indexArray[z + 2], out t, out baryCent);
                        }
                        else
                        {
                            IntersectTriangle(origin, direction, indexArray[z], indexArray[z + 1], indexArray[z + 2], out t, out baryCent);
                        }

                        if (baryCent.x > 0 && baryCent.x < 1 && baryCent.y > 0 && baryCent.y < 1 && baryCent.z > 0 && baryCent.z < 1)
                        {
                            float distSum = baryCent.x + baryCent.y + baryCent.z;
                            baryCent.x = baryCent.x / distSum;
                            baryCent.y = baryCent.y / distSum;
                            baryCent.z = baryCent.z / distSum;
                            Vector2 coords = new Vector2(baryCent.x * x + baryCent.y * x + baryCent.z * x, baryCent.x * y + baryCent.y * y + baryCent.z * y);
                            Color inputPixel = inputTexture.GetPixel((int)coords.x, (int)coords.y);
                           
                            cubeResult.SetPixel(x, y, inputPixel);
                            break;
                        }
                    }
                }
            }
            
            cubeResult.Apply();
            return cubeResult;
        }

        /*
         *  Finds the barycentric coordinate for a given triangle with vertices vA, vB, and vC
         */
        private bool IntersectTriangle(Vector3 origin,
                                        Vector3 direction,
                                        Vector3 vA,
                                        Vector3 vB,
                                        Vector3 vC,
                                        out float t,
                                        out Vector3 barycentricCoordinate)
        {
            barycentricCoordinate.x = CalcDist(direction, vC, vA, vB, out t);
            barycentricCoordinate.y = CalcDist(direction, vB, vC, vA, out t);
            barycentricCoordinate.z = CalcDist(direction, vA, vB, vC, out t);

            return true;
        }

        /*
         *  Calculates the distance between the point and a line on the triangle
         */
        public float CalcDist(Vector3 p, Vector3 vA, Vector3 vB, Vector3 vC, out float t)
        {
            Vector3 BC = vC - vB;
            Vector3 d = Vector3.Normalize(BC);
            Vector3 Bp = p - vB;
            Vector3 BA = vA - vB;
            Vector3 outer1 = Vector3.Cross(Bp, BC);
            Vector3 outer2 = Vector3.Cross(BA, BC);
            if (Sign(outer1, outer2))
            {
                float BG = Vector3.Dot(Bp, d);
                Vector3 g = vB + BG * d;
                t = (g - p).magnitude;
                return t;
            }
            else
            {
                t = -1;
                return t;
            }
        }

        /*
         *  Checks to see if the normal vector of v1 and v2 are on pointed the same direction. 
         */
        private bool Sign(Vector3 v1, Vector3 v2)
        {
            if(v1.x >= 0 && v1.y >= 0 && v1.z >= 0 && v2.x >= 0 && v2.y >= 0 && v2.z >= 0)
            {
                return true;  
            }
            if (v1.x <= 0 && v1.y <= 0 && v1.z <= 0 && v2.x <= 0 && v2.y <= 0 && v2.z <= 0)
            {
                return true;
            }
            return false;
        }
    }
}
