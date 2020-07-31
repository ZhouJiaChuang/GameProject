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
/// 
/// </summary>
public abstract class CSAvater : MonoBehaviour
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

    public CSAvaterModel _Model;
    /// <summary>
    /// 单位模型
    /// </summary>
    public CSAvaterModel Model
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
    public CSObjectPoolItem PoolItem
    {
        get { return mPoolItem; }
        set { mPoolItem = value; }
    }

    public CSAvater()
    {
        Info = new CSAvaterInfo(this);
        Model = new CSAvaterModel(this);
        SkillEngine = new CSSkillEngine(this);
        CreateAvaterGo();
    }

    private void CreateAvaterGo()
    {
        _Go = new GameObject();
        AddAvaterToGo(Go);
    }

    protected abstract void AddAvaterToGo(GameObject obj);
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