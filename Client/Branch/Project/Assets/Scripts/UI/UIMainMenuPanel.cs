using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenuPanel : UIBase
{
    private GameObject _LeftUpRoot;
    public GameObject LeftUpRoot
    {
        get { return Get("Widget/LeftUpRoot", ref _LeftUpRoot); }
    }

    private GameObject _LeftDownRoot;
    public GameObject LeftDownRoot
    {
        get { return Get("Widget/LeftDownRoot", ref _LeftDownRoot); }
    }

    private GameObject _RightUpRoot;
    public GameObject RightUpRoot
    {
        get { return Get("Widget/RightUpRoot", ref _RightUpRoot); }
    }

    private GameObject _RightDownRoot;
    public GameObject RightDownRoot
    {
        get { return Get("Widget/RightDownRoot", ref _RightDownRoot); }
    }

    public override void Init()
    {
        base.Init();
        AddWindowToWidgetRoot<UIRoleHeadPanel>(LeftUpRoot.transform);
        AddWindowToWidgetRoot<UIMiniMapPanel>(RightUpRoot.transform);
    }

    private void AddWindowToWidgetRoot<T>(Transform transform)
    {
        UIManager.Instance.CreatePanel<T>((ui) =>
        {
            CSAssist.SetParent(transform, ui.gameObject, true);
        });
    }

}
