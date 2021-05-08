using AutoFixture;
using LvisBot.BusinessLogic.Modules;
using LvisBot.BusinessLogic.Services;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;
using Moq;
using NUnit.Framework;

namespace LvisBot.Tests
{
    public class TimeCodeHandlerTests
    {
        [Test]
        public void Execute()
        {
            // arrange
            var fixture = new Fixture();
            var fileServiceMock = new Mock<IFileService>();
            var serializationServiceMock = new Mock<ISerializationService>();

            var configuration = new ConfigurationService(serializationServiceMock.Object);

            var handler = new TimeCodeHandler(fileServiceMock.Object, serializationServiceMock.Object, configuration);

            var messageResponse = fixture.Create<YTMessageResponse>();
            
            // act
            handler.Execute(messageResponse);

            // assert
        }
    }
}