using System.Linq;
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

        List<NetPeer> peers = new List<NetPeer>();
        List<LoggedInPeer> loggedInPeers = new List<LoggedInPeer>();

        NetManager server;
        EventBasedNetListener listener;
        public Server()
        {
            listener = new EventBasedNetListener();
            server = new NetManager(listener);
        }

        public void Start()
        {
            Logger.Debug("Assigning listeners");
            listener.PeerConnectedEvent +=  NewPeer;
            listener.ConnectionRequestEvent += AcceptRequest;
            listener.NetworkReceiveEvent += NetworkReceiveEvent;
            listener.PeerDisconnectedEvent += LostPeer;

            Logger.Info("Starting server");
            server.Start(9050);
            while (true)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
        }

        private void SendToAllLoggedInPeers(Message message)
        {
            foreach (var peer in loggedInPeers)
            {
                peer.Peer.Send(message);
            }
        }

        private void NewPeer(NetPeer peer)
        {
            Logger.Info("New connection from {ip}", peer.EndPoint);
            NetDataWriter writer = new NetDataWriter();
            peers.Add(peer);
            peer.Send(new AdminMessage("Connected to server"));
        }

        private void LostPeer(NetPeer peer, DisconnectInfo info)
        {
            Logger.Info("Lost connection from {ip} ({reason})", peer.EndPoint, info.Reason);
            NetDataWriter writer = new NetDataWriter();
            peers.Remove(peer);
            if (LoggedIn(peer))
            {
                SendToAllLoggedInPeers(new AdminMessage($"{GetUser(peer).Username} has left ({info.Reason.ToString()})"));
                loggedInPeers.RemoveAll((i)=>i.Peer==peer);
            }
        }

        private void AcceptRequest(ConnectionRequest request)
        {
            {
                if(server.PeersCount < 128 /* max connections */)
                    request.AcceptIfKey("SomeConnectionKey");
                else
                    request.Reject();
            };
        }

        private bool LoggedIn(NetPeer peer)
        {
            return loggedInPeers.Any((i)=>i.Peer == peer);
        }

        private LoggedInPeer GetUser(NetPeer peer)
        {
            return loggedInPeers.First((i)=>i.Peer==peer);
        }

        private void SendNotLoggedIn(NetPeer peer)
        {
            peer.Send(new AdminMessage("You need to be logged in to perform this action"));
        }

        private void LogIn(NetPeer peer, string name)
        {
            SendToAllLoggedInPeers(new AdminMessage($"{name} has joined"));
            var user = new LoggedInPeer();
            user.Username = name;
            user.Peer = peer;
            loggedInPeers.Add(user);
        }

        private void NetworkReceiveEvent(NetPeer sender,
                                          NetPacketReader dataReader,
                                          DeliveryMethod deliveryMethod)
        {
            var received = Message.Deserialize(dataReader, sender);
            switch(received)
            {
                case LoginMessage message:
                    LogIn(sender, message.Name);
                    break;

                case ChatMessage message:
                    if (LoggedIn(sender))
                    {
                        Logger.Info("{sender}: {message}", sender.EndPoint.ToString(), message.Contents);
                        SendToAllLoggedInPeers(new ChatMessage(message.Contents, sender.EndPoint.ToString()));
                    }
                    else
                    {
                        Logger.Info("${sender} is not logged in but attempted to Chat", sender);
                        SendNotLoggedIn(sender);
                    }
                    break;
            }
            dataReader.Recycle();
        }
    }
}