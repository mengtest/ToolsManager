using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RendererSortAdapt : MonoBehaviour {
	private Renderer[] m_renderer;
	//original sort order
	private Dictionary<Renderer, int> m_oriRSO = new Dictionary<Renderer, int>();
	private UIPanel m_panel;
	// Use this for initialization
	void Start () {
		m_renderer = GetComponentsInChildren<Renderer> ();
		m_panel = GetComponentInParent<UIPanel> ();

//		m_panel.OnSortOrderChange = () => {
//		};

		if (m_renderer == null || m_renderer.Length <= 0) {
			Debug.LogError ("[RendererSortAdapt] No Renderer in children!!!");
			return;
		}

		for (int i = 0; i < m_renderer.Length; i++) {
			if (!m_oriRSO.ContainsKey (m_renderer [i])) {
				m_oriRSO.Add (m_renderer[i], m_renderer[i].sortingOrder);
			}
			m_renderer [i].sortingOrder = m_oriRSO [m_renderer [i]] + m_panel.sortingOrder + 1;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
