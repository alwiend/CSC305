/*
 *  Alwien Dippenaar
 *  V00849850
 *  CSC 305 Assignment 3
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HailController : MonoBehaviour {

    public GameObject hailPrefab;
    public int numHail = 300;
    
    public class HailStatus
    {
        public Vector3 startPosition;
        public Vector3 fallingDirection;
        public float fallingSpeed;
        public GameObject hailObject;
    }

    public static List<HailStatus> hailList; // List for every piece of hail

    //public static List<BoidController.BoidStatus> boidList = new List<BoidController.BoidStatus>();

    // Use this for initialization
    void Awake () { 
        // boidList = BoidController.statusList;
        hailList = new List<HailStatus>();

        for (int i = 0; i < numHail; ++i)
        {
            GameObject newHail = new GameObject();
            newHail.transform.parent = gameObject.transform;
            newHail.name = "Hail no. " + i.ToString();

            GameObject instPrefab = Instantiate(hailPrefab); // Instantiate the hail piece
            instPrefab.transform.parent = newHail.transform;

            Vector3 startingPos = new Vector3(Random.Range(-7f, 7f), 12f, Random.Range(-7f, 7f));
            HailStatus status = new HailStatus();
            newHail.transform.position = startingPos;
            status.startPosition = startingPos;
            status.hailObject = newHail;
            status.fallingSpeed = Random.Range(1f, 4f);
            status.fallingDirection = new Vector3(Random.Range(-.25f, .25f), -1.0f, Random.Range(-.25f, .25f));

            hailList.Add(status);
        }
        hailPrefab.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < numHail; ++i)
        {
            HailStatus status = hailList[i];

            Vector3 falling = new Vector3(Random.Range(-.25f, .25f), -1f, Random.Range(-.25f, .25f));
            status.fallingDirection = falling;
            Vector3 direction = Vector3.Normalize(status.fallingDirection);

            if (status.hailObject.transform.position.y <= 0.3f)
            {
                status.hailObject.transform.position = status.startPosition;
            }

            status.hailObject.transform.position += direction * Time.deltaTime * status.fallingSpeed;
        }
	}
}
