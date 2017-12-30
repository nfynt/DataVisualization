using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDataController : MonoBehaviour {


	public GameObject pointPrefab;

	List<GameObject> points = new List<GameObject>();
	public GameObject maxHeightObj;
	public GameObject maxLengthObj;
	public Material yellowMat;

	private float randLen;
	private float maxHeight;
	private float maxLength;

	public void RefreshRecords()
	{
		if (DataVizualiser.Instance.trafficInst == null)
			return;
		foreach (GameObject go in points)
			Destroy (go);

		points.Clear ();
		maxHeight = maxHeightObj.transform.position.y - transform.position.y;
		maxLength = maxLengthObj.transform.position.z - transform.position.z;

		for (int i = 0; i < DataVizualiser.Instance.scoreInst.marks.Count; i++) {
			GameObject go = Instantiate (pointPrefab, transform.position, Quaternion.identity) as GameObject;
			go.transform.parent = DataVizualiser.Instance.scoreParentObj.transform;
			go.transform.localPosition = new Vector3 (-Random.Range (0.1f, maxLength), maxHeight * ((float)DataVizualiser.Instance.scoreInst.marks [i] / 100f), 0f);
			points.Add (go);
		}

		//Reorient ();
		int best = DataVizualiser.Instance.scoreInst.bestInd;
		int worst = DataVizualiser.Instance.scoreInst.worstInd;

		Debug.Log (best.ToString () + "\t" + worst.ToString ());

		points[best].transform.localScale+=points[DataVizualiser.Instance.scoreInst.bestInd].transform.localScale/3f;
		points [best].transform.GetComponentInChildren<MeshRenderer> ().material = yellowMat;
		points [best].transform.GetChild(1).gameObject.SetActive(true);
		points [best].transform.GetComponentInChildren<TextMesh> ().text = DataVizualiser.Instance.scoreInst.name [best]+"\n"+DataVizualiser.Instance.scoreInst.marks[best].ToString("F1")+"%";
		Debug.Log (points [best].transform.GetComponentInChildren<MeshRenderer> ().material.color);


		points[worst].transform.localScale+=points[DataVizualiser.Instance.scoreInst.worstInd].transform.localScale/3f;
		points [worst].transform.GetComponentInChildren<MeshRenderer> ().material = yellowMat;
		points [worst].transform.GetChild(1).gameObject.SetActive(true);
		points [worst].transform.GetComponentInChildren<TextMesh> ().text = DataVizualiser.Instance.scoreInst.name [worst]+"\n"+DataVizualiser.Instance.scoreInst.marks[worst].ToString("F1")+"%";
		Debug.Log (points [worst].transform.GetComponentInChildren<MeshRenderer> ().material.color);
	}

}
