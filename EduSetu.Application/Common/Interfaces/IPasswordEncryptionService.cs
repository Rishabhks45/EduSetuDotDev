using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduSetu.Application.Common.Interfaces
{
    /// <summary>
    /// Service for encrypting and decrypting passwords using AES encryption
    /// WARNING: This should only be used for temporary credentials, API keys, or similar use cases.
    /// This interface provides reversible password encryption for demonstration purposes.
    /// </summary>
    public interface IPasswordEncryptionService
    {
        /// <summary>
        /// Encrypts a password using AES encryption with a randomly generated salt (async version)
        /// </summary>
        /// <param name="plainTextPassword">The password to encrypt</param>
        /// <param name="masterKey">The master encryption key (should be securely stored)</param>
        /// <returns>Base64 encoded encrypted password with salt</returns>
        Task<string> EncryptPasswordAsync(string plainTextPassword, string masterKey);

        /// <summary>
        /// Decrypts an encrypted password back to plain text (async version)
        /// </summary>
        /// <param name="encryptedPassword">The encrypted password (Base64 encoded)</param>
        /// <param name="masterKey">The master encryption key used for encryption</param>
        /// <returns>The original plain text password</returns>
        Task<string> DecryptPasswordAsync(string encryptedPassword, string masterKey);

    }
}
