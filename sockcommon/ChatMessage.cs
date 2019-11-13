using LiteNetLib;
using LiteNetLib.Utils;

namespace sockcommon
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
    }
}