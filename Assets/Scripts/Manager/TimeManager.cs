using System.Collections;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.IO;

public class TimeManager : Manager<TimeManager>
{
    //n초뒤에 액션 등록 (1초마다 확인)

    public const int Reset_Hour = 9;
    public const int Reset_Minute = 32;
    public const int Reset_Second = 55;
    public const DayOfWeek Reset_DOW = DayOfWeek.Wednesday;
    public const int Reset_Day = 1;

    double Time_Diff; // NetUTC - LocalUTC

    public DateTime Cur_Time
    {
        get
        {
            if (isCurTimeSet == false)
            {
                throw new Exception("Cur Time is Not Set");
            }
            else
            {
                return DateTime.UtcNow.AddSeconds(Time_Diff);
            }
        }
    }

    public static bool isCurTimeSet; //현재시간 설정이 되어 있는가  (시작시, 홈화면으로나갔다가 들어올시 false 로 바뀜)

    public static event EventHandler<int> ev_DailyReset;
    public static event EventHandler<int> ev_WeeklyReset;
    public static event EventHandler<int> ev_MonthReset;

    private void Awake()
    {
        User_Time.m_UserTime = new User_Time();
        GameManager.fc_DataLoadedAsync += Access_Reset_Check;
        InputManager.ev_ApplicationPause += Ev_ApplicationPause;
    }
    float update_time;
    private void Update()
    {
        update_time += Time.unscaledDeltaTime;
        if (update_time >= 1)
        {
            if (isCurTimeSet && GameManager.isDataLoaded)
            {
                update_time = 0;
                if (Cur_Time.CompareTo(User_Time.m_UserTime.Get_AccessTime().AddDays(1)) > 0)
                {
                    Access_Reset_Check();
                }
            }
        }
    }
    public IEnumerator Get_CurTime(Action ac_success = null)
    {
        DateTime Net_UTC = DateTime.UtcNow;
        DateTime Local_UTC = DateTime.UtcNow;
        Time_Diff = (Net_UTC - Local_UTC).TotalSeconds;
        Debug.Log("시간 세팅 Net : " + Net_UTC.ToString() + " Local : " + Local_UTC + " DiffValue : " + Time_Diff);
        isCurTimeSet = true;
        ac_success?.Invoke();
        yield break;
    }
    void Ev_ApplicationPause(object sender, bool pause)
    {
        if (pause == false)
        {
            isCurTimeSet = false;
            StartCoroutine(Get_CurTime());
        }
    }
    public async Task Access_Reset_Check()
    {
        Debug.Log("엑세스 리셋 체크");
        var data = await User_Time.m_UserTime.Access_Reset(Cur_Time);
        if(data.day_count == -1)
        {
            Debug.Log("첫접속");
        }
        else
        {
            if (data.day_count != 0)
            {
                ev_DailyReset?.Invoke(this, data.day_count);
            }
            if(data.week_count != 0)
            {
                ev_WeeklyReset?.Invoke(this, data.week_count);
            }
            if (data.month_count != 0)
            {
                ev_MonthReset?.Invoke(this, data.month_count);
            }
        }
    }

    #region Test
    [SerializeField] double Test_TimeDiff;
    [ContextMenu("시간차이 설정")]
    public void Test_DiffSet()
    {
        Time_Diff = Test_TimeDiff;
    }
    [ContextMenu("로그")]
    public void Test_Log()
    {
        Debug.Log(User_Time.m_UserTime.Get_AccessTime());
    }
    #endregion
}

public class User_Time : UserData_Server
{
    public const string Path = "Time";
    public static User_Time m_UserTime;

    public string Access_Time;

    public override async Task Load()
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        if (UserManager.Use_Local)
        {
            if (UserManager.Exist_LocalUD(Path))
            {
                var data = await UserManager.Load_LocalUDAsync<User_Time>(Path);
                Access_Time = data.Access_Time;
            }
            else
            {
                Debug.Log("Time_Init");

                UserManager.Save_LocalUD(Path, this);
            }
        }
        else
        {
            //서버에서 있는지없는지 확인 후 없으면 생성해서 보내면 그거 받으면됨
        }
    }

    public async Task<(int month_count, int week_count, int day_count)> Access_Reset(DateTime Cur_Time)
    {
        if (UserManager.Use_Local)
        {
            var data = await LocalUser_Time.Access_Reset(Cur_Time);
            this.Set_AccessTime(data.access_time);
            return (data.month_count, data.week_count, data.day_count);
        }
        else
        {
            //server
        }
    }
}
public class LocalUser_Time
{
    public static async Task<(DateTime access_time, int month_count, int week_count, int day_count)> Access_Reset(DateTime Cur_Time)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Time m_usertime = UserManager.Load_LocalUD<User_Time>(User_Time.Path);

