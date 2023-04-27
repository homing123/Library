using System;
using System.Collections.Generic;
using static Data_User;
using static UD_Func;


//퀘스트의 type 별로 알람유무 받을수있는 bool 값과 event 가 제공되어야함
//퀘스트의 진행도가 변경될시 갱신할수있는 event 가 있어야함
//수락 : 수락가능한 상태일때만 가능하며 수락시 진행중인 퀘스트ud에 추가되며 그후 퀘스트 객체의 상태를 갱신한다.
//클리어 : 클리어 가능한 상태일때만 가능하며 클리어시 진행중인 퀘스트ud에서 제외되며 클리어한 퀘스트ud에 추가된다. 그후 퀘스트 객체의 상태를 갱신한다.
//초기화 : 진행중 및 클리어한 퀘스트ud에 있는 애들의 초기화되는 타입인 퀘스트를 모두 제거하며 그후 퀘스트 객체의 상태를 갱신한다.


public class QuestManager : SingleMono<QuestManager>
{
    static Dictionary<E_QuestType, int> D_Completable_Count = new Dictionary<E_QuestType, int>();
    public static Dictionary<E_QuestType, EventHandler<bool>> D_Completable_Event = new Dictionary<E_QuestType, EventHandler<bool>>();
    static Dictionary<int, EventHandler> D_Mission_Event = new Dictionary<int, EventHandler>();

   
    Dictionary<int, QuestInfo> D_QuestInfo = new Dictionary<int, QuestInfo>();
    public QuestManager()
    {
        foreach(E_QuestType value in Enum.GetValues(typeof(E_QuestType)))
        {
            D_Completable_Event.Add(value, null);
            D_Completable_Count.Add(value, 0);
        }

        UserData_Set_Action(Set_D_QuestInfo);
    }
    void Set_D_QuestInfo()
    {
        //전체 퀘스트 중 Release == true 인것만 상태 확인
        for (int i = 0; i < Data_Quest.Instance.M_ID.Length; i++)
        {
            D_Quest Cur_Quest = Data_Quest.Instance.Get_Quest(Data_Quest.Instance.M_ID[i]);
            if (Cur_Quest.Release == true)
            {
                QuestInfo info = new QuestInfo(Cur_Quest);
                D_QuestInfo.Add(Data_Quest.Instance.M_ID[i], info);
            }
        }
    }
    
