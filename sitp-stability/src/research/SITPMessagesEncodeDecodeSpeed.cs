//Copyright(c) 2026 Oleksandr Havryliuk
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace Alifesoft.SITPResearch
{
    internal class SITPMessagesEncodeDecodeSpeed
    {
        private const int RandomSeed = 0x53495450;

        internal static void EncodeDecodeSITPMessagesSpeed(string filename, string system)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# SITP Encode/Decode Performance Test");
            sb.AppendLine();
            sb.AppendLine("The test measures the average execution time of complete SITP message encoding and decoding operations.");
            sb.AppendLine("The measured time includes memory allocation, generation of the LCG mask, XOR masking, CRC calculation or validation, and message object construction.");
            sb.AppendLine("A preliminary cold run is performed before each measurement and is not included in the reported results.");
            sb.AppendLine();
            sb.AppendLine("## Test environment");
            sb.AppendLine();
            sb.AppendLine(system);
            sb.AppendLine();
            sb.AppendLine("- Random payload seed: `0x" + RandomSeed.ToString("X8", CultureInfo.InvariantCulture) + "`");

            TestPayloadSpeed(sb, SitpMode.Mode11, 100000);
            TestPayloadSpeed(sb, SitpMode.Mode10, 1000);
            TestPayloadSpeed(sb, SitpMode.Mode01, 100);

            IOUtils.WriteTextTransactional(filename, sb.ToString());
        }

        private static void TestPayloadSpeed(StringBuilder sb, SitpMode mode, int iterations)
        {
            if (mode == SitpMode.Auto)
                mode = SitpMode.Mode01;

            int payloadSize = SitpModeUtils.GetModePayloadSize(mode);
            TestPayloadSpeed(sb, mode, payloadSize, iterations);
        }

        private static void TestPayloadSpeed(StringBuilder sb, SitpMode mode, int payloadSize, int iterations)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));

            if (mode == SitpMode.Auto)
                throw new ArgumentException("A concrete SITP mode is required.", nameof(mode));

            if (payloadSize < 0 || payloadSize > SitpModeUtils.GetModePayloadSize(mode))
                throw new ArgumentOutOfRangeException(nameof(payloadSize));

            if (iterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(iterations));

            Stopwatch sw = new Stopwatch();
            Random random = new Random(RandomSeed);

            byte[] payload = new byte[payloadSize];
            random.NextBytes(payload);

            // Perform an unmeasured run to initialize the relevant code paths.
            SitpMessage message = SitpMessageCodec.Encode(mode, payload);

            message = SitpMessageCodec.Decode(message.EncodedData);

            if (message == null)
                throw new InvalidOperationException("The SITP message could not be decoded.");

            // Measure complete SITP message encoding, including allocations,
            // LCG masking, CRC calculation, and object construction.
            sw.Restart();
            for (int i = 0; i < iterations; ++i)
                message = SitpMessageCodec.Encode(mode, payload);
            sw.Stop();

            double encodeMicroseconds = sw.Elapsed.TotalMilliseconds * 1000.0 / iterations;

            SitpMessage messageForDecode = SitpMessageCodec.Encode(mode, payload);

            // Measure complete SITP message decoding, including header parsing,
            // allocations, LCG masking, CRC validation, and object construction.
            sw.Restart();
            for (int i = 0; i < iterations; ++i)
                message = SitpMessageCodec.Decode(messageForDecode.EncodedData);
            sw.Stop();

            if (message == null)
                throw new InvalidOperationException("The SITP message could not be decoded.");

            double decodeMicroseconds = sw.Elapsed.TotalMilliseconds * 1000.0 / iterations;

            sb.AppendLine();
            sb.AppendLine("## " + mode.ToString());
            sb.AppendLine();
            sb.AppendLine("- Payload size: " + payloadSize.ToString(CultureInfo.InvariantCulture) + " bytes");
            sb.AppendLine("- Encoded message size: " + messageForDecode.EncodedData.Length.ToString(CultureInfo.InvariantCulture) + " bytes");
            sb.AppendLine("- Iterations: " + iterations.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine("- Average encode time: " + encodeMicroseconds.ToString("F3", CultureInfo.InvariantCulture) + " microseconds");
            sb.AppendLine("- Average decode time: " + decodeMicroseconds.ToString("F3", CultureInfo.InvariantCulture) + " microseconds");
        }

    }
}
