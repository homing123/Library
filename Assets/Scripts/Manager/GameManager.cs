using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{

    public static bool LoadindStart = false;
    public static Action ac_DataLoaded;
    public static bool isDataLoaded = false;
    public static GameManager Instance;
    WaitForSecondsRealtime seconds = new WaitForSecondsRealtime(0.1f);
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        StartCoroutine(LoadingStart());
    }
    IEnumerator LoadingStart()
    {
        yield return seconds;
        object temp = InvenManager.Instance;
        temp = LanguageManager.Instance;
        temp = TimeManager.Instance;
        temp = SoundManager.Instance;
        temp = QuestManager.Instance;
        temp = InputManager.Instance;
        temp = StoreManager.Instance;
        temp = RandomBoxManager.Instance;

        StartCoroutine(TimeManager.Instance.Get_CurTime());
        while(TimeManager.isCurTimeSet == false)
        {
            Debug.Log(TimeManager.isCurTimeSet);
            yield return seconds;
        }
        //로컬
        UserManager.ac_LoadLocal.Invoke();

        //스트리밍
        Debug.Log("스트리밍 부르기");
        bool Streaming_Loaded = false;
        StreamingManager.Load_StreamingData(() => Streaming_Loaded = true);
        while (Streaming_Loaded == false)
        {
            yield return seconds;
        }
        Debug.Log("스트리밍 부르기 완료");
        //로그인

        //서버데이터
        LoadindStart = true;
        bool Serverdata_Loaded = false;
        UserManager.Load_Server(() => Serverdata_Loaded = true);
        while(Serverdata_Loaded == false)
        {
            yield return seconds;
        }


        ac_DataLoaded?.Invoke();
        isDataLoaded = true;

        Debug.Log("완료");

        //SceneHandler.SceneLoad(SceneHandler.PlayScene);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    #region Test_Function
    [ContextMenu("UD 리셋")]
    public async void UD_Reset()
    {
        UserManager.Delete_Local();
        UserManager.ac_LoadLocal?.Invoke();
        await UserManager.fc_LoadServer?.Invoke();
        ac_DataLoaded?.Invoke();
    }
  

    #endregion
}

