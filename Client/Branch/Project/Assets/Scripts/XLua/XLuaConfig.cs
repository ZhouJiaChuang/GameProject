using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using XLua;
#if UNITY_EDITOR
#endif
/// <summary>
/// xlua生成说明:
/// 一个类如果被xlua生成wrap代码，如果在lua中调用了这个wrap中不存在的变量或者属性或者函数，会找不到这个数据从而报错，一般屏蔽的原则就是不会在lua中调用的public member屏蔽。
/// 1 客户端表格Proto非字符串Specified不生成，表示这些字段在C#和lua中都不能使用
/// 2 服务器Proto Specified字段C#还是生成，wrap代码不生成，表示lua中无法使用这些字段（当前没有发现有这样调用服务器proto的逻辑）
/// 3 fieldBlackGenericDic:表示fieldInfo字段（变量）需要屏蔽的字段
/// 4 notMethodOrProperBlackGenericDic:表示属性和函数不屏蔽的字段。
/// </summary>
public static partial class XLuaConfig
{
    public static List<Assembly> assemblysList = new List<Assembly>();
    public static List<Assembly> projectCacheAssemblysList = new List<Assembly>();
    public static List<string> errorList = new List<string>();


    private static List<Type> mCustomTypeList = new List<Type>();


    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>() {
        typeof(UnityEngine.Object),
        typeof(Vector2),
        typeof(Vector3),
        typeof(Vector4),
        typeof(Quaternion),
        typeof(Color),
        typeof(Color32),
        typeof(Time),
        typeof(GameObject),
        typeof(Component),
        typeof(Behaviour),
        typeof(Transform),
        typeof(MonoBehaviour),
        typeof(SkinnedMeshRenderer),
        typeof(Renderer),
        typeof(WWW),
        typeof(System.Collections.Generic.List<int>),
        typeof(Action<string>),
        typeof(UnityEngine.Application),
        typeof(MemoryStream),
        typeof(BinaryWriter),
        typeof(Mathf),
        typeof(Screen),
        typeof(Texture2D),
        typeof(Texture),
        typeof(Camera),
        typeof(CSStringBuilder),
        typeof(UnityEngine.PlayerPrefs),
        typeof(UnityEngine.Coroutine),
        typeof(CSDebug),
        typeof(UnityEngine.Collider),
        typeof(UnityEngine.Material),
        typeof(UnityEngine.BoxCollider),
    };

    public static Dictionary<Type, bool> notDelayLoaderNestedTypeDic = new Dictionary<Type, bool>()
    {
        {typeof(UnityEngine.Camera),true },
        {typeof(UnityEngine.ParticleSystem),true },
        {typeof(UnityEngine.Texture2D),true },
    };

    public static void Clear()
    {
        assemblysList.Clear();
        projectCacheAssemblysList.Clear();
    }

    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp2
    {
        get
        {
            return CustomTypeList();
        }
    }

    /// <summary>
    /// 下面的最好有什么工具找到他们。
    /// </summary>
    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>() {
                typeof(Action),
                typeof(Func<double, double, double>),
                typeof(Action<string>),
                typeof(Action<double>),
                typeof(UnityEngine.Events.UnityAction),
                typeof(System.Collections.IEnumerator),
                typeof(CSEventDelegate<CSResource>.OnLoaded),
                typeof(Type),
                typeof(UIBase),
                typeof(System.Action<UIBase>),
                typeof(BaseEvent.Callback),
                typeof(Func<object, bool>),
                typeof(Action<object, float>),
                typeof(Action<float, float>),
                typeof(Action<object, int>),
                typeof(Action<object,float, float>),
                typeof(Func<object, object, int>),
                typeof(UnityEngine.Camera.CameraCallback),
                typeof(System.Action<int>),
                typeof(System.Action<int, object>),
                typeof(System.Action<long, object>),
                typeof(System.Action<string, object>),
                typeof(System.Action<object>),
                typeof(Func<object, object, bool>),
                typeof(Action<long, int, object>),
                typeof(Action<long, int>),
                typeof(Action<object>),
                typeof(Func<object, string, object[], bool>),
                typeof(Func<object, int, object[], bool>),
                typeof(Action<object, bool, int, int>),
                typeof(Action<object, int, int, int, bool>),
                typeof(Action<object, int>),
            };

