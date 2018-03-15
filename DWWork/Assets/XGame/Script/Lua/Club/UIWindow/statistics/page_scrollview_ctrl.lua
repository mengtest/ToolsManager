--------------------------------------------------------------------------------
-- 	 File       : page_scrollview_ctrl.lua
--   author     : zhisong
--   function   : 分页请求列表显示基类（其他类club_rounds_statistics_ctrl这种是继承与这个类）
--   date       : 2018年1月29日
--   copyright  : Copyright 2018 DW Inc.P
--------------------------------------------------------------------------------

require "LuaWindow.Module.UINetTipsCtrl"

page_scrollview_ctrl = class("page_scrollview_ctrl")

-------------------------------------------------------------------------------------------------------
function page_scrollview_ctrl:ctor(lua_window_root)
	self.m_lua_win_root = lua_window_root
	self:InitScrollView()
	self:RegisterEvent()
	self:InitNetTipsCtrl()
end

function page_scrollview_ctrl:RegisterEvent()
end

function page_scrollview_ctrl:UnRegisterEvent()
end

function page_scrollview_ctrl:Destroy()
	self:ClearScrollView()
	self:UnRegisterEvent()
	self:DestroyNetTipsCtrl()
end

function page_scrollview_ctrl:InitParams(params)
	self.m_params = params
end
-----------------------------必须重写的几个接口----------------------------------
-- 初始化ui，此模块显示总入口
function page_scrollview_ctrl:InitUI()
end
-- 设置单个记录显示内容
function page_scrollview_ctrl:InitTrans(root, index, data)
end
-- 请求一页数据函数
function page_scrollview_ctrl:Request(succ_cb, fail_cb, page_index, page_size)
end

function page_scrollview_ctrl:HandleWidgetClick(gb)
	if self.m_netTipsCtrl then
		self.m_netTipsCtrl:HandleWidgetClick(gb)
	end
end
--------------------------------------------------------------------------------

function page_scrollview_ctrl:InitNetTipsCtrl()
    self.m_netTipsCtrl = UINetTipsCtrl.New()

	self.m_loadFunc = function (...)
		self:Request(
		function(body, head)
			DwDebug.LogWarning("xxx", self.__cname, "recv data", body)
            if self.m_lua_win_root and self.m_lua_win_root.m_open then
                self.m_netTipsCtrl:StopWork(true)
				if not body or not body.data or not body.data.list then
					return
				else 
					self:LoadSuc(body.data)
				end
            end
        end,
        function(body, head)
			DwDebug.Log("WebNetHelper RequestCheckAgent failed")
			self.m_lua_win_root:SetActive(self.m_params.m_root, false)
            self.m_netTipsCtrl:StopWork(false)
        end, ...)
	end
    self.m_netTipsCtrl:Init(self.m_lua_win_root,self.m_lua_win_root:GetTrans("netTipsRoot"), self.m_loadFunc)
end

function page_scrollview_ctrl:DestroyNetTipsCtrl()
    if self.m_netTipsCtrl then
        self.m_netTipsCtrl:Destroy()
        self.m_netTipsCtrl = nil
	end
end

-- 取数据并在数据返回后设置ui显示
function page_scrollview_ctrl:TryRenderUI(show_root_flag, page_index, page_size)
	self.m_data_list = {}
	self.m_curr_page = page_index
	self.m_lua_win_root:SetActive(self.m_params.m_root, false)
    self.m_netTipsCtrl:StartWork(page_index, page_size)
end

function page_scrollview_ctrl:CleanDealyTask()
	if self.m_timeout_event then
		TimerTaskSys.RemoveTask(self.m_timeout_event)
	end
end

-- 注册预加载函数
function page_scrollview_ctrl:RegisterPreloadFunc()
	local preload_func = function () 
		self:PreRequestData()
	end
	self.m_preload_func_seq = LuaCsharpFuncSys.RegisterFunc(preload_func)
end

-- 注销预加载函数，每次注册前调用一下，关闭窗口调用一下
function page_scrollview_ctrl:UnRegisterPreloadFunc()
	if self.m_preload_func_seq then
		LuaCsharpFuncSys.UnRegisterFunc(self.m_preload_func_seq)
		self.m_preload_func_seq = nil	
	end
end

-- 初始化滑动列表
function page_scrollview_ctrl:InitScrollView()
	if self.m_panel_scrollView == nil then
		local rootTrans = self.m_lua_win_root:GetTrans(self.m_params.m_root, "record_scrollview")
		self.m_panel_scrollView = EZfunLimitScrollView.GetOrAddLimitScr(rootTrans)
		local item = self.m_lua_win_root:GetTrans(self.m_params.m_root, "record").gameObject
		self.m_panel_scrollView:InitForLua(rootTrans, item, UnityEngine.Vector2.New(0, 0), self.m_params.m_show_num, LimitScrollViewDirection.SVD_Vertical, false)

		local initFunc = function (trans, item_index) 
			self:InitTrans(trans, item_index + 1, self.m_data_list[item_index + 1])
		end
		self.m_initFuncSeq = LuaCsharpFuncSys.RegisterFunc(initFunc)

		self.m_panel_scrollView:SetInitItemCallLua(self.m_initFuncSeq)
	end 
end

-- 清理滑动列表
function page_scrollview_ctrl:ClearScrollView()
	if self.m_initFuncSeq then
		LuaCsharpFuncSys.UnRegisterFunc(self.m_initFuncSeq)
		self.m_initFuncSeq = nil	
	end
	self:UnRegisterPreloadFunc()
	self.m_panel_scrollView = nil    
end

-- 渲染加载失败状态
function page_scrollview_ctrl:LoadFailed()
	self.m_lua_win_root:SetActive(self.m_params.m_root, false)
end

-- 渲染加载状态
function page_scrollview_ctrl:Loading(show_root_flag)
	self.m_lua_win_root:SetActive(self.m_params.m_root, show_root_flag)
end

-- 渲染正常状态
function page_scrollview_ctrl:LoadSuc(data)
	self.m_lua_win_root:SetActive(self.m_params.m_root, true)
	self:RenderNormalList(data)
end

function page_scrollview_ctrl:RenderNormalList(data)
	self:InitData(data)
	local params = data.list
	if self.m_panel_scrollView and self.m_data_list then
		self.m_panel_scrollView:InitItemCount(#self.m_data_list, true)
		-- 后边还有数据，注册预加载函数
		if params.page_index and params.page_count and params.page_index < params.page_count then
			self:UnRegisterPreloadFunc()
			self:RegisterPreloadFunc()
			self.m_panel_scrollView:InitPreLoadDataForLua(10, self.m_preload_func_seq)
		end
	end
end

function page_scrollview_ctrl:InitData(data)
	if not self.m_data_list then
		self.m_data_list = {}
	end

	for i,v in ipairs(data.list.items or {}) do 
		table.insert(self.m_data_list, v)
	end
end


function page_scrollview_ctrl:PreRequestData()
	--请求数据回调里调用下两句
	self:Request(
		function(body, head)
			local recv_data
			if not body or not body.data or not body.data.list then
				return
			else 
				recv_data = body.data
			end
			
			self.m_curr_page = self.m_curr_page + 1
			self:RenderNormalList(recv_data)
		end,
		function(body, head)
			-- TODOTODO 没有测过请求失败的情况
			print("下页数据请求失败")
		end
		,self.m_curr_page + 1, 20)
end


