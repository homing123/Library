using System;
using System.Collections.Generic;
using static Data_User;
using static UD_Func;

public class QuestManager : SingleMono<QuestManager>
{
    public static event EventHandler Ev_Quest_Accept;
    public static event EventHandler Ev_Quest_Complete;

    public bool is_Daily_Completable; //현재 완료할수있는 일일 퀘스트가 있는지 여부
    public bool is_Weekly_Completable; //현재 완료할수있는 주간 퀘스트가 있는지 여부
    public static event EventHandler<bool> Ev_Daily_Completable;
    public static event EventHandler<bool> Ev_Weekly_Completable;

    Dictionary<int, E_QuestState> D_QuestState;
    public QuestManager()
    {
        UserData_Set_Action(Set_D_QuestState);
        Data.Ev_UserData_Change += Update_D_QuestState;
    }

    void Set_D_QuestState()
    {
        D_QuestState = new Dictionary<int, E_QuestState>();

        int Daily_Completable_Count = 0;
        int Weekly_Completable_Count = 0;

        //전체 퀘스트 중 Release == true 인것만 상태 확인
        for (int i = 0; i < Data_Quest.Instance.M_ID.Length; i++)
        {
            D_Quest Cur_Quest = Data_Quest.Instance.Get_Quest(Data_Quest.Instance.M_ID[i]);
            if (Cur_Quest.Release == true)
            {
                E_QuestState Cur_State = Check_State(Cur_Quest);
                D_QuestState.Add(Data_Quest.Instance.M_ID[i], Cur_State);
                if(Cur_State == E_QuestState.Completable)
                {
                    switch (Cur_Quest.Q_Type)
                    {
                        case E_QuestType.Daily:
                            Daily_Completable_Count++;
                            break;
                        case E_QuestType.Weekly:
                            Weekly_Completable_Count++;
                            break;
                    }
                }
              
            }
        }

        //이벤트 처리
        if (Daily_Completable_Count > 0)
        {
            is_Daily_Completable = true;
            Ev_Daily_Completable?.Invoke(this, true);
        }
        if (Weekly_Completable_Count > 0)
        {
            is_Weekly_Completable = true;
            Ev_Weekly_Completable?.Invoke(this, true);
        }
    }
    void Update_D_QuestState(object sender = null, EventArgs args = null)
    {
        if (D_QuestState == null)
        {
            int Daily_Completable_Count = 0;
            int Weekly_Completable_Count = 0;

            //Release == true 인것만 딕셔너리 안에 있음
            foreach (KeyValuePair<int, E_QuestState> keys in D_QuestState)
            {
                D_Quest Cur_Quest = Data_Quest.Instance.Get_Quest(keys.Key);

                E_QuestState Cur_State = Check_State(Cur_Quest);

                D_QuestState[keys.Key] = Cur_State;


                if (Cur_State == E_QuestState.Completable)
                {
                    switch (Cur_Quest.Q_Type)
                    {
                        case E_QuestType.Daily:
                            Daily_Completable_Count++;
                            break;
                        case E_QuestType.Weekly:
                            Weekly_Completable_Count++;
                            break;
                    }
                }
            }

            //이벤트 처리
            if (is_Daily_Completable ==false && Daily_Completable_Count > 0)
            {
                is_Daily_Completable = true;
                Ev_Daily_Completable?.Invoke(this, true);
            }
            else if(is_Daily_Completable == true && Daily_Completable_Count == 0)
            {
                is_Daily_Completable = false;
                Ev_Daily_Completable?.Invoke(this, false);
            }

            if (is_Weekly_Completable == false && Weekly_Completable_Count > 0)
            {
                is_Weekly_Completable = true;
                Ev_Weekly_Completable?.Invoke(this, true);
            }
            else if (is_Weekly_Completable == true && Weekly_Completable_Count == 0)
            {
                is_Weekly_Completable = false;
                Ev_Weekly_Completable?.Invoke(this, false);
            }
        }
    }

    public E_QuestState Get_State_ID(int quest_id)
    {
        if (D_QuestState.ContainsKey(quest_id))
        {
            return D_QuestState[quest_id];
        }
        else
        {
            return E_QuestState.None;
        }
    }
    E_QuestState Check_State(D_Quest cur_quest)
    {
        //if (cur_quest == null)
        //{
        //    return E_QuestState.None;
        //}

        //수락했는지 확인
        bool isAccept = false;
        for (int i = 0; i < UserData.L_Quest.Count; i++)
        {
            if (UserData.L_Quest[i].Quest_ID == cur_quest.M_ID)
            {
                isAccept = true;
                break;
            }
        }

        if (isAccept)
        {
            //클리어 가능한지 확인
            if (Get_Quest_User_Value(cur_quest.Mission_Kind) >= cur_quest.Mission_Value)
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

    public int Get_Quest_User_Value(int quest_kind)
    {
        switch (quest_kind)
        {
            case 1:
                return UserData.Attendance_Count;
            case 2:
                return UserData.Access_Time;
        }
        return 0;
    }
    public void Accept(D_Quest cur_quest)
    {
        if (D_QuestState[cur_quest.M_ID] == E_QuestState.Acceptable)
        {
            //현재 퀘스트에 추가
            Quest m_quest = new Quest();
            m_quest.Quest_ID = cur_quest.M_ID;
            m_quest.Progress = 0;
            UserData.L_Quest.Add(m_quest);

            Ev_Quest_Accept?.Invoke(this, EventArgs.Empty);
            Data.Instance.UserData_Changed();
        }
    }
    public void Complete(D_Quest cur_quest)
    {
        if (D_QuestState[cur_quest.M_ID] == E_QuestState.Completable)
        {
            //현재 퀘스트에서 제거
            Data_User.Quest User_Quest = Quest_Get_Progress_Quest(cur_quest.M_ID);
            UserData.L_Quest.Remove(User_Quest);

            //완료 퀘스트에 추가
            UserData.L_Clear_Quest.Add(cur_quest.M_ID);

            //보상받기
            for (int i = 0; i < cur_quest.Reward_Kind.Length; i++)
            {
                InvenManager.Instance.Acquire_Item(cur_quest.Reward_Kind[i], cur_quest.Reward_ID[i], cur_quest.Reward_Value[i]);
            }
            Ev_Quest_Complete?.Invoke(this, EventArgs.Empty);
            Data.Instance.UserData_Changed();
        }
    }
}
