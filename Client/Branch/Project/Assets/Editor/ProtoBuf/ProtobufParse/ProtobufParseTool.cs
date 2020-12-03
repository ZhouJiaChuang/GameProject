using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ProtobufParseTool : IEditorTool
{
    public string[] ToolbarTitles = { "客户端Proto", "服务器Proto" };

    private int ToolbarIndex = 0;

    public List<CSProtobufParseFolder> ParseFolderList = new List<CSProtobufParseFolder>();

    public string SerachTxt = string.Empty;

    public ProtobufParseTool()
    {
        ParseFolderList.Clear();

        ParseFolderList.Add(new CSProtobufParseFolder(CSEditorPath.Get(EEditorPath.LocalTableProtoPath), 
            CSEditorPath.Get(EEditorPath.LocalTableProtoClassPath), EProtobufParseFolderType.Table));

        ParseFolderList.Add(new CSProtobufParseFolder(CSEditorPath.Get(EEditorPath.LocalServerProtoPath), 
            CSEditorPath.Get(EEditorPath.LocalServerProtoClassPath), EProtobufParseFolderType.Server));
    }

    public void OnGUI()
    {
        EditorGUILayout.HelpBox("这个是Proto的转CS的路径,对应的路径需要在工程设置里面去配置", MessageType.Info);

        GUILayout.BeginHorizontal();
        {
            ToolbarIndex = GUILayout.Toolbar(ToolbarIndex, ToolbarTitles);
        }
        GUILayout.EndHorizontal();

        CSProtobufParseFolder ProtobufParseFolder;
        {
            ProtobufParseFolder = ParseFolderList[ToolbarIndex];
            ProtobufParseFolder.ShowOrigionFolder();
            ProtobufParseFolder.ShowOutFolder();
        }

        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("搜索:", GUILayout.Width(40));
            SerachTxt = EditorGUILayout.TextField(SerachTxt, GUILayout.Width(300));
            if (GUILayout.Button("刷新", GUILayout.Width(80)))
            {
                ProtobufParseFolder.RefushFolder();
            }
            if (GUILayout.Button("全部生成", GUILayout.Width(80)))
            {
                ProtobufParseFolder.GenerateAll();
            }

            GUILayout.Space(30);
        }
        GUILayout.EndHorizontal();

        ProtobufParseFolder.ShowFolderFileList(SerachTxt);
    }

    public void Save()
    {
    }

    public void Load()
    {
    }
}

public enum EProtobufParseFolderType
{
    Table,
    Server,
}

public class CSProtobufParseFolder
{
    /// <summary>
    /// 原始目录
    /// </summary>
    public string OrigionFolder = string.Empty;
    /// <summary>
    /// 输出目录
    /// </summary>
    public string OutFolder = string.Empty;

    CSBetterList<FileInfo> FileList = new CSBetterList<FileInfo>();

    EProtobufParseFolderType type = EProtobufParseFolderType.Table;

    public CSProtobufParseFolder(string OrigionFolder,string OutFolder, EProtobufParseFolderType type)
    {
        this.OrigionFolder = OrigionFolder;
        this.OutFolder = OutFolder;
        this.type = type;
        RefushFolder();
    }

