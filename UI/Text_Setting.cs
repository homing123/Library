using UnityEngine;
using UnityEngine.UI;
using static Define;

public class Text_Setting : TMPro.TextMeshPro
{
    [SerializeField] int m_Lang_ID;
    [SerializeField] Text m_Text;
    [SerializeField] E_Text_Kind m_Cur_TextKind;

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnEnable();
        Debug.Log("b" + this.transform.name);
    }

}
    
