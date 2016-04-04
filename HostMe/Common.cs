using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HostMe
{
    public static class Common
    {
        private static readonly string LogPath;
        private static string _logLineDateFormat = "yyyy-MM-dd hh:mm:ss.fff tt";

        static Common()
        {
            var logsFolder = NormaliePath("logs");

            if (!Directory.Exists(logsFolder))
                Directory.CreateDirectory(logsFolder);

            LogPath = Path.Combine(logsFolder, DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt");
        }

        public static string NormaliePath(string path)
        {
            if (Path.IsPathRooted(path))
                return path;

            var codeBase = Assembly.GetExecutingAssembly().GetName().CodeBase;
            var rootPath = Path.GetDirectoryName(codeBase).Replace(@"file:\", "");
            return rootPath + @"\" + path;
        }

        public static void WriteLog(string log)
        {
            string logLine = $"{DateTime.Now.ToString(_logLineDateFormat)} | {log} \r\n\r\n";
            WriteToFile(logLine);
        }

        public static void WriteLog(Guid requestId, string log)
        {
            string logLine = $"{DateTime.Now.ToString(_logLineDateFormat)} | {requestId} | {log} \r\n\r\n";
            WriteToFile(logLine);
        }

        private async static void WriteToFile(string logLine)
        {
            try
            {
                await Task.Run(() =>
                {
                    File.AppendAllText(LogPath, logLine);
                }).ConfigureAwait(false);
            }
            catch (Exception)
            {
            }
        }
    }
}
