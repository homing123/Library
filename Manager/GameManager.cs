using System.Collections;
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

    public static event EventHandler Ev_GameLoading;
    public static event EventHandler Ev_GameStart;

    WaitForEndOfFrame Wait_Frame;
    WaitForSeconds Wait_OneSecond;
    private void Start()
    {
        Wait_Frame = new WaitForEndOfFrame();
        Wait_OneSecond = new WaitForSeconds(1);
        StartCoroutine(GameLoading());
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // 앱이 일시정지될 때 수행할 작업
            if (Use_Network_Time)
            {
                TimeManager.Instance.isNetwork_Time = false;
            }
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UI_Escape.Escape();
        }
    }
    IEnumerator GameLoading()
    {


        //Appear UI_Loading
        UI_Loading.Create();

        //Set Current Time
        if (Use_Network_Time) 
        {
            TimeManager.Instance.Get_NetworkTime_Start();
            while (TimeManager.Instance.isNetwork_Time == false)
            {
                yield return Wait_Frame;
            }
        }

        UI_Loading.Instance.Change_State(10, "Time");

        //Set UserData
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

        UI_Loading.Instance.Change_State(20, "UserData");

        //Set StreamingData
        bool isStreamLoad_Success = false;
        Data.Instance.Read_StreamingData_Async(() => isStreamLoad_Success = true);
        while (isStreamLoad_Success == false)
        {
            float Cur_StreamLoad_Per = Data.Instance.Get_StreamingDataRead_Percent();
            yield return Wait_Frame;
        }
        //Addressable 세팅

        UI_Loading.Instance.Change_State(70, "StreamingData");


        //Check Login
        switch (UserData.LoginState)
        {
            case E_LoginState.None:
                //UI_Loading Destroy

                //Appear UI_Login

                break;
            case E_LoginState.Guest:
                //Guest Login
                //UI_Loading Destroy
                break;
            case E_LoginState.Google:
                //Google Login
                //UI_Loading Destroy
                break;
            case E_LoginState.Apple:
                //Apple Login
                //UI_Loading Destroy
                break;
        }

        UI_Loading.Instance.Change_State(100, "Login");

        yield return Wait_OneSecond;

        UI_Loading.Destroy();
        UI_Lobby.Create();
        //Appear UI_Lobby
    }


    //IEnumerator GameStart()
    //{
    //    //Appear UI_Loading_Play

    //    while()

    //    //Disappear UI_Loading_Play

    //    //Appear UI_Play
    //}
    //IEnumerator GameCleanUp()
    //{

    //}
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

