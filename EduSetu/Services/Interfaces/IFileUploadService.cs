using EduSetu.Services.Implementations;
using Microsoft.AspNetCore.Components.Forms;

namespace EduSetu.Services.Interfaces;

public interface IFileUploadService
{
    Task<string> HandleFileUploadAsync(IBrowserFile file);
    Task<string> HandleFileUploadInByteAsync(byte[] file);
    Task<FileValidationResult> ValidateFileAsync(IBrowserFile file);
    Task<bool> DeleteFileAsync(string relativePath);
    bool FileExists(string relativePath);
}
