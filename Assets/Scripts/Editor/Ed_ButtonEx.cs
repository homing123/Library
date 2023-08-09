using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
[CustomEditor(typeof(ButtonEx))]
public class Ed_ButtonEx : Editor
{
    [Header("Button Setting")]
    ButtonEx m_Target;

    SerializedProperty m_AudioClip;

    SerializedProperty m_Interactable;
    SerializedProperty m_Transition;
    SerializedProperty m_TargetGraphic;
    //colors
    SerializedProperty m_Colors;

    SerializedProperty m_OnClick;


    private void OnEnable()
    {
        m_AudioClip = serializedObject.FindProperty("m_AudioClip");
        m_Interactable = serializedObject.FindProperty("m_Interactable");
        m_Transition = serializedObject.FindProperty("m_Transition");
        m_TargetGraphic = serializedObject.FindProperty("m_TargetGraphic");
        m_Colors = serializedObject.FindProperty("m_Colors");
        m_OnClick = serializedObject.FindProperty("m_OnClick");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_AudioClip);
        EditorGUILayout.PropertyField(m_Interactable);
        EditorGUILayout.PropertyField(m_Transition);
        switch ((Selectable.Transition)m_Transition.enumValueFlag)
        {
            case Selectable.Transition.None:
                break;
            case Selectable.Transition.ColorTint:
                EditorGUILayout.PropertyField(m_TargetGraphic);
                EditorGUILayout.PropertyField(m_Colors);
                break;

            case Selectable.Transition.SpriteSwap:
                break;

            case Selectable.Transition.Animation:
                break;
        }

        EditorGUILayout.PropertyField(m_OnClick);
        serializedObject.ApplyModifiedProperties();
    }

}
