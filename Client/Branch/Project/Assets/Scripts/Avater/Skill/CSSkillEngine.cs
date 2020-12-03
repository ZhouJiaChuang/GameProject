/***
 * Author --- ZJC
 * Description --- 
 * Function:
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能引擎
/// </summary>
public class CSSkillEngine
{
    /// <summary>
    /// 信息主体
    /// </summary>
    public CSAvater Avater;

    public List<CSSkill> Skills = new List<CSSkill>();

    public CSSkillEngine(CSAvater avater)
    {
        this.Avater = avater;
    }

    public void Update()
    {
    }
}
