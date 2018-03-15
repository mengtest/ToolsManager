using NIM;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using NIMAudio;

public class DWChatAudio
{
    private string _audioFormat {  set; get; }

    private volatile static DWChatAudio _instance = null;
    private static readonly object lockHelper = new object();
    private string _audioSampleRate;
    private string _audioDuration;
    private bool _initialized;


    private DWChatAudio() { }
    public static DWChatAudio getInstance()
    {
        if (_instance == null)
        {
            lock (lockHelper)
            {
                if (_instance == null)
                    _instance = new DWChatAudio();
            }
        }
        return _instance;
    }

    public void DW_audio2Text(string filepath)
    {
        if (System.IO.File.Exists(filepath))
        {
            _audioFormat = getAudioFormat(filepath);
            NIM.Nos.NosAPI.Upload(filepath, OnAudioFileUploaded, null);
        }
    }

    private void OnAudioFileUploaded(int resCode, string url)
    {
        if (resCode == 200)
        {
            DWChatUtils.DebugLog("语音上传成功，开始转换------");
            NIM.NIMAudioInfo ai = new NIM.NIMAudioInfo();
            ai.MimeType = _audioFormat;
            ai.SampleRate = _audioSampleRate;
            long duratoin;
            if (long.TryParse(_audioDuration, out duratoin))
                ai.Duration = duratoin;
            ai.URL = url;
            //NIM.ToolsAPI.GetAudioTextAsync(ai, null, ConverteAudio2TextCallback);
            NIM.ToolsAPI.ConverteAudio2Text(ai, OnConverteAudioCompleted, "this is a test string");
        }
        else
        {
            DWChatUtils.DebugLog("语音上传失败:" + resCode.ToString());
        }
    }

    private void OnConverteAudioCompleted(int rescode, string text, object userData)
    {
        DWChatUtils.DebugLog(string.Format("语音转文字:{0},{1},{2}", rescode, text, userData));
    }

    private string getAudioFormat(string filepath)
    {
        return "aac";
    }



}