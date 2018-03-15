using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;

public class ExtractAnimWindow : EditorWindow {
	
	private const string DefaultState = "idle";
	private const int StateNamePositon = 2;
	private const string m_fbxPath = "Assets/XGame/Data/Animation/";
	private bool b_showPlayer = false;
	private bool b_showEnemy = false;

	[MenuItem ("EZFun/提取人物动画控制器")]
	static void Init () {
		ExtractAnimWindow window = (ExtractAnimWindow)EditorWindow.GetWindow (typeof (ExtractAnimWindow));
		window.titleContent = new GUIContent("动画控制器提取");
	}
	
	void OnGUI()
	{
		HandlePlayerGUI();
		HanldeEnemyGUI();
	}



	#region create enemy prefab
	private string s_modleFBXPath = "";
	private string s_modleAnimationPath = "";
	private const string EnemyModleDefaultPath = "Assets/XGame/Data/models/monster/";
	private const string EnemyAnimDefaultPath = "Assets/XGame/Data/Animation/monster/";
	private const string EnemyPrefabPath = "Assets/XGame/Resources/Prefab/Enemy/Sample Monster/";
	private const string EnemyControllerPath = "Assets/XGame/Data/AnimatorController/monster/";
	private const string ShadowPlanePath = "Assets/XGame/Data/models/other/";
	private const int EnemyStateNamePath = 1;
	private void HanldeEnemyGUI()
	{
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb, GUILayout.ExpandWidth(true));
		{
			b_showEnemy =  EditorGUILayout.Foldout(b_showEnemy, new GUIContent("Enemy Prefab 提取", "导入创建Enemy动画,Prefab"));
		}
		EditorGUILayout.EndVertical();
		
