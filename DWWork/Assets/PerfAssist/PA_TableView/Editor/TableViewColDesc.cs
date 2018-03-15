using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

public class TableViewColDesc
{
    public string PropertyName;
    public string TitleText;

    public TextAnchor Alignment;
    public string Format;
    public float WidthInPercent;

    public FieldInfo FieldInfo;

    public PropertyInfo PropertyInfo;

    public string FormatObject(object obj)
    {
        if (FieldInfo != null)
            return PAEditorUtil.FieldToString(obj, FieldInfo, Format);
        if (PropertyInfo != null)
            return PAEditorUtil.FieldToString(obj, PropertyInfo, Format);
        return "";
    }

    public int Compare(object o1, object o2)
    {
        object fv1 = null;
        object fv2 = null;
        if (FieldInfo != null)
            fv1 = PAEditorUtil.FieldValue(o1, FieldInfo);
        if (FieldInfo != null)
            fv2 = PAEditorUtil.FieldValue(o2, FieldInfo);
        if (PropertyInfo != null)
            fv1 = PAEditorUtil.FieldValue(o1, PropertyInfo);
        if (PropertyInfo != null)
            fv2 = PAEditorUtil.FieldValue(o2, PropertyInfo);

        IComparable fc1 = fv1 as IComparable;
        IComparable fc2 = fv2 as IComparable;
        if (fc1 == null || fc2 == null)
        {
            return fv1.ToString().CompareTo(fv2.ToString());
        }

        return fc1.CompareTo(fc2);
    }
}

