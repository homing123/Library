using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManager : Manager<TimeManager>
{
    //n�ʵڿ� �׼� ��� (1�ʸ��� Ȯ��)

    public const int Reset_Hour = 9;
    public const int Reset_Minute = 32;
    public const int Reset_Second = 55;
    public const DayOfWeek Reset_DOW = DayOfWeek.Wednesday;
    public const int Reset_Day = 1;


    public User_Time m_UserTime;

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

    public static bool isCurTimeSet; //����ð� ������ �Ǿ� �ִ°�  (���۽�, Ȩȭ�����γ����ٰ� ���ý� false �� �ٲ�)

    public static event EventHandler<int> ev_DailyReset;
    public static event EventHandler<int> ev_WeeklyReset;
    public static event EventHandler<int> ev_MonthReset;

    private void Awake()
    {
        UserManager.Add_Local(User_Time.LocalPath, Init_UD, () => UserManager.Save_LocalUD(User_Time.LocalPath, m_UserTime), () => m_UserTime = UserManager.Load_LocalUD<User_Time>(User_Time.LocalPath));
        GameManager.ac_DataLoaded += Access_Reset_Check;
        InputManager.ev_ApplicationPause += Ev_ApplicationPause;
    }
    public IEnumerator Get_CurTime(Action ac_success = null)
    {
        DateTime Net_UTC = DateTime.UtcNow;
        DateTime Local_UTC = DateTime.UtcNow;
        Time_Diff = (Net_UTC - Local_UTC).TotalSeconds;
        Debug.Log("�ð� ���� Net : " + Net_UTC.ToString() + " Local : " + Local_UTC + " DiffValue : " + Time_Diff);
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
    void Init_UD()
    {
        m_UserTime = new User_Time();
    }
    public void Access_Reset_Check()
    {
        DateTime cur_time = Cur_Time;
        DateTime Before_Access = m_UserTime.Get_AccessTime();

        //Before_Access = new DateTime(2024, 8, 3, 0, 0, 0);
        //cur_time = new DateTime(2024, 8, 5, 0, 0, 0);
        Debug.Log("���� : " + Before_Access.ToString() + " ����: " + cur_time.ToString());
        if (Before_Access.Year == 1)
        {
            //ù ����
            m_UserTime.Set_AccessTime(cur_time.Get_TodayResetTime());
            Debug.Log("ù����");
            //�⼮ + 1
        }
        else
        {
            int day_count = (cur_time - Before_Access).Days;
            int week_count;
            int month_count;
            int nextDOWcount = Before_Access.NextDayOfWeek(Reset_DOW);
            if (nextDOWcount > day_count)
            {
                week_count = 0;
            }
            else
            {
                week_count = (day_count - nextDOWcount) / 7 + 1;
            }
            int month_value = (cur_time.Year - Before_Access.Year) * 12 + (cur_time.Month - Before_Access.Month) - 1;

            if(Before_Access.Day < Reset_Day)
            {
                month_value++;
            }
            if(cur_time.Day >= Reset_Day)
            {
                month_value++;
            }
            month_count = month_value;

            Debug.Log("���� Ƚ�� : " + day_count + " �ְ� Ƚ�� : " + week_count + " ���� Ƚ�� : " + month_count);
            m_UserTime.Set_AccessTime(Cur_Time.Get_TodayResetTime());
            ev_DailyReset?.Invoke(this, day_count);
            ev_WeeklyReset?.Invoke(this, week_count);
            ev_MonthReset?.Invoke(this, month_count);


        }
    }

    #region Test

    [ContextMenu("�α�")]
    public void Test_Log()
    {
        Debug.Log(m_UserTime.Get_AccessTime());
    }
    #endregion
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

