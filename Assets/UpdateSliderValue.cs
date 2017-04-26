using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateSliderValue : MonoBehaviour {

    Slider slider;
    Text text;
	// Use this for initialization
	void Start () {
        slider = transform.parent.parent.parent.GetComponent<Slider>();
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        text.text = slider.value.ToString();
        Debug.Log(slider.value.ToString());
        transform.position = transform.parent.transform.position;
	}
}
