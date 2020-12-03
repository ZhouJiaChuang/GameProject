using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// 编辑器下SVN拓展
/// </summary>
public class SVNEditorUtility
{
    public static void SetSVNButton(string path)
    {
        if (GUILayout.Button("更新", GUILayout.Width(50)))
        {
            SVNUpdate(path);
        }
        if (GUILayout.Button("提交", GUILayout.Width(50)))
        {
            SVNCommit(path);
        }
        if (GUILayout.Button("还原", GUILayout.Width(50)))
        {
            SVNRevert(path);
        }
        if (GUILayout.Button("日志", GUILayout.Width(50)))
        {
            SVNLog(path);
        }
    }

    public static void SVNCommit(string path)
    {
        ProcSVNCmd(path, "commit");
    }
    public static void SVNCommit(string Root, List<string> dirs)
    {
        ProcSVNCmd(GetPath(Root, dirs), "commit");
    }

    public static void SVNLog(string path)
    {
        ProcSVNCmd(path, "log");
    }
    public static void SVNLog(string Root, List<string> dirs)
    {
        ProcSVNCmd(GetPath(Root, dirs), "log");
    }

    public static void SVNUpdate(string path)
    {
        ProcSVNCmd(path, "update");
    }

    public static void SVNUpdate(string Root, List<string> dirs, string attachPath = "")
    {
        if (string.IsNullOrEmpty(attachPath))
        {
            ProcSVNCmd(GetPath(Root, dirs), "update");
        }
        else
        {
            ProcSVNCmd(GetPath(Root, dirs) + "*" + attachPath, "update");
        }
    }

    public static void SVNRevert(string path)
    {
        ProcSVNCmd(path, "revert");
    }

    public static void SVNRevert(string Root, List<string> dirs)
    {
        ProcSVNCmd(GetPath(Root, dirs), "revert");
    }

    public static void SVNResolve(string path)
    {
        ProcSVNCmd(path, "resolve");
    }

    public static void SVNResolve(string Root, List<string> dirs)
    {
        ProcSVNCmd(GetPath(Root, dirs), "resolve");
    }

    public static void SVNBlameS(string path)
    {
        ProcSVNCmd(path, "blame");
    }

    public static void SVNBlameS(string Root, List<string> dirs)
    {
        ProcSVNCmd(GetPath(Root, dirs), "blame");
    }

    private static string GetPath(string Root, List<string> dirs)
    {
        string path = "";
        for (int i = 0; i < dirs.Count; i++)
        {
            if (i == 0)
            {
                path += Root + dirs[i];
            }
            else
            {
                path += "*" + Root + dirs[i];
            }
        }
        return path;
    }

    public static void ProcSVNCmd(string path, string cmd)
    {
        if (!string.IsNullOrEmpty(path))
        {
            Process pp = Process.Start("TortoiseProc.exe", @"/command:" + cmd + " /path:" + path + " /closeonend:0");
            pp.WaitForExit();
        }
    }
}