using System;
using System.Diagnostics;

namespace AWSConsoleUtils
{
    class Program
    {
        private const string _credentialProfileName = "default";
        private const string _tokenProfileName = "AWS_MFA";

        private static CommandType _commandType;
        private static bool _copyToClipboard;

        static void Main(string[] args)
        {
            ParseCommandArgs(args);

            if (_commandType == CommandType.None)
            {
                GetHelp();
            }

            if (_commandType == CommandType.OpenConsole)
            {
                OpenConsole();
            }

            if (_commandType == CommandType.GetToken)
            {
                GetToken();
            }

            if (_commandType == CommandType.GetTemporaryCredentials)
            {
                GetTemporaryCredentials();
            }
        }

        private static void ParseCommandArgs(string[] args)
        {
            if (args.Length == 0) return;

            if (args[0] == "OpenConsole")
            {
                _commandType = CommandType.OpenConsole;
                return;
            }

            if (args[0] == "GetToken")
            {
                _commandType = CommandType.GetToken;

                if (args.Length > 1 && args[1] == "-CopyToClipboard")
                {
                    _copyToClipboard = true;
                }
                return;
            }

            if (args[0] == "GetTemporaryCredentials")
            {
                _commandType = CommandType.GetTemporaryCredentials;
            }
        }

        private static void GetHelp()
        {
            Console.WriteLine("usage:");
            Console.WriteLine("AWSConsoleUtils OpenConsole");
            Console.WriteLine("AWSConsoleUtils GetToken [-CopyToClipboard]");
            Console.WriteLine("AWSConsoleUtils GetTemporaryCredentials");
        }

        private static void OpenConsole()
        {
            var account = GetAccount();

            new AwsConsole(_credentialProfileName, _tokenProfileName, account).Open().Wait();
        }

        private static void GetTemporaryCredentials()
        {
            var account = GetAccount();

            var credentials = AwsCredentialHelper.GetTemporaryCredentials(_credentialProfileName, _tokenProfileName, account).Result;

            Console.WriteLine($"aws_access_key_id={credentials.AccessKeyId}");
            Console.WriteLine($"aws_secret_access_key={credentials.SecretAccessKey}");
            Console.WriteLine($"aws_session_token={credentials.SessionToken}");
            Console.Read();
        }

        private static AwsAccount GetAccount()
        {
            Console.WriteLine("1 - ps-paas-test");
            Console.WriteLine("2 - ps-paas-uat");
            Console.WriteLine("3 - ps-paas-prod");
            Console.WriteLine("4 - biz-test");
            Console.WriteLine("5 - biz-uat");
            Console.WriteLine("6 - biz-prod");

            Console.Write("Select an account: ");
            var option = Console.ReadLine();

            if (option == "1") return AwsAccount.XERO_PS_PAAS_TEST;
            if (option == "2") return AwsAccount.XERO_PS_PAAS_UAT;
            if (option == "3") return AwsAccount.XERO_PS_PAAS_PROD;
            if (option == "4") return AwsAccount.XERO_BIZ_TEST;
            if (option == "5") return AwsAccount.XERO_BIZ_UAT;
            if (option == "6") return AwsAccount.XERO_BIZ_PROD;

            return GetAccount();
        }

        private static void GetToken()
        {
            AmazonMFAToken token = AwsCredentialHelper.GetMFAToken(_tokenProfileName);

            if (_copyToClipboard)
            {
                CopyToClipboard(token.TokenCode);
            }

            Console.WriteLine(token.TokenCode);
        }

        private static void CopyToClipboard(string val)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"echo {val}|clip\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }

    public enum CommandType
    {
        None,
        OpenConsole,
        GetToken,
        GetTemporaryCredentials
    }
}