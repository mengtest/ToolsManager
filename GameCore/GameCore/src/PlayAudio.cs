using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(UIButton))]
//[RequireComponent(typeof(UIButtonMessage))]
public class PlayAudio : MonoBehaviour {

	public delegate void OnPlayAudio (PlayAudio ins, int audioId);

	public static System.Action<PlayAudio, int> onPlayCallback;

	[SerializeField]
	private int audioId;
	public int AudioId
	{
		get { return audioId; }
		set { audioId = value; }
	}

	public string displayName;

	void OnClick ()
	{
		if (onPlayCallback != null) {
			onPlayCallback (this, audioId);
		}
	}

//	[ContextMenu("刷新")]
//	private void RefreshItems()
//	{
//		
//	}

}
