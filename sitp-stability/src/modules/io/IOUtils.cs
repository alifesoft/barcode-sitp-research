//Copyright(c) 2026 Oleksandr Havryliuk

using System.Text;

namespace Alifesoft.SITPResearch
{
    internal static class IOUtils
    {
        /// <summary>
        /// Load file to MemoryStream
        /// </summary>
        /// <param name="filename">loading filename.</param>
        /// <returns>loaded MemoryStream</returns>
        internal static MemoryStream LoadToMemoryStream(string filename)
        {
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(filename));
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Save MemoryStream to file
        /// </summary>
        /// <param name="filename">saving filename.</param>
        /// <param name="ms">provided MemoryStream.</param>
        internal static void SaveMemoryStream(string filename, MemoryStream ms)
        {
            filename = Path.GetFullPath(filename);
            if (File.Exists(filename))
            {
                File.SetAttributes(filename, FileAttributes.Normal);
                File.Delete(filename);
            }

            var path = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(path))
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                if (!Directory.Exists(path))
                    throw new ArgumentException("Path cannot be created:" + path);
            }

            using (FileStream lFstr = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                byte[] buff = ms.ToArray();
                lFstr.Write(buff, 0, buff.Length);
            }
        }

        /// <summary>
        /// Load stream to MemoryStream
        /// </summary>
        /// <param name="stream">loading stream.</param>
        /// <returns>loaded MemoryStream</returns>
        internal static MemoryStream LoadToMemoryStream(Stream stream)
        {
            if (stream.CanSeek)
                stream.Position = 0;

            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;

            return ms;
        }

        /// <summary>
        /// Save MemoryStream to stream
        /// </summary>
        /// <param name="stream">saving stream.</param>
        /// <param name="ms">provided MemoryStream.</param>
        internal static void SaveMemoryStream(Stream stream, MemoryStream ms)
        {
            stream.SetLength(0);
            byte[] buff = ms.ToArray();
            stream.Write(buff, 0, buff.Length);

            ms.Position = 0;
            stream.Position = 0;
        }

        internal static bool DeleteFile(string filename)
        {
            if (File.Exists(filename))
            {
                File.SetAttributes(filename, FileAttributes.Normal);
                File.Delete(filename);
            }
            return !File.Exists(filename);
        }

        internal static void WriteStreamTransactional(string filename, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead) throw new ArgumentException("The stream does not support reading.", nameof(stream));
            if (stream.CanSeek) stream.Position = 0;

            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            WriteFileTransactional(filename, ms.ToArray());
        }

        internal static void WriteTextTransactional(string filename, string text)
        {
            byte[] data = new UTF8Encoding(false).GetBytes(text);
            WriteFileTransactional(filename, data);
        }

        internal static void WriteFileTransactional(string filename, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Filename is empty.", nameof(filename));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            filename = Path.GetFullPath(filename);
            string directory = Path.GetDirectoryName(filename);

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            string temporaryFilename = filename + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (FileStream stream = new FileStream(temporaryFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush(true);
                }

                File.Move(temporaryFilename, filename, true);
            }
            finally
            {
                if (File.Exists(temporaryFilename))
                    File.Delete(temporaryFilename);
            }
        }
    }
}
