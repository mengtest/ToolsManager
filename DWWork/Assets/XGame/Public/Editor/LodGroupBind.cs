using UnityEngine;
using System.Collections;
using UnityEditor;

public class LodGroupBind : EditorWindow {

	public string sceneObjectName = null;
	public float lodScaleMid = 0.13f;
	public float lodScaleSmall = 0.2f;

	[MenuItem ("EZFun/绑定场景物件的LodGroup组件")]
	static void Init()
	{
		LodGroupBind bindWindow = (LodGroupBind)EditorWindow.GetWindow(typeof(LodGroupBind));
		bindWindow.titleContent = new GUIContent("场景Lod绑定");
	}

	void OnGUI()
	{
		BindLodGUI();
	}


	private void BindLodGUI()
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("LOD切换系数是指物体占屏幕的比例");
		EditorGUILayout.Space();
		lodScaleMid = EditorGUILayout.FloatField("mid lod scale: ",lodScaleMid);
		EditorGUILayout.Space();
		lodScaleSmall = EditorGUILayout.FloatField("small lod scale: ",lodScaleSmall);
		EditorGUILayout.Space();

		if(GUILayout.Button("绑定Lod"))
		{
			AddLODGroupToObjec();
		}

		EditorGUILayout.Space();
		if(GUILayout.Button("清除所有Lod"))
		{
			RemoveAllLod();
		}
		EditorGUILayout.Space();
	}

	private void AddLODGroupToObjec()
	{
		Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

		for(int i = 0 ; i < objects.Length; i++)
		{
			GameObject targetObject = (GameObject)objects[i];
			Renderer[] renders = {targetObject.GetComponent<Renderer>()};
			if(objects[i].name.Contains("mid") )
			{
				LOD[] lod = {new LOD(lodScaleMid,renders)};
				LODGroup lodGroup = targetObject.AddComponent<LODGroup>();
				lodGroup.SetLODS(lod);
			}
			else if(objects[i].name.Contains("small"))
			{
				LOD[] lod = {new LOD(lodScaleSmall,renders)};
				LODGroup lodGroup = targetObject.AddComponent<LODGroup>();
				lodGroup.SetLODS(lod);
			}
		}
		Debug.LogError(objects.Length);
	}

	private void RemoveAllLod()
	{
		Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
		
		for(int i = 0 ; i < objects.Length; i++)
		{
			GameObject targetObject = (GameObject)objects[i];
			DestroyImmediate(targetObject.GetComponent<LODGroup>());
		}
	}

}
