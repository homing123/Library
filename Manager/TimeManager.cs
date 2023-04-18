using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using static Data_User;
using static UD_Func;
public class TimeManager : SingleMono<TimeManager>
{


    public Dictionary<(E_ReservationTime_Kind kind, int idx), ReservationTime_Info> D_ReservationTime_Info;

    float Cur_Value;
    public DateTime Cur_UTC;
    public bool isNetwork_Time;
    Coroutine Cor_Get_NetworkTime;

    public static event EventHandler Ev_Daily_Update;
    public static event EventHandler Ev_Weekly_Update;

    const int Difference_From_UTC = 9;

    

    public TimeManager()
    {
        isNetwork_Time = false;
        Cur_Value = 0;

        UserData_Set_Action(Set_D_ReservationTime);
    }

    void Set_D_ReservationTime()
    {
        D_ReservationTime_Info = new Dictionary<(E_ReservationTime_Kind kind, int idx), ReservationTime_Info>();
        for (int i = 0; i < UserData.L_Reservation_Time.Count; i++)
        {
            User_Reservation_Time cur_user_reservation = UserData.L_Reservation_Time[i];
            ReservationTime_Info info = new ReservationTime_Info(cur_user_reservation);
            D_ReservationTime_Info.Add((cur_user_reservation.Key, cur_user_reservation.Idx), info);
        }
    }
    void Update_D_ReservationTime()
    {
        if (D_ReservationTime_Info == null)
        {
            foreach (KeyValuePair<(E_ReservationTime_Kind kind, int idx), ReservationTime_Info> keys in D_ReservationTime_Info)
            {
                ReservationTime_Info info = D_ReservationTime_Info[(keys.Key.kind, keys.Key.idx)];
                info.Remaining_Time = Get_RemainingTime(info.Reservation_Time);
                info.Cur_State = info.Remaining_Time > 0 ? E_ReservationTime_State.Before : E_ReservationTime_State.After;
            }
        }
    }
  
    private void Update()
    {
        if (UserData == null)
        {
            return;
        }

        Cur_Value += Time.unscaledDeltaTime;
        if (Cur_Value >= 1)
        {
            Cur_Value -= 1;
            OneSecond_Update();
        }
    }
    void OneSecond_Update()
    {
        if (GameManager.Instance.Use_Network_Time)
        {
            if (isNetwork_Time == true)
            {
                Check_Daily_Time();
                Update_D_ReservationTime();
            }
        }
        else 
        { 
            Check_Daily_Time();
            Update_D_ReservationTime();
        }
    }



    void Check_Daily_Time()
    {
        DateTime nextupdate_time = Get_Daily_Update().AddDays(1);
        if (Cur_UTC >= nextupdate_time)
        {
            Daily_Update();
            Check_Weekly_Time();
        }
    }
    void Check_Weekly_Time()
    {
        if (Get_Daily_Update().DayOfWeek == DayOfWeek.Wednesday)
        {
            Weekly_Update();
        }
    }
    void Daily_Update()
    {
        UserData.Daily_Update_Year = Cur_UTC.Year;
        UserData.Daily_Update_Month = Cur_UTC.Month;
        UserData.Daily_Update_Day = Cur_UTC.Day;

        Ev_Daily_Update?.Invoke(this, EventArgs.Empty);
    }
    void Weekly_Update()
    {
        Ev_Weekly_Update?.Invoke(this, EventArgs.Empty);
    }
    IEnumerator Get_UTC(Action Ac_GetTime_Success = null, Action Ac_GetTime_Fail = null)
    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get("www.google.com"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                LogManager.Log("Get_UTC Error : " + request.error);
                Ac_GetTime_Fail?.Invoke();
            }
            else
            {
                string date = request.GetResponseHeader("date"); //이곳에서 반송된 데이터에 시간 데이터가 존재
                Cur_UTC = DateTime.Parse(date).ToUniversalTime();
                isNetwork_Time = true;
                Ac_GetTime_Success?.Invoke();
            }
        }
    }

    public void Get_NetworkTime_Start()
    {
        if (GameManager.Instance.Use_Network_Time == false)
        {
            return;
        }
        else
        {
            if (Cor_Get_NetworkTime == null)
            {
                Cor_Get_NetworkTime = StartCoroutine(Get_UTC());
            }
        }
    }
    public void Get_NetworkTime_Pause()
    {
        if (GameManager.Instance.Use_Network_Time == false)
        {
            return;
        }
        else
        {
            if (Cor_Get_NetworkTime == null)
            {
                Cor_Get_NetworkTime = StartCoroutine(Get_UTC());
            }
        }
    }

    public int Get_RemainingTime(DateTime time)
    {
        return (int)(time - Cur_UTC).TotalSeconds;
    }
   
}

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Net;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Networking;

//public class TimeManager : MonoBehaviour
//{
//    public DateTime currentUtcTime;

//    /// <summary>
//    /// 인터넷 시간 가져오기
//    /// </summary>
//    public void Init(System.Action action)
//    {
//        CurretnTimeSetting(() =>
//        {
//            StartCoroutine(UpdateTime());
//            UserDataAttdentansSetting();
//            action?.Invoke();
//        });
//    }

