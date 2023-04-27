using UnityEngine;
using UnityEngine.UI;

public enum E_Alarm
{
    None=0,
    Quest_Completable = 1,
    Inven_Equip_Available = 10,
}

public class Alarm_Setting : MonoBehaviour
{
    [SerializeField] E_Alarm m_AlarmKind;

    [SerializeField] E_QuestType[] m_QuestType;
    [SerializeField] E_ItemKind[] m_ItemKind;
    Image m_Image;

    private void Awake()
    {
        if (m_Image == null)
        {
            m_Image = GetComponent<Image>();
        }
    }
    void Set_Enable(object sender, bool enable)
    {
        m_Image.enabled = enable;
    }

    public void OnEnable()
    {
        bool isenable = false;
        switch (m_AlarmKind)
        {
            case E_Alarm.Quest_Completable:
                for (int i = 0; i < m_QuestType.Length; i++)
                {
                    isenable = QuestManager.Get_Alarm_Active(m_QuestType[i]) ? true : isenable;
                    QuestManager.D_Completable_Event[m_QuestType[i]] += Set_Enable;
                }
                break;
        }
        Set_Enable(this, isenable);

    }
    private void OnDisable()
    {
        switch (m_AlarmKind)
        {
            case E_Alarm.Quest_Completable:
                for (int i = 0; i < m_QuestType.Length; i++)
                {
                    QuestManager.D_Completable_Event[m_QuestType[i]] -= Set_Enable;

                }
                break;
        }
    }
}
