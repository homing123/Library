using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.IO;
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] AudioMixer m_AudioMixer;

    public enum E_SoundType
    {
        BGM,
        SFX,
        Echo,
    }

    static Dictionary<E_SoundType, string> D_Mixer = new Dictionary<E_SoundType, string>() { { E_SoundType.BGM, "BGM" }, { E_SoundType.SFX, "SFX" } ,
        {E_SoundType.Echo, "Echo" } };

    AudioSource m_BGMSource;
    AudioSource m_SFXSource;
    AudioSource m_EchoSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        User_Sound.m_UserSound = new User_Sound();
        m_BGMSource = gameObject.AddComponent<AudioSource>();
        m_BGMSource.loop = true;
        m_BGMSource.outputAudioMixerGroup = m_AudioMixer.FindMatchingGroups("BGM")[0];
        m_SFXSource = gameObject.AddComponent<AudioSource>();
        m_SFXSource.loop = false;
        m_SFXSource.outputAudioMixerGroup = m_AudioMixer.FindMatchingGroups("SFX")[0];
        m_EchoSource = gameObject.AddComponent<AudioSource>();
        m_EchoSource.loop = false;
        m_EchoSource.outputAudioMixerGroup = m_AudioMixer.FindMatchingGroups("Echo")[0];

        GameManager.ac_DataLoaded += Ac_DataLoaded;
    }

    void Ac_DataLoaded()
    {
        Set_Volume(E_SoundType.BGM, User_Sound.m_UserSound.BGM_Volume);
        Set_Volume(E_SoundType.SFX, User_Sound.m_UserSound.SFX_Volume);
    }
  
    public static AudioClip GetButtonClip()
    {
        return null;
    }
    public void Play_Sound(AudioClip clip)
    {
        m_SFXSource.PlayOneShot(clip);
    }
    public void Play_Echo(AudioClip clip)
    {
        m_EchoSource.PlayOneShot(clip);
    }
    public void Play_BGM(AudioClip clip)
    {
        m_BGMSource.Stop();
        m_BGMSource.clip = clip;
        m_BGMSource.Play();
    }
    public void Play_Vibration()
    {
        if (User_Sound.m_UserSound.Vibration)
        {
            Vibration.Vibrate();
        }
    }
    public void Set_Vibration(bool enable)
    {
        User_Sound.m_UserSound.Vibration = enable;
    }
    /// <summary>
    /// volume range = (0 ~ 1)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="volume"></param>
    public void Set_Volume(E_SoundType type, float volume)
    {
        m_AudioMixer.SetFloat(D_Mixer[type], User_To_AM(volume));
    }
    float AM_To_User(float audiomixer_volume)
    { 
        if (audiomixer_volume == -80)
        {
            return 0;
        }
        else
        {
            return audiomixer_volume * 0.025f + 1;
        }
    }
    float User_To_AM(float user_volume)
    {
        //volume (0 ~ 1) => (-40 ~ 0)
        if (user_volume == 0)
        {
            return -80;
        }
        else
        {
            return (user_volume - 1) * 40;
        }
    }
    /// <summary>
    /// 소리 설정값은 바뀔때마다 저장이 아닌 변경 완료 후 저장되도록 해야함 즉 savedata를 따로 불러야함
    /// </summary>
    public void SaveData()
    {
        float bgm_volume, sfx_volume;
        m_AudioMixer.GetFloat(D_Mixer[E_SoundType.BGM], out bgm_volume);
        User_Sound.m_UserSound.BGM_Volume = AM_To_User(bgm_volume);

        m_AudioMixer.GetFloat(D_Mixer[E_SoundType.SFX], out sfx_volume);
        User_Sound.m_UserSound.SFX_Volume = AM_To_User(sfx_volume);

        UserManager.Save_LocalUD(User_Sound.Path, User_Sound.m_UserSound);
    }

    #region Test
    [SerializeField] float Test_BGMVolume;
    [SerializeField] float Test_SFXVolume;
    [ContextMenu("저장")]
    public void Test_Save()
    {
        Set_Volume(E_SoundType.BGM, Test_BGMVolume);
        Set_Volume(E_SoundType.SFX, Test_SFXVolume);
        SaveData();
    }

    [ContextMenu("로그")]
    public void Test_Log()
    {
        Debug.Log("Volume : bgm " + User_Sound.m_UserSound.BGM_Volume + " volume " + User_Sound.m_UserSound.SFX_Volume);
    }
    #endregion
}

public class User_Sound : UserData_Local
{
    public const string Path = "Sound";
    public static User_Sound m_UserSound;

    public float SFX_Volume;
    public float BGM_Volume;
    public bool Vibration;
    public override void Load()
    {
        if (UserManager.Exist_LocalUD(Path))
        {
            var data = UserManager.Load_LocalUD<User_Sound>(Path);
            SFX_Volume = data.SFX_Volume;
            BGM_Volume = data.BGM_Volume;
            Vibration = data.Vibration;
        }
        else
        {
            Debug.Log("Sound_Init");

            SFX_Volume = 1;
            BGM_Volume = 1;
            Vibration = true;
            UserManager.Save_LocalUD(Path, this);
        }
    }
}