        DateTime cur_time = Cur_Time;
        DateTime Before_Access = m_usertime.Get_AccessTime();

        //Before_Access = new DateTime(2024, 8, 3, 0, 0, 0);
        //cur_time = new DateTime(2024, 8, 5, 0, 0, 0);

        Debug.Log("이전 : " + Before_Access.ToString() + " 현재: " + cur_time.ToString());
        int day_count = 0;
        int week_count = 0;
        int month_count = 0;
        if (Before_Access.Year == 1)
        {
            //첫 접속
            m_usertime.Set_AccessTime(Cur_Time.Get_TodayResetTime());
            Debug.Log("첫접속");
            Debug.Log(m_usertime.Access_Time);
            day_count = -1;
        }
        else
        {
            day_count = (cur_time - Before_Access).Days;
            int nextDOWcount = Before_Access.NextDayOfWeek(TimeManager.Reset_DOW);
            if (nextDOWcount > day_count)
            {
                week_count = 0;
            }
            else
            {
                week_count = (day_count - nextDOWcount) / 7 + 1;
            }
            int month_value = (cur_time.Year - Before_Access.Year) * 12 + (cur_time.Month - Before_Access.Month) - 1;

            if (Before_Access.Day < TimeManager.Reset_Day)
            {
                month_value++;
            }
            if (cur_time.Day >= TimeManager.Reset_Day)
            {
                month_value++;
            }
            month_count = month_value;

            Debug.Log("일일 횟수 : " + day_count + " 주간 횟수 : " + week_count + " 월간 횟수 : " + month_count);
            m_usertime.Set_AccessTime(Cur_Time.Get_TodayResetTime());
        }
        UserManager.Save_LocalUD(User_Time.Path, m_usertime);
        return (m_usertime.Get_AccessTime(), month_count, week_count, day_count);
    }
}

public static class Ex_Time
{
    public static int NextDayOfWeek(this DateTime datetime, DayOfWeek DOW)
    {
        if (datetime.DayOfWeek < DOW)
        {
            return DOW - datetime.DayOfWeek;
        }
        else
        {
            return 7 - (datetime.DayOfWeek - DOW);
        }
    }
    public static DateTime Get_TodayResetTime(this DateTime datetime)
    {
        if (datetime.Hour > TimeManager.Reset_Hour)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, TimeManager.Reset_Hour, TimeManager.Reset_Minute, TimeManager.Reset_Second);
        }
        else if (datetime.Hour == TimeManager.Reset_Hour)
        {
            if (datetime.Minute > TimeManager.Reset_Minute)
            {
                return new DateTime(datetime.Year, datetime.Month, datetime.Day, TimeManager.Reset_Hour, TimeManager.Reset_Minute, TimeManager.Reset_Second);
            }
            else if (datetime.Minute == TimeManager.Reset_Minute)
            {
                if (datetime.Second >= TimeManager.Reset_Second)
                {
                    return new DateTime(datetime.Year, datetime.Month, datetime.Day, TimeManager.Reset_Hour, TimeManager.Reset_Minute, TimeManager.Reset_Second);
                }
            }
        }
        return new DateTime(datetime.Year, datetime.Month, datetime.Day, TimeManager.Reset_Hour, TimeManager.Reset_Minute, TimeManager.Reset_Second).AddDays(-1);

    }
    public static DateTime Get_AccessTime(this User_Time m_usertime)
    {
        if (string.IsNullOrEmpty(m_usertime.Access_Time))
        {
            return new DateTime(1, 1, 1, 0, 0, 0);
        }
        return DateTime.Parse(m_usertime.Access_Time);
    }
    public static void Set_AccessTime(this User_Time m_usertime, DateTime datetime)
    {
        m_usertime.Access_Time = datetime.ToString();
    }

}

