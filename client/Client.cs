using LiteNetLib;
using System;
using System.Threading;
using SockCommon;

namespace client
{
    class Client
    {
        const string SERVER = "localhost";
        const int PORT = 9050;
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
            client.Connect(SERVER, PORT, "SomeConnectionKey");
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
                    ProcessAdminMessage(message);
                    break;
            }
            dataReader.Recycle();
        }

        internal void ProcessAdminMessage(AdminMessage message)
        {
            switch (message.MessageType)
            {
                case AdminMessageType.NotLoggedIn:
                    messageWrite("Log in with \\login <name>");
                    break;
                default:
                    break;
            }
        }

        internal void SendChatMessage(string message)
        {
            var messageToSend = new ChatMessage(message);
            client.FirstPeer.Send(messageToSend);
        }

        internal void ProcessCommand(string command)
        {
            var sanitizedCommand = command;
            if (sanitizedCommand[0] == '\\')
            {
                sanitizedCommand = sanitizedCommand.TrimStart('\\');
            }
            var args = sanitizedCommand.Split(' ');
            if (args.Length == 0)
            {
                messageWrite("Invalid command");
                return;
            }
            switch (args[0].ToLowerInvariant())
            {
                case "login":
                    if (args.Length > 1)
                    {
                        client.FirstPeer.Send(new LoginMessage(args[1]));
                    }
                    else
                    {
                        messageWrite("Must provide name");
                    }
                    break;
                default:
                    messageWrite("Unknown command");
                    break;
            }
        }

        public delegate void MessageWriteDelegate(string text);
        public MessageWriteDelegate messageWrite = null;
    }
}