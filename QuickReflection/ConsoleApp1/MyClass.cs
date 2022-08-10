using System.Collections;
using System.Collections.Generic;


public struct Vector3
{
    public Vector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public float x;
    public float y;
    public float z;
    public override string ToString()
    {
        return "(" + x + ","+ y + ","+ z + ")"; 
    }
}

public class MyClass
{
    public int one;
    public string str;
    public Vector3 point;

    public int One { get; set; }
    public string Str { get; set; }
    public Vector3 Point { get; set; }


    public int[] ones;
    public string[] strs;
    public Vector3[] points;


    public int point3;
    public int dd = 11;
}

