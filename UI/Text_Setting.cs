using UnityEngine;

public class Text_Setting : TMPro.TextMeshPro
{
    [SerializeField] E_Text_Kind m_TextKind;
    [SerializeField] int m_Language_ID;

    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.Ev_Language_Change += Language_Change;
        switch (m_TextKind)
        {
            case E_Text_Kind.Text:
                break;
            case E_Text_Kind.Lang_ID:
                text = Data_Language.Instance.Get_Lang(m_Language_ID);
                break;
        }
    }
    protected override void OnDisable()
    {
        base.OnEnable();
        GameManager.Ev_Language_Change -= Language_Change;
        switch (m_TextKind)
        {
            case E_Text_Kind.Text:
                break;
            case E_Text_Kind.Lang_ID:
                break;
        }
    }

    void Language_Change(object sender, E_Language kind)
    {
        switch (m_TextKind)
        {
            case E_Text_Kind.Text:
                break;
            case E_Text_Kind.Lang_ID:
                text = Data_Language.Instance.Get_Lang(m_Language_ID);
                break;
        }
    }


    public void Change_Text(string str)
    {
        text = str;
    }
    public void Change_Lang_ID(int lang_id)
    {
        text = Data_Language.Instance.Get_Lang(lang_id);
    }


}
    
