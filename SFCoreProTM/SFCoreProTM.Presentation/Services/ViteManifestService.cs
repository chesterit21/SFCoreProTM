using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace SFCoreProTM.Presentation.Services;

public class ViteManifestService
{
    private readonly IWebHostEnvironment _env;
    private Dictionary<string, ManifestChunk>? _manifest;
    private readonly ILogger<ViteManifestService> _logger;
    private readonly ConcurrentDictionary<string, IEnumerable<string>> _cssCache = new();

    public ViteManifestService(IWebHostEnvironment env, ILogger<ViteManifestService> logger)
    {
        _env = env;
        _logger = logger;
        LoadManifest();
    }

    private void LoadManifest()
    {
        // In production, the manifest is loaded once. In development, it's reloaded on each call to GetCssPathsForEntry.
        if (!_env.IsDevelopment() && _manifest != null) return;

        var manifestPath = Path.Combine(_env.WebRootPath, "pages", ".vite", "manifest.json");
        if (!File.Exists(manifestPath))
        {
            _logger.LogWarning("Vite manifest.json not found at {Path}. Run `npm run build`.", manifestPath);
            _manifest = new Dictionary<string, ManifestChunk>();
            return;
        }

        try
        {
            var manifestJson = File.ReadAllText(manifestPath);
            // Note: PropertyNameCaseInsensitive only applies to properties of ManifestChunk, not the dictionary key itself.
            // The keys in the dictionary are case-sensitive and must match the manifest.json file exactly.
            _manifest = JsonSerializer.Deserialize<Dictionary<string, ManifestChunk>>(manifestJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            _cssCache.Clear(); // Clear cache if the manifest is reloaded
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read or parse Vite manifest.json.");
            _manifest = new Dictionary<string, ManifestChunk>();
        }
    }

    public IEnumerable<string> GetCssPathsForEntry(string entry)
    {
        // In development, always reload the manifest and recalculate paths to reflect changes without restarting.
        if (_env.IsDevelopment())
        {
            LoadManifest();
            return GetCssPathsInternal(entry);
        }

        // In production, use the cache for performance.
        return _cssCache.GetOrAdd(entry, GetCssPathsInternal);
    }

    private IEnumerable<string> GetCssPathsInternal(string entry)
    {
        var cssPaths = new HashSet<string>();
        if (_manifest == null) return cssPaths;
        CollectCssPaths(entry, cssPaths, new HashSet<string>());
        return cssPaths;
    }

    private void CollectCssPaths(string chunkName, ISet<string> collectedCss, ISet<string> visitedChunks)
    {
        // Prevent infinite loops and redundant processing
        if (string.IsNullOrEmpty(chunkName) || !visitedChunks.Add(chunkName) || _manifest == null)
        {
            return;
        }

        if (_manifest.TryGetValue(chunkName, out var chunk))
        {
            // Add CSS from the current chunk
            if (chunk.Css != null)
            {
                foreach (var cssFile in chunk.Css)
                {
                    collectedCss.Add(cssFile);
                }
            }

            // Recursively process all imported chunks
            if (chunk.Imports != null)
            {
                foreach (var importName in chunk.Imports)
                {
                    // PERFORMANCE FIX: The importName from the manifest is the direct key for the next chunk.
                    // The previous FirstOrDefault loop was incorrect and very slow.
                    CollectCssPaths(importName, collectedCss, visitedChunks);
                }
            }
        }
    }

    public class ManifestChunk
    {
        [JsonPropertyName("file")]
        public string? File { get; set; }

        [JsonPropertyName("css")]
        public string[]? Css { get; set; }

        [JsonPropertyName("imports")]
        public string[]? Imports { get; set; }
    }
}
