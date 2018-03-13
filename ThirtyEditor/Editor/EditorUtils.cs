using UnityEngine;
using UnityEditor;

public static class EditorUtils
{
    private static string m_ProgressBarTitle = "";

    public static void BeginProgressBar(string title)
    {
        m_ProgressBarTitle = title;
    }

    public static void ProgressBarUpdate(string detail, float progress)
    {
        EditorUtility.DisplayProgressBar(m_ProgressBarTitle, detail, progress);
    }

    public static void EndProgressBar()
    {
        m_ProgressBarTitle = "";
        EditorUtility.ClearProgressBar();
    }
}
