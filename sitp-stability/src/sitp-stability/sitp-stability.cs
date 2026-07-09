//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal class SitpStabilityProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SITP stability research started.");

            // Generate the reproducible 16 MiB random baseline dataset
            // used for false-positive detection experiments.
            //EntropyDataStore.GenerateAndSave(OutputPath.GetSubFoldersPath("dataset"), "sitp-baseline", 16777216);
            //Console.WriteLine("Baseline dataset generation completed.");

            // Measure complete SITP encoding and decoding performance,
            // including memory allocation, LCG masking, CRC processing,
            // and SitpMessage object construction.
            //SITPMessagesEncodeDecodeSpeed.EncodeDecodeSITPMessagesSpeed(OutputPath.GetSubFoldersPath("sitp-encode-decode-speed.txt", "research"), "Ryzen 9 9900X, single core");
            //Console.WriteLine("Performance testing completed.");

            // Scan the complete random dataset byte by byte and store every
            // accidentally detected valid SITP message as a false positive.
            //SITPMessagesDecodeStability.CheckDecodeStability(DataPath.GetSubFoldersPath("dataset"), "sitp-baseline", OutputPath.GetSubFoldersPath("sitp-decode-stability.xml", "research"));
            //Console.WriteLine("Decode stability testing completed.");

            //Aggregate false-positive detection statistics by SITP mode,
            //including detection counts and minimum, average, and maximum
            //encoded message sizes.
            //SITPMessagesDecodeStability.GenerateDecodeStabilityStatistics(OutputPath.GetSubFoldersPath("sitp-decode-stability.xml", "research"), OutputPath.GetSubFoldersPath("sitp-decode-stability-statistics.txt", "research"), 16777216);
            //Console.WriteLine("Decode stability statistics generation completed.");

            Console.WriteLine("SITP stability research completed.");
            Console.ReadLine();
        }
    }
}
