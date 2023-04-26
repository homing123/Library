using UnityEngine;
using System;
public class UI_Notch : MonoBehaviour
{
    [SerializeField] RectTransform m_TopNotch;
    [SerializeField] RectTransform m_BottomNotch;

    private void Awake()
    {
        Setting();
    }
    void Setting(object sender = null, EventArgs args = null)
    {
        Vector2 maxAnchor;
        Vector2 minAnchor;

        maxAnchor.x = 1;
        maxAnchor.y = 1;
        minAnchor.x = 0;
        minAnchor.y = (Screen.safeArea.position.y + Screen.safeArea.size.y) / Screen.height;

        m_TopNotch.anchorMin = minAnchor;
        m_TopNotch.anchorMax = maxAnchor;


        maxAnchor.x = 0;
        maxAnchor.y = (Screen.safeArea.position.y + NotchHandler.Bottom_Notch_Height) / Screen.height;
        minAnchor.x = 0;
        minAnchor.y = 0;

        m_BottomNotch.anchorMin = minAnchor;
        m_BottomNotch.anchorMax = maxAnchor;
    }
    private void OnEnable()
    {
        NotchHandler.Ev_Notch_Change += Setting;
    }
    private void OnDisable()
    {
        NotchHandler.Ev_Notch_Change -= Setting;
    }
}
