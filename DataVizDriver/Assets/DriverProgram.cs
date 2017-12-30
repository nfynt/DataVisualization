using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.paas.sdk.csharp;
using com.paas.sdk.csharp.storage;
using com.paas.sdk.csharp.user;
using com.paas.sdk.csharp.upload;
using LitJson;
using System;
using UnityEngine.UI;

public class DriverProgram : MonoBehaviour {

	public enum DataType{
		TRAFFIC,SCORE
	}

	public DataType currDataType = DataType.TRAFFIC;
	public bool updateRecord = false;
	public TrafficData trafficInst;
	/// <summary>
	/// Number of points
	/// </summary>
	public int trafficPoints = 4;
	public ScoreData scoreInst;

	//Private data members
	int randInt;
	ServiceAPI serviceApi;
	const string apiKey = "e16";
	const string secretKey = "11c5";

	//SERVICES
	StorageService storageService = null;
	StorageResponse storageCallback = new StorageResponse ();
	//--------------------------

	string dbName = "DataViz";
	string collectionName = "test";
	string jsonData = "{}";
	const string trafficDocId = "5a2b1ee021";
	const string scoreDocId = "5a436f";
	const string randomBase = "askjhflkdsfblkn234lksdhfsdf0";
	const string nameBase = "abcdefghijklmnopqrstuvwx yz";

	void Awake()
	{
		serviceApi = new ServiceAPI (apiKey, secretKey);
	}

	void Start()
	{
		trafficInst = new TrafficData ();
		scoreInst = new ScoreData ();
		UpdateTrafficPoints (trafficPoints);
		InvokeRepeating ("PopulateData", 0.1f, 1f);
	}

	public void SwitchDataType(Dropdown dataType)
	{
		switch (dataType.value) {
		case 0:		//Traffic Data
			currDataType = DataType.TRAFFIC;
			CancelInvoke ("PopulateData");
			InvokeRepeating ("PopulateData", 0.1f, 1f);
			break;

		case 1:		//Score Data
			currDataType = DataType.SCORE;
			CancelInvoke ("PopulateData");
			InvokeRepeating ("PopulateData", 0.1f, 5f);
			break;
		}
	}

	public void ToggleUpdateRecord()
	{
		updateRecord = !updateRecord;

		if (updateRecord) {
			if (currDataType == DataType.TRAFFIC)
				InvokeRepeating ("SyncDataWithWeb", 0.1f, 2f);
			else if (currDataType == DataType.SCORE)
				InvokeRepeating ("SyncDataWithWeb", 0.1f, 5f);
		}
		else
			CancelInvoke ("SyncDataWithWeb");
	}

	void SyncDataWithWeb()
	{
		if (currDataType == DataType.TRAFFIC)
			jsonData = JsonMapper.ToJson (trafficInst);
		else if (currDataType == DataType.SCORE)
			jsonData = JsonMapper.ToJson(scoreInst);
		else
			jsonData = "";

//		#if UNITY_EDITOR
//		Debug.Log ("Pushing: " + jsonData);
//		#endif

		if (jsonData == null || jsonData == "")
			return;

		storageService = serviceApi.BuildStorageService ();
		//storageService.InsertJSONDocument (dbName, collectionName, jsonData, storageCallback);
		if (currDataType == DataType.TRAFFIC)
			storageService.UpdateDocumentByDocId (dbName, collectionName, trafficDocId, jsonData, storageCallback);
		else if (currDataType == DataType.SCORE)
			storageService.UpdateDocumentByDocId (dbName, collectionName, scoreDocId, jsonData, storageCallback);
	}

	public class StorageResponse : App42CallBack
	{
		private string result = "";
		public void OnSuccess(object obj)
		{
			result = obj.ToString();
			if (obj is Storage)
			{
				Storage storage = (Storage)obj;
				Debug.Log ("Storage Response : " + storage);
			}
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

	/// <summary>
	/// Updates the number of traffic points
	/// </summary>
	/// <param name="points">Number of Points.</param>
	public void UpdateTrafficPoints(int points){
		trafficInst.points.Clear ();
		trafficInst.weights.Clear ();

		for (int i = 0; i < points; i++) {
			trafficInst.weights.Add (0);
			string ss = "";
			for (int j = 0; j < 5; j++)
				ss += randomBase [UnityEngine.Random.Range (0, 20)];
			trafficInst.points.Add (ss);
		}

		trafficPoints = points;
	}

	void PopulateData()
	{
		if (updateRecord) {

			//TRAFFIC DATA DRIVER
			if (currDataType == DataType.TRAFFIC) {
				randInt = UnityEngine.Random.Range (0, trafficPoints);
				trafficInst.weights [randInt]++;
			} 
			else if (currDataType == DataType.SCORE) {		//SCORE DATA DRIVER
				
				string ss = "";
				for (int i = 0; i < 8; i++)
					ss += nameBase [UnityEngine.Random.Range (0, 27)];
				//scoreInst.name.Add (ss);
				scoreInst.AddData (UnityEngine.Random.Range (10f, 100f), ss);
			}

		}
	}
}

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
