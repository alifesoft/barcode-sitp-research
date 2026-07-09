// Copyright (c) 2026 Oleksandr Havryliuk
using System.Text;
using System.Globalization;
using System.Security.Cryptography;

namespace Alifesoft.SITPResearch
{
    internal static class EntropyDataStore
    {
        private const string BinaryExtension = ".bin";
        private const string Sha256Extension = ".sha256";
        private const string DescriptionExtension = ".md";

        /// <summary>
        /// Generates an entropy byte array using the operating system
        /// cryptographically secure random number generator.
        /// Saves the entropy data as:
        /// name.bin, name.sha256, and name.md.
        /// </summary>
        public static void GenerateAndSave(string folder, string name, int size)
        {
            byte[] data = Generate(size);
            Save(folder, name, data);
        }

        /// <summary>
        /// Generates an entropy byte array using the operating system
        /// cryptographically secure random number generator.
        /// </summary>
        public static byte[] Generate(int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            return RandomNumberGenerator.GetBytes(size);
        }

        /// <summary>
        /// Saves the entropy data as:
        /// name.bin, name.sha256, and name.md.
        /// </summary>
        public static void Save(string folder, string name, byte[] data)
        {
            ValidateFolderAndName(folder, name);

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Length == 0)
                throw new ArgumentException("Entropy data is empty.", nameof(data));

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string binaryPath = GetPath(folder, name, BinaryExtension);
            string sha256Path = GetPath(folder, name, Sha256Extension);
            string descriptionPath = GetPath(folder, name, DescriptionExtension);

            string sha256 = CalculateSha256(data);
            string description = CreateDescription(name, data.Length, sha256);

            IOUtils.WriteFileTransactional(binaryPath, data);
            IOUtils.WriteTextTransactional(sha256Path, sha256 + Environment.NewLine);
            IOUtils.WriteTextTransactional(descriptionPath, description);
        }

        /// <summary>
        /// Loads the entropy data from name.bin.
        /// When validateSha256 is true, the SHA-256 value is read from
        /// name.sha256 and compared with the calculated hash.
        /// </summary>
        public static byte[] Load(string folder, string name, bool validateSha256 = true)
        {
            ValidateFolderAndName(folder, name);

            string binaryPath = GetPath(folder, name, BinaryExtension);
            if (!File.Exists(binaryPath))
                throw new FileNotFoundException("Entropy binary file was not found.", binaryPath);

            byte[] data = File.ReadAllBytes(binaryPath);
            if (!validateSha256) return data;

            string sha256Path = GetPath(folder, name, Sha256Extension);
            if (!File.Exists(sha256Path))
                throw new FileNotFoundException("SHA-256 file was not found.", sha256Path);

            string expectedSha256 = File.ReadAllText(sha256Path).Trim();
            if (string.IsNullOrEmpty(expectedSha256))
                throw new InvalidDataException("The SHA-256 file is empty.");

            string actualSha256 = CalculateSha256(data);
            if (!string.Equals(expectedSha256, actualSha256, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("The entropy data SHA-256 value does not match " + "the value stored in the SHA-256 file.");

            return data;
        }

        private static string CalculateSha256(byte[] data)
        {
            byte[] hash = SHA256.HashData(data);
            return HexConverter.ToHexString(hash);
        }

        private static string CreateDescription(string name, int size, string sha256)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("# Random Dataset");
            builder.AppendLine();
            builder.AppendLine(
                "This file describes a cryptographically secure random binary dataset generated for reproducible SITP experiments.");
            builder.AppendLine();
            builder.AppendLine("- Binary file: `" + name + BinaryExtension + "`");
            builder.AppendLine("- Size: " + size.ToString(CultureInfo.InvariantCulture) + " bytes");
            builder.AppendLine("- SHA-256: `" + sha256 + "`");
            builder.AppendLine("- Generator: `System.Security.Cryptography.RandomNumberGenerator.GetBytes`");
            builder.AppendLine("- Generated at: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture));
            builder.AppendLine();
            builder.AppendLine( "The dataset was generated using the operating system cryptographically secure random number generator.");

            return builder.ToString();
        }

        private static string GetPath(string folder, string name, string extension)
        {
            return Path.Combine(folder, name + extension);
        }

        private static void ValidateFolderAndName(string folder, string name)
        {
            if (string.IsNullOrWhiteSpace(folder))
                throw new ArgumentException("Folder is empty.", nameof(folder));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is empty.", nameof(name));

            if (!string.Equals(name, Path.GetFileName(name), StringComparison.Ordinal))
                throw new ArgumentException("Name must not contain a directory path.", nameof(name));

            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException("Name contains invalid filename characters.", nameof(name));
        }
    }
}
