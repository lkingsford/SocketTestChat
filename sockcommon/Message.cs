﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;

namespace sockcommon
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

        public static Message Deserialize(NetDataReader reader)
        {
            // Get type
            var typeHash= reader.GetInt();
            var newType = MessageTypes[typeHash];
            var messageBytes = reader.GetRemainingBytes();
            var message = (Message)System.Activator.CreateInstance(newType, messageBytes);
            return message;
        }

        public abstract byte[] Serialize();

        public void PutToWriter(NetDataWriter writer)
        {
            writer.Put(this.GetType().FullName.GetHashCode());
            writer.Put(Serialize());
        }
    }
}