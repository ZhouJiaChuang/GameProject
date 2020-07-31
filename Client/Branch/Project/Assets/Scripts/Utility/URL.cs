using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URL
{
    private static string mClientResURL;
    /// <summary>
    /// 资源读取路径
    /// 在window上就是file:///C:/Users/admin/AppData/LocalLow/公司/项目名字/
    /// 在Android上就是Android/data/data/包名/files/
    /// </summary>
    public static string ClientResURL
    {
        get
        {
            if (string.IsNullOrEmpty(mClientResURL))
            {
                if (CSGameState.RunPlatform == ERunPlatform.Editor)
                {
                    if(CSGame.Instance.ResourceLoadType == EResourceLoadType.Local)
                    {
                        mClientResURL = CSEditorPath.Get(EEditorPath.LocalResourcesLoadPath);
                    }
                    else
                    {
                        mClientResURL = "file:///" + Application.persistentDataPath + "/";
                    }
                }
                else
                {
                    mClientResURL = "file://" + Application.persistentDataPath + "/";
                }
            }
            return mClientResURL;
        }
    }

    private static string _LocalProjectPath;
    /// <summary>
    /// 工程路径
    /// </summary>
    public static string LocalProjectPath
    {
        get
        {
            if (string.IsNullOrEmpty(_LocalProjectPath))
            {
                _LocalProjectPath = Application.dataPath.Replace("/Assets", "");
            }
            return _LocalProjectPath;
        }
    }

    /// <summary>
    /// 项目名字
    /// </summary>
    private static string _LocalProjectName = string.Empty;
    public static string LocalProjectName
    {
        get
        {
            if (string.IsNullOrEmpty(_LocalProjectName))
            {
                _LocalProjectName = Application.dataPath.Replace("/Assets", "");
                _LocalProjectName = _LocalProjectName.Substring(_LocalProjectName.LastIndexOf("/") + 1);
            }
            return _LocalProjectName;
        }
    }

    private static string filePath;
    /// <summary>
    /// 资源加载路径
    /// </summary>
    public static string FilePrePath
    {
        get
        {
            if (string.IsNullOrEmpty(filePath))
            {
                EResourceLoadType loadType = CSGame.Instance.ResourceLoadType;
                if (loadType == EResourceLoadType.StreamAssest)
                {
                    filePath = Application.streamingAssetsPath;
                }
                else if (loadType == EResourceLoadType.Local)
                {
                    filePath = CSEditorPath.Get(EEditorPath.LocalResourcesLoadPath);
                }
                else if (loadType == EResourceLoadType.Project)
                {
                    filePath = string.Empty;
                }
                else
                {
                    filePath = Application.persistentDataPath;
                }
            }

            return filePath;
        }
    }
}
