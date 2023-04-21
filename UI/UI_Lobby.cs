using UnityEngine;

public class UI_Lobby : MonoBehaviour
{
    public static UI_Lobby Instance;
    public static UI_Lobby Create()
    {
        if (Instance == null)
        {
            UI_Lobby M_Lobby = Instantiate(Temp_Object.Instance.UI_Lobby).GetComponent<UI_Lobby>();
            Instance = M_Lobby;
        }
        Instance.Setting();
        return Instance;
    }
    public static void Destroy()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }

    void Setting()
    {

    }
    public void Btn_Shop()
    {

    }
    public void Btn_Inven()
    {

    }
    public void Btn_Quest()
    {

    }
    public void Btn_Start()
    {

    }
}
