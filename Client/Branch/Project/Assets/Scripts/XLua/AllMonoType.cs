using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public static class AllMonoType
{
    public static Dictionary<string, Type> dic = new Dictionary<string, Type>()
    {
        #region Unity�Դ�����
        {"BoxCollider",typeof(BoxCollider)},
        {"Animator",typeof(Animator)},
        {"MeshRenderer",typeof(MeshRenderer)},
        {"Transform",typeof(Transform)},
        #endregion
        
        #region
        {"Button",typeof(Button)},
        #endregion

        #region ������
        {"LuaBehaviour",typeof(LuaBehaviour)},
        #endregion

        #region ��������
        #endregion
    };

}