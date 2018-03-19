using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

public class ProfilerUtil
{
	private const string HtmlOutputPath = "profiler.txt";
	private const string TAB = "    ";
	private const string SPACE = "    ";
	private const string LONG_SPACE = "                                ";
	[MenuItem("EZFun/Dump selected profiler frame to txt")]
	public static void DumpProfilerFrame()
	{
		var property = new ProfilerProperty();
		property.SetRoot(GetSelectedFrame(), ProfilerColumn.TotalPercent, ProfilerViewType.Hierarchy);
		property.onlyShowGPUSamples = false;

		if (File.Exists(HtmlOutputPath))
			File.Delete(HtmlOutputPath);
		var stream = File.OpenWrite(HtmlOutputPath);
		var writer = new StreamWriter(stream);

		while (property.Next(true))
		{
			writer.Write(GetTagSpace(property.depth));
			writer.Write(property.GetColumn(ProfilerColumn.FunctionName));

//			writer.Write("<td>");
//			writer.Write(property.GetColumn(ProfilerColumn.TotalPercent));
//			writer.Write("</td>");
//
//			writer.Write("<td>");
//			writer.Write(property.GetColumn(ProfilerColumn.SelfPercent));
//			writer.Write("</td>");
//
//			writer.Write("<td>");
//			writer.Write(property.GetColumn(ProfilerColumn.Calls));
//			writer.Write("</td>");
//
			writer.Write (LONG_SPACE);
			writer.Write(property.GetColumn(ProfilerColumn.GCMemory));

			writer.Write (SPACE);
			writer.Write(property.GetColumn(ProfilerColumn.TotalTime));
			writer.Write ("ms");

			writer.Write (SPACE);
			writer.Write(property.GetColumn(ProfilerColumn.SelfTime));
			writer.Write ("ms");

			writer.Write (writer.NewLine);

		}

		writer.Close();
	}

	private static System.Text.StringBuilder m_sb = new System.Text.StringBuilder();
	private static string GetTagSpace(int num)
	{
		if (num == 0) {
			return string.Empty;
		}

		m_sb.Remove (0, m_sb.Length);

		for (int i = 0; i < num; i++) {

			m_sb.Append (TAB);

		}

		return m_sb.ToString ();
	}

	private static int GetSelectedFrame()
	{
		var editorAssembly = Assembly.GetAssembly(typeof(EditorApplication));
		var profilerWindowType = editorAssembly.GetType("UnityEditor.ProfilerWindow");
		var profilerWindowsField = profilerWindowType.GetField("m_ProfilerWindows", BindingFlags.NonPublic | BindingFlags.Static);
		var firstProfilerWindow = ((System.Collections.IList) profilerWindowsField.GetValue(null))[0];
		var getFrameMethod = profilerWindowType.GetMethod("GetActiveVisibleFrameIndex");
		return (int) getFrameMethod.Invoke(firstProfilerWindow, null);
	}
}