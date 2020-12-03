using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSAvaterModel3D : CSAvaterModel<Model3DComponentLoadData>
{
    private Animator _animator;

    public Animator AvaterAnimator
    {
        get { return _animator; }
        set
        {
            if (_animator == value) return;
            _animator = value;
        }
    }

    public CSAvaterModel3D(CSAvater Avater) : base(Avater)
    {
    }

    public override bool SetAction(CSMotion motion, bool IsTrigger = false, CSMotion triggerRevertMotion = CSMotion.Stand, bool IsForceSet = false)
    {
        if(!base.SetAction(motion, IsTrigger, triggerRevertMotion, IsForceSet))
        {
            return false;
        }
        if (AvaterAnimator == null) return false;


        return true;
    }
    
    /// <summary>
    /// 每个组件完成的时候,都需要尝试去组装
    /// </summary>
    protected override void SingleComponentLoaded()
    {
        base.SingleComponentLoaded();
        TryCreateBody();
    }

    private void TryCreateBody()
    {
        Model3DComponentLoadData data = GetWaitModelComponent(EModelStructure.Body) as Model3DComponentLoadData;
        if (data != null)
        {
            RemoveWaitModelComponent(EModelStructure.Body);
            CSObjectPoolItem body = AddPool(data.ResPath);
            if (body != null && body.go != null)
            {

            }
        }
    }

    private CSObjectPoolItem AddPool(string resPath)
    {
        CSResource resource = CSResourceManager.Instance.GetLoadedRes(resPath);
        if (resource == null) return null;
        CSObjectPoolItem item = resource.GetPoolItem(EPoolType.Normal);

        return item;
    }
}

public struct SAvaterActionData
{
    /// <summary>
    /// 目标朝向,会在Update中,不断的改变自己的旋转角度
    /// </summary>
    public Quaternion TargetQuaternion;
    /// <summary>
    /// 当前动作
    /// </summary>
    public CSMotion NowMotion;
    /// <summary>
    /// 上一次动作
    /// </summary>
    public CSMotion LastMotion;
    /// <summary>
    /// 上一个动作是否为触发动作(当被石化等状态的时候,不能设置动作,但是,当结束的时候,需要重新去设置)
    /// </summary>
    public bool LastMotionIsTrigger;
}
