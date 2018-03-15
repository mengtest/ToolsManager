using System.Collections;
using UnityEditor;

namespace ReferenceViewer
{
	public class EditorCoroutine
	{
		private IEnumerator m_Enumerator;
		private bool m_Started;

		public event System.Action Done = delegate { };

		public EditorCoroutine(IEnumerator enumerator)
		{
			Initialise(enumerator);
		}

		public EditorCoroutine(IEnumerator enumerator, System.Action doneCallback)
		{
			Done += doneCallback;
			Initialise(enumerator);
		}

		private void Initialise(IEnumerator enumerator)
		{
			m_Enumerator = enumerator;
			Start();
		}

		private void Start()
		{
			m_Started = true;
			EditorApplication.update += Update;
		}

		public void Stop()
		{
			if (m_Started)
			{
				m_Started = false;
				EditorApplication.update -= Update;
				Done();
			}
		}

		private void Update()
		{
			if (!m_Enumerator.MoveNext())
			{
				Stop();
			}
		}
	}
}
