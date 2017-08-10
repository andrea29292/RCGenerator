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
    public int lap = 1;

    public GameObject WinnerCanvas;


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
    public GameObject SinglePlayerRaceButton;
    public GameObject MultiPlayerRaceButton;
    public GameObject Player2ReadyCanvas;
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
        SinglePlayerRaceButton.GetComponent<Button>().interactable = false;
        MultiPlayerRaceButton.GetComponent<Button>().interactable = false;
        Player2ReadyCanvas.SetActive(false);
    }

    public void SinglePlayerRaceMode()
    {
        EditorMusic.Stop();
        TrackObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        //bestLap = 0;
        BestLapText.GetComponent<Text>().text = "" + FloatToTime(bestLap);
        EditorCanvas.SetActive(false);
        RaceCanvas.SetActive(true);
        TrackCamera.gameObject.SetActive(false);
        SpaceShip = Instantiate(SpaceShipPrefab, new Vector3(StartPosition.x, StartPosition.y + 0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        SpaceShip.GetComponent<SpaceShipMovement>().SetBlueModel();
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
        bestLap = 0f;
        Player1BestLap = 0f;
        Player2BestLap = 0f;
        BestLapText.GetComponent<Text>().text = "" + FloatToTime(bestLap);
        EditorCanvas.SetActive(false);
        RaceCanvas.SetActive(true);
        TrackCamera.gameObject.SetActive(false);
        SpaceShip = Instantiate(SpaceShipPrefab, new Vector3(StartPosition.x, StartPosition.y + 0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        SpaceShip.GetComponent<SpaceShipMovement>().SetBlueModel();
        StartSign = Instantiate(StartSignPrefab, new Vector3(StartPosition.x, StartPosition.y + 0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        ShipCamera = SpaceShip.GetComponentInChildren<Camera>();
        ShipCamera.enabled = true;
        secondsCount = 0f;
        timer = true;
        RaceMusic.Play();
    }

    public void ChangePlayer()
    {
        Player2ReadyCanvas.SetActive(true);
        Player2ReadyCanvas.transform.GetComponentInChildren<Text>().text = "TIME TO BEAT: " + FloatToTime(Player1BestLap) + "\n WAITING FOR PLAYER 2";
        Time.timeScale = 0f;
        bestLap = 0f;
        BestLapText.GetComponent<Text>().text = ""+bestLap;
        SpaceShip.GetComponent<SpaceShipMovement>().SetRedModel();
        isPlayer1Racing = false;
        timer = false;
        ResetRace(false);
        firstLap = false;
        
    }

    

    public void EditorMode()
    {
        Time.timeScale = 1f;
        if (RaceMusic.isPlaying)
        {
            RaceMusic.Stop();
        }
        EditorMusic.Play();
        timer = false;
        ResetRace(false);
        //bestLap = 0;
       
        BestLapText.GetComponent<Text>().text = "";
        EditorCanvas.SetActive(true);
        RaceCanvas.SetActive(false);
        WinnerCanvas.SetActive(false);
        Destroy(SpaceShip);
        Destroy(StartSign);
        TrackCamera.gameObject.SetActive(true);
    }

    void RaceResults(float time1, float time2) {
        Time.timeScale = 0f;
        WinnerCanvas.SetActive(true);
        int winner;
        float winnerTime;
        Text[] texts = WinnerCanvas.transform.GetComponentsInChildren<Text>();
        //0 is winner text, 1 is winner time, 2 is looser time

        winner = time1 < time2 ? 1 : 2;
        winnerTime = time1 < time2 ? time1 : time2;

        texts[0].text = "PLAYER "+winner+", YOU WIN!";

        
        texts[Math.Abs(winner - 1)+1].text = "PLAYER " + winner +" TIME: "+ FloatToTime(winnerTime);
        if(winner == 1) {
            texts[Math.Abs(winner - 2) + 1].text = "PLAYER 2 TIME: " + FloatToTime(time2);
        }
        else
        {
            texts[Math.Abs(winner - 2) + 1].text = "PLAYER 1 TIME: " + FloatToTime(time1);
        }

    }

    String FloatToTime(float time) {
        TimeSpan t = TimeSpan.FromSeconds(time);
        string str = string.Format("{0:00}:{1:00}:{2:00}",
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
        return str;
    }

    public void SetStartPoint(Vector3 position, Vector3 rotation)
    {
        StartPosition = position;
        StartRotation = rotation;
    }

    public void ResetRace(bool falling)
    {
        if (falling)
        {
            TryAgainSound.Play();
            SpaceShip.transform.SetPositionAndRotation(new Vector3(StartPosition.x, StartPosition.y + 0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
            firstLap = true;
        }
        else
        {
            SpaceShip.transform.SetPositionAndRotation(new Vector3(StartPosition.x, StartPosition.y + 0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
            secondsCount = 0f;
            firstLap = true;
            lap = 1;
            LapCounterText.GetComponent<Text>().text = "Lap: " + lap;
        }
        timer = true;
    }

    public void AddLap()
    {
        if (secondsCount< bestLap || bestLap == 0)
        {
            bestLap = secondsCount;
            BestLapText.GetComponent<Text>().text = FloatToTime(bestLap);
            BestLapSound.Play();
        }
     
            LapSound.Play();
        
        secondsCount = 0f;
        lap += 1;
        if(isMultiplayer && lap == 4)
        {
            if (isPlayer1Racing)
            {
                Player1BestLap = bestLap;
                ChangePlayer();
                
            }
            else
            {
                Player2BestLap = bestLap;
                RaceResults(Player1BestLap,Player2BestLap);
            }
            
        }
        LapCounterText.GetComponent<Text>().text = "Lap: "+lap;
    }
    public void EnableRace()
    {
        SinglePlayerRaceButton.GetComponent<Button>().interactable = true;
        MultiPlayerRaceButton.GetComponent<Button>().interactable = true;
    }

    public void Player2Ready()
    {
        Time.timeScale = 1f;
        Player2ReadyCanvas.SetActive(false);
    }


    public void UpdateTimerUI()
    {
        //set timer UI
        secondsCount += Time.deltaTime;
        secondsCount = Mathf.Round(secondsCount * 100f) / 100f;
        timerText.text = FloatToTime(secondsCount) ;
 
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (timer)
        {
            UpdateTimerUI();
        }
	}
}
