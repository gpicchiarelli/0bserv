using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using _0bserv.Models;
using Microsoft.EntityFrameworkCore;

namespace _0bserv.Services
{
    public class CredenzialiMailService
    {
        private readonly _0bservDbContext _dbContext;

        public CredenzialiMailService(_0bservDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddEmailCredentialsAsync(CredenzialiEmail credentials, string password)
        {
            // Crittografa la password prima di memorizzarla nel database
            byte[] encryptedPassword = EncryptPassword(password);
            credentials.EncryptedPassword = encryptedPassword;

            _dbContext.CredenzialiEmails.Add(credentials);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> GetDecryptedPasswordAsync(int id)
        {
            var credentials = await _dbContext.CredenzialiEmails.FindAsync(id);
            if (credentials != null)
            {
                // Decripta la password memorizzata nel database
                return DecryptPassword(credentials.EncryptedPassword);
            }
            return null;
        }

        private byte[] EncryptPassword(string password)
        {
            using (var aesAlg = Aes.Create())
            {
                var key = aesAlg.Key;
                var iv = aesAlg.IV;
                using (var encryptor = aesAlg.CreateEncryptor(key, iv))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(password);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        private string DecryptPassword(byte[] encryptedPassword)
        {
            using (var aesAlg = Aes.Create())
            {
                var key = aesAlg.Key;
                var iv = aesAlg.IV;
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                using (var msDecrypt = new MemoryStream(encryptedPassword))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
