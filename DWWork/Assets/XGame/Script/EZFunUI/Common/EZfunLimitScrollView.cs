using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/// <summary>
/// 这个scollview只生成摄像机范围上下+1的transform，减少压力
/// </summary>

[LuaWrap]
public enum LimitScrollViewDirection
{
    SVD_Horizontal,
    SVD_Vertical,
}
/// <summary>
/// 初始化每个item时调用
/// </summary>
/// <param name="trans"></param>
/// <param name="index"></param>
public delegate void InitItemCall(Transform trans, int index);
/// <summary>
/// 当某个item没有被选中是调用，和上面的那个是相反的过程，当且仅有一个可以被select
/// ,关于这个当然可以自己去扩展,
///
/// 当然这里可能需要拆分成两个
/// </summary>
/// <param name="trans"></param>
/// <param name="index"></param>
public delegate void SelectItemCall(Transform trans, int index);


/// <summary>
///
/// </summary>
/// <param name="trans"></param>
/// <param name="index"></param>
public delegate void UnSelectItemCall(Transform trans, int index);

public delegate void ScrItemDrag(GameObject gb);

[LuaWrap]
public class EZfunLimitScrollView : MonoBehaviour
{

    private GameObject m_ItemObjSource;

    private Transform m_rootTrans;

    private InitItemCall m_initItemCall;

    private SelectItemCall m_selectCall;

    private UnSelectItemCall m_unSelectCall;

    private Vector2 m_itemBox;

    private Vector2i m_camCanSeeNums;
    private ScrItemDrag m_itemDragCB = null;
    private LimitScrollViewDirection m_direction;

    //add bby deanzhao 用于创建ScrollView Topwidget
    private float m_offestScrollViewSpringPosition = 22f;
    private Transform topSpace = null;
    private Vector2 colliderCenter = Vector2.zero;

    private bool m_isResetCameraPosition = false;

    //获得层的方法类型  type 1: 只提供给窗口自己使用；2，同一个窗口的会共用一个layer；3，完全属于自己的layer
    public int m_layerGetType = 2;

    public int m_specialScrollType;
    public ItemShowCallBack m_springToCallBack;

    private UIScrollView m_scrollView;
    public LimitScrollViewDirection Direction
    {
        get
        {
            return m_direction;
        }
    }

    //ScrollView初始位置 用于drag或press后位置调整
    public float offestScrollViewSpringPosition
    {
        get
        {
            return m_offestScrollViewSpringPosition;
        }
        set
        {
            m_offestScrollViewSpringPosition = value;
        }
    }

    /// <summary>
    /// 各个节点的父节点
    /// </summary>
    private Transform m_itemParentTrans;

    /// <summary>
    /// 摄像机大小盒子
    /// </summary>
    //private Vector2 m_CameraBox;


    private Rect m_cameraRect;

    private Transform m_windowRootTrans;

    private Vector2 m_rootBoxSize;

    private Vector2 m_cameraStartPos;

    private Transform m_unusedItemParent;

    private Vector3 m_lastVec = Vector3.up;

    private float m_topSpace = 0f;

    private UIScrollBar m_scrollbar;

    public bool m_isStableCamera = false;

    protected void Start()
    {

    }

    /// <summary>
    /// 是否显示阴影
    /// </summary>
    private bool m_isShowShadow;
    /// <summary>
    /// 滑动摄像机
    /// </summary>
    /// <param name="trans">摄像机和对应子节点的父节点</param>
    /// <param name="itemObj">每个item的object</param>
    /// <param name="initItemCall">给item初始化的回调</param>
    /// <param name="selectItemCall">选中时操作</param>
    /// <param name="itemBox">每个itme的大小</param>
    /// <param name="camCanSeeNums">摄像机能看到的数目总数</param>
    /// <param name="direction">方向 SVD_Horizontal 上下移动，SVD_Vertical左右移动 </param>
    public void Init(Transform trans, GameObject itemObj, InitItemCall initItemCall, SelectItemCall selectItemCall, UnSelectItemCall unSelectCall,
        Vector2 itemBox, Vector2 camCanSeeNums, LimitScrollViewDirection direction, bool isShowShadow = false, ScrItemDrag dragCB = null, float topSpace = 0)
    {
        this.m_rootTrans = trans;
        this.m_ItemObjSource = itemObj;
        this.m_initItemCall = initItemCall;
        this.m_selectCall = selectItemCall;
        var box = itemObj.GetComponent<BoxCollider>();
        if (box != null)
        {
            this.m_itemBox = box.size;
        }
        else
        {
            this.m_itemBox = itemBox;
        }
        var rootBox = trans.GetComponent<BoxCollider>();
        if (rootBox != null && box != null)
        {
            this.m_camCanSeeNums = new Vector2i(
                (int)(rootBox.size.x / this.m_itemBox.x + (direction == LimitScrollViewDirection.SVD_Horizontal ? 1 : 0)),

                (int)(rootBox.size.y / this.m_itemBox.y + (direction == LimitScrollViewDirection.SVD_Vertical ? 1 : 0)));
        }
        else
        {
            this.m_camCanSeeNums = new Vector2i(camCanSeeNums);
        }
        this.m_direction = direction;
        this.m_windowRootTrans = FindWindowTrans();
        this.m_unSelectCall = unSelectCall;
        this.m_isShowShadow = isShowShadow;
        this.m_itemDragCB = dragCB;
        this.m_topSpace = topSpace;
        InitCamera();

        //SetDepth();
    }

    public void InitForLua(Transform trans, GameObject itemObj, Vector2 itemBox, Vector2 camCanSeeNums, LimitScrollViewDirection direction, bool isShowShadow = true)
    {
        Init(trans, itemObj, null, null, null, itemBox, camCanSeeNums, direction, isShowShadow);
    }

    public static EZfunLimitScrollView GetOrAddLimitScr(Transform trans)
    {
        return EZFunTools.GetOrAddComponent<EZfunLimitScrollView>(trans.gameObject);
    }

    public void SetInitItemCall(InitItemCall initItemCall)
    {
        this.m_initItemCall = initItemCall;
    }

    string m_itemCallluaFuncPath = "";
	string m_selCallluaFuncPath = "";
    int m_funcId = 0;
    int m_selectFuncId = 0;
    int m_hideFuncId = 0;
    public void SetInitItemCallLua(string funcPath)
    {
        m_itemCallluaFuncPath = funcPath;
    }

    public void SetInitItemCallLua(int funcID)
    {
        m_funcId = funcID;
    }

    public void SetHideItemCallLua(int funcID)
    {
        m_hideFuncId = funcID;
    }

	public void SetSelectOrUnSelectFuncByName(string  funcPath)
	{
		m_selCallluaFuncPath = funcPath;
	}

