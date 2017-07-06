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
        else if (Input.GetKey(KeyCode.W)) {
            float step = zoomSpeed * Time.deltaTime;
            if (Vector3.Distance(track.transform.position, transform.position) > 40f)
                track.transform.position = Vector3.MoveTowards(track.transform.position, transform.position, step);
        }
        else if (Input.GetKey(KeyCode.S)) {
            float step = zoomSpeed * Time.deltaTime;
            if (Vector3.Distance(track.transform.position, transform.position) <70f)
                track.transform.position = Vector3.MoveTowards(track.transform.position, transform.position, -step);
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
