using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Data_User;
using static UD_Func;


public class GameManager : Single<GameManager>
{

    public bool Use_Log;
    public bool Use_Log_Popup;
    public bool Use_ErrorLog_Popup;
    public bool Use_Cheat;
    public bool Use_NetworkLock;

    [Header("Advertisement")]
    public bool Use_AD;
    public E_ADSDK AD_SDK;

    [Header("Sound")]
    public bool Use_BGM_Fade;

    private void Start()
    {


        //UD 세팅
        if (UserData_Load(out UserData) == false)
        {

        }

        //StreamingData 세팅
        Data.Instance.Read_StreamingData_Async();

        //Addressable 세팅

        //로그인 확인


    }


    #region 매니저 없는 이벤트
    public static event EventHandler<E_Language_Kind> Ev_Language_Change;

    public static event EventHandler Ev_Vibration_On;
    public static event EventHandler Ev_Vibration_Off;

    public static void Language_Change(E_Language_Kind lang_kind)
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

