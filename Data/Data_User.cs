using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using static Data_User;
using static UD_Func;

public class Data_User
{
    public static Data_User UserData;
    public static event EventHandler Ev_UserData_Set;
    public static void Set_UserData(Data_User userdata)
    {
        UserData = userdata;

        for(int i=0;i< UserData.L_Mission.Count; i++)
        {
            Mission mission = UserData.L_Mission[i];
            UserData.D_Mission.Add(mission.Mission_Kind, mission.Mission_Value);
            QuestManager.Call_MissionEvent(mission.Mission_Kind);
        }

        Ev_UserData_Set?.Invoke(UserData, EventArgs.Empty);
    }
    public static void SaveSetting_UserData()
    {
        UserData.L_Mission.Clear();
        foreach(KeyValuePair<int, int> keys in UserData.D_Mission)
        {
            Mission mission = new Mission();
            mission.Mission_Kind = keys.Key;
            mission.Mission_Value = keys.Value;
            UserData.L_Mission.Add(mission);
        }
    }

    public E_LoginState LoginState;

    public int Daily_Update_Year; //일일갱신 년도
    public int Daily_Update_Month; //일일갱신 월
    public int Daily_Update_Day; //일일갱신 일

    public int Attendance_Count; //출석일수
    public int Access_Time; //접속시간 (초)

    public E_Language Language_Kind; //언어 종류
    public bool Sound_On;
    public bool BGM_On;
    public bool Vibration_On;

    public Inventory[] Arr_Inventory = new Inventory[0]; //인벤 리스트

    public List<Mission> L_Mission = new List<Mission>();
    public Dictionary<int, int> D_Mission = new Dictionary<int, int>();
    public List<int> L_Quest = new List<int>(); //진행중인 퀘스트
    public List<int> L_Clear_Quest = new List<int>(); //클리어한 퀘스트

    public List<User_Reservation_Time> L_Reservation_Time = new List<User_Reservation_Time>(); //예약시간 리스트

    [Serializable]
    public class Inventory 
    {
        public E_ItemKind Item_Kind;
        public int Item_ID;
        public int Count;
    }
    [Serializable]
    public class Mission
    {
        public int Mission_Kind;
        public int Mission_Value;
    }
    [Serializable]
    public class User_Reservation_Time
    {
        public string End_Time;
        public E_ReservationTime_Kind Key;
        public int Idx;
    }


}

public class UD_Func
{
    #region Time
    public static System.DateTime Get_Daily_Update()
    {
        return new System.DateTime(UserData.Daily_Update_Year, UserData.Daily_Update_Month, UserData.Daily_Update_Day);
    }
    #endregion
    #region Inventory
    // 인벤 아이템클래스 가져오는 함수 같은아이템 없을경우 null 리턴
    public static Data_User.Inventory Inven_Get_Item(E_ItemKind kind, int item_id)
    {
        for(int i = 0; i < Data_User.UserData.Arr_Inventory.Length; i++)
        {
            if(Data_User.UserData.Arr_Inventory[i].Item_Kind==kind && Data_User.UserData.Arr_Inventory[i].Item_ID == item_id)
            {
                return Data_User.UserData.Arr_Inventory[i];
            }
        }
        return null;
    }
    //인벤 빈 배열 아이템클래스 가져오는 함수 배열 빈칸 없을경우에 null 리턴
    public static Data_User.Inventory Inven_Get_Empty()
    {
        for(int i = 0; i < Data_User.UserData.Arr_Inventory.Length; i++)
        {
            if (Data_User.UserData.Arr_Inventory[i].Item_Kind == E_ItemKind.None)
            {
                return Data_User.UserData.Arr_Inventory[i];
            }
        }
        return null;
    }
    #endregion
    #region Quest
    public static void Add_MissionValue(int mission_kind, int mission_value)
    {
        if (UserData.D_Mission.ContainsKey(mission_kind) == false)
        {
            UserData.D_Mission.Add(mission_kind, mission_value);
        }
        else
        {
            UserData.D_Mission[mission_kind] += mission_value;
        }

        QuestManager.Call_MissionEvent(mission_kind);
    }
    #endregion
    #region User
    readonly public static string UserData_FileName = "UserData";

    public static void UserData_Save()
    {
        if (UserData != null)
        {
            SaveSetting_UserData();
            File.WriteAllText(Path.Combine(Application.persistentDataPath, UserData_FileName), JsonUtility.ToJson(UserData));
        }
    }
    public static Data_User UserData_Load()
    {
        string path = Path.Combine(Application.persistentDataPath, UserData_FileName);
        Debug.Log(Path.Combine(Application.persistentDataPath, UserData_FileName));
        if (File.Exists(path))
        {
            return JsonUtility.FromJson<Data_User>(File.ReadAllText(path)); ;
        }
        else
        {
            return null;
        }
    }
    public static void UserData_Delete()
    {
        string path = Path.Combine(Application.persistentDataPath, UserData_FileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {

        }
    }
    public static void UserData_Reset()
    {
        Data_User userdata = new Data_User();

        userdata.Daily_Update_Year = TimeManager.Instance.Cur_UTC.Year;
        userdata.Daily_Update_Month = TimeManager.Instance.Cur_UTC.Month;
        userdata.Daily_Update_Day = TimeManager.Instance.Cur_UTC.Day;

        userdata.Arr_Inventory = new Inventory[500];

        Set_UserData(userdata);
    }
    /// <summary>
    /// UD 세팅되어 있으면 Action 실행, 세팅안되어있으면 세팅될때 실행
    /// </summary>
    /// <param name="Ac_UD_Set"></param>
    public static void UserData_Set_Action(Action Ac_UD_Set)
    {
        if(UserData == null)
        {
            Ev_UserData_Set += (sender, args) => { Ac_UD_Set?.Invoke(); };
        }
        else
        {
            Ac_UD_Set?.Invoke();
        }
    }
    #endregion
}