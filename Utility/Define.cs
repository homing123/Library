using UnityEngine;

public enum E_QuestState
{
    None = 0, //없는
    Complete = 1, //완료된
    Acceptable = 2, //수락가능한
    UnAcceptable = 3, //수락불가능한
    Completable = 4,  //완료가능한
    UnCompletable = 5 //완료불가능한
}
public enum E_QuestType
{
    None=0,
    Daily=1,
    Weekly=2,
}
public enum E_ProductState
{

}
public enum E_ProductType
{
    None=0,

}
public enum E_ItemKind
{
    InAppPurchasing=-2,
    Ad=-1,
    None=0,
    Gold=1,
    Character=100,
}
public enum E_Check_Available_Purchase_Info
{
    Available=0,
    Lack_Of_Price,
}

public enum E_DataRead_State 
{
    None,
    Fail,
    Success
}

public enum E_Language_Kind 
{
    Kor=0,
    Eng=1,
}
public enum E_ADSDK
{
    AdPopcorn
}
public enum E_AdKind
{
    Banner,
    Interstitial,
    Reward_Video
}


public class Define : MonoBehaviour
{
    //사운드
    public static float Sound_Volume = 1f;
    public static float BGM_Volume = 1f;
    public static float BGM_Fade_Speed = 0.5f;

    //광고
    public static float AD_ReLoad_Time;
}

public class Single<T> : MonoBehaviour where T : MonoBehaviour
{
    // Check to see if we're about to be destroyed
    private static bool m_ShuttingDown = false;
    private static object m_Lock = new object();
    private static T m_Instance;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (m_ShuttingDown)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed. Returning null.");
                return null;
            }

            lock (m_Lock)
            {
                if (m_Instance == null)
                {
                    // Search for existing instance.
                    m_Instance = (T)FindObjectOfType(typeof(T));

                    // Create new instance if one doesn't already exist.
                    if (m_Instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var singletonObject = new GameObject();
                        m_Instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";

                        // Make instance persistent.
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return m_Instance;
            }
        }
    }

    private void OnApplicationQuit()
    {
        m_ShuttingDown = true;
    }


    private void OnDestroy()
    {
        m_ShuttingDown = true;
    }
}

public class Single_Data<T> where T : Single_Data<T>
{
    static T m_Instance;
    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = Data_Reader.ReadData<T>(typeof(T).Name.Remove(0, 5));
                m_Instance.Setting();
            }
            return m_Instance;
        }
    }

    public static T Set_Instance
    {
        set
        {
            m_Instance = value;
        }
    }
    public virtual void Setting()
    {

    }

}