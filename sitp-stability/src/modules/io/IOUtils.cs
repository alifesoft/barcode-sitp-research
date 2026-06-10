//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal class IOUtils
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
    }
}
