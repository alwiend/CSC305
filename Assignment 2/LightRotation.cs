/* 
UVic CSC 305, 2019 Spring
Assignment 02
Name: Alwien Dippenaar 
UVic ID: V00849850
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRotation : MonoBehaviour {

    public float speed = 10.0f;

    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
