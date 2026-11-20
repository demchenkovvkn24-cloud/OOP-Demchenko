using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndependentWork9
{
    public class FileMetadata
    {
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; } = string.Empty;
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache = new(StringComparer.OrdinalIgnoreCase);

        public void Add(FileMetadata metadata) => _cache[metadata.FileName] = metadata;
        public FileMetadata? Get(string fileName) => _cache.TryGetValue(fileName, out var meta) ? meta : null;
        public bool Remove(string fileName) => _cache.Remove(fileName);
        public bool Contains(string fileName) => _cache.ContainsKey(fileName);
    }

    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags = new();

        public void AddTag(string articleId, string tag)
        {
            if (!_articleTags.TryGetValue(articleId, out var tags))
            {
                tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                _articleTags[articleId] = tags;
            }
            tags.Add(tag);
        }

        public void RemoveTag(string articleId, string tag)
        {
            if (_articleTags.TryGetValue(articleId, out var tags))
                tags.Remove(tag);
        }

        public HashSet<string> GetAllUniqueTags()
        {
            var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tags in _articleTags.Values)
                result.UnionWith(tags);
            return result;
        }

        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            if (!_articleTags.TryGetValue(articleId1, out var tags1) ||
                !_articleTags.TryGetValue(articleId2, out var tags2))
                return new HashSet<string>();

            var common = new HashSet<string>(tags1, StringComparer.OrdinalIgnoreCase);
            common.IntersectWith(tags2);
            return common;
        }
    }

    public class SearchIndex
    {
        private readonly Dictionary<string, HashSet<int>> _invertedIndex = new(StringComparer.OrdinalIgnoreCase);

        public void AddDocument(int docId, string content)
        {
            var words = Regex.Matches(content.ToLowerInvariant(), @"\w+").Select(m => m.Value);
            foreach (var word in words)
            {
                if (!_invertedIndex.TryGetValue(word, out var docIds))
                {
                    docIds = new HashSet<int>();
                    _invertedIndex[word] = docIds;
                }
                docIds.Add(docId);
            }
        }

        public HashSet<int> Search(string word) =>
            _invertedIndex.TryGetValue(word, out var ids) ? new HashSet<int>(ids) : new HashSet<int>();

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words.Length == 0) return new HashSet<int>();

            var result = Search(words[0]);
            for (int i = 1; i < words.Length; i++)
            {
                result.IntersectWith(Search(words[i]));
            }
            return result;
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var fileCache = new FileMetadataCache();
            fileCache.Add(new FileMetadata { FileName = "report.pdf", Size = 2048, LastModified = DateTime.Now, Hash = "ABC123" });
            Console.WriteLine($"Contains: {fileCache.Contains("REPORT.PDF")}, Hash: {fileCache.Get("report.pdf")?.Hash}");

            var tagManager = new TagManager();
            tagManager.AddTag("art1", "C#");
            tagManager.AddTag("art1", ".NET");
            tagManager.AddTag("art2", "C#");
            tagManager.AddTag("art2", "LINQ");
            Console.WriteLine("Unique tags: " + string.Join(", ", tagManager.GetAllUniqueTags()));
            Console.WriteLine("Common tags: " + string.Join(", ", tagManager.FindCommonTags("art1", "art2")));

            var searchIndex = new SearchIndex();
            searchIndex.AddDocument(1, "C# and .NET are great for backend");
            searchIndex.AddDocument(2, "Python is great for data science");
            searchIndex.AddDocument(3, "C# LINQ makes backend data processing clean");
            Console.WriteLine("Docs with 'c#' and 'backend': " + string.Join(", ", searchIndex.SearchMultiple("c#", "backend")));
        }
    }
}