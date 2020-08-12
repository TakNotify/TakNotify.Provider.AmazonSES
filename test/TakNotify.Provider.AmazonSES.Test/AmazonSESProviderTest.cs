// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using TakNotify.Test;
using Xunit;

namespace TakNotify.Provider.AmazonSES.Test
{
    public class AmazonSESProviderTest
    {
        private readonly Mock<AmazonSimpleEmailServiceClient> _sesClient;
        private readonly Mock<ILoggerFactory> _loggerFactory;
        private readonly Mock<ILogger<Notification>> _logger;

        public AmazonSESProviderTest()
        {
            _sesClient = new Mock<AmazonSimpleEmailServiceClient>(RegionEndpoint.USWest1);
            
            _loggerFactory = new Mock<ILoggerFactory>();
            _logger = new Mock<ILogger<Notification>>();

            _loggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(_logger.Object);
        }

        [Fact]
        public async void Send_Success()
        {
            _sesClient.Setup(client => client.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SendEmailResponse { HttpStatusCode = System.Net.HttpStatusCode.OK });

            var message = new AmazonSESMessage
            {
                FromAddress = "sender@example.com",
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var provider = new AmazonSESProvider(new AmazonSESOptions(), _sesClient.Object, _loggerFactory.Object);

            var result = await provider.Send(message.ToParameters());

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);

            var startMessage = LoggerHelper.FormatLogValues(AmazonSESLogMessages.Sending_Start, message.Subject, message.ToAddresses);
            _logger.VerifyLog(LogLevel.Debug, startMessage);

            var endMessage = LoggerHelper.FormatLogValues(AmazonSESLogMessages.Sending_End, message.Subject, message.ToAddresses);
            _logger.VerifyLog(LogLevel.Debug, endMessage);
        }

        [Fact]
        public async void Send_Failed()
        {
            _sesClient.Setup(client => client.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SendEmailResponse { HttpStatusCode = System.Net.HttpStatusCode.BadRequest, MessageId = "1" });

            var message = new AmazonSESMessage
            {
                FromAddress = "sender@example.com",
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var provider = new AmazonSESProvider(new AmazonSESOptions(), _sesClient.Object, _loggerFactory.Object);

            var result = await provider.Send(message.ToParameters());

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            Assert.StartsWith("Status code", result.Errors[0]);

            var failedMessage = LoggerHelper.FormatLogValues(AmazonSESLogMessages.Sending_FailedWithId, message.Subject, message.ToAddresses, System.Net.HttpStatusCode.BadRequest, "1");
            _logger.VerifyLog(LogLevel.Warning, failedMessage);

            var endMessage = LoggerHelper.FormatLogValues(AmazonSESLogMessages.Sending_End, message.Subject, message.ToAddresses);
            _logger.VerifyLog(LogLevel.Debug, endMessage, Times.Never());
        }

        [Fact]
        public async void Send_WithDefaultFromAddress_Success()
        {
            _sesClient.Setup(client => client.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SendEmailResponse { HttpStatusCode = System.Net.HttpStatusCode.OK });

            var message = new AmazonSESMessage
            {
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var provider = new AmazonSESProvider(
                new AmazonSESOptions { DefaultFromAddress = "sender@example.com" }, 
                _sesClient.Object, 
                _loggerFactory.Object);

            var result = await provider.Send(message.ToParameters());

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async void Send_WithoutFromAddress_ReturnError()
        {
            var message = new AmazonSESMessage
            {
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var provider = new AmazonSESProvider(new AmazonSESOptions(), _sesClient.Object, _loggerFactory.Object);

            var result = await provider.Send(message.ToParameters());

            Assert.False(result.IsSuccess);
            Assert.Equal("From Address should not be empty", result.Errors[0]);
        }
    }
}
