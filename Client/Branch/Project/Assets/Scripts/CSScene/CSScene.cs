using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景类
/// 在每个场景创建的时候进行初始化,切换场景的时候进行销毁
/// </summary>
public class CSScene : Singleton<CSScene>
{
    /// <summary>
    /// 主角信息类
    /// 每次切换场景的时候,主角模型会进行销毁,但是数据属性始终存在
    /// </summary>
    public static CSMainPlayerInfo MainPlayerInfo = null;

    public Dictionary<long, CSAvater> AvatersID = new Dictionary<long, CSAvater>();
    public Dictionary<int, Dictionary<long, CSAvater>> AvaterDic = new Dictionary<int, Dictionary<long, CSAvater>>();

    /// <summary>
    /// 网络事件管理
    /// </summary>
    protected EventHandlerManager SocketEventHandler = new EventHandlerManager(EventHandlerManager.DispatchType.Socket);

    #region 场景类的必要处理
    /// <summary>
    /// 
    /// </summary>
    public CSScene()
    {
        BindEvent();
    }

    private void BindEvent()
    {
        //SocketEventHandler.AddEvent(ESocketEvent.ResUpdateViewPlayerMessage, OnResUpdateViewMessage);
    }


    /// <summary>
    /// 每次切换场景后,都需要调用
    /// </summary>
    public void Enter()
    {

    }

    /// <summary>
    /// 每次离开场景都需要调用
    /// </summary>
    public void Leave()
    {

    }

    public void Desotroy()
    {

    }
    #endregion

    #region 场景上的单位处理 -- 涉及到单位的进入,移除,创建
    /// <summary>
    /// 刷新场景上的视野
    /// </summary>
    /// <param name="uiEvtID"></param>
    /// <param name="data"></param>
    private void OnResUpdateViewMessage(uint uiEvtID, object[] data)
    {
        ProcessAvatarEnterView<CSMainPlayer>(0001, null, EAvaterType.MainPlayer);
    }

    /// <summary>
    /// 单位进入视野
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <param name="type"></param>
    public virtual void ProcessAvatarEnterView<T>(long id, object data, EAvaterType type) where T : CSAvater, new()
    {
        if (id == 0)
        {
            return;
        }

        CSAvater avater = GetAvater(id);
        if (avater == null)
        {
            avater = GetPoolItem<T>(typeof(T));
        }
        //刷新的视野还在场景中,那么不进行创建,而是进行单位的刷新
        avater.InitData(data);

        TryCreateAvater(avater, null);
        //AddWaitDeal(avatar, TryCreateAvater, CreateAvatarInternal, null);
    }

    /// <summary>
    /// 得到Avater的对象缓存池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    protected CSAvater GetPoolItem<T>(Type type) where T : CSAvater, new ()
    {
        if (CSObjectPoolMgr.Instance == null)
        {
            return null;
        }

        CSAvater avater = null;
        string s = type.ToString();
        CSObjectPoolItem poolItem = CSObjectPoolMgr.Instance.GetAndAddPoolItem_Class_ProjectOverride(s, s, null, type, null);
        if (poolItem.objParam == null && type != null)
        {
            poolItem.objParam = CSAvater.Create<T>();
        }
        avater = poolItem.objParam as CSAvater;
        avater.InitCreate();
        avater.PoolItem = poolItem;
        return avater;
    }

    public CSAvater GetAvater(long id)
    {
        CSAvater avater = null;
        if(AvatersID.TryGetValue(id,out avater))
        {
            return avater;
        }
        return null;
    }

    protected bool TryCreateAvater(object obj, object param)
    {
        CSAvater avater = obj as CSAvater;
        if (avater == null)
        {
            return false;
        }

        //刷新模型
        avater.Model.FlushModel();

        return true;
    }
    #endregion
}
