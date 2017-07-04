using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSignScript : MonoBehaviour {

    GameManager GM;
	// Use this for initialization
	void Start () {

        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("SpaceShip"))
        {
            if (!GM.firstLap)
            {
                GM.AddLap();
            }
            else
            {
                GM.firstLap = false;

            }
            
        }
    }
}
