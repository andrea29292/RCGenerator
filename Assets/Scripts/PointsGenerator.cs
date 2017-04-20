using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsGenerator : MonoBehaviour {
    //constant
    int MAX_POINTS = 250;
    float MAX_LENGHT = 100;
    //params
    public GameObject pointPrefab;
    public float maxAngleD=45;  //expected in degree
    public float curviness=0.5f;
    public float segmentLen=50;
    public int trackLenght=80;   //to be converted in actual points
    //derivated params, make private then
    public int totalPoints;
    public float maxAngleR;
    //structures
    public Vector3[] points;
    public GameObject[] pointsObject;
    //utility
    System.Random random = new System.Random();
    //controls
    public Boolean showPoints;

    // Use this for initialization
    void Start() {
        GenTrack();
    }

    void GenTrack() {
        maxAngleR = (maxAngleD * Mathf.PI) / 180; //now in radians
        totalPoints = Convert.ToInt32((trackLenght * MAX_POINTS) / MAX_LENGHT);
        points = new Vector3[totalPoints];
        pointsObject = new GameObject[totalPoints];
        float direction = 0;

        points[0] = new Vector3();
        for (int i = 1; i < totalPoints; i++) {
            float varX = (float)Math.Sin(direction);
            float varZ = (float)Math.Cos(direction);
            points[i] = new Vector3(points[i - 1].x + varX, points[i - 1].y, points[i - 1].z + varZ);
            float turn = (float)Math.Pow(random.NextDouble(), 1 / curviness);
            if (random.NextDouble() > .5f) turn = -turn;
            direction += turn * maxAngleR;
        }

        int okPoints = Convert.ToInt32(totalPoints * .75);
        int  toFixPoints= totalPoints - okPoints;
        float x0 = points[0].x;
        float z0 = points[0].z;

        for (int i = okPoints; i < totalPoints; i++) {
            float x = points[i].x;
            float z = points[i].z;
            float donePoints = i - okPoints;
            points[i].x = x0 * donePoints / toFixPoints + x * (1 - donePoints / toFixPoints);
            points[i].z = z0 * donePoints / toFixPoints + z * (1 - donePoints / toFixPoints);
        }

        //create GameObject
        for (int i=0;i<totalPoints;i++) {
            pointsObject[i] =
            Instantiate(pointPrefab,
            points[i],
            Quaternion.identity) as GameObject;
            pointsObject[i].transform.parent = transform;
            pointsObject[i].active = false;
        }
    }

    void PointsVisibility(Boolean visibility) {
        foreach(GameObject pointObject in pointsObject) {
            pointObject.active = visibility;
        }
    }
    // Update is called once per frame
    void Update() {
        PointsVisibility(showPoints);
    }
}
