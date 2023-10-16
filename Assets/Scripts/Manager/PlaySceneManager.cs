using System.Collections;
using UnityEngine;
using System;

public class PlaySceneManager : MonoBehaviour
{
    public static PlaySceneManager Instance;

    public static event EventHandler ev_GameLoad;
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
        ev_GameLoad?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(0.5f);
        GameStart();
    }
    void GameStart()
    {
        Debug.Log("GameStart");
        ev_GameStart?.Invoke(this, EventArgs.Empty);
    }

    IEnumerator End()
    {
        ev_GameEnd?.Invoke(this, EventArgs.Empty);
        Debug.Log("End");

        yield return null;
    }
   
}
