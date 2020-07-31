using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSEventDelegate<T>
{
    public delegate void OnLoaded(T obj);

    private CSBetterList<OnLoaded> onLoadedList = new CSBetterList<OnLoaded>();

    private void AddCallBack(OnLoaded onLoaded)
    {
        onLoadedList.Insert(0, onLoaded);//保证调用的先后性,插到最前面,调用是从最后一个往前调用的
    }

    public void AddFrontCallBack(OnLoaded onLoaded)
    {
        onLoadedList.Add(onLoaded);
    }

    private void RemoveCallBack(OnLoaded onloaded)
    {
        onLoadedList.Remove(onloaded);
    }

    public void CallBack(T obj)
    {
        for (int i = onLoadedList.Count - 1; i >= 0; i--)//防止回调里面做了-=的操作
        {
            if (i >= onLoadedList.Count) continue;
            if (onLoadedList[i] != null)
            {
                onLoadedList[i](obj);
            }
        }
    }

    public void Clear()
    {
        onLoadedList.Clear();
    }

    public void Release()
    {
        onLoadedList.Release();
    }

    public static CSEventDelegate<T> operator +(CSEventDelegate<T> dele, OnLoaded onload)
    {
        dele.AddCallBack(onload);
        return dele;
    }

    public static CSEventDelegate<T> operator -(CSEventDelegate<T> dele, OnLoaded onload)
    {
        dele.RemoveCallBack(onload);
        return dele;
    }
}