    /// <summary>
    /// 类似public delegate void VoidDelegate(GameObject go);这样写法的委托能够找到。
    /// </summary>
    [CSharpCallLua]
    public static List<Type> CSharpCallLua2
    {
        get
        {
            List<Type> list = new List<Type>();
            Type[] types = GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                //if (IsUnityEditor(types[i])||IsUnityEditor(types[i].ReflectedType)) continue;
                Type type = types[i];
                if (type.Namespace != null && type.Namespace == "MikuLuaProfiler")
                {
                    continue;
                }

                if (type.Name == "ArrayAction" ||
                    type.Name == "ArrayAction_IntArray")
                {
                    continue;
                }
                //if (type.IsGenericType)
                //{
                //    if (IsDelegate(type))
                //    {
                //        //泛型类，里面有委托。
                //        //UnityEngine.Debug.Log(type.ToString());
                //    }
                //}
                if (IsDelegate(type))
                {
                    if (type.Namespace != null)
                    {
                        if (type.Namespace == "XLua" ||
                            type.Namespace.StartsWith("XLua.") ||
                            type.Namespace == "Spine" ||
                            type.Namespace.StartsWith("Spine."))
                            continue;
                    }
                    if (!list.Contains(type))
                    {
                        string typeString = type.ToString();
                        if (!typeString.Contains("<"))
                        {
                            //UnityEngine.Debug.LogError(type + " add");
                            list.Add(type);
                        }
                    }
                }
                else
                {
                    //Type memberType = GetInternalDelegeteType(type);
                    //if (memberType != null)
                    //{
                    //    if (!list.Contains(memberType))
                    //    {
                    //        string typeString = memberType.ToString();
                    //        if (!typeString.Contains("<"))
                    //        {
                    //            //UnityEngine.Debug.LogError(memberType + " add");
                    //            list.Add(memberType);
                    //        }
                    //    }
                    //}
                }
            }
            //int index = 0;
            //foreach(var c in list)
            //{
            //    index++;
            //    UnityEngine.Debug.Log("delegate "+index+"/"+list.Count+" "+c.ToString());
            //}
            //UnityEngine.Debug.LogError("delegate num = " + list.Count);
            return list;
        }
    }

    /// <summary>
    /// 对于值类型（例如Struct）只对public变量进行xlua-C#的传值GC优化，建议将有非public的值类型打印出来手动改成public，不通过
    /// AdditionalProperties来配置了
    /// </summary>
    /// 1、所有的基本值类型（所有整数，所有浮点数，decimal）；

    ///2、所有的枚举类型；

    ///3、字段只包含值类型的struct，可嵌套其它只包含值类型struct；

    ///其中2、3需要把该类型加到GCOptimize。
    ///需要Unity的值类型也加到GCOptimize里面？
    [GCOptimize]
    public static List<Type> GCOptimizeList
    {
        get
        {
            List<Type> list = CustomTypeList();
            List<Type> resultList = new List<Type>();
            for (int i = 0; i < list.Count; i++)
            {
                Type type = list[i];
                //if (type.IsAbstract && type.IsSealed) continue;
                //resultList.Add(type);
                if (type.IsValueType)
                {
                    resultList.Add(type);
                    //UnityEngine.Debug.LogError(type + " v add");
                }

                if (type.IsEnum)
                {
                    resultList.Add(type);
                    //UnityEngine.Debug.LogError(type + " e add");
                }
            }
            return resultList;
        }
    }
    //C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface

    [Hotfix(HotfixFlag.Stateless)]
    public static List<Type> HotfixList
    {
        get
        {
            return GetHotFixList(true);
        }
    }

