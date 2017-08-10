using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsGenerator : MonoBehaviour {

    //constant
    float RAISE = 0.9f;
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
    public float fieldDimension = 0.2f; //how big is the field considering the totalpoints
    float farPointDistance = 15f;      //how far is the far point to reach considering the max angle
    //derivated params, make private then
    public int totalPoints;
    //define the boundaries of the track
    float maxX;
    float minX;
    float maxZ;
    float minZ;
    //structures
    public List<List<Vector3>> curvePoints; //control point of our spline
    public List<GameObject> pointsObjects;   //collections of game objects corrisponding to points
    public List<GameObject> curveObjects;    //collections of game objects corrisponding to bezier curve
    public GameObject[] curveColliders; //to detect intersections on the track
    public List<float> directions;  //just need it to rotate the collider
    //miscelaneus
    Boolean go = true;
    int fromHereReach;
    int level = 0;
    System.Random random = new System.Random();
    Boolean pointsAreVisible;
    Vector3 farReturnPoint = new Vector3(); //used this to avoid peak on the track
    float _direction;               //see property
    Boolean _lastCurve;

    public BezierSpline spline;
    public ProceduralMesh2 mesh;
    public GameObject meshObject;
    

    GameObject GameManager;



    // Use this for initialization
    void Start() {
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
        farPointDistance = farPointDistance * segmentLen;
        curvinessSlider = GameObject.Find("curvinessSlider").GetComponent<Slider>();
        angleSlider = GameObject.Find("angleSlider").GetComponent<Slider>();
        lenghtSlider = GameObject.Find("lenghtSlider").GetComponent<Slider>();
        //visibilityToggle = GameObject.Find("visibilityToggle").GetComponent<Toggle>();
        //pointsAreVisible = visibilityToggle.isOn;
    }
    //TODO: try GenTrackDone for x times, than alert the users to try with other params
    public void GenTrack() {
        GameManager.GetComponent<GameManager>().bestLap = 0f;
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;
        int attempt = 0;
        bool res = false;
        while (true) {
            attempt++;
            if (attempt >= 10 & res == false) {

                break;
            };
            res = GenTrackDone();
            if (res) break;
        }
        if (res) {
            float maxX = Mathf.NegativeInfinity;
            float minX = Mathf.Infinity;
            float maxZ = Mathf.NegativeInfinity;
            float minZ = Mathf.Infinity;
            for (int i = 0; i < curvePoints.Count; i++)
                for (int j = 0; j < 3; j++) {
                    Vector3 current = curvePoints[i][j];
                    if (current.x > maxX) maxX = current.x;
                    if (current.x < minX) minX = current.x;
                    if (current.z > maxZ) maxZ = current.z;
                    if (current.z < minZ) minZ = current.z;
                }
            Vector3 newCenter = new Vector3((maxX + minX) / 2, 0, (maxZ + minZ) / 2);
            Debug.DrawLine(new Vector3(maxX, 0, maxZ), new Vector3(minX, 0, maxZ), Color.cyan,20);
            Debug.DrawLine(new Vector3(maxX, 0, minZ), new Vector3(minX, 0, minZ), Color.cyan, 20);
            Debug.DrawLine(new Vector3(maxX, 0, minZ), new Vector3(maxX, 0, maxZ), Color.cyan, 20);
            Debug.DrawLine(new Vector3(minX, 0, minZ), new Vector3(minX, 0, maxZ), Color.cyan, 20);

            //GameObject pointObject = Instantiate(pointPrefab,
              //  newCenter,
                //Quaternion.identity) as GameObject;
            Camera.main.GetComponent<TrackCameraManager>().ResetCamera(trackLenght, newCenter);
            
        }
        attempt = 0;

    }

    //select a "random" point, also add the direction
    Vector3 randomPoint(Vector3 prevPoint, float prevDirection) {
        float turn = (float)Math.Pow(random.NextDouble(), 1 / curviness);   //how much the next point will turn?
        if (random.NextDouble() > .5f) turn = -turn;        //turn "left" or "right" randomly
        float nextDir = direction + turn * maxAngleD;    //the first choosed direction    
        Vector3 nextPoint = Support.MovePoint(prevPoint, nextDir, segmentLen);
        float outness = Support.ComputeOutness(nextPoint, maxX, minX, maxZ, minZ);  //how much that point is out of the boundaries
        if (outness != 0) {
            float direction2 = direction - turn * maxAngleD;    //opposite turn
            Vector3 point2 = Support.MovePoint(prevPoint, direction2, segmentLen);
            if (Support.ComputeOutness(point2, maxX, minX, maxZ, minZ) < outness) {    //pick the best turn
                nextDir = direction2;
                nextPoint = point2;
            }
        }
        direction = nextDir;
        directions.Add(direction);
        return nextPoint;
    }

    Boolean GenTrackDone() {
        if (!meshObject.activeSelf)
        {
            meshObject.SetActive(true);
        }
        level = 0;
        spline.Reset();
        spline.firstTime = true;
        _lastCurve = false;

        DestroyTrack(); //destroy every point and collider GameObject
        totalPoints = Convert.ToInt32((trackLenght * MAX_POINTS) / MAX_LENGHT) / 3;
        totalPoints = totalPoints * 3;

        //define boundaries
        maxX = transform.position.x + totalPoints * fieldDimension;
        minX = transform.position.x - totalPoints * fieldDimension;
        maxZ = transform.position.z + totalPoints * fieldDimension;
        minZ = transform.position.z - totalPoints * fieldDimension;


        //Initialize Structures
        curvePoints = new List<List<Vector3>>();
        directions = new List<float>();

        direction = (float)(random.NextDouble() * 360);   //start from a random direction
        Vector3 startPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);  //but on a center point
        float howFar = farPointDistance + farPointDistance * (1 - maxAngleD / 45);
        farReturnPoint = Support.MovePoint(startPoint, (float)(direction - 180), howFar);
        //GameObject pointObject = Instantiate(pointPrefab,
          //      farReturnPoint,
            //    Quaternion.identity) as GameObject;

        for (int i = 0; i < totalPoints / 3; i++) { //foreach curve
            List<Vector3> newCurvePoint = new List<Vector3>();
            Vector3 secPoint;
            if (i == 0) {
                newCurvePoint.Add(startPoint);
                secPoint = randomPoint(startPoint, direction);
            }
            else {
                newCurvePoint.Add(curvePoints[curvePoints.Count - 1][3]);   //add the last point of the previous curve as my first
                secPoint = Support.MovePoint(newCurvePoint[0], direction, segmentLen);  //go "random"
                directions.Add(direction);
            }

            if (level != 0) {
                secPoint = new Vector3(secPoint.x, RAISE * level, secPoint.z);
                level = 0;
            }

            Vector3 trdpoint = randomPoint(secPoint, direction);
            Vector3 frtPoint = randomPoint(trdpoint, direction);

            newCurvePoint.Add(secPoint);
            newCurvePoint.Add(trdpoint);
            newCurvePoint.Add(frtPoint);

            if (!correctSpline(newCurvePoint, false)) return false;
            curvePoints.Add(newCurvePoint);
            //buildSpline(newCurvePoint);


        }
        fromHereReach = curvePoints.Count;
        //head for the far return point, that is placed at the right opposite direction of the first point
        int k = 0;
        Vector3 lastPoint = curvePoints[curvePoints.Count - 1][3];
        while (Vector3.Distance(lastPoint, farReturnPoint) > segmentLen * 4) {
            List<Vector3> newCurve = reachPointCurve(ref lastPoint);
            if (level != 0) {
                newCurve[1] = new Vector3(newCurve[1].x, RAISE * level, newCurve[1].z);
                level = 0;
            }

            if (!correctSpline(newCurve, false)) return false;
            curvePoints.Add(newCurve);
            k++;
            if (k >= totalPoints / 2) {  return false; }
        }


        //now head for the initial point
        k = 0;
        while (lastPoint != startPoint) {

            List<Vector3> newCurve = reachFirstPoint(ref lastPoint, startPoint);

            if (level != 0) {
                newCurve[1] = new Vector3(newCurve[1].x, RAISE * level, newCurve[1].z);
                level = 0;
            }
            if (!correctSpline(newCurve, _lastCurve)) return false;
            curvePoints.Add(newCurve);
            k++;
            if (k >= totalPoints / 2) return false;
        }


        pointsObjects = new List<GameObject>();



        //CreateDots();
        mesh.CreateMesh();
        spline.curves = new Dictionary<int, List<Vector3>>();

        
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ControlMesh")) {
            Destroy(obj);
        }
        List<Vector3> firstCurve = curvePoints[0];
        Vector3 pos = Bezier.GetPoint(firstCurve[0], firstCurve[1], firstCurve[2], firstCurve[3], 0);
        Vector3 rot1 = Bezier.GetFirstDerivative(firstCurve[0], firstCurve[1], firstCurve[2], firstCurve[3], 0);
        Vector3 rot2 = Bezier.GetFirstDerivative(firstCurve[0], firstCurve[1], firstCurve[2], firstCurve[3], 0.5f);
        
        GameManager.GetComponent<GameManager>().SetStartPoint(pos, rot1);
        GameManager.GetComponent<GameManager>().isTrack = true;
        GameManager.GetComponent<GameManager>().EnableRace();
        return true;
    }

    Boolean correctSpline(List<Vector3> newCurve, bool lastCurve) {
        if (buildSpline(newCurve, lastCurve, null)) return true;

        List<Vector3> prevCurve = curvePoints[curvePoints.Count - 1];
        level = 1;
        raiseLowerCurve(newCurve, prevCurve, level);
        if (lastCurve) raiseLowerFirstCurve(curvePoints[0], level);
        if (buildSpline(newCurve, prevCurve, lastCurve, curvePoints[0])) return true;

        level = -1;
        raiseLowerCurve(newCurve, prevCurve, level);
        if (lastCurve) raiseLowerFirstCurve(curvePoints[0], level);
        if (buildSpline(newCurve, prevCurve, lastCurve, curvePoints[0])) return true;

        spline.ClearIntersectionPoints();
        GameManager.GetComponent<GameManager>().isTrack = false;
        return false;

    }

    void raiseLowerCurve(List<Vector3> newCurve, List<Vector3> prevCurve, int level) {
        for (int i = 0; i < 4; i++) {
            newCurve[i] = new Vector3(newCurve[i].x, RAISE * level, newCurve[i].z);
        }
        //we also need to raise/lower the last two points of the previous curve
        prevCurve[2] = new Vector3(prevCurve[2].x, RAISE * level, prevCurve[2].z);
        prevCurve[3] = new Vector3(prevCurve[3].x, RAISE * level, prevCurve[3].z);

    }

    void raiseLowerFirstCurve(List<Vector3> firstCurve, int level) {
        firstCurve[0] = new Vector3(firstCurve[0].x, RAISE * level, firstCurve[0].z);
        firstCurve[1] = new Vector3(firstCurve[1].x, RAISE * level, firstCurve[1].z);
        curvePoints[0] = firstCurve;
    }

    //
    List<Vector3> reachFirstPoint(ref Vector3 lastPoint, Vector3 startPoint) {
        List<Vector3> newCurvePoint = new List<Vector3>();
        newCurvePoint.Add(lastPoint);
        Vector3 secPoint = Support.MovePoint(lastPoint, direction, segmentLen);
        directions.Add(direction);
        Vector3 anchor = Support.MovePoint(secPoint, direction, segmentLen);
        Vector3 trdPoint = ReachPoint(anchor, startPoint, secPoint);
        Vector3 frtPoint;
        if (Vector3.Distance(trdPoint, startPoint) < segmentLen * 4) {
            _lastCurve = true;
            frtPoint = startPoint;
            trdPoint = Support.MovePoint(startPoint, directions[0] + 180, segmentLen);
        }
        else {
            anchor = Support.MovePoint(trdPoint, direction, segmentLen);
            frtPoint = ReachPoint(anchor, startPoint, trdPoint);
        }
        newCurvePoint.Add(secPoint);
        newCurvePoint.Add(trdPoint);
        newCurvePoint.Add(frtPoint);
        lastPoint = frtPoint;
        return newCurvePoint;
    }

    List<Vector3> reachPointCurve(ref Vector3 lastPoint) {
        List<Vector3> newCurvePoint = new List<Vector3>();
        newCurvePoint.Add(lastPoint);
        Vector3 secPoint = Support.MovePoint(lastPoint, direction, segmentLen);
        directions.Add(direction);
        Vector3 anchor = Support.MovePoint(secPoint, direction, segmentLen);
        Vector3 trdPoint = ReachPoint(anchor, farReturnPoint, secPoint);
        anchor = Support.MovePoint(trdPoint, direction, segmentLen);
        Vector3 frtPoint = ReachPoint(anchor, farReturnPoint, trdPoint);
        newCurvePoint.Add(secPoint);
        newCurvePoint.Add(trdPoint);
        newCurvePoint.Add(frtPoint);
        lastPoint = frtPoint;
        return newCurvePoint;
    }


    Boolean buildSpline(List<Vector3> points, bool lastCurve, List<Vector3> firstCurve) {

        return spline.AddCurve(points, lastCurve);

    }
    Boolean buildSpline(List<Vector3> points, List<Vector3> prevPoints, bool lastCurve, List<Vector3> firstCurve) {
        spline.DestroyLastCurve();
        //TODO
        if (lastCurve) {
            spline.CorrectFirstCurve(firstCurve);
        }

        return spline.AddCurve(prevPoints, false) && spline.AddCurve(points, lastCurve);

    }
    //generate a new point toward the given point
    Vector3 ReachPoint(Vector3 anchor, Vector3 target, Vector3 center) {

        //this will return an angle, but we cannot know if that angle is to add or subtract from the direction...
        float curve = Support.AngleBetweenVector2(new Vector2(anchor.x, anchor.z), new Vector2(target.x, target.z), new Vector2(center.x, center.z));
        if (Math.Abs(curve) > maxAngleD) {
            curve = maxAngleD;
        }
        //...so we test both...
        float direction1 = direction + curve;    //the first choosed direction   
        float direction2 = direction - curve;    //opposite direction, considering the turn 
        Vector3 point1 = Support.MovePoint(center, direction1, segmentLen);
        Vector3 point2 = Support.MovePoint(center, direction2, segmentLen);
        ///..and we see wich one is the correct one (nearest to the point we want to reach)
        if (Vector3.Distance(point1, target) < Vector3.Distance(point2, target)) {
            direction = direction1;
            //points.Add(point1);
            directions.Add(direction);

            return point1;
        }
        else {
            direction = direction2;
            directions.Add(direction);

            //points.Add(point2);
            return point2;

        }
    }


    //create GameObject on points position
    void CreateDots() {
        for (int i = 0; i < curvePoints.Count; i++) {
            GameObject bezier = new GameObject();
            for (int j = 0; j < 4; j++) {
                GameObject pointObject = Instantiate(pointPrefab,
                curvePoints[i][j],
                Quaternion.identity) as GameObject;
                if (i >= fromHereReach) {
                    pointObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
                }
                pointsObjects.Add(pointObject);
                pointObject.transform.parent = bezier.transform;
                pointObject.SetActive(pointsAreVisible);

            }
            bezier.transform.parent = transform.GetChild(0).transform;
            curveObjects.Add(bezier);

        }
    }



    /*
    public void PointsVisibility() {
        if (pointsAreVisible != visibilityToggle.isOn) {
            pointsAreVisible = visibilityToggle.isOn;
            foreach (GameObject pointObject in pointsObjects) {
                pointObject.SetActive(pointsAreVisible);
            }
        }
    }
    */

    void DestroyTrack() {
        foreach (GameObject pointObject in pointsObjects) {
            Destroy(pointObject);
        }

        foreach (GameObject curveObject in curveObjects) Destroy(curveObject);

        foreach (GameObject col in GameObject.FindGameObjectsWithTag("ControlMesh")) {
            Destroy(col);

        }
        spline.ClearIntersectionPoints();
    }


    //sin and cos works fine even with x>2PI or 359, but we like to have angle from 0 to 359 
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
        if (curvePoints != null)
            if (curvePoints[curvePoints.Count - 1] != null) Debug.Log("index : " + (curvePoints.Count - 1) + " position: " + curvePoints[curvePoints.Count - 1][0]);
        curviness = curvinessSlider.value / 100;
        trackLenght = Convert.ToInt32(lenghtSlider.value);
        maxAngleD = angleSlider.value;
        //PointsVisibility();
        Debug.DrawLine(new Vector3(maxX, transform.position.y, minZ), new Vector3(maxX, transform.position.y, maxZ), Color.red, 0.1f);
        Debug.DrawLine(new Vector3(minX, transform.position.y, minZ), new Vector3(minX, transform.position.y, maxZ), Color.red, 0.1f);
        Debug.DrawLine(new Vector3(minX, transform.position.y, minZ), new Vector3(maxX, transform.position.y, minZ), Color.red, 0.1f);
        Debug.DrawLine(new Vector3(minX, transform.position.y, maxZ), new Vector3(maxX, transform.position.y, maxZ), Color.red, 0.1f);
        Support.ShowPoint(farReturnPoint);
    }
}
