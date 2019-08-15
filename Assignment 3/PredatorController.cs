/*
 *  Alwien Dippenaar
 *  V00849850
 *  CSC 305 Assignment 3
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorController : MonoBehaviour {

    private GameObject target;
    public float speed = 3.2f;
    private int targetNum;
    public static List<BoidController.BoidStatus> boidList = new List<BoidController.BoidStatus>();
    private bool targetKilled = false;
    private bool listInitialized = false;
    private Vector3 startPos = new Vector3(0f, 10f, 0f);
    public float killRadius = 0.3f;
    public float spawnRadius = 0.2f;

    Vector3 direction;

    // Use this for initialization
    void Start() 
    {
        boidList = BoidController.statusList; // get boid list from BoidController
        transform.position = startPos;
        targetNum = Mathf.RoundToInt(Random.Range(1, boidList.Count)); // choose a random target from boid list
    }

    // Update is called once per frame
    void Update () {
        
        if (listInitialized)
        {
            //string boidName = "Boid No. " + targetNum;
            if (targetKilled && Vector3.Distance(transform.position, startPos) > spawnRadius) 
            {
               // do nothing until back at starting position
            }
            else if(targetKilled) // at starting position, get a new random target 
            {
                targetNum = Mathf.RoundToInt(Random.Range(1, boidList.Count));
                listInitialized = SetTarget();
                targetKilled = false;
            }
            else // chase target
            {
                if (Vector3.Distance(transform.position, target.transform.position) < killRadius) // "kill" the target
                {
                    target.transform.position = new Vector3(Random.Range(-2.4f, -2.5f), Random.Range(0.9f, 1f), Random.Range(-2.4f, -2.5f)); // back to starting position
                    targetKilled = true;
                    direction = Vector3.Normalize(startPos - transform.position); // send predator back to start position, so bot continuously killing boids
                }
                else
                {
                    direction = Vector3.Normalize(target.transform.position - transform.position);
                }
            }  
            transform.up = direction;
            transform.position += direction * Time.deltaTime * speed;
        }
        else
        {
            listInitialized = SetTarget(); // get target
        }        
	}

    private bool SetTarget()
    {
        target = boidList[targetNum].boidObject; // set target and check if null
        if (target == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
