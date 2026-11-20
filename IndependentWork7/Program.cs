using IndependentWork7;

var cache = new Cache<string, string>(3);

cache.Add("a", "Apple");
cache.Add("b", "Banana");
cache.Add("c", "Cherry");
Console.WriteLine($"Count: {cache.Count}");
Console.WriteLine($"Get a: {cache.Get("a")}");

cache.Add("d", "Date");
Console.WriteLine($"Count after overflow: {cache.Count}");
Console.WriteLine($"Contains a: {cache.ContainsKey("a")}");
Console.WriteLine($"Contains b: {cache.ContainsKey("b")}");
Console.WriteLine($"Contains d: {cache.ContainsKey("d")}");

cache.Add("e", "Elderberry");
Console.WriteLine($"Contains b: {cache.ContainsKey("b")}");
Console.WriteLine($"Contains c: {cache.ContainsKey("c")}");

try
{
    cache.Get("a");
}
catch (KeyNotFoundException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}