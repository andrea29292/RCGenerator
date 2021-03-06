﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipMovement : MonoBehaviour {

   float speed = 0f;
    public GameObject ShipModelBlue;
    public GameObject ShipModelRed;
    Quaternion originalRotation;
    GameManager GameManager;
    public GameObject particleLeft;
    public GameObject particleRight;
    Rigidbody rb;
    void Start()
    {

        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        originalRotation = ShipModelBlue.transform.rotation;
        //particles.SetActive(false);
    }

    void Update()
    {
        if (transform.position.y < -5f)
        {
            GameManager.ResetRace(true);
        }
        if (Input.GetButton("Accelerate"))
        {
            particleLeft.GetComponent<ParticleSystem>().Play();
            particleRight.GetComponent<ParticleSystem>().Play();
            speed += 0.2f;
        }else if (Input.GetButton("Brake"))
        {
           

            speed -= 0.2f;
        }
        else
        {
            particleLeft.GetComponent<ParticleSystem>().Stop();
            particleRight.GetComponent<ParticleSystem>().Stop();
            //particles.SetActive(false);
            speed -= 0.05f;
        }
        float moveHorizontal = Input.GetAxis("Horizontal");
        if (speed < 0)
        {
            speed = 0f;
        }
        if (speed > 1f)
        {
            speed = 1f;
        }

            transform.Rotate(Vector3.up, moveHorizontal *60f*Time.deltaTime);

            //Debug.Log(shipModel.transform.rotation.eulerAngles);




        
        transform.Translate(Vector3.forward*(speed*6f*Time.deltaTime));
    }

    public void SetBlueModel()
    {
        ShipModelBlue.SetActive(true);
        ShipModelRed.SetActive(false);
    }
    public void SetRedModel()
    {
        ShipModelBlue.SetActive(false);
        ShipModelRed.SetActive(true);
    }
}

