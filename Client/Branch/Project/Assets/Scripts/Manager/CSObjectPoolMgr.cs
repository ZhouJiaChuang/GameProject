
using System;
using System.Collections.Generic;
using UnityEngine;

public enum EPoolType
{
    Normal,
    Resource,//场景模型Atlas，一直在缓存池里面，知道refCount=0并且超过最大未使用时间时，使用ResourcesManager删除资源
    Normal_ObjParam2,
}
public class CSObjectPoolMgr : SingletonMono<CSObjectPoolMgr>
{
    private CSBetterList<CSObjectPoolBase> mList = new CSBetterList<CSObjectPoolBase>();
    private Dictionary<string, CSObjectPoolBase> mDic = new Dictionary<string, CSObjectPoolBase>();

    //设置了移除的时间,等待移除队列
    CSBetterList<CSObjectPoolItem> WaitRemoveList = new CSBetterList<CSObjectPoolItem>();

    public void Update()
    {
        for (int i = mList.Count - 1; i >= 0; i--)
        {
            CSObjectPoolBase b = mList[i];
            b.CSUpdate();
        }
        for (int i = WaitRemoveList.Count - 1; i >= 0; i--)
        {
            if (WaitRemoveList[i].RemoveTime < Time.time)
            {
                RemovePoolItem(WaitRemoveList[i]);
                //DeleteOtherCacheObject(WaitRemoveList[i]);
                WaitRemoveList.RemoveAt(i);
            }
        }

    }

    public CSObjectPoolItem GetAndAddPoolItem_Class_ProjectOverride(string poolNameShow, string poolName, GameObject go, Type type, params object[] args)
    {
        CSObjectPoolItem poolItem = GetPoolItem(poolNameShow, poolName, EPoolType.Normal, 1000, true);
        CSObjectPoolBase pool = mDic[poolName];
        poolItem.go = go;
        pool.AddPoolItem(poolItem);
        return poolItem;
    }

    public CSObjectPoolItem GetAndAddPoolItem_GameObject(string poolNameShow, string poolName, GameObject origin, bool isForever = false,
        EPoolType type = EPoolType.Normal, bool IsClone = true)
    {
        CSObjectPoolItem poolItem = GetPoolItem(poolNameShow, poolName, type, 1000, isForever);
        CSObjectPoolBase pool = mDic[poolName];
        pool.AddPoolItem(poolItem);

        if (poolItem.go == null && IsClone)
        {
            poolItem.go = GameObject.Instantiate(origin) as GameObject;
        }
        return poolItem;
    }

    /// <summary>
    /// 从缓存池里面获得GameObject，并且从缓存池里面去除,返回的Gameobject.active = false
    /// </summary>resPath 资源路径，也是缓存池名称
    /// <param name="poolName">资源路径</param>
    private CSObjectPoolItem GetPoolItem(string resName, string resPath, EPoolType poolType, int poolNum = 0, bool isForever = false)
    {
        CSObjectPoolBase pool = null;
        string poolName = resPath;

        if (mDic.TryGetValue(poolName, out pool) && pool != null)
        {
            pool = mDic[poolName];
        }
        else
        {
            if (poolNum == 0)
            {
                return null;
            }

            CSStringBuilder.Clear();

            GameObject poolGO = new GameObject();
            Transform trans = poolGO.transform;
            trans.parent = transform;
            if (poolType == EPoolType.Normal)
            {
                pool = poolGO.AddComponent<CSObjectPoolNormal>();
                CSStringBuilder.Append("Normal Pool->", resName);
                poolGO.name = CSStringBuilder.ToString();
            }
            pool.resName = resName;
            //pool.resType = resType;
            pool.Init();
            mDic[poolName] = pool;
            pool.transform.localScale = Vector3.zero;
            //mDic.Add(poolName, pool);
            mList.Add(pool);
        }
        pool.poolNum = poolNum;
        pool.poolName = poolName;
        pool.isForever = isForever;
        return pool.GetGOFromPool();
    }

    public void RemovePoolItem(CSObjectPoolItem item, bool isDestroyResImmi = false, EPoolItemRemoveMethod removeMethod = EPoolItemRemoveMethod.OnDisEnable)
    {
        if (item == null)
        {
            return;
        }

        if (item.owner != null)
        {
            CSObjectPoolBase pool = item.owner;

            if (pool != null)
            {
                pool.RemovePoolItem(item, isDestroyResImmi);
            }
            else
            {
                GameObject.Destroy(item.go);
                item.go = null;
            }
        }
        else
        {
            if (item.go != null)
            {
                GameObject.Destroy(item.go);
                item.go = null;
            }
        }
    }

    public bool DestroyPool(string poolName)
    {
        //UnityEngine.Debug.LogError(poolName);
        if (mDic.ContainsKey(poolName))
        {
            CSObjectPoolBase pool = mDic[poolName];
            bool isDestroy = false;
            if (pool != null)
            {
                //isDestroy = SFOut.IResourceManager.DestroyResource(poolName, false, true, false, true);
                if (isDestroy)
                {
                    pool.CSOnDestroy();
                    mList.Remove(pool);
                }
            }
            if (isDestroy)
            {
                mDic.Remove(poolName);
                return true;
            }

        }
        return false;
    }

    /// <summary>
    /// 设置放入对象池的时间
    /// </summary>
    /// <param name="mPoolItem"></param>
    /// <param name="time">秒</param>
    public void SetPoolRemoveTime(CSObjectPoolItem mPoolItem, float time, EPoolItemRemoveMethod removeMethod = EPoolItemRemoveMethod.OnDisEnable)
    {
        if (mPoolItem == null)
        {
            return;
        }
        mPoolItem.RemoveTime = Time.time + time;
        WaitRemoveList.Add(mPoolItem);
    }
}