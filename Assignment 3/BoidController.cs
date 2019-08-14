/*
 *  Alwien Dippenaar
 *  V00849850
 *  CSC 305 Assignment 3
 */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{

    public GameObject boidPrefab;
    public GameObject goalMarker;

    public GameObject predator;
    
    public int boidCount = 7;
    public float speed = 1.25f;

    public float separation = .75f;

    public float alignment = 1.0f;
    public float allignmentWeight = 1.0f;

    public float cohesion = .75f;
    public float cohesionWeight = 1.0f;

    public float predDistance = 3f;
    private float predSeparation = 1.0f;

    private Vector3[] direction;

    private List<GameObject> boidsList;

    public class BoidStatus
    {
        public Vector3 position;
        public GameObject boidObject;
    }
    public static List<BoidStatus> statusList;

    public static List<HailController.HailStatus> hailList = new List<HailController.HailStatus>();

    void Awake() 
    {
        //Instantiate Boids
        boidsList = new List<GameObject>();
        statusList = new List<BoidStatus>();
        hailList = HailController.hailList; // Get list from HailController

        for (int i = 0; i < boidCount; ++i)
        {
            for (int j = 0; j < boidCount; ++j)
            {
                GameObject newBoid = new GameObject();
                newBoid.transform.parent = gameObject.transform;
                newBoid.name = "Boid No." + (i * boidCount + j).ToString();

                GameObject instPrefab = Instantiate(boidPrefab); // insantiate the bird
                instPrefab.transform.parent = newBoid.transform;

                Vector3 startingPos = new Vector3(Random.Range(-2f, -2.5f), Random.Range(0.5f, 1f), Random.Range(-2f, -2.5f));
                BoidStatus status = new BoidStatus();
                status.position = startingPos + goalMarker.transform.position;
                newBoid.transform.position = status.position;
                newBoid.transform.localScale = new Vector3(.25f, .25f, .25f);
                status.boidObject = newBoid;
                boidsList.Add(newBoid);
                statusList.Add(status);
            }
        }
        direction = new Vector3[statusList.Count];
        //hide the original prefab
        boidPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < statusList.Count; ++i)
        {
            BoidStatus status = statusList[i];
            List<BoidStatus> neighbors = new List<BoidStatus>();

            Vector3 V1 = new Vector3(0, 0, 0); // Separation
            Vector3 V2 = new Vector3(0, 0, 0); // Alignment
            Vector3 V3 = new Vector3(0, 0, 0); // Cohesion
            Vector3 V4 = new Vector3(0, 0, 0); // Predator Avoidance
           
            // Separation
            for (int j = 0; j < statusList.Count - 1; ++j)
            {
                BoidStatus nextStatus = statusList[j]; // Check every boid in list

                Vector3 boidVec = Vector3.Normalize(status.boidObject.transform.position - nextStatus.boidObject.transform.position);
                float distance = Vector3.Distance(status.boidObject.transform.position, nextStatus.boidObject.transform.position);
                // if distance is 0 then boids are the same
                if (distance <= separation && distance > 0) // Boids are too close
                {
                    V1 = (boidVec / distance) * separation;
                    status.boidObject.transform.position += V1 * Time.deltaTime * speed;
                    
                    neighbors.Add(nextStatus); // Keep track of all neighbors
                }
                else // They are an appropriate distance away
                {
                    status.boidObject.transform.position += new Vector3(0f, 0f, 0f) * speed * Time.deltaTime;
                }
            }

            // Alignment
            V2 = BoidDir(status.position);

            // Cohesion
            Vector3 centerVec = new Vector3(0, 0, 0);
            V3 = new Vector3(0, 0, 0);
            for (int j = 0; j < neighbors.Count; ++j)
            {
                centerVec += goalMarker.transform.position;
            }
            centerVec = centerVec / neighbors.Count;
            if (centerVec.magnitude <= cohesion)
            {
                V3 = Vector3.Normalize(centerVec / centerVec.magnitude);
                status.boidObject.transform.position += V3 * Time.deltaTime * speed;
            }
            
            // Predator Avoidance, similar to separation -> separate from predator arrow
            Vector3 predVec = Vector3.Normalize(status.boidObject.transform.position - predator.transform.position);
            float pDistance = Vector3.Distance(predator.transform.position, status.boidObject.transform.position);

            if (pDistance <= predDistance && pDistance > 0) // change boids direction to keep away from predator
            {
                V4 = (predVec / pDistance) * predSeparation;
                direction[i] = Vector3.Normalize(V4 - status.boidObject.transform.position);
                predSeparation = predSeparation + .2f;
            }
            else // predator is far enough away
            {
                predSeparation = 1.0f;
                direction[i] = Vector3.Normalize(V2 - status.boidObject.transform.position);
            }

            // Check for hail hit
            for (int j = 0; j < hailList.Count; ++j)
            {
                Vector3 boid = status.boidObject.transform.position;
                Vector3 hail = hailList[j].hailObject.transform.position;
                if (Vector3.Distance(boid, hail) <= .1f && hail.y >= boid.y)
                {
                    float hitDiff = Mathf.Abs(direction[i].y - hailList[j].fallingDirection.y);
                    //direction[i] = Vector3.Normalize(new Vector3(direction[i].x, direction[i].z - hitDiff, direction[i].z));

                    status.boidObject.transform.position = Vector3.Normalize(new Vector3(direction[i].x, 0, direction[i].z));// When a piece of hail hits a boid then it falls to the same direction as the hail peice.
                } 
            }
            status.boidObject.transform.up = direction[i];
            status.boidObject.transform.position += direction[i] * Time.deltaTime * speed;
            
            statusList[i] = status;
        }
    }

    // Alignment, point in same direction as red arrow
    public Vector3 BoidDir(Vector3 status)
    {
        Vector3 tempMarker = goalMarker.transform.position;
        // For direction 
        if (tempMarker.y >= status.y) // Boid is below the red arrow
        {
            float heightDif = tempMarker.y - status.y;
            tempMarker.y = tempMarker.y + heightDif; // follow a point directly below red arrow
        }
        else if (tempMarker.y < status.y) // Boid is above red arrow
        {
            float heightDif = status.y - tempMarker.y;
            tempMarker.y = tempMarker.y - heightDif; // follow a point directly above red arrow
        }
        return tempMarker;
    }
}


