using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFCoreProTM.Shared.Extensions
{
    using System.Text.RegularExpressions;

public static class StringExtensions
    {
        public static string ToSlug(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            // 1. Convert to lowercase
            var slug = value.ToLowerInvariant();

            // 2. Replace invalid characters with a hyphen
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]+", "");

            // 3. Replace spaces with a single hyphen
            slug = Regex.Replace(slug, @"\s+", "-").Trim();

            // 4. Collapse consecutive hyphens
            slug = Regex.Replace(slug, @"-{2,}", "-");

            // 5. Trim hyphens from start and end
            slug = slug.Trim('-');

            return slug;
        }
    }
}