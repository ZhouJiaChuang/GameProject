using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSAssist
{
    public static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    private static Dictionary<float, WaitForSeconds> waitForSecondsDic = new Dictionary<float, WaitForSeconds>();
    /// <summary>
    /// 得到一个异步等待时间为了避免反复的new WaitForSeconds();
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static WaitForSeconds GetWaitForSeconds(float f)
    {
        WaitForSeconds delay;
        if (waitForSecondsDic.TryGetValue(f, out delay)) return delay;
        delay = new WaitForSeconds(f);
        waitForSecondsDic.Add(f, delay);
        return delay;
    }

    private static Quaternion DefalutQuaternion = Quaternion.Euler(0, 0, 0);
    public static void SetParent(Transform parent, GameObject c)
    {
        Transform trans = c.transform;
        trans.parent = parent;
        trans.localScale = Vector3.one;
        trans.localPosition = Vector3.zero;
        trans.localRotation = DefalutQuaternion;
    }
}