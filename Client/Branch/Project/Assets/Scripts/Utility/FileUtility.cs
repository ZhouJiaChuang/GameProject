using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class Utility
{
    public static void GetDeepAssetPaths(string path, List<string> list, string extension = "",
            bool isFullPath = false, bool isIncludeMeta = false, bool isOneLevel = false, string containDirPath = "")
    {

        if (Directory.Exists(path))
        {
            if (path.Contains(@".svn")) return;
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = new FileInfo[0];
            if (string.IsNullOrEmpty(extension))
                files = dir.GetFiles();
            else
                files = dir.GetFiles("*" + extension);
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i];
                if (!isIncludeMeta && file.Extension == ".meta") continue;
                string filePath = GetAssetPath(file.FullName, isFullPath);
                //Debug.LogError(filePath);
                if (!string.IsNullOrEmpty(containDirPath))
                {
                    if (!filePath.Contains(containDirPath)) continue;
                }
                if (!list.Contains(filePath))
                    list.Add(filePath);
            }
            if (isOneLevel) return;
            DirectoryInfo[] dirChild = dir.GetDirectories();
            for (int i = 0; i < dirChild.Length; i++)
            {
                GetDeepAssetPaths(dirChild[i].FullName, list, extension, isFullPath, isIncludeMeta, false, containDirPath);
            }
        }

        if (File.Exists(path))
        {
            string filePath = GetAssetPath(path, isFullPath);
            if (!list.Contains(filePath))
                list.Add(filePath);
        }
    }

    /// <summary>
    /// Application.dataPath
    /// </summary>
    /// <param name="fileFullPath"></param>
    /// <returns></returns>
    public static string GetAssetPath(string fileFullPath, bool isFullPath = false)
    {
        //Debug.LogError(fileFullPath);
        if (!isFullPath)
        {
            return "Assets" + fileFullPath.Replace("\\", "/").Replace(Application.dataPath, "");
        }
        else
        {
            return fileFullPath.Replace("\\", "/");
        }
    }

    public static string GetFileName(string path)
    {
        path = path.Replace("\\", "/");
        string dir = GetDirectory(path);
        path = path.Replace(dir + "/", "");
        path = path.Substring(0, path.LastIndexOf("."));
        return path;
    }

    public static string GetDirectory(string path)
    {
        path = path.Replace("\\", "/");
        int lastIndex = path.LastIndexOf("/");
        if (lastIndex == -1) return "";

        path = path.Substring(0, lastIndex);
        return path;
    }

    public static string GetPathWithExtend(string path)
    {
        int index = path.LastIndexOf(".");
        if (index == -1) return path;
        return path.Substring(0, index);
    }

    public static void DetectCreateDirectory(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        int index = path.LastIndexOf(".");
        if (index == -1 || path.Substring(index).Contains("/"))
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        else
        {
            if (!Directory.Exists(path + "/../"))
                Directory.CreateDirectory(path + "/../");
        }
    }

    public static System.Diagnostics.Process Open(string path)
    {
        System.Diagnostics.Process pp = System.Diagnostics.Process.Start(path);
        return pp;
    }
}
