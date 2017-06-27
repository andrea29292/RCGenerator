using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BezierSpline : MonoBehaviour {

    //intersection control structure
    public List<Vector3> intersectionPoints = new List<Vector3>();
    public GameObject pointPrefab;  //to see controlPoints
    int controlRes = 4;
    public Dictionary<Vector3, Vector3> pointsAndDirections = new Dictionary<Vector3, Vector3>();
    public Dictionary<int, List<Vector3>> curves = new Dictionary<int, List<Vector3>>();
    public int stepsPerCurve = 5;
    public GameObject colliderPrefab;
    public GameObject spherePrefab;
    [SerializeField]
    public Vector3[] points;
    public bool firstTime = true;

    List<float> distances;
    [SerializeField]
    private BezierControlPointMode[] modes;

    [SerializeField]
    private bool loop;

    public bool Loop
    {
        get
        {
            return loop;
        }
        set
        {
            loop = value;
            if (value == true) {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }

    public int ControlPointCount
    {
        get
        {
            return points.Length;
        }
    }

    public Vector3 GetControlPoint(int index) {
        return points[index];
    }

    public void SetControlPoint(int index, Vector3 point) {
        if (index % 3 == 0) {
            Vector3 delta = point - points[index];
            if (loop) {
                if (index == 0) {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if (index == points.Length - 1) {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else {
                if (index > 0) {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length) {
                    points[index + 1] += delta;
                }
            }
        }
        points[index] = point;
        EnforceMode(index % 4);
    }


    public BezierControlPointMode GetControlPointMode(int index) {
        return modes[(int)((index + 1) / 3)];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode) {
        int modeIndex = (int)((index + 1) / 3);
        modes[modeIndex] = mode;
        if (loop) {
            if (modeIndex == 0) {
                modes[modes.Length - 1] = mode;
            }
            else if (modeIndex == modes.Length - 1) {
                modes[0] = mode;
            }
        }
        EnforceMode(index);
    }

    private void EnforceMode(int index) {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex) {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0) {
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= points.Length) {
                enforcedIndex = 1;
            }
        }
        else {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= points.Length) {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0) {
                enforcedIndex = points.Length - 2;
            }
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned) {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }

    public int CurveCount
    {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    public Vector3 GetPoint(float t) {
        int i;
        if (t >= 1f) {
            t = 1f;
            i = points.Length - 4;
        }
        else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    public Vector3 GetVelocity(float t) {
        int i;
        if (t >= 1f) {
            t = 1f;
            i = points.Length - 4;
        }
        else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t) {
        return GetVelocity(t).normalized;
    }
    public bool AddCurve(List<Vector3> pointsList, bool lastCurve) {

     


        pointsAndDirections.Clear();
        List<Vector3> newPoints = new List<Vector3>();  //needs to be checked before adding them to intersectionPoints
        for (int i = 0; i < controlRes; i++) {
            float t = (float)i / controlRes;
            Vector3 point = Bezier.GetPoint(pointsList[0], pointsList[1], pointsList[2], pointsList[3], t);
            Vector3 dir = Bezier.GetFirstDerivative(pointsList[0], pointsList[1], pointsList[2], pointsList[3], t);
            newPoints.Add(point);
            pointsAndDirections.Add(point, dir);
            
            //GameObject temp = Instantiate(pointPrefab, point, Quaternion.identity) as GameObject;
        }
        if (CheckCollisions(newPoints, intersectionPoints, lastCurve)) {

            if (firstTime)
            {
                points[0] = pointsList[0];
                points[1] = pointsList[1];
                points[2] = pointsList[2];
                points[3] = pointsList[3];
                firstTime = false;
            }
            else
            {
                Array.Resize(ref points, points.Length + 3);
                points[points.Length - 4] = pointsList[0];
                points[points.Length - 3] = pointsList[1];
                points[points.Length - 2] = pointsList[2];
                points[points.Length - 1] = pointsList[3];

            }

            foreach (Vector3 newPoint in newPoints) intersectionPoints.Add(newPoint);

            return true;
        }
        else {
            return false;
        }
    }



    public bool CheckCollisions(List<Vector3> newPoints, List<Vector3> stablePoints, bool lastCurve) {


        return CheckPlaneIntersect(newPoints, stablePoints);



    }

    //foreach new point, check if the segments build on i and i+1 intersect with any other old points
    bool CheckIntersect(List<Vector3> newPoints, List<Vector3> oldPoints, bool lastCurve) {
        for (int i = 0; i < newPoints.Count - 2; i++)
            for (int j = 0; j < oldPoints.Count - 3; j++) { //skip the last segment
                if (lastCurve && j == 0) continue; 
                bool intersect = Math3d.AreLineSegmentsCrossing(newPoints[i], newPoints[i + 1], oldPoints[j], oldPoints[j + 1]);
                if (intersect) {
                    Debug.Log("new " + i + " stable " + j);
                    return false;
                }
            }
        return true;
    }
    bool CheckPlaneIntersect(List<Vector3> newPoints, List<Vector3> oldPoints)
    {
        List<GameObject> colList = new List<GameObject>();
        foreach (Vector3 pt in newPoints)
        {
            Vector3 dir = pointsAndDirections[pt];
            GameObject col = Instantiate(colliderPrefab, new Vector3(pt.x,pt.y+0.01f,pt.z), Quaternion.LookRotation(dir)) as GameObject;
            colList.Add(col);
            Vector3[] verts = GetVertices(col);
            
        
            Vector3 r0 = verts[4];
            Vector3 r1 = verts[5];
            Vector3 r2 = verts[6];
            Vector3 r3 = verts[7];
            
            /*
            GameObject temp = Instantiate(pointPrefab, r0, Quaternion.identity) as GameObject;
            GameObject temp1 = Instantiate(pointPrefab, r1, Quaternion.identity) as GameObject;
            GameObject temp2 = Instantiate(pointPrefab, r2, Quaternion.identity) as GameObject;
            GameObject temp3 = Instantiate(pointPrefab, r3, Quaternion.identity) as GameObject;
            */
            
            foreach (Vector3 oldPt in oldPoints)
            {
                if(Math3d.IsPointInRectangle(oldPt, r0, r1, r2, r3))
                {
                    Debug.Log("Intersezione: " + pt + " con: " + oldPt);
                    foreach(GameObject colObj in colList)
                    {
                        Destroy(col);
                    }
                    colList.Clear();
                    return false;
                }
            }
        }
        return true;
    }


    public void AddCurve() {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop) {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }
    /*
    public void AddPoints(List<Vector3> newPoints)
    {
        int curveNum = 0;
        Array.Resize(ref points, 4);
        points[0] = newPoints[0];
        points[1] = newPoints[1];
        points[2] = newPoints[2];
        points[3] = newPoints[3];
        curves.Add(curveNum, new List<Vector3> { points[0], points[1], points[2], points[3] });
        for (int i = 4; i < newPoints.Count; i+=3)
        {
            curveNum += 1;
            AddCurve(newPoints[i], newPoints[i + 1], newPoints[i + 2]);
            curves.Add(curveNum, new List<Vector3> { points[i-1], points[i], points[i+1], points[i+2] });
            //points[i] = newPoints[i];
        }
        Debug.Log("Punti: " + points.Length);
    }
    */

    public void DestroyLastCurve() {
        Array.Resize(ref points, points.Length - 3);
        Debug.Log("control points before " + intersectionPoints.Count);
        intersectionPoints.RemoveRange(intersectionPoints.Count - controlRes, controlRes);
        Debug.Log("control points after " + intersectionPoints.Count);
    }


    //if the track cannot be done, delete all intersectionPoints cause we need to check again for intersection
    public void ClearIntersectionPoints() {
        intersectionPoints.Clear();
    }

    Vector3[] GetVertices(GameObject obj1)
    {

        Vector3[] vertices = new Vector3[8];
        Matrix4x4 thisMatrix = obj1.transform.localToWorldMatrix;
        Quaternion storedRotation = obj1.transform.rotation;
        obj1.transform.rotation = Quaternion.identity;

        Vector3 extents = obj1.GetComponent<BoxCollider>().bounds.extents;
        vertices[0] = thisMatrix.MultiplyPoint3x4(extents);
        //GameObject pointObject = Instantiate(pointPrefab, vertices[0], Quaternion.identity) as GameObject;
        vertices[1] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, extents.z));
        //GameObject pointObject2 = Instantiate(pointPrefab, vertices[1], Quaternion.identity) as GameObject;

        vertices[2] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, extents.y, -extents.z));
        //GameObject pointObject3 = Instantiate(pointPrefab, vertices[2], Quaternion.identity) as GameObject;

        vertices[3] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, extents.y, -extents.z));
        //GameObject pointObject4 = Instantiate(pointPrefab, vertices[3], Quaternion.identity) as GameObject;
        vertices[4] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, extents.z));
        vertices[5] = thisMatrix.MultiplyPoint3x4(new Vector3(-extents.x, -extents.y, extents.z));
        vertices[6] = thisMatrix.MultiplyPoint3x4(new Vector3(extents.x, -extents.y, -extents.z));
        vertices[7] = thisMatrix.MultiplyPoint3x4(-extents);

        obj1.transform.rotation = storedRotation;
        return vertices;
    }






    public void Reset() {
        points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
        modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    public void Update() {

    }

}