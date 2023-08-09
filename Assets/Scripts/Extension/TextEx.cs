using UnityEngine;


public class TextEx : TMPro.TextMeshProUGUI
{

    public enum E_TextType
    {
        Text = 0,
        Language = 1,
    }

    [SerializeField]
    E_TextType m_CurType;

    [SerializeField]
    int m_LanguageID;



    protected override void Awake()
    {
        base.Awake();
        LanguageManager.ev_LanguageChanged += ev_ChangeLanguage;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        LanguageManager.ev_LanguageChanged -= ev_ChangeLanguage;
    }

    void ev_ChangeLanguage(object sender, E_Language lang)
    {
        switch (m_CurType)
        {
            case E_TextType.Text:
                break;
            case E_TextType.Language:
                text = LanguageData.Get(m_LanguageID);
                break;
        }
    }

    public void Text(string str)
    {
        m_CurType = E_TextType.Text;
        text = str;
    }

    public void Text(int lang_id)
    {
        m_CurType = E_TextType.Language;
        m_LanguageID = lang_id;
        text = LanguageData.Get(m_LanguageID);
    }
}

