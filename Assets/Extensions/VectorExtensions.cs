using UnityEngine;
using System.Collections;

public static class VectorExtensions
{
    public static Vector3 Lerp(this Vector3 v1, Vector3 targVec, float t, float epsilon)
    {
        v1.x = v1.x.Lerp(targVec.x, t, epsilon);
        v1.y = v1.y.Lerp(targVec.y, t, epsilon);
        v1.z = v1.z.Lerp(targVec.z, t, epsilon);

        return v1;
    }

    public static Vector3 ScaleReturn(this Vector3 v1, Vector3 v2)
    {
        Vector3 scaledVec = v1;
        scaledVec.Scale(v2);
        return scaledVec;
    }

    //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
    //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
    //same plane, use ClosestPointsOnTwoLines() instead.
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        intersection = Vector3.zero;

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //Lines are not coplanar. Take into account rounding errors.
        if ((planarFactor >= 0.00001f) || (planarFactor <= -0.00001f))
        {

            return false;
        }

        //Note: sqrMagnitude does x*x+y*y+z*z on the input vector.
        float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;

        if ((s >= 0.0f) && (s <= 1.0f))
        {

            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }

        else
        {
            return false;
        }
    }

    public static Vector3 Divide(this Vector3 v1, Vector3 v2)
    {
        Vector3 dividedVec = Vector3.zero;
        dividedVec.x = v1.x / v2.x;
        dividedVec.x = v1.y / v2.y;
        dividedVec.x = v1.z / v2.z;

        return dividedVec;
    }

    public static Vector3 RangedToNormalized(this Vector3 v, float min, float max)
    {
        v.x.ScaleTo01(min, max);
        v.y.ScaleTo01(min, max);
        v.z.ScaleTo01(min, max);

        return v;
    }

    public static Vector3 RangedToUnitLegnth(this Vector3 v, float min, float max)
    {
        v.x.ScaleTo01(min, max);
        v.y.ScaleTo01(min, max);
        v.z.ScaleTo01(min, max);

        return v;
    }

    public static Vector3 NormalizedToRange(this Vector3 v, float min, float max)
    {
        v.x.ScaleTo01(min, max);
        v.y.ScaleTo01(min, max);
        v.z.ScaleTo01(min, max);

        return v;
    }

    public static Vector3 Clamp(this Vector3 v, float min, float max)
    {
        v.x = Mathf.Clamp(v.x, min, max);
        v.y = Mathf.Clamp(v.y, min, max);
        v.z = Mathf.Clamp(v.z, min, max);

        return v;
    }

    public static Vector3 Parse(string rString)
    {
        string[] temp = rString.Substring(1, rString.Length - 2).Split(',');
        float x = float.Parse(temp[0]);
        float y = float.Parse(temp[1]);
        float z = float.Parse(temp[2]);
        Vector3 rValue = new Vector3(x, y, z);
        return rValue;
    }

    public static Vector4 ParseVec4(string rString)
    {
        string[] temp = rString.Substring(1, rString.Length - 2).Split(',');
        float x = float.Parse(temp[0]);
        float y = float.Parse(temp[1]);
        float z = float.Parse(temp[2]);
        float w = float.Parse(temp[3]);
        Vector3 rValue = new Vector4(x, y, z, w);
        return rValue;
    }

    public static Vector3 RestrictToRadius(this Vector3 v, Vector3 inputPos, float radius)
    {
        Vector3 restrictedPos = v;
        Vector3 direction = inputPos - v;
        restrictedPos = v + (direction.normalized * radius);

        return restrictedPos;
    }

    public static Vector3 RotatePointAroundPivot(this Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }


    public static float SignedAngleBetweenVectors(this Vector3 A, Vector3 B)
    {
        float angle = 0;
        angle = Mathf.Atan2(A.x * B.y - A.y * B.x, A.x * B.x + A.y * B.y);
        return angle * Mathf.Rad2Deg;
    }

    public static float SignedAngleBetweenVectors(this Vector2 A, Vector2 B)
    {
        float angle = 0;
        angle = Mathf.Atan2(A.x * B.y - A.y * B.x, A.x * B.x + A.y * B.y);
        return angle * Mathf.Rad2Deg;
    }

}