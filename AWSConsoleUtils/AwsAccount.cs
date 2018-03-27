using Amazon;

namespace AWSConsoleUtils
{
    public class AwsAccount
    {
        public static readonly AwsAccount PS_PAAS_TEST = new AwsAccount("[accountNumber]", RegionEndpoint.APSoutheast2);
        public static readonly AwsAccount PS_PAAS_UAT = new AwsAccount("[accountNumber]", RegionEndpoint.USWest2);
        public static readonly AwsAccount PS_PAAS_PROD = new AwsAccount("[accountNumber]", RegionEndpoint.USEast1);
        public static readonly AwsAccount BIZ_TEST = new AwsAccount("[accountNumber]", RegionEndpoint.APSoutheast2);
        public static readonly AwsAccount BIZ_UAT = new AwsAccount("[accountNumber] ", RegionEndpoint.USWest2);
        public static readonly AwsAccount BIZ_PROD = new AwsAccount("[accountNumber]", RegionEndpoint.USEast1);

        private AwsAccount(string accountNumber, RegionEndpoint region)
        {
            AccountNumber = accountNumber;
            Role = "Developer";
            Region = region;
        }

        public string AccountNumber { get; }
        public string Role { get; }
        public RegionEndpoint Region { get; }
    }
}
