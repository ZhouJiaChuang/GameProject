/***
 * Author --- ZJC
 * Description --- 
 * Function:
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

/// <summary>
/// 网络事件
/// </summary>
public class CSNetwork : Singleton<CSNetwork>
{
    protected EventHandlerManager ClientEventHandler = new EventHandlerManager(EventHandlerManager.DispatchType.Event);
    protected EventHandlerManager SocketEventHandler = new EventHandlerManager(EventHandlerManager.DispatchType.Socket);

    public static void SendClientEvent(EClientEvent e, params object[] param)
    {
        Instance.ClientEventHandler.SendEvent(e, param);
    }

    public static void SendSocketEvent(ESocketEvent e, params object[] param)
    {
        Instance.SocketEventHandler.SendEvent(e, param);
    }


    private Thread thread;
    public bool IsAlive
    {
        get { return thread != null ? thread.IsAlive : false; }
    }

    /// <summary>
    /// 主机域名
    /// </summary>
    public string Host { get; set; }
    /// <summary>
    /// 主机端口
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// Tcp链接
    /// </summary>
    public TcpClient Client { get; set; }

    private const int READ_BUFFER_SIZE = 8192 * 2;
    private byte[] readBuf = new byte[READ_BUFFER_SIZE];

    private CircularBuffer<byte> ringBuf = new CircularBuffer<byte>(READ_BUFFER_SIZE, true);

    /// <summary>
    /// 初始解包线程
    /// </summary>
    public void Init()
    {
        CloseThread();
        thread = new Thread(AnalyticalNetInfo)
        {
            IsBackground = true
        };
        thread.Priority = System.Threading.ThreadPriority.BelowNormal;
        thread.Start();
    }

    public void Connect(string _host, int _port)
    {
        if (!IsAlive)
        {
            Init();
        }
        if (Client != null)
        {
            Close();
        }

        Host = _host;
        Port = _port;

        Client = new TcpClient();
        Client.SendTimeout = 3000;
        Client.ReceiveTimeout = 3000;
        Client.NoDelay = true;
        try
        {
            Client.BeginConnect(Host, Port, new AsyncCallback(OnConnect), Client);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("DayConnectClient Error");
        }

    }

    public void Close()
    {
        if (Client != null)
        {
            Client.Close();
        }
        Client = null;
    }

    /// <summary>
    /// 关闭解析线程
    /// </summary>
    public void CloseThread()
    {
        if (thread != null)
        {
            thread.Abort();
            thread = null;
        }
    }

    public void Update()
    {
        NetMsgEventParse();
    }

    private void OnConnect(IAsyncResult ar)
    {
        try
        {
            TcpClient c = (TcpClient)ar.AsyncState;
            c.EndConnect(ar);

            if (Client != null && Client.Connected)
            {
                lock (objThread)
                {
                    ServerConnectedSuccess();
                }
                Client.GetStream().BeginRead(readBuf, 0, READ_BUFFER_SIZE, new AsyncCallback(ServerByteReceive), null);
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("OnConnect Error");
        }
    }

    protected virtual void ServerConnectedSuccess()
    {
    }


    public virtual void ServerDisconnect(string error, bool needReconnect)
    {

    }


    protected static Queue<NetworkMsg> mMsgEvents = new Queue<NetworkMsg>();
    protected static Queue<NetInfo> mMsgCollectionEvents = new Queue<NetInfo>();
    //用来lock线程的
    private static readonly object objThread = new object();

    /// <summary>
    /// 服务器数据接收
    /// </summary>
    /// <param name="ar"></param>
    private void ServerByteReceive(IAsyncResult ar)
    {
        try
        {
            int sizeRead = 0;

            lock (Client.GetStream())
            {         //读取字节流到缓冲区
                sizeRead = Client.GetStream().EndRead(ar);
            }

            if (sizeRead < 1)
            {
                //包尺寸有问题，断线处理
                //若包尺寸为0,则为服务器故意发送的断线消息,不进行重连;若包尺寸不为0,则为网络故障引起的,尝试重连
                Close();
                ServerDisconnect("sizeRead < 1", sizeRead != 0);
                return;
            }

            ringBuf.Put(readBuf, 0, sizeRead);

            AnalyticalNetworkMsg();

            lock (Client.GetStream())
            {
                Array.Clear(readBuf, 0, readBuf.Length);   //清空数组
                Client.GetStream().BeginRead(readBuf, 0, READ_BUFFER_SIZE, new AsyncCallback(ServerByteReceive), null);
            }
        }
        catch (Exception ex)
        {
        }
    }

    /// <summary>
    /// 将服务器发的的bytes进行解析
    /// </summary>
    private void AnalyticalNetworkMsg()
    {
        while (ringBuf.Size >= sizeof(uint))
        {
            byte[] lengthByte = new byte[sizeof(uint)];

            ringBuf.CopyTo(0, lengthByte, 0, lengthByte.Length);

            uint msgLength = BitConverter.ToUInt32(lengthByte, 0);

            msgLength = NetworkMsg.NetworkToHostOrder(msgLength);

            if (ringBuf.Size >= msgLength)
            {
                byte[] msgdata = new byte[msgLength];

                ringBuf.Get(msgdata);

                if (msgLength == 0)
                {
                    ringBuf.Clear();
                    return;
                }

                using (MemoryStream stream = new MemoryStream(msgdata))
                {
                    BinaryReader br = new BinaryReader(stream);

                    NetworkMsg Nmsg = new NetworkMsg();

                    try
                    {
                        Nmsg.length = NetworkMsg.NetworkToHostOrder(br.ReadInt32());
                        Nmsg.msgId = NetworkMsg.NetworkToHostOrder(br.ReadInt32());
                        Nmsg.sequence = (short)NetworkMsg.NetworkToHostOrder(br.ReadInt16());
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    // 10 is header length    length:4  msgId:4 sequence:2
                    byte[] data = br.ReadBytes((int)msgLength - 10);

                    Nmsg.data = data;

                    lock (objThread)
                    {
                        mMsgEvents.Enqueue(Nmsg);
                    }
                }
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 解析成NetInfo,以供使用
    /// </summary>
    public void AnalyticalNetInfo()
    {
        while (true)
        {
            if (mMsgEvents.Count > 0)
            {
                NetworkMsg msg;
                lock (objThread)
                {
                    msg = mMsgEvents.Dequeue();
                    if (msg.msgId != 0)
                    {
                        Type type = null;
                        try
                        {
                            object obj = NetworkMsg.Deserializes(msg.data, type, msg.msgId);
                            lock (objThread)
                            {
                                mMsgCollectionEvents.Enqueue(new NetInfo(msg.msgId, obj));
                            }
                        }
                        catch (Exception)
                        {
                            lock (objThread)
                            {
                                mMsgCollectionEvents.Enqueue(new NetInfo(msg.msgId, null));
                            }
                        }
                    }
                }
            }
            Thread.Sleep(1);
        }
    }

    #region 服务器事件检查与事件下发

    /// <summary>
    /// 每帧处理最大消息包个数
    /// </summary>
    public const int MaxDealPackPerFrame = 200;
    private INetEventProcess _EventProcess;
    /// <summary>
    /// 事件处理器,用来分发事件
    /// </summary>
    public virtual INetEventProcess EventProcess
    {
        get
        {
            if (_EventProcess == null)
            {
                CSDebug.LogError("请先将NetWork种的事件处理器EventProcess实现");
                return null;
            }
            if (_EventProcess.Network == null)
            {
                _EventProcess.Init(this);
            }

            return _EventProcess;
        }
        set
        {
            _EventProcess = value;
        }
    }

    private void NetMsgEventParse()
    {
        if (mMsgCollectionEvents.Count > 0)
        {
            int maxDeal = MaxDealPackPerFrame;
            while (mMsgCollectionEvents.Count > 0 && maxDeal > 0)
            {
                NetInfo info;
                lock (objThread)
                {
                    info = mMsgCollectionEvents.Dequeue();
                    maxDeal--;
                    EventProcess.Process(info.msgId, info);
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// 客户端测试发送网络消息
    /// 直接跳过一系类东西,直接发送内容到这里面
    /// </summary>
    /// <param name="msgId"></param>
    /// <param name="obj"></param>
    public void ClientTestSendSocketMsg(int msgId,object obj)
    {
        NetInfo info = new NetInfo();
        info.msgId = msgId;
        info.obj = obj;
        mMsgCollectionEvents.Enqueue(info);
    }
}



public struct NetworkMsg
{
    public int length; //长度
    public int msgId;//id
    public byte[] data;//正文
    public short sequence;

    public NetworkMsg(int _length, int _msgId, byte[] _data, short sequence)
    {
        this.length = _length;
        this.msgId = _msgId;
        this.data = _data;
        this.sequence = sequence;
    }


    /// <summary>
    /// 线程中调用，反序列服务器数据
    /// </summary>
    /// <param name="buffs"></param>
    /// <param name="type"></param>
    /// <param name="msgID"></param>
    /// <returns></returns>
    public static object Deserializes(byte[] buffs, Type type, int msgID)
    {
        object obj = null;

        using (MemoryStream stream = new MemoryStream(buffs))
        {
            obj = ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(stream, null, type);
            stream.Close();
        }

        return obj;
    }

    public static uint NetworkToHostOrder(uint val)
    {
        byte[] array = BitConverter.GetBytes(val);
        Array.Reverse(array);
        return BitConverter.ToUInt32(array, 0);
    }

    public static int NetworkToHostOrder(int val)
    {
        byte[] array = BitConverter.GetBytes(val);
        Array.Reverse(array);
        return BitConverter.ToInt32(array, 0);
    }
}