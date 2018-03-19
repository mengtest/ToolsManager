using UnityEngine;
using System.Collections;
using FMODUnity;
using System.Collections.Generic;

[RegisterSystem(typeof(AudioSys), true)]
public class AudioSys : TCoreSystem<AudioSys>, IInitializeable {

    public static AudioSys TInstance
    {
        get { return Instance; }
    }
    public enum SystemState
	{
		NotReady,
		Initializing,
		LoadingBank,
		Ready
	}

	#region IInitializeable implementation
	private SystemState m_state = SystemState.NotReady;
	public SystemState State
	{
		get
		{
			return m_state;
		}
	}

	private FmodAudioSys m_fmodAudioSys;

	private GameObject m_rootNode;
	private FMODUnity.StudioListener m_fmodListener;

	private bool m_isMusicMute;
	private bool m_isEffectMute;
	private float m_MusicVolume;
	private float m_EffectVolume;
	private const string kIsMusicMute = "key_MusicMute";
	private const string kIsEffectMute = "key_EffectMute";

	private bool m_isPlayingMusic = false;

	public void Init ()
	{
		if (m_state != SystemState.NotReady) {
			return;
		}

		GameRoot.Instance.StartCoroutine (_InitCoroutine());

		m_rootNode = new GameObject ("AudioSys");
		m_fmodListener = m_rootNode.AddComponent<FMODUnity.StudioListener> ();
		GameObject.DontDestroyOnLoad (m_fmodListener);

		//读取存储的本地数据
		m_isMusicMute = PlayerPrefs.GetInt(kIsMusicMute) > 0;
		m_isEffectMute = PlayerPrefs.GetInt(kIsEffectMute) > 0;
		m_MusicVolume = PlayerPrefs.GetFloat ("MusicVolume", 1f);
		m_EffectVolume = PlayerPrefs.GetFloat ("EffectVolume", 1f);

		if (m_isMusicMute) {
			m_fmodAudioSys.SetMusicMute (m_isMusicMute);
		} 
		else {
			m_fmodAudioSys.SetMusicVolumn (m_MusicVolume);			
		}
		if (m_isEffectMute) {
			m_fmodAudioSys.SetEffectMute (m_isEffectMute);
		} 
		else {
			m_fmodAudioSys.SetEffectVolumn (m_EffectVolume);
		}
	}

	private IEnumerator _InitCoroutine()
	{
		m_state = SystemState.Initializing;

		m_fmodAudioSys = new FmodAudioSys ();
		m_fmodAudioSys.Init (new DefaultFmodBankProvider ());

		LoadBanks ();

		while (m_fmodAudioSys.IsLoading ()) {
			yield return null;
		}

		m_state = SystemState.Ready;

		m_fmodAudioSys.SetMusicMute (!m_isMusicMute);
		m_fmodAudioSys.SetEffectMute (!m_isEffectMute);
		Debug.Log ("audiosys ready.");
	}

    public FMOD.Studio.EventInstance PlayEffect(string sound)
	{
        //Debug.Log ("AudioSys play sound:" + sound);
	    return m_fmodAudioSys.PlayEffect (sound);
	}

	public void PlayEffect(string sound, Vector3 pos)
	{
		m_fmodAudioSys.PlayEffect (sound, pos);
	}

	private string m_playingMusicName;
	public void PlayMusic(string music)
	{
		Debug.Log ("PlayMusic:" + music);
		if (m_playingMusicName != null) {
			if (m_playingMusicName == music) {
				return;
			}
		}

		m_fmodAudioSys.PlayMusic (music);
		m_playingMusicName = music;
		m_isPlayingMusic = true;
	}

	public void PauseMusic()
	{
		m_fmodAudioSys.PauseMusic ();
	}

	public void ResumeMusic()
	{
		m_fmodAudioSys.ResumeMusic ();
	}

	public void PlayMusic(string music, params float[] parameters)
	{
		Debug.Log ("PlayMusic:" + music);
		Debug.Log ("Parameters count:" + parameters.Length);

		if (m_playingMusicName != null) {
			if (m_playingMusicName == music) {
				return;
			}
		}

		m_fmodAudioSys.PlayMusic (music, parameters);
		m_playingMusicName = music;
		m_isPlayingMusic = true;
	}

	public void StopMusic()
	{
		m_fmodAudioSys.StopMusic ();

		m_playingMusicName = null;
		m_isPlayingMusic = false;
	}

	public bool IsPlayingMusic
	{
		get {
			return m_isPlayingMusic;
		}
	}

	public void SetCurrentMusicParamters(params float[] parameters)
	{
		m_fmodAudioSys.SetCurrentMusicParameters (parameters);
	}

	public void SetMusicVolumn(float vol)
	{
		m_fmodAudioSys.SetMusicVolumn (vol);
	}

	public void SetEffectVolumn(float vol)
	{
		m_fmodAudioSys.SetEffectVolumn (vol);
	}

	public float GetEffectVolumn()
	{
		return m_fmodAudioSys.GetEffectVolumn ();
	}

	public float GetMusicVolumn()
	{
		return m_fmodAudioSys.GetMusicVolumn ();
	}

    public void SetMute(bool mute) 
    {
        SetMusicMute(mute);
        SetEffectMute(mute);
    }

	public void SetMusicMute(bool mute)
	{
		m_fmodAudioSys.SetMusicMute (mute);

		m_isMusicMute = mute;
		PlayerPrefs.SetInt (kIsMusicMute, mute ? 1:0);
		PlayerPrefs.Save ();
	}

	public void SetEffectMute(bool mute)
	{
		m_fmodAudioSys.SetEffectMute (mute);

		m_isEffectMute = mute;
		PlayerPrefs.SetInt (kIsEffectMute, mute ? 1:0);
		PlayerPrefs.Save();
	}

	public bool IsMusicMute
	{
		get
        {
			return m_isMusicMute;
		}
	}

	public bool IsEffectMute
	{
		get
		{
			return m_isEffectMute;
		}
	}

	private void LoadBanks()
	{
		m_fmodAudioSys.LoadBank("Music");
        m_fmodAudioSys.LoadBank("Common");
        //m_fmodAudioSys.LoadBank("50K");
	}

    public void LoadBank(string bankName)
    {
        m_fmodAudioSys.LoadBank(bankName);
    }

    public void UnLoadBank(string bankName)
    {
        m_fmodAudioSys.UnLoadBank(bankName);
    }

    public FmodAudioSys GetFmodAudioSys() 
    {
        return m_fmodAudioSys;
    }

	public void LoadBanks(List<string> banks)
	{
		m_state = SystemState.LoadingBank;
		GameRoot.StartMonoCoroutine (_LoadBankCoroutine (banks));
	}

	private IEnumerator _LoadBankCoroutine(List<string> banks)
	{
		if (banks == null || banks.Count == 0)
			yield break;

		for (int i = 0; i < banks.Count; i++) {
			m_fmodAudioSys.LoadBank (banks[i]);
		}

		while (m_fmodAudioSys.IsLoading ()) {
			yield return null;
		}

		m_state = SystemState.Ready;

		Debug.Log ("audiosys load banks finish.");
	}

	public void Release ()
	{
		
	}

	public void MuteMusicTemporarily()
	{
		m_fmodAudioSys.SetMusicMute (true);
	}

	public void ResumeMusicFromMuteTemporarily()
	{
		if (!m_isMusicMute) {
			m_fmodAudioSys.SetMusicMute (false);
		}
	}
	#endregion

}
