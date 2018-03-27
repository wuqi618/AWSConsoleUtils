using System;
using System.Linq;
using System.Security.Cryptography;

namespace AWSConsoleUtils
{
    public class AmazonMFA
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const int _timeCounter = 30;

        private readonly string _serialNumber;
        private readonly string _secret;

        public AmazonMFA(string serialNumber, string secret)
        {
            _serialNumber = serialNumber;
            _secret = secret;
        }

        public AmazonMFAToken GetToken()
        {
            var counter = GetCounter();

            return new AmazonMFAToken
            {
                SerialNumber = _serialNumber,
                TokenCode = ComputeCode(_secret, counter),
                Expiration = _epoch.AddSeconds((counter + 1) * _timeCounter)
            };
        }

        private long GetCounter()
        {
            return (long)(DateTime.UtcNow - _epoch).TotalSeconds / _timeCounter;
        }

        private string ComputeCode(string secret, long counter)
        {
            var buffer = Base32.Decode(secret);

            var hash = new HMACSHA1(buffer).ComputeHash(BitConverter.GetBytes(counter).Reverse().ToArray());

            var offset = hash.Last() & 0x0F;

            var truncatedHash = ((hash[offset] & 0x7f) << 24)
                                | (hash[offset + 1] << 16)
                                | (hash[offset + 2] << 8)
                                | hash[offset + 3];

            var code = (truncatedHash % 1000000).ToString().PadLeft(6, '0');

            return code;
        }
    }

    public class AmazonMFAToken
    {
        public string SerialNumber { get; set; }
        public string TokenCode { get; set; }
        public DateTime Expiration { get; set; }
    }
}
