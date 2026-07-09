//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal enum SitpMode { Auto, Mode11, Mode10, Mode01 }

    internal class ModeWithSize
    {
        private readonly SitpMode _mode;
        private readonly int _payloadSize;
        internal SitpMode Mode
        { 
            get { return _mode; } 
        }
        internal int PayloadSize
        { 
            get { return _payloadSize; } 
        }

        internal ModeWithSize(SitpMode mode, int payloadSize)
        {
            _mode = mode;
            _payloadSize = payloadSize;
        }
    }

    internal static class SitpModeUtils
    {
        internal static int GetModePayloadSize(SitpMode mode)
        {
            if (mode == SitpMode.Mode11) return 63;
            if (mode == SitpMode.Mode10) return 16383;
            if (mode == SitpMode.Mode01) return 4194303;

            return -1;
        }

        internal static int GetModeBytesSize(SitpMode mode)
        {
            if (mode == SitpMode.Mode11) return 1;
            if (mode == SitpMode.Mode10) return 2;
            if (mode == SitpMode.Mode01) return 3;

            return -1;
        }

        internal static int GetCrcBytesSize(SitpMode mode)
        {
            if (mode == SitpMode.Mode11) return 1;
            if (mode == SitpMode.Mode10) return 2;
            if (mode == SitpMode.Mode01) return 4;

            return -1;
        }

        internal static byte[] EncodeModeHeader(SitpMode mode, int payloadSize)
        {
            if (payloadSize < 0) return null;

            int maximumSize = GetModePayloadSize(mode);
            int bytesCount = GetModeBytesSize(mode);
            if (maximumSize <= 0 || bytesCount <= 0) return null;

            if (payloadSize > maximumSize) return null;

            byte[] result = new byte[bytesCount];

            byte modeBits;

            if (mode == SitpMode.Mode11) modeBits = 0xC0;
            else if (mode == SitpMode.Mode10) modeBits = 0x80;
            else if (mode == SitpMode.Mode01) modeBits = 0x40;
            // Mode 00 is reserved.
            else return null;

            /*
             * Header bit layout:
             *
             * Mode11:
             *   byte 0: MM LLLLLL
             *
             * Mode10:
             *   byte 0: MM LLLLLL
             *   byte 1: LLLLLLLL
             *
             * Mode01:
             *   byte 0: MM LLLLLL
             *   byte 1: LLLLLLLL
             *   byte 2: LLLLLLLL
             *
             * The least significant six length bits are stored
             * in the first byte. Remaining length bits are stored
             * in little-endian order.
             */

            result[0] = (byte)(modeBits | (payloadSize & 0x3F));

            if (bytesCount > 1)
                result[1] = (byte)(payloadSize >> 6);

            if (bytesCount > 2)
                result[2] = (byte)(payloadSize >> 14);

            return result;
        }

        internal static ModeWithSize DecodeModeHeader(byte[] data, int offset)
        {
            // Return null if the mode header cannot be decoded.
            // Mode header bytes are read from the specified offset.

            if (data == null) return null;

            if (offset < 0 || offset >= data.Length) return null;

            byte firstByte = data[offset];
            int modeBits = firstByte >> 6;

            SitpMode mode;
            int bytesCount;

            if (modeBits == 0b11)
            {
                mode = SitpMode.Mode11;
                bytesCount = 1;
            }
            else if (modeBits == 0b10)
            {
                mode = SitpMode.Mode10;
                bytesCount = 2;
            }
            else if (modeBits == 0b01)
            {
                mode = SitpMode.Mode01;
                bytesCount = 3;
            }
            else
                // Mode 00 is reserved.
                return null;

            if (offset + bytesCount > data.Length) return null;

            int payloadSize = firstByte & 0x3F;

            if (bytesCount > 1)
                payloadSize |= data[offset + 1] << 6;

            if (bytesCount > 2)
                payloadSize |= data[offset + 2] << 14;

            if (payloadSize > GetModePayloadSize(mode)) return null;

            return new ModeWithSize(mode, payloadSize);
        }

        internal static byte[] CalculateCRC(byte[] data, int offset, SitpMode mode, int payloadSize)
        {
            // Return null on error.
            //
            // CRC is calculated over:
            // [Mode + PayloadSize][Payload]
            //
            // Before CRC calculation, the protected message bytes are
            // XOR-masked using the SITP-v1 LCG sequence.
            // The original data array is not modified.

            if (data == null) return null;

            if (offset < 0 || offset >= data.Length) return null;

            if (payloadSize < 0) return null;

            int maximumPayloadSize = GetModePayloadSize(mode);
            int headerSize = GetModeBytesSize(mode);
            
            if (maximumPayloadSize <= 0 || headerSize <= 0) return null;

            if (payloadSize > maximumPayloadSize) return null;
            int protectedSize = headerSize + payloadSize;

            if (protectedSize < headerSize) return null;

            if (offset > data.Length - protectedSize) return null;

            byte[] mask = LcgGen.Generate(protectedSize);
            byte[] maskedData = new byte[protectedSize];

            for (int i = 0; i < protectedSize; ++i)
                maskedData[i] = (byte)(data[offset + i] ^ mask[i]);

            if (mode == SitpMode.Mode11)
            {
                byte crc = Crc8Sitp.Compute(maskedData);
                return new byte[] { crc };
            }

            if (mode == SitpMode.Mode10)
            {
                ushort crc = Crc16Sitp.Compute(maskedData);
                return new byte[] { (byte)crc, (byte)(crc >> 8) };
            }

            if (mode == SitpMode.Mode01)
            {
                uint crc = Crc32Sitp.Compute(maskedData);
                return new byte[] { (byte)crc, (byte)(crc >> 8), (byte)(crc >> 16), (byte)(crc >> 24) };
            }

            return null;
        }

        internal static byte[] ExtractCRC(byte[] data, int offset, SitpMode mode, int payloadSize)
        {
            // Return null on error.
            //
            // Message layout:
            // [Mode + PayloadSize][Payload][CRC]

            if (data == null) return null;

            if (offset < 0 || offset >= data.Length) return null;

            if (payloadSize < 0) return null;

            int maximumPayloadSize = GetModePayloadSize(mode);
            int headerSize = GetModeBytesSize(mode);
            int crcSize = GetCrcBytesSize(mode);
            if (maximumPayloadSize <= 0 || headerSize <= 0 || crcSize <= 0) return null;

            if (payloadSize > maximumPayloadSize) return null;

            int crcOffset = offset + headerSize + payloadSize;

            if (crcOffset < offset) return null;
            if (crcOffset > data.Length - crcSize) return null;

            byte[] result = new byte[crcSize];

            for (int i = 0; i < crcSize; ++i)
                result[i] = data[crcOffset + i];

            return result;
        }

        internal static bool ValidateCRC(byte[] data, int offset, SitpMode mode, int payloadSize)
        {
            byte[] calculated = CalculateCRC(data, offset, mode, payloadSize);
            byte[] extracted = ExtractCRC(data, offset, mode, payloadSize);

            if (calculated == null || extracted == null) return false;

            if (calculated.Length != extracted.Length) return false;

            for (int i = 0; i < calculated.Length; ++i)
                if (calculated[i] != extracted[i])
                    return false;

            return true;
        }
    }
}
