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
    public GameObject LapCounterText;
    public GameObject BestLapText;
    public GameObject LapTimeText;
    public float bestLap = 0f;

    private float secondsCount;
    private int minuteCount;
    private int hourCount;


    Text timerText;

    // Use this for initialization
    void Start () {
        isTrack = false;
        TrackCamera = Camera.main;
        timer = false;
        firstLap = true;
        timerText = LapTimeText.GetComponent<Text>();
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
        secondsCount= 0f;
        timer = true;
    }

    public void EditorMode()
    {
        timer = false;
        ResetRace();
        bestLap = 0;
        BestLapText.GetComponent<Text>().text = "Best Lap: ";
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
        secondsCount = 0f;
        firstLap = true;
        lap = 0;
        LapCounterText.GetComponent<Text>().text = "Lap: " + lap;
    }

    public void AddLap()
    {
        if (secondsCount< bestLap || bestLap == 0)
        {
            bestLap = secondsCount;
            BestLapText.GetComponent<Text>().text = "Best Lap: " + (int)bestLap;
        }
        secondsCount = 0f;
        lap += 1;
        LapCounterText.GetComponent<Text>().text = "Lap: "+lap;
        Debug.Log("LAP: " + lap);
    }


    public void UpdateTimerUI()
    {
        //set timer UI
        secondsCount += Time.deltaTime;
        timerText.text = "Lap: "+  (int)secondsCount;
 
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (timer)
        {
            UpdateTimerUI();
        }
	}
}
