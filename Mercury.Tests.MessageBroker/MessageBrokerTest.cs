using Mercury.Common.Models;
using Mercury.Common.Services;
using Mercury.MessageBroker.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Mercury.Tests.MessageBroker
{
    public class MessageBrokerTest
    {

        public Mock<IConnectionFactory> _connectionFactoryMock = new Mock<IConnectionFactory>();
        public Mock<ILogger<Broker>> _loggerMock = new Mock<ILogger<Broker>>();
        public Mock<IBasicProperties> _basicPropertiesMock = new Mock<IBasicProperties>();


        [Fact]
        public void Enqueue_ThrowErrorWhenQueueIsBad()
        {
            _connectionFactoryMock.Setup(x => x.CreateConnection().CreateModel().QueueDeclarePassive(It.IsAny<string>()))
            .Throws(new RabbitMQ.Client.Exceptions.OperationInterruptedException(
                        new RabbitMQ.Client.ShutdownEventArgs(ShutdownInitiator.Library, 0, "")));

            Broker test = new Broker(_loggerMock.Object, _connectionFactoryMock.Object);
            Assert.Throws<InvalidQueueException>(() => test.EnqueueServiceRequest(new ServiceJobMessage(Guid.NewGuid(), "service", "")));

        }

        [Fact]
        public void Enqueue_ThrowErrorWhenWorkersAreZero()
        {
            _connectionFactoryMock.Setup(x => x.CreateConnection().CreateModel().QueueDeclarePassive(It.IsAny<string>())).Returns(new QueueDeclareOk("", 0, 0));


            Broker test = new Broker(_loggerMock.Object, _connectionFactoryMock.Object);
            Assert.Throws<InvalidQueueException>(() => test.EnqueueServiceRequest(new ServiceJobMessage(Guid.NewGuid(), "service", "")));

        }

        [Fact]
        public void Enqueue_SetsCorrelationId()
        {

        }

        [Fact]
        public void RegisterListener_UsesCorrectName()
        {

        }
        [Fact]
        public void RegisterListener_LogsConsumerTag()
        {

        }

        [Fact]
        public async Task RegisterListener_DeserializesMessage()
        {

        }
    }
}
