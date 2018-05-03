using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class UpdateResFilter : IUpdateFilter
{
    private BaseUpdateExecutor m_executer;

    public UpdateResFilter(BaseUpdateExecutor executer)
    {
        this.m_executer = executer;
    }

    public void CheckNeedUpdate(List<UpdateInfo.ResInfo> list, UpdateInfo.ResInfo info)
    {
        if (string.IsNullOrEmpty(info.type))
        {
            return;
        }
        list.Add(info);
    }
}
