using System;
using System.IO;

namespace domi1819.UpServer
{
    internal class Logger : IDisposable
    {
        private readonly StreamWriter writer;

        internal Logger(string path)
        {
            this.writer = new StreamWriter(path, true);
            this.writer.WriteLine();
            this.writer.Flush();
        }

        internal void Log(object obj, bool newLine = true)
        {
            this.Log(obj, LogLevel.Info, newLine);
        }

        internal void Log(object obj, LogLevel level, bool newLine = true)
        {
            DateTime now = DateTime.Now;

            this.writer.Write(now.Year);
            this.writer.Write('-');
            this.writer.Write(now.Month < 10 ? "0" + now.Month : now.Month.ToString());
            this.writer.Write('-');
            this.writer.Write(now.Day < 10 ? "0" + now.Day : now.Day.ToString());
            this.writer.Write('-');
            this.writer.Write(now.Hour < 10 ? "0" + now.Hour : now.Hour.ToString());
            this.writer.Write(':');
            this.writer.Write(now.Minute < 10 ? "0" + now.Minute : now.Minute.ToString());
            this.writer.Write(':');
            this.writer.Write(now.Second < 10 ? "0" + now.Month : now.Second.ToString());
            this.writer.Write(" [");
            this.writer.Write(level);
            this.writer.Write("] ");

            if (level == LogLevel.Info)
            {
                this.writer.Write("   ");
            }
            else if (level == LogLevel.Error)
            {
                this.writer.Write("  ");
            }

            this.writer.Write(obj);

            if (newLine)
            {
                this.writer.WriteLine();
            }

            this.writer.Flush();
        }

        public void Dispose()
        {
            this.writer.Close();
            this.writer.Dispose();
        }
    }

    internal enum LogLevel
    {
        Console,
        Info,
        Warning,
        Error
    };
}
