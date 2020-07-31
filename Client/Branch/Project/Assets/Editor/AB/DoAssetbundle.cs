using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;

public class DoAssetbundle
{

    private static string uiAssetBundlesPath
    {
        get
        {
            return "";
            //return CSEditorPath.Get(EEditorPath.LocalResourcesLoadPath) + "Android/";
        }
    }

    /// <summary>
    /// 自动打包所有资源（设置了Assetbundle Name的资源）
    /// </summary>
    [MenuItem("AssetBundle/Create All AssetBundles")]
    public static void CreateAllAssetBundles()
    {
        CreateAllAssetBundles(true);
    }

    [MenuItem("AssetBundle/Create Select AssetBundles")] //设置编辑器菜单选项
    public static void CreateSelectAssetBundles()
    {
        CreateAllAssetBundles(true, true);
    }

    static void SetUIAssetBundleSelect()
    {

        string uiPath = Application.dataPath + "/UIAsset/chart";
        string uiPath1 = Application.dataPath + "/UIAsset/font";
        string uiPath2 = Application.dataPath + "/UIAsset/Prefabs";
        string uiPath3 = Application.dataPath + "/UIAsset/texture";
        string uiPath4 = Application.dataPath + "/UIAsset/material";
        string uiPath5 = Application.dataPath + "/UIAsset/shader";
        List<string> filePathList = new List<string>();
        Utility.GetDeepAssetPaths(uiPath, filePathList, "", false, false, true);
        Utility.GetDeepAssetPaths(uiPath1, filePathList, "", false, false, true);
        Utility.GetDeepAssetPaths(uiPath2, filePathList, "", false, false, true);
        Utility.GetDeepAssetPaths(uiPath3, filePathList, "", false, false, true);
        Utility.GetDeepAssetPaths(uiPath4, filePathList, "", false, false, true);
        Utility.GetDeepAssetPaths(uiPath5, filePathList, "", false, false, true);
        List<string> list = new List<string>();
        foreach (var cur in filePathList)
        {
            if (cur.Contains("/UIAsset/Prefabs") && Selection.activeGameObject != null)
            {
                string fName = Utility.GetFileName(cur);
                if (fName == Selection.activeGameObject.name)
                {
                    list.Add(cur);
                    string[] dependFileList = AssetDatabase.GetDependencies(cur);

                    foreach (var df in dependFileList)
                    {
                        if (filePathList.Contains(df))
                        {
                            if (!list.Contains(df))
                                list.Add(df);
                        }
                    }
                    break;
                }
            }
        }
        List<AssetBundleBuild> abuildList = new List<AssetBundleBuild>();
        foreach (var cur in list)
        {
            string aName = cur.Replace("Assets/UIAsset/", "").ToLower();
            string fileName = Utility.GetFileName(cur);
            aName = Utility.GetPathWithExtend(aName);
            AssetImporter importer = AssetImporter.GetAtPath(cur);
            if (importer != null)
            {
                if (cur.Contains("/UIAsset/Prefabs"))
                {
                    aName = "ui/" + fileName.ToLower();

                }
            }
            AssetBundleBuild buildMap = new AssetBundleBuild();
            buildMap.assetBundleName = aName;
            string[] enemyAssets = new string[1];
            enemyAssets[0] = cur;
            buildMap.assetNames = enemyAssets;
            abuildList.Add(buildMap);
        }
        string outPath = uiAssetBundlesPath + "Select/";
        Utility.DetectCreateDirectory(outPath);
        EditorUtility.DisplayProgressBar("正在启动选中资源打Assetbundle逻辑，请稍等", "正在启动打Assetbundle逻辑，请稍等...", 1);
        BuildPipeline.BuildAssetBundles(outPath, abuildList.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        EditorUtility.ClearProgressBar();
        Utility.Open(outPath);
    }
    public static void CreateAllAssetBundles(bool isDisplayDialog, bool isSelect = false)
    {
        if (!isSelect)
        {
            SetMainAssetBundleName();
            EditorUtility.DisplayProgressBar("正在启动打Assetbundle逻辑，请稍等", "正在启动打Assetbundle逻辑，请稍等...", 1);
            Caching.ClearCache();
            string path = uiAssetBundlesPath + (isSelect ? "Select/" : "");
            Utility.DetectCreateDirectory(path);
            //打包资源
            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            EditorUtility.ClearProgressBar();
            

            if (isDisplayDialog)
                EditorUtility.DisplayDialog("完成", "完成", "确定");
        }
        else
        {
            SetUIAssetBundleSelect();
        }
    }

    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }

    /// <summary>
    /// 将某一文件夹中的资源进行分离打包，即把依赖资源分离出来打包
    /// </summary>
    [MenuItem("AssetBundle/Set Main AssetbundleName And Clear Other")]
    public static void SetMainAssetBundleName()
    {
        ClearAssetBundlesName();
        SetUIAssetBundleName();
    }

