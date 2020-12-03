using UnityEngine;

/// <summary>
/// 用來进行组件的管理
/// </summary>
public static class StaticUtility
{
    //public static T GetComponentOrCreate<T>(GameObject obj) where T : Component
    //{
    //    T t = obj.GetComponent<T>();
    //    if (t != null)
    //    {
    //        return t;
    //    }
    //    else
    //    {
    //        t = obj.AddComponent<T>();
    //        return t;
    //    }
    //}

    public static bool IsNull(UnityEngine.Object o)
    {
        return o == null;
    }

    public static bool IsNullOfSystemObj(object o)
    {
        
        return o == null;
    }


    public static bool IsNullOrEmpty(string txt)
    {
        return string.IsNullOrEmpty(txt);
    }


}