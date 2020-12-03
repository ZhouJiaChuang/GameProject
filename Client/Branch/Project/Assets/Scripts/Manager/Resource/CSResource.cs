using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSResource
{
    private EResourceAssistType mResourceAssistType;
    public EResourceAssistType AssistType
    {
        get { return mResourceAssistType; }
        set { mResourceAssistType = value; }
    }

    private EResourceType mLocalType;
    /// <summary>
    /// 资源类型
    /// </summary>
    public EResourceType LocalType
    {
        get { return mLocalType; }
        set { mLocalType = value; }
    }

    private UnityEngine.Object mirrorObj;
    /// <summary>
    /// AB资源的文件
    /// </summary>
    public UnityEngine.Object MirrorObj
    {
        get { return mirrorObj; }
        set { mirrorObj = value; }
    }
    private byte[] mirroyBytes;
    /// <summary>
    /// 数据资源的数据
    /// </summary>
    public byte[] MirroyBytes
    {
        get { return mirroyBytes; }
        set { mirroyBytes = value; }
    }

    protected string fileName;
    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName
    {
        get { return fileName; }
        set { fileName = value; }
    }

    private string _Path = string.Empty;
    /// <summary>
    /// 文件路径
    /// </summary>
    public string Path
    {
        get { return _Path; }
    }

    public bool _IsDone = false;
    public bool IsDone
    {
        set
        {
            if (_IsDone == value) return;
            _IsDone = value;
            if (value == true)
                loadedTime = Time.time;
        }
        get { return _IsDone; }
    }
    /// <summary>
    /// 加载完成的时间
    /// </summary>
    public float loadedTime = 0;

    public bool _IsHotLoading = false;
    public bool IsHotLoading
    {
        set { _IsHotLoading = value; }
        get { return _IsHotLoading; }
    }

    public object Param;

    public CSEventDelegate<CSResource> onLoaded = new CSEventDelegate<CSResource>();

    public CSResource(string name, string path, EResourceType type)
    {
        fileName = name;
        LocalType = type;
        _Path = path;
    }


    public virtual void Load()
    {

    }

#if (UNITY_4_7 || UNITY_4_6)
    private static string assetbunldeStr = ".assetbundle";
#else
    private static string assetbunldeStr = "";
#endif

    public static string GetPath(string fileName, EResourceType type, bool isLocal)
    {
        string path = "";
        if (isLocal)
        {
            CSStringBuilder.Clear();
            CSStringBuilder.Append(GetTypeRelativePath(type), fileName);
            path = CSStringBuilder.ToString();//Resource.Load(存的是相对路径)
        }
        else
        {
            CSStringBuilder.Clear();

            bool isAssetBundle = IsAssetBundle(type);

            string applicationDataPath = URL.FilePrePath;
            if (CSGameState.RunPlatform == ERunPlatform.Editor)
            {
                string file = "file://";
                path = CSStringBuilder.Append(file, applicationDataPath, "/", GetTypeRelativePath(type), fileName, isAssetBundle ? assetbunldeStr : "").ToString();
            }
            else
            {
                if (CSGame.Instance.ResourceLoadType == EResourceLoadType.StreamAssest)
                {
                    path = CSStringBuilder.Append(applicationDataPath, "/", GetTypeRelativePath(type), fileName, isAssetBundle ? assetbunldeStr : "").ToString();
                }
                else
                {
                    path = CSStringBuilder.Append("file://", applicationDataPath, "/", GetTypeRelativePath(type), fileName, isAssetBundle ? assetbunldeStr : "").ToString();
                }
            }
        }
        return path;
    }

    /// <summary>
    /// 得到该类型资源的相对路径
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetTypeRelativePath(EResourceType type)
    {
        string t = "";
        switch (type)
        {
            case EResourceType.UIEffect: return t = "model/effect/";
            case EResourceType.Body: return t = "model/body/";
            default:
                break;
        }
        return t;
    }

    /// <summary>
    /// 根据资源的类型判定是ab资源还是数据
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsAssetBundle(EResourceType type)
    {
        switch (type)
        {
            case EResourceType.Table:
                return false;
        }
        return true;
    }

    /// <summary>
    /// 得到该资源的对象池个体(如果对象池内没有剩余可使用的,会新增)
    /// </summary>
    /// <param name="removeTime">在获取的同时,设置移除的时间</param>
    /// <returns></returns>
    public CSObjectPoolItem GetPoolItem(EPoolType type = EPoolType.Normal, int MaxLimitCount = 0, 
        float removeTime = 0, EPoolItemRemoveMethod removeMethod = EPoolItemRemoveMethod.None)
    {
        GameObject go = MirrorObj as GameObject;
        if (go == null || CSObjectPoolMgr.Instance == null)
        {
            return null;
        }
        CSObjectPoolItem mPoolItem = CSObjectPoolMgr.Instance.GetAndAddPoolItem_GameObject(go.name, Path, go, false, type);
        if (mPoolItem == null) return null;
        if (removeTime != 0)
        {
            CSObjectPoolMgr.Instance.SetPoolRemoveTime(mPoolItem, removeTime);
        }
        if (removeMethod != EPoolItemRemoveMethod.None && mPoolItem.owner != null)
        {
            mPoolItem.owner.RemoveMethod = removeMethod;
        }
        return mPoolItem;

    }
}