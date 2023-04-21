using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Alarm_Setting))]
public class Edit_Alarm_Setting : Editor
{
    SerializedProperty m_AlarmKind;
    SerializedProperty m_QuestType;
    SerializedProperty m_ItemKind;

    private void OnEnable()
    {
        m_AlarmKind = serializedObject.FindProperty("m_AlarmKind");
        m_QuestType = serializedObject.FindProperty("m_QuestType");
        m_ItemKind = serializedObject.FindProperty("m_ItemKind");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_AlarmKind);
        switch ((E_Alarm)m_AlarmKind.enumValueFlag)
        {
            case E_Alarm.Quest_Completable:
                EditorGUILayout.PropertyField(m_QuestType);
                break;
            case E_Alarm.Inven_Equip_Available:
                EditorGUILayout.PropertyField(m_ItemKind);
                break;
        }
        serializedObject.ApplyModifiedProperties();

    }
}
