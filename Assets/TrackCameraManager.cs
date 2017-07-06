using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCameraManager : MonoBehaviour {
    float minFov = 15f;
    float maxFov = 90f;
    float sensitivity = 1f;
    public GameObject track;
    PointsGenerator pointGen;
    bool rotate;
    float restartRotate = 2.5f;
    float time;
    float rotationVelocity = 50;
    // Use this for initialization
    void Start() {
        rotate = false;
        pointGen = track.GetComponent<PointsGenerator>();
        ResetCamera(50);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.D)) {
            track.transform.Rotate(new Vector3(0, rotationVelocity * Time.deltaTime));
            rotate = false;
            time = 0;
        }
        else if (Input.GetKey(KeyCode.A)) {
            track.transform.Rotate(new Vector3(0, -rotationVelocity * Time.deltaTime));
            rotate = false;
            time = 0;
        }
        else if (Input.GetKey(KeyCode.W)) {
            float fov = Camera.main.fieldOfView;
            fov += -1f * sensitivity;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            Camera.main.fieldOfView = fov;
        }
        else if (Input.GetKey(KeyCode.S)) {
            float fov = Camera.main.fieldOfView;
            fov += 1f * sensitivity;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            Camera.main.fieldOfView = fov;
        }

        if (rotate)
            track.transform.Rotate(new Vector3(0, 5 * Time.deltaTime, 0));
        if (!rotate) {
            time += Time.deltaTime;
            if (time >= restartRotate) rotate = true;
        }
    }

    public void ResetCamera(float lenght) {
        rotate = true;
        transform.position = new Vector3(track.transform.position.x, 20, -50 * lenght / 100);
        transform.LookAt(track.transform);
    }
}
