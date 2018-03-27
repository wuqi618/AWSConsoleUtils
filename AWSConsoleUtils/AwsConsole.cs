using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.SecurityToken.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AWSConsoleUtils
{
    public class AwsConsole
    {
        private readonly string _credentialProfileName;
        private readonly string _tokenProfileName;
        private readonly AwsAccount _account;

        public AwsConsole(string credentialProfileName, string tokenProfileName, AwsAccount account)
        {
            _credentialProfileName = credentialProfileName;
            _tokenProfileName = tokenProfileName;
            _account = account;
        }

        public async Task Open()
        {
            var credentials = await AwsCredentialHelper.GetTemporaryCredentials(_credentialProfileName, _tokenProfileName, _account);

            var signinToken = await GetSigninToken(credentials);

            var consoleUrl = $"https://{_account.Region.SystemName}.console.aws.amazon.com/console/home";

            var loginUrl = $"https://signin.aws.amazon.com/federation?Action=login&Destination={WebUtility.UrlEncode(consoleUrl)}&SigninToken={WebUtility.UrlEncode(signinToken)}";

            OpenBrowser(loginUrl, _account.AccountNumber);
        }

        private async Task<string> GetSigninToken(Credentials credentials)
        {
            var session = $"{{\"sessionId\":\"{credentials.AccessKeyId}\",\"sessionKey\":\"{credentials.SecretAccessKey}\",\"sessionToken\":\"{credentials.SessionToken}\"}}";

            var getSigninTokenUrl = $"https://signin.aws.amazon.com/federation?Action=getSigninToken&SessionDuration=43200&Session={WebUtility.UrlEncode(session)}";

            var httpClient = new HttpClient();

            var httpResponse = await httpClient.GetAsync(getSigninTokenUrl);

            var content = await httpResponse.Content.ReadAsStringAsync();

            var jsonObject = (JObject)JsonConvert.DeserializeObject(content);

            return jsonObject["SigninToken"].ToString();
        }

        private void OpenBrowser(string url, string userDataDir)
        {
            var path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                    Arguments = $"--user-data-dir=\"{path}\\{userDataDir}\" {url}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            //try
            //{
            //    Process.Start(url);
            //}
            //catch
            //{
            //    // hack because of this: https://github.com/dotnet/corefx/issues/10361
            //    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //    {
            //        url = url.Replace("&", "^&");
            //        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            //    }
            //    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            //    {
            //        Process.Start("xdg-open", url);
            //    }
            //    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            //    {
            //        Process.Start("open", url);
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}
        }
    }
}
