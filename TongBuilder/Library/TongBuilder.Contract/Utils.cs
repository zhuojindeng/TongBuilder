using System.Dynamic;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.NetworkInformation;
using System.Text;

namespace TongBuilder.Contract
{
    public sealed class Utils
    {
        #region Serialize
        public static string ToJson(object value)
        {
            if (value == null)
                return string.Empty;

            return JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        public static dynamic ToDynamic(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            var dics = FromJson<Dictionary<string, object>>(json);
            var obj = new ExpandoObject();
            foreach (var item in dics)
            {
                obj.TryAdd(item.Key, item.Value?.ToString());
            }

            return obj;
        }

        private static readonly JsonSerializerOptions dsOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        private static string FormatDSJson(string json) => json.Replace("{}", "null").Replace("\"\"", "null");

        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            try
            {
                json = FormatDSJson(json);
                return JsonSerializer.Deserialize<T>(json, dsOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{typeof(T).FullName}{Environment.NewLine}{json}{Environment.NewLine}{ex}");
                return default;
            }
        }

        public static object FromJson(Type type, string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                json = FormatDSJson(json);
                return JsonSerializer.Deserialize(json, type, dsOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{type.FullName}{Environment.NewLine}{json}{Environment.NewLine}{ex}");
                return default;
            }
        }

        public static T MapTo<T>(object value)
        {
            if (value == null)
                return default;

            if (value.GetType() == typeof(T))
                return (T)value;

            var json = ToJson(value);
            return FromJson<T>(json);
        }
        #endregion

        #region Resource
        public static string GetResource(Assembly assembly, string name)
        {
            var text = string.Empty;
            if (assembly == null || string.IsNullOrEmpty(name))
                return text;

            var names = assembly.GetManifestResourceNames();
            name = names.FirstOrDefault(n => n.Contains(name));
            if (string.IsNullOrEmpty(name))
                return text;

            var stream = assembly.GetManifestResourceStream(name);
            if (stream != null)
            {
                using var sr = new StreamReader(stream);
                text = sr.ReadToEnd();
            }
            return text;
        }

        public static string GetFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            var file = new FileInfo(fileName);
            return file.Name;
        }

        public static void EnsureFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var file = new FileInfo(fileName);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
        }

        public static void CopyFile(string sourceFileName, string destFileName, bool overwrite = true)
        {
            var info = new FileInfo(destFileName);
            if (!info.Directory.Exists)
                info.Directory.Create();

            File.Copy(sourceFileName, destFileName, overwrite);
        }

        public static string ReadFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            if (!File.Exists(path))
                return string.Empty;

            return File.ReadAllText(path);
        }

        public static void SaveFile(string path, string content)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (string.IsNullOrEmpty(content))
                return;

            var info = new FileInfo(path);
            if (!info.Directory.Exists)
                info.Directory.Create();

            File.WriteAllText(path, content);
        }

        public static void SaveFile(string path, byte[] bytes)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (bytes == null || bytes.Length == 0)
                return;

            var info = new FileInfo(path);
            if (!info.Directory.Exists)
                info.Directory.Create();

            File.WriteAllBytes(path, bytes);
        }

        public static void DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (!File.Exists(path))
                return;

            File.Delete(path);
        }
        #endregion

        public static string GetCaptcha(int length)
        {
            var chars = "abcdefghijkmnpqrstuvwxyz2345678ABCDEFGHJKLMNPQRSTUVWXYZ";
            var rnd = new Random();
            var code = "";
            for (int i = 0; i < length; i++)
            {
                code += chars[rnd.Next(chars.Length)];
            }
            return code;
        }

        #region Network
        public static bool Ping(string host, int timeout = 120)
        {
            try
            {
                var ping = new Ping();
                var options = new PingOptions { DontFragment = true };
                var data = "";
                var buffer = Encoding.UTF8.GetBytes(data);
                var reply = ping.Send(host, timeout, buffer, options);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool HasNetwork() => Ping("www.baidu.com");
        #endregion
    }
}
