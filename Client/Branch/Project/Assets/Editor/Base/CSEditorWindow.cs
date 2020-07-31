using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class CSEditorWindow : EditorWindow
{
    private List<string> ToolbarTitleList;

    public string[] _ToolbarTitles = null;
    public virtual string[] ToolbarTitles
    {
        set { _ToolbarTitles = value; }
        get
        {
            if (_ToolbarTitles == null || ToolbarTitleList == null)
            {
                _ToolbarTitles = new string[0];

                InitToolDic();
            }
            return _ToolbarTitles;
        }
    }

    private int _ToolbarIndex = 0;
    public virtual int ToolbarIndex { set { _ToolbarIndex = value; } get { return _ToolbarIndex; } }
    
    public List<IEditorTool> testList = new List<IEditorTool>();
    public Dictionary<int, List<IEditorTool>> BarToolsDic = new Dictionary<int, List<IEditorTool>>() { };
    protected virtual void OnGUI()
    {
        GUIWindow.DrawSeparator();

        if (ToolbarTitles.Length > 0 && BarToolsDic.Count == 0)
        {
            OnInit();
        }

        if (ToolbarTitles.Length > 1)
        {
            GUILayout.BeginHorizontal();
            {
                ToolbarIndex = GUILayout.Toolbar(ToolbarIndex, ToolbarTitles);
            }
            GUILayout.EndHorizontal();

            GUIWindow.DrawSeparator();
        }


        foreach (var bar in BarToolsDic)
        {
            if (bar.Key != ToolbarIndex) continue;
            for(int i = 0; i < bar.Value.Count; i++)
            {
                bar.Value[i].OnGUI();
            }
        }
        
    }

    public void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnInit()
    {
        ToolbarTitles = null;
        if (ToolbarTitleList != null) ToolbarTitleList.Clear();
        InitToolDic();
    }

    protected abstract void InitToolDic();

    protected virtual void AddTool(IEditorTool tool)
    {
        AddToolBar("Default", tool);
    }

    protected virtual void AddToolBar(string barName, IEditorTool tool)
    {
        if (ToolbarTitleList == null) ToolbarTitleList = new List<string>();

        if (!ToolbarTitleList.Contains(barName))
            ToolbarTitleList.Add(barName);
        ToolbarTitles = ToolbarTitleList.ToArray();
        int key = ToolbarTitleList.Count - 1;

        if (!BarToolsDic.ContainsKey(key))
            BarToolsDic.Add(key, new List<IEditorTool>());

        BarToolsDic[key].Add(tool);
        testList.Add(tool);
    }
}