﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

    public static long GetTimeinMilliseconds()
    {
        return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public static float GetDistance(Collider first, Collider second)
    {
        return (second.transform.position - first.transform.position).sqrMagnitude;
    }

    public static Collider[] DistanceSort(Collider[] colliders, Collider goCollider)
    {
        //return colliders array if there's at most one element (no ordering needed)
        if (colliders.Length <= 1)
            return colliders;

        List<Collider> ret = new List<Collider>(colliders.Length);
        ret.Insert(0,colliders[0]);
        for (var i=1; i < colliders.Length; i++)
        {
            for (var j = 0; j < ret.Count; j++)
            {
                if (GetDistance(colliders[i], goCollider) <= GetDistance(ret[j], goCollider))
                {
                    ret.Insert(j, colliders[i]);
                    break;
                }

                //last element
                if (j == ret.Count - 1)
                {
                    ret.Insert(ret.Count, colliders[i]);
                    break;
                }
            }
        }
        return ret.ToArray();
    }

}
