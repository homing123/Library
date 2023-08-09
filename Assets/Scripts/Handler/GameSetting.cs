using UnityEngine;

public class GameSetting : MonoBehaviour
{
    public static GameSetting Instance;
    
    [SerializeField] string m_Version;
    [SerializeField] bool m_Cheat;
    [SerializeField] bool m_Ad;

    public static string Version
    {
        get
        {
            if(Instance == null)
            {
                Instance = FindObjectOfType<GameSetting>();
            }
            return Instance.m_Version;
        }
    }

    public static bool Cheat
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<GameSetting>();
            }
            return Instance.m_Cheat;
        }
    }

    public static bool Ad
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<GameSetting>();
            }
            return Instance.m_Ad;
        }
    }

}
