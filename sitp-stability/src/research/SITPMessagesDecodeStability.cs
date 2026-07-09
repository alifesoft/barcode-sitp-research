//Copyright(c) 2026 Oleksandr Havryliuk
using System.Text;
using System.Globalization;

namespace Alifesoft.SITPResearch
{
    internal class SITPMessagesDecodeStability
    {
        /// <summary>
        /// Scans the complete entropy dataset byte by byte and attempts to
        /// decode a valid SITP message at every possible offset.
        ///
        /// Every successfully decoded message is added to the XML storage.
        /// The scan advances by exactly one byte even after a message is found,
        /// so overlapping candidate messages are also tested.
        /// </summary>
        internal static void CheckDecodeStability(string entropyDatasetFolder, string entropyDatasetName, string messagesXmlStorage)
        {
            CheckDecodeStability(entropyDatasetFolder, entropyDatasetName, messagesXmlStorage, true);
        }

        /// <summary>
        /// Scans the complete entropy dataset byte by byte and attempts to
        /// decode a valid SITP message at every possible offset.
        /// </summary>
        /// <param name="entropyDatasetFolder"> Folder containing the entropy dataset files. </param>
        /// <param name="entropyDatasetName">       Dataset name without the .bin extension.    </param>
        /// <param name="messagesXmlStorage">   Output XML filename used to store all detected SITP messages.    </param>
        /// <param name="validateSha256"> When true, validates the dataset against its .sha256 file. </param>
        internal static void CheckDecodeStability(string entropyDatasetFolder, string entropyDatasetName, string messagesXmlStorage, bool validateSha256)
        {
            if (string.IsNullOrWhiteSpace(entropyDatasetFolder))
                throw new ArgumentException("Entropy dataset folder is empty.", nameof(entropyDatasetFolder));

            if (string.IsNullOrWhiteSpace(entropyDatasetName))
                throw new ArgumentException("Entropy dataset name is empty.", nameof(entropyDatasetName));

            if (string.IsNullOrWhiteSpace(messagesXmlStorage))
                throw new ArgumentException("Messages XML storage filename is empty.", nameof(messagesXmlStorage));

            byte[] entropyData = EntropyDataStore.Load(entropyDatasetFolder, entropyDatasetName, validateSha256);
            SitpMessageCollection messages = new SitpMessageCollection();
            SitpMessageSaverXml saver = new SitpMessageSaverXml(messagesXmlStorage, messages);

            int ProgressStep = 10000;
            for (int offset = 0; offset < entropyData.Length; ++offset)
            {
                int processedCount = offset + 1;

                if (processedCount % ProgressStep == 0)
                    Console.WriteLine("Processed elements: " + processedCount.ToString() + " / " + entropyData.Length.ToString());

                SitpMessage message = SitpMessageCodec.Decode(entropyData, offset);
                if (message == null) continue;
                messages.Add(message);

                // Save only when the collection has changed. The saver decides
                // whether the configured save interval has elapsed.
                saver.Save(false);
            }

            // Persist the final state even when no regular save was triggered
            // after the last detected message.
            saver.Save(true);
        }


        internal static void GenerateDecodeStabilityStatistics(string messagesXmlStorage, string statisticsStorage, long datasetSize)
        {
            if (string.IsNullOrWhiteSpace(messagesXmlStorage))
                throw new ArgumentException("Messages XML storage filename is empty.", nameof(messagesXmlStorage));

            if (string.IsNullOrWhiteSpace(statisticsStorage))
                throw new ArgumentException("Statistics storage filename is empty.", nameof(statisticsStorage));

            if (datasetSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(datasetSize));

            SitpMessageCollection messages = SitpMessageXmlCodec.Read(messagesXmlStorage, false);
            SitpMode[] modes =        {     SitpMode.Mode11,     SitpMode.Mode10,     SitpMode.Mode01  };
            Dictionary<SitpMode, List<int>> messageSizes = new Dictionary<SitpMode, List<int>>();

            for (int i = 0; i < modes.Length; ++i)
                messageSizes.Add(modes[i], new List<int>());

            foreach (SitpMessage message in messages.Messages)
            {
                if (message == null) continue;

                if (!messageSizes.TryGetValue(message.Mode, out List<int> sizes))
                    continue;

                sizes.Add(message.EncodedData.Length);
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# SITP Decode Stability Statistics");
            sb.AppendLine();
            sb.AppendLine("The source dataset contains random binary data and does not include intentionally encoded SITP messages.");
            sb.AppendLine("Therefore, every successfully decoded SITP message is treated as a false-positive detection.");
            sb.AppendLine();
            sb.AppendLine("- Dataset size: " + datasetSize.ToString(CultureInfo.InvariantCulture) + " bytes");
            sb.AppendLine("- Total detected messages: " + messages.Count.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine();

            for (int i = 0; i < modes.Length; ++i)
            {
                SitpMode mode = modes[i];
                List<int> sizes = messageSizes[mode];
                int detectionCount = sizes.Count;

                sb.AppendLine("## " + mode.ToString());
                sb.AppendLine();
                sb.AppendLine("- Detected messages: " + detectionCount.ToString(CultureInfo.InvariantCulture));
                sb.AppendLine("- False-positive detections: " + detectionCount.ToString(CultureInfo.InvariantCulture));

                double falsePositiveRate = detectionCount / (double)datasetSize;
                sb.AppendLine("- False-positive detections per byte: " + falsePositiveRate.ToString("G10", CultureInfo.InvariantCulture));

                if (detectionCount > 0)
                {
                    double bytesPerDetection = datasetSize / (double)detectionCount;
                    sb.AppendLine("- Average bytes per false-positive detection: " + bytesPerDetection.ToString("F3", CultureInfo.InvariantCulture));

                    int minimumSize = sizes[0];
                    int maximumSize = sizes[0];
                    long totalSize = 0;

                    for (int sizeIndex = 0; sizeIndex < sizes.Count; ++sizeIndex)
                    {
                        int size = sizes[sizeIndex];
                        if (size < minimumSize) minimumSize = size;
                        if (size > maximumSize) maximumSize = size;
                        totalSize += size;
                    }

                    double averageSize = totalSize / (double)detectionCount;
                    sb.AppendLine("- Minimum encoded message size: " + minimumSize.ToString(CultureInfo.InvariantCulture) + " bytes");
                    sb.AppendLine("- Average encoded message size: " + averageSize.ToString("F3", CultureInfo.InvariantCulture) + " bytes");
                    sb.AppendLine("- Maximum encoded message size: " + maximumSize.ToString(CultureInfo.InvariantCulture) + " bytes");
                }
                else
                {
                    sb.AppendLine("- Average bytes per false-positive detection: N/A");
                    sb.AppendLine("- Minimum encoded message size: N/A");
                    sb.AppendLine("- Average encoded message size: N/A");
                    sb.AppendLine("- Maximum encoded message size: N/A");
                }

                sb.AppendLine();
            }

            IOUtils.WriteTextTransactional(statisticsStorage, sb.ToString());
        }

    }
}
