using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Boundary
{
    public float top;
    public float bottom;
    public float left;
    public float right;

    public Boundary(float top, float bottom, float left, float right)
    {
        this.top = top;
        this.bottom = bottom;
        this.left = left;
        this.right = right;
    }

}