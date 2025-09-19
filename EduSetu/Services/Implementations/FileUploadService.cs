using EduSetu.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;

namespace EduSetu.Services.Implementations;
public class FileValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string Base64Data { get; set; } = string.Empty;
}

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private const long MaxFileSize = 2 * 1024 * 1024;
    private static readonly string[] AllowedTypes = { ".jpg", ".jpeg", ".png", ".svg" };
    private const string ImagePath = "images";

    public FileUploadService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<FileValidationResult> ValidateFileAsync(IBrowserFile file)
    {
        var result = new FileValidationResult();

        try
        {
            if (file == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "No file selected.";
                return result;
            }

            var extension = Path.GetExtension(file.Name).ToLowerInvariant();
            if (!AllowedTypes.Contains(extension))
            {
                result.IsValid = false;
                result.ErrorMessage = $"File type {extension} not allowed. Allowed: {string.Join(", ", AllowedTypes)}";
                return result;
            }

            if (file.Size > MaxFileSize)
            {
                result.IsValid = false;
                result.ErrorMessage = $"File size exceeds {MaxFileSize / (1024 * 1024)} MB";
                return result;
            }

            // For SVG files, only validate basic format
            if (extension == ".svg")
            {
                using Stream svgStream = file.OpenReadStream(MaxFileSize);
                using var svgMemoryStream = new MemoryStream();
                await svgStream.CopyToAsync(svgMemoryStream);
                svgMemoryStream.Position = 0;

                var svgContent = System.Text.Encoding.UTF8.GetString(svgMemoryStream.ToArray());

                // Basic SVG validation - check for SVG tags
                if (!svgContent.Contains("<svg") || !svgContent.Contains("</svg>"))
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Invalid SVG file format.";
                    return result;
                }
            }

            // Read file content for base64 data
            using Stream stream = file.OpenReadStream(MaxFileSize);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var base64 = Convert.ToBase64String(memoryStream.ToArray());
            var dataUrl = $"data:{file.ContentType};base64,{base64}";

            result.IsValid = true;
            result.Base64Data = dataUrl;
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.ErrorMessage = $"File validation failed: {ex.Message}";
        }

        return result;
    }

    public async Task<string> HandleFileUploadAsync(IBrowserFile file)
    {
        var imagesDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", ImagePath);
        if (!Directory.Exists(imagesDir))
        {
            Directory.CreateDirectory(imagesDir);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
        var filePath = Path.Combine(imagesDir, fileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await using Stream uploadStream = file.OpenReadStream(MaxFileSize);
        await uploadStream.CopyToAsync(fileStream);

        var relativeUrl = $"/uploads/{ImagePath}/{fileName}";

        return relativeUrl;
    }

    public async Task<string> HandleFileUploadInByteAsync(byte[] file)
    {
        if (file == null || file.Length == 0)
        {
            return "File data is empty.";
        }

        double fileSizeInKB = file.Length / 1024.0;

        if (file.Length > MaxFileSize)
        {
            return $"File size {file.Length} bytes exceeds the maximum allowed size of {MaxFileSize} bytes.";
        }

        var fileName = $"{Guid.NewGuid()}.png";
        var imagesDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", ImagePath);

        if (!Directory.Exists(imagesDir))
        {
            Directory.CreateDirectory(imagesDir);
        }

        var filePath = Path.Combine(imagesDir, fileName);

        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await fileStream.WriteAsync(file, 0, file.Length);

        return $"/uploads/{ImagePath}/{fileName}";
    }

    public Task<bool> DeleteFileAsync(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return Task.FromResult(false);
        }

        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/'));
        var uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

        if (!IsWithinUploadsDirectory(fullPath, uploadsDir))
        {
            return Task.FromResult(false);
        }

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public bool FileExists(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return false;
        }

        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/'));
        var uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

        return IsWithinUploadsDirectory(fullPath, uploadsDir) && File.Exists(fullPath);
    }

    private static bool IsWithinUploadsDirectory(string fullPath, string uploadsDir)
    {
        return Path.GetFullPath(fullPath).StartsWith(Path.GetFullPath(uploadsDir), StringComparison.OrdinalIgnoreCase);
    }
}