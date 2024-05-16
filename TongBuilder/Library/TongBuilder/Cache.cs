using System.Collections.Concurrent;
namespace TongBuilder;

public sealed class Cache
{    
    private static readonly ConcurrentDictionary<string, object> cached = new();

    private Cache() { }

    public static T Get<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            return default;

        if (!cached.TryGetValue(key, out object value))
            return default;

        return (T)value;
    }

    public static void Set(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            return;

        cached[key] = value;
    }

    public static void Remove(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        if (!cached.ContainsKey(key))
            return;

        cached.TryRemove(key, out object _);
    }    
}

public class CodeInfo
{
    public CodeInfo(string code, object data = null) : this(code, code, data) { }
    public CodeInfo(string code, string name, object data = null) : this("", code, name, data) { }

    public CodeInfo(string category, string code, string name, object data = null)
    {
        Category = category;
        Code = code;
        Name = name;
        Data = data;
    }

    public string Category { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public object Data { get; set; }
    
}