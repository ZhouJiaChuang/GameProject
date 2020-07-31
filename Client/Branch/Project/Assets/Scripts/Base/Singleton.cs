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
/// 普通的单例
/// </summary>
public class Singleton<T> where T : new()
{
    private static T _Instance;

    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new T();
            }
            return _Instance;
        }
    }
}

/// <summary>
/// Mono单例类
/// 用来创建管理类,在创建的同时,会生成一个Gameobject
/// </summary>
public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour, new()
{
    private static T _Instance;

    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject(typeof(T).ToString());
                _Instance = obj.AddComponent<T>();
            }
            return _Instance;
        }
    }

    /// <summary>
    /// 设置这个Mono组件的挂载点
    /// </summary>
    /// <param name="tran"></param>
    public void SetParent(Transform tran)
    {
        this.transform.parent = tran;
    }

    public virtual void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
