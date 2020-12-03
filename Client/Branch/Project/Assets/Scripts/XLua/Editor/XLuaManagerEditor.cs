using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CSGame))]
public class XLuaManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Gen"))
        {
            if (EditorApplication.isCompiling)
            {
                UnityEngine.Debug.LogError("请先等待代码同步完成~~~");
                return;
            }
            XLuaConfig.Clear();
            GenSimply(EGenType.Normal);
            CSObjectWrapEditor.Generator.GenAll();
        }

        if (GUILayout.Button("SimplyGen"))
        {
            XLuaConfig.Clear();
            GenSimply(EGenType.Simply);
            CSObjectWrapEditor.Generator.GenAll();
        }

        if (GUILayout.Button("Clear"))
        {
            CSObjectWrapEditor.Generator.ClearAll();
        }


        if (GUILayout.Button("Hotfix Inject In Editor"))
        {
            //在Spriting Define Symbols 中加 HOTFIX_ENABLE
            //XLua.Hotfix.HotfixInject();
        }
    }

    public enum EGenType
    {
        Normal,
        Simply,
        SimplyAndIsGenericType,
    }
    public static void GenSimply(EGenType type)
    {
        string path = "";
        string content = "";
        switch (type)
        {
            //case EGenType.Normal:
            //    path = XLuaConfig.hotSaveTxtPath;
            //    content = "";
            //    FileUtilityEditor.Write(path, content, false);
            //    break;
            //case EGenType.Simply:
            //    path = XLuaConfig.hotSaveTxtPath;
            //    content = "XLuaTest";
            //    FileUtilityEditor.Write(path, content, false);
            //    break;
            //case EGenType.SimplyAndIsGenericType:
            //    path = XLuaConfig.hotSaveTxtPath;
            //    content = "SimplyAndIsGenericType";
            //    FileUtilityEditor.Write(path, content, false);
            //    break;
        }
    }
}
