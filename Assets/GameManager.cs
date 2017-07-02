using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject SpaceShipPrefab;
    GameObject SpaceShip;
    MeshRenderer ShipModel;
    Camera TrackCamera;
    Camera ShipCamera;
    Vector3 StartPosition;
    Vector3 StartRotation;
    public bool isTrack;
    public GameObject EditorCanvas;
    public GameObject RaceCanvas;

	// Use this for initialization
	void Start () {
        isTrack = false;
        TrackCamera = Camera.main;
        
	}
	
    public void RaceMode()
    {
        EditorCanvas.SetActive(false);
        RaceCanvas.SetActive(true);
        TrackCamera.enabled = false;
        SpaceShip = Instantiate(SpaceShipPrefab, new Vector3(StartPosition.x, StartPosition.y+0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        ShipCamera = SpaceShip.GetComponentInChildren<Camera>();
        ShipCamera.enabled = true;
    }

    public void EditorMode()
    {
        EditorCanvas.SetActive(true);
        RaceCanvas.SetActive(false);
        Destroy(SpaceShip);
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
    }

	// Update is called once per frame
	void Update () {
		
	}
}
