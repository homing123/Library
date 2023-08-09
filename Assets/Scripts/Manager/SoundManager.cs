using UnityEngine;
using UnityEngine.Audio;
public class SoundManager : Manager<SoundManager>
{
    [SerializeField] AudioMixer m_AudioMixer;
    public enum E_SoundType
    {
        BGM,
        SFX,
    }

    AudioSource m_BGMSource;
    AudioSource m_SFXSource;

    private void Awake()
    {
        m_BGMSource = gameObject.AddComponent<AudioSource>();
        m_BGMSource.loop = true;
        m_BGMSource.outputAudioMixerGroup = m_AudioMixer.FindMatchingGroups("BGM")[0];
        m_SFXSource = gameObject.AddComponent<AudioSource>();
        m_SFXSource.loop = false;
        m_SFXSource.outputAudioMixerGroup = m_AudioMixer.FindMatchingGroups("SFX")[0];

    }
    public static AudioClip GetButtonClip()
    {
        return null;
    }
    public void Play_Sound(AudioClip clip)
    {
        m_SFXSource.PlayOneShot(clip);
    }
    public void Play_BGM(AudioClip clip)
    {
        m_BGMSource.Stop();
        m_BGMSource.clip = clip;
        m_BGMSource.Play();
    }

    public void Set_Volume()
    {
        m_AudioMixer.SetFloat()
    }
}
