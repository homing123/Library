using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
public class GameManager : MonoBehaviour
{

    [SerializeField] public int TaskDelay;
    public static bool LoadindStart = false;
    public static Action ac_DataLoaded = () =>
    {
        Debug.Log("DataLoaded Action");
    };
    public static L_Task lt_DataLoadedAsync = new L_Task();

    public static bool isDataLoaded = false;
    public static GameManager Instance;
    public static WaitForSecondsRealtime seconds = new WaitForSecondsRealtime(0.1f);
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
        temp = AdManager.Instance;
        temp = NetworkManager.Instance;
        temp = LoginManager.Instance;
        
        //이 이후로 생성되는 유저데이터 클래스는 로딩목록에 추가되지않음 (json할때 생성해서 대입하기 때문에 그때 생성되는 애들은 로딩목록에 추가되면안됨)
        LoadindStart = true;

        yield return StartCoroutine(TimeManager.Instance.Get_CurTime());

        //로컬
        UserManager.ac_LoadLocal.Invoke();

        //스트리밍
        Debug.Log("스트리밍 부르기");

        yield return StreamingManager.Load_StreamingData();

        Debug.Log("스트리밍 부르기 완료");
        
        //로그인
        if (GameSetting.Login)
        {
            yield return StartCoroutine(LoginManager.Instance.LoginSequence());
        }

        //서버데이터 로드
        var task = UserManager.Load_Server();

        while (task.IsCompleted == false)
        {
            yield return seconds;
        }


        ac_DataLoaded?.Invoke();
        yield return lt_DataLoadedAsync.Invoke_Parallel();
        isDataLoaded = true;

        Debug.Log("완료");

        //SceneHandler.SceneLoad(SceneHandler.PlayScene);
    }

    // Update is called once per frame

    #region Test_Function
    [ContextMenu("UD 리셋")]
    public async void UD_Reset()
    {
        UserManager.Delete_Local();
        UserManager.ac_LoadLocal?.Invoke();
        await UserManager.fc_LoadServer?.Invoke();
        ac_DataLoaded?.Invoke();
        await lt_DataLoadedAsync.Invoke_Parallel();
    }
    [ContextMenu("UD 삭제")]
    public void UD_Delete()
    {
        UserManager.Delete_Local();
    }


    #endregion
}

