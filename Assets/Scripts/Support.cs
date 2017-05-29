using UnityEngine;
using System.Collections;
using System;

public class Support {

    //given two points and one direction, it gives the direction needed to go from vec1 to vec2
    public static float WhichCurve(Vector3 vec1, Vector3 vec2, float prevDirection, float segmentLenght) {
        Vector2 v1 = new Vector2(vec1.x, vec1.z);
        Vector2 v2 = new Vector2(vec2.x, vec2.z);
        Vector3 followPoint3 = MovePoint(vec1, prevDirection, segmentLenght);
        Vector2 followPoint = new Vector2(followPoint3.x, followPoint3.z);
        float angle = AngleBetweenVector2(followPoint, v2, v1);
        Vector3 point1 = MovePoint(v1, (prevDirection + angle) % 360, segmentLenght);
        Vector3 point2 = MovePoint(v1, (prevDirection - angle) % 360, segmentLenght);
        Debug.DrawLine(new Vector3(point1.x + 1, point1.y, point1.z), new Vector3(point1.x - 1, point1.y, point1.z),Color.green, 10);
        Debug.DrawLine(new Vector3(point1.x, point1.y, point1.z+1), new Vector3(point1.x, point1.y, point1.z-1), Color.green, 10);
        Debug.DrawLine(new Vector3(point2.x + 1, point2.y, point2.z), new Vector3(point2.x - 1, point2.y, point2.z), Color.green, 10);
        Debug.DrawLine(new Vector3(point2.x, point2.y, point2.z + 1), new Vector3(point2.x, point2.y, point2.z - 1),Color.green,10);

        if (Vector3.Distance( point1,v2) <= Vector3.Distance(point2, v2))
            return (prevDirection + angle) % 360;
        else return (prevDirection - angle) % 360;

    }

    public static float AngleBetweenVector2(Vector2 vec1, Vector2 vec2, Vector2 center) {
        vec2 = (vec2 - center).normalized;
        vec1 = (vec1 - center).normalized;
        //Vector2 v = (vec2 - vec1).normalized;
        //float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(vec2, vec1);
    }

    //create new point from point parameter and a direction
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

    public static void ShowPoint(Vector3 point) {
        Debug.DrawLine(new Vector3(point.x - 1, point.y, point.z), new Vector3(point.x + 1, point.y, point.z), Color.cyan);
        Debug.DrawLine(new Vector3(point.x, point.y, point.z + 1), new Vector3(point.x, point.y, point.z - 1), Color.cyan);
    }

    public static void ShowPoint(Vector3 point, float duration) {
        Debug.DrawLine(new Vector3(point.x - 1, point.y, point.z), new Vector3(point.x + 1, point.y, point.z), Color.cyan, duration);
        Debug.DrawLine(new Vector3(point.x, point.y, point.z + 1), new Vector3(point.x, point.y, point.z - 1), Color.cyan, duration);
    }

    //how much my point is out of the boundares?
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
