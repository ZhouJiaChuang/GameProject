using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[TaskDescription("返回Failure 打断Sequence")]
public class Action_BreakSequence : CSActionBase
{
    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Failure;
    }
}
