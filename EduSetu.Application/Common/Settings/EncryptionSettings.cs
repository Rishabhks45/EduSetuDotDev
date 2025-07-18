using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduSetu.Application.Common.Settings;

/// <summary>
/// Strongly typed settings class for encryption configuration
/// </summary>
public class EncryptionSettings
{
    /// <summary>
    /// Master key used for encryption/decryption operations
    /// Should be a base64 encoded string for security
    /// </summary>
    public string MasterKey { get; set; } = string.Empty;

    /// <summary>
    /// Validates that the encryption settings are properly configured
    /// </summary>
    /// <returns>True if settings are valid, false otherwise</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(MasterKey) && MasterKey.Length >= 32;
    }
}
