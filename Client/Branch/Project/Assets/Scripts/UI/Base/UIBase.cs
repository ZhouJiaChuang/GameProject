using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    protected EUILayerType mPanelLayerType = EUILayerType.WindowsPlane;
    public virtual EUILayerType PanelLayerType
    {
        get { return mPanelLayerType; }
    }

    #region 组件的获取
    public GameObject Get(GameObject partent, string path)
    {
        return GetObject<GameObject>(partent.transform, path);
    }

    public T Get<T>(string path) where T : UnityEngine.Object
    {
        return GetObject<T>(this.transform, path);
    }

    public T Get<T>(string name, ref T obj) where T : UnityEngine.Object
    {
        return obj ?? (obj = Get<T>(name));
    }

    public static T GetObject<T>(Transform parent, string path) where T : UnityEngine.Object
    {
        if (parent == null)
        {
            return null;
        }

        Transform transform = parent.Find(path);
        if (transform == null)
        {
            return null;
        }

        if (typeof(T) == typeof(Transform))
        {
            return transform as T;
        }

        if (typeof(T) == typeof(GameObject))
        {
            return transform.gameObject as T;
        }

        return transform.GetComponent(typeof(T)) as T;
    }
    #endregion

    public virtual void Show()
    {
    }

    public virtual void Init()
    {
    }
}
