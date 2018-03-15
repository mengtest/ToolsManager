using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReferenceViewer
{
	public class ReferenceViewer : EditorWindow
	{
		[SerializeField]
		private Texture2D m_SearchIcon;
		[SerializeField]
		private Texture2D m_SearchIconPro;

		private static ReferenceViewer m_Window;

		private static Object m_ReferencedObject;
		private static IDictionary<Object, List<SerializedProperty>> m_References;

		private static bool m_TargetingAsset;
		private static int m_ReferenceFindType; // 0 for scene, 1 for assets
		private static Vector2 m_ScrollPosition;

		private static GUIStyle m_ReferenceButtonStyle;
		private static GUIStyle m_RowStyle;
		private static GUIStyle m_PropertyLabelStyle;

		private static Texture2D SearchIcon
		{
			get { return EditorGUIUtility.isProSkin ? m_Window.m_SearchIconPro : m_Window.m_SearchIcon; }
		}

		private static void CreateWindow()
		{
			m_Window = GetWindow<ReferenceViewer>(false, "References", true);
		}

		private void OnGUI()
		{
			if (m_ReferenceButtonStyle == null)
			{
				CreateStyles();
			}

			GUILayout.Label("Find references to:");
			var referenced = EditorGUILayout.ObjectField(m_ReferencedObject, typeof(Object), true);
			if (referenced != m_ReferencedObject)
			{
				SetReferencedObject(referenced);
			}

			if (m_TargetingAsset)
			{
				GUILayout.Space(5);
				int oldFindType = m_ReferenceFindType;
				m_ReferenceFindType = GUILayout.SelectionGrid(m_ReferenceFindType,
					new[] { "Find in scene", "Find in assets" }, 2, GUILayout.Height(20));
				if (oldFindType != m_ReferenceFindType)
				{
					FindReferences();
				}
			}

			if (ReferenceFinder.DoingAsyncWork)
			{
				DrawProgressBar();
			}

			if (m_References != null && m_References.Any())
			{
				DrawReferences();
			}
			else if (referenced != null && !ReferenceFinder.DoingAsyncWork)
			{
				GUILayout.Label("No references found.");
			}
		}

		private static void CreateStyles()
		{
			m_ReferenceButtonStyle = new GUIStyle(GUI.skin.button);
			m_ReferenceButtonStyle.alignment = TextAnchor.MiddleLeft;
			const int paddingSize = 5;
			var padding = new RectOffset(paddingSize, paddingSize, paddingSize, paddingSize);
			m_ReferenceButtonStyle.padding = padding;
			m_ReferenceButtonStyle.margin = padding;
			m_ReferenceButtonStyle.richText = true;

			m_RowStyle = new GUIStyle(GUI.skin.box);
			m_RowStyle.padding = padding;
			m_RowStyle.margin = padding;
			m_RowStyle.margin.top = 0;

			m_PropertyLabelStyle = new GUIStyle(GUI.skin.label);
			m_PropertyLabelStyle.richText = true;
		}

		private void DrawReferences()
		{
			GUILayout.Space(5);
			GUILayout.Label("References:");
			m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

			var filtered = m_References.Where(FilterProperties);
			var groups = filtered.GroupBy(e =>
			{
				var component = e.Key as Component;
				return component != null ? component.gameObject : e.Key;
			})
			.Select(g => new { Parent = g.Key, Children = g.Select(p => p) })
			.ToArray();

			foreach (var group in groups)
			{
				EditorGUILayout.BeginVertical(m_RowStyle);

				var parent = group.Parent;
				var children = group.Children;

				EditorGUILayout.BeginHorizontal();
				var buttonContent = EditorGUIUtility.ObjectContent(parent, parent.GetType());
				if (GUILayout.Button(buttonContent, m_ReferenceButtonStyle, GUILayout.Height(24)))
				{
					SelectObject(parent);
				}

				if (GUILayout.Button(SearchIcon, m_ReferenceButtonStyle, GUILayout.Height(24), GUILayout.Width(25)))
				{
					SetReferencedObject(parent);
				}
				EditorGUILayout.EndHorizontal();

				foreach (var child in children)
				{
					DrawReferenceGroup(child, parent);
				}
				EditorGUI.indentLevel = 0;
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndScrollView();
		}

		private static void DrawReferenceGroup(KeyValuePair<Object, List<SerializedProperty>> group, Object parent)
		{
			EditorGUI.indentLevel = 1;
			var objectWithReference = group.Key;
			if (objectWithReference != parent)
			{
				var componentType = objectWithReference.GetType();
				var componentContent = EditorGUIUtility.ObjectContent(objectWithReference, componentType);
				componentContent.text = componentType.Name;
				EditorGUILayout.LabelField(componentContent, GUILayout.Height(18));
				EditorGUI.indentLevel = 3;
			}

			foreach (var serializedProp in group.Value)
			{
				const string textTemplate = "<color=#{0}>{1}</color>";
				const string proColor = "89baec";
				const string personalColor = "214BE0";
				var text = string.Format(textTemplate, EditorGUIUtility.isProSkin ?
					proColor : personalColor, GetPropertyDisplayName(serializedProp));
				EditorGUILayout.LabelField(text, m_PropertyLabelStyle);
			}
		}

		private static void SelectObject(Object obj)
		{
			var objectToSelect = obj;
			if (PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab)
			{
				// If the component is nested more than 1 level deep, it will not show up in the
				// asset browser. So we select the root of the prefab.
				var objectParent = ((GameObject)obj).transform.parent;
				if (objectParent != null && objectParent.parent != null)
				{
					objectToSelect = PrefabUtility.FindPrefabRoot(objectParent.gameObject);
				}
			}

			EditorGUIUtility.PingObject(objectToSelect);
			Selection.activeObject = objectToSelect;
		}

		private static string GetPropertyDisplayName(SerializedProperty property)
		{
			const string arrayElementName = "data";
			if (property.depth > 0 && property.name == arrayElementName)
			{
				var arrayName = property.propertyPath.Substring(0, property.propertyPath.IndexOf(".", System.StringComparison.Ordinal));
				return string.Format("{0} - {1}", arrayName, property.displayName);
			}
			return property.displayName;
		}

		private bool FilterProperties(KeyValuePair<Object, List<SerializedProperty>> kvp)
		{
			if (kvp.Key == null) return false;
			kvp.Value.RemoveAll(prop => prop == null || prop.objectReferenceValue == null);
			return kvp.Value.Count > 0;
		}

		private void DrawProgressBar()
		{
			var lastRect = GUILayoutUtility.GetLastRect();
			const float padding = 5;
			const float cancelButtonWidth = 50;
			GUILayout.Space(padding);
			var progressRect = new Rect(lastRect.x, lastRect.yMax + padding,
				lastRect.width - cancelButtonWidth - padding, 20);
			const string findingReferencesText = "Finding references...";
			EditorGUI.ProgressBar(progressRect, ReferenceFinder.AsyncProgress, findingReferencesText);
			var cancelRect = new Rect(progressRect.x + progressRect.width + padding, progressRect.y,
				cancelButtonWidth, 20);
			if (GUI.Button(cancelRect, "Stop"))
			{
				ReferenceFinder.StopAsyncOperation();
			}
			GUILayout.Space(progressRect.height);
		}

		[MenuItem("Assets/Reference Viewer", false)]
		private static void FindReferencesToAsset()
		{
			CreateWindow();
			SetReferencedObject(Selection.activeObject);
		}

		[MenuItem("Assets/Reference Viewer", true)]
		private static bool FindReferencesToAssetValidation()
		{
			return Selection.activeObject != null && !(Selection.activeObject is DefaultAsset);
		}

		[MenuItem("CONTEXT/Component/Reference Viewer", false, -1)]
		private static void FindReferencesToComponent(MenuCommand data)
		{
			CreateWindow();
			SetReferencedObject(data.context);
		}

		[MenuItem("GameObject/Reference Viewer", false, -1)]
		private static void FindReferencesToGameObject(MenuCommand data)
		{
			CreateWindow();
			SetReferencedObject(data.context);
		}

		[MenuItem("GameObject/Reference Viewer", true, -1)]
		private static bool FindReferencesToGameObjectValidation(MenuCommand data)
		{
			// The context menu item is not greyed out properly for the GameObject context menu.
			if (data.context == null)
			{
				Debug.LogWarning("Please select a valid Object to search for references.");
				return false;
			}
			return true;
		}

		private static void SetReferencedObject(Object obj)
		{
			m_TargetingAsset = AssetDatabase.Contains(obj);
			m_ReferenceFindType = m_TargetingAsset ? m_ReferenceFindType : 0;
			m_ReferencedObject = obj;
			FindReferences();
		}

		private static void FindReferences()
		{
			m_References = new Dictionary<Object, List<SerializedProperty>>();
			if (m_ReferenceFindType == 0)
			{
				FindReferencesInScene(m_ReferencedObject);
			}
			else
			{
				FindReferencesInAssets(m_ReferencedObject);
			}
			EditorApplication.update += UpdateProgressGUI;
		}

		private static void UpdateProgressGUI()
		{
			if (ReferenceFinder.DoingAsyncWork)
			{
				m_Window.Repaint();
			}
			else
			{
				EditorApplication.update -= UpdateProgressGUI;
			}
		}

		private static void FindReferencesInScene(Object obj)
		{
			// FindObjectsOfType does not return inactive objects.
			// We search through all Components in the scene this way.
			var componentsInScene = Resources.FindObjectsOfTypeAll<Component>()
				.Where(component => !AssetDatabase.Contains(component));
			var objects = componentsInScene.Cast<Object>().ToList();
			ReferenceFinder.FindReferencesAsync(obj, objects, m_References, ReferencesFound);
		}

		private static void FindReferencesInAssets(Object obj)
		{
			ReferenceFinder.FindReferencesInAssetsAsync(obj, m_References, ReferencesFound);
		}

		private static void ReferencesFound()
		{
			m_Window.Repaint();
		}
	}

	public static class ReferenceFinder
	{
		private static EditorCoroutine m_Coroutine;
		private static int m_ObjectsToSearchAsync;
		private static int m_ObjectsSearchedAsync;
		private const int MaxAssetsToSearchPerFrame = 400;
		private const int MaxPropsToSearchPerFrame = 600;

		// Extend this list to include asset types which do not include references to other Objects.
		private static readonly string[] ExcludedExtensions =
		{
			".unity",
			".mp3",
			".wav",
			".fbx",
			".anim",
			".asset",
			".tga",
			".png",
			".ttf",
			".json"
		};

		private static Dictionary<Object, SerializedObject> m_CachedSerializedObjects
			= new Dictionary<Object, SerializedObject>();

		public static bool DoingAsyncWork
		{
			get { return m_Coroutine != null; }
		}

		public static float AsyncProgress
		{
			get { return (float)m_ObjectsSearchedAsync / m_ObjectsToSearchAsync; }
		}

		private static IEnumerator GetSerializedPropsAsync(Object referencedObject, IEnumerable<Object> searchObjects,
			IDictionary<Object, List<SerializedProperty>> foundReferences)
		{
			m_ObjectsSearchedAsync = 0;
			m_ObjectsToSearchAsync = searchObjects.Count();
			foreach (var obj in searchObjects.Where(o => o != null))
			{
				if (m_ObjectsSearchedAsync % MaxPropsToSearchPerFrame == 0)
				{
					// Yield after doing some work so we don't block the main thread too long.
					yield return null;
				}
				++m_ObjectsSearchedAsync;
				GetMatchingProps(referencedObject, obj, foundReferences);
			}
			StopAsyncOperation();
		}

		private static IEnumerator GetSerializedPropsInAssets(Object referencedObject,
			IDictionary<Object, List<SerializedProperty>> foundReferences)
		{
			var assets = AssetDatabase.GetAllAssetPaths();
			m_ObjectsSearchedAsync = 0;
			m_ObjectsToSearchAsync = assets.Length;
			int numAssetsSearched = 0;

			foreach (var assetPath in assets)
			{
				if (!ExcludedExtensions.Contains(Path.GetExtension(assetPath)))
				{
					var assetsAtPath = AssetDatabase.LoadAllAssetsAtPath(assetPath);
					foreach (var asset in assetsAtPath.Where(a => a != null))
					{
						GetMatchingProps(referencedObject, asset, foundReferences);
						++numAssetsSearched;
						if (numAssetsSearched % MaxAssetsToSearchPerFrame == 0)
						{
							// Yield after doing some work so we don't block the main thread too long.
							yield return null;
						}
					}
				}
				++m_ObjectsSearchedAsync;
			}
			StopAsyncOperation();
		}

		private static void GetMatchingProps(Object referencedObject, Object objectToSearch,
			IDictionary<Object, List<SerializedProperty>> propsContainer)
		{
			SerializedObject so;
			if (!m_CachedSerializedObjects.TryGetValue(objectToSearch, out so))
			{
				so = new SerializedObject(objectToSearch);
				m_CachedSerializedObjects.Add(objectToSearch, so);
			}
			so.UpdateIfDirtyOrScript();

			var it = so.GetIterator();
			if (!it.hasChildren) return;
			while (it.NextVisible(true))
			{
				if (it.propertyType == SerializedPropertyType.ObjectReference &&
					it.objectReferenceValue == referencedObject)
				{
					List<SerializedProperty> list;
					if (!propsContainer.TryGetValue(objectToSearch, out list))
					{
						list = new List<SerializedProperty>();
						propsContainer[objectToSearch] = list;
					}
					list.Add(it.Copy());
				}
			}
		}

		public static void FindReferencesAsync(Object referencedObject, IEnumerable<Object> searchObjects,
			IDictionary<Object, List<SerializedProperty>> foundReferences, System.Action callback)
		{
			// We can't use threading because the Unity api is not thread-safe.
			// The second best thing here to minimize blocking the main thread too much is using a coroutine implementation.
			StopAsyncOperation();
			CleanCachedSerializedObjects();
			m_Coroutine = new EditorCoroutine(
				GetSerializedPropsAsync(referencedObject, searchObjects, foundReferences), callback);
		}

		public static void FindReferencesInAssetsAsync(Object referencedObject,
			IDictionary<Object, List<SerializedProperty>> foundReferences, System.Action callback)
		{
			StopAsyncOperation();
			CleanCachedSerializedObjects();
			m_Coroutine = new EditorCoroutine(
				GetSerializedPropsInAssets(referencedObject, foundReferences), callback);
		}

		private static void CleanCachedSerializedObjects()
		{
			m_CachedSerializedObjects = m_CachedSerializedObjects
				.Where(pair => pair.Key != null)
				.ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		public static void StopAsyncOperation()
		{
			if (m_Coroutine != null)
			{
				m_Coroutine.Stop();
				m_Coroutine = null;
			}
		}
	}
}
