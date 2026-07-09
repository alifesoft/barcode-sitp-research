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
