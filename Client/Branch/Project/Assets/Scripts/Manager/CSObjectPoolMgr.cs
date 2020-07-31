
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

    public CSObjectPoolItem GetAndAddPoolItem_Class_ProjectOverride(string poolNameShow, string poolName, GameObject go, Type type, params object[] args)
    {
        CSObjectPoolItem poolItem = GetPoolItem(poolNameShow, poolName, EPoolType.Normal, 1000, true);
        CSObjectPoolBase pool = mDic[poolName];
        poolItem.go = go;
        pool.AddPoolItem(poolItem);
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
}