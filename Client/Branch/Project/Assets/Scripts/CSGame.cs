/***
 * Author --- ZJC
 * Description --- 游戏总管理类,游戏开始运行的时候,进行各种管理组件的初始类
 * Function:
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 总管理类
/// </summary>
public class CSGame : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
        CreateAllManagerInstance();
    }

    void Start()
    {
    }

    public void CreateAllManagerInstance()
    {
        UIManager.Instance.SetParent(transform);
        CSSceneManager.Instance.SetParent(transform);
        CSPluginsManager.Instance.SetParent(transform);
    }
}
