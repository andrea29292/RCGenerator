using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipMovement : MonoBehaviour {

   float speed = 0f;
    public GameObject shipModel;
    Quaternion originalRotation;
    GameManager GameManager;
    public GameObject particleLeft;
    public GameObject particleRight;
    Rigidbody rb;
    void Start()
    {

        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        originalRotation = shipModel.transform.rotation;
        //particles.SetActive(false);
    }

    void Update()
    {
        if (transform.position.y < -5f)
        {
            GameManager.ResetRace();
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

            transform.Rotate(Vector3.up, moveHorizontal * 1.2f);

            //Debug.Log(shipModel.transform.rotation.eulerAngles);




        
        transform.Translate(Vector3.forward*(speed*0.1f));
    }
}

