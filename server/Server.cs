using System;
using System.Threading;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using SockCommon;

namespace server
{
    public class Server
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        List<NetPeer> peers;
        NetManager server;
        EventBasedNetListener listener;
        public Server()
        {
            listener = new EventBasedNetListener();
            server = new NetManager(listener);
            peers = new List<NetPeer>();
        }

        public void Start()
        {
            Logger.Debug("Assigning listeners");
            listener.PeerConnectedEvent +=  NewPeer;
            listener.ConnectionRequestEvent += AcceptRequest;
            listener.NetworkReceiveEvent += NetworkReceiveEvent;

            Logger.Info("Starting server");
            server.Start(9050);
            while (true)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
            server.Stop();
        }

        internal void SendToAllPeers(Message message)
        {
            foreach (var peer in peers)
            {
                peer.Send(message);
            }
        }

        internal void NewPeer(NetPeer peer)
        {
            Logger.Info("New connection from {ip}", peer.EndPoint);
            NetDataWriter writer = new NetDataWriter();
            peers.Add(peer);
            SendToAllPeers(new AdminMessage($"{peer.EndPoint} has joined"));
        }

        internal void AcceptRequest(ConnectionRequest request)
        {
            {
                if(server.PeersCount < 10 /* max connections */)
                    request.AcceptIfKey("SomeConnectionKey");
                else
                    request.Reject();
            };
        }

        internal void NetworkReceiveEvent(NetPeer sender,
                                          NetPacketReader dataReader,
                                          DeliveryMethod deliveryMethod)
        {
            var received = Message.Deserialize(dataReader, sender);
            switch(received)
            {
                case LoginMessage message:
                    break;

                case ChatMessage message:
                    break;
            }
            dataReader.Recycle();
        }
    }
}