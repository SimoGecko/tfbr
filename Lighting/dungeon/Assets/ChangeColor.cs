using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour {

	private float duration = 0.2f;

	// Alternate between color c1 and c2
	public Color c1;
	public Color c2;
	
	// Update is called once per frame
	void Update () {
		Light l = GetComponent<Light>();
		float t = Mathf.PingPong (Time.deltaTime, duration) / duration;
		l.color = Color.Lerp (c1, c2, t);
	}
}
