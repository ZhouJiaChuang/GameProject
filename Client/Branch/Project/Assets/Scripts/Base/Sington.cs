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
public class Sington<T> where T : new()
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
public class SingtonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _Instance;

    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = System.Activator.CreateInstance<T>();
                CreateInstanceGameobject<T>(_Instance);
            }
            return _Instance;
        }
    }

    /// <summary>
    /// 创建Gameobject 并挂载脚本
    /// </summary>
    /// <typeparam name="D"></typeparam>
    /// <param name="cla"></param>
    private static void CreateInstanceGameobject<D>(T cla) where D : MonoBehaviour
    {
        if (cla == null)
        {
            UnityEngine.Debug.LogError("传入的类型为null,不能创建类的Gameobject");
            return;
        }
        GameObject obj = new GameObject(cla.GetType().FullName);
        obj.AddComponent<D>();
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
