/*
 *  Code from Lecture Demo
 *  
 *  Alwien Dippenaar
 *  V00849850
 *  CSC 305 Assignment 3
 */

//This is the class for creating key driven 
//floating point properties with linear interpolation in between
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FloatKeyframeAnimator
{
    class KeyTimePair : IComparable<KeyTimePair>
    {
        public float key;
        public float time;
        public KeyTimePair(float key_in, float time_in)
        {
            this.key = key_in;
            this.time = time_in;
        }

        public int CompareTo(KeyTimePair rhs)
        {
            if (this.time < rhs.time) return -1;
            if (this.time > rhs.time) return 1;
            return 0;
        }
    }

    List<KeyTimePair> key_list;

    public FloatKeyframeAnimator()
    {
        key_list = new List<KeyTimePair>();
    }

    public int AddKey(float key, float time)
    {
        KeyTimePair new_pair = new KeyTimePair(key, time);
        key_list.Add(new_pair);
        key_list.Sort();
        return key_list.Count;
    }

    //Linear Interpolation, no loop, so once we are out of range the animation stops
    public float Sample(float time_in)
    {
        if (key_list.Count == 0) return 0;
        if (time_in < key_list[0].time) return key_list[0].key;
        for (int i = 1; i < key_list.Count; ++i)
        {
            if (time_in < key_list[i].time)
            {
                float key_span = key_list[i].time - key_list[i - 1].time;
                float fraction = 0;
                if (key_span > 0)
                {
                    fraction = (time_in - key_list[i - 1].time) / key_span;
                }
                return key_list[i - 1].key * (1 - fraction) + key_list[i].key * fraction;
            }
        }
        return key_list[key_list.Count - 1].key;
    }

    public void Clear()
    {
        key_list.Clear();
    }

}
