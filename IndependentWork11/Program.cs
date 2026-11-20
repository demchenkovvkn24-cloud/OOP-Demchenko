using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndependentWork11
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "the", "is", "are", "was", "were",
            "in", "on", "at", "to", "for", "of", "and", "or"
        };

        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Enumerable.Empty<string>();

            return Regex.Matches(text.ToLowerInvariant(), @"[a-z0-9#]+")
                        .Select(m => m.Value);
        }

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words) =>
            words.Where(w => !StopWords.Contains(w));
    }

    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, Document> _documents = new();

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;

        public void AddDocument(Document doc)
        {
            _documents[doc.Id] = doc;
            var tokens = TextProcessor.RemoveStopWords(TextProcessor.Tokenize(doc.Content));

            foreach (var word in tokens)
            {
                if (!_index.TryGetValue(word, out var docIds))
                {
                    docIds = new HashSet<int>();
                    _index[word] = docIds;
                }
                docIds.Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            var normalized = TextProcessor.Tokenize(word).FirstOrDefault();
            if (normalized != null && _index.TryGetValue(normalized, out var docIds))
            {
                return docIds.Select(id => _documents[id]).ToList();
            }
            return new List<Document>();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            var tokens = words.SelectMany(TextProcessor.Tokenize).Distinct().ToList();
            if (tokens.Count == 0) return new List<Document>();

            if (!_index.TryGetValue(tokens[0], out var firstSet))
                return new List<Document>();

            var resultIds = new HashSet<int>(firstSet);
            for (int i = 1; i < tokens.Count; i++)
            {
                if (_index.TryGetValue(tokens[i], out var nextSet))
                    resultIds.IntersectWith(nextSet);
                else
                    return new List<Document>();
            }

            return resultIds.Select(id => _documents[id]).ToList();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            var tokens = words.SelectMany(TextProcessor.Tokenize).Distinct();
            var resultIds = new HashSet<int>();

            foreach (var token in tokens)
            {
                if (_index.TryGetValue(token, out var set))
                    resultIds.UnionWith(set);
            }

            return resultIds.Select(id => _documents[id]).ToList();
        }
    }

    public static class PerformanceTest
    {
        public static List<Document> GenerateDocuments(int count)
        {
            var baseTemplates = new[]
            {
                "C# is a modern object-oriented programming language developed by Microsoft.",
                "Java is a popular programming language known for its portability and performance.",
                "Python is a high-level programming language with simple syntax.",
                "C# supports LINQ, async/await, and many modern programming paradigms.",
                "C#, Java, and Python are among the most popular programming languages today."
            };

            var docs = new List<Document>(count);
            for (int i = 1; i <= count; i++)
            {
                docs.Add(new Document
                {
                    Id = i,
                    Title = $"Document #{i}",
                    Content = baseTemplates[(i - 1) % baseTemplates.Length] + $" ExtraKeyword{i % 50}"
                });
            }
            return docs;
        }

        public static void RunBenchmarks()
        {
            int[] sizes = { 10, 100, 1000, 10000 };
            string searchWord = "programming";

            foreach (int size in sizes)
            {
                var docs = GenerateDocuments(size);
                var index = new InvertedIndex();
                foreach (var doc in docs) index.AddDocument(doc);

                var sw = Stopwatch.StartNew();
                var normalized = TextProcessor.Tokenize(searchWord).First();
                var linearResults = docs.Where(d => TextProcessor.Tokenize(d.Content).Contains(normalized)).ToList();
                sw.Stop();
                long linearTicks = sw.ElapsedTicks;

                sw.Restart();
                var indexResults = index.SearchSingleWord(searchWord);
                sw.Stop();
                long indexTicks = sw.ElapsedTicks;

                Console.WriteLine($"Docs: {size,-6} | Linear: {linearTicks,-8} ticks | Index: {indexTicks,-6} ticks | Found: {linearResults.Count}/{indexResults.Count}");
            }
            Console.WriteLine();
        }
    }

    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            PerformanceTest.RunBenchmarks();

            var index = new InvertedIndex();
            var sampleDocs = new List<Document>
            {
                new() { Id = 1, Title = "Introduction to C#", Content = "C# is a modern object-oriented programming language developed by Microsoft." },
                new() { Id = 2, Title = "Java Basics", Content = "Java is a popular programming language known for its portability and performance." },
                new() { Id = 3, Title = "Python Overview", Content = "Python is a high-level programming language with simple syntax." },
                new() { Id = 4, Title = "C# Advanced Features", Content = "C# supports LINQ, async/await, and many modern programming paradigms." },
                new() { Id = 5, Title = "Comparing Languages", Content = "C#, Java, and Python are among the most popular programming languages today." }
            };
            sampleDocs.ForEach(index.AddDocument);

            Console.WriteLine($"Loaded {index.DocumentCount} documents");
            Console.WriteLine($"Index contains {index.UniqueWords} unique words\n");

            while (true)
            {
                Console.Write("Enter search query (or 'exit'): ");
                var query = Console.ReadLine();
                if (string.Equals(query, "exit", StringComparison.OrdinalIgnoreCase)) break;
                if (string.IsNullOrWhiteSpace(query)) continue;

                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var results = index.SearchMultipleWords(words);

                Console.WriteLine("\n--- Search Results (AND) ---");
                foreach (var doc in results)
                {
                    Console.WriteLine(doc);
                    Console.WriteLine($"  {doc.Content[..Math.Min(100, doc.Content.Length)]}...");
                }
                Console.WriteLine($"\nFound {results.Count} documents\n");
            }
        }
    }
}