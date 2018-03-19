/************************************************************
     File      : UIModuleType.cs
     author    : shandong   shandong@ezfun.cn
     version   : 1.0
     date      : 2016-07-28 21:36:48.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System.Collections;
using System;


public class ModuleFieldAttribute : Attribute
{
    public Type m_moduleType;

    public string m_luaFile;

    /// <summary>
    /// c# module
    /// </summary>
    /// <param name="moduleType"></param>
    public ModuleFieldAttribute(Type moduleType)
    {
        this.m_moduleType = moduleType;
    }
    /// <summary>
    /// lua module
    /// </summary>
    /// <param name="luaFile"></param>
    public ModuleFieldAttribute(string luaFile)
    {
        this.m_luaFile = luaFile;
    }
    public ModuleFieldAttribute()
    {
       
    }
}

[LuaWrap]
public enum ModuleType
{
    [ModuleFieldAttribute]
    LuaModuleType,
}
