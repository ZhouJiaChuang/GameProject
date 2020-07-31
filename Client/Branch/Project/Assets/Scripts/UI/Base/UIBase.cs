using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    protected EUILayerType mPanelLayerType = EUILayerType.WindowsPlane;
    public virtual EUILayerType PanelLayerType
    {
        get { return mPanelLayerType; }
    }

    public virtual void Show()
    {
    }

    public virtual void Init()
    {
    }
}
