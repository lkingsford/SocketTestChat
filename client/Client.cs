using LiteNetLib;
using System;
using System.Threading;
using SockCommon;

namespace client
{
    class Client
    {
        EventBasedNetListener listener;
        NetManager client;

        public Client()
        {
            listener = new EventBasedNetListener();
            client = new NetManager(listener);
        }

        public void Init()
        {
            client.Start();
            client.Connect("localhost" /* host ip or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
            listener.NetworkReceiveEvent += NetworkReceiveEvent;
        }
        public void Stop()
        {
            client.Stop();
        }

        public void Poll()
        {
            client.PollEvents();
        }

        internal void NetworkReceiveEvent(NetPeer sender,
                                          NetPacketReader dataReader,
                                          DeliveryMethod deliveryMethod)
        {
            var received = Message.Deserialize(dataReader, sender);
            switch(received)
            {
                case ChatMessage message:
                    messageWrite($"{message.SenderName}:   {message.Contents}");
                    break;
                case AdminMessage message:
                    messageWrite($"Servers says: {message.ServerMessage}");
                    break;
            }
            dataReader.Recycle();
        }

        internal void SendChatMessage(string message)
        {
            var messageToSend = new ChatMessage(message);
            client.FirstPeer.Send(messageToSend);
        }

        public delegate void MessageWriteDelegate(string text);
        public MessageWriteDelegate messageWrite = null;
    }
}