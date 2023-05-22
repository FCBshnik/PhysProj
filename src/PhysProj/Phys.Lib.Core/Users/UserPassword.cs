using System.Security.Cryptography;
using System.Text;

namespace Phys.Lib.Core.Users
{
    public static class UserPassword
    {
        const int keySize = 64;
        const int iterations = 34649;
        private static readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        private static readonly byte[] salt = Encoding.UTF8.GetBytes("phys-proj-2023-salt");

        public static string HashPassword(string password)
        {
            var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations, hashAlgorithm, keySize);
            return Convert.ToHexString(hash);
        }
    }
}
