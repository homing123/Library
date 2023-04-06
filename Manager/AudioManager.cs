using UnityEngine;
using System;
using System.Collections;
using static Data_User;
using static Define;

public class AudioManager : Single<AudioManager>
{
    public static event EventHandler Ev_Sound_On;
    public static event EventHandler Ev_Sound_Off;

    public static event EventHandler Ev_BGM_On;
    public static event EventHandler Ev_BGM_Off;
    AudioSource BGM_Source;
    AudioSource Sound_Source;

    Coroutine m_BGM_Fade;

    public AudioManager()
    {
        BGM_Source = gameObject.AddComponent<AudioSource>();
        Sound_Source = gameObject.AddComponent<AudioSource>();
        gameObject.AddComponent<AudioListener>();

        BGM_Source.volume = UserData.BGM_On ? BGM_Volume : 0;
        Sound_Source.volume = UserData.Sound_On ? Sound_Volume : 0;

    }
    IEnumerator BGM_Fade(int bgm_id)
    {
        WaitForEndOfFrame frame = new WaitForEndOfFrame();
        while (BGM_Source.volume > 0)
        {
            //bgm ²ô´Â°æ¿ì
            if (UserData.BGM_On == false )
            {
                BGM_Source.clip = Data_Audio.Instance.Get_Audio(bgm_id);
                yield break;
            }
            BGM_Source.volume -= BGM_Fade_Speed * Time.deltaTime;
            if (BGM_Source.volume < 0)
            {
                BGM_Source.volume = 0;
                break;
            }

            yield return frame;
        }

        BGM_Source.clip = Data_Audio.Instance.Get_Audio(bgm_id);

        while (BGM_Source.volume < BGM_Volume)
        {
            if (UserData.BGM_On == false)
            {
                yield break;
            }
            BGM_Source.volume += BGM_Fade_Speed * Time.deltaTime;
            if (BGM_Source.volume > BGM_Volume)
            {
                BGM_Source.volume = BGM_Volume;
                break;
            }
            yield return frame;
        }
    }
    public void Add_Sound(int sound_id)
    {
        Sound_Source.PlayOneShot(Data_Audio.Instance.Get_Audio(sound_id));
    }
    public void Change_BGM(int bgm_id)
    {
        if (GameManager.Instance.Use_BGM_Fade)
        {
            if (UserData.BGM_On == false)
            {
                BGM_Source.clip = Data_Audio.Instance.Get_Audio(bgm_id);
            }
            else
            {
                if (m_BGM_Fade != null)
                {
                    StopCoroutine(m_BGM_Fade);
                }
                m_BGM_Fade = StartCoroutine(BGM_Fade(bgm_id));
            }
        }
        else
        {
            BGM_Source.clip = Data_Audio.Instance.Get_Audio(bgm_id);
        }
    }
    
    public void Sound_Change(bool sound_on)
    {
        if (UserData.Sound_On != sound_on)
        {
            if (UserData.Sound_On)
            {
                UserData.Sound_On = false; 
                Sound_Source.volume = 0;
                Ev_Sound_Off?.Invoke(null, EventArgs.Empty);
            }
            else
            {
                UserData.Sound_On = true;
                Sound_Source.volume = Sound_Volume;
                Ev_Sound_On?.Invoke(null, EventArgs.Empty);
            }
            Data.Instance.UserData_Changed();
        }
    }

    public void BGM_Change(bool bgm_on)
    {
        if (UserData.BGM_On != bgm_on)
        {
            if (UserData.BGM_On)
            {
                UserData.BGM_On = false;
                BGM_Source.volume = 0;
                Ev_BGM_Off?.Invoke(null, EventArgs.Empty);
            }
            else
            {
                UserData.BGM_On = true;
                BGM_Source.volume = BGM_Volume;
                Ev_BGM_On?.Invoke(null, EventArgs.Empty);
            }
            Data.Instance.UserData_Changed();
        }
    }

}
