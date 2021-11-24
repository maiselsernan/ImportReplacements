using System;
using System.Security.Cryptography;
using ImportReplacement.Api.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ImportReplacement.Api.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IConfiguration _configuration;

        public AuthenticationRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public  bool Authenticate(string password)
        {
            var hashedPassword = _configuration["RESTORE_METER_PASSWORD"];
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }
            return VerifyPassword(password, hashedPassword);
        }
        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                var fullHash = Convert.FromBase64String(hashedPassword);
                var valid = true;
                if (fullHash.Length != 321 || (fullHash[0] ^ (fullHash[1] ^ fullHash[2])) != 1)
                {
                    fullHash = new byte[321];
                    valid = false;
                }
                var salt = new byte[64];
                Array.Copy(fullHash, 1, salt, 0, salt.Length);
                var newHash = new Rfc2898DeriveBytes(password, salt, 10000).GetBytes(256);
                for (int i = 0, j = 65; i < 256; i++, j++) valid &= newHash[i] == fullHash[j];
                return valid;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
