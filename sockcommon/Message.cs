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
        private static Dictionary<int, Type> _messageTypes;

        private static Dictionary<int, Type> MessageTypes
        {
            get
            {
                if (_messageTypes != null)
                {
                    return _messageTypes;
                }
                else
                {
                    _messageTypes = new Dictionary<int, Type>();
                    // Init message types with reflection
                    // Stolen from https://stackoverflow.com/questions/857705/get-all-derived-types-of-a-type
                    var messageClasses = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                          from assemblyType in domainAssembly.GetTypes()
                                          where typeof(Message).IsAssignableFrom(assemblyType)
                                          select assemblyType).ToArray();
                    foreach (var m in messageClasses)
                    {
                        _messageTypes.Add(m.FullName.GetHashCode(), m);
                    }
                    return _messageTypes;
                }
            }
        }

        public static Message Deserialize(NetDataReader reader,
                                          NetPeer sender = null)
        {
            // Get type
            var typeHash= reader.GetInt();
            var newType = MessageTypes[typeHash];
            var messageBytes = reader.GetRemainingBytes();
            var message = (Message)System.Activator.CreateInstance(newType, messageBytes);
            message.sender = sender;
            return message;
        }

        public abstract byte[] Serialize();

        public void PutToWriter(NetDataWriter writer)
        {
            writer.Put(this.GetType().FullName.GetHashCode());
            writer.Put(Serialize());
        }

        /// <summary>
        /// The sender, if this is received from a peer.
        /// This will be null otherwise.
        /// </summary>
        public NetPeer sender = null;
    }

    public static class MessageExtensions
    {
        /// <summary>
        /// Send a message to a NetPeer
        /// </summary>
        /// <param name="peer">Peer to send to</param>
        /// <param name="message">Message to send</param>
        public static void Send(this NetPeer peer, Message message)
        {
            peer.Send(message.Serialize(), DeliveryMethod.ReliableOrdered);
        }
    }
}