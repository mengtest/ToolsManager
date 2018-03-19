using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
 
namespace UnityEditor.XCodeEditor
{
	public partial class XClass : System.IDisposable
	{
 
        private string filePath;
 
		public XClass(string fPath)
		{
            filePath = fPath;
			if( !System.IO.File.Exists( filePath ) ) {
					Debug.LogError( filePath +"路径下文件不存在" );
					return;
			}
		}
 
        public void WriteBelow(string below, string text)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();
 
            int beginIndex = text_all.IndexOf(below);
            if(beginIndex == -1){
                Debug.LogError(filePath +"中没有找到标致"+below);
                return; 
            }
 
            int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);
 
            text_all = text_all.Substring(0, endIndex) + "\n"+text+"\n" + text_all.Substring(endIndex);
 
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(text_all);
            streamWriter.Close();
        }
        /// <summary>
        /// 加了一个参数 by shandong
        /// </summary>
        /// <param name="below"></param>
        /// <param name="newText"></param>
        /// <param name="num"></param>
        public void Replace(string below, string newText, int num = 0)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();
            int beginIndex = text_all.IndexOf(below);
            if(beginIndex == -1){
                Debug.LogError(filePath +"中没有找到标致"+below);
                return; 
            }
            if (num == 0)
            {
                text_all = text_all.Replace(below, newText);
            }
            else
            {
                Regex r = new Regex(below);
                text_all = r.Replace(text_all, newText, num);
            }
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(text_all);
            streamWriter.Close();
 
        }
 
        public void Dispose()
        {
 
        }
	}
}