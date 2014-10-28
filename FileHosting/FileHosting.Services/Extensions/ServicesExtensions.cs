using System;
using System.Collections.Generic;
using System.Linq;
using FileHosting.Database.Models;

namespace FileHosting.Services.Extensions
{
    public static class ServicesExtensions
    {
        public static string ToTagsString(this IEnumerable<Tag> tags)
        {
            string tagsString = tags.Aggregate("", (current, tag) => current + ", " + tag.Name);

            tagsString = tagsString.Trim(new[] {'.', ',', ';', ':', '!', '?', '@', '#', '$', ' '});

            return tagsString;
        }

        public static string[] ToTagsArray(this string fileTagsString)
        {
            string[] tags = fileTagsString.Split(new[] {'.', ',', ';', ':', '!', '?', '@', '#', '$', ' '},
                StringSplitOptions.RemoveEmptyEntries);

            return tags.Any() ? tags.Select(tag => tag.ToLowerInvariant()).ToArray() : null;
        }
    }
}