
using UnityEngine;

public class UI_End : MonoBehaviour
{
    public void Btn_Restart()
    {
        PlayManager.Instance.GameRestart();
        Destroy(this.gameObject);
    }
}
