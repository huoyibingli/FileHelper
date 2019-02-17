using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHelper
{
    public class FileBaseClass
    {
        public static bool _initialized = false;
        public static log4net.ILog log;
        /// 将 Stream 转成 byte[]
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
        /// 初始化日志
        /// </summary>
        private static void InitLog()
        {
            //初始化日志文件 
            if (!_initialized)
            {
                var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                var fi = new System.IO.FileInfo(path);
                log4net.Config.XmlConfigurator.Configure(fi);

                log4net.Appender.RollingFileAppender appender = new log4net.Appender.RollingFileAppender();
                appender.File = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
                appender.AppendToFile = true;
                appender.MaxSizeRollBackups = 3;
                appender.MaximumFileSize = "1MB";
                appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Size;
                appender.StaticLogFileName = true;
                appender.Layout = new log4net.Layout.PatternLayout("%date [%thread] %-5level - %message%newline");
                appender.ActivateOptions();
                log4net.Config.BasicConfigurator.Configure(appender);
                log = log4net.LogManager.GetLogger("RollingLogFile");
                _initialized = true;
            }
        }
    }
}
