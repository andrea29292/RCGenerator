using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {
    public  Vector2 center = new Vector2();
    public Vector2 v1 = new Vector3(-1, -1);
    public Vector2 v2 = new Vector3(0, 1);
    // Use this for initialization
    void Start () {
        
    }
    float AngleBetweenVector2(Vector2 vec1, Vector2 vec2, Vector2 center) {
        vec2 = (vec2 - center).normalized;
        vec1 = (vec1 - center).normalized;
        Vector2 v = (vec2 - vec1).normalized;
        float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(vec2, vec1)*sign ;
        /*Vector2 diference = vec2 - vec1;
        float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, diference) * sign;*/
    }

    // Update is called once per frame
    void Update () {
        Debug.Log(AngleBetweenVector2(v1, v2, center));
        Debug.DrawLine(new Vector3(), new Vector3(v1.x, 0, v1.y), Color.cyan);
        Debug.DrawLine(new Vector3(), new Vector3(v2.x, 0, v2.y), Color.red);

    }
}
