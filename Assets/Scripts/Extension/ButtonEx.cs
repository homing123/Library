using UnityEngine;
using UnityEngine.UI;
[AddComponentMenu("Audio Source")]
public class ButtonEx : Button
{
    // ui 사운드는 카메라 오디오소스에 출력

    [SerializeField] AudioClip m_AudioClip;

    protected override void Awake()
    {
        base.Awake();
        if (m_AudioClip == null)
        {
            m_AudioClip = SoundManager.GetButtonClip(); 
        }
        onClick.AddListener(AudioPlay);
    }

    void AudioPlay()
    {
        SoundManager.Instance.Play_Sound(m_AudioClip);
    }
}
