using UnityEngine;

public class UI_Start : MonoBehaviour
{
    public void Btn_Start()
    {
        PlayManager.Instance.WaitStart();
        Destroy(this.gameObject);
    }
}