    /// <summary>
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包 
    /// 因为只要设置了AssetBundleName的，都会进行打包，不论在什么目录下 
    /// </summary> 
    public static void ClearAssetBundlesName()
    {
        List<string> fileList = new List<string>();
        Utility.GetDeepAssetPaths(Application.dataPath, fileList);
        int index = 0;
        foreach (var path in fileList)
        {
            if (path.Contains("/UIAsset/Prefabs/")) continue;
            if (path.Contains("/UIAsset/texture/")) continue;
            if (path.Contains("/UIAsset/font/")) continue;
            if (path.Contains("/UIAsset/chart/")) continue;
            if (path.Contains("/UIAsset/material/")) continue;
            if (path.Contains("/UIAsset/shader/")) continue;
            index++;
            if (index % 100 == 0)
            {
                EditorUtility.DisplayProgressBar("ClearSelectAssetBundleName", index + "/" + fileList.Count, index * 1f / fileList.Count);
            }
            var importer = AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                if (importer)
                {
                    if (!string.IsNullOrEmpty(importer.assetBundleName))
                    {
                        importer.assetBundleName = "";
                    }

                    if (!string.IsNullOrEmpty(importer.assetBundleVariant))
                    {
                        importer.assetBundleVariant = "";
                    }
                }
            }
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void DeleteNoUseUIRes(List<string> filePathList)
    {

        EditorUtility.DisplayProgressBar("删除无用资源", "删除无用资源中...", 0f);   //显示进程加载条
        List<string> useList = new List<string>();
        foreach (var cur in filePathList)
        {
            string aName = cur.Replace("Assets/UIAsset/", "").Replace("Prefabs/", "ui/").ToLower();
            aName = Utility.GetPathWithExtend(aName);
            string path = uiAssetBundlesPath + aName;
            useList.Add(path.ToLower());
        }

        List<string> allList = new List<string>();
        Utility.GetDeepAssetPaths(uiAssetBundlesPath, allList, "", true);

        int index = 0;
        int maxCount = allList.Count;
        foreach (var fullPath in allList)
        {
            if (fullPath.EndsWith(".manifest") || fullPath.EndsWith("/Android/Android")) continue;
            index++;
            EditorUtility.DisplayProgressBar(fullPath, "删除无用资源(" + index + "/" + maxCount + ")",
                index * 1f / maxCount);   //显示进程加载条
            if (!useList.Contains(fullPath.ToLower()))
            {
                File.Delete(fullPath);
                string manifestPath = fullPath + ".manifest";
                if (File.Exists(manifestPath))
                    File.Delete(manifestPath);
            }

        }

    }

    static void SetUIAssetBundleName()
    {
        string uiPath = Application.dataPath + "/UIAsset/chart";
        string uiPath1 = Application.dataPath + "/UIAsset/font";
        string uiPath2 = Application.dataPath + "/UIAsset/Prefabs";
        string uiPath3 = Application.dataPath + "/UIAsset/texture";
        string uiPath4 = Application.dataPath + "/UIAsset/material";
        string uiPath5 = Application.dataPath + "/UIAsset/shader";
        List<string> filePathList = new List<string>();
        Utility.GetDeepAssetPaths(uiPath, filePathList, "", false, false, true);
        Utility.GetDeepAssetPaths(uiPath1, filePathList, "", false, false, true);
        Utility.GetDeepAssetPaths(uiPath2, filePathList, "", false, false, true);
        Utility.GetDeepAssetPaths(uiPath3, filePathList, "", false, false, true);
        Utility.GetDeepAssetPaths(uiPath4, filePathList, "", false, false, true);
        Utility.GetDeepAssetPaths(uiPath5, filePathList, "", false, false, true);
        DeleteNoUseUIRes(filePathList);
        EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 0f);   //显示进程加载条
        int index = 0;
        int maxCount = filePathList.Count;
        foreach (var cur in filePathList)
        {
            index++;
            //if (index%100==0)
            //{
            EditorUtility.DisplayProgressBar(cur, "正在设置AssetName名称中(" + index + "/" + maxCount + ")",
                index * 1f / maxCount);   //显示进程加载条
                                          //}
            string aName = cur.Replace("Assets/UIAsset/", "").ToLower();
            string fileName = Utility.GetFileName(cur);
            aName = Utility.GetPathWithExtend(aName);
            AssetImporter importer = AssetImporter.GetAtPath(cur);
            if (importer != null)
            {
                if (cur.Contains("/UIAsset/Prefabs"))
                {
                    if (!cur.Contains("UILoading.prefab") && !cur.Contains("UIDownloading.prefab") && !cur.Contains("UIWaiting.prefab"))//还是C#处理的界面
                    {
                        GameObject go = AssetDatabase.LoadAssetAtPath(cur, typeof(GameObject)) as GameObject;
                        if (go != null && go.activeSelf)
                        {
                            go.SetActive(false);
                        }
                    }
                    else
                    {
                        GameObject go = AssetDatabase.LoadAssetAtPath(cur, typeof(GameObject)) as GameObject;
                        if (go != null && !go.activeSelf)
                        {
                            go.SetActive(true);
                        }
                    }
                    if (importer.assetBundleName != "ui/" + fileName.ToLower())
                    {
                        importer.assetBundleName = "ui/" + fileName.ToLower();
                    }
                }
                else
                {
                    if (importer.assetBundleName != aName)
                    {
                        importer.assetBundleName = aName;
                    }
                }
            }

            if (cur.Contains("/UIAsset/Prefabs"))
            {
                GameObject go = AssetDatabase.LoadAssetAtPath(cur, typeof(GameObject)) as GameObject;
                if (go != null)
                {
                    Transform[] trans = go.GetComponentsInChildren<Transform>(true);
                    foreach (var tran in trans)
                    {
                        MonoBehaviour[] mbs = tran.gameObject.GetComponents<MonoBehaviour>();
                        foreach (var mono in mbs)
                        {
                            if (mono == null)
                            {
                                continue;
                            }
                            if (!mono.GetType().ToString().StartsWith("Top_") && mono.GetType().ToString() != "LuaStateBehaviour" &&
                                mono.GetType().ToString() != "LuaUpdateBehaviour")
                            {
                                string topName = "Top_" + mono.GetType().ToString();
                            }
                        }
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
    }

}