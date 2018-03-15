
#ifndef MACRO_VARIADIC_ARGS_COUNT
#error 未定义可变参数数量宏，无法定义函数对象。
#endif

#define MACRO_FASTEVENT_CLASS_NAME						MACRO_FASTEVENT_CLASS_NAME_ARG( MACRO_VARIADIC_ARGS_COUNT )
#define MACRO_FASTEVENT_VARIADIC_ARGS					MACRO_VARIADIC_ARGS

template <typename TReturn MACRO_FASTEVENT_VARIADIC_ARGS(MACRO_FASTEVENT_TYPE_LIST_ARG, MACRO_COMMA, MACRO_COMMA)>
class MACRO_FASTEVENT_CLASS_NAME
{
public:
	typedef TReturn(*FnCallback)(MACRO_FASTEVENT_VARIADIC_ARGS(MACRO_FASTEVENT_PARAM_LIST_ARG, MACRO_EMPTY, MACRO_COMMA));

	/// <summary>
	/// 调用事件处理函数
	/// </summary>
	void Invoke(MACRO_FASTEVENT_VARIADIC_ARGS(MACRO_FASTEVENT_PARAM_LIST_ARG, MACRO_EMPTY, MACRO_COMMA))
	{
		typename set<FnCallback>::iterator iterCur = m_SetHandlers.begin();
		typename set<FnCallback>::iterator iterEnd = m_SetHandlers.end();
		for (; iterCur != iterEnd; ++iterCur)
		{
			FnCallback handlerCallback = *iterCur;
			handlerCallback(MACRO_FASTEVENT_VARIADIC_ARGS(MACRO_FASTEVENT_PARAM_NAME_ARG, MACRO_EMPTY, MACRO_COMMA));
		}
	}

	/// <summary>
	/// 注册事件处理回调函数，如已重复则不会重复注册。
	/// </summary>
	/// <param name="handlerCallback">回调函数</param>
	void RegisterHandler(FnCallback handlerCallback)
	{
		m_SetHandlers.insert(handlerCallback);
	}

	/// <summary>
	/// 注销事件处理回调函数，如不存在则直接返回。
	/// </summary>
	/// <param name="handlerCallback">回调函数</param>
	void UnregisterHandler(FnCallback handlerCallback)
	{
		m_SetHandlers.erase(handlerCallback);
	}

protected:
	set<FnCallback>	m_SetHandlers;
};



#undef MACRO_VARIADIC_ARGS_COUNT
#undef MACRO_FASTEVENT_CLASS_NAME
#undef MACRO_FASTEVENT_VARIADIC_ARGS
