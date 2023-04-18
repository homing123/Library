using UnityEngine;
#region enum
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
    None,
    Purchase_Available = 1, //구매가능
    Purchase_Not_Available = 2, //구매불가능
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
    Available = 0,
    Lack_Of_Price = 1,
    No_Ads = 2,
}

public enum E_DataRead_State 
{
    None,
    Fail,
    Success
}

public enum E_Language
{
    Kor=0,
    Eng=1,
}
public enum E_Text_Kind
{
    Text = 0,
    Lang_ID = 1,
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
public enum E_ReservationTime_State
{
    Before,
    After,
}
public enum E_ReservationTime_Kind
{
    Store,
}
public enum E_LoginState
{
    None = 0,
    Guest = 1,
    Google = 2,
    Apple = 3
}
public enum E_DataConnector_Request
{
    None = 0, //기본상태
    Success = 1, //성공
    No_Network_Connection = 2, //네트워크 연결안됨 
    Not_Logged_In = 3, //로그인 안됨
    File_None = 4, //파일 없음
    Fail_Unknown = 5, //실패 알수없음
}
#endregion
#region Struct
#endregion
#region class
public class ReservationTime_Info
{
    public DateTime Reservation_Time;
    public int Remaining_Time;
    public E_ReservationTime_State Cur_State;
    public ReservationTime_Info(Data_User.User_Reservation_Time user_reservation_time)
    {
        Reservation_Time = System.DateTime.Parse(user_reservation_time.End_Time);
        Remaining_Time = TimeManager.Instance.Get_RemainingTime(Reservation_Time);
        Cur_State = Remaining_Time > 0 ? E_ReservationTime_State.Before : E_ReservationTime_State.After;
    }
}
#endregion
public class Define : MonoBehaviour
{
    //사운드
    public static float Sound_Volume = 1f;
    public static float BGM_Volume = 1f;
    public static float BGM_Fade_Speed = 0.5f;

    //광고
    public static float AD_ReLoad_Time;

}
#region Singleton
public class SingleMono<T> : MonoBehaviour where T : MonoBehaviour
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

public class Single<T> where T : new()
{
    private static T m_Instance;

}

//인스턴스로 부르면 싱글톤개념으로 세팅해서 생성됨
//세팅함수를 공

public abstract class Single_Data<T> where T : Single_Data<T>, new()
{
    static T m_Instance;
    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = Data_Reader.ReadData<T>(Get_Name()); ;
                m_Instance.Setting();
            }
            return m_Instance;
        }
    }
    static string Name;
    public static string Get_Name()
    {
        if (Name == null) 
        {
            Name = typeof(T).Name.Remove(0, 5);
        }
        return Name;
    }
    public static T Get_New()
    {
        if (m_Instance == null)
        {
            m_Instance = new T();
            return m_Instance;
        }
        else
        {
            return null;
        }
    }
    public abstract void Setting();

}
#endregion
#region extends
public static class Ex_Define
{
    public static E_Language Get_LanguageKind(this SystemLanguage systemlanguage)
    {
        switch (systemlanguage)
        {
            case SystemLanguage.Korean:
                return E_Language.Kor;
            default:
                return E_Language.Eng;
        }
    }
}


#endregion
