/* 
UVic CSC 305, 2019 Spring
Assignment 02
Name: Alwien Dippenaar 
UVic ID: V00849850
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float speed = 10.0f;
    public float rotspeed = 50.0f;

    private Vector3 lastMouse = new Vector3(255, 255, 255);
    private readonly float camSens = 0.25f; 

    public Transform target;

    void Start()
    {
        Debug.Log("KEYS TO MOVE CAMERA ARE AS FOLLOWS:");
        Debug.Log("THE MOUSE CONRTOLS ALL ROTATION");
        Debug.Log("Q TO GO UP");
        Debug.Log("E TO GO DOWN");
        Debug.Log("W TO GO FORWARD");
        Debug.Log("S TO GO BACKWARDS");
        Debug.Log("A TO GO LEFT");
        Debug.Log("D TO GO RIGHT");
        Debug.Log("SPACE BAR TO ROTATE AROUND PLANE RELATIVE TO THE CAMERA Y AXIS");
    }

    // Update is called once per frame
    void Update () {
        lastMouse = Input.mousePosition - lastMouse;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
        transform.eulerAngles = lastMouse;
        lastMouse = Input.mousePosition;

        if (Input.GetKey(KeyCode.Q)) // UP
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }

        if (Input.GetKey(KeyCode.E)) // DOWN
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }

        if (Input.GetKey(KeyCode.W)) // FORWARD
        {
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.S)) // BACKWARD
        {
            transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.A)) // LEFT
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
       
        if (Input.GetKey(KeyCode.D)) // RIGHT
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }

        if (Input.GetKey(KeyCode.Space)) // ROTATE AROUND PLACE ON RELATIVE TO CAMERA Y AXIS
        {
            transform.RotateAround(target.position,  transform.up, rotspeed * Time.deltaTime);
        }
    }
}
