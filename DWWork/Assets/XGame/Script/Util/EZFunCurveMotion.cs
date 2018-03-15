using UnityEngine;
using System;
using System.Collections;

//andeeee from the Unity forum's steller Catmull-Rom class ( http://forum.unity3d.com/viewtopic.php?p=218400#218400 ):
public class EZfunCurve
{
    public Vector3[] points;

    private Vector3[] pts;

    private float[] lengthDistribute;

    private float length;

    public EZfunCurve(Vector3[] pts)
    {
        points = new Vector3[pts.Length];
        Array.Copy(pts, points, pts.Length);
        this.pts = PathControlPointGenerator(pts);
        length = CulculatePathLength();
    }

    public static Vector3 Interp(Vector3[] points, float t, out Vector3 tangent)
    {
        int numSections = points.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;
        Vector3 a = points[currPt];
        Vector3 b = points[currPt + 1];
        Vector3 c = points[currPt + 2];
        Vector3 d = points[currPt + 3];
        tangent = .5f * (3 * (-a + 3f * b - 3f * c + d) * (u * u) + 2 * (2f * a - 5f * b + 4f * c - d) * u + (-a + c));
        return .5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
    }

    private static Vector3[] PathControlPointGenerator(Vector3[] path)
    {
        Vector3[] suppliedPath;
        Vector3[] vector3s;

        //create and store path points:
        suppliedPath = path;

        //populate calculate path;
        int offset = 2;
        vector3s = new Vector3[suppliedPath.Length + offset];
        Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);

        //populate start and end control points:
        //vector3s[0] = vector3s[1] - vector3s[2];
        vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
        vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

        return (vector3s);
    }

    public Vector3 Interp(float t, out Vector3 tangent)
    {
        Vector3 pt = Interp(pts, t, out tangent);
        tangent = tangent.normalized;
        return pt;
    }

    public float CulculatePathLength()
    {
        float pathLength = 0;

        lengthDistribute = new float[points.Length];

        Vector3 dir;

        Vector3 prevPt = Interp(0, out dir);
        lengthDistribute[0] = 0f;
        int SmoothAmount = (points.Length - 1) * 20;
        for (int i = 1; i <= SmoothAmount; i++)
        {
            float pm = (float)i / SmoothAmount;
            Vector3 currPt = Interp(pm, out dir);
            float delta = Vector3.Distance(prevPt, currPt);
            pathLength += delta;
            if (i % 20 == 0)
            {
                lengthDistribute[i / 20] = pathLength;
            }
            prevPt = currPt;
        }

        return pathLength;
    }

    public Vector3 MoveAlongPath(ref float t, float distance, out Vector3 dir)
    {
        int numSections = pts.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        float beginLength = lengthDistribute[currPt];
        float endLength = lengthDistribute[currPt + 1];

        float curLength = Mathf.Lerp(beginLength, endLength, u);

        if (curLength + distance <= endLength)
        {
            //移动后还在本区间内
            t += distance / (endLength - beginLength) / numSections;
            return Interp(t, out dir);
        }
        else if (currPt + 1 < numSections)
        {
            //越过了本区间末尾，进入了下一区间
            t = (currPt + 1) / (float)numSections;
            return MoveAlongPath(ref t, curLength + distance - endLength, out dir);
        }
        else
        {
            //已到路径末尾，直接返回最后一个点
            t = 1;
            dir = (pts[currPt + 3] - pts[currPt + 2]).normalized;
            return pts[currPt + 2];
        }
    }

}

public class EZFunCurveMotion {
    
}
