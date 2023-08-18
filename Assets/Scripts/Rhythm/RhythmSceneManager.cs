using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RhythmSceneManager : MonoBehaviour
{
    public static RhythmSceneManager Instance;
    public static event EventHandler ev_GameStart;
    public static event EventHandler ev_GameEnd;
    [SerializeField] GameObject pre_bar;

    [SerializeField] GameObject Pre_TestCube;
    List<GameObject> L_Cube = new List<GameObject>();
    private void Awake()
    {
        Instance = this;
    }
    int[] value = new int[200];
    private void Start()
    {
        System.Random rnd = new System.Random();
        float m = 0;
        for (int i = 0; i < 10000; i++)
        {
            double v1, v2, s;
            do
            {
                v1 = rnd.Next(-500000, 500001) / 500000.0;
                v2 = rnd.Next(-500000, 500001) / 500000.0;
                s = v1 * v1 + v2 * v2;
            }
            while (s >= 1 || s == 0);
            s = Math.Sqrt((-2.0 * Math.Log(s)) / s);
            int rand_value = (int)((s + 100));
            if (rand_value < 200 && rand_value >= 0) 
            {
                value[rand_value]++;
            }
            else
            {
                Debug.Log(rand_value);
            }
        }
        //for(int i = 0; i < 100000; i++)
        //{
        //    value[(int)(UnityEngine.Random.insideUnitCircle.x * 100 + 100)]++;
        //}
        for(int i = 0; i < 200; i++)
        {
            L_Cube.Add(Instantiate(Pre_TestCube, new Vector3(i, value[i], 0), Quaternion.identity));
        }

        StartCoroutine(Wait());
    }
    private void Update()
    {

            
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
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
