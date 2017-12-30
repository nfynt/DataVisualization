using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour {

	public Transform target;
	public bool onlyOnEnable = false;

	void OnEnable()
	{
		if (target == null)
			target = Camera.main.transform;
		
		transform.LookAt (target.position);
		transform.rotation = Quaternion.Euler (new Vector3 (0f, transform.rotation.eulerAngles.y, 0f));
	}

	void Update()
	{
		if (target != null) {
			transform.LookAt (target.position);
			transform.rotation = Quaternion.Euler (new Vector3 (0f, transform.rotation.eulerAngles.y, 0f));
		}
	}
}
