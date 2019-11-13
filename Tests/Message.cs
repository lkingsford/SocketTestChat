using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using sockcommon;
using LiteNetLib.Utils;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        public static IEnumerable<TestCaseData> MessageTypes
        {
            get {
                var messageClasses = new List<Type> {
                    typeof(ChatMessage)
                };
                foreach (var yieldType in messageClasses)
                {
                    var tcd = new TestCaseData(yieldType);
                    tcd.TestName = yieldType.FullName;
                    yield return tcd;
                }
            }

        }

        [TestCaseSource("MessageTypes")]
        [Test]
        public void MessagesDeserializeToCorrectType(Type t)
        {
            byte[] messageBytes = {};
            var message = (Message)System.Activator.CreateInstance(t, messageBytes);
            var writer = new NetDataWriter();
            message.PutToWriter(writer);
            var reader = new NetDataReader(writer.Data);
            var deserialized = Message.Deserialize(reader);
            Assert.That(deserialized.GetType(), Is.EqualTo(t));
        }
    }
}