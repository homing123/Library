using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextEx))]
public class Ed_TextEx : Editor
{
    [Header("Text Setting")]
    TextEx m_Target;

    SerializedProperty m_CurType;
    SerializedProperty m_Language_ID;
    SerializedProperty m_Text;

    SerializedProperty m_FontSize;
    SerializedProperty m_Autosizing;
    SerializedProperty m_FontSizeMin;
    SerializedProperty m_FontSizeMax;
    SerializedProperty m_FontSizeBase;

    SerializedProperty m_FontStyle;
    SerializedProperty m_FontColor;
    SerializedProperty m_HorizontalAlignment;
    SerializedProperty m_VerticalAlignment;


    static readonly GUIContent k_MinLabel = new GUIContent("Min", "The minimum font size.");
    static readonly GUIContent k_MaxLabel = new GUIContent("Max", "The maximum font size.");


    private void OnEnable()
    {
        m_CurType = serializedObject.FindProperty("m_CurType");
        m_Language_ID = serializedObject.FindProperty("m_LanguageID");
        m_Text = serializedObject.FindProperty("m_text");

        m_FontSize = serializedObject.FindProperty("m_fontSize");
        m_Autosizing = serializedObject.FindProperty("m_enableAutoSizing");
        m_FontSizeMin = serializedObject.FindProperty("m_fontSizeMin");
        m_FontSizeMax = serializedObject.FindProperty("m_fontSizeMax");
        m_FontSizeBase = serializedObject.FindProperty("m_fontSizeBase");

        m_FontStyle = serializedObject.FindProperty("m_fontStyle");
        m_FontColor = serializedObject.FindProperty("m_fontColor");

        m_HorizontalAlignment = serializedObject.FindProperty("m_HorizontalAlignment");
        m_VerticalAlignment = serializedObject.FindProperty("m_VerticalAlignment");

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_CurType);
        switch ((TextEx.E_TextType)m_CurType.enumValueFlag)
        {
            case TextEx.E_TextType.Text:
                EditorGUILayout.PropertyField(m_Text);
                break;
            case TextEx.E_TextType.Language:
                EditorGUILayout.PropertyField(m_Language_ID);
                break;
        }


        //font size
        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginDisabledGroup(m_Autosizing.boolValue);
        EditorGUILayout.PropertyField(m_FontSize, GUILayout.MaxWidth(EditorGUIUtility.labelWidth + 50f));
        EditorGUI.EndDisabledGroup();
        if (EditorGUI.EndChangeCheck())
        {
            float fontSize = Mathf.Clamp(m_FontSize.floatValue, 0, 32767);
            m_FontSize.floatValue = fontSize;
            m_FontSizeBase.floatValue = fontSize;
        }

        //autosizing
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(m_Autosizing);
        if (EditorGUI.EndChangeCheck())
        {
            if (m_Autosizing.boolValue == false)
                m_FontSize.floatValue = m_FontSizeBase.floatValue;
        }

        if (m_Autosizing.boolValue)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            rect.width = (rect.width - EditorGUIUtility.labelWidth) / 4f;
            rect.x += EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 24;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, m_FontSizeMin, k_MinLabel);
            if (EditorGUI.EndChangeCheck())
            {
                float minSize = Mathf.Clamp(m_FontSizeMin.floatValue, 0, 32767);
                m_FontSizeMin.floatValue = minSize;
                float fontsize = minSize > m_FontSize.floatValue ? minSize : m_FontSize.floatValue;
                m_FontSize.floatValue = fontsize;
                m_FontSizeBase.floatValue = fontsize;
            }
            rect.x += rect.width;

            EditorGUIUtility.labelWidth = 27;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(rect, m_FontSizeMax, k_MaxLabel);
            if (EditorGUI.EndChangeCheck())
            {
                float maxSize = Mathf.Clamp(m_FontSizeMax.floatValue, 0, 32767);
                m_FontSizeMax.floatValue = maxSize;

                float fontsize = maxSize < m_FontSize.floatValue ? maxSize : m_FontSize.floatValue;
                m_FontSize.floatValue = fontsize;
                m_FontSizeBase.floatValue = fontsize;
            }
        }

        EditorGUILayout.PropertyField(m_FontStyle);
        EditorGUILayout.PropertyField(m_FontColor);
        EditorGUILayout.PropertyField(m_HorizontalAlignment);
        EditorGUILayout.PropertyField(m_VerticalAlignment);

        serializedObject.ApplyModifiedProperties();
    }
}
