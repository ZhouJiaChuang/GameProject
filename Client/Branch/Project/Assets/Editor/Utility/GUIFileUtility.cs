using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

//脚本OpenFileName
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public class LocalDialog
{

    //链接指定系统函数       打开文件对话框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    public static bool GetOFN([In, Out] OpenFileName ofn)
    {
        return GetOpenFileName(ofn);
    }

    //链接指定系统函数        另存为对话框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
    public static bool GetSFN([In, Out] OpenFileName ofn)
    {
        return GetSaveFileName(ofn);
    }
}

public static class FileManager
{
    private static OpenFileName OpenFileManager(string str, string extend)
    {
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        //文件类型 config配置文件,"Excel文件(*.xlsx)\0*.xlsx" ,"Txt文件(*.txt)\0*.txt"...
        openFileName.filter = string.Format("{0}文件(*{1})\0*{2}", extend, extend, extend);
        openFileName.file = new string(new char[256]);//new一个256字符的string
        openFileName.maxFile = openFileName.file.Length;//获取256字符的string的长度作为最大
        openFileName.fileTitle = new string(new char[64]);//64字符的string
        openFileName.maxFileTitle = openFileName.fileTitle.Length;//文件标题的最大长度
        openFileName.initialDir = UnityEngine.Application.dataPath;//默认路径
        openFileName.title = str;//文件标题
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        return openFileName;
    }

    public static void OpenFile(string extend, System.Action<string> action)
    {
        OpenFileName openFileName = OpenFileManager("OpenFile", extend);
        if (LocalDialog.GetOpenFileName(openFileName))
        {
            if (action != null)
                action(openFileName.file);
        }
    }

    public static void SaveFile(string extend, System.Action<string> action)
    {
        OpenFileName openFileName = OpenFileManager("SaveFile", extend);
        if (LocalDialog.GetSaveFileName(openFileName))
        {
            if (action != null)
                action(openFileName.file);
        }
    }

    public static string LoadFileContent(string filePath)
    {
        string xmlContent = "";
        if (!File.Exists(filePath)) return xmlContent;
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            xmlContent = Encoding.UTF8.GetString(bytes);

            stream.Close();
        }
        return xmlContent;
    }

    public static void SaveFileContent(string filePath, string content)
    {
        if (!string.IsNullOrEmpty(content))
        {
            FileInfo file = new FileInfo(filePath);
            if (!file.Directory.Exists)
            {
                Directory.CreateDirectory(file.Directory.FullName);
            }

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                byte[] myByte = System.Text.Encoding.UTF8.GetBytes(content);
                stream.Write(myByte, 0, (int)myByte.Length);
                stream.Flush();
                stream.Close();
            }
        }
    }
}