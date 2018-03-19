using UnityEngine;
using System.Collections;

public class SpriteAnimator : MonoBehaviour {

	public float scrollSpeed = 0.0f;
	public int countX = 1;
	public int countY = 1;
	public int invertalTime = 0;
	private Vector2 singleTexSize ;
	private float offsetX = 0.0f;
	private float offsetY = 0.0f;
	private bool waitForPlay = false;
	private bool hasPlayOnce = false;

	void Start ()
	{
		singleTexSize = new  Vector2(1.0f/countX,1.0f/countY);
		GetComponent<Renderer>().material.mainTextureScale = singleTexSize;
		waitForPlay = false;
		hasPlayOnce = false;
	}

	void Update () 
	{
		if(!hasPlayOnce)
		{
			var frame = Mathf.Ceil(Time.time * scrollSpeed);
			offsetX = frame / countX;
			offsetY = -(frame - frame % countX)/countY /countX;
			GetComponent<Renderer>().material.SetTextureOffset("_MainTex",new Vector2(offsetX,offsetY));	
			if( (frame+1)  % countX == 0)
			{
				hasPlayOnce = true;
			}
		}

		if(hasPlayOnce)
		{
			if(!waitForPlay)
			{
				StartCoroutine(DelayForInvertalTime());
				waitForPlay  = true;
			}
		}
	}

	IEnumerator DelayForInvertalTime()
	{
		yield return new WaitForSeconds(invertalTime);
		hasPlayOnce = false;
		waitForPlay = false;
	}
}
