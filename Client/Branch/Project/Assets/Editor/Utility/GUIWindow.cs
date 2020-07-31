using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GUIWindow
{
    #region GUIStype
    private static float LineHeight = 25;
    private static float InputHeight = 18;
    private static int FontSize = 12;

    private static GUIStyle _BtnStyle;
    public static GUIStyle BtnStyle
    {
        get
        {
            if (_BtnStyle == null)
            {
                _BtnStyle = new GUIStyle(GUI.skin.button);
                _BtnStyle.fixedWidth = 100;
                _BtnStyle.fixedHeight = LineHeight;
                _BtnStyle.fontSize = FontSize;
                _BtnStyle.alignment = TextAnchor.MiddleCenter;
            }
            return _BtnStyle;
        }
    }
    private static GUIStyle _BtnCustomWidthStyle;
    public static GUIStyle BtnCustomWidthStyle(int width)
    {
        if (_BtnCustomWidthStyle == null)
        {
            _BtnCustomWidthStyle = new GUIStyle(GUI.skin.button);
            _BtnCustomWidthStyle.fixedHeight = LineHeight;
            _BtnCustomWidthStyle.fontSize = FontSize;
            _BtnCustomWidthStyle.alignment = TextAnchor.MiddleCenter;
        }
        _BtnCustomWidthStyle.fixedWidth = width;
        return _BtnStyle;
    }

    private static GUIStyle _BtnShortNormalStyle;
    public static GUIStyle BtnShortNormalStyle
    {
        get
        {
            if (_BtnShortNormalStyle == null)
            {
                _BtnShortNormalStyle = new GUIStyle(GUI.skin.button);
                _BtnShortNormalStyle.fixedWidth = 30;
                _BtnShortNormalStyle.fixedHeight = LineHeight;
                _BtnShortNormalStyle.fontSize = FontSize;
                _BtnShortNormalStyle.alignment = TextAnchor.MiddleCenter;
            }
            return _BtnShortNormalStyle;
        }
    }

    private static GUIStyle _BtnShortSelectedStyle;
    public static GUIStyle BtnShortSelectedStyle
    {
        get
        {
            if (_BtnShortSelectedStyle == null)
            {
                _BtnShortSelectedStyle = new GUIStyle(GUI.skin.button);
                _BtnShortSelectedStyle.fixedWidth = 30;
                _BtnShortSelectedStyle.fixedHeight = LineHeight;
                _BtnShortSelectedStyle.fontSize = FontSize;
                _BtnShortSelectedStyle.alignment = TextAnchor.MiddleCenter;
                _BtnShortSelectedStyle.normal.textColor = Color.red;
            }
            return _BtnShortSelectedStyle;
        }
    }

    private static GUIStyle _TextFieldShortStyle;
    public static GUIStyle TextFieldShortStyle
    {
        get
        {
            if (_TextFieldShortStyle == null)
            {
                _TextFieldShortStyle = new GUIStyle(GUI.skin.textField);
                _TextFieldShortStyle.fixedWidth = 30;
                _TextFieldShortStyle.fixedHeight = InputHeight;
                _TextFieldShortStyle.margin.top = (int)((LineHeight - InputHeight) / 2) + 4;
                _TextFieldShortStyle.alignment = TextAnchor.MiddleLeft;
                _TextFieldShortStyle.fontSize = FontSize;
            }
            return _TextFieldShortStyle;
        }
    }
    private static GUIStyle _TextFieldNormalStyle;
    public static GUIStyle TextFieldNormalStyle
    {
        get
        {
            if (_TextFieldNormalStyle == null)
            {
                _TextFieldNormalStyle = new GUIStyle(GUI.skin.textField);
                _TextFieldNormalStyle.fixedWidth = 60;
                _TextFieldNormalStyle.fixedHeight = InputHeight;
                _TextFieldNormalStyle.margin.top = (int)((LineHeight - InputHeight) / 2) + 4;
                _TextFieldNormalStyle.alignment = TextAnchor.MiddleLeft;
                _TextFieldNormalStyle.fontSize = FontSize;
            }
            return _TextFieldNormalStyle;
        }
    }
    private static GUIStyle _TextFieldLongStyle;
    public static GUIStyle TextFieldLongStyle
    {
        get
        {
            if (_TextFieldLongStyle == null)
            {
                _TextFieldLongStyle = new GUIStyle(GUI.skin.textField);
                _TextFieldLongStyle.fixedWidth = 200;
                _TextFieldLongStyle.fixedHeight = InputHeight;
                _TextFieldLongStyle.margin.top = (int)((LineHeight - InputHeight) / 2) + 4;
                _TextFieldLongStyle.alignment = TextAnchor.MiddleLeft;
                _TextFieldLongStyle.fontSize = FontSize;
            }
            return _TextFieldLongStyle;
        }
    }

    private static GUIStyle _LabelShortStyle;
    public static GUIStyle LabelShortStyle
    {
        get
        {
            if (_LabelShortStyle == null)
            {
                _LabelShortStyle = new GUIStyle(GUI.skin.label);
                _LabelShortStyle.fixedWidth = 30;
                _LabelShortStyle.fixedHeight = LineHeight;
                _LabelShortStyle.alignment = TextAnchor.MiddleLeft;
                _LabelShortStyle.fontSize = FontSize;
            }
            return _LabelShortStyle;
        }
    }

    private static GUIStyle _LabelNormalStyle;
    public static GUIStyle LabelNormalStyle
    {
        get
        {
            if (_LabelNormalStyle == null)
            {
                _LabelNormalStyle = new GUIStyle(GUI.skin.label);
                _LabelNormalStyle.fixedWidth = 90;
                _LabelNormalStyle.fixedHeight = LineHeight;
                _LabelNormalStyle.alignment = TextAnchor.MiddleLeft;
                _LabelNormalStyle.fontSize = FontSize;
            }
            return _LabelNormalStyle;
        }
    }

    private static GUIStyle _LabelLongStyle;
    public static GUIStyle LabelLongStyle
    {
        get
        {
            if (_LabelLongStyle == null)
            {
                _LabelLongStyle = new GUIStyle(GUI.skin.label);
                _LabelLongStyle.fixedWidth = 180;
                _LabelLongStyle.fixedHeight = LineHeight;
                _LabelLongStyle.alignment = TextAnchor.MiddleLeft;
                _LabelLongStyle.fontSize = FontSize;
            }
            return _LabelLongStyle;
        }
    }
    #endregion


    #region textField 文字输入,实现copy
    /// <summary> 
    /// TextField复制粘贴的实现
    /// </summary> 
    public static string TextField(string value, params GUILayoutOption[] options)
    {
        int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(),
            FocusType.Keyboard) + 1;
        if (textFieldID == 0)
            return value;
        //处理复制粘贴的操作 
        value = HandleCopyPaste(textFieldID) ?? value; return EditorGUILayout.TextField(value, options);
    }

    public static string TextField(string value, GUIStyle style, params GUILayoutOption[] options)
    {
        int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(),
            FocusType.Keyboard) + 1;
        if (textFieldID == 0)
            return value;
        //处理复制粘贴的操作 
        value = HandleCopyPaste(textFieldID) ?? value; return EditorGUILayout.TextField(value, style, options);
    }

    public static string DelayedTextField(string label, string value, params GUILayoutOption[] options)
    {
        int textFieldID = GUIUtility.GetControlID("DelayedTextField".GetHashCode(),
            FocusType.Keyboard) + 1;
        if (textFieldID == 0)
            return value;
        //处理复制粘贴的操作 
        value = HandleCopyPaste(textFieldID) ?? value; return EditorGUILayout.DelayedTextField(label, value, options);
    }

    public static string DelayedTextField(string label, string value, GUIStyle style, params GUILayoutOption[] options)
    {
        int textFieldID = GUIUtility.GetControlID("DelayedTextField".GetHashCode(),
            FocusType.Keyboard) + 1;
        if (textFieldID == 0)
            return value;
        //处理复制粘贴的操作 
        value = HandleCopyPaste(textFieldID) ?? value; return EditorGUILayout.DelayedTextField(label, value, style, options);
    }

    public static string HandleCopyPaste(int controlID)
    {
        if (controlID == GUIUtility.keyboardControl)
        {
            if (Event.current.type == UnityEngine.EventType.KeyUp && (Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command))
            {
                if (Event.current.keyCode == KeyCode.C) { Event.current.Use(); TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl); editor.Copy(); }
                else if (Event.current.keyCode == KeyCode.V)
                {
                    Event.current.Use(); TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl); editor.Paste();
#if UNITY_5_3_OR_NEWER || UNITY_5_3
                    return editor.text;                    //以及更高的unity版本中editor.content.text已经被废弃，需使用editor.text代替
#else
    return editor.content.text;
#endif
                }
                else if (Event.current.keyCode == KeyCode.A)
                {
                    Event.current.Use();
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl); editor.SelectAll();
                }
            }
        }
        return null;
    }
    #endregion

    static public Texture2D blankTexture
    {
        get
        {
            return EditorGUIUtility.whiteTexture;
        }
    }


    static public bool minimalisticLook
    {
        get { return GetBool("NGUI Minimalistic", false); }
        set { SetBool("NGUI Minimalistic", value); }
    }

    static public void SetBool(string name, bool val) { EditorPrefs.SetBool(name, val); }

    /// <summary>
    /// Get the previously saved boolean value.
    /// </summary>
    static public bool GetBool(string name, bool defaultValue) { return EditorPrefs.GetBool(name, defaultValue); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text) { return DrawHeader(text, text, false, minimalisticLook); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false, minimalisticLook); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, bool detailed) { return DrawHeader(text, text, detailed, !detailed); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (!minimalistic) GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUI.changed = false;

        if (minimalistic)
        {
            if (state) text = "\u25BC" + (char)0x200a + text;
            else text = "\u25BA" + (char)0x200a + text;

            GUILayout.BeginHorizontal();
            GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
            if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
        else
        {
            text = "<b><size=11>" + text + "</size></b>";
            if (state) text = "\u25BC " + text;
            else text = "\u25BA " + text;
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
        }

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        if (!minimalistic) GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }

    /// <summary>
    /// Begin drawing the content area.
    /// </summary>

    static public void BeginContents() { BeginContents(minimalisticLook); }

    static bool mEndHorizontal = false;

    static public void BeginContents(bool minimalistic)
    {
        if (!minimalistic)
        {
            mEndHorizontal = true;
            GUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        }
        else
        {
            mEndHorizontal = false;
            EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
            GUILayout.Space(10f);
        }
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    /// <summary>
    /// End drawing the content area.
    /// </summary>

    static public void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (mEndHorizontal)
        {
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(3f);
    }

    /// <summary>
    /// Draw a visible separator in addition to adding some padding.
    /// </summary>

    static public void DrawSeparator()
    {
        GUILayout.Space(12f);

        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = blankTexture;
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = new Color(0f, 0f, 0f, 0.25f);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
            GUI.color = Color.white;
        }
    }

}