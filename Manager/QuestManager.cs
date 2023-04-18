using System;
using System.Collections.Generic;
using static Data_User;
using static UD_Func;

public class QuestManager : SingleMono<QuestManager>
{
    public static event EventHandler Ev_Quest_Accept;
    public static event EventHandler Ev_Quest_Complete;

    //모든 퀘스트들 현재상태 가지고있어야함 안그러면 모든ui가 매번판단할거임
    //ui들은 판단된 퀘스트들의 상태를 출력하는 형태로 제작

    Dictionary<int, E_QuestState> D_QuestState;
    public QuestManager()
    {
        UserData_Set_Action(Set_D_QuestState);
    }

    void Set_D_QuestState()
    {
        D_QuestState = new Dictionary<int, E_QuestState>();
        for (int i = 0; i < Data_Quest.Instance.M_ID.Length; i++)
        {
            D_Quest Cur_Quest = Data_Quest.Instance.Get_Quest(Data_Quest.Instance.M_ID[i]);
            if (Cur_Quest.Release == true)
            {
                D_QuestState.Add(Data_Quest.Instance.M_ID[i], Check_State(Cur_Quest));
            }
        }
    }
    void Update_D_QuestState()
    {
        if (D_QuestState == null)
        {
            foreach (KeyValuePair<int, E_QuestState> keys in D_QuestState)
            {
                D_Quest Cur_Quest = Data_Quest.Instance.Get_Quest(keys.Key);
                D_QuestState[keys.Key] = Check_State(Cur_Quest);
            }
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
        for (int i=0;i< UserData.L_Quest.Count;i++)
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
