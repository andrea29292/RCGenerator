using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public GameObject SpaceShipPrefab;
    GameObject SpaceShip;
    public GameObject StartSignPrefab;
    GameObject StartSign;
    MeshRenderer ShipModel;
    Camera TrackCamera;
    Camera ShipCamera;
    Vector3 StartPosition;
    Vector3 StartRotation;
    public bool isTrack;
    public GameObject EditorCanvas;
    public GameObject RaceCanvas;
     public float startTime;
    public int lap = 0;
    bool timer;
    public bool firstLap;
    public GameObject LapCounter;

	// Use this for initialization
	void Start () {
        isTrack = false;
        TrackCamera = Camera.main;
        timer = false;
        firstLap = true;

    }
	
    public void RaceMode()
    {
        EditorCanvas.SetActive(false);
        RaceCanvas.SetActive(true);
        TrackCamera.enabled = false;
        SpaceShip = Instantiate(SpaceShipPrefab, new Vector3(StartPosition.x, StartPosition.y+0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        StartSign = Instantiate(StartSignPrefab, new Vector3(StartPosition.x, StartPosition.y + 0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        ShipCamera = SpaceShip.GetComponentInChildren<Camera>();
        ShipCamera.enabled = true;
        startTime = 0f;
        timer = true;
    }

    public void EditorMode()
    {
        ResetRace();
        EditorCanvas.SetActive(true);
        RaceCanvas.SetActive(false);
        Destroy(SpaceShip);
        Destroy(StartSign);
        TrackCamera.enabled = true;
    }



    public void SetStartPoint(Vector3 position, Vector3 rotation)
    {
        StartPosition = position;
        StartRotation = rotation;
    }

    public void ResetRace()
    {
        SpaceShip.transform.SetPositionAndRotation(new Vector3(StartPosition.x, StartPosition.y + 0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        startTime = 0f;
        firstLap = true;
        lap = 0;
        LapCounter.GetComponent<Text>().text = "Lap: " + lap;
    }

    public void AddLap()
    {
        lap += 1;
        LapCounter.GetComponent<Text>().text = "Lap: "+lap;
        Debug.Log("LAP: " + lap);
    }

	// Update is called once per frame
	void Update () {
        if (timer)
        {
            startTime += Time.deltaTime;
        }
	}
}
