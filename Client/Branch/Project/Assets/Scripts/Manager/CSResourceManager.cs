/***
 * Author --- ZJC
 * Description --- 
 * Function:
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源管理器
/// </summary>
public class CSResourceManager : SingletonMono<CSResourceManager>
{

    private Dictionary<string, CSResource> waitingQueueDic = new Dictionary<string, CSResource>();//等待加载de列表
    private CSBetterList<CSResource> waitingWWWQueue = new CSBetterList<CSResource>();//正在等待加载的WWW资源

    private Dictionary<string, CSResource> loadingQueue = new Dictionary<string, CSResource>();//正在下载的WWW资源
    private CSBetterList<CSResource> loadingQueueList = new CSBetterList<CSResource>();

    private Dictionary<string, CSResource> loadedQueue = new Dictionary<string, CSResource>();//下载完成的资源，包括WWW和Resources

    /// <summary>
    /// 最大同时加载的数量限制
    /// </summary>
    public int maxSyncLoadingNum
    {
        get { return 10; }
    }

    void Update()
    {
        LoadWWWLine();
    }

    /// <summary>
    /// 在Update中检测资源的更新
    /// </summary>
    private void LoadWWWLine()
    {
        ///正在等待加载的资源列表
        if (waitingWWWQueue.Count == 0)
        {
            return;
        }
        for (int i = 0; i < waitingWWWQueue.Count; i++)
        {
            CSResource res = waitingWWWQueue[i];
            if (res != null)
            {
                if (res.AssistType == EResourceAssistType.ForceLoad)
                {
                    waitingWWWQueue.RemoveAt(i);
                    RemoveWaitingQueueDic(res);
                    AddLoadingQueue(res);
                    res.Load();
                    break;
                }
                else
                {
                    if (loadingQueue.Count > maxSyncLoadingNum)
                    {
                        return;
                    }
                    waitingWWWQueue.RemoveAt(i);
                    RemoveWaitingQueueDic(res);
                    res.Load();
                    break;
                }
            }
            else
            {
                waitingWWWQueue.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// 加载资源到队列中
    /// </summary>
    /// <param name="name">资源名字</param>
    /// <param name="type">资源类型</param>
    /// <param name="onLoadCallBack">加载完成后回调</param>
    /// <param name="assistType">资源加载优先级</param>
    /// <param name="isPath">是否为全路径</param>
    /// <returns></returns>
    public CSResource AddQueue(string name, EResourceType type, CSEventDelegate<CSResource>.OnLoaded onLoadCallBack, EResourceAssistType assistType, bool isFullPath = false,
        object param = null)
    {
        string path = isFullPath ? name : CSResource.GetPath(name, type, false);

        //从已经加载完成的字典中查询
        CSResource res = GetLoadedRes(path);
        
        //如果没有下载完成,从正在下载队列中查找
        if (res == null) res = GetLoadingRes(path);

        //如果没有下载,也没有正在下载,那么去等待下载队列中查找
        if (res == null) res = GetWaitingQueueRes(path);

        //如果在等待队列中找到了,那么去看看是否需要调整优先级
        if (res != null)
        {
            if ((int)assistType > (int)res.AssistType)
            {
                res.AssistType = assistType;//优先级 只曾不减，防止主角正在等待下载时，来了一个其他玩家优先级较低的下载
            }
            AdjustProri(res);
        }
        else
        {
            //如果3个队列都没有找到,说明之前没有下载过对应资源，那么,新增资源下载数据

            res = new CSResourceWWW(name, path, type);

            if ((int)assistType > (int)res.AssistType)
            {
                res.AssistType = assistType;//优先级 只曾不减，防止主角正在等待下载时，来了一个其他玩家优先级较低的下载
            }

            AddWaitingQueueDic(res);
            AdjustProri(res);
        }

        if (onLoadCallBack != null)
        {
            if (res.IsDone)
            {
                if (onLoadCallBack != null)
                {
                    onLoadCallBack(res);
                }
            }
            else
            {
                res.onLoaded -= onLoadCallBack;
                res.onLoaded += onLoadCallBack;
            }
        }

        res.Param = param;

        return res;
    }

    /// <summary>
    /// 得到已经加载完成的资源字典
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public CSResource GetLoadedRes(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        CSResource res;
        if (loadedQueue.TryGetValue(path, out res))
        {
            return res;
        }
        return null;
    }

    /// <summary>
    /// 得到正在下载队列中的资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public CSResource GetLoadingRes(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (loadingQueue.ContainsKey(path))
        {
            return loadingQueue[path];
        }
        return null;
    }

    /// <summary>
    /// 得到等待下载队列中的资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public CSResource GetWaitingQueueRes(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (waitingQueueDic.ContainsKey(path))
        {
            return waitingQueueDic[path];
        }
        return null;
    }

    private void AddWaitingQueueDic(CSResource res)
    {
        if (!waitingQueueDic.ContainsKey(res.Path))
        {
            waitingQueueDic.Add(res.Path, res);
        }
    }

    private void RemoveWaitingQueueDic(CSResource res)
    {
        if (waitingQueueDic.ContainsKey(res.Path))
        {
            waitingQueueDic.Remove(res.Path);
        }
    }

    private void AddLoadingQueue(CSResource res)
    {
        if (!loadingQueue.ContainsKey(res.Path))
        {
            loadingQueue.Add(res.Path, res);
        }
        if (!loadingQueueList.Contains(res))
        {
            loadingQueueList.Add(res);
        }
    }

    private void RemoveLoadingQueue(CSResource res)
    {
        if (loadingQueue.ContainsKey(res.Path))
        {
            loadingQueue.Remove(res.Path);
        }

        loadingQueueList.Remove(res);
    }

    private void AddLoadedQueue(CSResource res)
    {
        if (loadedQueue.ContainsKey(res.Path))
        {
            return;
        }
        loadedQueue.Add(res.Path, res);
    }


    /// <summary>
    /// 调整资源优先级
    /// </summary>
    /// <param name="res"></param>
    private void AdjustProri(CSResource res)
    {
        if (waitingQueueDic.ContainsKey(res.Path))
        {
            waitingWWWQueue.Remove(res);
            int index = FindProriIndex(res);
            waitingWWWQueue.Insert(index, res);
        }
        else
        {
            int index = FindProriIndex(res);
            waitingWWWQueue.Insert(index, res);
        }
    }


    /// <summary>
    /// WWW资源下载完成后调用
    /// </summary>
    /// <param name="res"></param>
    public void WWWLoaded(CSResource res)
    {
        RemoveLoadingQueue(res);
        AddLoadedQueue(res);
    }

    /// <summary>
    /// 查找当前资源优先级的最大下标
    /// </summary>
    /// <param name="newRes"></param>
    /// <returns></returns>
    private int FindProriIndex(CSResource newRes)
    {
        int index = -1;
        bool isFind = false;
        for (int i = 0; i < waitingWWWQueue.Count; i++)
        {
            CSResource res = waitingWWWQueue[i];
            if ((int)newRes.AssistType > (int)res.AssistType)
            {
                index = i;
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            return waitingWWWQueue.Count;
        }

        if (index == -1)
        {
            return 0;
        }

        return index;
    }

    public GameObject loadUIPanel(string _name)
    {
        GameObject obj = LoadUIAsset(_name);

        if (obj != null)
        {
            GameObject inst = GameObject.Instantiate(obj) as GameObject;

            if (inst != null)
            {
                return inst;
            }
        }

        Resources.UnloadAsset(obj);

        UnityEngine.GameObject.Destroy(obj);
        obj = null;

        return null;
    }

    public GameObject LoadUIAsset(string assetName)
    {
        if(CSGameState.RunPlatform == ERunPlatform.Editor)
        {
            string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName("ui/" + assetName.ToLower(), assetName);
            if (assetPaths.Length == 0)
            {
                if (CSDebug.developerConsoleVisible)
                {
                    CSDebug.LogError("There is no asset with name \"" + assetName + "\" in " + assetName);
                }

                return null;
            }
            UnityEngine.Object target = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);

            GameObject t = target as GameObject;
            return t;
        }
        //else
        //{
        //    LoadedAssetBundle assetBudle = AssetBundleManager.LoadUIAssetAsync("ui/" + assetName.ToLower());

        //    if (assetBudle == null)
        //    {
        //        return null;
        //    }

        //    return assetBudle.m_AssetBundle.LoadAsset<GameObject>(assetName);
        //}

        return null;
    }
}