using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ezfun_resource;


public class PopupItem
{
    public EZFunWindowEnum m_WindowEnum;
    public object p1 = 0;
    public object p2 = 0;
}

public class PopupWindowManager
{
    private PopupWindowManager() { }

    public static PopupWindowManager instance = null;

    public static PopupWindowManager getinstance()
    {
        if (instance != null)
        {
            return instance;
        }
        else
        {
            instance = new PopupWindowManager();
            return instance;
        }
    }

    void init()
    {
    }

    List<PopupItem> m_PopupWindowList = new List<PopupItem>();

    EZFunWindowEnum m_currentWindow = 0;

    bool m_lock = false;

    public void Lock()
    {
        m_lock = true;
    }

    public void Unlock()
    {
        m_lock = false;
    }

    public bool PushWindow(PopupItem needShowWindow)
    {
        if (needShowWindow == null)
        {
            return false;
        }
      
        if (!m_lock)
        {
			if (m_PopupWindowList != null)
            {
				PopupItem findValue = m_PopupWindowList.Find((PopupItem item)=>{
					return item.m_WindowEnum == needShowWindow.m_WindowEnum;
				});

				if(findValue == null)
				{
					m_PopupWindowList.Add(needShowWindow);
				}
				
                if (needShowWindow.m_WindowEnum != m_currentWindow)
                {
                    TrigerWindow();
                }
            }
        }
        return true;
    }

    public bool PopWindow(PopupItem eraseWindow)
    {
        for (int i = 0; i < m_PopupWindowList.Count; i++)
        {
            if (m_PopupWindowList[i].m_WindowEnum == eraseWindow.m_WindowEnum)
            {
                if (m_currentWindow == m_PopupWindowList[i].m_WindowEnum)
				{
					m_currentWindow = 0;
				}
				
                m_PopupWindowList.Remove(m_PopupWindowList[i]); 
                TrigerWindow();
                return true ;
            }
        }
        return false;
    }

    void TrigerWindow()
    {
        for (int i = 0; i < m_PopupWindowList.Count; i++)
        {
            if (m_PopupWindowList[i].m_WindowEnum == m_currentWindow)
			{
				return;
			}
			
            WindowRoot.s_argc1 = (int)m_PopupWindowList[i].p1;
            m_currentWindow = m_PopupWindowList[i].m_WindowEnum;

            //AudioSys.Instance.PlayEffect("UI/UI_effect_2");
            EZFunWindowMgr.Instance.SetWindowStatus(m_PopupWindowList[i].m_WindowEnum, true, (int)m_PopupWindowList[i].p2);
            return;
        }

        if (m_PopupWindowList.Count == 0)
		{
			m_currentWindow = 0;
		}
    }

    public int GetPopWindowNum()
    {
        return m_PopupWindowList.Count;
    }
}
