using UnityEngine;
using System.Collections;

public class DwObject {
    public object value;
    public DwObject(){ }
    public DwObject(double obj)
    {
        value = obj;
    }
    public DwObject(long obj)
    {
        value = obj;
    }
    public DwObject(float obj)
    {
        value = obj;
    }
    public DwObject(int obj)
    {
        value = obj;
    }
    public DwObject(string obj)
    {
        value = obj;
    }

    public DwObject(short obj)
    {
        value = obj;
    }
}
