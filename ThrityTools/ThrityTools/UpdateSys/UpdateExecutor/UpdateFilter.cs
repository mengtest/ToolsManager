/************************************************************
//     �ļ���      : UpdateFilter.cs
//     ��������    : 
//     ������      : guoliang
//     �ο��ĵ�    : ��
//     ��������    : 19/03/2018
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
