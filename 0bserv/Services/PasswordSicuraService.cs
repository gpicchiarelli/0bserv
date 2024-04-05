using System;
using System.Security.Cryptography;

namespace _0bserv.Services
{
    public interface IPasswordSicuraService
    {
        string GeneratePassword(int length);
    }

    public class PasswordSicuraService : IPasswordSicuraService
    {
        private const string ValidChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+";

        public string GeneratePassword(int length)
        {
            // Genera un array di byte randomici utilizzando RandomNumberGenerator
            byte[] randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Genera una password utilizzando i byte randomici e i caratteri validi
            char[] password = new char[length];
            for (int i = 0; i < length; i++)
            {
                int index = (int)(BitConverter.ToUInt32(randomBytes, i) % ValidChars.Length);
                password[i] = ValidChars[index];
            }
            return new string(password);
        }
    }
}