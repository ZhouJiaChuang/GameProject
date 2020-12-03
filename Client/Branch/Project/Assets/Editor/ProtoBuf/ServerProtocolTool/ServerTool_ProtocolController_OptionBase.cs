using System;
using System.Collections.Generic;

//public interface IServerTool_ProtocolController_OptionBase
//{
//    object Cache { get; set; }
//    string OptionName { get; }
//    void OnGUI();
//    object GetDefaultData();
//    void OnInitialized();
//    void OnShown();
//    void OnHiden();
//    /// <summary>
//    /// 获取一键提交时需要提交的路径列表
//    /// </summary>
//    /// <returns></returns>
//    List<string> GetSubmitPathes();
//}

//public abstract class ServerTool_ProtocolController_OptionBase<T> : IServerTool_ProtocolController_OptionBase where T : class
//{
//    public ServerTool_ProtocolControllerWnd WndOwner { get; set; }

//    public ServerTool_ProtocolController_OptionBase(ServerTool_ProtocolControllerWnd wnd) { WndOwner = wnd; }

//    private object cache;
//    /// <summary>
//    /// 缓存
//    /// </summary>
//    object IServerTool_ProtocolController_OptionBase.Cache { get { return cache; } set { cache = value; } }

//    private T data;
//    /// <summary>
//    /// 选项数据缓存
//    /// </summary>
//    public T Data
//    {
//        get
//        {
//            if (data == null)
//            {
//                if (cache != null)
//                {
//                    data = cache as T;
//                }
//            }
//            return data;
//        }
//    }

//    object IServerTool_ProtocolController_OptionBase.GetDefaultData()
//    {
//        return Activator.CreateInstance<T>();
//    }

//    public abstract string OptionName { get; }
//    public abstract void OnGUI();
//    public virtual void OnInitialized() { }
//    public virtual void OnShown() { }
//    public virtual void OnHiden() { }
//    public virtual List<string> GetSubmitPathes()
//    {
//        return new List<string>();
//    }
//}