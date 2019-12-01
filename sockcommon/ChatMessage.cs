using LiteNetLib;
using LiteNetLib.Utils;

namespace SockCommon
{
    public class ChatMessage : Message
    {
        public ChatMessage(byte[] messageBytes)
        {

        }

        internal override byte[] SerializeSpecific()
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