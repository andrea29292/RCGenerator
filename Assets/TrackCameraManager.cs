using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCameraManager : MonoBehaviour {
    Vector3 currentCenter;
    float minFov = 40f;
    float maxFov = 60f;
    float zoomSpeed = 50f;
    public GameObject track;
    PointsGenerator pointGen;
    bool rotate;
    float restartRotate = 2.5f;
    float time;
    float rotationVelocity = 50;
    // Use this for initialization
    void Start() {
        currentCenter = track.transform.position;
        rotate = false;
        pointGen = track.GetComponent<PointsGenerator>();
        ResetCamera(50, new Vector3(0, 0, 0));
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.D)) {
            track.transform.RotateAround(currentCenter, Vector3.up, rotationVelocity * Time.deltaTime);
            rotate = false;
            time = 0;
        }
        else if (Input.GetKey(KeyCode.A)) {
            track.transform.RotateAround(currentCenter, Vector3.up, -rotationVelocity * Time.deltaTime);
            rotate = false;
            time = 0;
        }
        else if (Input.GetKey(KeyCode.W) & Vector3.Distance(transform.position, currentCenter) > 40f) {
            float step = zoomSpeed * Time.deltaTime;
            Vector3 toward = Vector3.MoveTowards(currentCenter, transform.position, step);
            track.transform.position = new Vector3(track.transform.position.x, toward.y, toward.z);
            currentCenter = toward;
        }
        else if (Input.GetKey(KeyCode.S) & Vector3.Distance(transform.position, currentCenter) < 70f) {
            float step = zoomSpeed * Time.deltaTime;
            Vector3 toward = Vector3.MoveTowards(currentCenter, transform.position, -step);
            track.transform.position = new Vector3(track.transform.position.x, toward.y, toward.z);
            currentCenter = toward;
        }

        if (rotate)
            //track.transform.RotateAround(currentCenter, Vector3.up, 10 * Time.deltaTime);

            if (!rotate) {
                time += Time.deltaTime;
                if (time >= restartRotate) rotate = true;
            }
    }

    public void ResetCamera(float lenght, Vector3 newCenter) {
        currentCenter = newCenter;
        rotate = true;
        transform.position = new Vector3(newCenter.x, 30, -80 * lenght / 100);
        transform.LookAt(track.transform);
    }
}
