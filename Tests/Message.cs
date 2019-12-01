using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SockCommon;
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
                var ChatMessage = new ChatMessage("Message");
                var ChatTest = new TestCaseData(typeof(ChatMessage), ChatMessage);
                ChatTest.TestName = "ChatMessage";
                yield return ChatTest;

                var AdminMessage = new AdminMessage("Message");
                var AdminTest = new TestCaseData(typeof(AdminMessage), AdminMessage);
                AdminTest.TestName = "AdminMessage";
                yield return AdminTest;

                var LoginMessage = new LoginMessage();
                var LoginTest = new TestCaseData(typeof(LoginMessage), LoginMessage);
                LoginTest.TestName = "LoginMessage";
                yield return LoginTest;
            }

        }

        [TestCaseSource("MessageTypes")]
        [Test]
        public void MessagesDeserializeToCorrectType(Type t, Message message)
        {
            byte[] messageBytes = {};
            var messageData = message.Serialize();
            var reader = new NetDataReader(messageData);
            var deserialized = Message.Deserialize(reader);
            Assert.That(deserialized.GetType(), Is.EqualTo(t));
        }
    }
}