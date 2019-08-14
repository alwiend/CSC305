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
    public class SphereGenerator
    {
        string name;
        Vector3 sphereCenter;
        float sphereRadius;
        Vector3 lightSource;
        Vector3 lightCol;
        float lightPow;
        Vector3 ambientCol;
        Vector3 diffuseCol;
        Vector3 specularCol;
        float shininess;
        
        public SphereGenerator()
        {
            this.sphereCenter = new Vector3((float)1.25, (float).59, 1);
            this.sphereRadius = .55f;
            this.name = "SphereGenerator";
            this.lightSource = new Vector3(.5f, 0f, .15f);
            lightCol = new Vector3(1f, 1f, 1f);
            lightPow = 2f;
            ambientCol = new Vector3(0f, 0f, 0f);
            diffuseCol = new Vector3(.5f, 0f, 0f);
            specularCol = new Vector3(1f, 1f, 1f);
            shininess = 20.0f;
        }

        public Texture2D GenSphere(int width, int height)
        {
            Texture2D sphereResult = new Texture2D(width, height);
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    float i = x / (float)256;
                    float j = y / (float)256;
                    Vector3 origin = new Vector3(0, 0, -1);
                    Vector3 oc = sphereCenter - origin;
                    Vector3 direction = new Vector3(i, j, 1);
                    Vector3 cg = direction - sphereCenter;
                    float t;
                    Vector3 normal;

                    if (IntersectSphere(origin, direction, sphereCenter, sphereRadius, out t, out normal))
                    {
                        normal = Vector3.Normalize(normal); ;
                        Vector3 position = origin + t * direction;
                        Vector3 lightDir = lightSource - position;
                        float distance = lightDir.magnitude;
                        distance = distance * distance;
                        lightDir = Vector3.Normalize(lightDir);

                        float lambertian = Math.Max(Vector3.Dot(lightDir, normal), 0.0f);
                        float specular = 0.0f;

                        Vector3 color = new Vector3(0, 0, 0);
                        if (lambertian > 0.0f)
                        {
                            Vector3 viewDir = Vector3.Normalize(-position);
                            Vector3 halfDir = Vector3.Normalize(lightDir + viewDir);
                            float specAngle = Math.Max(Vector3.Dot(halfDir, normal), 0.0f);
                            specular = (float)Math.Pow(specAngle, shininess);
                            Vector3 diffuse = Vector3.Scale(diffuseCol, lightCol) * lambertian;
                            Vector3 spec = Vector3.Scale(specularCol, lightCol) * specular;
                            float light = lightPow / distance;

                            color = ambientCol +  diffuse * light +  spec * light;
                        }
                        sphereResult.SetPixel(x, y, new Color(color.x, color.y, color.z));
                    }
                }
            }
            
            sphereResult.Apply();
            return sphereResult;
        }

        private bool IntersectSphere(Vector3 origin,
                                        Vector3 direction,
                                        Vector3 sphereCenter,
                                        float sphereRadius,
                                        out float t,
                                        out Vector3 intersectNormal)
        {
            Vector3 d = origin - sphereCenter;
            float p1 = -Vector3.Dot(direction, d);
            float p2sqr = p1 * p1 - Vector3.Dot(d, d) + sphereRadius * sphereRadius;

            float p2 = (float)Math.Sqrt(p2sqr);
            t = p1 - p2 > 0 ? p1 - p2 : p1 + p2;
            Vector3 cg = direction - sphereCenter;
            if (sphereRadius >= cg.magnitude)
            {
                intersectNormal = Vector3.Normalize(cg - sphereCenter);
                return true;
            }
            intersectNormal = new Vector3(-1, -1, -1);
            return false;
        }
    }
}
