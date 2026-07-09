//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal class TestEncodeDecodeSITPMessages
    {
        private const int MessageSetCount = 10;
        private const int MaximumTestPayloadSize = 150;
        private const int RandomSeed = 0x53495450;

        internal static void ReadWriteSITPMessagesAndStoreInXML(string folder)
        {
            // folder is a relative path passed to OutputPath.GetSubFoldersPath.
            if (string.IsNullOrWhiteSpace(folder))
                throw new ArgumentException("Folder is empty.", nameof(folder));

            byte[] data = CreateSITPMessagesArray();

            string hotFilename = OutputPath.GetSubFoldersPath("hot.xml", folder);
            string finalFilename = OutputPath.GetSubFoldersPath("final.xml", folder);

            SitpMessageCollection hotMessages = new SitpMessageCollection();
            SitpMessageSaverXml saver = new SitpMessageSaverXml(hotFilename, hotMessages);
            List<SitpMessage> decodedMessages = ReadSITPMessagesFromArray(data, saver);

            // Ensure that the last state is persisted even when the regular
            // save timeout has not elapsed.
            saver.Save(true);

            SitpMessageCollection reloadedMessages = SitpMessageXmlCodec.Read(hotFilename, false);

            if (reloadedMessages.Count != decodedMessages.Count)
                throw new InvalidDataException("The number of SITP messages loaded from hot.xml does not match the number of decoded messages.");

            SitpMessageXmlCodec.Write(finalFilename, reloadedMessages);
        }


        private static List<SitpMessage> ReadSITPMessagesFromArray(byte[] data, SitpMessageSaverXml saver)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (saver == null)
                throw new ArgumentNullException(nameof(saver));

            List<SitpMessage> result = new List<SitpMessage>();
            int offset = 0;
            while (offset < data.Length)
            {
                SitpMessage message = SitpMessageCodec.Decode(data, offset);

                if (message == null)
                {
                    // Move by one byte so the method can also scan an array
                    // containing unrelated or damaged data between messages.
                    ++offset;
                    continue;
                }

                result.Add(message);
                saver.Messages.Add(message);
                saver.Save(false);

                offset += message.EncodedData.Length;
            }

            return result;
        }

        private static byte[] CreateSITPMessagesArray()
        {
            Random random = new Random(RandomSeed);
            List<byte> result = new List<byte>();

            SitpMode[] modes = { SitpMode.Auto, SitpMode.Mode11, SitpMode.Mode10, SitpMode.Mode01 };

            for (int setIndex = 0; setIndex < MessageSetCount; ++setIndex)
            {
                for (int modeIndex = 0; modeIndex < modes.Length; ++modeIndex)
                {
                    SitpMode mode = modes[modeIndex];

                    int maximumPayloadSize = Math.Min(MaximumTestPayloadSize, mode == SitpMode.Auto ? MaximumTestPayloadSize : SitpModeUtils.GetModePayloadSize(mode));

                    int payloadSize = random.Next(1, maximumPayloadSize + 1);
                    byte[] payload = new byte[payloadSize];
                    random.NextBytes(payload);

                    SitpMessage message = SitpMessageCodec.Encode(mode, payload);
                    result.AddRange(message.EncodedData);
                }
            }

            return result.ToArray();
        }
    }
}
