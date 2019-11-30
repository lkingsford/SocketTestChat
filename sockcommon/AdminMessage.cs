using LiteNetLib;
using LiteNetLib.Utils;

namespace SockCommon
{
    public class AdminMessage : Message
    {
        public AdminMessage()
        {

        }

        public AdminMessage(byte[] messageBytes)
        {
            var reader = new NetDataReader(messageBytes);
            ServerMessage = reader.GetString();
        }

        public string ServerMessage;

        public override byte[] Serialize()
        {
            var writer = new NetDataWriter();
            writer.Put(ServerMessage);
            return writer.Data;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}