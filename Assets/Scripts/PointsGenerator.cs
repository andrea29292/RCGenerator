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
    //derivated params, make private then
    public int totalPoints;
    public float maxAngleR;
    //structures
    public Vector3[] points;
    public GameObject[] pointsObject;
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
        DestroyPoints();
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

    void DestroyPoints() {
        foreach(GameObject pointObject in pointsObject) {
            Destroy(pointObject);
        }
    }
    // Update is called once per frame
    void Update() {
        curviness = curvinessSlider.value / 100;
        trackLenght = Convert.ToInt32(lenghtSlider.value);
        maxAngleD = angleSlider.value;
        PointsVisibility();
        
    }
}
