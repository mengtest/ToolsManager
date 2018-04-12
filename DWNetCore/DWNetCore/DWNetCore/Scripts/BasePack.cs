/************************************************************
//     文件名      : IPack.cs
//     功能描述    : 
//     负责人      : guoliang
//     参考文档    : 无
//     创建日期    : 2017-9-20
//     Copyright   : 
**************************************************************/

using System.Collections;

public abstract class BasePack
{
    public short m_errno;

    public bool m_hasData = false;

    public abstract void UnPack(byte[] data, int offset, int length);

    public abstract byte[] PackMsg();
}