    public QuestInfo Get_QuestInfo(int quest_id)
    {
        if (D_QuestInfo.ContainsKey(quest_id))
        {
            return D_QuestInfo[quest_id];
        }
        else
        {
            return null;
        }
    }
    public static void Completable_Count(E_QuestType type,int i)
    {
        int before_count = D_Completable_Count[type];
        D_Completable_Count[type] += i;
        int current_count = D_Completable_Count[type];

        //알람제거
        if(before_count==0 && current_count == 1)
        {
            D_Completable_Event[type]?.Invoke(null, true);
        }
        else if(before_count == 1 && current_count == 0)
        {
            D_Completable_Event[type]?.Invoke(null, false);
        }  
    }
    public static bool Get_Alarm_Active(E_QuestType type)
    {
        if (D_Completable_Count[type] > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static E_QuestState Get_State(D_Quest cur_quest)
    {
        //if (cur_quest == null)
        //{
        //    return E_QuestState.None;
        //}

        //수락했는지 확인
        bool isAccept = false;
        for (int i = 0; i < UserData.L_Quest.Count; i++)
        {
            if (UserData.L_Quest[i] == cur_quest.M_ID)
            {
                isAccept = true;
                break;
            }
        }

        if (isAccept) //진행중
        {
            //클리어 가능한지 확인
            if (UserData.D_Mission[cur_quest.Mission_Kind] >= cur_quest.Mission_Value)
            {
                return E_QuestState.Completable;
            }
            else
            {
                return E_QuestState.UnCompletable;
            }
        }
        else
        {
            if (UserData.L_Clear_Quest.Contains(cur_quest.M_ID) == true)
            {
                //클리어한 퀘스트
                return E_QuestState.Complete;
            }
            else
            {
                //수락가능한지 판단
                bool Acceptable = true;

                if (Acceptable == false)
                {
                    return E_QuestState.UnAcceptable;
                }
                else
                {
                    return E_QuestState.Acceptable;
                }
            }
        }
    }

    public static void Accept(QuestInfo info, Action<D_Quest> Ac_Accept = null)
    {
        if (info.State == E_QuestState.Acceptable)
        {
            //현재 퀘스트에 추가
            UserData.L_Quest.Add(info.Cur_Quest.M_ID);
            Data.Instance.UserData_Changed();

            Ac_Accept?.Invoke(info.Cur_Quest);
            info.CheckState();
        }
    }
    public static void Complete(QuestInfo info, Action<D_Quest> Ac_Complete = null)
    {
        if (info.State == E_QuestState.Completable)
        {
            D_Quest Cur_Quest = info.Cur_Quest;

            //현재 퀘스트에서 제거
            UserData.L_Quest.Remove(Cur_Quest.M_ID);

            //완료 퀘스트에 추가
            UserData.L_Clear_Quest.Add(Cur_Quest.M_ID);

            //보상받기
            for (int i = 0; i < Cur_Quest.Reward_Kind.Length; i++)
            {
                InvenManager.Instance.Acquire_Item(Cur_Quest.Reward_Kind[i], Cur_Quest.Reward_ID[i], Cur_Quest.Reward_Value[i]);
            }
            Data.Instance.UserData_Changed();

            Ac_Complete?.Invoke(Cur_Quest);
            info.CheckState();
        }
    }
   
    public static void Add_MissionEvent(int mission_kind, EventHandler ev)
    {
        if (D_Mission_Event.ContainsKey(mission_kind))
        {
            D_Mission_Event[mission_kind] += ev;
        }
        else
        {
            D_Mission_Event.Add(mission_kind, ev);
        }
    }
    public static void Remove_MisstionEvent(int mission_kind, EventHandler ev)
    {
        if (D_Mission_Event.ContainsKey(mission_kind))
        {
            D_Mission_Event[mission_kind] -= ev;
        }
    }
    public static void Call_MissionEvent(int mission_kind)
    {       
        //이벤트 없다는건 등록된게 없다는 뜻 부를필요 x
        if (D_Mission_Event.ContainsKey(mission_kind))
        {
            D_Mission_Event[mission_kind]?.Invoke(null, EventArgs.Empty);
        }
    }
}


public class QuestInfo
{
    public D_Quest Cur_Quest;
    public E_QuestState State;
    public event EventHandler Ev_Acceptable;
    public event EventHandler Ev_Completable;


    public QuestInfo(D_Quest cur_quest)
    {
        Cur_Quest = cur_quest;
        State = QuestManager.Get_State(Cur_Quest);
        QuestManager.Add_MissionEvent(Cur_Quest.Mission_Kind, (sender, args) => { CheckState(); });
    }
    public void CheckState()
    {
        E_QuestState cur_state = QuestManager.Get_State(Cur_Quest);
        if (State != cur_state)
        {
            //이전값 처리
            switch (State)
            {
                case E_QuestState.Completable:
                    QuestManager.Completable_Count(Cur_Quest.Q_Type, -1);
                    break;
            }

            //현재값 처리
            switch (cur_state)
            {
                case E_QuestState.Acceptable:
                    Ev_Acceptable?.Invoke(this, EventArgs.Empty);
                    break;
                case E_QuestState.Completable:
                    Ev_Completable?.Invoke(this, EventArgs.Empty);
                    QuestManager.Completable_Count(Cur_Quest.Q_Type, 1);
                    break;
            }

            State = cur_state;
        }
    }
}