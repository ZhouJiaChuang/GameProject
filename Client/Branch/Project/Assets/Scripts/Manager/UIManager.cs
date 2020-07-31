/***
 * Author --- ZJC
 * Description --- 用来管理UI的创建/销毁/获取 以及各种界面操作
 * Function:
 * 1.UI创建/销毁/获取
 * 2.层级管理
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI管理类
/// </summary>
public class UIManager : SingletonMono<UIManager>
{
    public GameObject UIRoot;

    public Stack<UIBase> ShowPanes = new Stack<UIBase>();
    /// <summary>
    /// 已显示的UI面板集合
    /// </summary>
    public List<UIBase> ShowPaneList = new List<UIBase>();

    /// <summary>
    /// 设置UI总节点
    /// </summary>
    /// <param name="obj"></param>
    public void SetUIRoot(GameObject obj)
    {
        UIRoot = obj;
        UILayerManager.Instance.SetUILayerRoot(obj);
    }


    public void CreatePanel<T>(System.Action<UIBase> action = null)
    {
        Type type = typeof(T);
        CreatePanelByType(type, action);
    }

    public void CreatePanelByName(string name, System.Action<UIBase> action = null)
    {

    }

    public void CreatePanelByType(Type type, System.Action<UIBase> action = null)
    {
        for (int i = 0; i < ShowPaneList.Count; i++)
        {
            UIBase ins = ShowPaneList[i];
            if (ins == null)
            {
                ShowPaneList.RemoveAt(i);
                continue;
            }
            if (ins.name == type.ToString())
            {
                ins.Show();
                //if (isHasAudio && CSAudioMgr.Instance != null) CSAudioMgr.Instance.Play(true, 6);
                if (action != null)
                {
                    action(ins);
                }

                return;
            }
        }
        CSGame.Instance.StartCoroutine(StartCreatePanel(type, action));
    }

    private IEnumerator StartCreatePanel(Type type, System.Action<UIBase> action)
    {
        string name = type.ToString();
        GameObject go = CSResourceManager.Instance.loadUIPanel(name);
        ///如果之前没有加载过m需要去异步去加载
        if (go == null)
        {
            AddLoadUIToList(name);
            while (true)
            {
                yield return CSAssist.GetWaitForSeconds(0.1f);
                if (IsLoadingUI(name))
                {
                    go = CSResourceManager.Instance.loadUIPanel(name);
                    if (go)
                    {
                        RemoveLoadedUIFromList(name);
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        ///已经加载完成了
        if (go != null)
        {
            go.SetActive(true);
            go.name = name;
            UIBase ret = go.GetComponent(name) as UIBase;
            if (ret == null)
            {
                ret = go.AddComponent(type) as UIBase;
            }

            if (ret == null)
            {
                CSDebug.LogError("UIManger  is  Error");
            }

            //添加
            Push(ret);
            //TODO 讲UI逻辑后一帧处理，分散CPU压力
            UILayerManager.Instance.SetLayer(go, ret.PanelLayerType);
            yield return CSAssist.waitForEndOfFrame;
            if (ret == null)
            {
                yield break;
            }

            ret.Init();
            ret.Show();
            if (action != null)
            {
                action(ret);
            }
        }
    }

    private List<string> LoadingUITypes = new List<string>();
    /// <summary>
    /// 由于是异步加载UI界面,所以需要加入正在加载队列,避免反复创建
    /// </summary>
    /// <param name="uiName"></param>
    public void AddLoadUIToList(string uiName)
    {
        if (string.IsNullOrEmpty(uiName))
        {
            return;
        }

        for (int i = 0; i < LoadingUITypes.Count; i++)
        {
            if (LoadingUITypes[i] == uiName)
            {
                return;
            }
        }
        LoadingUITypes.Add(uiName);
    }

    /// <summary>
    /// 正在加载队列中
    /// </summary>
    /// <param name="uiName"></param>
    /// <returns></returns>
    public bool IsLoadingUI(string uiName)
    {
        for (int i = 0; i < LoadingUITypes.Count; i++)
        {
            if (LoadingUITypes[i] == uiName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 移除正在等待队列
    /// </summary>
    /// <param name="uiName"></param>
    public void RemoveLoadedUIFromList(string uiName)
    {
        for (int i = 0; i < LoadingUITypes.Count; i++)
        {
            if (LoadingUITypes[i] == uiName)
            {
                LoadingUITypes.RemoveAt(i);
                break;
            }
        }
    }

    public void ClosePanel<T>()
    {

    }

    private void Push(UIBase pane)
    {
        ShowPanes.Push(pane);
        ShowPaneList.Add(pane);
    }

    //移除
    private void Pop(UIBase pane)
    {
        ShowPaneList.Remove(pane);
        ShowPanes.Pop();
    }
    
}
