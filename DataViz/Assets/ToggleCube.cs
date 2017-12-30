using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCube : MonoBehaviour {

	public float selectionTimer = 2f;
	public bool gazing = false;
	private float gazeTime;

	private bool showtraffic = true;

	public void GazeEnter()
	{
		gazing = true;
		gazeTime = 0;
	}

	void Update()
	{
		if (gazing) {
			gazeTime += Time.deltaTime;
			if (gazeTime >= selectionTimer) {
				Select ();
				gazing = false;
			}
		}
	}

	public void Select()
	{
		showtraffic = !showtraffic;

		DataVizualiser.Instance.SwitchDataType (showtraffic);

		if (showtraffic) {
			transform.GetComponentInChildren<TextMesh> ().text = "Traffic";
		} else {
			transform.GetComponentInChildren<TextMesh> ().text = "Score";
		}
	}

	public void GazeExit()
	{
		gazing = false;
		gazeTime = 0;
		DataVizualiser.Instance.RefreshRecord ();
	}

}