    public void ShowOrigionFolder()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Proto存放路径:", GUILayout.Width(150));
            GUILayout.Label(OrigionFolder);
            SVNEditorUtility.SetSVNButton(OrigionFolder);
        }
        GUILayout.EndHorizontal();
    }

    public void ShowOutFolder()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Proto输出脚本路径:", GUILayout.Width(150));
            GUILayout.Label(OutFolder);
            SVNEditorUtility.SetSVNButton(OutFolder);
        }
        GUILayout.EndHorizontal();
    }


    public void RefushFolder()
    {
        DirectoryInfo directory = new DirectoryInfo(OrigionFolder);
        if (!directory.Exists) return;
        FileList.Clear();

        FileInfo[] files = directory.GetFiles("*.proto");
        for (int i = 0; i < files.Length; i++)
        {
            FileList.Add(files[i]);
        }
    }

    Vector2 pos = Vector2.zero;

    public void ShowFolderFileList(string serachTxt)
    {
        if (FileList == null) return;
        pos = EditorGUILayout.BeginScrollView(pos);
        {
            for (int i = 0; i < FileList.Count; i++)
            {
                if (string.IsNullOrEmpty(serachTxt) || FileList[i].Name.Contains(serachTxt))
                    ShowItemProtoFile(FileList[i]);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void ShowItemProtoFile(FileInfo file)
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("文件:", GUILayout.Width(40));
            GUILayout.Label(file.Name, GUILayout.Width(300));

            if (GUILayout.Button("Proto->Class", GUILayout.Width(120)))
            {
                GenerateProtoClass(file.FullName, file.Name.Replace(file.Extension, ""));
                AssetDatabase.Refresh();
            }
            if(type == EProtobufParseFolderType.Table)
            {
                if (GUILayout.Button("CSV->XLS->Bytes", GUILayout.Width(140)))
                {
                }
                if (GUILayout.Button("提交CSV/XLS/Bytes", GUILayout.Width(150)))
                {
                }

                if (GUILayout.Button("CSV->XLS->Proto->Class", GUILayout.Width(180)))
                {
                }
                if (GUILayout.Button("XLS->Bytes", GUILayout.Width(80)))
                {
                }
                if (GUILayout.Button("提交CSV/XLS/Proto/Bytes", GUILayout.Width(200)))
                {
                }
            }
        }
        GUILayout.EndHorizontal();
    }


    public void GenerateAll()
    {
        if (FileList == null) return;
        for (int i = 0; i < FileList.Count; i++)
        {
            FileInfo file = FileList[i];
            GenerateProtoClass(file.FullName, file.Name.Replace(file.Extension, ""));
        }
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 生成proto对应的C#代码
    /// </summary>
    /// <param name="fileFullPath"></param>
    public void GenerateProtoClass(string fileFullPath,string fileName)
    {
        Directory.SetCurrentDirectory(OrigionFolder);

        try
        {
            if (ProcessProto(fileName) == false)
            {
                Directory.SetCurrentDirectory(URL.LocalProjectPath);
                return;
            }
            else
            {
                AddPropertyToTableCS(fileFullPath);
            }

            Directory.SetCurrentDirectory(URL.LocalProjectPath);

            UnityEngine.Debug.Log("卐  打前端【Proto】文件 Success   卍");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex);
            Directory.SetCurrentDirectory(URL.LocalProjectPath);
        }
    }

    /// <summary>
    /// 调用protogen.exe指向命令,将proto生成cs脚本
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private bool ProcessProto(string name)
    {
        string param = string.Format("-i:{0}.proto -o:{0}.cs -p:detectMissing", name);
        if (CallProcess("protogen.exe", param))
        {
            File.Copy(@".\" + name + ".cs", OutFolder + name + ".cs", true);
            File.Delete(@".\" + name + ".cs");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 呼叫流程
    /// </summary>
    /// <param name="processName"></param>
    /// <param name="param"></param>
    /// <returns>成功true</returns>
    protected static bool CallProcess(string processName, string param)
    {
        ProcessStartInfo process = new ProcessStartInfo
        {
            CreateNoWindow = false,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            FileName = processName,
            Arguments = param,
        };

        UnityEngine.Debug.Log(processName + " " + param);

        Process p = Process.Start(process);
        p.WaitForExit();

        string error = p.StandardError.ReadToEnd();
        if (!string.IsNullOrEmpty(error))
        {
            UnityEngine.Debug.LogError(processName + " " + param + "  ERROR! " + "\n" + error);

            string output = p.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(output))
            {
                UnityEngine.Debug.Log(output);
            }

            return false;
        }
        return true;
    }

    public void AddPropertyToTableCS(string protoFilePath)
    {
        string protoFileName = Path.GetFileName(protoFilePath);
        string className = protoFileName.Replace("c_table_", "").Replace(".proto", "");
        string csFileName = protoFileName.Replace("proto", "cs");

        string csFilePath = OutFolder + csFileName;

        string replaceStr = "";

        //读取proto文件，解析要添加的属性
        using (StreamReader protoReader = new StreamReader(protoFilePath))
        {
            string saveProperty = "";
            string propertyType = "";
            while (!protoReader.EndOfStream)
            {
                string parseStr = protoReader.ReadLine();
                if (!parseStr.Contains("@"))
                {
                    continue;
                }

                parseStr = System.Text.RegularExpressions.Regex.Replace(parseStr, @"\s+", " ").Trim(' ');
                parseStr = parseStr.Remove(0, parseStr.IndexOf(' ') + 1);
                CSDebug.Log(parseStr);
                if (!string.IsNullOrEmpty(parseStr))
                {
                    saveProperty = "_" + parseStr.Replace("#", "").Replace(" ", "_");
                    propertyType = "uint";

                    string[] newPropertys = parseStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int beginPlace = 0;
                    string newProperty = "";
                    for (int i = newPropertys.Length - 1; i >= 0; i--)
                    {
                        newProperty = newPropertys[i];

                        string[] newPropertyParams = newProperty.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                        if (newPropertyParams.Length == 2)
                        {
                            replaceStr += string.Format("    public {0} {1} {{ {2} }}\r\n", propertyType, newPropertyParams[0],
                                GetGetMethodStr2(saveProperty, newPropertyParams[1], ref beginPlace));
                        }
                        else
                        {
                            CSDebug.LogError(string.Format("{0} {0}, 书写错误", protoFilePath, newProperty));
                            continue;
                        }
                    }
                }
            }
            protoReader.Close();
        }

        if (string.IsNullOrEmpty(replaceStr))
        {
            return;
        }

        //读取cs文件
        using (StreamReader csReader = new StreamReader(csFilePath))
        {
            string csText = csReader.ReadToEnd();
            csReader.Close();

            //添加新属性
            string constructorMethod = string.Format("public {0}() {{}}", className.ToUpper());

            if (csText.Contains(constructorMethod))
            {
                csText = csText.Replace(constructorMethod, constructorMethod + "\r\n" + replaceStr.TrimEnd('\r', '\n'));
            }

            using (StreamWriter csWriter = new StreamWriter(csFilePath))
            {
                csWriter.Write(csText);
                csWriter.Flush();
                csWriter.Close();
            }
        }
    }

    private string GetGetMethodStr2(string saveProperty, string placeCount, ref int beginPlace)
    {
        int count = 0;
        if (beginPlace >= 0 && int.TryParse(placeCount, out count) && count > 0)
        {
            string result = string.Format("get {{ return {0}{1}{2}; }}", saveProperty, beginPlace > 0 ? string.Format(" >> {0}", beginPlace) : "",
                string.Format(" & {0}", (int)Mathf.Pow(2, count) - 1));
            beginPlace = beginPlace + count;
            return result;
        }
        return "";
    }
}
