using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CodexUsageCompanion.RateLimits;

/// <summary>
/// Merges renewed tokens back into the Claude CLI credentials file without disturbing the
/// fields this app does not own, replacing the file atomically so a crash cannot truncate it.
/// </summary>
public static class ClaudeCredentialsWriter
{
    private const UnixFileMode OwnerOnly = UnixFileMode.UserRead | UnixFileMode.UserWrite;

    public static void Write(string credentialsPath, ClaudeRefreshedTokens tokens)
    {
        var root = ReadRoot(credentialsPath);
        if (root["claudeAiOauth"] is not JsonObject oauth)
        {
            throw new ClaudeCredentialsFormatException("Unable to parse Claude credentials file.");
        }

        oauth["accessToken"] = tokens.AccessToken;
        oauth["refreshToken"] = tokens.RefreshToken;
        oauth["expiresAt"] = tokens.ExpiresAtUnixMs;
        if (tokens.RefreshTokenExpiresAtUnixMs is { } refreshTokenExpiresAt)
        {
            oauth["refreshTokenExpiresAt"] = refreshTokenExpiresAt;
        }

        if (tokens.Scopes is { Count: > 0 } scopes)
        {
            oauth["scopes"] = new JsonArray(scopes.Select(scope => (JsonNode)scope!).ToArray());
        }

        WriteAtomically(credentialsPath, root.ToJsonString(SerializerOptions));
    }

    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    private static JsonObject ReadRoot(string credentialsPath)
    {
        try
        {
            return JsonNode.Parse(File.ReadAllText(credentialsPath)) as JsonObject ??
                throw new ClaudeCredentialsFormatException("Unable to parse Claude credentials file.");
        }
        catch (JsonException)
        {
            throw new ClaudeCredentialsFormatException("Unable to parse Claude credentials file.");
        }
    }

    private static void WriteAtomically(string credentialsPath, string json)
    {
        var directory = Path.GetDirectoryName(credentialsPath);
        if (string.IsNullOrEmpty(directory))
        {
            directory = ".";
        }

        Directory.CreateDirectory(directory);
        var temporaryPath = Path.Combine(
            directory,
            $".{Path.GetFileName(credentialsPath)}.{Guid.NewGuid():N}.tmp");
        try
        {
            using (var stream = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                // Restrict the replacement before any token reaches the disk.
                if (!OperatingSystem.IsWindows())
                {
                    File.SetUnixFileMode(temporaryPath, OwnerOnly);
                }

                var bytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).GetBytes(json);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush(flushToDisk: true);
            }

            File.Move(temporaryPath, credentialsPath, overwrite: true);
        }
        catch
        {
            TryDelete(temporaryPath);
            throw;
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException)
        {
        }
    }
}
