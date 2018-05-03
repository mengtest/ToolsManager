using UnityEngine;
using System.Collections;

public class AudioUpdateExecutor : BaseUpdateExecutor {

	#region implemented abstract members of BaseUpdateExecutor

	public override string GetUpdateType ()
	{
		return "Audio";
	}

	#endregion

}
