using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILayerManager : Singleton<UILayerManager>
{
    private Dictionary<int, GameObject> mLayerDic = new Dictionary<int, GameObject>();

    /// <summary>
    /// 设置所有UI层级节点
    /// </summary>
    /// <param name="root"></param>
    public void SetUILayerRoot(GameObject root)
    {
        GameObject.DontDestroyOnLoad(root);
        int nums = Enum.GetNames(typeof(EUILayerType)).Length;
        for (int i = 0; i < nums; i++)
        {
            object obj = Enum.GetValues(typeof(EUILayerType)).GetValue(i);
            int key = System.Convert.ToInt32(obj);
            if (mLayerDic.ContainsKey(key))
            {
                mLayerDic[key] = CreateLayerGameObject(obj.ToString(), root.transform, (EUILayerType)obj);
            }
            else
            {
                mLayerDic.Add(key, CreateLayerGameObject(obj.ToString(), root.transform, (EUILayerType)obj));
            }
        }
    }
    private GameObject CreateLayerGameObject(string name, Transform parent, EUILayerType type)
    {
        GameObject layer = new GameObject(name);
        CSAssist.SetParent(parent, layer);

        RectTransform rectTransform = layer.AddComponent<RectTransform>(); ;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;

        int z = ((int)type) * -1;
        //由于界面上存在层级设置,不能再直接修改UILayerType的层级(新增可以  修改不行)  所以之后的z轴修改在这里处理
        layer.transform.localPosition = new Vector3(0f, 0f, z);

        return layer;
    }


    public void SetLayer(GameObject go, EUILayerType type)
    {
        int t = (int)type;
        GameObject layerobj = mLayerDic[t];

        CSAssist.SetParent(mLayerDic[t].transform, go);

        RectTransform rectTransform = go.GetComponent<RectTransform>(); ;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }

}
