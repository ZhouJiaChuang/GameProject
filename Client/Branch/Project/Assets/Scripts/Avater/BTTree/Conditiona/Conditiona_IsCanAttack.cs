using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

/// <summary>
/// 判定玩家是否在等待
/// </summary>
public class Conditiona_IsCanAttack : CSConditionalBase
{
    public CSAvater Avater;

    public override TaskStatus OnUpdate()
    {
        if (Avater == null)
        {
            return TaskStatus.Failure;
        }
        return TaskStatus.Success;
    }
}
