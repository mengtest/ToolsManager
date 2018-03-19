using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ShaderEntry
{
    public string name;
    public string path;
    public ShaderEntry()
    {

    }
}

public class ShaderList
{
    public List<ShaderEntry> shaders = new List<ShaderEntry>();
    public ShaderList() { }
}

