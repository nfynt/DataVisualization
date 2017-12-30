using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.storage;
using com.shephertz.app42.paas.sdk.csharp.user;
using com.shephertz.app42.paas.sdk.csharp.upload;
using LitJson;
using System;
using UnityEngine.UI;


public class DataVizualiser : MonoBehaviour {

	public static DataVizualiser Instance;

	public enum DataType{
		TRAFFIC,SCORE
	}

	public DataType currDataType = DataType.TRAFFIC;
	public TrafficDataController tdcInstance;
	public ScoreDataController sdcInstance;
	public GameObject trafficParentObj;
	public TrafficData trafficInst;
	public GameObject scoreParentObj;
	public ScoreData scoreInst;

	//Private members
	ServiceAPI serviceApi;
	const string apiKey = "1a3c5db90391bbfc66219d9d77fb43bf9b78fcebecd18907ac797b33ea648e16";
	const string secretKey = "fb93b7ad8d2a6f51ddf1f37b19be269fa4bb3592c8e7cc3835ed6e9daa1611c5";

	//SERVICES
	StorageService storageService = null;
	StorageResponse storageCallback = new StorageResponse ();
	//--------------------------

	string dbName = "DataViz";
	string collectionName = "test";
	string jsonData = "{}";
	string trafficDocId = "5a437636e4b06d9d2b1ee021";
	string scoreDocId = "5a438b44e4b009a7f592ca6f";

	void Awake()
	{
		serviceApi = new ServiceAPI (apiKey, secretKey);
		Instance = this;
		trafficParentObj.SetActive (false);
		scoreParentObj.SetActive (false);
	}

	void Start()
	{
		trafficInst = new TrafficData ();
		scoreInst = new ScoreData ();
		RefreshRecord ();
	}

	public void SwitchDataType(Dropdown dataType)
	{
		switch (dataType.value) {
		case 0:		//Traffic Data
			currDataType = DataType.TRAFFIC;
			RefreshRecord ();
			break;

		case 1:		//Score Data
			currDataType = DataType.SCORE;
			RefreshRecord ();
			break;
		}
	}

	public void SwitchDataType(bool showTraffic){
		if (showTraffic) {
			currDataType = DataType.TRAFFIC;
			RefreshRecord ();
		} else {
			currDataType = DataType.SCORE;
			RefreshRecord ();
		}
	}

	public void RefreshRecord()
	{
		switch (currDataType) {
		case DataType.TRAFFIC:
			trafficParentObj.SetActive (true);
			scoreParentObj.SetActive (false);
			storageService = serviceApi.BuildStorageService ();
			storageService.FindDocumentById (dbName, collectionName, trafficDocId, storageCallback);
			break;

		case DataType.SCORE:
			trafficParentObj.SetActive (false);
			scoreParentObj.SetActive (true);
			storageService = serviceApi.BuildStorageService ();
			storageService.FindDocumentById (dbName, collectionName, scoreDocId, storageCallback);
			break;
		}
	}

	public void JsonResponse(string jstr){
		switch (currDataType) {
		case DataType.TRAFFIC:
			Debug.Log ("Traffic mapping: " + jstr);
			TrafficDataStr tds = new TrafficDataStr ();
			tds = JsonMapper.ToObject<TrafficDataStr> (jstr);
			trafficInst = tds.GetActualData ();
			tdcInstance.RefreshRecords ();
			break;

		case DataType.SCORE:
			Debug.Log ("Score mapping: " + jstr);
			ScoreDataStr sds = new ScoreDataStr ();
			sds = JsonMapper.ToObject<ScoreDataStr> (jstr);
			scoreInst = sds.GetActualData ();
			sdcInstance.RefreshRecords ();
			break;
		}
	}

	public class StorageResponse : App42CallBack
	{
		private string result = "";
		private string jsonResp;
		public void OnSuccess(object obj)
		{
			result = obj.ToString();
			if (obj is Storage) {
				Storage storage = (Storage)obj;
				Debug.Log ("Storage Response : " + storage);
//				IList<Storage.JSONDocument> jsonDocList = storage.GetJsonDocList ();
//				for (int i = 0; i < storage.GetJsonDocList ().Count; i++) {
//					Debug.Log ("ObjectId is : " + jsonDocList [i].GetDocId ());
//					Debug.Log ("jsonDoc is : " + jsonDocList [i].GetJsonDoc ());
//				}
				jsonResp = storage.GetJsonDocList () [0].GetJsonDoc ();
				//Debug.Log (jsonResp);
				DataVizualiser.Instance.JsonResponse (jsonResp);
			}
		}

		public string GetResponse()
		{
			return jsonResp;
		}

		public void OnException(Exception e)
		{
			result = e.ToString();
			Debug.Log ("Exception is : " + e);

		}

		public string getResult() {
			return result;
		}


	}

}//end class Data Visualizer

[Serializable]
public class TrafficData{
	public List<int> weights;
	public List<string> points;

	public TrafficData()
	{
		weights = new List<int> ();
		points = new List<string> ();
	}
}

class TrafficDataStr{
	public List<string> weights;
	public List<string> points;

	public TrafficDataStr()
	{
		weights = new List<string> ();
		points = new List<string> ();
	}

	public TrafficData GetActualData()
	{
		TrafficData td = new TrafficData ();
		for(int i=0;i<weights.Count;i++){
			td.weights.Add (Int32.Parse (weights[i]));
			td.points.Add (points [i]);
		}
		return td;
	}
}

[Serializable]
public class ScoreData{
	public List<double> marks;
	public List<string> name;
	public double avg;
	public int bestInd;
	public int worstInd;

	float bestmark;
	float worstmark;
	float sum;

	public ScoreData()
	{
		marks = new List<double> ();
		name = new List<string> ();
		avg = 0f;
		bestInd = worstInd = -1;
		sum = bestmark = 0;
		worstmark = 10000;
	}

	public void AddData(float mark, string name)
	{
		marks.Add ((double)mark);
		this.name.Add (name);

		if (mark > bestmark) {
			bestInd = marks.Count-1;
			bestmark = mark;
		}
		if (mark < worstmark) {
			worstInd = marks.Count - 1;
			worstmark = mark;
		}

		sum += mark;
		avg = (double)( sum / marks.Count);
	}
}

class ScoreDataStr{
	public List<string> marks;
	public List<string> name;
	public string avg;
	public string bestInd;
	public string worstInd;

	public ScoreDataStr()
	{
		marks = new List<string> ();
		name = new List<string> ();
		avg = "";
		bestInd = worstInd = "";
	}

	public ScoreData GetActualData()
	{
		ScoreData sd = new ScoreData ();

		sd.avg = float.Parse (avg);
		sd.bestInd = Int32.Parse (bestInd);
		sd.worstInd = Int32.Parse (worstInd);

		for (int i = 0; i < marks.Count; i++) {
			sd.marks.Add (double.Parse (marks [i]));
			sd.name.Add (name [i]);
		}

		return sd;
	}
}

