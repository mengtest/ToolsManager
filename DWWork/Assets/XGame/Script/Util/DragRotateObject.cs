using UnityEngine;
using System.Collections;

public class DragRotateObject : MonoBehaviour 
{
	public Vector2 m_rotateAxis = new Vector2(1, 0);
	private Vector3 m_prevPos;      
	private bool m_isPress;

	// Use this for initialization
	void Start () 
	{
		m_isPress = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(m_isPress)
		{
			if(Input.GetMouseButtonUp(0))
			{
				m_isPress = false;
				return;
			}
			else
			{
				var offset = m_prevPos - Input.mousePosition;
				transform.Rotate(Vector3.up * offset.x * m_rotateAxis.x, Space.World);                
				transform.Rotate(Vector3.right*offset.y * m_rotateAxis.y, Space.World);             
				m_prevPos = Input.mousePosition;
			}
		}
		else
		{
			if(Input.GetMouseButtonDown(0))
			{
				if(m_isPress == false)
				{
					m_isPress = true;
					m_prevPos = Input.mousePosition;
				}
			}
		}
	}
}
