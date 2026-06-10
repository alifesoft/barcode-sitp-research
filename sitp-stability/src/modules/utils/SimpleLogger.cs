//Copyright(c) 2026 Oleksandr Havryliuk
using System.Text;

namespace Alifesoft.SITPResearch
{
    internal enum LogMessageType { Error, Critical, Info}
    internal interface ILogMessage
    {
        void Log(string message);
        void Log(string message, LogMessageType mType);

    }

    internal abstract class SimpleLoggerCommon
    {
        protected static string StringToCSharpString(string str, bool quotes)
        {
            StringBuilder sb = new StringBuilder(str.Length + 2);
            if (quotes)
                sb.Append("\"");
            foreach (char c in str)
            {
                switch (c)
                {
                    case '\'': sb.Append(@"\'"); break;
                    case '\"': sb.Append("\\\""); break;
                    case '\\': sb.Append(@"\\"); break;
                    case '\0': sb.Append(@"\0"); break;
                    case '\a': sb.Append(@"\a"); break;
                    case '\b': sb.Append(@"\b"); break;
                    case '\f': sb.Append(@"\f"); break;
                    case '\n': sb.Append(@"\n"); break;
                    case '\r': sb.Append(@"\r"); break;
                    case '\t': sb.Append(@"\t"); break;
                    case '\v': sb.Append(@"\v"); break;
                    default:
                        // ASCII printable character
                        if (c >= 0x20 && c <= 0x7e)
                        {
                            sb.Append(c);
                            // As UTF16 escaped character
                        }
                        else
                        {
                            sb.Append(@"\u");
                            sb.Append(((int)c).ToString("x4"));
                        }
                        break;
                }
            }
            if (quotes)
                sb.Append("\"");
            return sb.ToString();
        }

        protected static string StringToHEX(byte[] array, bool quotes)
        {
            StringBuilder sb = new StringBuilder();
            char[] _hexDigits = "0123456789ABCDEF".ToCharArray();
            
            if (quotes)
                sb.Append("\"");
            
            if (null != array)
                if (0 < array.Length)
                {
                    char[] chars = new char[array.Length * 2];
                    for (int i = 0; i < array.Length; i++)
                    {
                        byte b = array[i];
                        chars[i * 2] = _hexDigits[b >> 4];
                        chars[i * 2 + 1] = _hexDigits[b & 0xF];
                    }
                    sb.Append(chars);
                }
            
            if (quotes)
                sb.Append("\"");

            return sb.ToString();
        }

        protected static string CreateLogLine(string str, LogMessageType logt)
        {
            StringBuilder sb = new StringBuilder();

            //time
            sb.Append(DateTime.Now.ToString("HH:mm:ss"));
            sb.Append(":");
            //log type
            switch (logt)
            {
                case LogMessageType.Error:
                    sb.Append("ERROR");
                    break;
                case LogMessageType.Critical:
                    sb.Append("CRIT ");
                    break;
                case LogMessageType.Info:
                    sb.Append("INFO ");
                    break;
            }
            sb.Append(":");

            //message \n
            sb.Append(str);
            sb.Append("\n");

            return sb.ToString();
        }

        protected static void WriteStringToFile(string filename, string str, bool append = true)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, append, Encoding.UTF8))
                    writer.Write(str);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
    
    internal sealed class SLog : SimpleLoggerCommon, ILogMessage
    {
        private string _filename;
        private readonly static SLog _log = new SLog("sitp.log");
        
        private SLog(string filename) 
        {
            _filename = filename;
        }
        
        public static string RootFolder
        {
            get { return _log._filename; }
            set { _log.SetFilename(value); }
        }

        private void SetFilename(string filename)
        {
            _filename = filename;
        }

        public void Log(string message) 
        {
            Log(message, LogMessageType.Info);
        }
        public void Log(string message, LogMessageType mType)
        {
            //append to file
            WriteStringToFile(DataPath.GetRootFolderPath(_filename), CreateLogLine(message, mType), true);
        }

        public static ILogMessage Logger
        { get { return _log; } }
    }
}
