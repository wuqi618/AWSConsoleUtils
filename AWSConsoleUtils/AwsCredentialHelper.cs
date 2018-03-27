using System;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace AWSConsoleUtils
{
    public static class AwsCredentialHelper
    {
        public static AmazonMFAToken GetMFAToken(string profileName)
        {
            var profile = GetAWSCredentialProfile(profileName);

            var amazonMfa = new AmazonMFA(profile.Options.AccessKey, profile.Options.SecretKey);

            return amazonMfa.GetToken();
        }

        public static CredentialProfile GetAWSCredentialProfile(string profileName)
        {
            return new NetSDKCredentialsFile().TryGetProfile(profileName, out var profile) ? profile : null;
        }

        public static AWSCredentials GetAWSCredentials(string profileName)
        {
            return new CredentialProfileStoreChain().TryGetAWSCredentials(profileName, out var credentials) ? credentials : null;
        }

        public static async Task<Credentials> GetTemporaryCredentials(string credentialProfileName, string tokenProfileName, AwsAccount account)
        {
            var credentials = GetAWSCredentials(credentialProfileName);

            var token = GetMFAToken(tokenProfileName);

            var roleArn = $"arn:aws:iam::{account.AccountNumber}:role/{account.Role}";

            var client = new AmazonSecurityTokenServiceClient(credentials);
            var request = new AssumeRoleRequest
            {
                DurationSeconds = 3600,
                RoleArn = roleArn,
                RoleSessionName = Guid.NewGuid().ToString(),
                SerialNumber = token.SerialNumber,
                TokenCode = token.TokenCode
            };

            var response = await client.AssumeRoleAsync(request);
            return response.Credentials;
        }
    }
}
