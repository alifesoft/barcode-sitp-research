//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal static class Crc8Sitp
    {
        // x^8 + x^6 + x^3 + x^2 + 1
        // MSB-first: 0x4D
        // Koopman notation: 0xA6
        // Parameters:
        // RefIn  = false
        // RefOut = false
        // Init   = 0x00
        // XorOut = 0x00
        private const byte Polynomial = 0x4D;

        private static readonly byte[] Table = GenerateTable();

        private static byte[] GenerateTable()
        {
            byte[] table = new byte[256];

            for (int i = 0; i < table.Length; i++)
            {
                byte crc = (byte)i;

                for (int bit = 0; bit < 8; bit++)
                {
                    crc = (crc & 0x80) != 0
                        ? (byte)((crc << 1) ^ Polynomial)
                        : (byte)(crc << 1);
                }

                table[i] = crc;
            }

            return table;
        }

        public static byte Compute(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            byte crc = 0x00;
            for (int i = 0; i < data.Length; ++i)
                crc = Table[crc ^ data[i]];

            return crc;
        }
    }
}
