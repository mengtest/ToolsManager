using UnityEngine;
using System.Collections;

public class AudioUpdateExecutor : ParentUpdateExecutor
{

	#region implemented abstract members of BaseUpdateExecutor

	public override string GetUpdateType ()
	{
		return "Audio";
	}

	#endregion

}
