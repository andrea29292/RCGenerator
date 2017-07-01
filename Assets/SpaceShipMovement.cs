using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipMovement : MonoBehaviour {

   float speed = 0f;

  

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetButton("Accelerate"))
        {
            speed += 0.2f;
        }else if (Input.GetButton("Brake"))
        {
            speed -= 0.2f;
        }
        else
        {
            speed -= 0.1f;
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
        
        //Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        transform.Rotate(Vector3.up, moveHorizontal*1f);
        //transform.Rotate(Vector3.forward, moveHorizontal * 0.1f);
        transform.Translate(Vector3.forward*(speed*0.1f));
    }
}

