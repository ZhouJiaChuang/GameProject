using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility_Lua
{
    public static UnityEngine.Object GetComponent(Transform trans, string type)
    {
        if (trans == null) return null;
        if (type == "Transform")
        {
            return trans;
        }
        GameObject goTemp = trans.gameObject;
        if (type == "GameObject")
        {
            return goTemp;
        }
        if (type == "BoxCollider")
        {
            return goTemp.GetComponent<BoxCollider>();
        }
        Type t;
        if (AllMonoType.dic.TryGetValue(type, out t))
        {
            return goTemp.GetComponent(t);
        }
#if !UNITY_EDITOR
        
        return goTemp.GetComponent(type);
#endif
        UnityEngine.Debug.LogError("AllMonoType not Exist please add it = " + type);
        return null;
    }

    public static UnityEngine.Object GetComponent(GameObject go, string type)
    {
        if (go == null) return null;
        if (type == "Transform")
        {
            return go.transform;
        }
        GameObject goTemp = go;
        if (type == "GameObject")
        {
            return goTemp;
        }

        Type t;
        if (AllMonoType.dic.TryGetValue(type, out t))
        {
            return goTemp.GetComponent(t);
        }
#if !UNITY_EDITOR
        
        return goTemp.GetComponent(type);
#endif
        UnityEngine.Debug.LogError("AllMonoType not Exist please add it = " + type);
        return null;
    }
}