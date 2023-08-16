using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum E_QuestState
{
    None = 0, //없는
    Complete = 1, //완료된
    Acceptable = 2, //수락가능한
    UnAcceptable = 3, //수락불가능한
    Completable = 4,  //완료가능한
    UnCompletable = 5 //완료불가능한
}
public class QuestManager : Manager<QuestManager>
{
    User_Quest m_UserQuest;

    public static event EventHandler ev_QuestChanged;
    private void Awake()
    {
        UserManager.Add_Local(User_Quest.LocalPath, Init_UD, () => UserManager.Save_LocalUD(User_Quest.LocalPath, m_UserQuest), () => m_UserQuest = UserManager.Load_LocalUD<User_Quest>(User_Quest.LocalPath));

        StreamingManager.Read_Data<J_QuestData>(StreamingManager.Get_StreamingPath(J_QuestData.Path), QuestData.Data_DicSet);
    }
    private void LateUpdate()
    {
        if (User_Quest.isChanged)
        {
            m_UserQuest.SaveData();
            Debug.Log("퀘스트 변경 이벤트 실행");
            ev_QuestChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    void Init_UD()
    {
        m_UserQuest = new User_Quest();
    }
    
    public E_QuestState Get_State(int id)
    {
        return m_UserQuest.Get_State(id);
    }
    public void Accept(int id)
    {
        m_UserQuest.Accept(id);
    }
    #region Cancel override
    public void Cancel(int id)
    {
        m_UserQuest.Cancel(new int[] { id });
    }
    public void Cancel(int[] id)
    {
        m_UserQuest.Cancel(id);
    }
    public void Cancel(List<int> id)   
    { 
        m_UserQuest.Cancel(id.ToArray());
    }
    #endregion
    public void Complete(int id)
    {
        m_UserQuest.Complete(id);
    }

    #region Test
    [SerializeField] int Test_QuestID;
    [ContextMenu("퀘스트 수락")]
    public void Test_Accept()
    {
        Accept(Test_QuestID);
    }
    [ContextMenu("퀘스트 완료")]
    public void Test_Complete()
    {
        Complete(Test_QuestID);
    }
    [ContextMenu("퀘스트 취소")]
    public void Test_Cancel()
    {
        Cancel(Test_QuestID);
    }
    [ContextMenu("퀘스트 상태 확인")]
    public void Test_StateLog()
    {
        Debug.Log(Test_QuestID + " " + Get_State(Test_QuestID));
    }
    [ContextMenu("유저 퀘스트 로그")]
    public void Test_UserLog()
    {
        foreach(int key in m_UserQuest.D_Quest.Keys)
        {
            Debug.Log("key : " + key + " state : " + m_UserQuest.D_Quest[key].State);
        }
    }
    #endregion
}
public class User_Quest
{
    static string m_localpath;
    public static bool isChanged;

    public static string LocalPath
    {
        get
        {
            if (m_localpath == null)
            {
                m_localpath = Application.persistentDataPath + "/Quest.txt";
            }
            return m_localpath;
        }
    }
    public Dictionary<int, Quest> D_Quest = new Dictionary<int, Quest>();
    public class Quest 
    {
        public E_QuestState State;
    }
    E_QuestState UserQuest_State(int id)
    {
        if (D_Quest.ContainsKey(id))
        {
            return D_Quest[id].State;
        }
        else
        {
            return E_QuestState.None;
        }
    }
    public E_QuestState Get_State(int id)
    {
        QuestData cur_quest = QuestData.Get(id);
        if (cur_quest == null)
        {
            return E_QuestState.None;
        }
        E_QuestState state = UserQuest_State(id);


        switch (state)
        {
            case E_QuestState.None:
                //수락 가능한지 확인 후 리턴
                bool Acceptable_Item = false;
                if (cur_quest.Accept_ItemKind == 0 && cur_quest.Accept_ItemID == 0)
                {
                    Acceptable_Item = true;
                }
                else
                {
                    Acceptable_Item = InvenManager.Instance.AvailablePurchase((cur_quest.Accept_ItemKind, cur_quest.Accept_ItemID, cur_quest.Accept_ItemCount));
                }

                bool Acceptable_Quest = false;
                if (cur_quest.Accept_QuestID == 0 || UserQuest_State(cur_quest.Accept_QuestID) == E_QuestState.Complete)
                {
                    Acceptable_Quest = true;
                }

                if (Acceptable_Item && Acceptable_Quest)
                {
                    return E_QuestState.Acceptable;
                }
                else
                {
                    return E_QuestState.UnAcceptable;
                }
            case E_QuestState.Complete:
                return E_QuestState.Complete;
            case E_QuestState.Completable:
            case E_QuestState.UnCompletable:
                //완료 가능한지 확인 후 리턴
                if (InvenManager.Instance.AvailablePurchase((cur_quest.Mission_Kind, cur_quest.Mission_ID, cur_quest.Mission_Count)))
                {
                    return E_QuestState.Completable;
                }
                else
                {
                    return E_QuestState.UnCompletable;
                }
        }

        throw new Exception("QuestState case is empty : " + state);
    }

    public void Accept(int id)
    {
        E_QuestState state = Get_State(id);
        if (state == E_QuestState.Acceptable) 
        {
            if (D_Quest.ContainsKey(id))
            {
                D_Quest[id].State = E_QuestState.UnCompletable;
            }
            else
            {
                D_Quest.Add(id, new Quest() { State = E_QuestState.UnCompletable });

            }
        }
        else
        {
            throw new Exception("State is not Acceptable : id " + id + " ,state " + state);
        }
        isChanged = true;
    }
    public void Cancel(int[] id)
    {
        for (int i = 0; i < id.Length; i++)
        {
            D_Quest.Remove(id[i]);
        }
        isChanged = true;
    }
    public void Complete(int id)
    {
        E_QuestState state = Get_State(id);
        if (state == E_QuestState.Completable)
        {
            if (D_Quest.ContainsKey(id))
            {
                D_Quest[id].State = E_QuestState.Complete;

                QuestData cur_quest = QuestData.Get(id);
                InvenManager.Instance.Add(cur_quest.Reward_Kind, cur_quest.Reward_ID, cur_quest.Reward_Count);
            }
            else
            {
                D_Quest.Add(id, new Quest() { State = E_QuestState.Complete });

            }
        }
        else
        {
            throw new Exception("State is not Completable : id " + id + " ,state " + state);
        }
        isChanged = true;
    }
    public void SaveData()
    {
        isChanged = false;
        UserManager.Save_LocalUD(LocalPath, this);
    }

}
public class J_QuestData
{
    public const string Path = "Quest";
    public int[] ID;
    public int[] Type;
    public int[] Title;
    public int[] Description;
    public int[] Accept_ItemKind;
    public int[] Accept_ItemID;
    public int[] Accept_ItemCount;
    public int[] Accept_QuestID;
    public int[] Mission_Kind; 
    public int[] Mission_ID;
    public int[] Mission_Count;
    public int[] Reward_Kind_0; 
    public int[] Reward_ID_0;
    public int[] Reward_Count_0;
}
public class QuestData
{
    static Dictionary<int, QuestData> D_Data = new Dictionary<int, QuestData>();

    public int ID;
    public int Type;
    public int Title;
    public int Description;
    public int Accept_ItemKind;
    public int Accept_ItemID;
    public int Accept_ItemCount;
    public int Accept_QuestID;
    public int Mission_Kind;
    public int Mission_ID;
    public int Mission_Count;
    public int[] Reward_Kind;
    public int[] Reward_ID;
    public int[] Reward_Count;

    public static void Data_DicSet(J_QuestData j_obj)
    {
        for (int i = 0; i < j_obj.ID.Length; i++)
        {
            QuestData obj = new QuestData()
            {
                ID = j_obj.ID[i],
                Type = j_obj.Type[i],
                Title = j_obj.Title[i],
                Description = j_obj.Description[i],
                Accept_ItemKind = j_obj.Accept_ItemKind[i],
                Accept_ItemID = j_obj.Accept_ItemID[i],
                Accept_ItemCount = j_obj.Accept_ItemCount[i],
                Accept_QuestID = j_obj.Accept_QuestID[i],
                Mission_Kind = j_obj.Mission_Kind[i],
                Mission_ID = j_obj.Mission_ID[i],
                Mission_Count = j_obj.Mission_Count[i],
                Reward_Kind = new int[] { j_obj.Reward_Kind_0[i] },
                Reward_ID = new int[] { j_obj.Reward_ID_0[i] },
                Reward_Count = new int[] { j_obj.Reward_Count_0[i] },
            };
            D_Data.Add(obj.ID, obj);
        }
    }

    public static QuestData Get(int key)
    {
        if (D_Data.ContainsKey(key))
        {
            return D_Data[key];
        }
        else
        {
            if (D_Data.Count == 0)
            {
                throw new Exception("QuestData count is 0");
            }
            else
            {
                throw new Exception("QuestData key is null : " + key);
            }
        }
    }
    public static bool Get(int key, out QuestData questdata)
    {
        if (D_Data.Count == 0)
        {
            throw new Exception("QuestData count is 0");
        }
        else
        {
            if (D_Data.ContainsKey(key))
            {
                questdata = D_Data[key];
                return true;
            }
            else
            {
                questdata = null;
                return false;
            }
        }
    }
}
