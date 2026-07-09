//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    /// <summary>
    /// Generates a byte sequence using a 32-bit linear congruential generator.
    ///
    /// X(i + 1) = (a * X(i) + c) mod 2^32
    ///
    /// Each generated 32-bit state is written to the output in little-endian byte order. The first output value is X0.
    /// </summary>
    internal static class LcgGen
    {
        //SITP-v1 Lcg
        public static byte[] Generate(int length)
        {
            //0x53495450 is "SITP" in ASCII encoding
            uint x0 = 0x53495450;
            uint a = 1664525;
            uint c = 1013904223;
            return Generate(a, c, x0, length);
        }

        public static byte[] Generate(uint a, uint c, uint x0, int length)
        {
            //https://learn.microsoft.com/dotnet/csharp/language-reference/statements/checked-and-unchecked
            //https://en.wikipedia.org/wiki/Linear_congruential_generator

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            byte[] result = new byte[length];
            uint x_next = x0;
            int offset = 0;

            while (offset < result.Length)
            {
                for (int i = 0; i < 4 && offset < result.Length; ++i)
                    result[offset++] = (byte)(x_next >> (8 * i));

                x_next = unchecked(x_next * a + c);
            }

            return result;
        }

        internal static uint Next(uint state, uint a, uint c)
        {
            return unchecked(state * a + c);
        }
    }
}
