using LiteNetLib;
using LiteNetLib.Utils;

namespace SockCommon
{
    public class ChatMessage : Message
    {
        public ChatMessage(string message, string senderName = null)
        {
            Contents = message;
            SenderName = senderName;
        }

        public ChatMessage(byte[] messageBytes)
        {
            var reader = new NetDataReader(messageBytes);
            SenderName = reader.GetString();
            Contents = reader.GetString();
        }

        internal override byte[] SerializeSpecific()
        {
            var writer = new NetDataWriter();
            writer.Put(SenderName);
            writer.Put(Contents);
            return writer.Data;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public string Contents;
        public string SenderName;
    }
}