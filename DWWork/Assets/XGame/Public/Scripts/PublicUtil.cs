using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ezfun_resource;

public static class NullUtil
{
    public static bool IsNull(this System.Object o)
    {
        return o == null;
    }
    public static bool IsNull(this UnityEngine.Object o)
    {
        return o == null;
    }
}

public static class Vector2Util
{
    public static Vector2 MaxBound(this Vector2 v1, Vector2 v2)
    {
        return v1.Bound() > v2.Bound() ? v1 : v2;
    }

    public static float Bound(this Vector2 v)
    {
        return v.x * v.y;
    }

    public static bool MaxBound(this Vector2 v1, Vector2 v2, ref Vector2 maxVec)
    {
        if (v1.Bound() > v2.Bound())
        {
            maxVec = v1;
            return true;
        }
        else
        {
            maxVec = v2;
            return false;
        }
    }
}
