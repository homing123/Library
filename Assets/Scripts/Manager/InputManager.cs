using System;
using UnityEngine;
using System.Collections.Generic;
using Define;
public class InputManager : Manager<InputManager>
{
    public static event EventHandler<bool> ev_ApplicationPause;

    bool Character;
    bool User;
    public static event EventHandler ev_Character; //캐릭터 조작 가능할때 불림
    public static event EventHandler ev_User; //유저 조작 가능할때 불림

    Camera m_Camera;
    Vector3 m_MouseInputPos;
    static Vector3 m_MouseWorldPos;
    public static Vector3 MouseWorldPos 
    {
        get
        {
            if (InputManager.Instance == null)
            {
                throw new Exception("Input Manager Instance is Null");
            }
            else
            {
                return m_MouseWorldPos;
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        ev_ApplicationPause?.Invoke(this, pause);
    }
    private void Start()
    {
        PlaySceneManager.ev_GameLoad += Ev_GameLoad;
        PlaySceneManager.ev_GameStart += Ev_GameStart;
        PlaySceneManager.ev_GameEnd += Ev_GameEnd;

    }

    void Ev_GameLoad(object sender, EventArgs args)
    {
        Character = false;
        User = true;
    }
    void Ev_GameStart(object sender, EventArgs args)
    {
        Character = true;
        User = true;
    }
    void Ev_GameEnd(object sender, EventArgs args)
    {
        Character = false;
        User = true;
    }
    private void Update()
    {
        if (Character)
        {
            ev_Character?.Invoke(this, EventArgs.Empty);
        }
        if (User)
        {
            ev_User?.Invoke(this, EventArgs.Empty);
        }

        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }
        m_MouseInputPos = Input.mousePosition;
        m_MouseInputPos.z = -m_Camera.transform.position.z;
        m_MouseWorldPos = m_Camera.ScreenToWorldPoint(m_MouseInputPos);
    }
}
