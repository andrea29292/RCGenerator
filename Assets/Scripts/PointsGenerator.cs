using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsGenerator : MonoBehaviour {

    //constant
    int MAX_POINTS = 200;
    float MAX_LENGHT = 100;
    //GameObject Reference
    public GameObject pointPrefab;
    public GameObject colliderPrefab;
    Toggle visibilityToggle;
    //UI params    
    Slider curvinessSlider;
    Slider angleSlider;
    Slider lenghtSlider;
    //actual params
    public float maxAngleD;  //expected in degree, maximum is 45, fixed in the slider
    public float curviness;     //from 0 to 1
    public int trackLenght;   //to be converted in actual points
    //not to be decided by the user
    public float segmentLen = 0.2f;
    public float trackWidth = 0.1f;     //Width of the colliders
    public float fieldDimension = 0.1f; //how big is the field considering the totalpoints
    public float farPointDistance;
    //derivated params, make private then
    public int totalPoints;
    public float maxAngleR;
    float maxX;
    float minX;
    float maxZ;
    float minZ;
    //structures
    public List<Vector3> points;
    public GameObject[] pointsObject;
    public GameObject[] curveColliders;
    public Dictionary<int, GameObject> numberCollider;
    public Dictionary<GameObject, int> colliderNumber;
    public List<float> directions;  //just need it to rotate the collider
    //miscelaneus
    System.Random random = new System.Random();
    Boolean pointsAreVisible;
    Vector3 farReturnPoint = new Vector3();
    Vector3 nearReturnPoint = new Vector3();
    float _direction;

    public BezierSpline spline;
    public ProceduralMesh2 mesh;



    // Use this for initialization
    void Start() {
        farPointDistance = 10 * segmentLen;
        curvinessSlider = GameObject.Find("curvinessSlider").GetComponent<Slider>();
        angleSlider = GameObject.Find("angleSlider").GetComponent<Slider>();
        lenghtSlider = GameObject.Find("lenghtSlider").GetComponent<Slider>();
        visibilityToggle = GameObject.Find("visibilityToggle").GetComponent<Toggle>();
        pointsAreVisible = visibilityToggle.isOn;
    }
    void GenTrack() {
        //while (!GenTrackDone()) ;
        GenTrackDone();
    }
    Boolean GenTrackDone() {

        DestroyTrack(); //destroy every point and collider GameObject
        maxAngleR = maxAngleD * Mathf.Deg2Rad; //now in radians
        totalPoints = Convert.ToInt32((trackLenght * MAX_POINTS) / MAX_LENGHT) / 3;
        totalPoints = (totalPoints * 3) + 1;

        maxX = transform.position.x + totalPoints * fieldDimension;
        minX = transform.position.x - totalPoints * fieldDimension;
        maxZ = transform.position.z + totalPoints * fieldDimension;
        minZ = transform.position.z - totalPoints * fieldDimension;


        //Inizializa Structures
        points = new List<Vector3>();

        directions = new List<float>();

        direction = (float)(random.NextDouble() * 360);   //start from a random direction

        points.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z));  //but on a center point

        for (int i = 1; i < totalPoints; i++) {
            float turn = (float)Math.Pow(random.NextDouble(), 1 / curviness);
            if (random.NextDouble() > .5f) turn = -turn;        //turn "left" or "right" randomly
            float direction1 = direction + turn * maxAngleD;    //the first choosed direction   
            float direction2 = direction - turn * maxAngleD;    //opposite direction, considering the turn 
            Vector3 point1 = Support.MovePoint(points[i - 1], direction1, segmentLen);
            Vector3 point2 = Support.MovePoint(points[i - 1], direction2, segmentLen);
            //if the point is outside the boundaries, or it will get there, i want to make it turn back
            //so i will check wheter direction brings the next point nearest the boundaries
            if (Support.ComputeOutness(point1, maxX, minX, maxZ, minZ) <= Support.ComputeOutness(point2, maxX, minX, maxZ, minZ)) {
                points.Add(point1);
                direction = direction1;
            }
            else {
                points.Add(point2);
                direction = direction2;
            }
            directions.Add(direction);
            if (i == 1) {
                float howFar = 3 + farPointDistance * (1 - maxAngleD / 45);
                farReturnPoint = Support.MovePoint(points[0], (float)(directions[0] - 180), howFar);
                //nearReturnPoint = MovePoint(points[0], (float)(directions[0] - (Math.PI)), 2);
            }
        }

        int k = 0;
        while (Vector3.Distance(points[points.Count - 1], farReturnPoint) > segmentLen) {
            ReachPoint(farReturnPoint);
            k++;
            if (k >= totalPoints / 2) return false;
        }
        k = 0;
        while (Vector3.Distance(points[points.Count - 1], points[0]) > segmentLen) {
            ReachPoint(points[0]);
            k++;
            if (k >= totalPoints / 2) return false;
        }
        totalPoints = points.Count;
        //check if we can make arrange the number of points
        if (RemoveSomePoints() == false) return false;

        pointsObject = new GameObject[totalPoints];
        curveColliders = new GameObject[totalPoints - 1];



        CrossingFix();
        //create GameObject
        CreateDots();
        //Vector3[] passToSpline = {points[0], points[1], points[2], points[3] };

        spline.AddPoints(points);
        mesh.CreateMesh();

        //spline.AddPoints(points);


        return true;
    }

    void ReachPoint(Vector3 g) {    //get it?

        int last = points.Count - 1;
        float curve = Support.AngleBetweenVector2(new Vector2(points[last].x, points[last].z), new Vector2(g.x, g.z), new Vector2(points[last - 1].x, points[last - 1].z));
        //Vector3 myDirection = (farReturnPoint - points[last]).normalized;
        if (Math.Abs(curve) > maxAngleD) {
            curve = maxAngleD;
        }
        float direction1 = direction + curve;    //the first choosed direction   
        float direction2 = direction - curve;    //opposite direction, considering the turn 
        Vector3 point1 = Support.MovePoint(points[last], direction1, segmentLen);
        Vector3 point2 = Support.MovePoint(points[last], direction2, segmentLen);
        if (Vector3.Distance(point1, g) < Vector3.Distance(point2, g)) {
            direction = direction1;
            points.Add(point1);
        }
        else {
            direction = direction2;
            points.Add(point2);
        }
        directions.Add(direction);

    }

    //since for draw the spline we need 3x+1 number of points, i have to delete some points to make this happen
    Boolean RemoveSomePoints() {
        int toDelete;
        int remainder = totalPoints % 3;
        if (remainder == 0) toDelete = 2;
        else if (remainder == 2) toDelete = 1;
        else toDelete = 0;
        for(int i = 0; i < toDelete; i++) {
            if (!RemovablePoint()) return false;
        }
        if (points.Count == totalPoints - toDelete) Debug.Log("TUTTO OK");
        else Debug.Log("PUNTI NON CORRETTI");
        totalPoints = points.Count;
        return true;

    }

    Boolean RemovablePoint() {
        for(int i=1; i < directions.Count-2; i++) {
            if (Math.Abs(directions[i] - directions[i + 2]) <= maxAngleD) {
                //now update directions
                float angle = Support.AngleBetweenVector2(new Vector2(points[i + 1].x, points[i + 1].z),
                                                            new Vector2(points[i + 2].x, points[i + 2].z),
                                                            new Vector2(points[i].x, points[i].z));
                float direction1 = directions[i - 1] + angle;
                float direction2 = directions[i - 1] - angle;

                Vector3 point1 = Support.MovePoint(points[i], direction1, segmentLen);
                Vector3 point2 = Support.MovePoint(points[i], direction2, segmentLen);

                if (point1.x == points[i + 2].x && point1.z == points[i + 2].z) directions[i] = direction1;
                else directions[i] = direction2;
                directions.RemoveAt(i + 1);
                points.RemoveAt(i + 1);


                return true;
            }
        }
        return false;
    }
    //this won't only find the intersect but also those condictions where two parts of the track are too much close eachother
    Boolean CrossingFix() {
        //this create the colliders
        for (int i = 0; i < totalPoints - 1; i++) {
            Vector3 a = points[i];
            Vector3 b = points[(i + 1) % totalPoints];
            Vector3 center = new Vector3((a.x + b.x) / 2, 0, (a.z + b.z) / 2);
            curveColliders[i] =
            Instantiate(colliderPrefab,
            center,
            Quaternion.identity) as GameObject;
            curveColliders[i].transform.parent = transform.GetChild(1).transform;
            //let's modify it
            float distance = Vector3.Distance(a, b);
            curveColliders[i].GetComponent<BoxCollider>().size = new Vector3(trackWidth, trackWidth, distance);
            curveColliders[i].transform.Rotate(new Vector3(0, directions[i], 0));
        }
        //now detect all collision but keep track only of one of the two collider, the one with lowest "id"
        HashSet<int> collidedSet = new HashSet<int>();
        for (int i = 0; i < curveColliders.Length; i++)
            for (int j = 0; j < curveColliders.Length; j++) {
                if (i == j || Math.Abs(i % (totalPoints - 1) - j % (totalPoints - 1)) == 1) break;
                if (curveColliders[i].GetComponent<BoxCollider>().bounds.Intersects(curveColliders[j].GetComponent<BoxCollider>().bounds)) {
                    collidedSet.Add(Math.Min(i, j));
                }
            }
        if (collidedSet.Count == 0) return false;
        //else
        List<int> collided = new List<int>(collidedSet);
        collided.Sort();

        //i want too group all near collider that has collided so i can lift them up all togheter and also lift some segment confining
        List<List<int>> collidedGroup = new List<List<int>>();

        for (int i = 0; i < collided.Count; i++) {
            int size = collidedGroup.Count;
            if (size == 0) {
                List<int> toAdd = new List<int>();
                toAdd.Add(collided[i]);
                collidedGroup.Add(toAdd);
            }
            else {
                List<int> current = collidedGroup[size - 1];
                if (collided[i] == current[current.Count - 1] + 1) {//basicly if collided[i] and the last added number are consecutive
                    current.Add(collided[i]);
                }
                else {
                    List<int> toAdd = new List<int>();
                    toAdd.Add(collided[i]);
                    collidedGroup.Add(toAdd);
                }
            }
        }

        //it can happen that the last collider and the first has collission, in that case they should form a unique group
        List<int> firstGroup = collidedGroup[0];
        List<int> lastGroup = collidedGroup[collidedGroup.Count - 1];
        if ((lastGroup[lastGroup.Count - 1]) == (totalPoints - 1) && firstGroup[0] == 0) {
            lastGroup.AddRange(firstGroup);
            collidedGroup.Remove(firstGroup);
        }
        foreach (List<int> group in collidedGroup) {
            string print = "GROUP: ";
            foreach (int coll in group) {
                print += " " + coll;
            }
            Debug.Log(print);
        }

        return true;
    }



    //create GameObject on points position
    void CreateDots() {
        for (int i = 0; i < totalPoints; i++) {
            pointsObject[i] =
            Instantiate(pointPrefab,
            points[i],
            Quaternion.identity) as GameObject;
            pointsObject[i].transform.parent = transform.GetChild(0).transform;
            pointsObject[i].SetActive(pointsAreVisible);
        }
    }

    public void PointsVisibility() {
        if (pointsAreVisible != visibilityToggle.isOn) {
            pointsAreVisible = visibilityToggle.isOn;
            foreach (GameObject pointObject in pointsObject) {
                pointObject.SetActive(pointsAreVisible);
            }
        }
    }

    void DestroyTrack() {
        foreach (GameObject pointObject in pointsObject) {
            Destroy(pointObject);
        }
        foreach (GameObject colliderObject in curveColliders) {
            Destroy(colliderObject);
        }
    }



    public float direction
    {
        get
        {
            return _direction;
        }
        set
        {
            _direction = value % 360;
        }
    }
    // Update is called once per frame
    void Update() {
        curviness = curvinessSlider.value / 100;
        trackLenght = Convert.ToInt32(lenghtSlider.value);
        maxAngleD = angleSlider.value;
        PointsVisibility();
        Debug.DrawLine(new Vector3(maxX, transform.position.y, minZ), new Vector3(maxX, transform.position.y, maxZ), Color.red, 0.1f);
        Debug.DrawLine(new Vector3(minX, transform.position.y, minZ), new Vector3(minX, transform.position.y, maxZ), Color.red, 0.1f);
        Debug.DrawLine(new Vector3(minX, transform.position.y, minZ), new Vector3(maxX, transform.position.y, minZ), Color.red, 0.1f);
        Debug.DrawLine(new Vector3(minX, transform.position.y, maxZ), new Vector3(maxX, transform.position.y, maxZ), Color.red, 0.1f);
        Debug.DrawLine(new Vector3(farReturnPoint.x - 1, farReturnPoint.y, farReturnPoint.z), new Vector3(farReturnPoint.x + 1, farReturnPoint.y, farReturnPoint.z), Color.cyan);
        Debug.DrawLine(new Vector3(farReturnPoint.x, farReturnPoint.y, farReturnPoint.z + 1), new Vector3(farReturnPoint.x, farReturnPoint.y, farReturnPoint.z - 1), Color.cyan);
        Debug.DrawLine(new Vector3(nearReturnPoint.x - 1, nearReturnPoint.y, nearReturnPoint.z), new Vector3(nearReturnPoint.x + 1, nearReturnPoint.y, nearReturnPoint.z), Color.green);
        Debug.DrawLine(new Vector3(nearReturnPoint.x, nearReturnPoint.y, nearReturnPoint.z + 1), new Vector3(nearReturnPoint.x, nearReturnPoint.y, nearReturnPoint.z - 1), Color.green);
    }
}
