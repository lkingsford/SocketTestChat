using LiteNetLib;
using LiteNetLib.Utils;

namespace SockCommon
{
    public class ChatMessage : Message
    {
        public ChatMessage(byte[] messageBytes)
        {

        }

        public override byte[] Serialize()
        {
            return new byte[] {};
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public string Contents;
        public string SenderName;
    }
}