using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class XLuaManager : SingletonMono<XLuaManager>
{
    [XLua.CSharpCallLua]
    public delegate void XLuaMgr_Init();

    private LuaEnv luaenv;
    public LuaEnv Luaenv
    {
        get { return luaenv; }
        set { luaenv = value; }
    }

    /// <summary>
    /// lua的GameMgr的Update
    /// </summary>
    public Action luaGameMgrUpdate = null;

    private static string _LuaRootFolder;
    /// <summary>
    /// Lua文件的根目录
    /// </summary>
    public static string LuaRootFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_LuaRootFolder))
            {
                if (CSGameState.RunPlatform == ERunPlatform.Editor)
                {
                    _LuaRootFolder = CSEditorPath.Get(EEditorPath.LuaPath);
                }
                else
                {
                    _LuaRootFolder = Application.persistentDataPath + "/luaRes/";
                }
                _LuaRootFolder = _LuaRootFolder.Replace("/", "\\");
            }
            return _LuaRootFolder;
        }
    }


    private string mainPath = string.Empty;

    public void Init()
    {
        CSDebug.Log("XLua开始进行初始化");

        mainPath = LuaRootFolder + "main_out.lua";

        Luaenv = new LuaEnv();

        luaenv.DoString(LuaEncryptLoader(ref mainPath), "LuaTestScript", luaenv.Global);

        XLuaMgr_Init f = luaenv.Global.GetInPath<XLuaMgr_Init>("InitRequire");
        if (f != null)
            f();
    }

    private void Update()
    {
        if (luaGameMgrUpdate != null)
            luaGameMgrUpdate();
    }

    /// <summary>
    /// lua加密加载
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    private byte[] LuaEncryptLoader(ref string filepath)
    {
        try
        {
            byte[] fileContent = File.ReadAllBytes(mainPath);
            return fileContent;
        }
        catch (Exception ex)
        {
            CSDebug.LogError(ex.Message);
            return null;
        }
    }

    #region lua的携程
    public void YieldAndCallback(object to_yield, Action callback)
    {
        StartCoroutine(CoBody(to_yield, callback));
    }

    private IEnumerator CoBody(object to_yield, Action callback)
    {
        if (to_yield is IEnumerator)
            yield return StartCoroutine((IEnumerator)to_yield);
        else
            yield return to_yield;
        callback();
    }
    #endregion
}
