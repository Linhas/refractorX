using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTime : MonoBehaviour {

	public float Limit; //seconds or frames

	public bool UseFrames;

	private float counter=0;
	
	// Update is called once per frame
	void Update () {
		if (UseFrames) {
			counter++;
		} else {
			counter += Time.deltaTime;
		}

		if (counter > Limit)
			Destroy (gameObject);
	}
}
