/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;
using System;
using System.IO;

[System.Serializable]
public class Injection
{
    public string name;
    public GameObject value;
}

[LuaCallCSharp]
public class LuaBehaviour : MonoBehaviour
{
    public Injection[] injections;

    internal static LuaEnv luaEnv = new LuaEnv(); //all lua behaviour shared one luaenv only!
    internal static float lastGCTime = 0;
    internal const float GCInterval = 1;//1 second 

    private Action luaStart;
    private Action luaUpdate;
    private Action luaOnDestroy;

    private LuaTable scriptEnv;
    private string luaName;
    private LuaTable chunkTable;

    /// <summary>
    /// 传入lua文件路径进行初始化
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="luaName"></param>
    public void InitWithFilePath(string filePath, string luaName)
    {
        string fullPath = XLuaManager.LuaRootFolder + filePath;
        //UnityEngine.Debug.Log(fullPath);
        if (!File.Exists(fullPath))
        {
            return;
        }
        string content = File.ReadAllText(fullPath);
        Init(content, luaName);
    }

    public void Init(string luaScript)
    {
        Init(luaScript, "");
    }

    public void Init(string luaScript, string luaName)
    {
        //UnityEngine.Debug.Log("luaName = " + luaName);
        this.luaName = luaName;
        LuaEnv luaEnv = XLuaManager.Instance.Luaenv;
        scriptEnv = luaEnv.NewTable();

        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);
        object[] objs;
        if (string.IsNullOrEmpty(luaName))
        {
            objs = luaEnv.DoString(luaScript, "LuaBehaviour", scriptEnv);
        }
        else
        {
            objs = luaEnv.DoString(luaScript, luaName, scriptEnv);
        }
        if (objs != null && objs.Length > 0)
        {
            chunkTable = objs[0] as LuaTable;
        }
        else
        {
            chunkTable = null;
        }

        Action luaAwake = scriptEnv.Get<Action>("awake");
        scriptEnv.Get("start", out luaStart);
        scriptEnv.Get("update", out luaUpdate);
        scriptEnv.Get("ondestroy", out luaOnDestroy);

        if (luaAwake != null)
        {
            luaAwake();
        }
    }

    public LuaTable GetLuaTable()
    {
        return chunkTable;
    }

    // Use this for initialization
    void Start()
    {
        if (luaStart != null)
        {
            luaStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (luaUpdate != null)
        {
            luaUpdate();
        }
        if (Time.time - LuaBehaviour.lastGCTime > GCInterval)
        {
            luaEnv.Tick();
            LuaBehaviour.lastGCTime = Time.time;
        }
    }

    void OnDestroy()
    {
        if (luaOnDestroy != null)
        {
            luaOnDestroy();
        }
        luaOnDestroy = null;
        luaUpdate = null;
        luaStart = null;
        scriptEnv.Dispose();
        injections = null;
    }
}