	public void SetSelectOrUnSelectFunc(int funcId)
	{
		m_selectFuncId = funcId;
	}

	#region 初始化相关
	int m_InBatchLayer;
    /// <summary>
    ///
    /// </summary>
    public void SetDepth()
    {
        EZFunWindowMgr.UICameraStruct cameraStruct = m_windowRootTrans.GetComponent<WindowRoot>().GetCameraStruct();
        int layer = m_windowRootTrans.gameObject.layer;// EZFunWindowMgr.Instance.getUnusedLayer(m_layerGetType, m_windowRootTrans);
        m_InBatchLayer = layer;
        WindowRoot.SetLayer(m_rootTrans.gameObject, layer);
    }

    public void ResetCameraBoxCollider()
    {
        InitCamera();
    }

    protected virtual void InitCamera()
    {
        var archors = m_rootTrans.GetComponentsInParent<UIAnchor>();
        for (int i = archors.Length - 1; i >= 0; i--)
        {
            Camera cam = null;
            if (EZFunWindowMgr.Instance.m_cameraRootTrans != null && EZFunWindowMgr.Instance.m_cameraRootTrans.childCount > 0 && EZFunWindowMgr.Instance.m_cameraRootTrans.GetChild(0).GetComponent<Camera>() != null)
            {
                cam = EZFunWindowMgr.Instance.m_cameraRootTrans.GetChild(0).GetComponent<Camera>();
            }
            archors[i].uiCamera = cam;
            archors[i].ForceUpdate();
        }

        //GameObject draggableGB = null;
        //var dragTrans = EZFunUITools.GetTrans(m_rootTrans, "DraggableCamera");
        //if (dragTrans != null)
        //{
        //    draggableGB = dragTrans.gameObject;
        //}
        //else
        //{
        //    draggableGB = (GameObject)MonoBehaviour.Instantiate(ResourceMgr.InitAsset(RessType.RT_CommonUItem, "DraggableCamera"));
        //    draggableGB.name = "DraggableCamera";
        //    draggableGB.transform.parent = m_rootTrans;
        //}
        //Transform trans = draggableGB.transform;
        //trans.localPosition = Vector3.zero;
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
        var scrollViewTrans = EZFunUITools.GetTrans(m_rootTrans, m_rootTrans.name);
        if (scrollViewTrans == null)
        {
            GameObject itemParentGB = new GameObject(m_rootTrans.name);
            itemParentGB.name = m_rootTrans.name;
            m_itemParentTrans = itemParentGB.transform;
        }
        else
        {
            m_itemParentTrans = scrollViewTrans;
        }
        m_itemParentTrans.parent = m_rootTrans;
        m_itemParentTrans.localPosition = Vector3.zero;
        m_itemParentTrans.localScale = Vector3.one;
        m_itemParentTrans.localEulerAngles = Vector3.zero;


        var panel = m_rootTrans.GetComponent<UIPanel>();
        if (panel == null)
        {
            panel = EZFunTools.GetOrAddComponent<UIPanel>(m_rootTrans.gameObject);
            panel.depth = 100;
        }
        Rigidbody rigibody = EZFunTools.GetOrAddComponent<Rigidbody>(m_rootTrans.gameObject);// itemParentGB.AddComponent<Rigidbody>();
        rigibody.useGravity = false;
        rigibody.isKinematic = true;
        BoxCollider boxCollider = m_rootTrans.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            panel.clipping = UIDrawCall.Clipping.SoftClip;
            panel.baseClipRegion = new Vector4(0, 0, boxCollider.size.x, boxCollider.size.y);
            panel.softBorderPadding = false;
        }
        var scrollView = EZFunTools.GetOrAddComponent<UIScrollView>(m_rootTrans.gameObject);
        scrollView.movement = m_direction == LimitScrollViewDirection.SVD_Vertical ? UIScrollView.Movement.Vertical : UIScrollView.Movement.Horizontal;
        scrollView.momentumAmount = 100;
        m_scrollView = scrollView;
        //m_scrollView.restrictWithinPanel = false;

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
                if (m_springToCallBack != null && centerChild.centeredObject != null)
                {
                    m_springToCallBack(centerChild.centeredObject.transform, 0, 0);
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
                if (this.m_springToCallBack != null && centerChild.centeredObject != null)
                {
                    m_springToCallBack(centerChild.centeredObject.transform, 0, 0);
                }
            };
        }

        float size = m_cameraRect.height;

        m_rootBoxSize = new Vector2(boxCollider.size.x, boxCollider.size.y);


        m_cameraStartPos = new Vector3(boxCollider.center.x, boxCollider.center.y, 0);


        //m_camera.transform.localPosition = m_cameraStartPos;


        if (m_direction == LimitScrollViewDirection.SVD_Vertical)
        {
            float boxHeight = m_cameraRect.height * EZFunWindowMgr.Instance.GetScreenHeight();
        }
        else
        {
            float boxWidth = m_cameraRect.width * EZFunWindowMgr.Instance.GetScreenWidth();
         }
        colliderCenter = new Vector2(boxCollider.center.x, boxCollider.center.y);   //add by deanzhao

        MonoBehaviour.Destroy(boxCollider);

        //Destroy(trans.GetComponent<UICamera>());


        m_unusedItemParent = EZFunUITools.GetTrans(m_rootTrans, "unuseditems");
        if (m_unusedItemParent == null)
        {
            GameObject obj = new GameObject("unuseditems");
            m_unusedItemParent = obj.transform;
            m_unusedItemParent.parent = m_rootTrans;
        }
        m_unusedItemParent.position = new UnityEngine.Vector3(int.MaxValue, 0, 0);
    }

    //private int m_scollId = -1;

    public void UpdateCameraRect(float scale)
    {
        Rect rect = new Rect(m_cameraRect.x, m_cameraRect.y, m_cameraRect.width * scale, m_cameraRect.height * scale);
        if (rect.x >= 0.5f)
        {
            rect.x = 0.5f + (rect.x - 0.5f) * scale;
        }
        else
        {
            rect.x = 0.5f - (0.5f - rect.x) * scale;
        }

        if (rect.y >= 0.5f)
        {
            rect.y = 0.5f + (rect.y - 0.5f) * scale;
        }
        else
        {
            rect.y = 0.5f - (0.5f - rect.y) * scale;
        }
    }



    #endregion


    private int m_maxCount = 0;
	private int m_maxRow = 0;
    Dictionary<int, Transform> m_usedTrans = new Dictionary<int, Transform>();

    public virtual void InitItemCount(int count, bool isResetCameraPosition = true)
    {
        int canSeeCount = ((int)this.m_camCanSeeNums.x) * ((int)this.m_camCanSeeNums.y);
        m_maxCount = count;
        if (this.m_direction == LimitScrollViewDirection.SVD_Horizontal)
        {
            canSeeCount += ((int)this.m_camCanSeeNums.y) * 2;
			m_maxRow = Mathf.CeilToInt (count / (float)m_camCanSeeNums.y);
        }
        else
        {
            canSeeCount += ((int)this.m_camCanSeeNums.x) * 2;
			m_maxRow = Mathf.CeilToInt (count / (float)m_camCanSeeNums.x);
        }

        m_selectIndex = 0;
        if (Vector3.up == m_lastVec)
        {
            RefreshCameraPosition(m_cameraStartPos, true, true);
        }
        else if (isResetCameraPosition)
        {
            RefreshCameraPosition(m_cameraStartPos, true, true);
        }
        else
        {
            m_lastVec = m_rootTrans.localPosition;
            RefreshCameraPosition(m_lastVec, true, false);
        }
        SetDepth();
    }

    private int m_lastSeeStartIndex = 0;
    private int m_lastSeeEndIndex = 0;

    private List<Transform> m_unusedTrans = new List<Transform>();
    int m_maxCanSeeNum = 0;
    void RefreshCameraPosition(Vector2 cameraPos, bool isInitAll = false, bool disableSpringPanel = false)
    {
        //this.SetLookPosition(new Vector3(0, cameraPos.y, 0), disableSpringPanel);
        m_lastVec = this.m_rootTrans.localPosition;
        int canSeeStartIndex = 0;
        int canSeeEndIndex = 0;
        int moveRow = 0;
		int canSeeEndRow = 0;
        if (m_direction == LimitScrollViewDirection.SVD_Horizontal)
        {
            //this.m_draggableGBTrans.localPosition.y = 0;
            this.SetLookPosition(new Vector3(cameraPos.x, 0, 0), disableSpringPanel);
            //this.SetLookPosition(new Vector3(this.m_rootTrans.localPosition.x, 0, this.m_rootTrans.localPosition.z), disableSpringPanel);
            Vector2 moveDistance = cameraPos - this.m_cameraStartPos;
            m_maxCanSeeNum = (m_camCanSeeNums.x + 1) * (m_camCanSeeNums.y);
            moveRow = (int)Math.Ceiling(Math.Abs(moveDistance.x) / this.m_itemBox.x);
			canSeeEndRow = (int)Math.Ceiling((Math.Abs(moveDistance.x) + m_rootBoxSize.x) / m_itemBox.x);
            if (moveRow > 1)
            {
                canSeeStartIndex = m_camCanSeeNums.y * (moveRow - 2);
                canSeeEndIndex = m_camCanSeeNums.y * (m_camCanSeeNums.x + moveRow + 1) - 1;

            }
            else if (moveRow == 0)
            {
                canSeeStartIndex = 0;
                canSeeEndIndex = m_camCanSeeNums.y * (m_camCanSeeNums.x + moveRow) - 1;
            }
            else
            {
                canSeeStartIndex = 0;
                canSeeEndIndex = m_camCanSeeNums.y * (m_camCanSeeNums.x + 1) - 1;
            }
        }
        else
        {
            //this.m_draggableGBTrans.localPosition = new Vector3(0, this.m_draggableGBTrans.localPosition.y, this.m_draggableGBTrans.localPosition.z);
            this.SetLookPosition(new Vector3(0, cameraPos.y, 0), disableSpringPanel);
            this.SetLookPosition(new Vector3(0, this.m_rootTrans.localPosition.y, this.m_rootTrans.localPosition.z), disableSpringPanel);
            Vector2 moveDistance = -(this.m_cameraStartPos - cameraPos);
            m_maxCanSeeNum = (m_camCanSeeNums.x) * (m_camCanSeeNums.y + 1);
            moveRow = (int)Math.Ceiling(moveDistance.y / this.m_itemBox.y);
			canSeeEndRow = (int)Math.Ceiling((Math.Abs(moveDistance.y) + m_rootBoxSize.y) / m_itemBox.y);
            if (moveRow > 1)
            {
                canSeeStartIndex = m_camCanSeeNums.x * (moveRow - 2);
                canSeeEndIndex = m_camCanSeeNums.x * (m_camCanSeeNums.y + moveRow + 1) - 1;
            }
            else if (moveRow == 0)
            {
                canSeeStartIndex = 0;
                canSeeEndIndex = m_camCanSeeNums.x * (m_camCanSeeNums.y + moveRow) - 1;
            }
            else
            {
                canSeeStartIndex = 0;
                canSeeEndIndex = m_camCanSeeNums.x * (m_camCanSeeNums.y + 1) - 1;
            }
        }

		if (m_maxRow - canSeeEndRow <= m_preloadDataOffset) {
			if (m_onTriggedPreloadData != null) {
				m_onTriggedPreloadData ();
				m_onTriggedPreloadData = null;
			}
		}

		// 翻页功能
		if (m_pageSize!=-1)
		{

			int page_index = canSeeEndRow / m_pageSize + 1;
			if (m_curPageIndex == -1)
			{
				UnityEngine.Debug.Log("page_index " + page_index.ToString());
				m_curPageIndex = page_index;
			}
			//页码变化
			if (m_curPageIndex!=page_index)
			{
				if (m_onTrigged_pageIndexChange != null)
				{
					UnityEngine.Debug.LogError("列表换页回调 page_index change " + page_index.ToString()+"   "+ canSeeEndRow.ToString() + "   "+ m_pageSize.ToString());
					m_onTrigged_pageIndexChange(page_index);
					m_curPageIndex = page_index;
				}
			}
			// 上拉
			if (canSeeStartIndex<=m_upIndex)
			{
				if(m_onTrigged_upIndexCallback != null)
				{
					m_onTrigged_upIndexCallback(page_index);
					m_onTrigged_upIndexCallback = null;
				}
			}
			// 下拉
			if (m_maxRow - canSeeEndRow <= m_downIndex)
			{
				if (m_onTrigged_downIndexCallback != null)
				{
					m_onTrigged_downIndexCallback(page_index);
					m_onTrigged_downIndexCallback = null;
				}
			}
		}
        if (m_maxCount > m_maxCanSeeNum)
        {
            if (canSeeStartIndex > m_maxCount - m_maxCanSeeNum)
            {
                canSeeStartIndex = m_maxCount - m_maxCanSeeNum;
            }
        }
        else
        {
            canSeeStartIndex = 0;
        }
        if (canSeeEndIndex >= m_maxCount)
        {
            canSeeEndIndex = m_maxCount - 1;
        }

        List<Transform> listTrans = new List<Transform>();

        //把不用的暂存起来
        if (m_lastSeeStartIndex < canSeeStartIndex)
        {
            for (int i = m_lastSeeStartIndex; i < canSeeStartIndex; i++)
            {
                if (m_usedTrans.ContainsKey(i))
                {
                    if (m_hideFuncId != 0)
                    {
                        LuaScriptMgr.Instance.CallLuaFunction("LuaCsharpFuncSys.CallFunc",
                            m_hideFuncId, m_usedTrans[i], i);
                    }
                    listTrans.Add(m_usedTrans[i]);
                    m_usedTrans.Remove(i);
                }
            }
        }
        if (m_lastSeeEndIndex > canSeeEndIndex)
        {
            for (int i = canSeeEndIndex; i <= m_lastSeeEndIndex; i++)
            {
                if (m_usedTrans.ContainsKey(i))
                {
                    if (m_hideFuncId != 0)
                    {
                        LuaScriptMgr.Instance.CallLuaFunction("LuaCsharpFuncSys.CallFunc",
                            m_hideFuncId, m_usedTrans[i], i);
                    }
                    listTrans.Add(m_usedTrans[i]);
                    m_usedTrans.Remove(i);
                }
            }
        }
        //add by deanzhao
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
                    tran.localPosition = new Vector3(m_rootBoxSize.x, colliderCenter.y + m_rootBoxSize.y / 2 - itemTopSpacer / 2, 0f);
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

        for (int i = canSeeStartIndex; i <= canSeeEndIndex; i++)
        {
            if (!m_usedTrans.ContainsKey(i))
            {
                Transform trans = null;
                if (listTrans.Count > 0)
                {
                    trans = listTrans[listTrans.Count - 1];
                    listTrans.RemoveAt(listTrans.Count - 1);
                }
                else
                {
                    trans = GetTrans();
                }
                m_usedTrans.Add(i, trans);
                SetPosition(trans, i);
            }
            else
            {
                if (isInitAll)
                {
                    SetPosition(m_usedTrans[i], i);
                }
            }
        }
        m_lastSeeEndIndex = canSeeEndIndex;
        m_lastSeeStartIndex = canSeeStartIndex;
        for (int i = 0; i < listTrans.Count; i++)
        {
            ReusePoolPush(listTrans[i]);
        }
    }


    Transform GetTrans()
    {
        Transform tran = null;
        if (m_unusedTrans.Count == 0)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(m_ItemObjSource);
            tran = obj.transform;
            tran.SetParent(this.m_itemParentTrans, false);
            tran.localScale = this.m_itemParentTrans.localScale;
        }
        else
        {
            tran = ReusePoolPop();
            tran.SetParent(this.m_itemParentTrans, false);
            tran.localScale = this.m_itemParentTrans.localScale;
        }
        WindowRoot.SetLayer(tran.gameObject, m_InBatchLayer);
        EZFunUITools.SetActive(tran.gameObject, true);
        return tran;
    }

    void ReusePoolPush(Transform trans)
    {
        EZFunUITools.SetActive(trans.gameObject, false);
        trans.SetParent(m_unusedItemParent, false);
        trans.localPosition = m_unusedItemParent.localPosition;
        this.m_unusedTrans.Add(trans);
    }

    Transform ReusePoolPop()
    {
        Transform tran = m_unusedTrans[m_unusedTrans.Count - 1];
        m_unusedTrans.RemoveAt(m_unusedTrans.Count - 1);
        return tran;
    }

    void SetPosition(Transform trans, int index)
    {
        Vector2 startPos = Vector2.zero;
        startPos.x = m_cameraStartPos.x - m_rootBoxSize.x / 2;
        startPos.y = m_cameraStartPos.y + m_rootBoxSize.y / 2;
        float itemTopSpacer = 0;
        float itemRightSpacer = 0;
        if (trans.GetComponentInParent<EZFunScrollViewSpacer>() != null)
        {
            itemTopSpacer = trans.GetComponentInParent<EZFunScrollViewSpacer>().topSpacer;
            itemRightSpacer = trans.GetComponentInParent<EZFunScrollViewSpacer>().rightSpacer;
        }

        Vector3 pos = Vector3.zero;
        if (m_direction == LimitScrollViewDirection.SVD_Horizontal)
        {
            pos.x = startPos.x + ((int)((index) / m_camCanSeeNums.y)) * m_itemBox.x + (m_itemBox.x / 2) + itemRightSpacer;
            pos.y = startPos.y - (index % m_camCanSeeNums.y) * m_itemBox.y - m_itemBox.y / 2 - itemTopSpacer;
        }
        else
        {
            pos.y = startPos.y - ((int)((index) / m_camCanSeeNums.x)) * m_itemBox.y - (m_itemBox.y / 2) - itemTopSpacer;
            pos.x = startPos.x + (index % m_camCanSeeNums.x) * m_itemBox.x + m_itemBox.x / 2 + itemRightSpacer;
        }
        trans.localPosition = pos;
        //SetDragCamera(trans);
        var dragView = EZFunTools.GetOrAddComponent<UIDragScrollView>(trans.gameObject);
        dragView.isDisableNeedPress = false;
        //var message = EZFunTools.GetOrAddComponent<UIButtonMessage>(trans.gameObject);
        //  by ydh 2016.12.16
        //message.mTweenScaleOpen = false;
        //message.mTweenScaleOpen = true;
        if (m_specialScrollType != 0)
        {
            var clickCenter = EZFunTools.GetOrAddComponent<UICenterOnClick>(trans.gameObject);
        }
        dragView.scrollView = m_scrollView;
        if (!m_initItemCall.IsNull())
        {
            m_initItemCall(trans, index);
        }
        if (m_funcId != 0)
        {
            LuaScriptMgr.Instance.CallLuaFunction("LuaCsharpFuncSys.CallFunc",
                m_funcId, trans, index
                );
        }
        if (!string.IsNullOrEmpty(m_itemCallluaFuncPath))
        {
            WindowBaseLua.m_luaMgr.CallLuaFunction(m_itemCallluaFuncPath, trans, index);
        }

        if (m_selectIndex == index)
        {
            if (m_selectFuncId != 0)
            {
                LuaScriptMgr.Instance.CallLuaFunction("LuaCsharpFuncSys.CallFunc",
               m_selectFuncId, trans, index, true
               );
            }
            else
            if (!m_selectCall.IsNull())
            {
                m_selectCall(m_usedTrans[index], index);
            }
			else
			if (!string.IsNullOrEmpty(m_selCallluaFuncPath))
			{
				UnityEngine.Debug.Log("Set position 1");
				WindowBaseLua.m_luaMgr.CallLuaFunction(m_selCallluaFuncPath, m_usedTrans[index], index);
			}
		}
        else
        {
            if (m_selectFuncId != 0)
            {
                LuaScriptMgr.Instance.CallLuaFunction("LuaCsharpFuncSys.CallFunc",
               m_selectFuncId, m_usedTrans[index], index, false
               );
            }
            else
           if (!m_unSelectCall.IsNull())
            {
                m_unSelectCall(m_usedTrans[index], index);
            }
			else
			if (!string.IsNullOrEmpty(m_selCallluaFuncPath))
			{
				WindowBaseLua.m_luaMgr.CallLuaFunction(m_selCallluaFuncPath, m_usedTrans[index], index);
			}
		}
        CheckCanClickAndDrag(trans);
    }

    private int m_selectIndex = -1;
    public int SelectIndex
    {
        get { return m_selectIndex; }
        set
        {
            if (SelectIndex != value)
            {
                if (m_usedTrans.ContainsKey(m_selectIndex))
                {
                    if (m_selectFuncId != 0)
                    {
                        LuaScriptMgr.Instance.CallLuaFunction("LuaCsharpFuncSys.CallFunc",
                       m_selectFuncId, m_usedTrans[m_selectIndex], m_selectIndex, false
                       );
                    }
                    else
                    if (!m_unSelectCall.IsNull())
                    {
                        m_unSelectCall(m_usedTrans[m_selectIndex], m_selectIndex);
                    }
					else
					if (!string.IsNullOrEmpty(m_selCallluaFuncPath))
					{
						UnityEngine.Debug.Log("修改之前的"+ m_selectIndex.ToString());

						WindowBaseLua.m_luaMgr.CallLuaFunction(m_selCallluaFuncPath, m_usedTrans[m_selectIndex], m_selectIndex);
					}
				}
                m_selectIndex = value;
                if (m_usedTrans.ContainsKey(value))
                {
                    if (m_selectFuncId != 0)
                    {
                        LuaScriptMgr.Instance.CallLuaFunction("LuaCsharpFuncSys.CallFunc",
                       m_selectFuncId, m_usedTrans[value], value, true
                       );
                    }
                    else
                   if (!m_selectCall.IsNull())
                    {
                        m_selectCall(m_usedTrans[value], value);
                    }
					else
					if (!string.IsNullOrEmpty(m_selCallluaFuncPath))
					{
						UnityEngine.Debug.Log("修改之后的" + m_selectIndex.ToString());
						WindowBaseLua.m_luaMgr.CallLuaFunction(m_selCallluaFuncPath, m_usedTrans[value], value);
					}
				}
            }
        }
    }


    /// <summary>
    /// 让想要看到的某个索引作为第一项, By Atin
    /// </summary>
    private int m_startIndex;
    public int StartIndex
    {
        set
        {
            m_startIndex = value;
            Vector2 vec = m_cameraStartPos;
            if (m_direction == LimitScrollViewDirection.SVD_Horizontal)
            {
                int moveRow = value / this.m_camCanSeeNums.y;
                vec.x = m_cameraStartPos.x - this.m_itemBox.x * moveRow;
            }
            else
            {
                int moveRow = value / this.m_camCanSeeNums.x;
                vec.y = -m_cameraStartPos.y + this.m_itemBox.y * moveRow;
            }
            RefreshCameraPosition(vec, true);
        }
        get
        {
            return m_startIndex;
        }
    }

    /// <summary>
    /// 让想要看到的某个索引作为窗口的最后一项 By Atin
    /// </summary>
    private int m_endIndex;
    public int EndIndex
    {
        set
        {
            m_endIndex = value;
            Vector2 vec = m_cameraStartPos;
            BoxCollider boxcol = m_rootTrans.GetComponent<BoxCollider>();
            UIPanel panel = m_rootTrans.GetComponent<UIPanel>();
            if (m_direction == LimitScrollViewDirection.SVD_Horizontal)
            {
                //if (panel != null)
                //{
                //    int canShowColNum = (int)(panel.width / this.m_itemBox.x);
                //    float offset = panel.width - canShowColNum * this.m_itemBox.x;
                //    int start_index = value - (canShowColNum - 1) * this.m_camCanSeeNums.y;
                //    int moveCol = start_index / this.m_camCanSeeNums.y;
                //    vec.x = m_cameraStartPos.x - this.m_itemBox.x * moveCol - offset;//还没测
                //}
                //这么想，首先计算出从index=0到index=EndIndex的距离，然后将移动看作的是 可视范围 的移动即可。
                int totalRow = value / m_camCanSeeNums.y + 1;//计算出行数
                float totalDis = totalRow * m_itemBox.x;//计算出总距离
                float moveDis;
                if (boxcol != null)
                {
                    moveDis = totalDis - boxcol.size.x;//计算移动范围
                }
                else if (panel != null)
                {
                    moveDis = totalDis - panel.width;//计算移动范围
                }
                else
                {
                    moveDis = totalDis - (m_camCanSeeNums.x - 1) * m_itemBox.x;
                }
                if (moveDis <= 0)//说明在可视范围内，不需要移动
                {
                    //不用处理
                }
                else
                {
                    vec.x = m_cameraStartPos.x - moveDis;//还没测
                }
            }
            else
            {
                //if(panel != null)
                //{
                //    int canShowRowNum = (int)(panel.height / this.m_itemBox.y);//求出滑动窗口最多显示的行的数量
                //    float offset = panel.height - canShowRowNum * this.m_itemBox.y;//求出滑动窗口减去完整显示项之后的剩余小距离
                //    //如果说让endIndex对应的行在窗口内完整显示的时候，那么它之前有canShowRowNum-1行，那么这时要想让endIndex前canShowRowNum-1的那一行的对应项作为开始index，就可以就得相机应该位移的距离，这个距离再减去offset就可以让endIndex在窗口最后
                //    int start_index = value - (canShowRowNum - 1) * this.m_camCanSeeNums.x;//求出对应前canShowNum-1的那项的index
                //    int moveRow = start_index / this.m_camCanSeeNums.x;
                //    vec.y = -m_cameraStartPos.y + this.m_itemBox.y * moveRow - offset;
                //}
                int totalCol = value / m_camCanSeeNums.x + 1;//计算出列数
                float totalDis = totalCol * m_itemBox.y;//计算出总距离
                float moveDis;
                if (boxcol != null)
                {
                    moveDis = totalDis - boxcol.size.y;//计算移动范围
                }
                else if (panel != null)
                {
                    moveDis = totalDis - panel.height;//计算移动范围
                }
                else
                {
                    moveDis = totalDis - (m_camCanSeeNums.y - 1) * m_itemBox.y;
                }
                if (moveDis <= 0)//说明在可视范围内，不需要移动
                {
                    //不用处理
                }
                else
                {
                    vec.y = -m_cameraStartPos.y + moveDis;//还没测
                }
            }
            RefreshCameraPosition(vec, true);
        }
        get
        {
            return m_endIndex;
        }
    }


    /// <summary>
    /// 想要看到某个索引
    /// </summary>
    public int FocusIndex
    {
        set
        {
            if (m_lastSeeStartIndex <= value && m_lastSeeEndIndex >= value)
            {
                OnCenterOnIndex(value, true);
                return;
            }
            Vector2 vec = m_cameraStartPos;
            if (m_direction == LimitScrollViewDirection.SVD_Horizontal)
            {
                int moveRow = value / this.m_camCanSeeNums.y;
                if (moveRow >= (m_maxCount / this.m_camCanSeeNums.y) - (this.m_camCanSeeNums.x))
                {
                    moveRow = (m_maxCount / this.m_camCanSeeNums.y) - (this.m_camCanSeeNums.x);
                    if (moveRow < 0)
                    {
                        moveRow = 0;
                    }
                }
                vec.x = - m_cameraStartPos.x + this.m_itemBox.x * moveRow;
            }
            else
            {
                int moveRow = value / this.m_camCanSeeNums.x;

                if (moveRow >= (m_maxCount / this.m_camCanSeeNums.x) - (this.m_camCanSeeNums.y))
                {
                    moveRow = (m_maxCount / this.m_camCanSeeNums.x) - (this.m_camCanSeeNums.y);
                    if (moveRow < 0)
                    {
                        moveRow = 0;
                    }
                }
                vec.y = m_cameraStartPos.y - this.m_itemBox.y * moveRow;
            }
            RefreshCameraPosition(-vec, true);
            OnCenterOnIndex(value, true);
        }
    }

    /// <summary>
    /// 用这个之前得保证对应的tranform已经被加载了
    /// </summary>
    /// <param name="index"></param>
    private void OnCenterOnIndex(int index, bool isInRange)
    {
        if (!m_usedTrans.ContainsKey(index))
        {
            return;
        }
        var trans = m_usedTrans[index];
        if (trans == null)
        {
            return;
        }
        if (m_specialScrollType != 0 && isInRange)
        {
            var centerOnChild = this.GetComponentInChildren<UICenterOnChild>();
            //Debug.LogError(this.name + "trans.POSITION=" + trans.position + " trans:" + trans);
            centerOnChild.CenterOn(trans);
        }
        //如果能当前看到的最大数量小于  能看到的数量  那么直接置0
        else if (m_lastSeeEndIndex <= m_camCanSeeNums.x * m_camCanSeeNums.y)
        {
            this.transform.localPosition = Vector3.zero;
        }
        else
        {
            var scrollView = GetComponent<UIScrollView>();
            Transform panelTrans = scrollView.panel.cachedTransform;
            Vector3[] corners = scrollView.panel.worldCorners;
            Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;
            // Figure out the difference between the chosen child and the panel's center in local coordinates
            Vector3 cp = trans.position;// panelTrans.InverseTransformPoint(trans.position);
            Vector3 cc = panelCenter;// panelTrans.InverseTransformPoint(panelCenter);
            Vector3 localOffset = -cp + cc;

            // Offset shouldn't occur if blocked
            if (!scrollView.canMoveHorizontally) localOffset.x = 0f;
            if (!scrollView.canMoveVertically) localOffset.y = 0f;
            localOffset.z = 0f;
            scrollView.MoveAbsolute(localOffset);
        }
    }

    /// <summary>
    /// 用来设置scrollbar的
    /// </summary>
    /// <param name="scrollbar"></param>
    public void InitScrollBar(UIScrollBar scrollbar)
    {
        this.m_scrollbar = scrollbar;
    }

    public void SetLookPosition(Vector3 pos, bool isDisableSpringPanel = false)
    {
        UIPanel springPosition = m_rootTrans.GetComponent<UIPanel>();
        if (pos != null)
        {
            m_rootTrans.localPosition = pos;
            springPosition.clipOffset = -pos;
        }
        if (isDisableSpringPanel)
        {
            var springPanel = m_rootTrans.GetComponent<SpringPanel>();
            if (springPanel != null)
            {
                springPanel.enabled = false;
            }
        }

    }

    protected virtual void LateUpdate()
    {
        Vector3 pos = -m_rootTrans.localPosition;//Vector3.zero;// m_draggableGBTrans.localPosition;
        SpringPanel springPosition = m_rootTrans.GetComponent<SpringPanel>();

        if (m_isResetCameraPosition && springPosition != null && !springPosition.enabled)//dragcamera 不在回归动作
        {
            if (m_direction == LimitScrollViewDirection.SVD_Horizontal)
            {
                if ((pos - new Vector3(m_cameraStartPos.x, m_cameraStartPos.y, 0)).magnitude >= m_itemBox.x / 2)
                {
                    RefreshCameraPosition(m_cameraStartPos);
                    m_isResetCameraPosition = false;
                    return;
                }
                else
                {
                    m_isResetCameraPosition = false;
                }
            }
            else
            {
                if ((pos - new Vector3(m_cameraStartPos.x, m_cameraStartPos.y, 0)).magnitude >= m_itemBox.y / 2)
                {
                    RefreshCameraPosition(m_cameraStartPos);
                    m_isResetCameraPosition = false;
                    return;
                }
                else
                {
                    m_isResetCameraPosition = false;
                }
            }
        }
        if (this.m_scrollbar != null && (m_lastVec - pos).sqrMagnitude > 1)
        {
            float maxPos = 0;
            if (m_direction == LimitScrollViewDirection.SVD_Horizontal)
            {
                maxPos = Math.Max(m_cameraStartPos.x, (this.m_maxCount / this.m_camCanSeeNums.y - this.m_camCanSeeNums.x) * this.m_itemBox.x);
                if (pos.x <= m_cameraStartPos.x)
                {
                    this.m_scrollbar.value = 0;
                }
                else if (pos.x > maxPos)
                {
                    this.m_scrollbar.value = 1;
                }
                else
                {
                    this.m_scrollbar.value = (pos.x - m_cameraStartPos.x) / (maxPos - m_cameraStartPos.x);
                }
            }
            else
            {
                maxPos = -Math.Max(m_cameraStartPos.y, (this.m_maxCount / this.m_camCanSeeNums.x - this.m_camCanSeeNums.y) * this.m_itemBox.y);
                if (pos.y >= m_cameraStartPos.y)
                {
                    this.m_scrollbar.value = 0;
                }
                else if (pos.y < maxPos)
                {
                    this.m_scrollbar.value = 1;
                }
                else
                {
                    this.m_scrollbar.value = (pos.y - m_cameraStartPos.y) / (maxPos - m_cameraStartPos.y);
                }
            }
        }

        if (m_direction == LimitScrollViewDirection.SVD_Horizontal)
        {
            m_rootTrans.localPosition = new Vector3(m_rootTrans.localPosition.x, 0, m_rootTrans.localPosition.z);
            pos = m_rootTrans.localPosition;
            if ((pos - m_lastVec).magnitude < m_itemBox.x / 2)
            {
                return;
            }
        }
        else
        {
            m_rootTrans.localPosition = new Vector3(0, m_rootTrans.localPosition.y, m_rootTrans.localPosition.z);
            pos = m_rootTrans.localPosition;
            if ((pos - m_lastVec).magnitude < m_itemBox.y / 2)
            {
                return;
            }
        }
        m_lastVec = pos;
        RefreshCameraPosition(pos);
    }


    void OnDisable()
    {
        m_lastVec = Vector3.up;
    }


    private bool m_isCanDrag = true;
    public void SetCanDrag(bool canDrag)
    {
        //var bc = m_draggableCamera.transform.GetComponent<BoxCollider>();
        //if (bc != null)
        //{
        //    bc.enabled = canDrag;
        //}
        //m_isCanDrag = canDrag;
        //foreach (var kv in m_usedTrans)
        //{
        //    CheckCanClickAndDrag(kv.Value);
        //}
        //}
        m_isCanDrag = canDrag;
        var enumertor = m_usedTrans.GetEnumerator();
        while (enumertor.MoveNext())
        {
            var cur = enumertor.Current.Value;
            if (cur == null)
            {
                continue;
            }
            var trans = cur;
            if (trans == null)
            {
                continue;
            }
            var dc = trans.GetComponentsInChildren<UIDragScrollView>();
            if (dc == null)
            {
                continue;
            }
            for (int j = 0; j < dc.Length; j++)
            {
                dc[j].enabled = canDrag;
            }
        }
    }


    private string m_ClickName = "";
    private bool m_CanClick = true;

    public void SetCanClickItem(bool canClick, string name = null)
    {
        m_CanClick = canClick;
        m_ClickName = name;
        var enm = m_usedTrans.GetEnumerator();
        while(enm.MoveNext())
        {
            CheckCanClickAndDrag(enm.Current.Value);
        }
    }

    private void CheckCanClickAndDrag(Transform trans)
    {
        var dc = trans.GetComponent<UIDragScrollView>();
        if (dc != null)
        {
            dc.enabled = m_isCanDrag;
        }
        var boxcollider = trans.GetComponent<BoxCollider>();
        if (boxcollider != null)
        {
            if (trans.name.Contains(m_ClickName))
            {
                boxcollider.enabled = m_CanClick;
            }
            else
            {
                boxcollider.enabled = !m_CanClick;
            }
        }
    }

    public UIDraggableCamera getDraggableCamera()
    {
        return null;
    }

    #region 用于实现左右滑动切换页面的
    class DragRange
    {
        private float min;
        private float max;

        public DragRange(float _min, float _max)
        {
            if (_min > _max)
            {
                min = _max;
                max = _min;
            }
            else
            {
                min = _min;
                max = _max;
            }
        }

        public int CheckIsInRange(float x)
        {
            if (x < min)
            {
                return -1;
            }
            else if (x > max)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    private Action<int> m_onDragFinished;
    private DragRange m_range;
    private int togglePage = 0;//0代表不用切页;1代表滑动窗口的滑动范围已经大于最大值,代表上一页;-1代表滑动窗口的滑动范围已经小于最小值，代表要进入下一页

    private void DragStart()
    {
        togglePage = 0;
    }

    private void DragEnd()
    {
        CalculateDragOffset();
        if(m_onDragFinished != null)
        {
            m_onDragFinished(togglePage);
        }
        togglePage = 0;
    }

    private void DragIng()
    {

    }

    private void CalculateDragOffset()
    {
        float delta = m_direction == LimitScrollViewDirection.SVD_Horizontal ? m_rootTrans.localPosition.x : m_rootTrans.localPosition.y;
        togglePage = m_range.CheckIsInRange(delta);
    }

    private void InitDragRange(float _offset)
    {
        float limitDist_min = 0f;
        float limitDist_max = 0f;
        int itemTotalNum = 0;//如果是水平移动，则代表col列数；竖直移动，则为row行数
        int canSeeItemNum = 0;//能看到 完整的 Item的数量.例如:如果能看到3个半，那为3
        BoxCollider box = m_rootTrans.GetComponent<BoxCollider>();
        UIPanel panel = m_rootTrans.GetComponent<UIPanel>();
        Vector3 winSize;//窗口大小
        if(box != null)
        {
            winSize = box.size;
        }
        else if(panel != null)
        {
            winSize = new Vector3(panel.width, panel.height);
        }
        else
        {
            return;
        }

        float offsetToBound = 0f;//差多少距离就到滑动窗口显示范围的边界
        switch (m_direction)
        {
            case LimitScrollViewDirection.SVD_Horizontal:
                itemTotalNum = m_maxCount / m_camCanSeeNums.y + (m_maxCount % m_camCanSeeNums.y == 0 ? 0 : 1);
                canSeeItemNum = (int)(winSize.x / m_itemBox.x);
                offsetToBound = winSize.x - canSeeItemNum * m_itemBox.x;
                limitDist_min = Mathf.Abs(_offset);
                limitDist_max = -((itemTotalNum - canSeeItemNum) * m_itemBox.x - offsetToBound) - Mathf.Abs(_offset);
                break;

            case LimitScrollViewDirection.SVD_Vertical:
                itemTotalNum = m_maxCount / m_camCanSeeNums.x + (m_maxCount % m_camCanSeeNums.x == 0 ? 0 : 1);
                canSeeItemNum = (int)(winSize.y / m_itemBox.y);
                offsetToBound = winSize.y - canSeeItemNum * m_itemBox.y;
                limitDist_min = -Mathf.Abs(_offset);
                limitDist_max = (itemTotalNum - canSeeItemNum) * m_itemBox.y - offsetToBound + Mathf.Abs(_offset);
                break;
            default:
                break;
        }
        m_range = new DragRange(limitDist_min, limitDist_max);
    }


    /// <summary>
    /// 要在scrollview的Init和InitItemCount函数调完之后再设置
    /// </summary>
    /// <param name="offset">当到滑动窗口的尽头(左右滑动包括左尽头和右尽头),再偏移多少就进行回调</param>
    /// <param name="onDragFinished">回调,这里的回调函数需要接受一个int的参数，由limitScrollview传入三个值0、-1、1；但对应的处理由回调决定。
    /// 举个栗子，现设定1,0,-1分别代表拉到左尽头、没到两端、拉到右尽头，那么就可以根据不同参数做处理</param>
    public void InitDragTogglePage(float offset = 0f, Action<int> onDragFinished = null)
    {
        if(m_scrollView == null || m_rootTrans == null)
        {
            return;
        }
        togglePage = 0;

        InitDragRange(offset);//计算好滑动范围

        m_onDragFinished = onDragFinished;
        m_scrollView.onDragStarted = DragStart;
        m_scrollView.onDragFinished = DragEnd;
        m_scrollView.onMomentumMove = DragIng;
    }

    /// <summary>
    /// 给lua调的
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="luaFunc"></param>
    public void InitDragTogglePageForLua(float offset = 0f, String luaFunc = null)
    {
        Action<int> temp = null;
        if (!String.IsNullOrEmpty(luaFunc))
        {
            temp = (int x) =>
            {
                WindowBaseLua.m_luaMgr.CallLuaFunction(luaFunc, x);
            };
        }
        InitDragTogglePage(offset, temp);
    }
    #endregion

	#region //翻页功能
	private Action<int> m_onTrigged_pageIndexChange = null;
	private Action<int> m_onTrigged_upIndexCallback = null;
	private Action<int> m_onTrigged_downIndexCallback = null;
	private int m_upIndex = -1;
	private int m_downIndex = -1;
	private int m_pageSize = -1;
	private int m_curPageIndex = -1;
	public int m_dataStartPageIndex = -1;
	// 注册页码相关回调 : 页码大小 页码改变的回调函数名,上拉index,下拉index,上拉回调函数名,下拉回调函数名
	public void RegisterPageFunctions(int pageSize = 0,  int upIndex = 0, int downIndex = 0, string pageIndexChangeCallback_luaFunName = "", string upCallback_luaFunName = "", string downCallback_luaFunName = "")
	{
		Action<int> action_pageChange = null; //页码改变时候回调 根据pageSize来判断 当前页码
		Action<int> action_upCallback = null; //上拉回调
		Action<int> action_DownCallback = null; //下拉回调

		if (!String.IsNullOrEmpty(upCallback_luaFunName))
		{
			action_upCallback = (pageIndex) =>
			{
				WindowBaseLua.m_luaMgr.CallLuaFunction(upCallback_luaFunName, pageIndex);
			};
		}
		if (!String.IsNullOrEmpty(downCallback_luaFunName))
		{
			action_DownCallback = (pageIndex) =>
			{
				WindowBaseLua.m_luaMgr.CallLuaFunction(downCallback_luaFunName, pageIndex);
			};
		}
		if (!String.IsNullOrEmpty(pageIndexChangeCallback_luaFunName))
		{
			action_pageChange = (pageIndex) =>
			{
				WindowBaseLua.m_luaMgr.CallLuaFunction(pageIndexChangeCallback_luaFunName, pageIndex);
			};
		}
		if (m_scrollView == null || m_rootTrans == null)
		{
			return;
		}
		m_onTrigged_pageIndexChange = action_pageChange;
		m_onTrigged_upIndexCallback = action_upCallback;
		m_onTrigged_downIndexCallback = action_DownCallback;

		m_pageSize = pageSize;
		m_upIndex = upIndex;
		m_downIndex = downIndex;

	}
	#endregion

	#region 用来预判提前请求数据
	private Action m_onTriggedPreloadData = null;
	private int _m_preloadDataOffset = 0;
	private int m_preloadDataOffset
	{
		set{
			if (m_direction == LimitScrollViewDirection.SVD_Horizontal) {
				if (m_maxRow - m_camCanSeeNums.x <= value) {
					Debug.Log ("[ALERT] Get bigger maxRow or smaller Offset!!!");
				} else {
					_m_preloadDataOffset = value;
				}
			} else {
				if (m_maxRow - m_camCanSeeNums.y <= value) {
					Debug.Log ("[ALERT] Get bigger maxRow or smaller Offset!!!");
				} else {
					_m_preloadDataOffset = value;
				}
			}
		}
		get{
			if (m_maxRow - m_camCanSeeNums.x <= _m_preloadDataOffset) {
				return -999999;
			} else {
				return _m_preloadDataOffset;
			}
		}
	}
	/// <summary>
	/// 要在scrollview的Init和InitItemCount函数调完之后再设置
	/// </summary>
	public void InitPreLoadData(int offset = 0, Action onReached = null)
	{
		if(m_scrollView == null || m_rootTrans == null)
		{
			return;
		}
		m_preloadDataOffset = offset;
		m_onTriggedPreloadData = onReached;
	}

	/// <summary>
	/// 给lua调的
	/// </summary>
	/// <param name="offset"></param>
	/// <param name="luaFunc"></param>
	public void InitPreLoadDataForLua(int offset = 0, String luaFunc = null)
	{
		Action temp = null;
		if (!String.IsNullOrEmpty(luaFunc))
		{
			temp = () =>
			{
				WindowBaseLua.m_luaMgr.CallLuaFunction(luaFunc);
			};
		}
		InitPreLoadData(offset, temp);
	}

    public void InitPreLoadDataForLua(int offset = 0, int func_id = -1)
    {
        Action temp = null;
        if (func_id > 0)
        {
            temp = () =>
            {
                LuaScriptMgr.Instance.CallLuaFunction("LuaCsharpFuncSys.CallFunc",
                    func_id
                    );
            };
        }
        InitPreLoadData(offset, temp);
    }
    #endregion

    #region tools
    Transform FindWindowTrans()
    {
        if (m_rootTrans == null)
            return null;
        Transform trans = m_rootTrans;
        while (!trans.name.Contains("ui_window") && trans.parent != null)
        {
            trans = trans.parent;
        }

        if (trans != null)
        {
            WindowRoot windowRoot = trans.GetComponent<WindowRoot>();
            if (windowRoot != null)
            {
                windowRoot.AddLimitScrollView(this);
            }
        }
        return trans;
    }

    public void ResetCameraRect()
    {

    }

    //获取现在用到的Transform
    public Transform GetUseTransformByIndex(int index)
    {
        if(m_usedTrans != null && m_usedTrans.ContainsKey(index))
        {
            return m_usedTrans[index];
        }
        return null;
    }
    #endregion
}


public struct Vector2i
{
    public int x;
    public int y;

    public Vector2i(int i, int j)
    {
        x = i;
        y = j;
    }
    public Vector2i(Vector2 vec)
    {
        x = (int)vec.x;
        y = (int)vec.y;
    }
}

public struct Vector3i
{
    public int x;
    public int y;
    public int z;

    public Vector3i(int i, int j, int m)
    {
        x = i;
        y = j;
        z = m;
    }
    public Vector3i(Vector3 vec)
    {
        x = (int)vec.x;
        y = (int)vec.y;
        z = (int)vec.z;
    }
}
