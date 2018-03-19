/************************************************************
//     文件名      : UpdateFilter.cs
//     功能描述    : 
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 19/03/2018
//     Copyright  : 
**************************************************************/
using System;
using System.Collections.Generic;
using UpdateDefineSpace;


public class BaseUpdateResFilter
{
    protected BaseUpdateExecutor m_executer;
    public BaseUpdateResFilter()
    {

    }
    public BaseUpdateResFilter(BaseUpdateExecutor executer)
    {
        m_executer = executer;
    }
}

public class ParentUpdateResFilter : BaseUpdateResFilter
{
    public ParentUpdateResFilter()
    {

    }
    public ParentUpdateResFilter(BaseUpdateExecutor executer)
    {
        m_executer = executer;
    }

    public void CheckNeedUpdate(List<ParentResInfo> list, ParentResInfo info)
    {
        if (string.IsNullOrEmpty(info.type))
        {
            return;
        }
        list.Add(info);
    }
}


public class ChildUpdateResFilter : BaseUpdateResFilter
{
    public ChildUpdateResFilter()
    {

    }
    public ChildUpdateResFilter(BaseUpdateExecutor executer)
    {
        m_executer = executer;
    }

    public void CheckNeedUpdate(List<ChildResInfo> list, ChildResInfo info)
    {
        if (string.IsNullOrEmpty(info.type))
        {
            return;
        }
        list.Add(info);
    }
}
