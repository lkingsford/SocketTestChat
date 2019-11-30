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

        public void Start()
        {
            client.Start();
            client.Connect("localhost" /* host ip or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
            listener.NetworkReceiveEvent += NetworkReceiveEvent;

            while (!Console.KeyAvailable)
            {
                client.PollEvents();
                Thread.Sleep(15);
            }

            client.Stop();
        }

        internal void NetworkReceiveEvent(NetPeer sender,
                                          NetPacketReader dataReader,
                                          DeliveryMethod deliveryMethod)
        {
            var received = Message.Deserialize(dataReader, sender);
            switch(received)
            {
                case ChatMessage message:
                    break;
            }
            dataReader.Recycle();
        }
    }
}