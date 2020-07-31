using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskDescription("设定自动状态--根据Avater下的AutoState枚举值进行判定")]
public class Conditiona_AutoState : CSConditionalBase
{
    public CSAvater Avater;

    public EAutoState TargetAutoState = EAutoState.None;

    public override TaskStatus OnUpdate()
    {
        if (Avater == null) return TaskStatus.Failure;
        //if (Avater.AutoState == TargetAutoState) return TaskStatus.Success;
        return TaskStatus.Failure;
    }
}
