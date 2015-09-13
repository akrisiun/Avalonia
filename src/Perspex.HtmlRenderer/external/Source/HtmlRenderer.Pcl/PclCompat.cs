using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Core.Handlers;

namespace TheArtOfDev.HtmlRenderer
{
    internal static class Platform
    {
        public const StringComparison DefaultStringComparison = StringComparison.OrdinalIgnoreCase;
        public static StringComparer DefaultStringComparer = StringComparer.OrdinalIgnoreCase;
        public static string[] Split(this string s, char[] chars, int limit)
        {
            var arr = s.Split(chars);
            if (arr.Length <= limit)
                return arr;
            var rv = new string[limit];
            for (var c = 0; c < limit - 1; c++)
                rv[c] = arr[c];
            rv[limit - 1] = string.Join("", arr.Skip(limit - 1));
            return rv;
        }

        public static string[] GetSegments(Uri uri)
        {
            var splitted = uri.AbsolutePath.Split('/');
            var rv = new string[splitted.Length + 1];
            rv[0] = "/";
            for (var c = 0; c < splitted.Length; c++)
                rv[c + 1] = splitted[c] + ((c != splitted.Length - 1) ? "/" : "");
            return rv;
        }

        public static string ToString(this string s, object culture)
        {
            return s;
        }
    }
}

namespace System.IO
{
    internal class FileStream : MemoryStream
    {
        public FileStream(string fullName, FileMode open, FileAccess read)
            : base(FileSystem.GetData(fullName))
        {

        }

        public string ReadToEnd()
        {
            this.Position = 0;
            var sr = new StreamReader(this);
            String s = sr.ReadToEnd();
            return s;
        }
    }

    static class FileSystem
    {
        public static Dictionary<string, byte[]> Data = new Dictionary<string, byte[]>();

        public static byte[] GetData(string file)
        {
            lock (Data)
            {
                return Data[file];
            }
        }
    }

    internal class FileInfo
    {
        public FileInfo(string path)
        {
            lock (FileSystem.Data)
            {
                byte[] data;
                if (FileSystem.Data.TryGetValue(path, out data))
                {
                    Exists = true;
                    Length = data.Length;
                }
                FullName = path;
            }
        }

        public bool Exists { get;  private set; }
        public string FullName { get; private set; }
        public int Length { get; private set; }
    }

    public static class Directory
    {
        public static bool Exists(string tempPath)
        {
            return true;
        }

        public static void CreateDirectory(string tempPath)
        {
        }
    }

    internal static class File
    {
        public static bool Exists(string s)
        {
            lock (FileSystem.Data)
            {
                return FileSystem.Data.ContainsKey(s);
            }
        }

        public static void Move(string fromPath, string to)
        {
            lock (FileSystem.Data)
            {
                byte[] data;
                if (FileSystem.Data.TryGetValue(fromPath, out data))
                {
                    FileSystem.Data.Remove(fromPath);
                    FileSystem.Data[to] = data;
                }
            }
        }

        public static FileStream Open(string source, FileMode open, FileAccess read, FileShare readWrite)
        {
            return new FileStream(source, open, read);
        }
    }

    internal static class Path
    {
        public static string Combine(string tempPath, string validFileName)
        {
            return tempPath + "/" + validFileName;
        }

        public static string GetTempFileName()
        {
            return "/temp/" + Guid.NewGuid();
        }

        public static string GetExtension(string validFileName)
        {
            var parts = GetFileName(validFileName).Split('.');
            if (parts.Length == 1)
                return null;
            return parts.Last();
        }

        public static string GetTempPath()
        {
            return "/temp/";
        }

        public static char[] GetInvalidFileNameChars()
        {
            return new[] {'/'};
        }

        public static string GetFileName(string file)
        {
            return file.Split('/').Last();
        }
    }

    internal enum FileAccess
    {
        Read
    }

    internal enum FileMode
    {
        Open
    }

    internal enum FileShare
    {
        ReadWrite
    }
}

namespace System.Net
{
    internal class WebClient : IDisposable
    {
        public class ResponseHeaderCollection : IEnumerable<string>
        {
            public Dictionary<string, string> Headers { get; private set; }

            public ResponseHeaderCollection(Dictionary<string, string> headers)
            {
                Headers = headers;
            }

            public IEnumerator<string> GetEnumerator()
            {
                return Headers.Keys.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public string this[string header]
            {
                get { return Headers[header]; }
            }
        }

        public ResponseHeaderCollection ResponseHeaders { get; private set; }
        public Encoding Encoding { get; set; }
        public event AsyncCompletedEventHandler DownloadFileCompleted = delegate { };
        public event DownloadStringCompletedEventHandler DownloadStringCompleted = delegate { };

        public WebClient()
        {
            Encoding = Encoding.UTF8;
        }

        public void Dispose()
        {
        }

        byte[] GetAsByteArray(Uri uri)
        {
            
            var res = new HttpClient().GetAsync(uri).Result;
            ProcessHeaders(res);
            return res.Content.ReadAsByteArrayAsync().Result;
        }

        void ProcessHeaders(HttpResponseMessage msg)
        {
            
        }

        void GetAsByteArrayWithCallback(Uri uri, Action<byte[], Exception> cb)
        {
            var ctx = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem( _ =>
            {
                byte[] data = null;
                Exception e = null;
                try
                {
                    data = GetAsByteArray(uri);
                }
                catch (Exception ex)
                {
                    ex = e;
                }
                if (ctx == null)
                    cb(data, e);
                else
                    ctx.Post(__ => cb(data, e), null);
            });
        }

        public void DownloadFile(Uri source, string tempPath)
        {
            lock (FileSystem.Data)
                FileSystem.Data[tempPath] = GetAsByteArray(source);
        }

        public string DownloadString(Uri uri)
        {
            var data = GetAsByteArray(uri);
            return Encoding.GetString(data, 0, data.Length);
        }

        public void DownloadFileAsync(Uri uri, string path, object state)
        {
            GetAsByteArrayWithCallback(uri, (data, e) =>
            {
                if (data != null)
                {
                    lock (FileSystem.Data)
                        FileSystem.Data[path] = data;
                }
                DownloadFileCompleted(this, new AsyncCompletedEventArgs(e, false, state));
            });
        }

        public void DownloadStringAsync(Uri uri)
        {
            GetAsByteArrayWithCallback(uri, (data, e) =>
            {
                string result = null;
                try
                {
                    if (data != null)
                    {
                        result = Encoding.GetString(data, 0, data.Length);
                    }
                }
                catch (Exception ex)
                {
                    e = ex;
                }
                DownloadStringCompleted(this, new DownloadStringCompletedEventArgs(e, false, null, result));
            });
        }

        public void CancelAsync()
        {
            //TODO: May be support that thing later
        }
    }


    internal delegate void AsyncCompletedEventHandler(Object sender, AsyncCompletedEventArgs e);

    internal delegate void DownloadStringCompletedEventHandler(object sender, DownloadStringCompletedEventArgs args);
    
    internal class DownloadStringCompletedEventArgs : AsyncCompletedEventArgs
    {
        public string Result { get; set; }

        public DownloadStringCompletedEventArgs(Exception error, bool cancelled, object userState, string result) : base(error, cancelled, userState)
        {
            Result = result;
        }
    }

}
