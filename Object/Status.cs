using UnityEngine;

public class Status : MonoBehaviour
{
    public float MaxHP;
    public float HP;
    public float Damage;
    public E_Object Object_Kind;
    public void Init(object data)
    {
        switch (Object_Kind)
        {
            case E_Object.Character:
                Object_Kind = E_Object.Character;
                break;
        }
    }
}
