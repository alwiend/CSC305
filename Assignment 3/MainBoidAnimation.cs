/*
 *  Alwien Dippenaar
 *  V00849850
 *  CSC 305 Assignment 3
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBoidAnimation : MonoBehaviour {

    FloatKeyframeAnimator animatorX;
    FloatKeyframeAnimator animatorZ;

	// Use this for initialization
	void Start () {
        animatorX = new FloatKeyframeAnimator();
        animatorZ = new FloatKeyframeAnimator();

        animatorX.Clear();
        animatorX.AddKey(-5, 0);
        animatorZ.AddKey(-4, 0);

        animatorX.AddKey(-5, 2);
        animatorZ.AddKey(0, 2);

        animatorX.AddKey(-3, 4);
        animatorZ.AddKey(4, 4);

        animatorX.AddKey(0, 6);
        animatorZ.AddKey(5, 6);

        animatorX.AddKey(2.5f, 8);
        animatorZ.AddKey(3, 8);

        animatorX.AddKey(5, 10);
        animatorZ.AddKey(0, 10);

        animatorX.AddKey(4.5f, 12);
        animatorZ.AddKey(-4, 12);

        animatorX.AddKey(.25f, 14);
        animatorZ.AddKey(-4, 14);

        animatorX.AddKey(-5, 16);
        animatorZ.AddKey(-4, 16);
        
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 currPos = gameObject.transform.position;
        Vector3 nextPos = gameObject.transform.position;
        
        float time = Time.time;
        float time_mod16 = time - ((int)(time / 16)) * 16;

        currPos.x = animatorX.Sample(time_mod16);
        currPos.z = animatorZ.Sample(time_mod16);

        // Find position of red arrow at next key frame
        if (time_mod16 == 16)
        {
            nextPos.x = animatorX.Sample(0);
            nextPos.z = animatorZ.Sample(0);
        }
        else
        {
            nextPos.x = animatorX.Sample(time_mod16 + 2);
            nextPos.z = animatorZ.Sample(time_mod16 + 2);
        }

        gameObject.transform.position = currPos; // Position for Red Bird
        
        gameObject.transform.up = nextPos - currPos; // Rotation for Red bird
    }
}
