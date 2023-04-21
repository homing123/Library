using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_Object : MonoBehaviour
{
    public static Temp_Object Instance;
    private void Awake()
    {
        Instance = this;
    }

    public GameObject UI_Loading;
    public GameObject UI_Lobby;
}
