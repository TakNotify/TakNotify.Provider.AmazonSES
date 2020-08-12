// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
namespace TakNotify
{
    /// <summary>
    /// The options for the <see cref="AmazonSESProvider"/>
    /// </summary>
    public class AmazonSESOptions : NotificationProviderOptions
    {
        internal static string Parameter_AccessKey = $"{AmazonSESConstants.DefaultName}_{nameof(AccessKey)}";
        internal static string Parameter_SecretKey = $"{AmazonSESConstants.DefaultName}_{nameof(SecretKey)}";
        internal static string Parameter_RegionEndpoint = $"{AmazonSESConstants.DefaultName}_{nameof(RegionEndpoint)}";
        internal static string Parameter_DefaultFromAddress = $"{AmazonSESConstants.DefaultName}_{nameof(DefaultFromAddress)}";

        /// <summary>
        /// Instantiate the <see cref="AmazonSESOptions"/>
        /// </summary>
        public AmazonSESOptions()
        {
            Parameters.Add(Parameter_AccessKey, "");
            Parameters.Add(Parameter_SecretKey, "");
            Parameters.Add(Parameter_RegionEndpoint, "");
            Parameters.Add(Parameter_DefaultFromAddress, "");
        }

        /// <summary>
        /// The AWS Access Key
        /// <br/>(Please check https://docs.aws.amazon.com/general/latest/gr/aws-sec-cred-types.html on how to obtain the value)
        /// </summary>
        public string AccessKey
        {
            get => Parameters[Parameter_AccessKey].ToString();
            set => Parameters[Parameter_AccessKey] = value;
        }

        /// <summary>
        /// The AWS Secret Key
        /// <br/>(Please check https://docs.aws.amazon.com/general/latest/gr/aws-sec-cred-types.html on how to obtain the value)
        /// </summary>
        public string SecretKey
        {
            get => Parameters[Parameter_SecretKey].ToString();
            set => Parameters[Parameter_SecretKey] = value;
        }

        /// <summary>
        /// The AWS region endpoint
        /// <br/>(Please check https://docs.aws.amazon.com/general/latest/gr/rande.html for the list of regions)
        /// </summary>
        public string RegionEndpoint
        {
            get => Parameters[Parameter_RegionEndpoint].ToString();
            set => Parameters[Parameter_RegionEndpoint] = value;
        }

        /// <summary>
        /// The default "From Address" that will be used if the <see cref="AmazonSESMessage.FromAddress"/> is empty
        /// </summary>
        public string DefaultFromAddress
        {
            get => Parameters[Parameter_DefaultFromAddress].ToString();
            set => Parameters[Parameter_DefaultFromAddress] = value;
        }
    }
}