#if UNITY_IOS
    //IOS防止代码裁剪，没有生成适配代码的函数无法通过反射获得
    [ReflectionUse]
    public static List<Type> ReflectionUseList
    {
        get
        {
            return GetTypesList();
        }
    }
#endif
    #region 黑名单
    //类 函数 函数参数
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()
    {
    };

    //类 函数名（不管该函数有多少个重载，都不生成）
    private static Dictionary<string, List<string>> blackTypeToMedthods = new Dictionary<string, List<string>>()
    {
        //{"UIInput",new List<string>(){"ProcessEvent"}},
        //{"UIWidget",new List<string>(){"showHandlesWithMoveTool","showHandles"}},
    };

    //类（整个类都不生成）
    private static List<string> blackList = new List<string>()
    {
        "CSDebug",
    };

    //#endif

    private static List<string> mTrulyBlackList;
    private static bool blackListIsDirty = true;
    /// <summary>
    /// 获取类的黑名单列表
    /// </summary>
    /// <returns></returns>
    public static List<string> GetClassNameBlackList()
    {
        if (mTrulyBlackList == null || blackListIsDirty)
        {
            blackListIsDirty = false;
            mTrulyBlackList = new List<string>();
            mTrulyBlackList.AddRange(blackList);
            var listTemp = ReadProtobufWrapBlackList();
            if (listTemp != null)
            {
                for (int i = 0; i < listTemp.Count; i++)
                {
                    if (!mTrulyBlackList.Contains(listTemp[i]))
                    {
                        mTrulyBlackList.Add(listTemp[i]);
                    }
                }
            }
        }
        return mTrulyBlackList;
    }

    /// <summary>
    /// 清空黑名单列表
    /// </summary>
    public static void ClearTrulyBlackList()
    {
        mTrulyBlackList = null;
    }

    private static List<string> ReadProtobufWrapBlackList()
    {
        if (CSGameState.RunPlatform != ERunPlatform.Editor)
        {
            return null;
        }
        //string protobufWrapBlackFilePath = dynamichotSaveTxtPath + "ProtobufCSWrapBlackList.txt";
        //try
        //{
        //    string[] lines = File.ReadAllLines(protobufWrapBlackFilePath);
        //    if (lines != null)
        //    {
        //        List<string> blackList = new List<string>();
        //        for (int i = 0; i < lines.Length; i++)
        //        {
        //            string str = lines[i].Trim();
        //            if (str[0] == '/' && str[1] == '/')
        //            {
        //                continue;
        //            }
        //            blackList.Add(str);
        //        }
        //        return blackList;
        //    }
        //}
        //catch (Exception) { }
        return null;
    }


    #endregion


    private static Type GetInternalDelegeteType(Type type)
    {
        if (type == null)
        {
            return null;
        }

        BindingFlags allFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        FieldInfo[] fs = type.GetFields(allFlag);
        foreach (FieldInfo f in fs)
        {
            if (IsDelegate(f.FieldType))
            {
                //UnityEngine.Debug.Log(type.ToString() + " " + f.ToString() + " " + f.FieldType.ToString());
                return f.FieldType;
            }
        }
        PropertyInfo[] ps = type.GetProperties(allFlag);
        foreach (PropertyInfo p in ps)
        {
            if (IsDelegate(p.PropertyType))
            {
                //UnityEngine.Debug.Log(type.ToString() + " " + p.ToString() + " " + p.PropertyType.ToString());
                return p.PropertyType;
            }
        }
        MethodInfo[] ms = type.GetMethods(allFlag);
        foreach (MethodInfo m in ms)
        {
            if (IsDelegate(m.ReturnType))
            {
                //UnityEngine.Debug.Log(type.ToString() + " " + m.ToString() + " " + m.ReturnType.ToString());
                return m.ReturnType;
            }
            ParameterInfo[] pts = m.GetParameters();
            foreach (ParameterInfo pt in pts)
            {
                if (IsDelegate(pt.ParameterType))
                {
                    //UnityEngine.Debug.Log(type.ToString() + " " + m.ToString() + " " + pt.ParameterType.ToString());
                    return pt.ParameterType;
                }
            }
        }
        return null;
    }

    private static bool IsUnityEditor(Type type)
    {
        if (type == null)
        {
            return false;
        }

        if (type.Namespace == "UnityEditor")
        {
            return true;
        }

        type = type.BaseType;
        return IsUnityEditor(type);
    }

    public static bool IsDelegate(this Type type)
    {
        return type.IsSubclassOf(typeof(Delegate));
    }

    private static List<Type> CustomTypeList()
    {
        bool isLocalTestTest;
        //if (isHotFix)
        //{
        //    return GetHotFixList(isHotFix);
        //}

        List<Type> list = GetSaveList(out isLocalTestTest, false);
        //if (isLocalTestTest)
        //{
        //    return list;
        //}

        //mCustomTypeList = GetGameCustomType(isHotFix);

        return list;
    }

    public static List<Type> GetSaveList(out bool isLocalTestTest, bool isHotFix = false)
    {
        blackListIsDirty = true;
        List<Type> saveList = new List<Type>();
        isLocalTestTest = true;
        //string path = hotSaveTxtPath;
        //string path2 = luahotSaveTxtPath;
        //string path3 = dynamichotSaveTxtPath;
        //isLocalTestTest = true;
        //string content = "";
        //bool SimplyAndIsGenericType = false;

        //if (File.Exists(path))
        //{
        //    content += ReadToEnd(path) + "\r\n";
        //    string[] strsTemp = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        //    if (strsTemp.Length != 0)
        //    {
        //        if (strsTemp[0] == "SimplyAndIsGenericType")
        //        {
        //            SimplyAndIsGenericType = true;
        //        }
        //        else
        //        {
        //            foreach (var t in strsTemp)
        //            {
        //                Type type = GetType(t);
        //                if (type != null)
        //                {
        //                    isLocalTestTest = true;
        //                    saveList.Add(type);
        //                }
        //            }
        //            return saveList;
        //        }
        //    }
        //}
        //if (File.Exists(path2))
        //{
        //    content += ReadToEnd(path2) + "\r\n";
        //}
        //if (Directory.Exists(path3))
        //{
        //    //content += ReadToEnd(path3) + "\r\n";
        //    content += GetLocalSelfDynamicContent();
        //}


        //string[] strs = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        ////List<Type> list = XLuaConfig.GetGameCustomType(isHotFix);
        //for (int i = 0; i < strs.Length; i++)
        //{
        //    string t = strs[i];
        //    //if (t == "System.String") continue;
        //    //if (t == "System.IO.Directory") continue;
        //    //if (t == "System.IO.DirectoryInfo") continue;
        //    //if (t == "System.Boolean") continue;
        //    //if (t == "UIRect") continue;
        //    ////if (t.Contains("`")) continue;
        //    ////if (t.Contains("+")) continue;
        //    //if (t == "TXVoiceInterfaceClient") continue;
        //    //if (!t.Contains("CSGameMgrBase")) continue;
        //    if (SimplyAndIsGenericType &&
        //        !t.CustomStartsWith("System.Collections.Generic.List") &&
        //        !t.CustomStartsWith("System.Collections.Generic.Dictionary") &&
        //        !t.CustomStartsWith("Map")) continue;
        //    if (t.Contains("+Enumerator") ||
        //        t.Contains("KeyValuePair`2") ||
        //        t.Contains("ParticleSystem"))
        //    {
        //        continue;
        //    }

        //    if (t == "zstring" ||
        //        t == "System.Boolean" ||
        //        t == "NetV2" ||
        //         t == "System.Convert")
        //    {
        //        continue;
        //    }
        //    //if (t == "System.Reflection.Module") continue;
        //    ////字典序里面用Array数组生成报错，如果List里面带Array也可能会报错，自行添加
        //    if (t.Contains("System.Collections.Generic.Dictionary`2") && t.Contains("[]"))
        //    {
        //        continue;
        //    }
        //    if (t.Contains("System.Collections.Generic.List`1") && t.Contains("[]"))
        //    {
        //        continue;
        //    }


        //    Type type = GetType(t);
        //    if (type != null)
        //    {
        //        if (GetClassNameBlackList().Contains(type.Name))
        //        {
        //            continue;
        //        }

        //        if (GetClassNameBlackList().Contains(type.FullName))
        //        {
        //            continue;
        //        }

        //        //if (type == typeof(System.Collections.Generic.Dictionary<int, int[]>)) continue;
        //        if (!saveList.Contains(type))
        //        {
        //            saveList.Add(type);
        //        }
        //    }
        //    else
        //    {
        //        type = GetAllProjectCacheAssemType(t);
        //        if (type != null)
        //        {
        //            if (type.Namespace != null)
        //            {
        //                if (type.Namespace.CustomStartsWith("UnityEngine") ||
        //                    (type.Namespace.CustomStartsWith("System") && !type.Namespace.CustomStartsWith("System.Collections.Generic")))
        //                {
        //                    UnityEngine.Debug.Log(t);
        //                    continue;
        //                }
        //            }
        //            //if (type == typeof(System.Collections.Generic.Dictionary<int, int[]>)) continue;
        //            if (!saveList.Contains(type))
        //            {
        //                saveList.Add(type);
        //            }
        //        }
        //        else
        //        {
        //            //if (!errorList.Contains("类型没有找到，请找下程序 = " + t))
        //            //{
        //            //    errorList.Add("类型没有找到，请找下程序 = " + t);
        //            //}
        //        }
        //    }
        //}

        ////string errorPath = FileUtility.GetBackDir(Application.dataPath, 1) + "/ProjectCaches/Error.txt";
        ////string errorContent = "";
        ////foreach (string cur in errorList)
        ////{
        ////    errorContent += cur + "\r\n";
        ////}
        ////FileUtility.Write(errorPath, errorContent, true);
        //errorList.Clear();
        return saveList;

    }

    public static string ReadToEnd(string path)
    {
        string content = "";
        if (!File.Exists(path))
        {
            return content;
        }

        using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fileStream, System.Text.Encoding.UTF8))
            {
                content = sr.ReadToEnd();
            }
        }
        return content;
    }

    private static void GetAllBlackCS(List<string> list, string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            return;
        }

        DirectoryInfo dir = new DirectoryInfo(dirPath);
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo f in files)
        {
            if (f.Extension != ".cs")
            {
                continue;
            }

            list.Add(f.Name.Replace(f.Extension, ""));
        }
        DirectoryInfo[] dirChilds = dir.GetDirectories();
        foreach (DirectoryInfo d in dirChilds)
        {
            GetAllBlackCS(list, d.FullName);
        }
    }

    public static bool IsInBlackTypeToMedthods(Type type, string s)
    {
        if (blackTypeToMedthods.ContainsKey(type.FullName))
        {
            if (blackTypeToMedthods[type.FullName].Contains(s))
            {
                return true;
            }
        }
        return false;
    }

    private static Type[] GetTypes()
    {
        List<Assembly> assList = GetAssemblys();
        List<Type> list = new List<Type>();
        foreach (Assembly a in assList)
        {
            list.AddRange(a.GetExportedTypes());
        }
        return list.ToArray();
    }

    public static Assembly GetAssembly(string typeName, List<Assembly> aList)
    {
        foreach (Assembly a in aList)
        {
            Type t = a.GetType(typeName);
            if (t != null)
            {
                return a;
            }
        }
        return null;
    }

    public static Assembly GetAssembly(string typeName)
    {
        List<Assembly> aList = GetAssemblys();
        foreach (Assembly a in aList)
        {
            Type t = a.GetType(typeName);
            if (t != null)
            {
                return a;
            }
        }
        return null;
    }

    public static Type GetType(string typeName)
    {
        List<Assembly> aList = GetAssemblys();
        foreach (Assembly a in aList)
        {
            Type t = a.GetType(typeName);
            if (t != null)
            {
                return t;
            }
        }
        return null;
    }

    public static Type GetAllProjectCacheAssemType(string typeName)
    {
        List<Assembly> aList = GetAllProjectCacheAssemblys();
        foreach (Assembly a in aList)
        {
            Type t = a.GetType(typeName);
            if (t != null)
            {
                return t;
            }
        }
        return null;
    }

    public static List<Type> GetTypesList()
    {
        List<Assembly> assList = GetAssemblys();
        List<Type> list = new List<Type>();
        foreach (Assembly a in assList)
        {
            list.AddRange(a.GetExportedTypes());
        }
        return list;
    }

    public static List<Assembly> GetAssemblys()
    {
        if (assemblysList.Count != 0)
        {
            return assemblysList;
        }

        Assembly a = Assembly.GetAssembly(typeof(CSGame));
        assemblysList.Add(a);
        
        return assemblysList;
    }
    //Library\PlayerDataCache\Data\Managed下有所有dll
    public static List<Assembly> GetAllProjectCacheAssemblys()
    {
        if (projectCacheAssemblysList.Count != 0)
        {
            return projectCacheAssemblysList;
        }

        //string allDllPath = FileUtility.GetBackDir(prePath, 1) + "/ProjectCaches";
        //string[] filePathList = Directory.GetFiles(allDllPath, "*.dll");
        //foreach (string cur in filePathList)
        //{
        //    Assembly a = Assembly.LoadFile(cur);
        //    if (a != null)
        //    {
        //        projectCacheAssemblysList.Add(a);
        //    }
        //}

        return projectCacheAssemblysList;
    }

    public static string GetLocalDynamicPath()
    {
        //string s = XLuaConfig.dynamichotSaveTxtPath + "DynamichotSaveTxtPath_" + CSMisc.GetIP().Replace(".", "_") + ".txt";
        //return s;
        return "";
    }

    public static string GetLocalSelfDynamicContent()
    {
        string content = ReadToEnd(GetLocalDynamicPath()) + "\r\n";
        return content;
    }

    public static string GetLocalDynamicContent(string excluderPath = "")
    {
        string content = "";
        //string[] dStrs = Directory.GetFiles(XLuaConfig.dynamichotSaveTxtPath, "*.txt");
        //foreach (string cur in dStrs)
        //{
        //    if (excluderPath == cur)
        //    {
        //        continue;
        //    }

        //    if (cur.Contains("DynamichotSaveTxtPath_"))
        //    {
        //        content += ReadToEnd(cur) + "\r\n";
        //    }
        //}
        return content;
    }

    public static void DeleteLocalDynamicContent(string excluderPath = "")
    {
        //string[] dStrs = Directory.GetFiles(XLuaConfig.dynamichotSaveTxtPath, "*.txt");
        //foreach (string cur in dStrs)
        //{
        //    if (excluderPath == cur)
        //    {
        //        continue;
        //    }

        //    if (cur.Contains("DynamichotSaveTxtPath_"))
        //    {
        //        if (File.Exists(cur))
        //        {
        //            File.Delete(cur);
        //        }
        //    }
        //}
    }
    private static List<string> hotfixBlackNameSpaceList = new List<string>()
    {
        "MikuLuaProfiler",
        "XLua",
        "ProtoOpti",
        "ProtoBuf",
        "Spine",
        "SharpJson",
        "GCCode",
        "LitJson",
        "YunvaVideoTroops",
        "mapEditor",
        "MapEditor",
        "TABLE",
        "XLua.TemplateEngine",
        "Spine.Unity",
        "Spine.Unity.Modules",
        "Spine.Unity.Modules.AttachmentTools",
        "UnityStandardAssets.ImageEffects",
        "XLua.Cast",
        "XLua.LuaDLL",
        "MiniJSON",
    };
    private static List<string> hotfixBlackTypeList = new List<string>
    {

    };
    public static List<Type> GetHotFixList(bool TypeIsGenWrap = false)
    {
        blackListIsDirty = true;
        List<Type> list = new List<Type>();
        Type[] types = GetTypes();
        //List<string> nguiList = new List<string>();
        //List<Type> nguiList2 = new List<Type>();
        //List<string> tempOnlyHotfixList = new List<string>();
        List<string> namespaceList = new List<string>();
        List<string> typeNameList = new List<string>();
        //GetAllBlackCS(tempOnlyHotfixList, prePath + "Script/UI");//UI大部分已经被移动到lua那边了
        for (int i = 0; i < types.Length; i++)
        {
            Type type = types[i];

            if (IsUnityEditor(types[i]) || IsUnityEditor(types[i].ReflectedType)) continue;
            if (type == typeof(XLuaConfig)) continue;
            if (type.Namespace != null)
            {
                if (hotfixBlackNameSpaceList.Contains(type.Namespace) ||
                    type.Namespace.CustomEndsWith("V2")||
                    type.Namespace.Contains("XLua."))
                    continue;
            }
            if (type.IsDefined(typeof(ProtoBuf.ProtoContractAttribute), false)) continue;
            if (type.Name.Contains("UIDebug")) continue;
            if (type.Name.Contains("VaryingList")) continue;
            if (type.BaseType != null && type.BaseType.Name.Contains("VaryingList")) continue;
            if (type.Name == "zstring") continue;

            if (GetClassNameBlackList().Contains(type.Name)) continue;
            if (GetClassNameBlackList().Contains(type.FullName)) continue;
            //if (type.IsSubclassOf(typeof(UIBase)))
            //{
            //    if (!TypeIsGenWrap)
            //    {
            //        continue;
            //    }
            //}

            //if (type.IsSubclassOf(typeof(UIRect))||
            //    type.IsSubclassOf(typeof(UIWidgetContainer))||
            //    type.IsSubclassOf(typeof(BehaviorState)))
            //    continue;

            if (type.IsEnum ||
                type.IsInterface || type.Name.Contains("`")
                || type.IsAbstract)
                continue;

            bool isCanAdd = true;
            //if (type.IsGenericType)//???
            //{
            //    if (type.BaseType == null)
            //    {
            //        isCanAdd = false;
            //    }
            //    else if (type.BaseType.IsGenericType)
            //    {
            //        isCanAdd = true;
            //    }
            //    else
            //    {
            //        isCanAdd = false;
            //    }
            //}
            if (isCanAdd)
            {
                list.Add(type);
            }
            list.Add(type);
            if (type.Namespace != null)
            {
                if (!namespaceList.Contains(type.Namespace))
                    namespaceList.Add(type.Namespace);
            }
            if (!typeNameList.Contains(type.FullName))
            {
                typeNameList.Add(type.FullName);
            }
        }
        //int index = 0;
        //string nameSpaceContent = "";
        //foreach(var cur in namespaceList)
        //{
        //    index++;
        //    nameSpaceContent += index + "." + cur+"\n";
        //}
        //UnityEngine.Debug.Log("nameSpaceContent= " + nameSpaceContent);
        //string typeName = "";
        //index = 0;
        //foreach (var cur in typeNameList)
        //{
        //    index++;
        //    typeName +="\"" + cur + "\",\n";
        //}
        //UnityEngine.Debug.Log("typeName" + index + "= " + typeName);
        return list;
    }
}


