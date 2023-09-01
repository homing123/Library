using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.IO;
public enum E_QuestState
{
    None = 0, //����
    Complete = 1, //�Ϸ��
    Acceptable = 2, //����������
    UnAcceptable = 3, //�����Ұ�����
    Completable = 4,  //�Ϸᰡ����
    UnCompletable = 5 //�Ϸ�Ұ�����
}
public class QuestManager : Manager<QuestManager>
{
    public static readonly int[] Attendance_Quest = new int[] { 1, 2, 3, 4, 5, 6, 7 };
    private void Awake()
    {
        User_Quest.m_UserQuest = new User_Quest();
        StreamingManager.LT_StrLoad.Add_Task(new Task(() =>
        {
            StreamingManager.Read_Data<J_QuestData>(StreamingManager.Get_StreamingPath(J_QuestData.Path), QuestData.Data_DicSet);
        }));
        TimeManager.ev_DailyReset += Check_Attendance;
    }
    public async void Check_Attendance(object sender, int daycount)
    {
        if (Get_State(Attendance_Quest.Length - 1) == E_QuestState.Complete)
        {
            await InvenManager.Instance.Add_Remove_byKind(null, ((int)E_ItemKind.SystemData, ItemData.AttendanceQuest_Count_ID, 7).ToArray());
            await Cancel(Attendance_Quest);
            await Accept(Attendance_Quest);
        }
    }
    public E_QuestState Get_State(int id)
    {
        return User_Quest.m_UserQuest.Get_State(id);
    }
    public async Task Accept(int[] id)
    {
        User_Quest.m_UserQuest.Accept(id);
    }

    public async Task Cancel(int[] id)
    {
        User_Quest.m_UserQuest.Cancel(id);
    }

    public async Task Complete(int[] id)
    {
        var reward_info = await User_Quest.m_UserQuest.Complete(id);
        Debug.Log("����Ʈ ���� : ");
        for (int i = 0; i < reward_info.Length; i++)
        {
            Debug.Log(" kind : " + reward_info[i].kind + " id : " + reward_info[i].id + " count : " + reward_info[i].count);
        }
    }
    public async Task Set_State(int[] id, E_QuestState state)
    {

    }

    #region Test
    [SerializeField] int Test_QuestID;
    [ContextMenu("����Ʈ ����")]
    public void Test_Accept()
    {
        Accept(Test_QuestID.ToArray());
    }
    [ContextMenu("����Ʈ �Ϸ�")]
    public void Test_Complete()
    {
        Complete(Test_QuestID.ToArray());
    }
    [ContextMenu("����Ʈ ���")]
    public void Test_Cancel()
    {
        Cancel(Test_QuestID.ToArray());
    }
    [ContextMenu("����Ʈ ���� Ȯ��")]
    public void Test_StateLog()
    {
        Debug.Log(Test_QuestID + " " + Get_State(Test_QuestID));
    }
    [ContextMenu("���� ����Ʈ �α�")]
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
public class User_Quest : UserData_Server
{
    public const string Path = "Quest";
    public static User_Quest m_UserQuest;
    public static Action ac_QuestChanged = () =>
    {
        Debug.Log("QuestChanged Action");
    };

    public Dictionary<int, Quest> D_Quest = new Dictionary<int, Quest>();
    public class Quest 
    {
        public E_QuestState State;
    }
    public override async Task Load()
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        if (UserManager.Use_Local)
        {
            if (UserManager.Exist_LocalUD(Path))
            {
                var data = UserManager.Load_LocalUD<User_Quest>(Path);
                D_Quest = data.D_Quest;
            }
            else
            {
                Debug.Log("Quest_Init");
                for (int i = 0; i < QuestManager.Attendance_Quest.Length; i++)
                {
                    D_Quest.Add(QuestManager.Attendance_Quest[i], new Quest() 
                    {
                        State = E_QuestState.UnCompletable
                    });
                }
                UserManager.Save_LocalUD(Path, this);
            }
        }
        else
        {
            //�������� �ִ��������� Ȯ�� �� ������ �����ؼ� ������ �װ� �������
        }
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
                //���� �������� Ȯ�� �� ����
                bool Acceptable_byItem = false;
                if (cur_quest.L_AcceptItem.Empty() == false)
                {
                    Acceptable_byItem = true;
                }
                else
                {
                    var accept_items = cur_quest.L_AcceptItem.ToArray();
                    Acceptable_byItem = InvenManager.Instance.CheckItemCount(accept_items);
                }

                bool Acceptable_byQuest = false;
                if (cur_quest.AcceptQuest_ID == 0 || m_UserQuest.UserQuest_State(cur_quest.AcceptQuest_ID) == E_QuestState.Complete)
                {
                    Acceptable_byQuest = true;
                }

