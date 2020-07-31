using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSObjectPoolBase : MonoBehaviour
{
    public bool isForeverCanChange = true;

    public bool mIsForever;
    public bool isForever
    {
        get { return mIsForever; }
        set
        {
            if (!isForeverCanChange)
            {
                return;
            }

            mIsForever = value;
        }
    }
    public string resName;

    //public ResourceType resType;

    public string poolName;

    /// <summary>
    /// 这个参数表示该缓存池，外部有数量
    /// 最大=poolNum,超过的话,就会创建新的
    /// </summary>
    public int refCount = 0;

    /// <summary>
    /// 该对象当前在场景内
    /// 同时最大存活个数
    /// 超过的话,就会继续创建新的对象
    /// </summary>
    public int poolNum = 10;

    /// <summary>
    /// 外部将GameObject返还给缓存池，如果超过了maxLiveCount，超过部分删除的时间间隔
    /// </summary>
    public float releaseInterval = 1;

    protected float mLastRealseTime = 0;

    public float releaseTime//缓存池在多久没用的情况下，自动进行根据releaseInterval删除
    {
        get
        {
            return 3000;
        }
    }

    public EPoolItemRemoveMethod RemoveMethod = EPoolItemRemoveMethod.OnDisEnable;

    //#if UNITY_EDITOR
    //    public float leftReleaseTime = 0;//剩余删除缓存池的时间
    //#endif

    protected float mLastNotUseTime = 0;

    public CSBetterList<CSObjectPoolItem> mList = new CSBetterList<CSObjectPoolItem>();

    public int ListCount
    {
        get
        {
            return mList.Count;
        }
    }

    public virtual void Init()
    {
    }

    public void MarkForeverCanChange(bool b)
    {
        isForeverCanChange = b;
    }

    public virtual CSObjectPoolItem GetGOFromPool()
    {
        return null;
    }

    public virtual void AddPoolItem(CSObjectPoolItem item)
    {

    }

    public virtual void Clear()
    {
        mList.Clear();
        refCount = 0;
    }

    public virtual void ForceUpdateList(CSObjectPoolItem item)
    {

    }

    public virtual void RemovePoolItem(CSObjectPoolItem item, bool isDestroyResImmi = false)
    {

    }

    protected virtual void DestroyPoolItem(CSObjectPoolItem item)
    {

    }

    public virtual void CSUpdate()
    {
        //base.CSUpdate();
    }

    public virtual void CSOnDestroy()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }

        if (mList != null)
        {
            mList.Release();
        }
    }

    public virtual void CSOnDestroy(bool isDestroyGameObject)
    {
        if (isDestroyGameObject && gameObject != null)
        {
            Destroy(gameObject);
        }

        if (mList != null)
        {
            mList.Release();
        }
    }
}
