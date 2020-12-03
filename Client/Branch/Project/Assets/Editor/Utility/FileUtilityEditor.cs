using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using Object = UnityEngine.Object;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ExtendEditor
{
    public static class FileUtilityEditor
    {
        /// <summary>
        /// path can be a object,as xxx/x/x/x.jpg,relatively Application.dataPath
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void GetDeepAssetPaths(string path, List<string> list, string extension = "",
            bool isFullPath = false,bool isIncludeMeta = false,bool isOneLevel = false,string containDirPath = "")
        {
           
            if (Directory.Exists(path))
            {
                if (path.Contains(@".svn")) return;
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = new FileInfo[0];
                if (string.IsNullOrEmpty(extension))
                    files = dir.GetFiles();
                else
                    files = dir.GetFiles("*" + extension);
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo file = files[i];
                    if (!isIncludeMeta&&file.Extension == ".meta") continue;
                    string filePath = GetAssetPath(file.FullName,isFullPath);
                    //Debug.LogError(filePath);
                    if (!string.IsNullOrEmpty(containDirPath))
                    {
                        if (!filePath.Contains(containDirPath)) continue;
                    }
                    if (!list.Contains(filePath))
                    list.Add(filePath);
                }
                if (isOneLevel) return;
                DirectoryInfo[] dirChild = dir.GetDirectories();
                for (int i = 0; i < dirChild.Length; i++)
                {
                    GetDeepAssetPaths(dirChild[i].FullName, list, extension,isFullPath, isIncludeMeta, false, containDirPath);
                }
            }

            if (File.Exists(path))
            {
                string filePath = GetAssetPath(path,isFullPath);
                if(!list.Contains(filePath))
                list.Add(filePath);
            }
        }

        /// <summary>
        /// get all directories by the path,relatively Application.dataPath
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dirList"></param>
        /// <param name="extension"></param>
        public static void GetDeepAssetDirs(string path, List<string> dirList)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                string dirPathRelatively = GetAssetPath(path);
                if (!dirList.Contains(dirPathRelatively))
                    dirList.Add(dirPathRelatively);
                DirectoryInfo[] dirChild = dir.GetDirectories();
                for (int i = 0; i < dirChild.Length; i++)
                {
                    GetDeepAssetDirs(dirChild[i].FullName, dirList);
                }
            }
            if (File.Exists(path))
            {
                GetDeepAssetDirs(path + "/../", dirList);
            }
        }

        public static void GetSelectAssetPaths(List<string> list)
        {
            Object[] mSelectObjs = FileUtilityEditor.GetFiltered();
            for (int i = 0; i < mSelectObjs.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(mSelectObjs[i]);
                string metaPath = path + ".meta";
                list.Add(path);
                if (!metaPath.Contains("/CB/"))
                {
                    if(File.Exists(metaPath))
                    {
                        list.Add(metaPath);
                    }
                }
                UnityEngine.Debug.Log(path);
               
            }
        }


        public static bool IsFileExist(string filePath, bool isIgnoreExtension = false)
        {
            if (!isIgnoreExtension)
            {
                return File.Exists(filePath);
            }
            string dirPath = filePath + "/../";
            int starIndex = filePath.LastIndexOf("/")+1;
            int length = filePath.LastIndexOf(".") - starIndex;
            string fileName = filePath.Substring(starIndex, length);
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo info = new DirectoryInfo(dirPath);
                foreach (FileInfo fileInfo in info.GetFiles())
                {
                    if (fileInfo.Extension.Contains("meta")) continue;
                    string fName = fileInfo.Name.Substring(0,fileInfo.Name.LastIndexOf("."));
                    if (fName==fileName)
                    {
                        //Debug.Log("fileInfo = "+fileInfo.FullName);
                        return true;
                    }
                    //Debug.Log(fileInfo.Name);
                    //if(fileInfo.Name)
                }
            }
            return false;
        }
        /// <summary>
        /// Application.dataPath
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <returns></returns>
        public static string GetAssetPath(string fileFullPath, bool isFullPath = false)
        {
            //Debug.LogError(fileFullPath);
            if (!isFullPath)
            {
                return "Assets" + fileFullPath.Replace("\\", "/").Replace(Application.dataPath, "");
            }
            else
            {
                return fileFullPath.Replace("\\", "/");
            }
        }

        public static string GetAssetFullPath(string assetPath)
        {
            return Application.dataPath.Replace("Assets","") + assetPath;
        }

        public static Object GetObject(string filePath)
        {
            Object obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
            return obj;
        }

        /// <summary>
        /// cur select object's directory
        /// </summary>
        /// <returns></returns>
        public static string GetSelectDirectory()
        {
            string path = "Assets";
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }
                break;
            }
            return path;
        }

        public static string GetBackDir(string dir,int time)
        {
            dir = dir.Replace("\\", "/");
            for (int i=0;i<time;i++)
            {
                int lastIndex = dir.LastIndexOf("/");
                if (lastIndex == dir.Length - 1)
                {
                    dir = dir.Substring(0, dir.Length - 1);
                }
                lastIndex = dir.LastIndexOf("/");
                dir = dir.Substring(0, lastIndex);
            }
            return dir;
        }

        public static string GetDirectory(UnityEngine.Object obj)
        {
            string path = "Assets";
            path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            return path;
        }

        public static string GetDirectory(string path)
        {
            path = path.Replace("\\", "/");
            int lastIndex = path.LastIndexOf("/");
            if (lastIndex == -1) return "";

            path = path.Substring(0, lastIndex);
            return path;
        }

        public static string GetPathWithExtend(string path)
        {
            int index = path.LastIndexOf(".");
            if (index == -1) return path;
            return path.Substring(0, index);
        }

        public static string GetFileName(string path)
        {
            path = path.Replace("\\", "/");
            string dir = GetDirectory(path);
            path = path.Replace(dir+"/", "");
            path = path.Substring(0, path.LastIndexOf("."));
            return path;
        }

        public static string GetABFileName(string path)
        {
            path = path.Replace("\\", "/");
            string dir = GetDirectory(path);
            path = path.Replace(dir + "/", "");
            return path;
        }

        public static string GetFileWithoutExtension(string path)
        {
            string dir = GetDirectory(path);
            path = path.Substring(0, path.LastIndexOf("."));
            return path;
        }

        public static void Rename(string path, string newPath)
        {
            if (File.Exists(path))
            {
                File.Move(path, newPath);
            }
            else
            {
                UnityEngine.Debug.LogError(path + " is Not exist");
            }
        }

        /// <summary>
        /// File + Directory(not deep asset)
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Object[] GetFiltered(SelectionMode mode = SelectionMode.Assets)
        {
            return Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        }

        public static List<string> GetSelectAllFiles()
        {
            Object[] mSelectObjs = FileUtilityEditor.GetFiltered();
            List<string> mPathList = new List<string>();
            foreach (Object obj in mSelectObjs)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                FileUtilityEditor.GetDeepAssetPaths(path, mPathList);
            }
            return mPathList;
        }

        //public static bool FileIsChange(string path,string md5Path)
        //{
        //    string saveContent = ReadToEnd(md5Path);
        //    UnityEngine.Debug.Log("saveContent = " + saveContent);
        //    string Md5 = MD5Utility.GetMD5HashFromFile(path);
        //    UnityEngine.Debug.Log("Md5 = " + Md5);
        //    return saveContent != Md5;
        //}

        //public static void SaveMD5ToFile(string path,string md5Path)
        //{
        //    string Md5 = MD5Utility.GetMD5HashFromFile(path);
        //    Write(md5Path, Md5, false);
        //}

        //public static void DetectCreateDirectory(string path)
        //{
        //    int index = path.LastIndexOf(".");
        //    if (index == -1 || path.Substring(index).Contains("/"))
        //    {
        //        if (!Directory.Exists(path))
        //            Directory.CreateDirectory(path);
        //    }
        //    else
        //    {
        //        //if (!Directory.Exists(path + "/../"))
        //        //    Directory.CreateDirectory(path + "/../");
        //        string dirName = Path.GetDirectoryName(path);
        //        if (!Directory.Exists(dirName))
        //            Directory.CreateDirectory(dirName);
        //    }
        //}

        /// <summary>
        /// 创建该文件/文件夹所在目录
        /// </summary>
        /// <param name="path"></param>
        public static void DetectCreateDirectory(string path)
        {
            int index = path.LastIndexOf(".");
            if (index == -1 || path.Substring(index).Contains("/"))
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            else
            {
                if (!Directory.Exists(path + "/../"))
                    Directory.CreateDirectory(path + "/../");
            }
        }

        public static void DetectCreateFile(string path)
        {
            DetectCreateDirectory(path);
            if (!File.Exists(path))
            {
                File.Create(path);
            }
        }

        public static string ReadToEnd(string path)
        {
            string content = "";
            if (!File.Exists(path)) return content;
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate,FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fileStream, System.Text.Encoding.UTF8))
                {
                    content = sr.ReadToEnd();
                }
            }
            return content;
        }

        public static byte[] ReadBytes(string path)
        {
            if (!File.Exists(path)) return null;
            byte[] buffer = null;
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
            {
                buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
            }
            return buffer;
        }

        public static List<Assembly> GetAllAssembly()
        {
            List<Assembly> list = new List<Assembly>();
            //string pre = Application.dataPath+"/Plugins/Dll/";
            //List<string> dllList = new List<string>();
            //dllList.Add(pre + "Assets.dll");
            ////dllList.Add(pre + "AssetsEditor.dll");
            //dllList.Add(pre + "CB_Base.dll");
            //dllList.Add(pre + "CB_PublicCS.dll");
            //dllList.Add(pre + "Proto.dll");
            //foreach (var cur in dllList)
            //{
            //    if(File.Exists(cur))
            //    {
            //        Assembly a = Assembly.LoadFrom(cur);
            //        list.Add(a);
            //    }
            //}

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var cur in assemblies)
            {
                try
                {
                    //UnityEngine.Debug.Log("Find dll = " + cur.Location);
                    //if (cur.Location.EndsWith("Assembly-CSharp.dll"))
                    //{
                    //    UnityEngine.Debug.Log("Find Assembly-CSharp.dll = " + cur.Location);
                    //    Assembly a = list.Find(p => p.Location == cur.Location);
                    //    if (a == null)
                    //    {
                    //        list.Add(cur);
                    //    }
                    //    else
                    //    {
                    //        UnityEngine.Debug.LogError("找到重复 = " + a.Location);
                    //    }
                    //}
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.Log("无法获得Location参数 = " + cur.ToString());
                }
            }
            return list;
        }

        public static void Write(string path, string content, bool append = true)
        {
            string old = ReadToEnd(path);
            DetectCreateDirectory(path);
            var utf8WithoutBom = new System.Text.UTF8Encoding(false);
            using (StreamWriter sw = new StreamWriter(path, append, utf8WithoutBom))
            {
                sw.Write(content);
            }
        }

        public static void UseCommandUploadFileToSVN(string dir)
        {
            // 路径不存在直接返回
            if (!Directory.Exists(dir))
            {
                UnityEngine.Debug.LogError("路径不存在直接返回 = " + dir);
                return;
            }
            Process pp = Process.Start("TortoiseProc.exe", @"/command:commit /path:" + dir + " /closeonend:1");
            pp.WaitForExit();
        }


        public static void CopyText(string s)
        {
            TextEditor te = new TextEditor();
            te.content = new GUIContent(s);
            te.OnFocus();
            te.Copy();
        }

        public static void CopyFile()
        {
            UnityEngine.Object[] objs = GetFiltered();
            if (objs.Length > 0)
            {
                string path = AssetDatabase.GetAssetPath(objs[0]);
                CopyText(path);
                UnityEngine.Debug.Log("复制" + path);
            }
        }

        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static System.Diagnostics.Process Open(string path)
        {
            System.Diagnostics.Process pp = System.Diagnostics.Process.Start(path);
            return pp;
        }

        /// <summary>
        /// 相对Assets/
        /// </summary>
        /// <param name="path"></param>
        /// <param name="?"></param>
        /// <returns></returns>lastModityTime
        public static bool IsFileChange(string path, bool isDetectDependenice)
        {
            string txtPath = Application.dataPath.Replace("Assets", "") + path.Replace("Assets", "AssetsChangeDetect");
            txtPath = txtPath.Substring(0, txtPath.LastIndexOf(".")) + ".txt";
            string txt = ReadToEnd(txtPath);
            if (string.IsNullOrEmpty(txt)) return true;
            Dictionary<string, DateTime> dataDic = new Dictionary<string, DateTime>();
            string[] ts = txt.Split('\n', '\r');
            for (int i = 0; i < ts.Length; i++)
            {
                string t = ts[i];
                string[] data = t.Split('#');
                if (data.Length != 2) continue;
                string assetPath = data[0];
                string lastModityTime = data[1];
                FileInfo fileInfo = new FileInfo(Application.dataPath.Replace("Assets", "") + assetPath);
                DateTime lastDt = DateTime.Parse(lastModityTime);
                DateTime dt = fileInfo.LastWriteTime;

                if (assetPath == path)
                {
                    TimeSpan tts = dt - lastDt;
                    long delta = (long)tts.TotalSeconds;
                    if (delta >= 1) return true;
                }
                dataDic.Add(assetPath, lastDt);
            }

            if (isDetectDependenice)
            {
                List<UnityEngine.Object> list = new List<UnityEngine.Object>();
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                UnityEngine.Object[] dependObjs = EditorUtility.CollectDependencies(new UnityEngine.Object[1] { obj });
                if (dataDic.Count != dependObjs.Length*2) return true;//meta文件
                for (int i = 0; i < dependObjs.Length; i++)
                {
                    UnityEngine.Object o = dependObjs[i];
                    string oPath = AssetDatabase.GetAssetPath(o);
                    if (!dataDic.ContainsKey(oPath)) return true;
                    DateTime lastDt = dataDic[oPath];
                    FileInfo fileInfo = new FileInfo(Application.dataPath.Replace("Assets", "") + oPath);
                    DateTime dt = fileInfo.LastWriteTime;
                    TimeSpan tts = dt - lastDt;
                    long delta = (long)tts.TotalSeconds;
                    if (delta >=1) return true;

                    string metaPath = oPath + ".meta";
                    if (!dataDic.ContainsKey(metaPath)) return true;
                    lastDt = dataDic[metaPath];
                    fileInfo = new FileInfo(Application.dataPath.Replace("Assets", "") + metaPath);
                    dt = fileInfo.LastWriteTime;
                    tts = dt - lastDt;
                    delta = (long)tts.TotalSeconds;
                    if (delta >= 1) return true;
                }
            }
            return false;
        }

        public static void WriteFileChange(string path)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            UnityEngine.Object[] dependObjs = EditorUtility.CollectDependencies(new UnityEngine.Object[1] { obj });
            List<UnityEngine.Object> list = new List<UnityEngine.Object>();
            list.AddRange(dependObjs);
            string txt = "";
            for (int i = 0; i < list.Count; i++)
            {
                string p = AssetDatabase.GetAssetPath(list[0]);
                string fullP = Application.dataPath.Replace("Assets", "") + p;
                FileInfo f = new FileInfo(fullP);
                txt += p + "#" + f.LastWriteTime.ToString()+"\r\n";

                p = p+".meta";
                fullP = Application.dataPath.Replace("Assets", "") + p;
                f = new FileInfo(fullP);
                txt += p + "#" + f.LastWriteTime.ToString()+"\r\n";
            }
            string txtPath = Application.dataPath.Replace("Assets", "") + path.Replace("Assets", "AssetsChangeDetect");
            txtPath = txtPath.Substring(0, txtPath.LastIndexOf(".")) + ".txt";
            //UnityEngine.Debug.LogError(txtPath);
            Write(txtPath, txt, false);
        }

		public static Int64 GetFileSize(string path)
		{
			try
			{
				FileStream file = new FileStream(path, FileMode.Open);
				return file.Length;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(string.Format("GetFileSize error {0}", ex.Message));
			}
			
			return 0;
		}
        public static void DeleteDir(string dir,string extend = "",bool isDeleteDir = false,bool isSync = false)
        {
           
            if (!Directory.Exists(dir))
            {
                UnityEngine.Debug.LogError("dir is not exist = " + dir);
                return;
            }
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (isSync)
                EditorUtility.DisplayProgressBar("正在DeleteDir文件...", dirInfo.Name, 1);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (var file in files)
            {
                if(string.IsNullOrEmpty(extend)||file.FullName.EndsWith(extend))
                {
                    File.Delete(file.FullName);
                }
            }
            DirectoryInfo[] childDir = dirInfo.GetDirectories();
            foreach(var cur in childDir)
            {
                DeleteDir(cur.FullName,extend, isDeleteDir, isSync);//子文件夹不用传入isDeleteDir
            }
            if(isDeleteDir)
            {
                Directory.Delete(dir, true);
            }
        }

        //全字匹配
        public static bool IsFindWholeWorld(string content,string wordToFind, RegexOptions op)
        {
            string pattern = String.Format(@"\b{0}\b", wordToFind);
            return Regex.IsMatch(content, pattern,op);
        }
        //全字匹配
        public static int FindWholeWorldCount(string content, string wordToFind, RegexOptions op)
        {
            string pattern = String.Format(@"\b{0}\b", wordToFind);
            MatchCollection mc =  Regex.Matches(content, pattern, op);
            if(mc.Count>0)
            {
                return mc.Count;
            }
            return 0;
        }

        public static Dictionary<uint, T> GetCfgData<T,TArray>(string cfgName)
        {
            string cfgMapPath = CSEditorPath.Get(EEditorPath.LocalResourcesLoadPath) + "Table/" + cfgName + ".bytes";
            Dictionary<uint, T> dic = new Dictionary<uint, T>();
            TArray list = default(TArray);
            byte[] cfgData = File.ReadAllBytes(cfgMapPath);
            using (MemoryStream stream = new MemoryStream(cfgData))
            {
                list = ProtoBuf.Serializer.Deserialize<TArray>(stream);
            }
            if (list == null) return dic;
            Type arrayType = list.GetType();

            //foreach (var cur in list.rows)
            //{
            //    if (!dic.ContainsKey(cur.id))
            //    {
            //        dic.Add(cur.id, cur);
            //    }
            //}
            return dic;
        }

        public delegate bool CopyDirCheckCallBack(string dirPath);
        public static void CopyDir(string dir, string dstDirPath,string extend = "",
            List<string> excludePathList = null,
            bool copySync = false,bool isOverWrite = false, CopyDirCheckCallBack checkCallBack = null)
        {
            if (dir.EndsWith(".svn")) return;
            if (!Directory.Exists(dir))
            {
                UnityEngine.Debug.LogError("dir is not exist = " + dir);
                return;
            }
            if(checkCallBack!=null)
            {
                if (!checkCallBack(dir)) return;
            }
            if (excludePathList != null)
            {
                string d = dir.Replace("\\", "/");
                foreach (var p in excludePathList)
                {
                    if (d.Contains(p)) return;
                }
            }
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (copySync)
                EditorUtility.DisplayProgressBar("正在CopyDir文件...", dirInfo.Name, 1);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (var file in files)
            {
                if (string.IsNullOrEmpty(extend) || file.FullName.EndsWith(extend))
                {
                    string dstFilePath = dstDirPath +"/"+file.Name;
                    if(string.IsNullOrEmpty(file.Extension))
                    {
                        string bDir = GetBackDir(dstFilePath,1);
                        DetectCreateDirectory(bDir);
                    }
                    else
                    {
                        DetectCreateDirectory(dstFilePath);
                    }

                    File.Copy(file.FullName, dstFilePath, isOverWrite);
                }
            }
            if (copySync) EditorUtility.ClearProgressBar();
            DirectoryInfo[] childDir = dirInfo.GetDirectories();
            foreach (var cur in childDir)
            {
                CopyDir(cur.FullName, dstDirPath+"/"+cur.Name,
                    extend, excludePathList, copySync,isOverWrite, checkCallBack);
            }
        }

        public delegate void LoadABCallBack(AssetBundle ab, UnityEngine.Object obj);
        public static UnityEngine.Object LoadAssetbundle(string path, LoadABCallBack callBack=null, bool isUnloadAb = false)
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            UnityEngine.Object obj = null;
            if (assetBundle != null)
            {
                
                string[] strs = assetBundle.GetAllAssetNames();
                if (strs.Length > 0)
                {
                    obj = assetBundle.LoadAsset(strs[0]);
                }
                if (callBack != null)
                {
                    callBack(assetBundle, obj);
                }
                assetBundle.Unload(isUnloadAb);
            }
            return obj;
        }

        public static Dictionary<string, string> GetAllCSharpLuaContent()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string cPath = Application.dataPath;
            List<string> fileList = new List<string>();
            GetDeepAssetPaths(cPath, fileList, ".cs",true);
            string luaPath = GetBackDir(Application.dataPath, 1) +
                "/luaRes";
            GetDeepAssetPaths(luaPath, fileList, ".lua",true);
            foreach (var cur in fileList)
            {
                string content = File.ReadAllText(cur);
                dic.Add(cur, content);
            }
            return dic;
        }

        public static Dictionary<string, string> GetAllCSharpContent()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string cPath = Application.dataPath;
            List<string> fileList = new List<string>();
            GetDeepAssetPaths(cPath, fileList, ".cs", true);
            foreach (var cur in fileList)
            {
                string content = File.ReadAllText(cur);
                dic.Add(cur, content);
            }
            return dic;
        }

        public static Dictionary<string, string> GetAllLuaContent()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string cPath = Application.dataPath;
            List<string> fileList = new List<string>();
            string luaPath = GetBackDir(Application.dataPath, 1) +
                             "/luaRes";
            GetDeepAssetPaths(luaPath, fileList, ".lua", true);
            foreach (var cur in fileList)
            {
                string content = File.ReadAllText(cur);
                dic.Add(cur, content);
            }
            return dic;
        }

    }
}

public class FileStatus
{
    [DllImport("kernel32.dll")]
    private static extern IntPtr _lopen(string lpPathName, int iReadWrite);
    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject);
    private const int OF_READWRITE = 2;
    private const int OF_SHARE_DENY_NONE = 0x40;
    private static readonly IntPtr HFILE_ERROR = new IntPtr(-1);
    public static int FileIsOpen(string fileFullName)
    {
        if (!File.Exists(fileFullName))
        {
            return -1;
        }
        IntPtr handle = _lopen(fileFullName, OF_READWRITE | OF_SHARE_DENY_NONE);
        if (handle == HFILE_ERROR)
        {
            return 1;
        }
        CloseHandle(handle);
        return 0;
    }
}
