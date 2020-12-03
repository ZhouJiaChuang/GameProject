using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSAvaterInfo
{
    /// <summary>
    /// 信息主体
    /// </summary>
    public CSAvater Avater;
    /// <summary>
    /// 单位的唯一ID
    /// </summary>
    public long ID;
    /// <summary>
    /// 账号用户ID
    /// </summary>
    public string UID;
    /// <summary>
    /// 配置ID
    /// </summary>
    public int CfgID = 0;
    /// <summary>
    /// 单位的名字
    /// </summary>
    public string Name;
    /// <summary>
    /// 等级
    /// </summary>
    public int Level;

    public EAutoState AutoState = EAutoState.None;

    public CSAvaterInfo(CSAvater avater)
    {
        this.Avater = avater;
    }

    public void Update()
    {
    }

    public virtual void Init(object data)
    {
    }
}
