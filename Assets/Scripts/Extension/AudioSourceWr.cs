using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceWr : MonoBehaviour
{
    AudioSource m_Source;
    private void Awake()
    {
        m_Source = GetComponent<AudioSource>();
    }
    private void OnDestroy()
    {
        
    }
    void Ev_VolumeChanged(object sender, (SoundManager.E_SoundType, float) info)
    {
    }
}
