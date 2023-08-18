using System.Collections;
using UnityEngine;
using System;
public class PlaySceneManager : MonoBehaviour
{
    int Score;
    public bool isdark;

    public static PlaySceneManager Instance;
    public static event EventHandler ev_GameStart;
    public static event EventHandler ev_GameEnd;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
        GameStart();
    }
    void GameStart()
    {
        Debug.Log("GameStart");
        isdark = true;
        ev_GameStart?.Invoke(this, EventArgs.Empty);
        InputManager.ev_MouseDown += Click;
    }

    IEnumerator End()
    {
        ev_GameEnd?.Invoke(this, EventArgs.Empty);
        InputManager.ev_MouseDown -= Click;
        Debug.Log("End");

        yield return null;
    }
    void ClickSuccess()
    {
        Debug.Log(++Score);
    }
    void Click(object sender, int buttonidx)
    {
        if(buttonidx == 0)
        {
            if (isdark)
            {
                ClickSuccess();
            }
            else
            {
                StartCoroutine(End());
            }
        }
    }
   
   
    private void Update()
    {
       
    }
}
