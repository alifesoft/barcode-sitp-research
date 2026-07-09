//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal static class Crc16Sitp
    {
        // x^16 + x^14 + x^13 + x^12 + x^10 +
        // x^8 + x^6 + x^4 + x^3 + x + 1
        // MSB-first: 0x755B
        // Koopman notation: 0xBAAD
        // Parameters:
        // RefIn  = false
        // RefOut = false
        // Init   = 0x0000
        // XorOut = 0x0000
        private const ushort Polynomial = 0x755B;

        private static readonly ushort[] Table = GenerateTable();

        private static ushort[] GenerateTable()
        {
            ushort[] table = new ushort[256];

            for (int i = 0; i < table.Length; i++)
            {
                ushort crc = (ushort)(i << 8);

                for (int bit = 0; bit < 8; bit++)
                {
                    crc = (crc & 0x8000) != 0
                        ? (ushort)((crc << 1) ^ Polynomial)
                        : (ushort)(crc << 1);
                }

                table[i] = crc;
            }

            return table;
        }

        public static ushort Compute(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            ushort crc = 0x0000;
            for (int i = 0; i < data.Length; ++i)
            {
                int index = ((crc >> 8) ^ data[i]) & 0xFF;
                crc = (ushort)((crc << 8) ^ Table[index]);
            }

            return crc;
        }
    }
}
