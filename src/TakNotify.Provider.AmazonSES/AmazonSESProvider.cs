// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TakNotify
{
    /// <summary>
    /// The notification provider to send email via Amazon SES
    /// </summary>
    public class AmazonSESProvider : NotificationProvider
    {
        private readonly AmazonSESOptions _options;
        private readonly AmazonSimpleEmailServiceClient _sesClient;

        /// <summary>
        /// Instantiate the <see cref="AmazonSESProvider"/>
        /// </summary>
        /// <param name="options">The options for this provider</param>
        /// <param name="loggerFactory">The logger factory</param>
        public AmazonSESProvider(AmazonSESOptions options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
        {
            _options = options;
            _sesClient = GetSESClient(options.AccessKey, options.SecretKey, options.RegionEndpoint);
        }

        /// <summary>
        /// Instantiate the <see cref="AmazonSESProvider"/>
        /// </summary>
        /// <param name="options">The options for this provider</param>
        /// <param name="sesClient">The instance of <see cref="AmazonSimpleEmailServiceClient"/></param>
        /// <param name="loggerFactory">The logger factory</param>
        public AmazonSESProvider(AmazonSESOptions options, AmazonSimpleEmailServiceClient sesClient, ILoggerFactory loggerFactory) : base(options, loggerFactory)
        {
            _options = options;
            _sesClient = sesClient;
        }

        /// <summary>
        /// Destructor of <see cref="AmazonSESProvider"/>
        /// </summary>
        ~AmazonSESProvider()
        {
            _sesClient.Dispose();
        }

        /// <summary>
        /// Instantiate the <see cref="AmazonSESProvider"/>
        /// </summary>
        /// <param name="options">The options for this provider</param>
        /// <param name="loggerFactory">The logger factory</param>
        public AmazonSESProvider(IOptions<AmazonSESOptions> options, ILoggerFactory loggerFactory) : base(options.Value, loggerFactory)
        {
            _options = options.Value;
            _sesClient = GetSESClient(_options.AccessKey, _options.SecretKey, _options.RegionEndpoint);
        }

        /// <inheritdoc cref="NotificationProvider.Name"/>
        public override string Name => AmazonSESConstants.DefaultName;

        /// <inheritdoc cref="NotificationProvider.Send(MessageParameterCollection)"/>
        public override async Task<NotificationResult> Send(MessageParameterCollection messageParameters)
        {
            var message = new AmazonSESMessage(messageParameters);

            var source = message.FromAddress;
            if (string.IsNullOrEmpty(source) && _options != null)
                source = _options.DefaultFromAddress;
            if (string.IsNullOrEmpty(source))
                return new NotificationResult(new List<string> { "From Address should not be empty" });

            var destination = new Destination()
            {
                ToAddresses = message.ToAddresses,
                CcAddresses = message.CCAddresses,
                BccAddresses = message.BCCAddresses
            };

            var reqMessage = new Message(
                new Content(message.Subject),
                new Body
                {
                    Text = new Content(message.PlainContent),
                    Html = new Content(message.HtmlContent)
                }
                );

            var request = new SendEmailRequest(source, destination, reqMessage)
            {
                ReplyToAddresses = message.ReplyToAddresses
            };

            try
            {
                Logger.LogDebug(AmazonSESLogMessages.Sending_Start, message.Subject, message.ToAddresses);

                var response = await _sesClient.SendEmailAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    Logger.LogDebug(AmazonSESLogMessages.Sending_End, message.Subject, message.ToAddresses);
                    return new NotificationResult(true);
                }

                Logger.LogWarning(AmazonSESLogMessages.Sending_FailedWithId, message.Subject, message.ToAddresses, response.HttpStatusCode, response.MessageId);
                return new NotificationResult(new List<string> { $"Status code = {response.HttpStatusCode}, Message Id = {response.MessageId}" });
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, AmazonSESLogMessages.Sending_Failed, message.Subject, message.ToAddresses);
                return new NotificationResult(new List<string> { ex.Message });
            }
        }

        private AmazonSimpleEmailServiceClient GetSESClient(string accessKey, string secretKey, string regionName)
        {
            var cred = new BasicAWSCredentials(accessKey, secretKey);
            var region = RegionEndpoint.GetBySystemName(regionName);
            if (region == null)
                region = RegionEndpoint.USWest1;

            return new AmazonSimpleEmailServiceClient(cred, region);
        }
    }
}