                if (Acceptable_byItem && Acceptable_byQuest)
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
                //�Ϸ� �������� Ȯ�� �� ����
                var mission_item = cur_quest.L_MissionItem.ToArray();
                if (InvenManager.Instance.CheckItemCount(mission_item))
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
    public async Task Accept(int[] id)
    {
        if (UserManager.Use_Local)
        {
            m_UserQuest.D_Quest = await LocalUser_Quest.Accept(id);
            ac_QuestChanged?.Invoke();
        }
        else
        {
            //server
        }
    }
    public async Task Cancel(int[] id)
    {
        if (UserManager.Use_Local)
        {
            m_UserQuest.D_Quest = await LocalUser_Quest.Cancel(id);
            ac_QuestChanged?.Invoke();
        }
        else
        {
            //server
        }
    }
    public async Task<(int kind, int id, int count)[]> Complete(int[] id)
    {
        //�Ϸ�� �κ�, �����ڽ�, ����Ʈ �����ͺ����
        if (UserManager.Use_Local)
        {
            var data = await LocalUser_Quest.Complete(id);
            m_UserQuest.D_Quest = data.d_quest;
            User_Inven.m_UserInven.D_Inven = data.d_inven;
            User_Randombox.m_UserRandombox.D_Randombox = data.d_randombox;
            ac_QuestChanged?.Invoke();
            User_Inven.ac_InvenChanged?.Invoke();
            User_Randombox.ac_RandomboxChanged?.Invoke();
            return data.reward_info;
        }
        else
        {
            //server
        }
    }
}

public class LocalUser_Quest
{
    static async  Task<E_QuestState[]> Get_State(int[] id)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Quest m_userquest = UserManager.Load_LocalUD<User_Quest>(User_Quest.Path);

        var arr_state = new E_QuestState[id.Length];
        for (int i = 0; i < id.Length; i++) 
        {
            QuestData cur_quest = QuestData.Get(id[i]);
            if (cur_quest == null)
            {
                arr_state[i] = E_QuestState.None;
            }
            else 
            {
                E_QuestState state = m_userquest.UserQuest_State(id[i]);

                switch (state)
                {
                    case E_QuestState.None:
                        //�������� ���� ������ Ȯ��
                        bool Acceptable_byItem = false;
                        if (cur_quest.L_AcceptItem.Empty() == false)
                        {
                            Acceptable_byItem = true;
                        }
                        else
                        {
                            var accept_items = cur_quest.L_AcceptItem.ToArray();
                            Acceptable_byItem = await LocalUser_Inven.CheckItemCount(accept_items);
                        }
                        //�������� ���� ����Ʈ Ȯ��
                        bool Acceptable_byQuest = false;
                        if (cur_quest.AcceptQuest_ID == 0 || m_userquest.UserQuest_State(cur_quest.AcceptQuest_ID) == E_QuestState.Complete)
                        {
                            Acceptable_byQuest = true;
                        }

                        if (Acceptable_byItem && Acceptable_byQuest)
                        {
                            arr_state[i] = E_QuestState.Acceptable;
                        }
                        else
                        {
                            arr_state[i] = E_QuestState.UnAcceptable;
                        }
                        break;
                    case E_QuestState.Complete:
                        arr_state[i] = E_QuestState.Complete;
                        break;
                    case E_QuestState.Completable:
                    case E_QuestState.UnCompletable:
                        //�Ϸ� �������� Ȯ�� �� ����
                        var mission_item = cur_quest.L_MissionItem.ToArray();
                        if (await LocalUser_Inven.CheckItemCount(mission_item))
                        {
                            arr_state[i] = E_QuestState.Completable;
                        }
                        else
                        {
                            arr_state[i] = E_QuestState.UnCompletable;
                        }
                        break;
                    default:
                        throw new Exception("QuestState case is empty : " + state);
                }
            }
        }
        return arr_state;
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async Task<Dictionary<int, User_Quest.Quest>> Accept(int[] id)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Quest m_userquest = UserManager.Load_LocalUD<User_Quest>(User_Quest.Path);

        //����Ʈ�� ���� ���� �޾ƿ���
        var arr_state = await Get_State(id);

        //���� ó��
        for (int i = 0; i < id.Length; i++)
        {
            if (arr_state[i] == E_QuestState.Acceptable)
            {
                if (m_userquest.D_Quest.ContainsKey(id[i]))
                {
                    m_userquest.D_Quest[id[i]].State = E_QuestState.UnCompletable;
                }
                else
                {
                    m_userquest.D_Quest.Add(id[i], new User_Quest.Quest() { State = E_QuestState.UnCompletable });

                }
            }
            else
            {
                throw new Exception("State is not Acceptable : id " + id[i] + " ,state " + arr_state[i]);
            }
        }

