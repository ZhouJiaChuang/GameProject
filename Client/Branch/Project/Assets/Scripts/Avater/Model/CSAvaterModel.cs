using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICSAvaterModel
{
    /// <summary>
    /// 模型管理的Update生命周期
    /// </summary>
    void Update();
    /// <summary>
    /// 刷新模型
    /// </summary>
    void FlushModel();
}

public class CSAvaterModel<T> : ICSAvaterModel where T : ModelComponentLoadData, new()
{
    private CSAvater Avater;

    public CSAvaterModel(CSAvater Avater)
    {
        this.Avater = Avater;
    }

    private static Dictionary<int, string> mMotionName;
    /// <summary>
    /// 动作的名字字典,避免由于频繁的设置动作,使用tostring的话造成大量的内存消耗
    /// </summary>
    public static Dictionary<int, string> MotionName
    {
        get
        {
            if (mMotionName == null)
            {
                mMotionName = new Dictionary<int, string>();
                int nums = Enum.GetNames(typeof(CSMotion)).Length;
                for (int i = 0; i < nums; i++)
                {
                    object obj = Enum.GetValues(typeof(CSMotion)).GetValue(i);
                    int key = System.Convert.ToInt32(obj);
                    mMotionName[key] = obj.ToString();
                }
            }
            return mMotionName;
        }
    }

    public virtual void Update()
    {
        UpdatePosition();
        UpdateRotate();
    }

    /// <summary>
    /// 更新坐标
    /// </summary>
    protected virtual void UpdatePosition()
    {

    }

    /// <summary>
    /// 更新旋转
    /// </summary>
    protected virtual void UpdateRotate()
    {

    }

    /// <summary>
    /// 设置模型动作
    /// </summary>
    /// <param name="motion">模型动作</param>
    /// <param name="IsTrigger">模型动作设置状态(如果为true,那么该状态是触发型,触发完成后,会回归到其他状态上,如果为false,直接进入一个持续状态)</param>
    /// <param name="IsForceSet"></param>
    public virtual bool SetAction(CSMotion motion, bool IsTrigger = false, CSMotion triggerRevertMotion = CSMotion.Stand, bool IsForceSet = false)
    {
        return true;
    }

    public virtual bool PlayAction(CSMotion motion, float normalizedTime)
    {
        return true;
    }

    /// <summary>
    /// 设置方向
    /// 跟进传入的方向值,进行旋转角度的设置
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public virtual bool SetDirection(CSDirection direction)
    {
        return true;
    }

    internal void Init()
    {
    }

    /// <summary>
    /// 设置旋转
    /// </summary>
    /// <param name="quaternion">旋转角度</param>
    /// <param name="time">转向时间</param>
    /// <returns></returns>
    public virtual bool SetQuaternion(Quaternion quaternion, float time = 0)
    {
        return true;
    }

    public void FlushModel()
    {
        Analyze();
    }

    /// <summary>
    /// 解析当前需要加载的模型
    /// </summary>
    protected virtual void Analyze()
    {
        BeginAnalyze();

        EndAnalyze();
    }

    protected virtual void BeginAnalyze()
    {

    }

    protected virtual void EndAnalyze()
    {
        StartLoadModelRes();
    }

    /// <summary>
    /// 每个组件加载完成后会进行回调
    /// </summary>
    protected virtual void SingleComponentLoaded()
    {

    }

    /// <summary>
    /// 所有组件加载完成后进行的回调
    /// 每次刷新模型,最后都会回调一次
    /// </summary>
    protected virtual void AllComponentLoaded()
    {

    }

    /// <summary>
    /// 所有的组件加载完成后,都会塞入缓存中,以供下次使用
    /// </summary>
    private static Dictionary<long, ModelComponentLoadData> ModelComponentLoadedCacheDic = new Dictionary<long, ModelComponentLoadData>();
    public CSBetterList<ModelComponentLoadData> WaitLoadComponentKey = new CSBetterList<ModelComponentLoadData>();

    /// <summary>
    /// 添加一个模型的加载数据
    /// 在结束解析的时候,开始加载
    /// </summary>
    /// <param name="model"></param>
    /// <param name="structure"></param>
    /// <param name="motion"></param>
    /// <param name="direction"></param>
    /// <param name="resourceType"></param>
    /// <param name="resourceAssistType"></param>
    protected void Load(uint model, EModelStructure structure, CSMotion motion, CSDirection direction, EResourceType resourceType, EResourceAssistType resourceAssistType)
    {
        //加载的模型不存在
        if (model == 0 )
        {
            //如果要加载身体模型不存在,那么要注意清理其他的组件
            if (structure == EModelStructure.Body)
            {

            }
            return;
        }
        
        long key = GetKey((int)model, (int)motion, (int)direction, (int)structure);
        ModelComponentLoadData data = null;
        if (!ModelComponentLoadedCacheDic.TryGetValue(key, out data))
        {
            data = new T();
            ModelComponentLoadedCacheDic[key] = data;
        }
        data.Model = model;
        data.Key = key;
        data.Structure = structure;
        data.ResourceType = resourceType;
        data.ResourceAssistType = resourceAssistType;

        WaitLoadComponentKey.Add(data);
    }

