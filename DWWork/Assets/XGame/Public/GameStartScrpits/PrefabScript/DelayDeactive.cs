using UnityEngine;
using System.Collections;

public class DelayDeactive : MonoBehaviour {

	public float Delay_Time = 1f;
	// Use this for initialization
	void OnEnable () {
		StartCoroutine ("Deactive");
	}

	void OnDisable(){
		StopCoroutine ("Deactive");
	}

	IEnumerator Deactive(){
		yield return new WaitForSeconds (Delay_Time);
		transform.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
