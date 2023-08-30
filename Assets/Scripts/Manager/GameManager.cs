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
    WaitForSecondsRealtime seconds = new WaitForSecondsRealtime(0.1f);
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        //StartCoroutine(LoadingStart());
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

        StartCoroutine(TimeManager.Instance.Get_CurTime());
        while(TimeManager.isCurTimeSet == false)
        {
            Debug.Log(TimeManager.isCurTimeSet);
            yield return seconds;
        }
        //����
        UserManager.ac_LoadLocal.Invoke();

        //��Ʈ����
        Debug.Log("��Ʈ���� �θ���");
        bool Streaming_Loaded = false;
        StreamingManager.Load_StreamingData(() => Streaming_Loaded = true);
        while (Streaming_Loaded == false)
        {
            yield return seconds;
        }
        Debug.Log("��Ʈ���� �θ��� �Ϸ�");
        //�α���

        //����������
        LoadindStart = true;
        bool Serverdata_Loaded = false;
        UserManager.Load_Server(() => Serverdata_Loaded = true);
        while(Serverdata_Loaded == false)
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

        Debug.Log("�Ϸ�");

        //SceneHandler.SceneLoad(SceneHandler.PlayScene);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    #region Test_Function
    [ContextMenu("UD ����")]
    public async void UD_Reset()
    {
        UserManager.Delete_Local();
        UserManager.ac_LoadLocal?.Invoke();
        await UserManager.fc_LoadServer?.Invoke();
        ac_DataLoaded?.Invoke();
        await fc_DataLoadedAsync?.Invoke();
    }
    [ContextMenu("UD ����")]
    public void UD_Delete()
    {
        UserManager.Delete_Local();
    }


    #endregion
}

