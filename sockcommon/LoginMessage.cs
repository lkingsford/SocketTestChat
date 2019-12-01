using LiteNetLib;
using LiteNetLib.Utils;

namespace SockCommon
{
    public class LoginMessage : Message
    {
        public LoginMessage()
        {

        }

        public LoginMessage(string name)
        {
            Name = name;
        }

        public LoginMessage(byte[] messageBytes)
        {
            var reader = new NetDataReader(messageBytes);
            Name = reader.GetString();
        }

        public string Name;

        internal override byte[] SerializeSpecific()
        {
            var writer = new NetDataWriter();
            writer.Put(Name);
            return writer.Data;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}