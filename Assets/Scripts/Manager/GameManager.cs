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
    public static Func<Task> fc_DataLoadedAsync = ()=>
    {
        return new Task(()=> Debug.Log("DataLoadedAsync Func"));
    };
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
        if (GameSetting.Login)
        {
            StartCoroutine(LoginManager.Instance.LoginSequence());

            while (LoginManager.LoginSequence_Success == false)
            {
                yield return seconds;
            }
        }

        //서버데이터 로드
        var task = UserManager.Load_Server();

        while (task.IsCompleted == false)
        {
            yield return seconds;
        }


        ac_DataLoaded?.Invoke();
        fc_DataLoadedAsync.Fc_Async(() =>
        {
            isDataLoaded = true;
        });

        while (isDataLoaded == false)
        {
            yield return seconds;
        }

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
        await fc_DataLoadedAsync?.Invoke();
    }
    [ContextMenu("UD 삭제")]
    public void UD_Delete()
    {
        UserManager.Delete_Local();
    }


    #endregion
}

