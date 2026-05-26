using System.Security.Cryptography;
using System.Text;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace RecipeBook.Services;

public class RecipeShareService
{
    private readonly HttpClient _httpClient;

    public RecipeShareService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ShareRecipeAsync(string title, string ingredients, string instructions, string? imageSource)
    {
        var recipeText = BuildRecipeText(title, ingredients, instructions);
        var files = new List<ShareFile>();

        var imageFile = await PrepareImageShareFileAsync(imageSource);
        if (imageFile is not null)
        {
            files.Add(imageFile);
        }

        var textFile = await CreateRecipeTextFileAsync(title, recipeText);
        files.Add(textFile);

        if (files.Count > 0)
        {
            await Share.Default.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = "Recept megosztása",
                Files = files
            });
            return;
        }

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Recept megosztása",
            Text = recipeText
        });
    }

    private static string BuildRecipeText(string title, string ingredients, string instructions)
    {
        return $"{title}\n\nHozzávalók:\n{ingredients}\n\nElkészítés:\n{instructions}";
    }

    private async Task<ShareFile?> PrepareImageShareFileAsync(string? imageSource)
    {
        if (string.IsNullOrWhiteSpace(imageSource))
        {
            return null;
        }

        try
        {
            if (Uri.TryCreate(imageSource, UriKind.Absolute, out var uri))
            {
                if (uri.IsFile)
                {
                    return await CopyLocalFileForSharingAsync(uri.LocalPath, "image");
                }

                if (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase)
                    || uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                {
                    return await DownloadRemoteFileForSharingAsync(uri);
                }
            }

            if (File.Exists(imageSource))
            {
                return await CopyLocalFileForSharingAsync(imageSource, "image");
            }
        }
        catch
        {
            // Ignore image-specific failure and fallback to text sharing.
        }

        return null;
    }

    private async Task<ShareFile?> DownloadRemoteFileForSharingAsync(Uri uri)
    {
        var extension = Path.GetExtension(uri.AbsolutePath);
        if (string.IsNullOrWhiteSpace(extension) || extension.Length > 6)
        {
            extension = ".jpg";
        }

        var path = BuildShareCachePath($"remote_{Hash(uri.ToString())}{extension}");

        if (!File.Exists(path))
        {
            var bytes = await _httpClient.GetByteArrayAsync(uri);
            await File.WriteAllBytesAsync(path, bytes);
        }

        return new ShareFile(path);
    }

    private async Task<ShareFile?> CopyLocalFileForSharingAsync(string sourcePath, string prefix)
    {
        if (!File.Exists(sourcePath))
        {
            return null;
        }

        var extension = Path.GetExtension(sourcePath);
        var targetPath = BuildShareCachePath($"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}{extension}");

        await using var source = File.OpenRead(sourcePath);
        await using var target = File.Create(targetPath);
        await source.CopyToAsync(target);

        return new ShareFile(targetPath);
    }

    private async Task<ShareFile> CreateRecipeTextFileAsync(string title, string text)
    {
        var safeTitle = MakeSafeFileName(title);
        var path = BuildShareCachePath($"recipe_{safeTitle}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        await File.WriteAllTextAsync(path, text, Encoding.UTF8);
        return new ShareFile(path);
    }

    private static string BuildShareCachePath(string fileName)
    {
        var folder = Path.Combine(FileSystem.CacheDirectory, "sharing-root");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, fileName);
    }

    private static string MakeSafeFileName(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            sb.Append(Array.IndexOf(invalid, c) >= 0 ? '_' : c);
        }

        var result = sb.ToString().Trim();
        return string.IsNullOrWhiteSpace(result) ? "recipe" : result;
    }

    private static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes)[..16].ToLowerInvariant();
    }
}
