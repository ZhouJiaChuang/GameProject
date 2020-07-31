using UnityEngine;

public class CSObjectPoolNormal : CSObjectPoolBase
{
    public override CSObjectPoolItem GetGOFromPool()
    {
        CSObjectPoolItem item = null;
        if (mList.size > 0 && !mList[0].isUse)//如果正在使用，新建Item，外部去克隆
        {
            item = mList[0];
            mList.MoveToBack(0);
        }
        else
        {
            item = new CSObjectPoolItem();
            item.owner = this;
            mList.Add(item);
        }
        return item;
    }

    public override void AddPoolItem(CSObjectPoolItem item)
    {
        refCount++;
        item.isUse = true;
        //mList.Remove(item);
        if (!mList.Contains(item))
            mList.Add(item);//isUse = true:插入到最后

    }

    public override void ForceUpdateList(CSObjectPoolItem item)
    {
        mList.Remove(item);
        mList.Add(item);//isUse = true:插入到最后
    }

    public override void RemovePoolItem(CSObjectPoolItem item, bool isDestroyResImmi = false)
    {
        int index = -1;
        if (mList.Contains(item, ref index))
        {
            refCount--;
            refCount = Mathf.Max(refCount, 0);
            item.isUse = false;
            if (item.go != null)
            {
                if (item.owner == null || item.owner.transform == null)
                {
                    //item.go.SetActive(false);
                    GameObject.Destroy(item.go);
                    item.go = null;
                }
                else if (item.owner.RemoveMethod == EPoolItemRemoveMethod.OnDisEnable)
                {
                    item.go.SetActive(false);
                    item.go.transform.parent = item.owner.transform;
                }
                else if (item.owner.RemoveMethod == EPoolItemRemoveMethod.RemoveVision)
                {
                    item.go.transform.localPosition = Vector3.zero;
                }
            }
            mList.MoveToFront(index);
            //mList.Remove(item);
            //mList.Insert(0, item);//isUse false:未使用的放到最前面
            if (refCount == 0)
            {
                if (!isDestroyResImmi)
                {
                    mLastNotUseTime = Time.time;
                }
                else
                {
                    mLastNotUseTime = -releaseTime;
                }
            }
        }
        else
        {
            if (item != null)
            {
                GameObject.Destroy(item.go);
                item.go = null;
            }
        }
    }

    protected override void DestroyPoolItem(CSObjectPoolItem item)
    {
        if (item == null)
        {
            return;
        }

        if (item.isUse)
        {
            return;//正在使用的话，不进行删除
        }

        if (CSObjectPoolMgr.Instance == null)
        {
            return;
        }

        if (item.go != null)
        {
            Destroy(item.go);
        }
        item.go = null;
        mList.Remove(item);

        if (mList.size == 0)
        {
            bool isDestroy = CSObjectPoolMgr.Instance.DestroyPool(poolName);
            if (!isDestroy)
            {
                //mLastNotUseTime = mLastNotUseTime+1;//防止不能被删除的资源没有引用时Update中不停调用。但是释放的时候同一帧会进行等待，确认下代码
                mList.Add(item);
            }
        }
    }

    public override void CSUpdate()
    {
        base.CSUpdate();

        if (mList.size == 0)
        {
            return;
        }

        if (Time.time - mLastRealseTime > releaseInterval)
        {
            if (mList.size > poolNum)
            {
                mLastRealseTime = Time.time;
                DestroyPoolItem(mList[0]);
            }
            else if (!isForever)
            {
                if (refCount == 0)
                {
                    //#if UNITY_EDITOR
                    //                    leftReleaseTime = releaseTime - (Time.time - mLastNotUseTime);
                    //#endif
                    if (Time.time - mLastNotUseTime > releaseTime)
                    {
                        DestroyPoolItem(mList[0]);
                    }
                }
            }
        }
    }

    public override void CSOnDestroy()
    {
        base.CSOnDestroy();
    }

    public override void CSOnDestroy(bool isDestroyGameObject)
    {
        base.CSOnDestroy(isDestroyGameObject);
    }
}