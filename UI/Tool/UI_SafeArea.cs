using UnityEngine;
using System;
public class UI_SafeArea : MonoBehaviour
{
    [HideInInspector] public RectTransform m_RectTransform;

    private void Awake()
    {
        Setting();
    }
    void Setting(object sender = null, EventArgs args = null)
    {
        Vector2 maxAnchor = Screen.safeArea.position + Screen.safeArea.size;
        Vector2 minAnchor = Screen.safeArea.position;

        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;
        minAnchor.x /= Screen.width;
        minAnchor.y += NotchHandler.Bottom_Notch_Height;
        minAnchor.y /= Screen.height;
        m_RectTransform.anchorMin = minAnchor;
        m_RectTransform.anchorMax = maxAnchor;
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
