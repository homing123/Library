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
        
        //�� ���ķ� �����Ǵ� ���������� Ŭ������ �ε���Ͽ� �߰��������� (json�Ҷ� �����ؼ� �����ϱ� ������ �׶� �����Ǵ� �ֵ��� �ε���Ͽ� �߰��Ǹ�ȵ�)
        LoadindStart = true;

        yield return StartCoroutine(TimeManager.Instance.Get_CurTime());

        //����
        UserManager.ac_LoadLocal.Invoke();

        //��Ʈ����
        var task = StreamingManager.Load_StreamingData();
        yield return new WaitUntil(() => task.IsCompleted);
        Debug.Log("��Ʈ ����");
        
        //�α���
        if (GameSetting.Login)
        {
            yield return StartCoroutine(LoginManager.Instance.LoginSequence());
        }

        //���������� �ε�
        task = UserManager.Load_Server();
        yield return new WaitUntil(() => task.IsCompleted);

        ac_DataLoaded?.Invoke();
        yield return lt_DataLoadedAsync.Invoke_Parallel();
        isDataLoaded = true;

        Debug.Log("�Ϸ�");

        //SceneHandler.SceneLoad(SceneHandler.PlayScene);
    }

    // Update is called once per frame

    #region Test_Function
    [ContextMenu("UD ����")]
    public async void UD_Reset()
    {
        UserManager.Delete_Local();
        UserManager.ac_LoadLocal?.Invoke();
        await UserManager.fc_LoadServer?.Invoke();
        ac_DataLoaded?.Invoke();
        await lt_DataLoadedAsync.Invoke_Parallel();
    }
    [ContextMenu("UD ����")]
    public void UD_Delete()
    {
        UserManager.Delete_Local();
    }


    #endregion
}

