using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Networking;
using static Data_User;
using static UD_Func;
public class TimeManager : Single<TimeManager>
{
    //Trigger_Event : Daily, Weekly, Month
    //Register_Event : 

    float Cur_Value;
    public enum E_TimeRegister_Kind
    {

    }
    DateTime Cur_UTC;
    public bool Required_TimeSet = true;

    public static event EventHandler Ev_Daily_Update;
    public static event EventHandler Ev_Weekly_Update;

    const int Difference_From_UTC = 9;
    void Daily_Update()
    {
        Ev_Daily_Update?.Invoke(this, EventArgs.Empty);
    }
    void Weekly_Update()
    {
        Ev_Weekly_Update?.Invoke(this, EventArgs.Empty);
    }
    private void Awake()
    {
        Required_TimeSet = true;
        Cur_Value = 0;
        StartCoroutine(Get_UTC());
    }
    private void Update()
    {
        Cur_Value += Time.unscaledDeltaTime;
        if (Cur_Value >= 1)
        {
            Cur_Value -= 1;
            OneSecond_Update();
        }
    }
    void OneSecond_Update()
    {
        if (Required_TimeSet == false)
        {
            Check_Daily_Time();
        }
    }
    void Check_Daily_Time()
    {
        DateTime nextupdate_time = Get_Daily_Update().AddDays(1);
        if (Cur_UTC >= nextupdate_time)
        {
            UserData.Daily_Update_Year = Cur_UTC.Year;
            UserData.Daily_Update_Month = Cur_UTC.Month;
            UserData.Daily_Update_Day = Cur_UTC.Day;
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
    IEnumerator Get_UTC()
    {
        UnityWebRequest request = new UnityWebRequest();
        using (request = UnityWebRequest.Get("www.google.com"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                string date = request.GetResponseHeader("date"); //이곳에서 반송된 데이터에 시간 데이터가 존재
                Cur_UTC = DateTime.Parse(date).ToUniversalTime();
                Required_TimeSet = false;
            }
        }
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