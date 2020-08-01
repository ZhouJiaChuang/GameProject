/***
 * Author --- ZJC
 * Description --- 
 * Function:
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景管理类
/// 用来切换场景
/// </summary>
public class CSSceneManager : SingletonMono<CSSceneManager>
{
    public delegate bool WaitLoadSceneCallback();
    public delegate void LoadingSceneCallback(float progress);
    public delegate void LoadedSceneCallback();

    private WaitLoadSceneCallback waitLoadSceneCallback;
    private LoadingSceneCallback loadingSceneCallback;
    private LoadedSceneCallback loadedSceneCallback;

    public string Now = ESceneType.FirstScene.ToString();
    /// <summary>
    /// 加载中的场景
    /// </summary>
    public string LoadingScene { get; protected set; }

    public AsyncOperation mAsync { get; protected set; }

    public void Load(ESceneType targetScene,
        WaitLoadSceneCallback waitLoadSceneCallback = null, LoadingSceneCallback loadingSceneCallback = null, LoadedSceneCallback loadedSceneCallback = null)
    {
        LoadByName(targetScene.ToString(), waitLoadSceneCallback, loadingSceneCallback, loadedSceneCallback);
    }

    public void LoadByName(string sceneName, 
        WaitLoadSceneCallback waitLoadSceneCallback = null, LoadingSceneCallback loadingSceneCallback = null, LoadedSceneCallback loadedSceneCallback = null)
    {
        StartCoroutine(WaitForLoadScenePassEmpty(sceneName, waitLoadSceneCallback, loadingSceneCallback, loadedSceneCallback));
    }

    protected IEnumerator WaitForLoadScenePassEmpty(string targetScene, 
        WaitLoadSceneCallback waitLoadSceneCallback = null, LoadingSceneCallback loadingSceneCallback = null, LoadedSceneCallback loadedSceneCallback = null)
    {
        this.waitLoadSceneCallback = waitLoadSceneCallback;
        this.loadingSceneCallback = loadingSceneCallback;
        this.loadedSceneCallback = loadedSceneCallback;

        yield return null;
        if (LoadingScene != targetScene)
        {
            LoadingScene = targetScene;
            //异步等待加载空场景
            yield return DynaLoadScene(ESceneType.EmptyScene.ToString());
            mAsync = SceneManager.LoadSceneAsync(ESceneType.EmptyScene.ToString());

            yield return mAsync;
            yield return CSAssist.waitForEndOfFrame;

            //如果存在开始加载真正场景之前的操作,那么在回调返回true之前,不进行下一步
            if (waitLoadSceneCallback != null)
            {
                while(waitLoadSceneCallback() == false)
                {
                    yield return CSAssist.waitForEndOfFrame;
                }
            }

            //开始加载真正需要的场景
            mAsync = SceneManager.LoadSceneAsync(targetScene.ToString());
            mAsync.allowSceneActivation = false;

            yield return null;

            while (!mAsync.isDone && mAsync.allowSceneActivation == false)
            {
                if (mAsync.progress < 0.9f)
                {
                    if (loadingSceneCallback != null)
                    {
                        loadingSceneCallback(mAsync.progress);
                    }
                    yield return null;
                }
                else
                {
                    mAsync.allowSceneActivation = true;
                    break;
                }

            }
            yield return mAsync;
            if (mAsync.isDone)
            {
                if (loadedSceneCallback != null)
                {
                    loadedSceneCallback();
                }
            }
        }
        else
        {
            if (CSDebug.developerConsoleVisible)
            {
                CSDebug.LogError("you try to load the same Scene = " + LoadingScene);
            }
        }
    }

    /// <summary>
    /// 读取本地的场景文件
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    protected virtual IEnumerator DynaLoadScene(string sceneName)
    {
        string fliePath = URL.FilePrePath;
        fliePath = fliePath + "/" + CSResource.GetTypeRelativePath(EResourceType.SceneRes) + sceneName;

        if (!System.IO.File.Exists(fliePath))
        {
            if (CSDebug.developerConsoleVisible)
            {
                CSDebug.Log("path is not exsit=" + fliePath);
            }

            yield break;
        }
        if (CSDebug.developerConsoleVisible)
        {
            CSDebug.Log("Loading Assetbundle Scene =" + fliePath);
        }

        CSResource res = CSResourceManager.Instance.AddQueue(sceneName, EResourceType.SceneRes, null, EResourceAssistType.ForceLoad);
        while (!res.IsDone)
        {
            yield return null;
        }
    }
}
