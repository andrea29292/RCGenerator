using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsGenerator : MonoBehaviour {
    //constant
    int MAX_POINTS = 250;
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
    public float segmentLen = 50; //not to be decided by the user
    public float trackWidth = 0.1f;
    public float fieldDimension = 0.1f;
    //derivated params, make private then
    public int totalPoints;
    public float maxAngleR;
    float maxX;
    float minX;
    float maxZ;
    float minZ;
    //structures
    public Vector3[] points;
    public GameObject[] pointsObject;
    public GameObject[] curveColliders;
    public float[] directions;  //just need it to rotate the collider
    //utility
    System.Random random = new System.Random();
    Boolean pointsAreVisible;


    // Use this for initialization
    void Start() {
        curvinessSlider = GameObject.Find("curvinessSlider").GetComponent<Slider>();
        angleSlider = GameObject.Find("angleSlider").GetComponent<Slider>();
        lenghtSlider = GameObject.Find("lenghtSlider").GetComponent<Slider>();
        visibilityToggle = GameObject.Find("visibilityToggle").GetComponent<Toggle>();
        pointsAreVisible = visibilityToggle.isOn;
    }

    public void GenTrack() {
        DestroyTrack(); //destroy every point and collider GameObject
        maxAngleR = maxAngleD * Mathf.Deg2Rad; //now in radians
        totalPoints = Convert.ToInt32((trackLenght * MAX_POINTS) / MAX_LENGHT);

        maxX = transform.position.x + totalPoints * fieldDimension;
        minX = transform.position.x - totalPoints * fieldDimension;
        maxZ = transform.position.z + totalPoints * fieldDimension;
        minZ = transform.position.z - totalPoints * fieldDimension;


        //Inizializa Structures
        points = new Vector3[totalPoints];
        pointsObject = new GameObject[totalPoints];
        curveColliders = new GameObject[totalPoints];
        directions = new float[totalPoints];

        float direction = (float)(random.NextDouble() * 2 * Math.PI);   //start from a random direction

        points[0] = new Vector3(transform.position.x, transform.position.y, transform.position.z);  //but on a center point
                                                                                                    //considering to make this also random. We'll see!

        for (int i = 1; i < totalPoints; i++) {
            float turn = (float)Math.Pow(random.NextDouble(), 1 / curviness);
            if (random.NextDouble() > .5f) turn = -turn;
            float direction1 = direction + turn * maxAngleR;    //the first choosed direction   
            float direction2 = direction - turn * maxAngleR;    //opposite direction, considering the turn 
            Vector3 point1 = MovePoint(points[i - 1], direction1);
            Vector3 point2 = MovePoint(points[i - 1], direction2);

            if(ComputeOutness(point1) <= ComputeOutness(point2)) {
                points[i] = point1;
                direction = direction1;
            }
            else {
                points[i] = point2;
                direction = direction2;
            }

            directions[i - 1] = direction * Mathf.Rad2Deg;

        }

        int okPoints = Convert.ToInt32(totalPoints * .75);
        int toFixPoints = totalPoints - okPoints;
        float x0 = points[0].x;
        float z0 = points[0].z;

        //in the last quarter of the track force to return to origin
        for (int i = okPoints; i < totalPoints; i++) {
            float x = points[i].x;
            float z = points[i].z;
            float donePoints = i - okPoints;
            points[i].x = x0 * donePoints / toFixPoints + x * (1 - donePoints / toFixPoints);
            points[i].z = z0 * donePoints / toFixPoints + z * (1 - donePoints / toFixPoints);
        }


        FindIntersect();
        //create GameObject
        CreateDots();

    }

    Vector3 MovePoint(Vector3 point, float direction) {
        float varX = (float)Math.Sin(direction);
        float varZ = (float)Math.Cos(direction);
        return new Vector3(point.x + varX, point.y, point.z + varZ);
    }

    float ComputeOutness(Vector3 point) {
        float outness = 0f;
        if (point.x > maxX) {
            Debug.Log("Stai fori!");
            outness = Math.Abs(point.x - maxX);
        }
        if (point.x < minX) {
            Debug.Log("Stai fori!");

            outness = Math.Abs(point.x - minX);
        }
        if (point.z < minZ) {
            Debug.Log("Stai fori!");

            outness = Math.Abs(point.z - minZ);
        }
        if (point.z > maxZ) {
            Debug.Log("Stai fori!");

            outness = Math.Abs(point.z - maxZ);
        }
        return outness;
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

    void FindIntersect() {
        for (int i = 0; i < totalPoints; i++) {
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
            Debug.Log(directions[i]);

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

    }
}
