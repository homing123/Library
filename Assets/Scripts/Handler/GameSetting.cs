using UnityEngine;
using static AdManager;

public class GameSetting : MonoBehaviour
{
    public static GameSetting Instance;
    
    [SerializeField] string m_Version;
    [SerializeField] bool m_Cheat;
    [SerializeField] bool m_Ad;
    [SerializeField] E_AdSdk m_AdSdk;
    [SerializeField] bool m_Login;
    [SerializeField] bool m_VersionCheck;
    [SerializeField] bool m_Inspection;
    [SerializeField] bool m_ServerStreamingData;

    public static E_AdSdk AdSdk
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<GameSetting>();
            }
            return Instance.m_AdSdk;
        }
    }
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
    public static bool Login
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<GameSetting>();
            }
            return Instance.m_Login;
        }
    }

    public static bool VersionCheck
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<GameSetting>();
            }
            return Instance.m_VersionCheck;
        }
    }
    public static bool Inspection
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<GameSetting>();
            }
            return Instance.m_Inspection;
        }
    }
    public static bool ServerStreamingData
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType<GameSetting>();
            }
            return Instance.m_ServerStreamingData;
        }
    }


}
