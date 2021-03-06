﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// WWW资源加载
/// </summary>
public class CSResourceWWW : CSResource
{
    /// <summary>
    /// 资源的相对路径
    /// </summary>
    private string relatePath;

    public AssetBundle assetBundle;

    public CSResourceWWW(string name, string path, EResourceType type) : base(name, path, type)
    {
    }

    public override void Load()
    {
        if (IsDone)
        {
            ///如果已经下载完成,那么直接信息事件回调
            CallBack();
            return;
        }
        LoadProc(Path);
    }

    /// <summary>
    /// 完成下载,进行事件回调
    /// </summary>
    private void CallBack()
    {
        if (base.onLoaded == null) return;
        try
        {
            base.onLoaded.CallBack(this);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
        }
        base.onLoaded.Clear();
    }


    Coroutine mCoroutine = null;
    private void LoadProc(string path)
    {
        relatePath = path.Replace(URL.ClientResURL, "");

        IsDone = false;
        if (mCoroutine != null)
        {
            CSGame.Instance.StopCoroutine(mCoroutine);
        }

        if (!IsAssetBundle(LocalType))
        {
            mCoroutine = CSGame.Instance.StartCoroutine(GetData(path));
        }
        else
        {
            mCoroutine = CSGame.Instance.StartCoroutine(GetAssetBundleData(path));
        }
    }

    /// <summary>
    /// 加载数据资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private IEnumerator GetData(string path)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                if (CSGame.Instance.ResourceLoadType == EResourceLoadType.Local)
                {
                    OnLoadedErrorProc();
                }
                else
                {
                    DealNeedWaitHotUpdate();
                }
            }
            else
            {
                MirroyBytes = www.downloadHandler.data;
                LoadFinish();

            }
        }
    }

    /// <summary>
    /// 加载AB资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private IEnumerator GetAssetBundleData(string path)
    {
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(path))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                if (CSGame.Instance.ResourceLoadType == EResourceLoadType.Local)
                {
                    OnLoadedErrorProc();
                }
                else
                {
                    DealNeedWaitHotUpdate();
                }
            }
            else
            {
                try
                {
                    assetBundle = assetBundle != null ? assetBundle : DownloadHandlerAssetBundle.GetContent(www);
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogError(ex.Message);
                    OnLoadedErrorProc();
                    yield break;
                }
                yield return SyncLoadMainAsset(true);
            }
        }

    }

    /// <summary>
    /// 异步读取AB内的资源
    /// </summary>
    /// <param name="isFromLocalLoad"></param>
    /// <returns></returns>
    private IEnumerator SyncLoadMainAsset(bool isFromLocalLoad = false)
    {
        string[] strs = assetBundle.GetAllAssetNames();
        if (strs.Length > 0)
        {
            string arName = strs[0];
            //yield return null;
            AssetBundleRequest ar = assetBundle.LoadAssetAsync(arName);
            ar.priority = (int)System.Threading.ThreadPriority.BelowNormal;
            yield return ar;
            //yield return null;

            MirrorObj = ar.asset;
            ar = null;
            //yield return null;
            LoadFinish(isFromLocalLoad);
        }
    }

    /// <summary>
    /// 下载完成
    /// </summary>
    /// <param name="isFromLocalLoad"></param>
    private void LoadFinish(bool isFromLocalLoad = false)
    {
        IsDone = true;

        CSResourceManager.Instance.WWWLoaded(this);

        CallBack();
    }

    /// <summary>
    /// 资源下载错误
    /// </summary>
    private void OnLoadedErrorProc()
    {
        IsDone = true;
        CallBack();
    }

    /// <summary>
    /// 下载失败,或者该资源需要更新,那么从服务器上面等待下载
    /// </summary>
    public void DealNeedWaitHotUpdate()
    {
        loadedTime = UnityEngine.Time.time;
        IsDone = false;
        CSResourceManager.Instance.WWWLoaded(this);
        DownloadFromServer();
    }

    /// <summary>
    /// 添加到下载队列中
    /// </summary>
    private void DownloadFromServer()
    {
        //IsHotLoading = true;

        //CSResource res;

        //if (SFOut.IResUpdateMgr.CheckIsNeedDownload(relatePath))
        //{
        //    int resType = SFOut.IResUpdateMgr.GetResourceType(relatePath);
        //    res = new Resource(relatePath, 0, 0, resType, DownloadType.GamingDownload);
        //}
        //else
        //{
        //    res = new Resource(relatePath, 0, 0, DownloadType.GamingDownload);
        //}

        //SFOut.IResUpdateMgr.AddToDownloadQueue(res);
    }
}