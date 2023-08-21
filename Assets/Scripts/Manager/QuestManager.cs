using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
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
    private void Awake()
    {
        User_Quest.m_UserQuest = new User_Quest();
        StreamingManager.Read_Data<J_QuestData>(StreamingManager.Get_StreamingPath(J_QuestData.Path), QuestData.Data_DicSet);
    }
  
    public E_QuestState Get_State(int id)
    {
        return User_Quest.m_UserQuest.Get_State(id);
    }
    public void Accept(int id)
    {
        User_Quest.m_UserQuest.Accept(id);
    }
    #region Cancel override

    public void Cancel(int[] id)
    {
        User_Quest.m_UserQuest.Cancel(id);
    }
    public void Cancel(List<int> id)   
    {
        User_Quest.m_UserQuest.Cancel(id.ToArray());
    }
    #endregion
    public void Complete(int id)
    {
        User_Quest.m_UserQuest.Complete(id);
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
        Cancel(Test_QuestID.ToArray());
    }
    [ContextMenu("퀘스트 상태 확인")]
    public void Test_StateLog()
    {
        Debug.Log(Test_QuestID + " " + Get_State(Test_QuestID));
    }
    [ContextMenu("유저 퀘스트 로그")]
    public void Test_UserLog()
    {
        foreach(int key in User_Quest.m_UserQuest.D_Quest.Keys)
        {
            Debug.Log("key : " + key + " state : " + User_Quest.m_UserQuest.D_Quest[key].State);
        }
    }
    #endregion
}
#region UD_Quest
public class User_Quest : UserData
{
    public const string Path = "Quest";
    public static User_Quest m_UserQuest;
    public static event EventHandler ev_QuestChanged;

