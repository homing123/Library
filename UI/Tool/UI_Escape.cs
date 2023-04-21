using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine;
public class UI_Escape : MonoBehaviour
{

    public static List<ButtonClickedEvent> L_Escape_Action = new List<ButtonClickedEvent>();
    public static void Escape()
    {
        if (L_Escape_Action.Count > 0)
        {
            L_Escape_Action[L_Escape_Action.Count - 1].Invoke();
        }
        else
        {
            LogManager.Log("Escape GameExit");
        }
    }


    [Serializable]
    public class ButtonClickedEvent : UnityEvent { }

    [FormerlySerializedAs("onEscape")]
    public ButtonClickedEvent m_Escape_Click = new ButtonClickedEvent();

    private void OnEnable()
    {
        UI_Escape.L_Escape_Action.Add(m_Escape_Click);
    }
    private void OnDisable()
    {
        UI_Escape.L_Escape_Action.Remove(m_Escape_Click);
    }
}

[RequireComponent(typeof(UI_Escape))]
public class example_UI_Escape : MonoBehaviour
{
    UI_Escape m_Escape;

    void Setting()
    {
        if (m_Escape == null)
        {
            m_Escape = GetComponent<UI_Escape>();
            m_Escape.m_Escape_Click.AddListener(Escape);
        }
    }
    void Escape()
    {

    }
}