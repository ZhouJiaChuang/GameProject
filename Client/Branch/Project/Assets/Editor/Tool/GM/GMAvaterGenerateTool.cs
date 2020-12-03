using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 单位创建工具
/// </summary>
public class GMAvaterGenerateTool : IEditorTool
{
    public EAvaterType SelectAvaterType = EAvaterType.MainPlayer;
    public EAvaterType LastSelectAvaterType = EAvaterType.MainPlayer;

    public GMAvaterGenerateTool()
    {
        LastSelectAvaterType = SelectAvaterType;
        InitAvaterInfo(SelectAvaterType);
    }

    public void OnGUI()
    {
        OnGUI_SelectAvaterType();

        GUIWindow.DrawSeparator();

        OnGUI_AvaterInfo();
    }

    private void OnGUI_SelectAvaterType()
    {
        GUILayout.BeginHorizontal();
        {
            SelectAvaterType = (EAvaterType)EditorGUILayout.EnumPopup(SelectAvaterType, GUILayout.Width(100));
            if (LastSelectAvaterType != SelectAvaterType)
            {
                LastSelectAvaterType = SelectAvaterType;
                InitAvaterInfo(SelectAvaterType);
            }
            if (SelectAvaterType != EAvaterType.MainPlayer)
            {
                GUILayout.Label("生成数量:", GUILayout.Width(60));
                EditorGUILayout.TextField("", GUILayout.Width(80));

                if (GUILayout.Button("生成", GUILayout.Width(40)))
                {

                }
            }
        }
        GUILayout.EndHorizontal();
    }

    public CSAvaterInfo Info = null;

    private void InitAvaterInfo(EAvaterType type)
    {
        if (type == EAvaterType.MainPlayer)
        {
            Info = new CSMainPlayerInfo(null);
        }
    }

    public void OnGUI_AvaterInfo()
    {
        if(SelectAvaterType == EAvaterType.MainPlayer)
        {
            OnGUI_MainPlayerInfo(Info as CSMainPlayerInfo);
        }
    }

    private void OnGUI_MainPlayerInfo(CSMainPlayerInfo info)
    {
        if (info == null) return;
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("名字:", GUILayout.Width(40));
            info.Name = EditorGUILayout.TextField(info.Name, GUILayout.Width(100));
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("ID:", GUILayout.Width(40));
            info.ID = EditorGUILayout.LongField(info.ID, GUILayout.Width(100));
            if (GUILayout.Button("随机", GUILayout.Width(40)))
            {

            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("等级:", GUILayout.Width(40));
            info.Level = EditorGUILayout.IntField(info.Level, GUILayout.Width(100));
        }
        GUILayout.EndHorizontal();
    }

    public void Save()
    {
    }

    public void Load()
    {
    }
}
