using UnityEngine;

public class UI_Loading : MonoBehaviour
{
    public static UI_Loading Instance;
    public static UI_Loading Create()
    {
        if (Instance == null)
        {
            UI_Loading m_Loading = Instantiate(Temp_Object.Instance.UI_Loading).GetComponent<UI_Loading>();
            Instance = m_Loading;
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

    [SerializeField] RectTransform RT_GaugeBlank;
    [SerializeField] RectTransform RT_Gauge;

    [SerializeField] Text_Setting T_LoadingState;
    [SerializeField] Text_Setting T_LoadingPercent;

    float LoadingPercent;
    float Cur_LoadingPercent;
    float Width;
    void Setting()
    {

        RT_Gauge.sizeDelta = new Vector2(0, RT_Gauge.sizeDelta.y);

        Width = RT_GaugeBlank.rect.width;

        T_LoadingState.Change_Text("Start");
        T_LoadingPercent.Change_Text("0");
        LoadingPercent = 0;
        Cur_LoadingPercent = 0;
    }

    private void Update()
    {
        if (Cur_LoadingPercent < LoadingPercent)
        {
            Cur_LoadingPercent += Time.deltaTime + 20f;
            if (Cur_LoadingPercent > LoadingPercent)
            {
                Cur_LoadingPercent = LoadingPercent;
            }
            T_LoadingPercent.Change_Text(Cur_LoadingPercent.ToString("F1"));
            RT_Gauge.sizeDelta = new Vector2(Width * 0.01f * Cur_LoadingPercent, RT_Gauge.sizeDelta.y);
        }
    }
    public void Change_State(float loading_percent, string state_text)
    {
        T_LoadingState.Change_Text(state_text);
        LoadingPercent = loading_percent;
    }
    public void Escape()
    {
        Debug.Log("≈ª√‚");
    }
}