        UserManager.Save_LocalUD(User_Quest.Path, m_userquest);
        return m_userquest.D_Quest;

    }

    /// <summary>
    /// ����Ʈ ��� �� �Ϸ��Ѱ� ����
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async Task<Dictionary<int, User_Quest.Quest>> Cancel(int[] id)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Quest m_userquest = UserManager.Load_LocalUD<User_Quest>(User_Quest.Path);
        for (int i = 0; i < id.Length; i++)
        {
            m_userquest.D_Quest.Remove(id[i]);
        }
        UserManager.Save_LocalUD(User_Quest.Path, m_userquest);
        return m_userquest.D_Quest;
    }

    /// <summary>
    /// �Ϸ�
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async Task<(Dictionary<int, User_Quest.Quest> d_quest,Dictionary<int,User_Inven.Inven> d_inven, (int kind, int id, int count)[] reward_info, Dictionary<int,int> d_randombox, (int kind, int id, int count)[] Replacement_Info)> Complete(int[] id)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Quest m_userquest = UserManager.Load_LocalUD<User_Quest>(User_Quest.Path);
        List<(int kind, int id, int count)> l_reward = new List<(int kind, int id, int count)>();
        //���� �޾ƿ���
        var arr_state = await Get_State(id);

        //�Ϸ�ó��
        for (int i = 0; i < id.Length; i++)
        {
            if (arr_state[i] == E_QuestState.Completable)
            {
                if (m_userquest.D_Quest.ContainsKey(id[i]))
                {
                    m_userquest.D_Quest[id[i]].State = E_QuestState.Complete;
                }
                else
                {
                    m_userquest.D_Quest.Add(id[i], new User_Quest.Quest() { State = E_QuestState.Complete });
                }
                QuestData cur_quest = QuestData.Get(id[i]);

                //�ϷẸ���Ͽ� �߰�
                l_reward.Add(cur_quest.L_RewardItem);
            }
            else
            {
                throw new Exception("State is not Completable : id " + id + " ,state " + arr_state[i]);
            }
        }

        //�����ڽ� ������� ����
        var data = await LocalUser_Randombox.Open_Randombox(l_reward.ToArray());

        //����ȹ�� ó��
        var inven_change_data = await LocalUser_Inven.Add_Remove_byKind(data.reward_info, null);

        UserManager.Save_LocalUD(User_Quest.Path, m_userquest);
        return (m_userquest.D_Quest, inven_change_data.D_Inven, data.reward_info, data.d_randombox, inven_change_data.Replacement_Info);
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
[Serializable]
public class J_QuestData
{
    public const string Path = "Quest";
    public int[] ID;
    public int[] Type;
    public int[] Title;
    public int[] Description;
    public int[] AcceptItem_Kind;
    public int[] AcceptItem_ID;
    public int[] AcceptItem_Count;
    public int[] AcceptQuest_ID;
    public int[] Mission_Kind; 
    public int[] Mission_ID;
    public int[] Mission_Count;
    public int[] Reward_Kind; 
    public int[] Reward_ID;
    public int[] Reward_Count;
}
public class QuestData
{
    static Dictionary<int, QuestData> D_Data = new Dictionary<int, QuestData>();

    public int ID;
    public int Type;
    public int Title;
    public int Description;
    public List<(int kind, int id, int count)> L_AcceptItem = new List<(int kind, int id, int count)>();
    public int AcceptQuest_ID;
    public List<(int kind, int id, int count)> L_MissionItem = new List<(int kind, int id, int count)>();
    public List<(int kind, int id, int count)> L_RewardItem = new List<(int kind, int id, int count)>();


    public static void Data_DicSet(J_QuestData j_obj)
    {
        for (int i = 0; i < j_obj.ID.Length; i++)
        {
            if(D_Data.ContainsKey(j_obj.ID[i]) == false)
            {
                QuestData obj = new QuestData()
                {
                    ID = j_obj.ID[i],
                    Type = j_obj.Type[i],
                    Title = j_obj.Title[i],
                    Description = j_obj.Description[i],
                    AcceptQuest_ID = j_obj.AcceptQuest_ID[i],
                };
                D_Data.Add(obj.ID, obj);
            }

            QuestData cur_quest = D_Data[j_obj.ID[i]];
            cur_quest.L_AcceptItem.AddItemInfo(j_obj.AcceptItem_Kind[i], j_obj.AcceptItem_ID[i], j_obj.AcceptItem_Count[i]);
            cur_quest.L_MissionItem.AddItemInfo(j_obj.Mission_Kind[i], j_obj.Mission_ID[i], j_obj.Mission_Count[i]);
            cur_quest.L_RewardItem.AddItemInfo(j_obj.Reward_Kind[i], j_obj.Reward_ID[i], j_obj.Reward_Count[i]);
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