using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;
using SockCommon;

namespace server
{
    public class Server
    {
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
            server.Start(9050);
            listener.PeerConnectedEvent +=  NewPeer;
            listener.ConnectionRequestEvent += AcceptRequest;
            listener.NetworkReceiveEvent += NetworkReceiveEvent;

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
            server.Stop();
        }


        internal void NewPeer(NetPeer peer)
        {
            Console.WriteLine("We got connection: {0}", peer.EndPoint); // Show peer ip
            NetDataWriter writer = new NetDataWriter();                 // Create writer class
            writer.Put("Hello client!");                                // Put some string
            peer.Send(writer, DeliveryMethod.ReliableOrdered);          // Send with reliability
            peers.Add(peer);
            foreach (var i in peers)
            {
                NetDataWriter writer2 = new NetDataWriter();                 // Create writer class
                writer2.Put("Our friends are " + String.Join(",", peers.Select(j=>j.EndPoint)));
                i.Send(writer2, DeliveryMethod.ReliableOrdered);
            }
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
        }
    }
}