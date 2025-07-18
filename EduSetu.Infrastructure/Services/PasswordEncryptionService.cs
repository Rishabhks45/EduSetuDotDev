using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EduSetu.Application.Common.Interfaces;

namespace EduSetu.Infrastructure.Services
{
    /// <summary>
    /// Service for securely encrypting and decrypting passwords using AES-256-GCM
    /// WARNING: This should only be used for temporary credentials, API keys, or similar use cases.
    /// This service provides reversible password encryption for demonstration purposes.
    /// </summary>
    public class PasswordEncryptionService : IPasswordEncryptionService
    {
        private const int KeySize = 32; // 256 bits
        private const int IvSize = 12; // 96 bits for GCM
        private const int SaltSize = 16; // 128 bits
        private const int TagSize = 16; // 128 bits for GCM authentication tag
        private const int IterationCount = 100000; // PBKDF2 iterations

        public PasswordEncryptionService()
        {
        }

        /// <summary>
        /// Encrypts a password using AES-256-GCM with PBKDF2 key derivation
        /// </summary>
        /// <param name="plainTextPassword">The password to encrypt</param>
        /// <param name="masterKey">The master encryption key</param>
        /// <returns>Base64 encoded encrypted data (salt + iv + tag + ciphertext)</returns>
        public string EncryptPassword(string plainTextPassword, string masterKey)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
            {
                throw new ArgumentException("Password cannot be null or empty", nameof(plainTextPassword));
            }

            if (string.IsNullOrWhiteSpace(masterKey))
            {
                throw new ArgumentException("Master key cannot be null or empty", nameof(masterKey));
            }

            // Generate random salt and IV
            byte[] salt = new byte[SaltSize];
            byte[] iv = new byte[IvSize];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
                rng.GetBytes(iv);
            }

            // Derive encryption key from master key using PBKDF2
            byte[] derivedKey = DeriveKey(masterKey, salt);

            // Convert password to bytes
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainTextPassword);

            // Encrypt using AES-GCM
            byte[] ciphertext = new byte[plainTextBytes.Length];
            byte[] tag = new byte[TagSize];

            using (AesGcm aes = new AesGcm(derivedKey, TagSize))
            {
                aes.Encrypt(iv, plainTextBytes, ciphertext, tag);
            }

            // Combine all components: salt + iv + tag + ciphertext
            byte[] result = new byte[SaltSize + IvSize + TagSize + ciphertext.Length];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(iv, 0, result, SaltSize, IvSize);
            Buffer.BlockCopy(tag, 0, result, SaltSize + IvSize, TagSize);
            Buffer.BlockCopy(ciphertext, 0, result, SaltSize + IvSize + TagSize, ciphertext.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Encrypts a password using AES-256-GCM with PBKDF2 key derivation (async version)
        /// </summary>
        /// <param name="plainTextPassword">The password to encrypt</param>
        /// <param name="masterKey">The master encryption key</param>
        /// <returns>Base64 encoded encrypted data (salt + iv + tag + ciphertext)</returns>
        public async Task<string> EncryptPasswordAsync(string plainTextPassword, string masterKey)
        {
            // Use Task.Run to offload the CPU-intensive encryption to a background thread
            return await Task.Run(() => EncryptPassword(plainTextPassword, masterKey));
        }

        /// <summary>
        /// Decrypts an encrypted password back to plain text
        /// </summary>
        /// <param name="encryptedPassword">The encrypted password (Base64 encoded)</param>
        /// <param name="masterKey">The master encryption key</param>
        /// <returns>The original plain text password</returns>
        public string DecryptPassword(string encryptedPassword, string masterKey)
        {
            if (string.IsNullOrWhiteSpace(encryptedPassword))
            {
                throw new ArgumentException("Encrypted password cannot be null or empty", nameof(encryptedPassword));
            }

            if (string.IsNullOrWhiteSpace(masterKey))
            {
                throw new ArgumentException("Master key cannot be null or empty", nameof(masterKey));
            }

            // Decode from Base64
            byte[] encryptedData = Convert.FromBase64String(encryptedPassword);

            // Validate minimum length
            if (encryptedData.Length < SaltSize + IvSize + TagSize + 1)
            {
                throw new ArgumentException("Invalid encrypted password format");
            }

            // Extract components
            byte[] salt = new byte[SaltSize];
            byte[] iv = new byte[IvSize];
            byte[] tag = new byte[TagSize];
            byte[] ciphertext = new byte[encryptedData.Length - SaltSize - IvSize - TagSize];

            Buffer.BlockCopy(encryptedData, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(encryptedData, SaltSize, iv, 0, IvSize);
            Buffer.BlockCopy(encryptedData, SaltSize + IvSize, tag, 0, TagSize);
            Buffer.BlockCopy(encryptedData, SaltSize + IvSize + TagSize, ciphertext, 0, ciphertext.Length);

            // Derive the same key using the extracted salt
            byte[] derivedKey = DeriveKey(masterKey, salt);

            // Decrypt using AES-GCM
            byte[] plainTextBytes = new byte[ciphertext.Length];
            using (AesGcm aes = new AesGcm(derivedKey, TagSize))
            {
                aes.Decrypt(iv, ciphertext, tag, plainTextBytes);
            }

            string result = Encoding.UTF8.GetString(plainTextBytes);
            return result;
        }

        /// <summary>
        /// Decrypts an encrypted password back to plain text (async version)
        /// </summary>
        /// <param name="encryptedPassword">The encrypted password (Base64 encoded)</param>
        /// <param name="masterKey">The master encryption key</param>
        /// <returns>The original plain text password</returns>
        public async Task<string> DecryptPasswordAsync(string encryptedPassword, string masterKey)
        {
            // Use Task.Run to offload the CPU-intensive decryption to a background thread
            return await Task.Run(() => DecryptPassword(encryptedPassword, masterKey));
        }

        /// <summary>
        /// Derives an encryption key from the master key using PBKDF2
        /// </summary>
        /// <param name="masterKey">The master key</param>
        /// <param name="salt">The salt for key derivation</param>
        /// <returns>Derived encryption key</returns>
        private byte[] DeriveKey(string masterKey, byte[] salt)
        {
            byte[] masterKeyBytes = Convert.FromBase64String(masterKey);
            using Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(masterKeyBytes, salt, IterationCount, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(KeySize);
        }
    }
}
