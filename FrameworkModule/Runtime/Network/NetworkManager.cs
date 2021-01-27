using System;
using System.Text;
using System.Collections.Generic;
using CenturyGame.LuaModule.Runtime.Interfaces;
using CenturyGame.LoggerModule.Runtime;
using ILogger = CenturyGame.LoggerModule.Runtime.ILogger;

namespace CenturyGame.Framework.Network
{
    public abstract class NetworkClient
    {
        public abstract void Connect(string host, int port);

        public abstract void Disconnect();

        public abstract bool IsConnect();

        public abstract void SendMessage(byte[] data);

        public abstract void SetMsgProcesser(IMsgProcesser processer);
    }

    public class NetworkManager : FrameworkModule
    {
        private enum EConnectType
        {
            TCP,
        }

        private EConnectType ConnectType {get; set;}

        public NetworkClient Client { get; private set; }

        private Queue<byte[]> SendQueue = new Queue<byte[]>(Const.Max_Msg_Capacity);
        private Queue<OptionMsg> OptionQueue = new Queue<OptionMsg>(Const.Max_OptionMsg_Capacity);
        private Queue<byte[]> RecvQueue = new Queue<byte[]>(Const.Max_Msg_Capacity);

        public event Action onConnectSuccess;
        public event Action onConnectFailed;
        public event Action<string> onDisconnect; 

        public IMsgProcesser MsgProcesser { get; private set; }

        private readonly Lazy<ILogger> s_mLogger = new Lazy<ILogger>(() =>
            LoggerManager.GetLogger("Network"));

        public bool EnableProtoLog { get; set; } = false;

        internal override void Init()
        {
        }

        internal override void Update(float elapseTime, float realElapseTime)
        {
            TrySend();
            ProcessOptionMsg();
            ProcessMsg();
        }

        internal override void LateUpdate()
        {
            Trace.Instance.update(TraceUpdate);
        }

        internal override void Shutdown()
        {
            SendQueue.Clear();
            OptionQueue.Clear();
            RecvQueue.Clear();
            if (Client != null)
                Client.Disconnect();
        }

        private StringBuilder netLogBuilder = new StringBuilder();
        private void TraceUpdate(DateTime a_dataTime, ETracerLevel a_level, string a_context, string a_file, int a_line)
        {
            if (EnableProtoLog)
            {
                netLogBuilder.Length = 0;
                netLogBuilder.Append(a_dataTime.ToString("[yyyy-MM-dd HH:mm:ss]"));
                //netLogBuilder.Append("[" + a_level.ToString() + "]");
                netLogBuilder.Append(a_context);
                //netLogBuilder.Append("[" + a_file + "(" + a_line.ToString() + ")]");
                
                if (a_level >= ETracerLevel.ERROR)
                {
                    s_mLogger.Value.Error(netLogBuilder.ToString());
                }
                else if (a_level >= ETracerLevel.WARN)
                {
                    s_mLogger.Value.Warn(netLogBuilder.ToString());
                }
                else
                {
                    s_mLogger.Value.Debug(netLogBuilder.ToString());
                }
            }
        }

        public void SetMsgProcesser(IMsgProcesser processer)
        {
            MsgProcesser = processer;
        }

        private void EnqueueOptionMsg(OptionMsg msg)
        {
            OptionQueue.Enqueue(msg);
        }

        private void ProcessOptionMsg()
        {
            while (true)
            {
                if (OptionQueue.Count > 0)
                {
                    var msg = OptionQueue.Dequeue();
                    switch (msg.Type)
                    {
                        case EOptionMsgType.Connect:
                            var op_connect = msg as ConnectMsg;
                            OnConnect(op_connect.Success);
                            break;
                        case EOptionMsgType.Disconnect:
                            var op_disconnect = msg as DisconnectMsg;
                            OnDisconnect(op_disconnect.Reason);
                            break;
                        default:
                            break;
                    }
                }
                else
                    break;
            }
        }

        public void StartTcpConnect(string host, int port)
        {
            ConnectType = EConnectType.TCP;
            TcpClient tcp = new TcpClient();
            tcp.PostOptionMsgEvent += EnqueueOptionMsg;
            tcp.PostMsgEvent += EnqueueRecvMsg;
            Client = tcp;
            Client.SetMsgProcesser(MsgProcesser);
            Client.Connect(host, port);
        }

        [Obsolete("This method is deprecated. Use SendMessage(byte[] data) instead.", false)]
        public void SendMessage(string fullName, byte[] data)
        {
            SendQueue.Enqueue(data);
        }

        public void SendMessage(byte[] data)
        {
            SendQueue.Enqueue(data);
        }

        private void EnqueueRecvMsg(byte[] data)
        {
            RecvQueue.Enqueue(data);
        }

        private void ProcessMsg()
        {
            while (true)
            {
                if (RecvQueue.Count > 0)
                {
                    byte[] msgData = RecvQueue.Dequeue();
                    MsgProcesser.DispatchMsg(msgData);
                }
                else
                    break;
            }
        }

        private void TrySend()
        {
            if (Client == null)
                return;
            if (!Client.IsConnect())
                return;
            if (SendQueue.Count == 0)
                return;
            byte[] data = SendQueue.Dequeue();
            Client.SendMessage(data);
        }

        public void Disconnect()
        {
            if (Client == null)
                return;
            if (!Client.IsConnect())
                return;
            Client.Disconnect();
            Client = null;
        }

        private void OnConnect(bool success)
        {
            if (success)
                onConnectSuccess?.Invoke();
            else
                onConnectFailed?.Invoke();
        }

        private void OnDisconnect(DisconnectReason reason)
        {
            onDisconnect?.Invoke(reason.ToString());
        }

        private void OnErrorResponse(int code, string message)
        {
            Trace.Instance.error("recv ErrorResponse. code = {0}, message = {1}", code, message);
        }
    }
}