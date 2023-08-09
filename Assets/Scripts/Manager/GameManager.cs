using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameManager : MonoBehaviour
{

    public static Action ac_DataLoaded;
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

        StartCoroutine(TimeManager.Instance.Get_CurTime());
        while(TimeManager.isCurTimeSet == false)
        {
            Debug.Log(TimeManager.isCurTimeSet);
            yield return seconds;
        }
        //로컬
        UserManager.Get_LocalData();

        //스트리밍
        bool Streaming_Loaded = false;
        StreamingManager.Load_StreamingData(() => Streaming_Loaded = true);
        while (Streaming_Loaded == false)
        {
            yield return seconds;
        }
        //로그인


        ac_DataLoaded?.Invoke();
        Debug.Log("완료");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UserManager.Reset_Data();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Inven_Log();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Get_Item(1, 0, 500);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Get_Item(2, 0, 3);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Remove_Item(1, 0, 300);
        }
       
        if (Input.GetKeyDown(KeyCode.E))
        {
            Remove_Item(1, 1, 300);
        }
    }

    #region Test_Function
    void Inven_Log()
    {
        Debug.Log("인벤 로그 : ");
        foreach(var key in InvenManager.Instance.m_UserInven.D_Inven.Keys)
        {
            User_Inven.Inven inven = InvenManager.Instance.m_UserInven.D_Inven[key];
            Debug.Log(key+" "+inven.Kind + " " + inven.Id + " " + inven.Count);
        }
    }
    void Get_Item(int kind, int id, int count)
    {
        InvenManager.Instance.Add((kind, id, count));
    }
    void Remove_Item(int kind, int id, int count)
    {
        if(InvenManager.Instance.AvailablePurchase((kind, id, count)))
        {
            Debug.Log("제거 완료");
            InvenManager.Instance.Remove_bykind((kind, id, count));
        }
        else
        {
            Debug.Log("제거 못함");
        }
    }

    #endregion
}

