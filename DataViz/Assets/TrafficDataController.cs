using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficDataController : MonoBehaviour {

	public enum Orientation
	{
		LINEAR, CIRCULAR
	}

	public Orientation barOrient = Orientation.LINEAR;
	public GameObject barPrefab;
	public GameObject bartxtPrefab;
	public float distFromCam = 3f;
	public float maxHeight = 10f;

	List<GameObject> bars = new List<GameObject>();
	List<GameObject> barTxt = new List<GameObject> ();
	int maxWeight;

	public void RefreshRecords()
	{
		if (DataVizualiser.Instance.trafficInst == null)
			return;
		maxWeight = 1;
		foreach (int i in DataVizualiser.Instance.trafficInst.weights)
			if (i > maxWeight)
				maxWeight = i;

		Debug.Log ("Max weight: " + maxWeight);
		
		if (bars.Count != DataVizualiser.Instance.trafficInst.points.Count) {
			foreach (GameObject go in bars)
				Destroy (go);
			bars.Clear ();
			for (int i = 0; i < DataVizualiser.Instance.trafficInst.points.Count; i++) {
				GameObject go = Instantiate (barPrefab, transform.position, Quaternion.identity) as GameObject;
				go.transform.parent = DataVizualiser.Instance.trafficParentObj.transform;
				go.transform.localScale = new Vector3 (go.transform.localScale.x, maxHeight*((float)DataVizualiser.Instance.trafficInst.weights [i]/(float)maxWeight), go.transform.localScale.z);
				bars.Add (go);

				GameObject go2 = Instantiate (bartxtPrefab, transform.position, Quaternion.identity) as GameObject;
				go2.transform.position = new Vector3 (go.transform.position.x, go.transform.localScale.y * 0.1f, go.transform.position.z);
				go2.transform.parent = DataVizualiser.Instance.trafficParentObj.transform;
				go2.GetComponent<TextMesh> ().text = DataVizualiser.Instance.trafficInst.weights [i].ToString() + "\n"+DataVizualiser.Instance.trafficInst.points[i];//.ToString ("F");
				barTxt.Add (go2);
			}
			Reorient ();
		} else {
			for (int i = 0; i < DataVizualiser.Instance.trafficInst.points.Count; i++) {
				bars[i].transform.localScale = new Vector3 (bars[i].transform.localScale.x, maxHeight*((float)DataVizualiser.Instance.trafficInst.weights [i]/(float)maxWeight), bars[i].transform.localScale.z);
				barTxt[i].transform.position = new Vector3 (bars[i].transform.position.x, bars[i].transform.localScale.y * 0.1f, bars[i].transform.position.z);
				barTxt[i].GetComponent<TextMesh> ().text = DataVizualiser.Instance.trafficInst.weights [i].ToString() + "\n"+DataVizualiser.Instance.trafficInst.points[i];//.ToString ("F");
			}
		}

	}

	public void Reorient()
	{
		if (bars.Count > 1) {
			if (bars.Count < 10) {
				barOrient = Orientation.LINEAR;
				//Vector3 pos = transform.position + new Vector3 (0, 0, 10);
				int count = DataVizualiser.Instance.trafficInst.weights.Count;
				float delta = 360f / count;
				delta = 10;
				float currAng = 0f;

				foreach (GameObject go in bars) {
					go.transform.position = transform.position + new Vector3 (distFromCam * Mathf.Cos ((currAng + delta) * Mathf.Deg2Rad), 0, distFromCam * Mathf.Sin ((currAng + delta) * Mathf.Deg2Rad));
					currAng += delta;
					go.transform.LookAt (transform);
					Color col = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
					go.transform.GetComponentInChildren<MeshRenderer> ().material.color = col;
					go.transform.GetComponentInChildren<MeshRenderer> ().material.SetColor ("_EmissionColor", col);
				}

			} else {
				barOrient = Orientation.CIRCULAR;
				//Handle adjusting the populations
				int count = DataVizualiser.Instance.trafficInst.weights.Count;
				float delta = 360f / count;
				float currAng = 0f;

				foreach (GameObject go in bars) {
					go.transform.position = transform.position + new Vector3 (distFromCam * Mathf.Cos ((currAng + delta) * Mathf.Deg2Rad), 0, distFromCam * Mathf.Sin ((currAng + delta) * Mathf.Deg2Rad));
					currAng += delta;
					go.transform.LookAt (transform);
				}
			}
		} else {
			bars [0].transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 5f);
		}
	}
}
