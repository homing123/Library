using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RhythmSceneManager : PlayManager
{
    public static event EventHandler ev_GameStart;
    bool isPlaying;
    public static event EventHandler ev_GameEnd;
    [SerializeField] GameObject pre_bar;
    [SerializeField] Transform Bar_Parent;
    float cur_time;

    float m_interval = 1;
    float m_range = 0;
    [SerializeField] float m_speed = 5;
    float play_time;

    [SerializeField] GameObject pre_Start;
    [SerializeField] GameObject pre_End;

    List<GameObject> L_Bar = new List<GameObject>();

    float cur_interval;
    float[] Count_Per = new float[3] { 100, 0, 0 };

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        Instantiate(pre_Start);
    }
    public override void WaitStart()
    {
        StartCoroutine(Wait());
    }
    public override void GameRestart()
    {
        for(int i = 0; i < L_Bar.Count; i++)
        {
            Destroy(L_Bar[i].gameObject);
        }
        L_Bar.Clear();
        Bar_Parent.transform.position = Vector3.zero;
        WaitStart();
    }
    float Random_Custom(float min, float max)
    {
        float x = UnityEngine.Random.Range(0f, 2);
        float y = Mathf.Pow((x - 1), 3) + 1;

        return min + (y * (max - min) * 0.5f);
    }
    private void Update()
    {
        if (isPlaying)
        {
            play_time += Time.deltaTime;
            cur_time += Time.deltaTime;
            Check_PlayTime();
            StartCoroutine(Create_Bar());
            Move_Bar();
        }


    }
    void Check_PlayTime()
    {
        if(play_time < 30)
        {
            float value = play_time * 0.0025f;
            m_interval = 0.45f - value;
            m_range = value;
            m_speed = 7 + play_time / 30;
        }
        else
        {
            m_speed = 8;
            m_interval = 0.375f;
            m_range = 0.075f;
        }
        if (play_time < 20)
        {
            Count_Per[0] = Mathf.Max(0, 100 - play_time);
            Count_Per[1] = play_time * 10;
        }
        else
        {
            Count_Per[0] = 0;
            Count_Per[1] = play_time * 10;
            Count_Per[2] = play_time * 15;
        }
    }
    IEnumerator Create_Bar()
    {

        if (cur_time >= cur_interval)
        {
            cur_time -= cur_interval;
            cur_interval = Random_Custom(m_interval - m_range * 0.5f, m_interval + m_range * 0.5f);
            int count = Math_Define.Get_RandomResult(Count_Per) + 1;

            for (int i = 0; i < count; i++)
            {
                L_Bar.Add(Instantiate(pre_bar, new Vector3(0, 6, 0), Quaternion.identity, Bar_Parent));
                yield return new WaitForSeconds(0.08f);
            }
        }

       
       
    }
    void Move_Bar()
    {
        Bar_Parent.transform.position -= new Vector3(0, Time.deltaTime * m_speed, 0);
        for (int i = 0; i < Bar_Parent.childCount; i++)
        {
            if (Bar_Parent.GetChild(i).transform.position.y < -4)
            {
                StartCoroutine(End());
            }
        }
    }
    IEnumerator Wait()
    {
        isPlaying = false;
        yield return new WaitForSeconds(3);
        GameStart();
    }
    void GameStart()
    {
        cur_interval = m_interval;
        isPlaying = true;
        cur_time = 0;
        play_time = 0;
        Debug.Log("GameStart");
        ev_GameStart?.Invoke(this, EventArgs.Empty);
        InputManager.ev_MouseDown += Input;
    }
    [SerializeField] Transform m_Zone;
    void Input(object sender, int key)
    {
        if (key == 0)
        {
            float min_zone = m_Zone.transform.position.y - m_Zone.transform.localScale.y * 0.5f;
            float max_zone = m_Zone.transform.position.y + m_Zone.transform.localScale.y * 0.5f;
            GameObject down_bar = null;
            for (int i = 0; i < L_Bar.Count; i++)
            {
                if (down_bar == null)
                {
                    down_bar = L_Bar[i];
                }
                else
                {
                    if (L_Bar[i].transform.position.y < down_bar.transform.position.y)
                    {
                        down_bar = L_Bar[i];
                    }
                }
            }
            if (down_bar == null )
            {
                StartCoroutine(End());
            }
            else if(down_bar.transform.position.y < max_zone && down_bar.transform.position.y > min_zone)
            {
                Destroy(down_bar.gameObject);
                L_Bar.Remove(down_bar);
            }
            else
            {
                StartCoroutine(End());
            }
            
        }
    }
    IEnumerator End()
    {
        isPlaying = false;
        ev_GameEnd?.Invoke(this, EventArgs.Empty);
        Debug.Log("End");
        Instantiate(pre_End);
        InputManager.ev_MouseDown -= Input;
        yield return null;
    }
}
