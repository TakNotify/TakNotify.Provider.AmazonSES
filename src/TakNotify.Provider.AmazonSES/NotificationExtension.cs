// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TakNotify
{
    /// <summary>
    /// The extension for <see cref="INotification"/> to send email with Amazon SES provider
    /// </summary>
    public static class NotificationExtension
    {
        /// <summary>
        /// Send message with <see cref="AmazonSESProvider"/>
        /// </summary>
        /// <param name="notification">The notification object</param>
        /// <param name="message">The wrapper of the message that will be sent</param>
        /// <returns></returns>
        public static Task<NotificationResult> SendEmailWithAmazonSES(this INotification notification, AmazonSESMessage message)
        {
            return notification.Send(AmazonSESConstants.DefaultName, message.ToParameters());
        }
    }
}
