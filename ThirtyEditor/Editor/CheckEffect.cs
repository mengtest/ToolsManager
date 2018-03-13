using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;

public class CheckEffectWindow 
{
	private const string MatrialPath = "Assets/XGame/Data/FX/Materials/";
	private static Shader s_particle_add = null;
	private static Shader s_particle_blend = null;

	//[MenuItem ("EZFun/资源替换/Fx_Shader")]
	static void CheckFxShader()
	{
		DirectoryInfo playerDirectoryInfo = new DirectoryInfo(MatrialPath);
		FileInfo[]  matFileInfos = playerDirectoryInfo.GetFiles("*.mat");
		s_particle_add = Shader.Find("EZFun/Particles/Additive");
		s_particle_blend = Shader.Find("EZFun/Particles/AlphaBlend");

		for(int i = 0 ; i < matFileInfos.Length; i ++)
		{
			Material mat = AssetDatabase.LoadAssetAtPath(MatrialPath + matFileInfos[i].Name,
			                                              typeof(Material)) as Material;
			if(mat.shader.name == "Particles/Additive")
			{
				mat.shader = s_particle_add;

			}
			else if(mat.shader.name == "Particles/Alpha Blended")
			{
				mat.shader = s_particle_blend;
			}
		}
	}
	private const string skillItemPath = "Assets/XGame/Resources/Prefab/SkillItem/";
	//[MenuItem ("EZFun/资源替换/Fx_Animator")]
	static void CheckFxAnimator()
	{
		DirectoryInfo playerDirectoryInfo = new DirectoryInfo(skillItemPath);
		FileInfo[]  animFileInfos = playerDirectoryInfo.GetFiles("*.prefab");
		for(int i = 0 ; i < animFileInfos.Length; i++)
		{
			GameObject obj = AssetDatabase.LoadAssetAtPath(skillItemPath + animFileInfos[i].Name,typeof(GameObject)) as GameObject;
			Animator[] animators = obj.transform.GetComponentsInChildren<Animator>();


			for(int j = 0 ; j < animators.Length; j++)
			{
				animators[j].applyRootMotion = false;
				animators[j].cullingMode = AnimatorCullingMode.CullUpdateTransforms;
			}

			Animation [] anims = obj.GetComponentsInChildren<Animation>();
			for(int k = 0 ; k <anims.Length ; k++ )
			{
				anims[k].cullingType = AnimationCullingType.BasedOnRenderers;
			}
		}

	}


}
