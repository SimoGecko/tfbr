using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
	// Rotate around axis, object position unchanged
	public float rotateX;
	public float rotateY;
	public float rotateZ;

	// Orbit axis
	public Vector3 orbitAxis;
	// Orbit around a point
	public Vector3 rotationCenter;

	// Orbit around object
	public GameObject rotationObject;

	// Move along axis
	public float translateX;
	public float translateY;
	public float translateZ;
	
	// Update is called once per frame
	void Update () {
		/* Rotate Object */
		// Rotate around axis
		RotateAxis();

		// Orbit around a point
		//RotatePoint();

		// Rotate around an object
		if (rotationObject != null) {
			RotateObject();
		}
			
		TranslateAxis();
	}

	// Rotate around x,y,z axis
	private void RotateAxis() {
		transform.Rotate (Vector3.right * Time.deltaTime * rotateX);
		transform.Rotate (Vector3.up * Time.deltaTime * rotateY);
		transform.Rotate (Vector3.forward * Time.deltaTime * rotateZ);
	}

	// Orbit around a point
	private void RotatePoint() {
		transform.RotateAround(rotationCenter,orbitAxis, 10 * Time.deltaTime);
	}

	// Orbit around an object
	private void RotateObject() {
		transform.RotateAround (rotationObject.transform.position,orbitAxis, 10 * Time.deltaTime);
	}

	// Translate along axis
	private void TranslateAxis() {
		transform.Translate(Vector3.right * Time.deltaTime * translateX);
		transform.Translate(Vector3.up * Time.deltaTime * translateY);
		transform.Translate(Vector3.forward * Time.deltaTime * translateZ);
	}
}
