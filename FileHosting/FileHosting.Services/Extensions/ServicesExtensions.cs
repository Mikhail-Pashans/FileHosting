using System;
using FileHosting.Database.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHosting.Services.Extensions
{
    public static class ServicesExtensions
    {
        public static string ToTagsString(this IEnumerable<Tag> tags)
        {
            var tagsString = tags.Aggregate("", (current, tag) => current + ", " + tag.Name);

            tagsString = tagsString.Trim(new[] { '.', ',', ';', '!', '?', ' ' });

            return tagsString;
        }

        public static List<string> ToTagsList(this string fileTagsString)
        {
            var tags = fileTagsString.Split(new[] {'.', ',', ';', '!', '?', ' '}, StringSplitOptions.RemoveEmptyEntries);
            if (!tags.Any())
                return null;

            var fileTagsList = new List<string>();
            
            fileTagsList.AddRange(tags);

            return fileTagsList;
        }
    }
}