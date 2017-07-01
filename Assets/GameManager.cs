using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject SpaceShipPrefab;
    public GameObject SpaceShip;
    MeshRenderer ShipModel;
    Camera TrackCamera;
    Camera ShipCamera;
    Vector3 StartPosition;
    Vector3 StartRotation;
    public bool isTrack;

	// Use this for initialization
	void Start () {
        isTrack = false;
        TrackCamera = Camera.main;

	}
	
    public void RaceMode()
    {
        TrackCamera.enabled = false;
        SpaceShip = Instantiate(SpaceShipPrefab, new Vector3(StartPosition.x, StartPosition.y+0.5f, StartPosition.z), Quaternion.LookRotation(StartRotation));
        ShipCamera = SpaceShip.GetComponentInChildren<Camera>();
        ShipCamera.enabled = true;
    }

    public void EditorMode()
    {

    }

    public void SetStartPoint(Vector3 position, Vector3 rotation)
    {
        StartPosition = position;
        StartRotation = rotation;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
