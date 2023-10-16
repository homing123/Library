using UnityEngine;

public class StreamingData : MonoBehaviour
{
    public static StreamingData Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        //StreamingManager.lt_StrLoad.Add(async () =>
        //{
        //    var data = await StreamingManager.ReadDataAsync<J_SkillData>(StreamingManager.Get_StreamingPath(J_SkillData.Path));
        //    SkillData.Data_DicSet(data);
        //});
    }
}
