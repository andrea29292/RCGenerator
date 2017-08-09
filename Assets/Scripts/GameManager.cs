using System;
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
    public GameObject TrackObject;
    public float bestLap = 0f;
    public bool isPlayer1Racing;
    public bool isMultiplayer;

    public float Player1BestLap;
    public float Player2BestLap;
    public AudioSource EditorMusic;
    public AudioSource RaceMusic;
    public AudioSource BestLapSound;
    public AudioSource LapSound;
    public AudioSource TryAgainSound;
    public GameObject RaceButton;
    private float secondsCount;
    private int minuteCount;
    private int hourCount;


    Text timerText;

    // Use this for initialization
    void Start() {
        EditorMusic.Play();
        isTrack = false;
        TrackCamera = Camera.main;
        timer = false;
        firstLap = true;
        timerText = LapTimeText.GetComponent<Text>();
        bestLap = 0;
        RaceButton.GetComponent<Button>().interactable = false; ;
    }

    public void SinglePlayerRaceMode()
    {
        EditorMusic.Stop();
        TrackObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        //bestLap = 0;
        BestLapText.GetComponent<Text>().text = "" + bestLap;
        EditorCanvas.SetActive(false);
        RaceCanvas.SetActive(true);
        TrackCamera.gameObject.SetActive(false);
        SpaceShip = Instantiate(SpaceShipPrefab, new Vector3(StartPosition.x, StartPosition.y + 0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        StartSign = Instantiate(StartSignPrefab, new Vector3(StartPosition.x, StartPosition.y + 0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        ShipCamera = SpaceShip.GetComponentInChildren<Camera>();
        ShipCamera.enabled = true;
        secondsCount = 0f;
        timer = true;
        RaceMusic.Play();
    }

    public void MultiplayerRaceMode()
    {
        isPlayer1Racing = true;
        isMultiplayer = true;
        EditorMusic.Stop();
        TrackObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        BestLapText.GetComponent<Text>().text = "" + bestLap;
        EditorCanvas.SetActive(false);
        RaceCanvas.SetActive(true);
        TrackCamera.gameObject.SetActive(false);
        SpaceShip = Instantiate(SpaceShipPrefab, new Vector3(StartPosition.x, StartPosition.y + 0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        StartSign = Instantiate(StartSignPrefab, new Vector3(StartPosition.x, StartPosition.y + 0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        ShipCamera = SpaceShip.GetComponentInChildren<Camera>();
        ShipCamera.enabled = true;
        secondsCount = 0f;
        timer = true;
        RaceMusic.Play();
    }

    public void ChangePlayer()
    {

    }

    public void RaceResults()
    {

    }

    public void EditorMode()
    {
        if (RaceMusic.isPlaying)
        {
            RaceMusic.Stop();
        }
        EditorMusic.Play();
        timer = false;
        ResetRace();
        //bestLap = 0;
        BestLapText.GetComponent<Text>().text = "";
        EditorCanvas.SetActive(true);
        RaceCanvas.SetActive(false);
        Destroy(SpaceShip);
        Destroy(StartSign);
        TrackCamera.gameObject.SetActive(true);
    }



    public void SetStartPoint(Vector3 position, Vector3 rotation)
    {
        StartPosition = position;
        StartRotation = rotation;
    }

    public void ResetRace()
    {
        TryAgainSound.Play();
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
            BestLapText.GetComponent<Text>().text = "" + bestLap;
            BestLapSound.Play();
        }
     
            LapSound.Play();
        
        secondsCount = 0f;
        lap += 1;
        if(isMultiplayer && lap == 3)
        {
            if (isPlayer1Racing)
            {
                Player1BestLap = bestLap;
                ChangePlayer();
                
            }
            else
            {
                Player2BestLap = bestLap;
                RaceResults();
            }
            
        }
        LapCounterText.GetComponent<Text>().text = "Lap: "+lap;
        Debug.Log("LAP: " + lap);
    }
    public void EnableRace()
    {
        RaceButton.GetComponent<Button>().interactable = true;
    }


    public void UpdateTimerUI()
    {
        //set timer UI
        secondsCount += Time.deltaTime;
        secondsCount = Mathf.Round(secondsCount * 100f) / 100f;
        timerText.text = ""+  secondsCount;
 
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (timer)
        {
            UpdateTimerUI();
        }
	}
}
