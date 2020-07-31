using UnityEngine;
using System.Collections;
using System.Text;

public class CSStringBuilder
{
    private static CSStringBuilder mSB = new CSStringBuilder();
    public static CSStringBuilder SB
    {
        get { return mSB; }
        set { mSB = value; }
    }

    public StringBuilder strBuilder = new StringBuilder(1000);

    /// <summary>
    /// 用在频繁字符串连接里面（会产生大量碎片，影响GC.Collect的效率问题）
    /// </summary>
    /// <param name="str"></param>
    /// <param name="builder"></param>
    public static StringBuilder Append(string str)
    {
        StringBuilder strBuilder = mSB.strBuilder;
        strBuilder.Append(str);
        return strBuilder;
    }

    public static StringBuilder Append(string str0, string str1)
    {
        StringBuilder strBuilder = mSB.strBuilder;
        strBuilder.Append(str0);
        strBuilder.Append(str1);
        return strBuilder;
    }

    public static StringBuilder Append(string str0, string str1, string str2)
    {
        StringBuilder strBuilder = mSB.strBuilder;
        strBuilder.Append(str0);
        strBuilder.Append(str1);
        strBuilder.Append(str2);
        return strBuilder;
    }

    public static StringBuilder Append(string str0, string str1, string str2, string str3)
    {
        StringBuilder strBuilder = mSB.strBuilder;
        strBuilder.Append(str0);
        strBuilder.Append(str1);
        strBuilder.Append(str2);
        strBuilder.Append(str3);
        return strBuilder;
    }

    public static StringBuilder Append(string str0, string str1, string str2, string str3, string str4)
    {
        StringBuilder strBuilder = mSB.strBuilder;
        strBuilder.Append(str0);
        strBuilder.Append(str1);
        strBuilder.Append(str2);
        strBuilder.Append(str3);
        strBuilder.Append(str4);

        return strBuilder;
    }

    public static StringBuilder Append(string str0, string str1, string str2, string str3, string str4, string str5)
    {
        StringBuilder strBuilder = mSB.strBuilder;
        strBuilder.Append(str0);
        strBuilder.Append(str1);
        strBuilder.Append(str2);
        strBuilder.Append(str3);
        strBuilder.Append(str4);
        strBuilder.Append(str5);

        return strBuilder;
    }

    //public static StringBuilder Replace(string oldStr,string newStr)
    //{
    //    StringBuilder strBuilder = mSB.strBuilder;
    //    strBuilder.Replace(oldStr, newStr);
    //    return strBuilder;
    //}

    public static void Clear()
    {
        StringBuilder strBuilder = mSB.strBuilder;
        strBuilder.Length = 0;
        //strBuilder.Remove(0, strBuilder.Length);
    }



    public static string ToString()
    {
        StringBuilder strBuilder = mSB.strBuilder;

        return strBuilder.ToString();
    }


    public static StringBuilder AppendParams(params object[] strs)
    {
#if ProfileZString
        UnityEngine.Profiling.Profiler.BeginSample("ProfileZString_17");

#endif
        //if (!CSInterfaceSingleton.Unity_Editor.Is_UNITY_EDITOR) return null;
        if (strs != null)
        {
            for (int i = 0; i < strs.Length; i++)
            {
                mSB.strBuilder.Append(strs[i]);
            }
        }
        return mSB.strBuilder;
#if ProfileZString
        UnityEngine.Profiling.Profiler.EndSample();
#endif
    }
    public static string ToStringParams()
    {
        StringBuilder strBuilder = mSB.strBuilder;

        return strBuilder.ToString();
    }

}

public static class StaitcOpi
{
    public static bool CustomEndsWith(this string a, string b)
    {
        int ap = a.Length - 1;
        int bp = b.Length - 1;
        while (ap >= 0 && bp >= 0 && a[ap] == b[bp])
        {
            ap--;
            bp--;
        }
        return (bp < 0 || ap < 0) && a.Length >= b.Length;
    }
    public static bool CustomStartsWith(this string a, string b)
    {
        int aLen = a.Length;
        int bLen = b.Length;
        int ap = 0; int bp = 0;
        while (ap < aLen && bp < bLen && a[ap] == b[bp])
        {
            ap++;
            bp++;
        }
        return (bp == bLen || ap == aLen) && aLen >= bLen;
    }

    //和Mathf.abs性能差别不大。
    //public static float CustomMathfAbs(float x)
    //{
    //    return x > 0 ? x : -x;
    //}

    //public static int CustomMathfAbs(int x)
    //{
    //    return x > 0 ? x : -x;
    //}
}