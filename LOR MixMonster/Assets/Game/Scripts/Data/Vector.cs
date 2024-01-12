using System;
using UnityEngine;

//



[Serializable]
public class Vector
{
    public float x, y, z;

    public Vector()
    {
    }

    public Vector(Vector3 position)
    {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
    }

    public Vector(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 Vector3()
    {
        return new Vector3(x, y, z);
    }

    public void Set(Vector3 position)
    {
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
    }
}