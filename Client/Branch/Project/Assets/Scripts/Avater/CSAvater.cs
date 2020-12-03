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
/// 一个模型控制结构为:
/// Avater(MonoBehaviour)类
///    AvaterModel 控制Avater挂载点的坐标位移/坐标旋转/模型创建/模型动作
///    AvaterInfo  记录Avater的数据信息
/// 在Avater初始化的时候,从缓存池中获取到没有使用的Avater类
/// 如果Avater当前不存在挂载点(新创建的Avater),那么克隆该单位,并挂载到对应世界节点下面
/// </summary>
public class CSAvater : MonoBehaviour
{
    private GameObject _Go;
    public GameObject Go
    {
        get { return _Go; }
    }

    public CSAvaterInfo _Info;
    /// <summary>
    /// 单位信息
    /// </summary>
    public CSAvaterInfo Info
    {
        set { _Info = value; }
        get { return _Info; }
    }

    public ICSAvaterModel _Model;
    /// <summary>
    /// 单位模型
    /// </summary>
    public ICSAvaterModel Model
    {
        set { _Model = value; }
        get { return _Model; }
    }

    public CSSkillEngine _SkillEngine;
    /// <summary>
    /// 单位技能
    /// </summary>
    public CSSkillEngine SkillEngine
    {
        get { return _SkillEngine; }
        set { _SkillEngine = value; }
    }

    private BehaviorDesigner.Runtime.BehaviorTree _BehaviorTree;
    /// <summary>
    /// 该单位行为树
    /// </summary>
    public BehaviorDesigner.Runtime.BehaviorTree BehaviorTree
    {
        set { _BehaviorTree = value; }
        get { return _BehaviorTree; }
    }

    private CSObjectPoolItem mPoolItem;
    /// <summary>
    /// 自己Avater类在缓存池中的缓存对象,用来在自身销毁的时候,移除到缓存池中
    /// </summary>
    public CSObjectPoolItem PoolItem
    {
        get { return mPoolItem; }
        set { mPoolItem = value; }
    }

    public CSAvater()
    {
    }
    #region Avater实例化
    public static CSAvater Create<T>() where T : CSAvater
    {
        GameObject _Go = new GameObject();
        CSAvater avater = _Go.AddComponent<T>();
        avater.InitAvaterToGo(_Go);

        return avater;
    }

    /// <summary>
    /// 必要执行
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void InitAvaterToGo(GameObject go)
    {
        _Go = go;
    }

    /// <summary>
    /// 初始化创建
    /// </summary>
    public virtual void InitCreate()
    {
        SkillEngine = new CSSkillEngine(this);
    }

    #endregion

    #region Avater个体初始化
    public void InitData(object data)
    {
        Info.Init(data);
    }

    #endregion

    public void Update()
    {
        Info.Update();

        Model.Update();

        SkillEngine.Update();
    }
}

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedProperty : SharedVariable
    {
        public CSAvater Value { get { return mValue; } set { mValue = value; } }
        [SerializeField]
        private CSAvater mValue;

        public override object GetValue() { return mValue; }
        public override void SetValue(object value) { mValue = (CSAvater)value; }

        public override string ToString() { return (mValue == null ? "null" : mValue.name); }
        public static implicit operator SharedProperty(CSAvater value) { var sharedVariable = new SharedProperty(); sharedVariable.SetValue(value); return sharedVariable; }
    }
}