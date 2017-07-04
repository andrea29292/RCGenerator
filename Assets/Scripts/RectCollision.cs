using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectCollision : MonoBehaviour {
    public List<GameObject> pointsToCheck;

	// Use this for initialization
	void Start () {
	    foreach(GameObject obj in pointsToCheck)
        {
            Debug.Log(obj.tag);
        }
        RectCheck(this.transform.position, pointsToCheck);
	}
    


    public bool RectCheck(Vector3 pointToControl, List<GameObject> pointsToCheck)
    {
        
        Vector3 r0 = new Vector3(pointToControl.x-2, pointToControl.y, pointToControl.z-2);
        Vector3 r1 = new Vector3(pointToControl.x-2, pointToControl.y, pointToControl.z+2);
        Vector3 r2= new Vector3(pointToControl.x+2, pointToControl.y, pointToControl.z-2);
        Vector3 r3 = new Vector3(pointToControl.x+2, pointToControl.y, pointToControl.z+2);
        Debug.DrawLine(r0, r1);
        Debug.DrawLine(r2, r3);
        Debug.DrawLine(r1, r3);
        Debug.DrawLine(r0, r2);
        
        foreach(GameObject obj in pointsToCheck)
        {
           bool res =  Math3d.IsPointInRectangle(obj.transform.position, r0,r1,r2,r3);
            Debug.Log(res);
        }
        
        return true;
    }
    
	// Update is called once per frame
	void Update () {
      // RectCheck(Vector3.zero, pointsToCheck);
	}
}