    public Dictionary<int, Quest> D_Quest = new Dictionary<int, Quest>();
    public class Quest 
    {
        public E_QuestState State;
    }
    public override void Init_UD()
    {

    }
    public E_QuestState Get_State(int id)
    {
        QuestData cur_quest = QuestData.Get(id);
        if (cur_quest == null)
        {
            return E_QuestState.None;
        }
        E_QuestState state = m_UserQuest.UserQuest_State(id);


        switch (state)
        {
            case E_QuestState.None:
                //수락 가능한지 확인 후 리턴
                bool Acceptable_Item = false;
                if (cur_quest.Accept_ItemCount == 0)
                {
                    Acceptable_Item = true;
                }
                else
                {
                    Acceptable_Item = InvenManager.Instance.CheckItemCount((cur_quest.Accept_ItemKind, cur_quest.Accept_ItemID, cur_quest.Accept_ItemCount).ToArray());
                }

                bool Acceptable_Quest = false;
                if (cur_quest.Accept_QuestID == 0 || m_UserQuest.UserQuest_State(cur_quest.Accept_QuestID) == E_QuestState.Complete)
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
                if (InvenManager.Instance.CheckItemCount((cur_quest.Mission_Kind, cur_quest.Mission_ID, cur_quest.Mission_Count).ToArray()))
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
    public async void Accept(int id)
    {
        if (UserManager.Use_Local)
        {
            m_UserQuest.D_Quest = await LocalUser_Quest.Accept(id);
            ev_QuestChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            //server
        }
    }
    public async void Cancel(int[] id)
    {
        if (UserManager.Use_Local)
        {
            m_UserQuest.D_Quest = await LocalUser_Quest.Cancel(id);
            ev_QuestChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            //server
        }
    }
    public async void Complete(int id)
    {
        if (UserManager.Use_Local)
        {
            m_UserQuest.D_Quest = await LocalUser_Quest.Complete(id);
            ev_QuestChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            //server
        }
    }
}

public class LocalUser_Quest
{
    static async  Task<E_QuestState> Get_State(int id)
    {
        await Task.Delay(1);
        User_Quest m_userquest = UserManager.Load_LocalUD<User_Quest>(User_Quest.Path);
        QuestData cur_quest = QuestData.Get(id);
        if (cur_quest == null)
        {
            return E_QuestState.None;
        }
        E_QuestState state = m_userquest.UserQuest_State(id);


        switch (state)
        {
            case E_QuestState.None:
                //수락 가능한지 확인 후 리턴
                bool Acceptable_Item = false;
                if (cur_quest.Accept_ItemCount == 0)
                {
                    Acceptable_Item = true;
                }
                else
                {
                    Acceptable_Item = await LocalUser_Inven.CheckItemCount( (cur_quest.Accept_ItemKind, cur_quest.Accept_ItemID, cur_quest.Accept_ItemCount).ToArray());
                }

                bool Acceptable_Quest = false;
                if (cur_quest.Accept_QuestID == 0 || m_userquest.UserQuest_State(cur_quest.Accept_QuestID) == E_QuestState.Complete)
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
                if (await LocalUser_Inven.CheckItemCount((cur_quest.Mission_Kind, cur_quest.Mission_ID, cur_quest.Mission_Count).ToArray()))
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

    public static async Task<Dictionary<int, User_Quest.Quest>> Accept(int id)
    {
        await Task.Delay(1);
        User_Quest m_userquest = UserManager.Load_LocalUD<User_Quest>(User_Quest.Path);
        E_QuestState state = await Get_State(id);
        if (state == E_QuestState.Acceptable)
        {
            if (m_userquest.D_Quest.ContainsKey(id))
            {
                m_userquest.D_Quest[id].State = E_QuestState.UnCompletable;
            }
            else
            {
                m_userquest.D_Quest.Add(id, new User_Quest.Quest() { State = E_QuestState.UnCompletable });

            }
        }
        else
        {
            throw new Exception("State is not Acceptable : id " + id + " ,state " + state);
        }
        UserManager.Save_LocalUD(User_Quest.Path, m_userquest);
        return m_userquest.D_Quest;

    }
    public static async Task<Dictionary<int, User_Quest.Quest>> Cancel(int[] id)
    {
        await Task.Delay(1);
        User_Quest m_userquest = UserManager.Load_LocalUD<User_Quest>(User_Quest.Path);
        for (int i = 0; i < id.Length; i++)
        {
            m_userquest.D_Quest.Remove(id[i]);
        }
        UserManager.Save_LocalUD(User_Quest.Path, m_userquest);
        return m_userquest.D_Quest;
    }
    public static async Task<Dictionary<int, User_Quest.Quest>> Complete(int id)
    {
        await Task.Delay(1);
        User_Quest m_userquest = UserManager.Load_LocalUD<User_Quest>(User_Quest.Path);
        E_QuestState state = await Get_State(id);
        if (state == E_QuestState.Completable)
        {
            if (m_userquest.D_Quest.ContainsKey(id))
            {
                m_userquest.D_Quest[id].State = E_QuestState.Complete;

                QuestData cur_quest = QuestData.Get(id);
                InvenManager.Instance.Add(cur_quest.Reward_Info);
            }
            else
            {
                m_userquest.D_Quest.Add(id, new User_Quest.Quest() { State = E_QuestState.Complete });

            }
        }
        else
        {
            throw new Exception("State is not Completable : id " + id + " ,state " + state);
        }
        UserManager.Save_LocalUD(User_Quest.Path, m_userquest);
        return m_userquest.D_Quest;
    }
}
public static class Ex_Quest
{
    public static E_QuestState UserQuest_State(this User_Quest m_userquest, int id)
    {
        if (m_userquest.D_Quest.ContainsKey(id))
        {
            return m_userquest.D_Quest[id].State;
        }
        else
        {
            return E_QuestState.None;
        }
    }

}
#endregion
#region Streaming Quest
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
    public (int, int, int)[] Reward_Info;

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
            };
            obj.Reward_Info = InvenManager.ToItemInfo(new int[] { j_obj.Reward_Kind_0[i] }, new int[] { j_obj.Reward_ID_0[i] }, new int[] { j_obj.Reward_Count_0[i] });

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

#endregion