//    /// <summary>
//    /// 현재 시간 셋팅
//    /// </summary>
//    /// <param name="action"></param>
//    public void CurretnTimeSetting(System.Action action)
//    {
//        StartCoroutine(CrruentNetTime(currenttime =>
//        {
//            currentUtcTime = currenttime;
//            action?.Invoke();
//        }));
//    }

//    /// <summary>
//    /// 현재 시간 가져오기
//    /// </summary>
//    /// <param name="currenttime"></param>
//    /// <returns></returns>
//    IEnumerator CrruentNetTime(System.Action<DateTime> currenttime)
//    {
//#if UNITY_WEBGL
//        currenttime(DateTime.UtcNow);
//        yield break;
//#else
//        UnityWebRequest request = new UnityWebRequest();
//        using (request = UnityWebRequest.Get("www.google.com"))
//        {
//            yield return request.SendWebRequest();

//            if (request.isNetworkError)
//            {
//                Debug.Log(request.error);
//            }
//            else
//            {
//                string date = request.GetResponseHeader("date"); //이곳에서 반송된 데이터에 시간 데이터가 존재
//                DateTime dateTime = DateTime.Parse(date).ToUniversalTime();
//                currenttime(dateTime);
//            }
//        }
//#endif
//    }

//    private IEnumerator UpdateTime()
//    {
//        int count = 0;
//        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1f);
//        while (true)
//        {
//            // 시간을 1초씩 증가시킵니다.
//            currentUtcTime = currentUtcTime.AddSeconds(1f);
//            count++;
//            if (count == 10)
//            {
//                CurretnTimeSetting(null);
//                count = 0;
//            }
//            yield return wait;
//        }
//    }

//    /// <summary>
//    /// 유저 출석 셋팅 및 하루 최초 접속 시간 갱신
//    /// </summary>
//    void UserDataAttdentansSetting()
//    {
//        //첫 접속인지 파악 후 셋팅
//        if (string.IsNullOrEmpty(Gamemanager.M_UserData.TodayFirstConnetTime))
//        {
//            //첫 접속이면 오늘 시간부터 먼저 셋팅
//            TodaySetting();
//        }
//        //접속을 했던 유저 셋팅
//        else
//        {
//            System.DateTime userdatetime = System.DateTime.Parse(Gamemanager.M_UserData.TodayFirstConnetTime);
//            System.DateTime nexttime = userdatetime.AddDays(1).Date;
//            //오늘 최초 접속 시간을 가지고 다음 날인지 체크
//            if (nexttime <= currentUtcTime.Date)
//            {
//                //이미 접속을 해왔던 유저이면 주간 시간부터 먼저 셋팅 후 하루 시간 셋팅
//                WeeklySetting();
//                TodaySetting();
//            }
//        }
//    }

//    /// <summary>
//    /// 일일 접속 셋팅
//    /// </summary>
//    void TodaySetting()
//    {
//        //출석 카운트 증가
//        Gamemanager.M_UserData.AttendansData.AttendansCountSetting(1);
//        //오늘 최초 접속 시간
//        Gamemanager.M_UserData.TodayFirstConnetTime = currentUtcTime.ToString();
//        //시간 퀘스트 초기화
//        TimeQuestReset(1);
//        TimeQuestReset(4);
//    }

//    /// <summary>
//    /// 주간 접속 셋팅
//    /// </summary>
//    void WeeklySetting()
//    {
//        //특정 요일
//        DayOfWeek targetDayOfWeek = DayOfWeek.Wednesday; // 예시로 수요일을 대상으로 설정
//        DateTime firstconnettime = DateTime.Parse(Gamemanager.M_UserData.TodayFirstConnetTime);//유저 첫 접속
//        DateTime CriteriaTime = firstconnettime.AddDays(((int)targetDayOfWeek - (int)firstconnettime.DayOfWeek + 7) % 7).Date; // 첫 접속 시간 주를 기준으로 대상 요일의 날짜 가져오기
//        TimeSpan endtime = CriteriaTime - currentUtcTime;//현재 시간과 대상 날짜 시간 확인 하기
//        if (endtime.TotalMilliseconds < 0)//날짜가 지났는지 확인 후 셋팅하기
//        {
//            TimeQuestReset(2);
//            TimeQuestReset(5);
//        }
//    }


//    /// <summary>
//    /// 시간 퀘스트들 초기화 함수
//    /// </summary>
//    /// <param name="questkind"></param>
//    void TimeQuestReset(int questkind)
//    {
//        int maxcount = GameData.Data.D_QuestData[questkind].Count;

//        for (int i = 0; i < maxcount; i++)
//        {
//            //해당 퀘스트의 인덱스를 가져와 제거
//            int index = Gamemanager.M_UserData.L_ClearQuestList.FindIndex(x => x == GameData.Data.D_QuestData[questkind][i].Mid);
//            if (index != -1)
//            {
//                int questitemindex = Gamemanager.M_UserData.FindItemIndex(GameData.Data.D_QuestData[questkind][i].Clear_Kind);
//                Gamemanager.M_UserData.L_Item[questitemindex].Value = 0;
//                Gamemanager.M_UserData.L_ClearQuestList.RemoveAt(index);
//            }
//        }
//    }
//}