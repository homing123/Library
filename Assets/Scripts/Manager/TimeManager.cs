using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManager : Manager<TimeManager>
{
    //n초뒤에 액션 등록 (1초마다 확인)

    public const int Reset_Hour = 9;
    public const int Reset_Minute = 32;
    public const int Reset_Second = 55;
    public const DayOfWeek Reset_DOW = DayOfWeek.Wednesday;

    public User_Time m_UserTime;

    public DateTime Cur_Time;
    public static bool isCurTimeSet; //현재시간 설정이 되어 있는가  (시작시, 홈화면으로나갔다가 들어올시 false 로 바뀜)

    public static event EventHandler<int> ev_DailyReset;
    public static event EventHandler<int> ev_WeeklyReset;
    private void Awake()
    {

        UserManager.Add_Local(User_Time.LocalPath, Init_UD, () => UserManager.Save_LocalUD(User_Time.LocalPath, m_UserTime), () => m_UserTime = UserManager.Load_LocalUD<User_Time>(User_Time.LocalPath));

        GameManager.ac_DataLoaded += Access_Reset_Check;
    }
    public IEnumerator Get_CurTime(Action ac_success = null)
    {
        Cur_Time = DateTime.Now;
        isCurTimeSet = true;
        ac_success?.Invoke();
        yield break;
    }
    void Init_UD()
    {
        m_UserTime = new User_Time();
    }
    public void Access_Reset_Check()
    {
        if(isCurTimeSet == false)
        {
            throw new Exception("Cur Time Not Set");
        }

        DateTime Before_Access = m_UserTime.Get_AccessTime();
        Debug.Log("이전 : " + Before_Access.ToString() + " 현재: " + Cur_Time.ToString());
        if (Before_Access.Year == 1)
        {
            //첫 접속
            m_UserTime.Set_AccessTime(Cur_Time.Get_TodayResetTime());
            Debug.Log("첫접속");
            //출석 + 1
        }
        else
        {
            int day_count = (Cur_Time - Before_Access).Days;
            int week_count;
            int nextDOWcount = Before_Access.NextDayOfWeek(Reset_DOW);
            if (nextDOWcount > day_count)
            {
                week_count = 0;
            }
            else
            {
                week_count = (day_count - nextDOWcount) / 7 + 1;
            }
            Debug.Log("일일 횟수 : " + day_count + " 주간 횟수 : " + week_count);
            ev_DailyReset?.Invoke(this, day_count);
            ev_WeeklyReset?.Invoke(this, week_count);

        }
    }
}

public class User_Time
{
    static string m_localpath;
    public static string LocalPath
    {
        get
        {
            if (m_localpath == null)
            {
                m_localpath = Application.persistentDataPath + "/Time.txt";
            }
            return m_localpath;
        }
    }

    public int Access_Year = 1;
    public int Access_Month = 1;
    public int Access_Day = 1;
    public int Access_Hour;
    public int Access_Minute;
    public int Access_Second;

    public void Set_AccessTime(DateTime d_time)
    {
        Access_Year = d_time.Year;
        Access_Month = d_time.Month;
        Access_Day = d_time.Day;
        Access_Hour = d_time.Hour;
        Access_Minute = d_time.Minute;
        Access_Second = d_time.Second;

        SaveData();
    }
    public DateTime Get_AccessTime()
    {
        return new DateTime(Access_Year, Access_Month, Access_Day, Access_Hour, Access_Minute, Access_Second);
    }

    void SaveData()
    {
        UserManager.Save_LocalUD(LocalPath, this);
    }

}

public static class Ex_Time
{
    public static int NextDayOfWeek(this DateTime datetime, DayOfWeek DOW)
    {
        if(datetime.DayOfWeek < DOW)
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
        else if(datetime.Hour == TimeManager.Reset_Hour)
        {
            if(datetime.Minute > TimeManager.Reset_Minute)
            {
                return new DateTime(datetime.Year, datetime.Month, datetime.Day, TimeManager.Reset_Hour, TimeManager.Reset_Minute, TimeManager.Reset_Second);
            }
            else if(datetime.Minute == TimeManager.Reset_Minute)
            {
                if(datetime.Second >= TimeManager.Reset_Second)
                {
                    return new DateTime(datetime.Year, datetime.Month, datetime.Day, TimeManager.Reset_Hour, TimeManager.Reset_Minute, TimeManager.Reset_Second);
                }
            }
        }
        return new DateTime(datetime.Year, datetime.Month, datetime.Day, TimeManager.Reset_Hour, TimeManager.Reset_Minute, TimeManager.Reset_Second).AddDays(-1);

    }
}

