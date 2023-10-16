using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    public static Prefabs Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    [Header("UI")]
    [SerializeField] public GameObject UI_Login;
    [SerializeField] public GameObject UI_State;

    [Header("Object")]
    [SerializeField] public GameObject HitRange_Circle;
    [SerializeField] public GameObject HitRange_Rect;
    [SerializeField] public GameObject Bullet;

}
