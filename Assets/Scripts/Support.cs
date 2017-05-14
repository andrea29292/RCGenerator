using UnityEngine;
using System.Collections;
using System;

public class Support  {

    public static float AngleBetweenVector2(Vector2 vec1, Vector2 vec2, Vector2 center) {
        vec2 = (vec2 - center).normalized;
        vec1 = (vec1 - center).normalized;
        //Vector2 v = (vec2 - vec1).normalized;
        //float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(vec2, vec1);
    }

    public static Vector3 MovePoint(Vector3 point, float direction) {
        float varX = (float)Math.Sin(direction * Mathf.Deg2Rad);
        float varZ = (float)Math.Cos(direction * Mathf.Deg2Rad);
        return new Vector3(point.x + varX, point.y, point.z + varZ);
    }

    public static Vector3 MovePoint(Vector3 point, float direction, float distance) {
        float varX = (float)Math.Sin(direction * Mathf.Deg2Rad) * distance;
        float varZ = (float)Math.Cos(direction * Mathf.Deg2Rad) * distance;
        return new Vector3(point.x + varX, point.y, point.z + varZ);
    }

    public static float ComputeOutness(Vector3 point, float maxX, float minX, float maxZ, float minZ) {
        float outness = 0f;
        if (point.x > maxX) {
            outness = Math.Abs(point.x - maxX);
        }
        if (point.x < minX) {

            outness = Math.Abs(point.x - minX);
        }
        if (point.z < minZ) {

            outness = Math.Abs(point.z - minZ);
        }
        if (point.z > maxZ) {

            outness = Math.Abs(point.z - maxZ);
        }
        return outness;
    }
}