		if(b_showEnemy)
		{
			s_modleFBXPath = EditorGUILayout.TextField("FBX模型目录名称：" ,s_modleFBXPath);
			s_modleAnimationPath = EditorGUILayout.TextField("Anim动画文件目录名称：", s_modleAnimationPath);

			if (GUILayout.Button("创建Prefab"))
			{
				ExtractEenemyAnim();
				SetEenmyModel();
			}
		}
		EditorGUILayout.Space();
	
	}

	private UnityEditor.Animations.AnimatorController enemyController =  null;
	private void SetEenmyModel()
	{
		string modlePath = EnemyModleDefaultPath + s_modleFBXPath + "/";
		DirectoryInfo playerDirectoryInfo = new DirectoryInfo(modlePath);
		FileInfo[]  fbxFileInfos = playerDirectoryInfo.GetFiles("*.FBX");
		if(fbxFileInfos.Length != 0)
		{
			ModelImporter modelImporter = (ModelImporter) ModelImporter.GetAtPath(modlePath + fbxFileInfos[0].Name);
			modelImporter.importAnimation = false;
			modelImporter.animationType = ModelImporterAnimationType.Generic;
			
			GameObject enemyModel = (GameObject) Instantiate((GameObject)AssetDatabase.LoadAssetAtPath(modlePath + fbxFileInfos[0].Name,typeof(GameObject)));		
			enemyModel.name = enemyModel.name.Substring(0,enemyModel.name.Length -7);

			if(enemyModel != null)
			{
				string[] modelNameSplit = System.Text.RegularExpressions.Regex.Split(enemyModel.name,"@");
				string modelName = modelNameSplit[1];
				string tag = modelNameSplit[0];

				if(tag == "BOSS" || tag =="boss")
				{
					enemyModel.name = "monster_boss_" + modelName;
					enemyModel.tag = "Boss";
				}
				else
				{
					enemyModel.name = "monster_" + modelName;
					enemyModel.tag = "Enemy";
				}
				enemyModel.layer = LayerMask.NameToLayer("Enemy");
				
				SkinnedMeshRenderer[] renderers = enemyModel.GetComponentsInChildren<SkinnedMeshRenderer>();
				for(int i = 0 ; i < renderers.Length ; i++)
				{
					renderers[i].sharedMaterial.shader = Shader.Find("EZFun/ShieldDiffuse");
					renderers[i].transform.name = "MeshRenderer";
					renderers[i].transform.tag = "MeshRenderer";
					renderers[i].transform.gameObject.layer = LayerMask.NameToLayer("Enemy");
				}

				//添加阴影
				GameObject plane = (GameObject) Instantiate((GameObject)AssetDatabase.LoadAssetAtPath(ShadowPlanePath + "plane.Fbx", typeof(GameObject))) ; // .Load(ShadowPlanePath + "plane");
				plane.transform.parent = enemyModel.transform;
				plane.transform.localPosition = new Vector3(0,0.1f,0);
				plane.transform.localScale = new Vector3(0.1f,0.1f,0.1f);


				plane.transform.name = "shadow";
				DestroyImmediate(plane.transform.GetComponent<Animator>());
				
				GameObject prefab =  PrefabUtility.CreatePrefab(EnemyPrefabPath  + enemyModel.name + ".prefab"  , enemyModel);	
				BoxCollider boxColiider =  prefab.AddComponent<BoxCollider>();
				boxColiider.isTrigger = true;

				prefab.AddComponent<CharacterController>();

				if(enemyController != null)
				{
					Animator animator = prefab.GetComponent<Animator>();
					animator.runtimeAnimatorController = enemyController;
                    animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				}
				DestroyImmediate(enemyModel);
			}
		}
		else
		{
			Debug.LogError("cant't find FBX");
		}
	}




	private void ExtractEenemyAnim()
	{
		string enemyAnimPath = EnemyAnimDefaultPath + s_modleAnimationPath + "/";
		DirectoryInfo playerDirectoryInfo = new DirectoryInfo(enemyAnimPath);
		FileInfo[]  fbxFileInfos = playerDirectoryInfo.GetFiles("*.Fbx");
		
		foreach(FileInfo file in fbxFileInfos)
		{
			ModelImporter modelImporter = (ModelImporter) ModelImporter.GetAtPath(enemyAnimPath + file.Name);
			modelImporter.animationRotationError = 2;
			modelImporter.animationPositionError = 2;
			modelImporter.animationScaleError = 2;
			modelImporter.meshCompression = ModelImporterMeshCompression.Low;

			AnimationClip clip = AssetDatabase.LoadAssetAtPath(enemyAnimPath + file.Name,
			                                                   typeof(AnimationClip)) as AnimationClip;
			ModelImporterClipAnimation[] anims = modelImporter.clipAnimations;

			//anims[0].loopTime = true;

			AnimationClip tempClip = new AnimationClip();
			EditorUtility.CopySerialized(clip,tempClip);
			AssetDatabase.CreateAsset(tempClip, enemyAnimPath + clip.name +".anim");
		}

		//生成动画控制器
		string controllerName =  s_modleAnimationPath + ".controller" ;
		UnityEditor.Animations.AnimatorController controller = new UnityEditor.Animations.AnimatorController();
		controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(EnemyControllerPath + controllerName );
		enemyController = controller; 
        
		UnityEditor.Animations.AnimatorControllerLayer layer=controller.layers[0];
		UnityEditor.Animations.AnimatorStateMachine stateMachine=layer.stateMachine;

        

		FileInfo[]  animFileInfos = playerDirectoryInfo.GetFiles("*.anim");
		foreach(FileInfo file in animFileInfos)
		{
			AnimationClip clip = AssetDatabase.LoadAssetAtPath(enemyAnimPath + file.Name,
			                                                   typeof(AnimationClip)) as AnimationClip;
			string[] animationNameSplit = System.Text.RegularExpressions.Regex.Split(clip.name,"_");
			string stateName = animationNameSplit[EnemyStateNamePath];
			stateName = stateName.ToLower();
			if(stateName == "walk")
			{
				stateName = "run";
			}
			//将状态添加到状态机中
			UnityEditor.Animations.AnimatorState state=stateMachine.AddState(stateName);

            //state.SetAnimationClip(clip);

            state.motion = clip;

            if (state.name == DefaultState)
			{
				stateMachine.defaultState=state;
			}
		}
	}
	#endregion


	#region extract player anim
	private const string PlayerAnimFbxPath = "Assets/XGame/Data/Animation/";
	public string sex = "";
	public string weaponName = "";

	private void HandlePlayerGUI()
	{
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb, GUILayout.ExpandWidth(true));
		{
			b_showPlayer =  EditorGUILayout.Foldout(b_showPlayer, new GUIContent("主角动画控制器提取","创建主角动画"));
		}
		EditorGUILayout.EndVertical();
		if(b_showPlayer)
		{
			sex = EditorGUILayout.TextField("性别目录： " ,sex);
			weaponName = EditorGUILayout.TextField("动作目录： ", weaponName);

			if (GUILayout.Button("提取主角动画文件"))
			{	
				ExtractPlayerAnimatorFbx(sex,weaponName);
			}
		}
		EditorGUILayout.Space();
	}
	private UnityEditor.Animations.AnimatorController playerController = null;
	public void ExtractPlayerAnimatorFbx(string targetSex,string targetSet)
	{
		string targetPath  = PlayerAnimFbxPath + targetSex +   "/" + targetSet + "/";

		//提取anim动画
		DirectoryInfo playerDirectoryInfo = new DirectoryInfo(targetPath);
		FileInfo[]  fileInfos = playerDirectoryInfo.GetFiles("*.Fbx");

		foreach(FileInfo file in fileInfos)
		{
			AnimationClip clip = AssetDatabase.LoadAssetAtPath(targetPath + file.Name,
			                                                   typeof(AnimationClip)) as AnimationClip;
			AnimationClip tempClip = new AnimationClip();
			EditorUtility.CopySerialized(clip,tempClip);
			AssetDatabase.CreateAsset(tempClip, targetPath + clip.name +".anim");
		}
		string controllerName = targetSex + "_" + targetSet + ".controller" ;
		UnityEditor.Animations.AnimatorController controller=new UnityEditor.Animations.AnimatorController();
		controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(targetPath + controllerName );
		playerController = controller;
		UnityEditor.Animations.AnimatorControllerLayer layer = controller.layers[0];
		UnityEditor.Animations.AnimatorStateMachine stateMachine = layer.stateMachine;

		LoadCommonAnimToStateMachine(stateMachine,targetSex);

		FileInfo[]  animFileInfos = playerDirectoryInfo.GetFiles("*.anim");
		foreach(FileInfo file in animFileInfos)
		{
			AnimationClip clip = AssetDatabase.LoadAssetAtPath(targetPath + file.Name,
			                                                   typeof(AnimationClip)) as AnimationClip;
			string[] animationNameSplit = System.Text.RegularExpressions.Regex.Split(clip.name,"_");
			string stateName = animationNameSplit[2];
			stateName = stateName.ToLower();
			if(stateName == "walk")
			{
				stateName = "run";
			}
			AddStateToStateMachine(stateName,stateMachine,clip);
		}
	}

	public const string CommonTargetPath = "Assets/XGame/Data/Animation/";
	private void LoadCommonAnimToStateMachine(UnityEditor.Animations.AnimatorStateMachine machine,string sex)
	{
		string commonAnimPath = CommonTargetPath + sex + "/common" + "/";
		DirectoryInfo commonDirectoryInfo = new DirectoryInfo(commonAnimPath);
		FileInfo[]  animFileInfos = commonDirectoryInfo.GetFiles("*.anim");
		foreach(FileInfo file in animFileInfos)
		{
			AnimationClip clip = AssetDatabase.LoadAssetAtPath(commonAnimPath + file.Name,
			                                                   typeof(AnimationClip)) as AnimationClip;
			string[] animationNameSplit = System.Text.RegularExpressions.Regex.Split(clip.name,"_");
			string stateName = animationNameSplit[2];
			stateName = stateName.ToLower();
			AddStateToStateMachine(stateName,machine,clip);
		}
	}

	private void AddStateToStateMachine(string stateName,UnityEditor.Animations.AnimatorStateMachine machine,AnimationClip clip)
	{
		UnityEditor.Animations.AnimatorState state = machine.AddState(stateName);
        state.motion = clip;
		if(state.name == DefaultState)
		{
			machine.defaultState = state;
		}
	}

	#endregion
}
