using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Data_User;
using static UD_Func;


public class GameManager : SingleMono<GameManager>
{

    public bool Use_Log;
    public bool Use_Log_Popup;
    public bool Use_ErrorLog_Popup;
    public bool Use_Cheat;
    public bool Use_Network_Lock;
    public bool Use_Network_Time;

    [Header("Advertisement")]
    public bool Use_AD;
    public E_ADSDK AD_SDK;

    [Header("Sound")]
    public bool Use_BGM_Fade;


    public static bool isPaused = false;

    private void Start()
    {
        StartCoroutine(GameStart());
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // 앱이 일시정지될 때 수행할 작업
            TimeManager.Instance.isNetwork_Time = false;
            isPaused = true;
        }
        else
        {
            // 앱이 다시 시작될 때 수행할 작업
            //일시정지 창 띄우기
            isPaused = false;
            if (Use_Network_Time)
            {
                TimeManager.Instance.Get_NetworkTime_Pause();
            }
        }
    }
    IEnumerator GameStart()
    {
        //시간 가져오고
        WaitForEndOfFrame wait_frame = new WaitForEndOfFrame();
        if (Use_Network_Time) 
        {
            TimeManager.Instance.Get_NetworkTime_Start();
            while (TimeManager.Instance.isNetwork_Time == false)
            {
                yield return wait_frame;
            }
        }

        //UD 세팅
        Data_User userdata = UserData_Load();
        if (userdata == null)
        {
            UserData_Reset();
            UserData_Save();
        }
        else
        {
            Set_UserData(userdata);
        }

        //StreamingData 세팅
        bool isStreamLoad_Success = false;
        Data.Instance.Read_StreamingData_Async(() => isStreamLoad_Success = true);
        while (isStreamLoad_Success == false)
        {
            float Cur_StreamLoad_Per = Data.Instance.Get_StreamingDataRead_Percent();
            yield return wait_frame;
        }
        //Addressable 세팅

        //로그인 확인
    }

    #region 매니저 없는 이벤트
    public static event EventHandler<E_Language> Ev_Language_Change;

    public static event EventHandler Ev_Vibration_On;
    public static event EventHandler Ev_Vibration_Off;

    public static void Language_Change(E_Language lang_kind)
    {
        if (UserData.Language_Kind != lang_kind)
        {
            UserData.Language_Kind = lang_kind;
            Ev_Language_Change?.Invoke(null, lang_kind);
            Data.Instance.UserData_Changed();
        }
    }
  

    public static void Vibration_Change(bool sound_on)
    {
       
    }

    #endregion
}

