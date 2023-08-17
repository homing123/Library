//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//public enum E_AttendanceState
//{
//    Lock,
//    Open,
//    Receive
//}
//public class AttendanceManager : Manager<AttendanceManager>
//{
//    User_Attendance m_UserAttendance;
//    private void Awake()
//    {
//        UserManager.Add_Local(User_Attendance.LocalPath, Init_UD, () => UserManager.Save_LocalUD(User_Attendance.LocalPath, m_UserAttendance), () => m_UserAttendance = UserManager.Load_LocalUD<User_Attendance>(User_Attendance.LocalPath));
//        StreamingManager.Read_Data<J_AttendanceData>(StreamingManager.Get_StreamingPath(J_AttendanceData.Path), AttendanceData.Data_DicSet);
//        TimeManager.ev_DailyReset += Attendance;
//    }
//    void Init_UD()
//    {
//        m_UserAttendance = new User_Attendance()
//        {
//            Attendance_Count = 1,
//            D_Attendance_State = new Dictionary<int, E_AttendanceState>()
//            {
//                {0, E_AttendanceState.Open }
//            }
//        };
//    }

//    public void Attendance(object sender = null, int day_count = 0)
//    {
//        m_UserAttendance.Attendance();
//    }
//    public void Receive(int group, int day)
//    {
//        m_UserAttendance.Receive(new (int, int)[] { (group, day) });
//    }
//    public void Receive(int group, int[] days)
//    {
//        var receive_info = new (int, int)[days.Length];
//        for(int i = 0; i < receive_info.Length; i++)
//        {
//            receive_info[i] = (group, days[i]);
//        }
//        m_UserAttendance.Receive(receive_info);
//    }

//    public E_AttendanceState Get_State(int day)
//    {
//        return m_UserAttendance.Get_State(day);
//    }
//}
//public class User_Attendance
//{
//    static string m_localpath;

//    public static string LocalPath
//    {
//        get
//        {
//            if (m_localpath == null)
//            {
//                m_localpath = Application.persistentDataPath + "/Attendance.txt";
//            }
//            return m_localpath;
//        }
//    }
//    public int Attendance_Count;
//    public Dictionary<int, E_AttendanceState> D_Attendance_State = new Dictionary<int, E_AttendanceState>();

//    public void Attendance()
//    {
//        Attendance_Count++;
//    }
//    public void Receive((int group, int day)[] receive_info)
//    {
//        for(int i = 0; i < receive_info.Length; i++)
//        {
//            AttendanceData cur_att = AttendanceData.Get(receive_info[i]);
//            InvenManager.Instance.Add(cur_att.Kind, cur_att.ID, cur_att.Count);
//            if (D_Attendance_State.ContainsKey(receive_info[i].day))
//            {
//                D_Attendance_State[receive_info[i].day] = E_AttendanceState.Receive;
//            }
//            else
//            {
//                D_Attendance_State.Add(receive_info[i].day, E_AttendanceState.Receive);
//            }
//        }
//    }
//    public E_AttendanceState Get_State(int day)
//    {
//        if (D_Attendance_State.ContainsKey(day) == true)
//        {
//            return D_Attendance_State[day]
//        }
//        else
//        {
//            return E_AttendanceState.Lock;
//        }
//    }
//    public void SaveData()
//    {

//    }
//}
//public class J_AttendanceData
//{
//    public const string Path = "Attendance";

//    public int[] Group;
//    public int[] Day;
//    public int[] Kind_0;
//    public int[] ID_0;
//    public int[] Count_0;
//    public int[] Kind_1;
//    public int[] ID_1;
//    public int[] Count_1;
//    public int[] Kind_2;
//    public int[] ID_2;
//    public int[] Count_2;
//}
//public class AttendanceData
//{
//    static Dictionary<(int group, int day), AttendanceData> D_Data = new Dictionary<(int group, int day), AttendanceData>();
    
//    public int Group;
//    public int Day;
//    public int[] Kind;
//    public int[] ID;
//    public int[] Count;

//    public static void Data_DicSet(J_AttendanceData j_obj)
//    {
//        for (int i = 0; i < j_obj.Group.Length; i++)
//        {

//        }

//        for (int i = 0; i < j_obj.Group.Length; i++)
//        {
//            AttendanceData obj = new AttendanceData()
//            {
//                Group = j_obj.Group[i],
//                Day = j_obj.Day[i],
//                Kind = new int[] { j_obj.Kind_0[i], j_obj.Kind_1[i], j_obj.Kind_2[i] },
//                ID = new int[] { j_obj.ID_0[i], j_obj.ID_1[i], j_obj.ID_2[i] },
//                Count = new int[] { j_obj.Count_0[i], j_obj.Count_1[i], j_obj.Count_2[i] },
//            };
//            D_Data.Add((obj.Groupm, obj.Day), obj);
//        }
//    }

//    public static AttendanceData Get((int group, int day) key)
//    {
//        if (D_Data.ContainsKey(key))
//        {
//            return D_Data[key];
//        }
//        else
//        {
//            if (D_Data.Count == 0)
//            {
//                throw new Exception("AttendanceData count is 0");
//            }
//            else
//            {
//                throw new Exception("AttendanceData key is null : " + key.group + " " + key.day);
//            }
//        }
//    }
   
//}