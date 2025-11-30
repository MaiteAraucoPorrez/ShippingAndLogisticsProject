using Microsoft.Extensions.Options;
using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using System.Security.Cryptography;

namespace ShippingAndLogisticsManagement.Core.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordOptions _options;

        public PasswordService(IOptions<PasswordOptions> options)
        {
            _options = options.Value;

        }
        public string Hash(string password)
        {
            // Generate random salt
            byte[] salt = RandomNumberGenerator.GetBytes(_options.SaltSize);

            // PWD implementation
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                _options.Iterations,
                HashAlgorithmName.SHA256,
                _options.KeySize
            );

            var keyBase64 = Convert.ToBase64String(key);
            var saltBase64 = Convert.ToBase64String(salt);

            return $"{_options.Iterations}.{saltBase64}.{keyBase64}";
        }

        public bool VerifyPassword(string hash, string password)
        {
            var parts = hash.Split('.');
            if (parts.Length != 3)
            {
                throw new FormatException("Unexpected hash format. Should be formatted as `{iterations}.{salt}.{hash}`");
            }
            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);
            var keyToCheck = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                key.Length
            );
            return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
        }
    }
}
