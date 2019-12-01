using LiteNetLib;
using LiteNetLib.Utils;

namespace SockCommon
{
    public enum AdminMessageType {Other, NotLoggedIn, Error};

    public class AdminMessage : Message
    {
        public AdminMessageType MessageType;

        public AdminMessage(string message,
                            AdminMessageType messageType=AdminMessageType.Other)
        {
            ServerMessage = message;
            MessageType = messageType;
        }

        public AdminMessage(byte[] messageBytes)
        {
            var reader = new NetDataReader(messageBytes);
            ServerMessage = reader.GetString();
            MessageType = (AdminMessageType)reader.GetInt();
        }

        public string ServerMessage;

        internal override byte[] SerializeSpecific()
        {
            var writer = new NetDataWriter();
            writer.Put(ServerMessage);
            writer.Put((int)MessageType);
            return writer.Data;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}