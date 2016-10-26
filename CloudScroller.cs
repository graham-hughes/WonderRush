using UnityEngine;
using System.Collections;

public class CloudScroller : MonoBehaviour {
	// Runs the cloud scrolling animation in the homescreen

	public float speed = 0;
	public static CloudScroller current;

	float position = 0;

	void Start () {
		current = this;
	}
	
	void Update () {
		position += speed;
		if (position >= 1.0f){
			position -= 1f;
		}
		GetComponent<Renderer>().material.mainTextureOffset = new Vector2 (-1*position, 0);
	}
}