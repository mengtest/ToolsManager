using UnityEngine;
using System.Collections;

public static class AudioUtil {

	public static void PlayEffect(string effect)
	{
		AudioSys.Instance.PlayEffect (effect);
	}



}
