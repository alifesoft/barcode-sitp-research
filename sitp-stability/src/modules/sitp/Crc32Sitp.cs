//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal static class Crc32Sitp
    {
        // CRC-32-IEEE polynomial:
        // x^32 + x^26 + x^23 + x^22 + x^16 + x^12 + x^11 + x^10 + x^8 + x^7 + x^5 + x^4 + x^2 + x + 1
        // MSB-first: 0x04C11DB7
        // Parameters:
        // RefIn  = false
        // RefOut = false
        // Init   = 0x00000000
        // XorOut = 0x00000000
        private const uint Polynomial = 0x04C11DB7;

        private static readonly uint[] Table = GenerateTable();

        private static uint[] GenerateTable()
        {
            uint[] table = new uint[256];

            for (int i = 0; i < table.Length; ++i)
            {
                uint crc = (uint)i << 24;

                for (int bit = 0; bit < 8; ++bit)
                {
                    if ((crc & 0x80000000) != 0)
                        crc = (crc << 1) ^ Polynomial;
                    else
                        crc <<= 1;
                }

                table[i] = crc;
            }

            return table;
        }

        public static uint Compute(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            uint crc = 0x00000000;
            for (int i = 0; i < data.Length; ++i)
            {
                int index = (int)(((crc >> 24) ^ data[i]) & 0xFF);
                crc = (crc << 8) ^ Table[index];
            }

            return crc;
        }
    }
}