    /// <summary>
    /// 得到模型组件的唯一Key
    /// 注意,在3D中,所有方向/动作的模型都是同一个,但是2D中,模型是区分方向/动作的,每个2D的动作及方向都是单独的一个图集
    /// </summary>
    /// <param name="model"></param>
    /// <param name="motion"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected virtual long GetKey(int model, int motion, int direction, int structure)
    {
        return (long)model * 1000 + structure;
    }


    private void StartLoadModelRes()
    {
        for (int i = 0; i < WaitLoadComponentKey.Count; i++)
        {
            ModelComponentLoadData data = WaitLoadComponentKey[i];
            data.FileName = data.GetFileName();
            if (string.IsNullOrEmpty(data.ResPath))
            {
                CSResourceManager.Instance.AddQueue(data.FileName, EResourceType.Body, LoadedOneCallback, data.ResourceAssistType, false);
            }
        }
    }

    private void LoadedOneCallback(CSResource obj)
    {
        int MaxCount = WaitLoadComponentKey.Count;
        //例如左手武器/右手武器,可能不同架构使用同种资源,这需要去判定
        for (int i = 0; i < MaxCount; i++)
        {
            ModelComponentLoadData data = WaitLoadComponentKey[i];
            if (obj.FileName == data.FileName)
            {
                data.ResPath = obj.Path;

                ModelComponentLoadedCacheDic[data.Key] = data;

                AddWaitModelComponent(data);

                SingleComponentLoaded();

                WaitLoadComponentKey.RemoveAt(i);
                MaxCount--;
                i--;
            }
        }

        if (WaitLoadComponentKey.Count == 0)
            AllComponentLoaded();
    }

    #region 关于模型组件的相关处理
    /// <summary>
    /// 所有已经下载完成,但是还没有进行组装的组件
    /// </summary>
    private Dictionary<int, ModelComponentLoadData> SleftModelComponentLoadedDic = new Dictionary<int, ModelComponentLoadData>();

    /// <summary>
    /// 每个组件下载完成后,都会加入到等待组装的队列中
    /// 等Body完成后进行组装
    /// </summary>
    public void AddWaitModelComponent(ModelComponentLoadData data)
    {
        SleftModelComponentLoadedDic[(int)data.Structure] = data;
    }

    /// <summary>
    /// 得到已经下载完成的组件
    /// </summary>
    /// <param name="structure"></param>
    /// <returns></returns>
    public ModelComponentLoadData GetWaitModelComponent(EModelStructure structure)
    {
        ModelComponentLoadData data = null;
        if(SleftModelComponentLoadedDic.TryGetValue((int)structure, out data))
        {
            return data;
        }
        return data;
    }

    /// <summary>
    /// 从自身字典中清理下载完成的组件(组装完成后就清理掉)
    /// </summary>
    /// <param name="structure"></param>
    public void RemoveWaitModelComponent(EModelStructure structure)
    {
        int key = (int)structure;
        if (SleftModelComponentLoadedDic.ContainsKey(key))
        {
            SleftModelComponentLoadedDic.Remove(key);
        }
    }

    /// <summary>
    /// 清理所有下载完成的组件数据
    /// </summary>
    public void ClearWaitModelComponent()
    {
        SleftModelComponentLoadedDic.Clear();
    }
    #endregion
}

/// <summary>
/// 模型的组件加载的数据
/// </summary>
public class ModelComponentLoadData
{
    public uint Model = 0;
    public string FileName = string.Empty;
    /// <summary>
    /// 组件唯一Key
    /// </summary>
    public long Key = 0;
    /// <summary>
    /// 该模型所属结构
    /// </summary>
    public EModelStructure Structure;

    public EResourceType ResourceType;
    public EResourceAssistType ResourceAssistType;
    public string ResPath;

    public ModelComponentLoadData()
    {

    }

    public virtual string GetFileName()
    {
        return Model.ToString();
    }
}

public class Model3DComponentLoadData : ModelComponentLoadData
{
    public GameObject Go;

    public Model3DComponentLoadData()
    {
    }
}
