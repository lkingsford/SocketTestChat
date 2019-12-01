using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;

namespace SockCommon
{
    public abstract class Message
    {
        private static ulong GetIdHash(Type typeToHash)
        {
            //Lazily stolen from https://stackoverflow.com/questions/9545619/a-fast-hash-function-for-string-in-c-sharp
            //(A Knuth Hash)
            UInt64 hashedValue = 3074457345618258791ul;
            var read = typeToHash.FullName;
            for(int i=0; i<read.Length; i++)
            {
                hashedValue += read[i];
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static Dictionary<ulong, Type> _messageTypes;

        private static Dictionary<ulong, Type> MessageTypes
        {
            get
            {
                if (_messageTypes != null)
                {
                    return _messageTypes;
                }
                else
                {
                    _messageTypes = new Dictionary<ulong, Type>();
                    // Init message types with reflection
                    // Stolen from https://stackoverflow.com/questions/857705/get-all-derived-types-of-a-type
                    var messageClasses = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                          from assemblyType in domainAssembly.GetTypes()
                                          where typeof(Message).IsAssignableFrom(assemblyType)
                                          select assemblyType).ToArray();
                    foreach (var m in messageClasses)
                    {
                        _messageTypes.Add(GetIdHash(m), m);
                    }
                    return _messageTypes;
                }
            }
        }

        public static Message Deserialize(NetDataReader reader,
                                          NetPeer sender = null)
        {
            // Get type
            var typeHash = reader.GetULong();
            var newType = MessageTypes[typeHash];
            var messageBytes = reader.GetRemainingBytes();
            var message = (Message)System.Activator.CreateInstance(newType, messageBytes);
            message.sender = sender;
            return message;
        }

        internal abstract byte[] SerializeSpecific();

        public byte[] Serialize()
        {
            var writer = new NetDataWriter();
            var hash = GetIdHash(this.GetType());
            writer.Put(hash);
            writer.Put(SerializeSpecific());
            return writer.Data;
        }

        /// <summary>
        /// The sender, if this is received from a peer.
        /// This will be null otherwise.
        /// </summary>
        public NetPeer sender = null;
    }

    public static class MessageExtensions
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Send a message to a NetPeer
        /// </summary>
        /// <param name="peer">Peer to send to</param>
        /// <param name="message">Message to send</param>
        public static void Send(this NetPeer peer, Message message)
        {
            var messageToSend = message.Serialize();
            Logger.Debug($"Sending {messageToSend} to {peer.EndPoint}");
            peer.Send(messageToSend, DeliveryMethod.ReliableOrdered);
        }
    }
}