using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BG : MonoBehaviour
{
    SpriteRenderer M_SPRender;
    private void Awake()
    {
        M_SPRender = GetComponent<SpriteRenderer>();
        M_SPRender.color = Color.black;
        PlaySceneManager.ev_GameStart += Ev_GameStart;
        PlaySceneManager.ev_GameEnd += Ev_GameEnd;

    }
    private void OnDestroy()
    {
        PlaySceneManager.ev_GameStart -= Ev_GameStart;
        PlaySceneManager.ev_GameEnd -= Ev_GameEnd;

    }
    void Ev_GameStart(object sender, EventArgs args)
    {
        StartCoroutine(Wait());
    }
    void Ev_GameEnd(object sender, EventArgs args)
    {
        StopAllCoroutines();
    }
    [SerializeField]
    IEnumerator Wait()
    {
        float wait_time = UnityEngine.Random.Range(1, 2.5f);
        yield return new WaitForSeconds(wait_time);
        StartCoroutine(Light_Sequence());
    }
    IEnumerator Light_Sequence()
    {
        float lightOn_time = UnityEngine.Random.Range(0.2f, 0.5f);
        float cur_lightOn_time = 0;
        while (true)
        {
            cur_lightOn_time += Time.deltaTime;
            float timevalue = Math_Define.Get_TimeValue(cur_lightOn_time, lightOn_time, 0, 1);
            M_SPRender.color = new Color(timevalue, timevalue, timevalue, 1);
            yield return new WaitForEndOfFrame();
            if(cur_lightOn_time >= lightOn_time)
            {
                break;
            }
        }
        PlaySceneManager.Instance.isdark = false;
        M_SPRender.color = new Color(1, 0, 0, 0.6f);
        float light_time = UnityEngine.Random.Range(0.3f, 0.7f);
        yield return new WaitForSeconds(light_time);
        M_SPRender.color = Color.black;
        PlaySceneManager.Instance.isdark = true;
        StartCoroutine(Wait());
    }
   
}
