using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace GravataOnlineAuth.Common
{
    public class Hash
    {
        public static string GetHash(string password, string salt)
        {
            try
            {
                return Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: Encoding.UTF8.GetString(Convert.FromBase64String(password)),
                        salt: Convert.FromBase64String(salt),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 10000,
                        numBytesRequested: 256 / 8)
                    );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public static byte[] GetSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}