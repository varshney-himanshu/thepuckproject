using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Utils
{
    static public class Utils
    {
        static Vector3 GetUnitVector(Vector3 vector)
        {
            float x = vector.x;
            float y = vector.y;
            float z = vector.z;
            x = x < 0 ? -1 : 1;
            y = y < 0 ? -1 : 1;
            z = z < 0 ? -1 : 1;
            return new Vector3(x, y, z);
        }

        static Vector3 GetAbsVector3(Vector3 v)
        {
            float x = v.x;
            float y = v.y;
            float z = v.z;

            x = Mathf.Abs(x);
            y = Mathf.Abs(y);
            z = Mathf.Abs(z);
            return new Vector3(x, y, z);
        }

    }
}