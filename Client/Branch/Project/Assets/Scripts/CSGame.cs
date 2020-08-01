/***
 * Author --- ZJC
 * Description --- 游戏总管理类,游戏开始运行的时候,进行各种管理组件的初始类
 * Function:
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 总管理类
/// </summary>
public class CSGame : MonoBehaviour
{
    private static CSGame _Instance;

    public static CSGame Instance
    {
        get { return _Instance; }
    }

    public EResourceLoadType ResourceLoadType = EResourceLoadType.Normal;

    public GameObject UIRoot;

    private void Awake()
    {
        _Instance = this;
        DontDestroyOnLoad(this);
        CreateAllManagerInstance();
    }

    void Start()
    {
        UIManager.Instance.CreatePanel<UILoginPanel>();
    }

    public void CreateAllManagerInstance()
    {
        UIManager.Instance.SetParent(transform);
        CSSceneManager.Instance.SetParent(transform);
        CSPluginsManager.Instance.SetParent(transform);
        CSResourceManager.Instance.SetParent(transform);

        UIManager.Instance.SetUIRoot(UIRoot);

        CSResourceManager.Instance.AddQueue("5", EResourceType.Body, OnLoaded, EResourceAssistType.Player);
    }

    private void OnLoaded(CSResource obj)
    {
        GameObject.Instantiate(obj.MirrorObj);

        CSSceneManager.Instance.Load(ESceneType.GameScene);
    }
}
