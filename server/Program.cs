using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            NetManager server = new NetManager(listener);
            server.Start(9050 /* port */);

            listener.ConnectionRequestEvent += request =>
            {
                if(server.PeersCount < 10 /* max connections */)
                    request.AcceptIfKey("SomeConnectionKey");
                else
                    request.Reject();
            };

            var peers = new List<NetPeer>();
            listener.PeerConnectedEvent += peer =>
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
            };

            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
            server.Stop(); 
        }
    }
}
