using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ElateService.BLL.Services
{
    public static class CryptoService
    {
        public static string CreatePasswordHash(string password, out string dynamicSalt)
        {
            byte[] passwordInBytes = Encoding.UTF8.GetBytes(password);
            
            dynamicSalt = GenerateDynamicSalt();
            byte[] dynamicSaltInBytes = Encoding.UTF8.GetBytes(dynamicSalt);

            string globalSalt = Properties.Settings.Default.GlobalSalt;
            byte[] globalSaltInBytes = Encoding.UTF8.GetBytes(globalSalt);

            var sha = new SHA512CryptoServiceProvider();
            var hashOfPassword = sha.ComputeHash(passwordInBytes);

            var firstAddition = hashOfPassword.Concat(dynamicSaltInBytes).ToArray();
            var hashOfPasswordAndDS = sha.ComputeHash(firstAddition);

            var secondAddition = hashOfPasswordAndDS.Concat(globalSaltInBytes).ToArray();
            var passwordHash = sha.ComputeHash(secondAddition);

            return ByteArrayToString(passwordHash);
        }


        public static bool VerifyPassword(string hashPassword, string salt, string password)
        {
            byte[] passwordInBytes = Encoding.UTF8.GetBytes(password);            
            byte[] dynamicSaltInBytes = Encoding.UTF8.GetBytes(salt);
            string globalSalt = Properties.Settings.Default.GlobalSalt;
            byte[] globalSaltInBytes = Encoding.UTF8.GetBytes(globalSalt);

            var sha = new SHA512CryptoServiceProvider();
            var hashOfPassword = sha.ComputeHash(passwordInBytes);

            var firstAddition = hashOfPassword.Concat(dynamicSaltInBytes).ToArray();
            var hashOfPasswordAndDS = sha.ComputeHash(firstAddition);

            var secondAddition = hashOfPasswordAndDS.Concat(globalSaltInBytes).ToArray();
            var passwordHash = sha.ComputeHash(secondAddition);
            string passwordHashString = ByteArrayToString(passwordHash);

            return passwordHashString.Equals(hashPassword);
        }


        public static string GenerateDynamicSalt()
        {
            RNGCryptoServiceProvider rncCsp = new RNGCryptoServiceProvider();
            byte[] DynamicSaltInBytes = new byte[256];
            rncCsp.GetBytes(DynamicSaltInBytes);
            string dynamicSalt = ByteArrayToString(DynamicSaltInBytes);

            return dynamicSalt;
        }


        public static string GenerateConfirmationCode()
        {
            RNGCryptoServiceProvider rncCsp = new RNGCryptoServiceProvider();
            byte[] RandomCodeInBytes = new byte[64];
            rncCsp.GetBytes(RandomCodeInBytes);
            string randomCode = ByteArrayToString(RandomCodeInBytes);

            return randomCode;
        }


        private static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder output = new StringBuilder(arrInput.Length);
            for (i = 0; i < arrInput.Length - 1; i++)
            {
                output.Append(arrInput[i].ToString("X2"));
            }
            return output.ToString();
        }
    }
}
