using UnityEngine;
using System.Collections;
using FMODUnity;

public interface IFmodBankProvider
{
	/// <summary>
	/// 根据bankName提供一个可用的路径，比如对于热更的情况：
	/// 在StreamingAssets目录有一份随着包出去的资源，在
	/// PersistentPath也有一份热更的资源，此接口需要选用最新的path
	/// </summary>
	/// <returns>The available path.</returns>
	/// <param name="bankName">Bank name.</param>
	string GetAvailablePath(string bankName);
}

/// <summary>
/// Fmod的封装，支持特性：
/// 1、控制同时播放的最大音效数 (TODO)
/// 2、控制整体背景音乐、音效、环境音的音量 (TODO)
/// 3、fade in/out效果
/// 4、管理背景音乐，同时只播放一首。播放新的，fadeout老的
/// 5、3d音效，指定pos播放音效；attach音效到gameobject
/// 
/// 
/// TIPS:
/// 用VCA来分别控制音量
/// 常用的音效可以常驻内存，否则使用PlayOnShot
/// bank划分，权衡热更代价和加载开销的平衡
/// 
/// </summary>
public class FmodAudioSys {

	private IFmodBankProvider m_bankProvider;

	private const string MASTER_BANK_STRING_NAME = "Master Bank.strings";
	private const string MASTER_BANK_NAME = "Master Bank";

	private bool m_hasInit = false;
	private FMOD.Studio.System m_studioSys;
	private FMOD.System m_lowLevelSys;

	private FMOD.Studio.VCA m_musicVCA;
	private FMOD.Studio.VCA m_effectVCA;

	public void SetMusicVolumn(float volumn)
	{
		if (m_musicVCA != null) {
			m_musicVCA.setVolume (volumn);
		} else {
			Debug.LogError ("music CVA is null");
		}
	}

	public void SetEffectVolumn(float volumn)
	{
		if (m_effectVCA != null) {
			m_effectVCA.setVolume (volumn);
		} else {
			Debug.LogError ("effect CVA is null");
		}
	}

	public float GetMusicVolumn()
	{
		if (m_musicVCA != null) {
			float volumn = 0f;float finalvolume = 0f;
			m_musicVCA.getVolume (out volumn, out finalvolume);
			return volumn;
		} else {
			Debug.LogError ("m_musicVCA CVA is null");
			return 0f;
		}
	}

	public void SetMusicMute(bool mute)
	{
		if (m_musicVCA != null) {
			var result = m_musicVCA.setVolume (mute? 0f : PlayerPrefs.GetFloat ("MusicVolume", 1f));
			Debug.Log ("Set VCA Volume,Result:" + result.ToString());

		} else {
			Debug.LogError ("music CVA is null, please check fmod project settings");
		}
	}

	public void SetEffectMute(bool mute)
	{
		if (m_effectVCA != null) {
			m_effectVCA.setVolume (mute? 0f : PlayerPrefs.GetFloat ("EffectVolume", 1f));
		} else {
			Debug.LogError ("effect CVA is null");
		}
	}

	public float GetEffectVolumn()
	{
		if (m_effectVCA != null) {
			float volumn = 0f;float finalvolume = 0f;
			m_effectVCA.getVolume (out volumn, out finalvolume);
			return volumn;
		} else {
			Debug.LogError ("effect CVA is null");
			return 0f;
		}	}

	public void Init(IFmodBankProvider provider)
	{
		if (m_hasInit)
			return;

		m_bankProvider = provider;

		m_studioSys = FMODUnity.RuntimeManager.StudioSystem;
		FMOD.Studio.CPU_USAGE cpuUsage;
        if(m_studioSys != null)
        {
		    m_studioSys.getCPUUsage(out cpuUsage);
            m_lowLevelSys = FMODUnity.RuntimeManager.LowlevelSystem;
            uint version;
            m_lowLevelSys.getVersion(out version);
        }

		LoadBank (MASTER_BANK_STRING_NAME);
		LoadBank (MASTER_BANK_NAME);

		m_hasInit = true;

		//m_lowLevelSys.getChannel ();
		m_musicVCA = RuntimeManager.GetVCA("vca:/Music");
		if (m_musicVCA == null) {
			Debug.LogError ("music CVA is null");
		}

		m_effectVCA = RuntimeManager.GetVCA ("vca:/Effect");
		if (m_effectVCA == null) {
			Debug.LogError ("effect CVA is null");
		}

	}

	public void PauseMusic()
	{
		if (m_currentMusicInstance != null) {
			m_currentMusicInstance.setPaused (true);
		}
	}

	public void ResumeMusic()
	{
		if (m_currentMusicInstance != null) {
			m_currentMusicInstance.setPaused (false);
		}
	}

	public bool IsLoading()
	{
		return FMODUnity.RuntimeManager.AnyBankLoading ();
	}

	public void LoadBank(string bankName, bool loadSamples = false)
	{
		var path = m_bankProvider.GetAvailablePath (bankName);
		LoadBank (bankName, path, loadSamples);
	}

	public void UnLoadBank(string bankName)
	{
		RuntimeManager.UnloadBank (bankName);
	}

	private void LoadBank(string bankName, string bankPath, bool loadSamples = false)
	{
		RuntimeManager.LoadBank(bankName, bankPath, loadSamples);
	}

    public FMOD.Studio.EventInstance PlayEffect(string sound)
	{
		return FMODUnity.RuntimeManager.PlayOneShot (GetEventName(sound));
	}

	public void PlayEffect(string sound, Vector3 position)
	{
		FMODUnity.RuntimeManager.PlayOneShot (GetEventName(sound), position);
	}

	public enum PlayMode
	{
		FadeIn,
	}

	public enum StopMode
	{
		FadeOut,
	}

	private FMOD.Studio.EventInstance m_currentMusicInstance;

	public void PlayMusic(string musicName)
	{
		if (m_currentMusicInstance != null) {
			m_currentMusicInstance.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			m_currentMusicInstance.release ();
		}

		m_currentMusicInstance = FMODUnity.RuntimeManager.CreateInstance (GetEventName(musicName));
		if (m_currentMusicInstance != null)
		{
			m_currentMusicInstance.start ();
		}
	}

	public void PlayMusic(string musicName, params float[] parameters)
	{
		if (m_currentMusicInstance != null) {
			m_currentMusicInstance.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			m_currentMusicInstance.release ();
		}

		m_currentMusicInstance = FMODUnity.RuntimeManager.CreateInstance (GetEventName(musicName));

		if (m_currentMusicInstance != null)
		{
			if (parameters != null && parameters.Length > 0) {
				for (int i = 0; i < parameters.Length; i++) {
					m_currentMusicInstance.setParameterValueByIndex(i, parameters[i]);
				}
			}

			m_currentMusicInstance.start ();
		}

	}

	public void StopMusic()
	{
		if (m_currentMusicInstance != null) {
			m_currentMusicInstance.stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			m_currentMusicInstance = null;
		}
	}

	public void SetCurrentMusicParameters(params float[] parameters)
	{
		if (m_currentMusicInstance == null) {
			Debug.LogError ("current music is null");
			return;
		}

		if (parameters != null && parameters.Length > 0) {
			for (int i = 0; i < parameters.Length; i++) {
				m_currentMusicInstance.setParameterValueByIndex(i, parameters[i]);
			}
		}
	}

	private string GetEventName(string name)
	{
		var eventName = "event:/" + name;
        //Debug.Log ("FmodAudioSys::Get Event Name:" + eventName);

		return eventName;
	}

}
