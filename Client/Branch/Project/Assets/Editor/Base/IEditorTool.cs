using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEditorTool
{
    /// <summary>
    /// 界面刷新
    /// </summary>
    void OnGUI();
    /// <summary>
    /// 数据保存
    /// </summary>
    void Save();
    /// <summary>
    /// 数据读取
    /// </summary>
    void Load();
}