using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BezierSpline : MonoBehaviour {


    public Dictionary<int, List<Vector3>> curves = new Dictionary<int, List<Vector3>>();
    public List<GameObject> curveColliders; //to detect intersections on the track
    public int stepsPerCurve = 5;
    public GameObject colliderPrefab;
    public GameObject spherePrefab;
    [SerializeField]
    public Vector3[] points;
    public bool firstTime = true;
    public GameObject collidersObj;
    public List<GameObject> curveCol;

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
    public bool AddCurve(List<Vector3> pointsList) {

        if (firstTime) {
            points[0] = pointsList[0];
            points[1] = pointsList[1];
            points[2] = pointsList[2];
            points[3] = pointsList[3];
            firstTime = false;
        }
        else {
            Array.Resize(ref points, points.Length + 3);
            points[points.Length - 4] = pointsList[0];
            points[points.Length - 3] = pointsList[1];
            points[points.Length - 2] = pointsList[2];
            points[points.Length - 1] = pointsList[3];

        }
        curveCol = new List<GameObject>();


        Vector3 point, nextPoint, direction;
        for (int i = 0; i < 10; i++) {
            float t = (float)i / 10f;
            float nextT = (float)(i + 1) / 10f;
            point = Bezier.GetPoint(pointsList[0], pointsList[1], pointsList[2], pointsList[3], t);
            direction = Bezier.GetFirstDerivative(pointsList[0], pointsList[1], pointsList[2], pointsList[3], t);
            nextPoint = Bezier.GetPoint(pointsList[0], pointsList[1], pointsList[2], pointsList[3], nextT);
            Vector3 collPos = new Vector3(point.x, point.y, point.z);
            GameObject temp = Instantiate(colliderPrefab, collPos, Quaternion.LookRotation(direction)) as GameObject;
            temp.GetComponent<BoxCollider>().size = new Vector3(2f,2f, Vector3.Distance(point, nextPoint));

            curveCol.Add(temp);

        }
        if (CheckCollisions(curveCol, curveColliders)) {
            GameObject wrapper = new GameObject();
            foreach (GameObject col in curveCol) {
                curveColliders.Add(col);
                col.transform.parent = wrapper.transform;
            }
            wrapper.transform.parent = collidersObj.transform;
            return true;
        }
        else {
            return false;
        }
    }



    public bool CheckCollisions(List<GameObject> curveCol, List<GameObject> allCurvesCol) {

        
        for(int i=0; i<curveCol.Count-2; i++)
            for(int j = 0; j < allCurvesCol.Count-1; j++) {
                if (i == 0 && j == allCurvesCol.Count - 1) continue;
                if (curveCol[i].GetComponent<BoxCollider>().bounds.Intersects(allCurvesCol[j].GetComponent<BoxCollider>().bounds)) {
                    GameObject temp = Instantiate(spherePrefab, curveCol[i].transform.position, Quaternion.identity) as GameObject;
                    //foreach (GameObject coll in curveCol) Destroy(coll);

                    //Debug.Log("Collision beetwen new: " + i + " at "+curveCol[i].transform.position+" & old : " + j+" at "+allCurvesCol[j].transform.position);
                    //return false;
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
            77AddCurve(newPoints[i], newPoints[i + 1], newPoints[i + 2]);
            curves.Add(curveNum, new List<Vector3> { points[i-1], points[i], points[i+1], points[i+2] });
            //points[i] = newPoints[i];
        }
        Debug.Log("Punti: " + points.Length);
    }
    */
    public void GenerateCollisions() {
        /* int steps = stepsPerCurve * CurveCount;
         Vector3 point, nextPoint, direction;
         curveColliders.Clear();
         for (int i = 0; i < steps; i++)
         {
             point = GetPoint(i / (float)steps);
             direction = GetVelocity(i / (float)steps);
             nextPoint = GetPoint((i + 1) / (float)steps);
             GameObject temp = Instantiate(colliderPrefab, point, Quaternion.LookRotation(direction)) as GameObject;
             curveColliders.Add(temp); 
             curveColliders[i].GetComponent<BoxCollider>().size = new Vector3(2,2,Vector3.Distance(point, nextPoint));
             /*curveColliders[i].GetComponent<BoxCollider>().bounds.Intersects(curveColliders[j].GetComponent<BoxCollider>().bounds)*/
        // }


    }
    public void DestroyLastCurve() {
        Array.Resize(ref points, points.Length - 3);
        curveColliders.RemoveAt(curveColliders.Count - 1);
        curveColliders.RemoveAt(curveColliders.Count - 2);
        curveColliders.RemoveAt(curveColliders.Count - 3);
    }

    public void DestroyColliders() {
        foreach (Transform child in collidersObj.GetComponentInChildren<Transform>()) {
            Destroy(child.gameObject);
        }
        curveColliders.Clear();
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
    /*
    public void Update() {
        Boolean collision = false;
        for(int i=0; i< curveColliders.Count;i++)
            for(int j=0; j < curveCol.Count; j++) {
                if (curveColliders[i].GetComponent<BoxCollider>().bounds.Intersects(curveCol[j].GetComponent<BoxCollider>().bounds)) {
                    collision = true;
                }
            }
        Debug.Log("Collision: "+collision);

    }*/

}