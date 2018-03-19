/************************************************************
     File      : EZFunScollView.cs
     brief     : 横向列表，功能半完善，已测单行可行  
     author    : JanusLiu   janusliu@ezfun.cn
     version   : 1.0
     date      : 2014/11/3 15:48:52
     copyright : Copyright 2014 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

public delegate void ItemShowCallBack(Transform trans, int keyIndex = 0, int index = 0);

public delegate void InitBatchCallBack(Transform trans, int transIndex);

[LuaWrap]
public enum ScrollViewDirection
{
	SVD_Horizontal,
	SVD_Vertical,
}

public class CScrollConstruct
{
	public GameObject m_gb;
	public Vector2 m_scale;
	public int m_sideNumbers;
    public ItemShowCallBack m_itemShowFunc;
	public CScrollConstruct(GameObject gb, Vector2 scale, int sideNumbers, ItemShowCallBack itemShowFunc=null)
	{
		m_gb = gb;
		m_scale = scale;
		m_sideNumbers = sideNumbers;
		m_itemShowFunc = itemShowFunc;
	}
}

public class CScrollGet
{
	public int m_index;
	public int m_count;
	public InitBatchCallBack m_callBack = null;

	public CScrollGet(int index, int count, InitBatchCallBack callBack = null)
	{
		m_index = index;
		m_count = count;
		m_callBack = callBack;
	}
}

/// <summary>
/// 和ScrollView的dragCamera绑定在一起，用来稳定ScrollView的camera，不随ScrollView的缩放和位移变化
/// </summary>
public class EZFunScrollViewStableCamera : MonoBehaviour
{
    private Vector3 m_pos;
    private Vector3 m_lastPos;

    private Transform m_parentTrans;


    private Vector3 m_camreaPos;
    public float m_scale = 4;
    private Rect m_rect;
   
    public void MarkData()
    {
        m_parentTrans = transform.parent;
        m_pos = m_parentTrans.position;
        m_camreaPos = transform.position;
        Camera m_cam = GetComponent<Camera>();
         m_rect = m_cam.rect;
    }

    void LateUpdate()
    {
        if ((m_pos-m_parentTrans.position).sqrMagnitude > 0.000001f || m_lastPos != m_parentTrans.position)
        {
            m_lastPos = m_parentTrans.position;
            Vector3 delta = m_parentTrans.position - m_pos;
            delta *= EZFunWindowMgr.Instance.GetScreenHeight()/2;
            delta.x /= EZFunWindowMgr.Instance.GetScreenWidth();
            delta.y /= (EZFunWindowMgr.Instance.GetScreenHeight());
            Camera m_cam = GetComponent<Camera>();
            // var rect = m_cam.rect;
            var rect = m_rect;
            rect.x += delta.x;
            rect.y += delta.y;
            var pos = Vector3.zero;
            if (rect.x < 0)
            {
                //rect.width = rect.width + rect.x/2;
                pos.x = -rect.x * EZFunWindowMgr.Instance.GetScreenWidth();
                rect.x = 0;
            }
            else if (rect.x + rect.width > 1)
            {
                pos.x = -(rect.x + rect.width - 1) * EZFunWindowMgr.Instance.GetScreenWidth();
                rect.x = 1 - rect.width;
            }
            if (rect.y < 0)
            {
                //rect.width = rect.width + rect.x/2;
                pos.y = -rect.y * EZFunWindowMgr.Instance.GetScreenHeight();
                rect.y = 0;
            }
            else if (rect.y + rect.height > 1)
            {
                pos.y = -(rect.y + rect.height - 1) * EZFunWindowMgr.Instance.GetScreenHeight();
                rect.y = 1 - rect.height;
            }
            m_cam.transform.localPosition = pos;
            m_cam.rect = rect;
            //transform.position = m_camreaPos;
        }
    }
}

[LuaWrap]
public class EZFunScollView{

	//params
	private Transform m_rootTrans;
	public Transform m_barTrans = null;
	private List<GameObject> m_gbList = new List<GameObject>();
	public ScrollViewDirection m_direction;
	private List<Vector2> m_scaleList = new List<Vector2>();
	private List<int> m_sideNumbersList = new List<int>();
	private List<ItemShowCallBack> m_itemShowFunList = new List<ItemShowCallBack>();
    private string m_luaShowFunCB = "";
    private LuaFunction m_luaShowFunc = null;

	private Transform m_windowRootTrans;
	//private Transform m_draggableGBTrans;
    private int m_scrollViewId = 1;

	private Dictionary<int, List<Transform>> m_canUseTransListHash = new Dictionary<int, List<Transform>>();
	private Dictionary<int, List<Transform>> m_hasUsedTransListHash = new Dictionary<int, List<Transform>>();
    private int m_hasUsedTransListHashCount = 0;
	private Vector3 m_cameraStartPos = Vector3.zero;
	private List<Vector3> m_itemStartPos = new List<Vector3>();
	private Vector3 m_lastItemPos = Vector3.zero;

	public bool m_noClickWhileDrag = true;
    public List<Transform> m_SelectTrans = null;
	//进度条
	private Vector2 m_rootBoxSize;
	private Rect m_dragBox;
    Dictionary<int, List<Transform>> dicTransList = new Dictionary<int, List<Transform>>();
	//自适应排列
	private bool m_isSelfAdapt = false;
	private Rect m_originalMaxRect;
    private Rect m_MaxRect;
	private Rect m_currLineMaxRect;
	//private Rect m_cameraRect;
	private Transform m_itemParentTrans = null;
    public int m_layer;
	public bool m_hasShowd = false;
	public bool m_needClearName = true;

    private UIScrollView m_scrollView;

    private ScrollViewDirection m_pageScrollDir = ScrollViewDirection.SVD_Horizontal;
    private int m_pageCount;
    //这个camera是这个window的camera，主要是因为当scrollview在uianchor节点下时，因为uianchor起作用是在start中，所以在在这儿提供一个camera给uianchor强制设置位置用
    public Camera m_parentCamera;

    private UICenterOnChild m_centerOnChild;

    //获得层的方法类型  type 1: 只提供给窗口自己使用；2，同一个窗口的会共用一个layer；3，完全属于自己的layer
    public int m_layerGetType = 2;
    /// <summary>
    /// 用来处理是否跟随相机父节点移动
    /// </summary>
    public bool m_isStableCamera;
    /// <summary>
    ///     
    ///自适应的话，scale是子item边框的间隔，不自适应的话，scale是子item中心点的间隔
    /// </summary>
    /// <param name="rootTrans"></param>
    /// <param name="gb"></param>
    /// <param name="scale"></param>
    /// <param name="direction"></param>
    /// <param name="sideNumbers"></param>
    /// <param name="itemShowFun"></param>
    /// <param name="barTrans"></param>
    /// <param name="is_selfAdapt"></param>
    /// <param name="type">这个type表示居中效果，1参考刀锋1中翅膀界面 刀锋2中组队设置的等级(一般只能一行或者一列)，2多行,参照刀锋1的掠夺宝石的页面</param>
    public EZFunScollView(Transform rootTrans, GameObject gb, Vector2 scale, ScrollViewDirection direction,
	                      int sideNumbers,ItemShowCallBack itemShowFun , Transform barTrans = null, bool is_selfAdapt = false,int type = 0)
	{
		Init(rootTrans, gb, scale, direction, sideNumbers, itemShowFun, barTrans, is_selfAdapt,type);
	}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootTrans"></param>
    /// <param name="gb"></param>
    /// <param name="scale"></param>
    /// <param name="direction"></param>
    /// <param name="sideNumbers"></param>
    /// <param name="type">这个type表示居中效果，1参考刀锋1中翅膀界面 刀锋2中组队设置的等级(一般只能一行或者一列)，2多行,参照刀锋1的掠夺宝石的页面</param>
    public EZFunScollView(Transform rootTrans, GameObject gb, Vector2 scale, ScrollViewDirection direction = ScrollViewDirection.SVD_Vertical,int sideNumbers = 1, int type = 0)
	{
		Init(rootTrans, gb, scale, direction, sideNumbers, null, null, false, type);
	}
    /// <summary>
    /// 可设置深度
    /// </summary>
    /// <param name="rootTrans"></param>
    /// <param name="gb"></param>
    /// <param name="scale"></param>
    /// <param name="direction"></param>
    /// <param name="sideNumbers"></param>
    /// <param name="type">这个type表示居中效果，1参考刀锋1中翅膀界面 刀锋2中组队设置的等级(一般只能一行或者一列)，2多行,参照刀锋1的掠夺宝石的页面</param>
    public EZFunScollView(Transform rootTrans, GameObject gb, Vector2 scale, int depth, ScrollViewDirection direction = ScrollViewDirection.SVD_Vertical, int sideNumbers = 1, int type = 0)
    {
        Init(rootTrans, gb, scale, direction, sideNumbers, null, null, false, type, depth);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootTrans"></param>
    /// <param name="gb"></param>
    /// <param name="scale"></param>
    /// <param name="direction"></param>
    /// <param name="sideNumbers"></param>
    /// <param name="itemShowFun"></param>
    /// <param name="type">这个type表示居中效果，1参考刀锋1中翅膀界面 刀锋2中组队设置的等级(一般只能一行或者一列)，2多行,参照刀锋1的掠夺宝石的页面</param>
    public EZFunScollView(Transform rootTrans, GameObject gb, Vector2 scale, ScrollViewDirection direction,int sideNumbers,
                          ItemShowCallBack itemShowFun, int type)
    {
        Init(rootTrans, gb, scale, direction, sideNumbers, itemShowFun, null, false, type);
    }
   
	public EZFunScollView(Transform rootTrans, GameObject gb, Vector2 scale, ScrollViewDirection direction, int sideNumbers, Transform barTrans)
	{
		Init(rootTrans, gb, scale, direction, sideNumbers, null, barTrans, false);
	}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootTrans"></param>
    /// <param name="ob"></param>
    /// <param name="scale"></param>
    /// <param name="direction"></param>
    /// <param name="sideNumbers"></param>
    /// <param name="barTrans"></param>
	public EZFunScollView(Transform rootTrans, OBContainer ob, Vector2 scale, ScrollViewDirection direction, int sideNumbers, Transform barTrans)
	{
		Init(rootTrans, (GameObject)ob.m_object, scale, direction, sideNumbers, null, barTrans, false);
	}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rootTrans"></param>
    /// <param name="ob"></param>
    /// <param name="scale"></param>
    /// <param name="direction"></param>
    /// <param name="sideNumbers"></param>
	public EZFunScollView(Transform rootTrans, OBContainer ob, Vector2 scale, ScrollViewDirection direction = ScrollViewDirection.SVD_Vertical,int sideNumbers = 1)
	{
		Init(rootTrans, (GameObject)ob.m_object, scale, direction, sideNumbers, null, null, false, 0);
	}

    #region convenient for lua
    //can selfAdapt
    public EZFunScollView(Transform rootTrans, GameObject ob, Vector2 scale, ScrollViewDirection direction, int sideNumbers, bool is_selfAdapt)
    {
        Init(rootTrans, ob, scale, direction, sideNumbers, null, null, is_selfAdapt);
    }

    //sup different item 
    public EZFunScollView(Transform rootTrans,  ScrollViewDirection direction, bool is_selfAdapt)
    {
        m_rootTrans = rootTrans;
        m_barTrans = null;
        m_isSelfAdapt = is_selfAdapt;
        m_gbList.Clear();
        m_scaleList.Clear();
        m_sideNumbersList.Clear();
        m_canUseTransListHash.Clear();
        m_hasUsedTransListHash.Clear();
        m_itemShowFunList.Clear();
        m_hasUsedTransListHashCount = 0;
        m_direction = direction;
        m_windowRootTrans = FindWindowTrans();
        var wnd = m_windowRootTrans.GetComponent<WindowRoot>();
        if (wnd != null)
        {
            m_scrollViewId = wnd.m_scrollViewCnt++;
        }
        InitDraggableCamera();
        SetDepth();
    }

    public void AddCScrollConstruct(GameObject gb, Vector2 scale, int sideNumbers)
    {
        m_gbList.Add(gb);
        m_scaleList.Add(scale);
        m_sideNumbersList.Add(sideNumbers);
        int ind = m_gbList.Count - 1;
        m_canUseTransListHash.Add(ind, new List<Transform>());
        m_hasUsedTransListHash.Add(ind, new List<Transform>());
        m_hasUsedTransListHashCount++;

        BoxCollider boxCollider = m_rootTrans.GetComponent<BoxCollider>();
        if(m_direction == ScrollViewDirection.SVD_Horizontal)
        {
            Vector3 pos = Vector3.zero;
            int tInd = m_scaleList.Count - 1;
            pos.x = -boxCollider.size.x / 2 + m_scaleList[tInd].x / 2 + boxCollider.center.x;
            if (m_scaleList[tInd].y != 0)
            {
                pos.y = boxCollider.size.y / 2 - m_scaleList[tInd].y / 2 + boxCollider.center.y;
            }
            m_itemStartPos.Add(pos);
        }
        else
        {
            Vector3 pos = Vector3.zero;
            int tInd = m_scaleList.Count - 1;
            pos.y = boxCollider.size.y / 2 - m_scaleList[tInd].y / 2 + boxCollider.center.y;
            if (m_scaleList[tInd].x != 0)
			{
                pos.x = -boxCollider.size.x / 2 + m_scaleList[tInd].x / 2 + boxCollider.center.x;
	        }

	        m_itemStartPos.Add(pos);
        }
    }

    public void SetLuaShowCallBack(string luaFunPath)
    {
        m_luaShowFunCB = luaFunPath;
    }

    public void SetLuaShowCallBack(LuaFunction luaFuncction)
    {
        m_luaShowFunc = luaFuncction;
    }



    //for lua
    public void GetScrollView(string getDataFuncPath, bool needReposition = false)
    {
        object[] data = WindowBaseLua.m_luaMgr.CallLuaFunction(getDataFuncPath);
        if (data.Length <= 0)
        {
            return;
        }

        LuaInterface.LuaTable ob = data[0] as LuaInterface.LuaTable;
        Hashtable ta = SystemBaseLua.GetLuaTableData(ob);
        List<CScrollGet> getList = new List<CScrollGet>();
        foreach (var va in ta.Values)
        {
            Hashtable tab = (Hashtable)va;
            double ind = (double)tab["m_index"];
            double cou = (double)tab["m_count"];
            CScrollGet tSG = new CScrollGet((int)ind, (int)cou);
            getList.Add(tSG);
        }
        GetScrollView(getList, needReposition);
    }
#endregion 

    void Init(Transform rootTrans, GameObject gb, Vector2 scale, ScrollViewDirection direction,
	          int sideNumbers,ItemShowCallBack itemShowFun , Transform barTrans = null, bool is_selfAdapt = false, int type = 0, int depth = 100)
	{
		m_rootTrans = rootTrans;
		m_barTrans = barTrans;
		m_isSelfAdapt = is_selfAdapt;
		m_gbList.Clear();
		m_gbList.Add(gb);
		m_scaleList.Clear();
        if (gb != null)
        {
            var boxCollider = gb.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                scale = boxCollider.size;
            }
        }
		m_scaleList.Add(scale);
		m_sideNumbersList.Clear();
		m_sideNumbersList.Add(sideNumbers);
		m_itemShowFunList.Clear();
		m_itemShowFunList.Add(itemShowFun);
		
		m_canUseTransListHash.Add(0, new List<Transform>());
		m_hasUsedTransListHash.Add(0, new List<Transform>());
        m_hasUsedTransListHashCount = 1;
        m_specialScrollType = type;
		m_direction = direction;
		m_windowRootTrans = FindWindowTrans();
        var wnd = m_windowRootTrans.GetComponent<WindowRoot>();
        if (wnd != null)
        {
            m_scrollViewId = wnd.m_scrollViewCnt++;
        }
        InitDraggableCamera(depth);
		SetDepth();
	}

    /// <summary>
    /// 这就是在构造函数中初始化的烦恼点咯，必须不停的加参数
    /// </summary>
    /// <param name="rootTrans"></param>
    /// <param name="scrollContructList"></param>
    /// <param name="direction"></param>
    /// <param name="barTrans"></param>
    /// <param name="is_selfAdapt"></param>
    /// <param name="parentCamera"></param>
    /// <param name="isStableCamera">这个字段参照m_isStableCamera</param>
	public EZFunScollView(Transform rootTrans, List<CScrollConstruct> scrollContructList, ScrollViewDirection direction = ScrollViewDirection.SVD_Vertical, Transform barTrans = null, 
        bool is_selfAdapt = false,Camera parentCamera =null, bool isStableCamera = false, int iType = 0, int iDepth = 100)
	{
        m_isStableCamera = isStableCamera;
        m_parentCamera = parentCamera;
        m_rootTrans = rootTrans;
		m_barTrans = barTrans;
		m_isSelfAdapt = is_selfAdapt;
		m_gbList.Clear();
		m_scaleList.Clear();
		m_sideNumbersList.Clear();
		m_canUseTransListHash.Clear();
		m_hasUsedTransListHash.Clear();
		m_itemShowFunList.Clear();
		for(int i = 0; i < scrollContructList.Count; i ++)
		{
			CScrollConstruct csc = scrollContructList[i];
			m_gbList.Add(csc.m_gb);
			m_scaleList.Add(csc.m_scale);
			m_itemShowFunList.Add(csc.m_itemShowFunc);
			m_sideNumbersList.Add(csc.m_sideNumbers);
			m_canUseTransListHash.Add(i, new List<Transform>());
			m_hasUsedTransListHash.Add(i, new List<Transform>());
		}
        m_hasUsedTransListHashCount = scrollContructList.Count;
		m_direction = direction;
		m_windowRootTrans = FindWindowTrans();
        var wnd = m_windowRootTrans.GetComponent<WindowRoot>();
        if (wnd != null)
        {
            m_scrollViewId = wnd.m_scrollViewCnt++;
        }

        m_specialScrollType = iType;


        InitDraggableCamera(iDepth);
		SetDepth();
	}

    private EZFunScollView()
    {
    }
    public static EZFunScollView CreateFourceFirstAdaptSCR(Transform rootTrans, List<CScrollConstruct> scrollContructList, ScrollViewDirection direction = ScrollViewDirection.SVD_Vertical, Transform barTrans = null, bool is_selfAdapt = false)
    {
        var scr = new EZFunScollView();
        scr.m_rootTrans = rootTrans;
        scr.m_barTrans = barTrans;
        scr.m_isSelfAdapt = is_selfAdapt;
        scr.m_specialScrollType = 0;
        scr.m_gbList.Clear();
        scr.m_scaleList.Clear();
        scr.m_sideNumbersList.Clear();
        scr.m_canUseTransListHash.Clear();
        scr.m_hasUsedTransListHash.Clear();
        scr.m_itemShowFunList.Clear();
        for (int i = 0; i < scrollContructList.Count; i++)
        {
            CScrollConstruct csc = scrollContructList[i];
            scr.m_gbList.Add(csc.m_gb);
            scr.m_scaleList.Add(csc.m_scale);
            scr.m_itemShowFunList.Add(csc.m_itemShowFunc);
            scr.m_sideNumbersList.Add(csc.m_sideNumbers);
            scr.m_canUseTransListHash.Add(i, new List<Transform>());
            scr.m_hasUsedTransListHash.Add(i, new List<Transform>());
        }
        scr.m_hasUsedTransListHashCount = scrollContructList.Count;
        scr.m_direction = direction;
        scr.m_windowRootTrans = scr.FindWindowTrans();
        var wnd = scr.m_windowRootTrans.GetComponent<WindowRoot>();
        if (wnd != null)
        {
            scr.m_scrollViewId = wnd.m_scrollViewCnt++;
        }
        scr.InitDraggableCamera();
        //var camera = scr.getDraggableCamera() as EZFunDraggableCamera;
        //camera.m_bFirstAdpt = true;
        //BoxCollider boxCollider = scr.m_rootTrans.GetComponent<BoxCollider>();
        //scr.m_cameraStartPos = new Vector3(boxCollider.center.x, boxCollider.center.y, 0);
        //camera.transform.localPosition = scr.m_cameraStartPos;
        scr.SetDepth();
        return scr;
    }

    

    //参数：refrenceName表示，列表项位置适应是按照该refrenceName对应的transform 的 rect作为基准
    private string m_refrenceName = null;
    public void SetItemRefenceTrans( string refrenceName )
    {
        if (null == m_rootTrans)
        {
            //Debug.LogError(" must set up scrollview first");
            return;
        }
        m_refrenceName = refrenceName;
    }

	public Dictionary<int, List<Transform>> GetScrollView(List<CScrollGet> getList, bool needRepositon = false)
	{
        dicTransList.Clear();
        Clear();

        if (needRepositon)
		{
            m_rootTrans.localPosition = m_cameraStartPos;
            m_rootTrans.GetComponent<UIPanel>().clipOffset = m_cameraStartPos;
        }

        hasInit = false;
        //skip用来判断是否有getList.Count为0的情况，此时相应的lastIndex应该变为getList[i-1-skipNum].m_index。
        //加这个处理是为了有些情况下item的scale不同，导致两个item之间的空隙过大
        bool skip = false;
        int skipNum = 0;
		for(int i = 0; i < getList.Count; i ++)
		{
			if(getList[i].m_count <= 0)
			{
                skip = true;
                skipNum++;
                continue;
			}

			int keyIndex = getList[i].m_index;
			int lastIndex = 0;
			if(i != 0)
			{
				lastIndex = getList[i-1-skipNum].m_index;
                skip = false;
                skipNum = 0;
			}
			InitLastItemPos(keyIndex, lastIndex);
			List<Transform> transList = InitScrollView(keyIndex, getList[i].m_count, getList[i].m_callBack);
		
			dicTransList.Add(i, transList);
		}

		SetDragBox(getList);

		SetDepth();
        return dicTransList;
	}

	private int m_eachBatchNum = 2;
	int m_req = 0;
	public void InitScrollViewInBatch(List<CScrollGet> getList, int eachBatchNum, bool needRepositon = false)
	{
		if(needRepositon)
		{
            m_rootTrans.localPosition = m_cameraStartPos;
            m_rootTrans.GetComponent<UIPanel>().clipOffset = m_cameraStartPos;
        }

		m_eachBatchNum = eachBatchNum;

		Clear();

		hasInit = false;

		int totalCount = 0;
		for(int i = 0; i < getList.Count; i ++)
		{
			totalCount += getList[i].m_count;
		}

		m_req ++;
		InitEachBatch(getList, m_eachBatchNum, totalCount, m_req);
			
		SetDepth();
	}

	private void InitEachBatch(List<CScrollGet> getList, int needCount, int leftCount, int req)
	{
		if(req != m_req)
		{
			return;
		}

		int getListIndex = getList.Count - 1;
		int count = 0;
		int offset = 0;
		for(; getListIndex >= 0; getListIndex --)
		{
			count += getList[getListIndex].m_count;
			if(count >= leftCount)
			{
				needCount = getList[getListIndex].m_count - (count - leftCount);
				offset = getList[getListIndex].m_count - needCount;
				if(needCount > m_eachBatchNum)
				{
					needCount = m_eachBatchNum;
				}
				leftCount -= needCount;
				break;
			}
		}

		int keyIndex = getList[getListIndex].m_index;
		int lastIndex = 0;
		if(getListIndex != 0 && offset == 0)
		{
			lastIndex = getList[getListIndex - 1].m_index;
		}
		else
		{
			lastIndex = getList[getListIndex].m_index;
		}

		InitLastItemPos(keyIndex, lastIndex);

		List<Transform> transList = InitScrollView(keyIndex, needCount, null);
		for(int transIndex = 0; transIndex < transList.Count; transIndex ++)
		{
			WindowRoot.SetLayer(transList[transIndex].gameObject, m_InBatchLayer);
			getList[getListIndex].m_callBack(transList[transIndex],offset + transIndex);
		}

		if(leftCount > 0)
		{
			TimerSys.Instance.AddTimerEventByLeftTime(()=>{
				InitEachBatch(getList, m_eachBatchNum, leftCount, req);
			}, Time.deltaTime/4 * needCount);
		}
	}
	
	public void setUICameraEnable(bool isActive)
	{
	}

	bool hasInit = false;
	private void InitLastItemPos(int keyIndex,int lastIndex)
	{
		if(!hasInit)
		{
			m_lastItemPos = m_itemStartPos[keyIndex];
			hasInit = true;
		}
		else
		{
			if(m_direction == ScrollViewDirection.SVD_Vertical)
			{
				m_lastItemPos.x = m_itemStartPos[keyIndex].x;
				m_lastItemPos.y -= m_scaleList[lastIndex].y/2;
				m_lastItemPos.y -= m_scaleList[keyIndex].y/2;
			}
			else if(m_direction == ScrollViewDirection.SVD_Horizontal)
			{
				m_lastItemPos.y = m_itemStartPos[keyIndex].y;
				m_lastItemPos.x += m_scaleList[lastIndex].x/2;
				m_lastItemPos.x += m_scaleList[keyIndex].x/2;
			}
		}
	}

	private List<Transform> InitScrollView(int keyIndex, int count, InitBatchCallBack callBack)
	{
		List<Transform> transList = new List<Transform>();
		int index = 0;
		while(count > 0)
		{
			Transform trans = null;
			if(m_canUseTransListHash[keyIndex].Count > 0)
			{
				trans = m_canUseTransListHash[keyIndex][0];
				m_canUseTransListHash[keyIndex].RemoveAt(0);
			}
			else
			{
				GameObject gb = (GameObject)MonoBehaviour.Instantiate(m_gbList[keyIndex]);
				trans = gb.transform; 
			}

			m_hasUsedTransListHash[keyIndex].Add(trans);

            //以下几行顺序很重要，不设active设不了label（回调函数m_itemShowFunList[?]中），不设label之类的，算边框（GetRect）无意义
            EZFunUITools.SetActive(trans.gameObject, true);
			trans.parent = m_itemParentTrans;
			trans.localScale = Vector3.one;
			trans.localPosition = Vector3.zero;

            if (m_specialScrollType == 0)
            {
                if(keyIndex < m_itemShowFunList.Count)
                {
                    if(m_itemShowFunList[keyIndex] != null)
                    {
                        m_itemShowFunList[keyIndex](trans, keyIndex, index);
                    }
                }
            }
            if (!string.IsNullOrEmpty(m_luaShowFunCB))
            {
                WindowBaseLua.m_luaMgr.CallLuaFunction(m_luaShowFunCB, trans, keyIndex, index);
            }
            else if (m_luaShowFunc != null)
            {
                m_luaShowFunc.Call3Args(trans, keyIndex, index);
            }

            if (callBack != null)
			{
				callBack(trans, index);
			}

			if (m_isSelfAdapt)
			{
				SetTrans(trans, index, keyIndex);
			}
			else 
			{
                bool nextLine = (index > 0 && index % m_sideNumbersList[keyIndex] == 0);
                //桉页排
                if (m_specialScrollType == 3 && m_pageCount > 0)
                {
                    bool nextPage = (index > 0 && index % m_pageCount == 0);
                    SetTrans(trans, index, nextLine, nextPage, keyIndex);
                }
                else
                {
                    SetTrans(trans, index, nextLine, keyIndex);
                }
     
            }
			
			index ++;
			--count;
			transList.Add(trans);
		}

		return transList;
	}

    private List<CScrollGet> getlist = null;
	public List<Transform> GetScrollView(int count, bool needRePosition = false, int backup_count = 0)
	{
		if(needRePosition)
		{
            m_rootTrans.localPosition = m_cameraStartPos;
            m_rootTrans.GetComponent<UIPanel>().clipOffset = m_cameraStartPos;
            //SpringPanel sp = m_rootTrans.GetComponent<SpringPanel>();
            //if (sp != null) sp.target = m_rootTrans.position;
        }

        if (getlist == null)
        {
             getlist= new List<CScrollGet>();
             getlist.Add(new CScrollGet(0, 0));
        }
		Clear();

		if (!m_isSelfAdapt)
		{
			m_lastItemPos = m_itemStartPos[0];
		}

        InitScrollView(0, count + backup_count, null);
        getlist[0].m_count = count;
		SetDragBox(getlist);
		SetDepth();

        if (m_specialScrollType == 1)
        {
            UIButtonMessage btn = null;
            for(int i = 0; i < m_hasUsedTransListHash[0].Count; ++i)
            {
                btn = m_hasUsedTransListHash[0][i].GetComponent<UIButtonMessage>();

                if (btn != null)
                {
                    btn.functionName = "JumpTo";
                    btn.target = m_rootTrans.gameObject;
                }
            }
        }
        else if (m_specialScrollType == 2)
        {
            UIButtonMessage btn = null;
            for(int i = 0; i < m_hasUsedTransListHash[0].Count; ++i)
            {
                btn = m_hasUsedTransListHash[0][i].GetComponent<UIButtonMessage>();

                if (btn != null)
                {
                    btn.functionName = "JumpTo";
                    btn.target = m_rootTrans.gameObject;
                }
            }
        }

		return m_hasUsedTransListHash[0];
	}

	public void Clear()
	{
		if (m_isSelfAdapt)
		{
			m_MaxRect = m_originalMaxRect;
			ResetCurrLineRect();
		}

        for (int j = 0; j < m_hasUsedTransListHashCount; j++)
		{
            List<Transform> transList = m_hasUsedTransListHash[j];
			for(int i = 0; i < transList.Count; i ++)
			{
                m_canUseTransListHash[j].Add(transList[i]);
                EZFunUITools.SetActive(transList[i].gameObject, false);
				if(m_needClearName)
				{
					transList[i].name = "scrollViewItemTmp";
				}
			}
			transList.Clear();
		}

        //if (m_specialScrollType == 3 && m_draggableCamera is EZFunDraggableCameraPage)
        //{
        //    (m_draggableCamera as EZFunDraggableCameraPage).nowPage = 0;
        //}
    }

	public void Destory()
	{
		m_gbList.Clear();
		m_canUseTransListHash.Clear();
		m_hasUsedTransListHash.Clear();
        m_hasUsedTransListHashCount = 0;
	}

//    float m_scaleFactor = 1.2f;
    public void UpdateTransform()
    {
        Transform scale_node = null;
        for(int i = 0; i < m_hasUsedTransListHash[0].Count; ++i)
        {
            //Vector3 dis = m_hasUsedTransListHash[0][i].position - m_rootTrans.position;
            Vector3 dis = m_hasUsedTransListHash[0][i].position + m_rootTrans.position;
            float ratio = 0f, x = 0f;

            if (m_direction == ScrollViewDirection.SVD_Vertical)
            {
                x = Mathf.Abs(dis.y) * 320f / m_scaleList[0].y;
                ratio = -0.15f * x + 0.9f;
            }
            else if (m_direction == ScrollViewDirection.SVD_Horizontal)
            {
                x = Mathf.Abs(dis.x) * 320f / m_scaleList[0].x;
                ratio = -0.15f * x + 0.9f;
            }

            if (ratio < 0.1f)
            {
                ratio = 0.1f;
            }

            scale_node = m_hasUsedTransListHash[0][i].GetChild(0);
            scale_node.localScale = Vector3.one * ratio;
        }
    }

    public void UpdateSpacing()
    {
        for(int i = 0; i < m_hasUsedTransListHash[0].Count; ++i)
        {
            Vector3 dis = m_hasUsedTransListHash[0][i].position - m_rootTrans.position;
            float ratio = 0f, x = 0f;

            if (m_direction == ScrollViewDirection.SVD_Horizontal)
            {
                x = Mathf.Abs(dis.x) * 320f / m_scaleList[0].x;
                ratio = -0.15f * x + 1f;

                var transNode = m_hasUsedTransListHash[0][i].GetChild(0);

                int dir = 0;
                if (m_hasUsedTransListHash[0][i].position.x < m_rootTrans.position.x)
                {
                    dir = 1;
                }
                else if (m_hasUsedTransListHash[0][i].position.x > m_rootTrans.position.x)
                {
                    dir = -1;
                }
                else
                {
                    dir = 0;
                }

                Vector3 tempPos = transNode.localPosition;
                tempPos.x = 240 * (1 - ratio) * dir;
                transNode.localPosition = tempPos;
            }
        }
    }

    public void UpdateProgressBar()
	{
		if (m_barTrans != null)
		{
			UIScrollBar item = m_barTrans.GetComponent<UIScrollBar>();

			if (m_direction == ScrollViewDirection.SVD_Vertical)
			{
				item.barSize = 1f / (m_dragBox.height / m_rootBoxSize.y + 1);
				item.value = (m_dragBox.y - m_rootTrans.localPosition.y) / m_dragBox.height;
				if (item.value > 1)
					item.value = 1;
				if (item.value < 0)
					item.value = 0;
			}
			else 
			{
				item.barSize = 1f / (m_dragBox.width / m_rootBoxSize.x + 1);
				item.value = (m_rootTrans.localPosition.x - m_dragBox.x) / m_dragBox.width;
				if (item.value > 1)
					item.value = 1;
				if (item.value < 0)
					item.value = 0;
			}
		}
	}
	
	#region tool
	Transform FindWindowTrans()
	{
        if (m_rootTrans == null)
            return null;
		Transform trans = m_rootTrans;
		while(!trans.name.Contains("ui_window") && trans.parent != null)
		{
			trans = trans.parent;
		}

		if(trans != null)
		{
			WindowRoot windowRoot = trans.GetComponent<WindowRoot>();
			if(windowRoot != null)
			{
				windowRoot.AddScrollView(this);
			}
		}

		return trans;
	}

	public void SetDragCamera(Transform trans)
	{
        UIDragScrollView dragCamera = EZFunTools.GetOrAddComponent<UIDragScrollView>(trans.gameObject);
        //dragCamera.draggableCamera = m_draggableCamera;
        dragCamera.isDisableNeedPress = false;
        dragCamera.scrollView = m_scrollView;
        //所有box都加上dragcamera，有可能是按钮或者输入框等
        BoxCollider[] box_list = trans.GetComponentsInChildren<BoxCollider>();
        for(int i = 0; i < box_list.Length; ++i)
		{
			dragCamera = EZFunTools.GetOrAddComponent<UIDragScrollView>(box_list[i].gameObject);
            dragCamera.isDisableNeedPress = false;
            dragCamera.scrollView = m_scrollView;
		}
		
		if(m_noClickWhileDrag)
		{
            UIButtonMessage btnMessage = trans.GetComponent(typeof(UIButtonMessage)) as UIButtonMessage;// EZFunTools.GetComponent<UIButtonMessage>(trans.gameObject);

			if(btnMessage == null)
			{
				btnMessage = trans.gameObject.GetComponentInChildren<UIButtonMessage>();
			}

            if(btnMessage != null)
			{
                //btnMessage.trigger = UIButtonMessage.Trigger.OnTrueClick;
				btnMessage.mTweenScaleOpen = false;
			}
        }
	}

	void SetTrans(Transform trans, int index, bool nextLine, int keyIndex = 0)
	{
		Vector3 pos = Vector3.zero;
		if(m_direction == ScrollViewDirection.SVD_Vertical)
		{
			pos.x = (index % m_sideNumbersList[keyIndex])*m_scaleList[keyIndex].x + m_lastItemPos.x;
			if(nextLine)
			{
				m_lastItemPos.y -= m_scaleList[keyIndex].y;
			}
			pos.y = m_lastItemPos.y;
		}
		else
		{
			pos.y = -(index % m_sideNumbersList[keyIndex])*m_scaleList[keyIndex].y + m_lastItemPos.y;
			if(nextLine)
			{
				m_lastItemPos.x += m_scaleList[keyIndex].x;
			}
			pos.x = m_lastItemPos.x;
		}
		trans.localPosition = pos;

		SetDragCamera(trans);
	}

    //策划需求 商城 要横滑 但是要横排 按页排
    void SetTrans(Transform trans, int index, bool nextLine,bool nextPage, int keyIndex = 0)
    {
        Vector3 pos = Vector3.zero;
        if (m_pageScrollDir == ScrollViewDirection.SVD_Vertical)
        {
            if (nextPage)
            {
                m_lastItemPos.x += m_scaleList[keyIndex].x * m_sideNumbersList[keyIndex];
            }
            pos.x = (index % m_sideNumbersList[keyIndex]) * m_scaleList[keyIndex].x + m_lastItemPos.x;

            if (nextPage)
            {
                m_lastItemPos.y = m_itemStartPos[0].y;
            }
            else if (nextLine)
            {
                m_lastItemPos.y -= m_scaleList[keyIndex].y;
            }

            pos.y = m_lastItemPos.y;
        }
        else
        {
            if (nextPage)
            {
                m_lastItemPos.y -= m_scaleList[keyIndex].y * m_sideNumbersList[keyIndex];
            }
            pos.y = - (index % m_sideNumbersList[keyIndex]) * m_scaleList[keyIndex].y + m_lastItemPos.y;

            if (nextPage)
            {
                m_lastItemPos.x = m_itemStartPos[0].x;
            }
            else if (nextLine)
            {
                m_lastItemPos.x += m_scaleList[keyIndex].x;
            }
            pos.x = m_lastItemPos.x;
        }
        trans.localPosition = pos;

        SetDragCamera(trans);
    }

    public void SetScrollDicInPage(ScrollViewDirection pageScrollDir, int pageCount)
    {
        m_pageScrollDir = pageScrollDir;
        m_pageCount = pageCount;
    }

    int m_specialScrollType = 0;

    public void ResetDragCameraRect()
    {
        InitDraggableCamera();
    }

	void InitDraggableCamera(int iDepth = 100)
	{
        var archors = m_rootTrans.GetComponentsInParent<UIAnchor>();
        for (int i = archors.Length - 1; i >= 0; i--)
        {
            Camera cam = null;
            if (EZFunWindowMgr.Instance.m_cameraRootTrans != null && EZFunWindowMgr.Instance.m_cameraRootTrans.childCount > 0 && EZFunWindowMgr.Instance.m_cameraRootTrans.GetChild(0).GetComponent<Camera>()!=null)
            {
                cam = EZFunWindowMgr.Instance.m_cameraRootTrans.GetChild(0).GetComponent<Camera>();
            }
            archors[i].uiCamera = cam;
            archors[i].ForceUpdate();
        }
        GameObject itemParentGB = null;
        if (m_rootTrans.Find(m_rootTrans.name) != null)
        {
            itemParentGB = m_rootTrans.Find(m_rootTrans.name).gameObject;
        }
        else
        {
            itemParentGB = new GameObject();
        }

        GameObject scrollRoot = null;
        if (m_rootTrans.parent.name != m_rootTrans.name + "_root")
        {
            scrollRoot = new GameObject(m_rootTrans.name + "_root");
            scrollRoot.transform.parent = m_rootTrans.parent;
            scrollRoot.transform.localPosition = m_rootTrans.localPosition;
            scrollRoot.transform.localScale = m_rootTrans.localScale;
            m_rootTrans.parent = scrollRoot.transform;
            m_rootTrans.localPosition = Vector3.zero;
            m_rootTrans.localScale = Vector3.one;
        }

		itemParentGB.name = m_rootTrans.name;
		m_itemParentTrans = itemParentGB.transform;
		m_itemParentTrans.parent = m_rootTrans;
		m_itemParentTrans.localPosition = Vector3.zero;
		m_itemParentTrans.localScale = Vector3.one;
		m_itemParentTrans.localEulerAngles = Vector3.zero;
        BoxCollider boxCollider = m_rootTrans.GetComponent<BoxCollider>();
     
        var scrollView = EZFunTools.GetOrAddComponent<UIScrollView>(m_rootTrans.gameObject);
        scrollView.movement = m_direction == ScrollViewDirection.SVD_Vertical ? UIScrollView.Movement.Vertical : UIScrollView.Movement.Horizontal;
        m_scrollView = scrollView;
        var panel = m_rootTrans.GetComponent<UIPanel>();
        if (panel != null)
        {
            panel = EZFunTools.GetOrAddComponent<UIPanel>(m_rootTrans.gameObject);
            panel.depth = iDepth;
        }
        Rigidbody rigibody = EZFunTools.GetOrAddComponent<Rigidbody>(m_rootTrans.gameObject);// itemParentGB.AddComponent<Rigidbody>();
        rigibody.useGravity = false;
        rigibody.isKinematic = true;
        if (boxCollider != null)
        {
            panel.clipping = UIDrawCall.Clipping.SoftClip;
            panel.baseClipRegion = new Vector4(0, 0, boxCollider.size.x, boxCollider.size.y);
            panel.softBorderPadding = false;
        }
        var centerChild = m_itemParentTrans.GetComponent<UICenterOnChild>();
        if (m_specialScrollType != 0)
        {
            
        }
        else
        {
            if (centerChild != null)
                centerChild.enabled = false;
        }
        if (m_specialScrollType == 1)
        {
            if (centerChild == null)
            {
                centerChild = m_itemParentTrans.gameObject.AddComponent<EZFunCenterOnChild>();
            }
            centerChild.onFinished = () =>
            {
                if (m_itemShowFunList[0] != null && centerChild.centeredObject != null)
                {
                    m_itemShowFunList[0](centerChild.centeredObject.transform, 0, 0);
                }
            };
        }
        else if (m_specialScrollType == 2)
        {
            if (centerChild == null)
            {
                centerChild = m_itemParentTrans.gameObject.AddComponent<EZFunCenterOnPage>();
            }
            centerChild.onFinished = () =>
            {
                if (m_itemShowFunList[0] != null && centerChild.centeredObject != null)
                {
                    m_itemShowFunList[0](centerChild.centeredObject.transform, 0, 0);
                }
            };
        }
  
        m_centerOnChild = centerChild;
        int sHeight = EZFunWindowMgr.Instance.GetScreenHeight();
		Vector3 vect = m_rootTrans.position * sHeight / 2;

		Transform animTrans = EZFunUITools.GetTrans(m_windowRootTrans, "animation_ui_root");
		if(animTrans != null)
		{
			vect /= animTrans.localScale.x;
		}
		m_rootBoxSize = new Vector2(boxCollider.size.x, boxCollider.size.y);
        if (m_direction == ScrollViewDirection.SVD_Vertical)
		{
			if (m_isSelfAdapt)
			{
				m_originalMaxRect = Rect.MinMaxRect(-boxCollider.size.x/2 + boxCollider.center.x,  //left
				                                    boxCollider.size.y/2 + boxCollider.center.y,  //top
						                            boxCollider.size.x/2 + boxCollider.center.x,  //right
				                                    boxCollider.size.y/2 + boxCollider.center.y   //bottom
						                            );
			}

			for (int i = 0; i < m_scaleList.Count; i++)
			{
				Vector3 pos = Vector3.zero;
				pos.y = boxCollider.size.y/2 - m_scaleList[i].y/2 + boxCollider.center.y;
				if(m_scaleList[i].x != 0)
				{
					pos.x = -boxCollider.size.x/2 + m_scaleList[i].x/2 + boxCollider.center.x;
	            }

	            m_itemStartPos.Add(pos);
	        }
        }
        else
		{
			if (m_isSelfAdapt)
			{
				m_originalMaxRect = Rect.MinMaxRect(-boxCollider.size.x/2 + boxCollider.center.x,  //left
				                                    boxCollider.size.y/2 + boxCollider.center.y,  //top
				                            		-boxCollider.size.x/2 + boxCollider.center.x,  //right
				                                    -boxCollider.size.y/2 + boxCollider.center.y   //bottom
                                            		);
            }

			for (int i = 0; i < m_scaleList.Count; i++)
			{
				Vector3 pos = Vector3.zero;
				pos.x = -boxCollider.size.x/2 + m_scaleList[i].x/2 + boxCollider.center.x;
				if(m_scaleList[0].y != 0)
				{
					pos.y = boxCollider.size.y/2 - m_scaleList[i].y/2 + boxCollider.center.y;
	            }
	            m_itemStartPos.Add(pos);
	        }
        }

        MonoBehaviour.Destroy(boxCollider);
    }

	public void ResetCameraRect()
	{
        if (m_scrollView != null && m_scrollView.panel != null)
            m_scrollView.MoveRelative(Vector3.zero);
	}

	int m_InBatchLayer = 0;
	public void SetDepth()
	{

    }

    static public void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }

    public UIDraggableCamera getDraggableCamera()
    {
        return null;
    }

    public UIScrollView GetScrollView()
    {
        return m_scrollView;
    }

    public UICenterOnChild GetCenterOnChild()
    {
        return m_centerOnChild;
    }

	//用这个重置下就可以用旧的transform list，不用重新生成，生成起来特别耗性能
	public void ResetScrollView(bool resetCamera = true)
	{
		SetDepth();
        if (resetCamera)
        {
            //  by ydh 4.1pm
            if (m_rootTrans.GetComponent<SpringPanel>() != null)
            {
                m_rootTrans.GetComponent<SpringPanel>().enabled = false;
            }
            ResetCameraPos();
        }
	}

	//重置摄像机位置到初始位置
	public void ResetCameraPos()
	{
        SetLookPosition(Vector3.zero);
    }

    //设置摄像机到scroll底部，摄像机能滑到的最底部,没写成工具函数，慎用！
    //目前只支持 纵向，单种item的scrollview
    public void SetCameraAtEnd()
    {
        //只支持一种item的scrollview
        List<Transform> trans_list = m_hasUsedTransListHash[0];
        float y = 0;
        if (m_direction == ScrollViewDirection.SVD_Vertical)
        {
            Rect last_rect = EZFunUITools.GetRect(trans_list[trans_list.Count -1]);
            y = last_rect.yMin + m_rootBoxSize.y / 2;
        }
    }

    //设置摄像机，使第index个item刚好处在可以看到的第一个
    public void SetIndexFirst(int show_num, int index)
    {
        if (show_num <= 0 || index <= 0)
        {
            return;
        }

        //只支持一种item的scrollview
        List<Transform> trans_list = m_hasUsedTransListHash[0];
        float y = 0, x = 0;
        float fill_num = 0;

        if (m_direction == ScrollViewDirection.SVD_Vertical)
        {
            fill_num = m_rootBoxSize.y / m_scaleList[0].y;
            if (show_num <= fill_num)
            {
                return;
            }
            else if (show_num - index + 1 < fill_num)
            {
                y = trans_list[show_num - 1].localPosition.y - m_scaleList[0].y / 2 + m_rootBoxSize.y / 2;
                //m_camera.transform.localPosition = new Vector3(m_camera.transform.localPosition.x, y, m_camera.transform.localPosition.z);
            }
            else 
            {
                y = trans_list[index - 1].localPosition.y + m_scaleList[0].y / 2 - m_rootBoxSize.y / 2;
                //m_camera.transform.localPosition = new Vector3(m_camera.transform.localPosition.x, y, m_camera.transform.localPosition.z);
            }
        }
        else 
        {
            fill_num = m_rootBoxSize.x / m_scaleList[0].x;
            if (show_num <= fill_num)
            {
                return;
            }
            else if (show_num - index + 1 < fill_num)
            {
                x = trans_list[show_num - 1].localPosition.x + m_scaleList[0].x / 2 - m_rootBoxSize.x / 2;
                //m_camera.transform.localPosition = new Vector3(x, m_camera.transform.localPosition.y, m_camera.transform.localPosition.z);
            }
            else 
            {
                x = trans_list[index - 1].localPosition.x - m_scaleList[0].x / 2 + m_rootBoxSize.x / 2;
                //m_camera.transform.localPosition = new Vector3(x, m_camera.transform.localPosition.y, m_camera.transform.localPosition.z);
            }
        }
    }

    public List<Vector3> getItemStartPos()
	{
		return m_itemStartPos;
	}
	#endregion

	void SetDragBox(List<CScrollGet> getList)
	{
		if (m_isSelfAdapt)
		{
			m_MaxRect = AddRect(m_MaxRect, m_currLineMaxRect);
			m_dragBox = new Rect(m_cameraStartPos.x, m_cameraStartPos.y, m_MaxRect.width - m_rootBoxSize.x, m_MaxRect.height - m_rootBoxSize.y);
		}
		else 
		{
			float w = 0, h = 0, line = 0;
            CScrollGet k = null;
			for(var i = 0; i < getList.Count; i ++)
			{
                k = getList[i];
				if (k.m_count <= 0)
				{
					continue;
				}

				line = k.m_count / m_sideNumbersList[k.m_index];
				if (k.m_count % m_sideNumbersList[k.m_index] > 0)
				{
					line += 1;
				}

				if (m_direction == ScrollViewDirection.SVD_Vertical)
				{
					h += line * m_scaleList[k.m_index].y;
				
				}
				else 
				{
					w += line * m_scaleList[k.m_index].x;
				}

			}
			if(getList.Count >0 && m_barTrans != null)
			{
				EZFunUITools.SetActive(m_barTrans,(m_direction == ScrollViewDirection.SVD_Vertical) ? (h > m_rootBoxSize.y) :(w > m_rootBoxSize.x));
			}
			m_dragBox = new Rect(m_cameraStartPos.x, m_cameraStartPos.y, w - m_rootBoxSize.x, h - m_rootBoxSize.y);
		}
	}
	#region self adapt
	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO
	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO
	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO	//TODOTODO
	//好些rect不用重新new，改下数据就好
	//两个prefab交替的时候scale用较大值
    void ResetCurrLineRect()
	{
		m_currLineMaxRect = new Rect(m_originalMaxRect.x, m_originalMaxRect.y, 0, 0);
    }


    public void SetLookPosition(Vector3 pos)
    {
        UIPanel springPosition = m_rootTrans.GetComponent<UIPanel>();
        if (pos != null)
        {
            m_rootTrans.localPosition = pos;
            springPosition.clipOffset = -pos;
            SpringPanel sp = m_rootTrans.GetComponent<SpringPanel>();
            if (sp != null) sp.target = m_rootTrans.position;
        }
    }

    bool CheckIfNextLine(Rect curr_rect, int key_index, int index)
	{
		bool result = false;

		if(index == 0 ||
		   index > 0 && index % m_sideNumbersList[key_index] == 0)
		{
			result = true;
        }
		else 
		{
			if (m_direction == ScrollViewDirection.SVD_Vertical)
			{
				if (m_currLineMaxRect.width + m_scaleList[key_index].x + curr_rect.width > m_originalMaxRect.width)
				{
					result = true;
				}
				else 
				{
					result = false;
				}
			}
			else 
			{
				if (m_currLineMaxRect.height + m_scaleList[key_index].y + curr_rect.height > m_originalMaxRect.height)
                {
                    result = true;
                }
                else 
                {
                    result = false;
                }
            }
			if (result)
			{
				if (m_currLineMaxRect.width == 0 || m_currLineMaxRect.height == 0)
				{
					//CDebug.LogError("[Scroll] scroll view item is bigger than root!!!");
                }
            }
		}

		if (result)
		{
            return true;
        }
		else 
		{
			return false;
        }
    }
    private Transform topSpace;


    void addEZFunViewSpace()
    {
        if (topSpace == null)
        {
            Transform tran = EZFunUITools.GetTrans(m_rootTrans, "offsetTop");
            if (m_rootTrans.GetComponentInParent<EZFunScrollViewSpacer>() != null)
            {
                float itemTopSpacer = m_rootTrans.GetComponentInParent<EZFunScrollViewSpacer>().topSpacer;
                if (itemTopSpacer != 0)
                {
                    if (tran == null)
                    {
                        GameObject obj = new GameObject("offsetTop");
                        obj.name = "offsetTop";
                        obj.transform.DestroyChildren();
                        tran = obj.transform;
                        obj.AddComponent<UIWidget>();
                    }
                    tran.parent = this.m_itemParentTrans;
                    tran.localScale = this.m_itemParentTrans.localScale;
                    tran.GetComponent<UIWidget>().width = (int)m_rootBoxSize.x;
                    tran.GetComponent<UIWidget>().height = (int)itemTopSpacer;
                    tran.localPosition = new Vector3(m_rootBoxSize.x, m_rootTrans.localPosition.y + m_rootBoxSize.y / 2 - itemTopSpacer / 2, 0f);
                    topSpace = tran;
                    EZFunUITools.SetActive(topSpace.gameObject, true);
                }
            }
            else
            {
                if (tran != null)
                    EZFunUITools.SetActive(tran.gameObject, false);
            }
        }
    }

    void SetTrans(Transform trans, int index, int key_index)
	{
		Vector3 pos = trans.localPosition;
		Vector2 off_set = Vector2.zero;
		Rect curr_rect = EZFunUITools.GetRect(trans);
        //add by deanzhao 
        addEZFunViewSpace();
        if (null != m_refrenceName)
        {
            curr_rect = EZFunUITools.GetRect( EZFunUITools.GetTrans( trans, m_refrenceName) );
        }
		bool next_line = CheckIfNextLine(curr_rect, key_index, index);
		
		if (next_line)
		{
			m_MaxRect = AddRect(m_MaxRect, m_currLineMaxRect);
			
			ResetCurrLineRect();
		}
        int itemTopSpacer = 0;
        int itemRightSpacer = 0;
        if (m_rootTrans.GetComponentInParent<EZFunScrollViewSpacer>() != null)
        {
            itemTopSpacer = trans.GetComponentInParent<EZFunScrollViewSpacer>().topSpacer;
            itemRightSpacer = trans.GetComponentInParent<EZFunScrollViewSpacer>().rightSpacer;
        }
        if (m_direction == ScrollViewDirection.SVD_Vertical)
		{
			//第一个子物件直接挤着上边沿就好了，不用加上间隔m_scaleList[key_index].y
			if (m_MaxRect.height == 0)
			{
				off_set.y = m_MaxRect.yMin - curr_rect.yMax - itemTopSpacer;
			}
			else 
			{
				off_set.y = m_MaxRect.yMin - curr_rect.yMax - m_scaleList[key_index].y;
			}
			
			pos.y += off_set.y;

			if(m_currLineMaxRect.width == 0)
			{
				off_set.x = m_currLineMaxRect.xMax - curr_rect.xMin - itemRightSpacer;
			}
			else 
			{
				off_set.x = m_currLineMaxRect.xMax - curr_rect.xMin + m_scaleList[key_index].x;
			}
			pos.x += off_set.x;
		}
		else 
		{
			//同纵向注释
			if (m_MaxRect.width == 0)
			{
				off_set.x = m_MaxRect.xMax - curr_rect.xMin - itemRightSpacer;
			}
			else 
			{
				off_set.x = m_MaxRect.xMax - curr_rect.xMin + m_scaleList[key_index].x;
			}

			pos.x += off_set.x;

			if(m_currLineMaxRect.height == 0)
			{
				off_set.y = m_currLineMaxRect.yMin - curr_rect.yMax - itemTopSpacer;
			}
			else 
			{
				off_set.y = m_currLineMaxRect.yMin - curr_rect.yMax + m_scaleList[key_index].y;
			}
			pos.y += off_set.y;
		}

		trans.localPosition = pos;
        
		m_currLineMaxRect = AddRect(m_currLineMaxRect, new Rect(curr_rect.x + off_set.x, curr_rect.y + off_set.y, curr_rect.width, curr_rect.height));
        SetDragCamera(trans);
    }

	private Rect AddRect(Rect dst, Rect src)
	{
		return Rect.MinMaxRect(src.xMin < dst.xMin ? src.xMin : dst.xMin,
		                       src.yMin < dst.yMin ? src.yMin : dst.yMin,
		                       src.xMax > dst.xMax ? src.xMax : dst.xMax,
		                       src.yMax > dst.yMax ? src.yMax : dst.yMax);
	}
    
	#endregion

    public void SetCanDrag(bool canDrag)
    {
        //var bc = m_draggableCamera.transform.GetComponent<BoxCollider>();
        //if (bc != null)
        //{
        //    bc.enabled = canDrag;
        //}
        var enm = m_hasUsedTransListHash.GetEnumerator();
        while(enm.MoveNext())
        {
            var transList = enm.Current.Value;
            if (transList == null)
            {
                continue;
            }
            for (int i = 0; i < transList.Count; ++i)
            {
                var trans = transList[i];
                if (trans == null)
                {
                    continue;
                }
                var dc = trans.GetComponentsInChildren<UIDragScrollView>();
                if (dc == null)
                {
                    continue;
                }
                for (int j = 0; j < dc.Length;j ++)
                {
                    dc[j].enabled = canDrag;
                }
            }
        }
    }

    public void SetCanClickItem(bool canClick, string name = null)
    {
        var enm = m_hasUsedTransListHash.GetEnumerator();
        while(enm.MoveNext())
        {
            var transList = enm.Current.Value;
            if (transList == null)
            {
                continue;
            }
            for (int i = 0; i < transList.Count; ++i)
            {
                var trans = transList[i];
                if (trans == null)
                {
                    continue;
                }
                var bc = trans.GetComponent<BoxCollider>();
                if (bc == null)
                {
                    continue;
                }
                if (name == null ||
                    name.Equals(""))
                {
                    bc.enabled = canClick;
                }
                else
                {
                    if (trans.name.Equals(name))
                    {
                        bc.enabled = canClick;
                    }
                    else
                    {
                        bc.enabled = !canClick;
                    }
                }
            }
        }
    }
    public Rect GetMaxRect()
    {
        return m_MaxRect;
    }

    public Vector2 GetRootBoxSize()
    {
        return m_rootBoxSize;
    }

    public void SetLayerGetType(int get_type)
    {
        m_layerGetType = get_type;
    }

    public void JumpPage(int offset)
    {
        if (m_direction == ScrollViewDirection.SVD_Vertical)
        {
            m_scrollView.MoveRelative(new Vector3(0, m_rootBoxSize.y, 0) * offset);
        }
        else 
        {
            m_scrollView.MoveRelative(new Vector3(m_rootBoxSize.x, 0, 0) * offset);
        }
    }

    //当前父节点
    public Transform GetParentTrans()
    {
        return m_itemParentTrans;
    }

    //当前大小
    public Rect GetCurrLineMaxRect()
    {
        return m_currLineMaxRect;
    }
}
