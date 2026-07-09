//Copyright(c) 2026 Oleksandr Havryliuk
using System.Xml;
using System.Text;

namespace Alifesoft.SITPResearch
{
    internal static class SitpMessageXmlCodec
    {
        private const string RootElement = "SitpMessages";
        private const string MessageElement = "SitpMessage";
        private const string ModeElement = "Mode";
        private const string PayloadElement = "Payload";
        private const string EncodedDataElement = "EncodedData";

        internal static void Write(Stream stream, SitpMessageCollection collection)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (!stream.CanWrite)
                throw new ArgumentException("The stream does not support writing.", nameof(stream));

            using MemoryStream memoryStream = new MemoryStream();

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                CloseOutput = false
            };

            using (XmlWriter writer = XmlWriter.Create(memoryStream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(RootElement);

                foreach (SitpMessage message in collection.Messages)
                {
                    if (message == null)
                        throw new InvalidOperationException("The SITP message collection contains null.");

                    WriteMessage(writer, message);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            memoryStream.Position = 0;

            // The target stream is modified only after the complete XML
            // document has been successfully generated.
            IOUtils.SaveMemoryStream(stream, memoryStream);
        }

        internal static void Write(string filename, SitpMessageCollection collection)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Filename is empty.", nameof(filename));

            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            filename = Path.GetFullPath(filename);
            string directory = Path.GetDirectoryName(filename);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string temporaryFilename = filename + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (FileStream stream = new FileStream(temporaryFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    Write(stream, collection);
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

        internal static SitpMessageCollection Read(string filename, bool ignoreIncorrectMessage = true)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Filename is empty.", nameof(filename));

            using FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

            return Read(stream, ignoreIncorrectMessage);
        }

        internal static SitpMessageCollection Read(Stream stream, bool ignoreIncorrectMessage = true)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanRead)
                throw new ArgumentException("The stream does not support reading.", nameof(stream));

            using MemoryStream memoryStream = IOUtils.LoadToMemoryStream(stream);

            XmlReaderSettings settings = new XmlReaderSettings
            {
                CloseInput = false,
                IgnoreComments = true,
                IgnoreWhitespace = true,
                DtdProcessing = DtdProcessing.Prohibit
            };

            SitpMessageCollection result = new SitpMessageCollection();
            using XmlReader reader = XmlReader.Create(memoryStream, settings);

            reader.MoveToContent();

            if (reader.NodeType != XmlNodeType.Element || !string.Equals(reader.Name, RootElement, StringComparison.Ordinal))
                throw new InvalidDataException("Invalid SITP XML root element.");

            if (reader.IsEmptyElement)
            {
                reader.Read();
                return result;
            }

            reader.ReadStartElement(RootElement);

            while (reader.MoveToContent() == XmlNodeType.Element)
            {
                if (!string.Equals(reader.Name, MessageElement, StringComparison.Ordinal))
                {
                    if (!ignoreIncorrectMessage)
                        throw new InvalidDataException("Unexpected XML element: " + reader.Name);

                    reader.Skip();
                    continue;
                }

                string messageXml = reader.ReadOuterXml();
                try
                {
                    result.Add(ReadMessage(messageXml));
                }
                catch (InvalidDataException)
                {
                    if (!ignoreIncorrectMessage)
                        throw;
                }
            }

            reader.ReadEndElement();

            return result;
        }

        private static void WriteMessage(XmlWriter writer, SitpMessage message)
        {
            writer.WriteStartElement(MessageElement);

            writer.WriteElementString(ModeElement, message.Mode.ToString());
            writer.WriteElementString(PayloadElement, HexConverter.ToHexString(message.Payload));
            writer.WriteElementString(EncodedDataElement, HexConverter.ToHexString(message.EncodedData));

            writer.WriteEndElement();
        }

        private static SitpMessage ReadMessage(string messageXml)
        {
            if (string.IsNullOrEmpty(messageXml))
                throw new InvalidDataException("SITP message XML is empty.");

            XmlReaderSettings settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
                DtdProcessing = DtdProcessing.Prohibit
            };

            string modeText = null;
            string payloadText = null;
            string encodedDataText = null;

            using StringReader stringReader = new StringReader(messageXml);
            using XmlReader reader = XmlReader.Create(stringReader, settings);

            reader.MoveToContent();

            if (reader.NodeType != XmlNodeType.Element || !string.Equals(reader.Name, MessageElement, StringComparison.Ordinal))
                throw new InvalidDataException("Invalid SITP message XML element.");

            if (reader.IsEmptyElement)
                throw new InvalidDataException("SITP message XML element is empty.");

            reader.ReadStartElement(MessageElement);

            while (reader.MoveToContent() == XmlNodeType.Element)
            {
                if (string.Equals(reader.Name, ModeElement, StringComparison.Ordinal))
                {
                    if (modeText != null)
                        throw new InvalidDataException("Duplicate Mode element.");

                    modeText = reader.ReadElementContentAsString();
                }
                else if (string.Equals(reader.Name, PayloadElement, StringComparison.Ordinal))
                {
                    if (payloadText != null)
                        throw new InvalidDataException("Duplicate Payload element.");

                    payloadText = reader.ReadElementContentAsString();
                }
                else if (string.Equals(reader.Name, EncodedDataElement, StringComparison.Ordinal))
                {
                    if (encodedDataText != null)
                        throw new InvalidDataException("Duplicate EncodedData element.");

                    encodedDataText = reader.ReadElementContentAsString();
                }
                else
                {
                    reader.Skip();
                }
            }

            reader.ReadEndElement();

            if (modeText == null)
                throw new InvalidDataException("Mode element is missing.");

            if (payloadText == null)
                throw new InvalidDataException("Payload element is missing.");

            if (encodedDataText == null)
                throw new InvalidDataException("EncodedData element is missing.");

            if (!Enum.TryParse(modeText, false, out SitpMode storedMode) || !Enum.IsDefined(typeof(SitpMode), storedMode) || storedMode == SitpMode.Auto)
                throw new InvalidDataException("Unknown or unsupported SITP mode: " + modeText);

            byte[] storedPayload;
            byte[] encodedData;

            try
            {
                storedPayload = HexConverter.FromHexString(payloadText);
                encodedData = HexConverter.FromHexString(encodedDataText);
            }
            catch (Exception exception)
            {
                throw new InvalidDataException("Invalid hexadecimal data in the SITP XML.", exception);
            }

            SitpMessage decodedMessage = SitpMessageCodec.Decode(encodedData);

            if (decodedMessage == null)
                throw new InvalidDataException("EncodedData does not contain a valid SITP message.");

            if (decodedMessage.Mode != storedMode)
                throw new InvalidDataException("The stored SITP mode does not match EncodedData.");

            if (!decodedMessage.Payload.SequenceEqual(storedPayload))
                throw new InvalidDataException("The stored payload does not match EncodedData.");

            return decodedMessage;
        }
    }
}
