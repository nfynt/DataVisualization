using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	RaycastHit hit;
	GameObject currGazeObj;

	void Update()
	{
		Debug.DrawRay (transform.position, transform.forward, Color.red);

		if (Physics.Raycast (transform.position, transform.forward, out hit)) {

			if (hit.transform.tag == "ToggleCube" && hit.transform.gameObject != currGazeObj) {
				if (currGazeObj != null)
					currGazeObj.GetComponent<ToggleCube> ().GazeExit ();
				hit.transform.GetComponent<ToggleCube> ().GazeEnter ();
				currGazeObj = hit.transform.gameObject;
			} else if (hit.transform.gameObject != currGazeObj) {
				currGazeObj.GetComponent<ToggleCube> ().GazeExit ();
				currGazeObj = null;
			}
		} else if (currGazeObj != null) {
			currGazeObj.GetComponent<ToggleCube> ().GazeExit ();
			currGazeObj = null;
		}
	}
}
