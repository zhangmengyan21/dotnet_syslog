namespace Test;

using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Test.Program.Syslog;

public static partial class Program
{
    internal static partial class Syslog
    {
        [Flags]
        internal enum Option
        {
            NoDelay = 0x08,
        }

        [Flags]
        internal enum Facility
        {
            LOG_LOCAL0 = 16 << 3
        }

        [Flags]
        public enum Level
        {
            Info = 6,
        }


        [LibraryImport("libc")]
        internal static partial void openlog(IntPtr ident, Option option, Facility facility);

        [LibraryImport("libc", StringMarshalling = StringMarshalling.Utf16)]
        internal static partial void syslog(int priority, string message);

        [LibraryImport("libc")]
        internal static partial void closelog();
    }

    const int STRING_LENGTH = 32;
    const int RUN_TIMES = 100000;

    public static void Main(string[] args)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int j = 0; j < STRING_LENGTH; j++)
        {
            stringBuilder.Append('a');
        }
        string msg = stringBuilder.ToString();
        var tag = Marshal.StringToHGlobalAnsi("server_1");
        
        Stopwatch sw = new();
        sw.Start();
        
        openlog(tag, Option.NoDelay, Facility.LOG_LOCAL0);
        for (int i = 0; i < RUN_TIMES; i++) {
            syslog((int)Level.Info, msg);
        }
        
        sw.Stop();
        Console.WriteLine($"{STRING_LENGTH}:" + sw.ElapsedMilliseconds);
    }
}
