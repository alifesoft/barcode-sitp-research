//Copyright(c) 2026 Oleksandr Havryliuk

namespace Alifesoft.SITPResearch
{
    internal class SitpMessage
    {
        private readonly SitpMode _mode;
        private readonly byte[] _payload;
        private readonly byte[] _encodedData;

        internal SitpMode Mode
        {
            get { return _mode; }
        }

        internal byte[] Payload
        {
            get { return _payload; }
        }

        internal byte[] EncodedData
        {
            get { return _encodedData; }
        }

        internal SitpMessage(SitpMode mode, byte[] payload, byte[] encodedData)
        {
            if (mode == SitpMode.Auto) throw new ArgumentException("SITP message must use a concrete mode.", nameof(mode));
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (encodedData == null) throw new ArgumentNullException(nameof(encodedData));

            _mode = mode;
            _payload = payload;
            _encodedData = encodedData;
        }
    }

    internal static class SitpMessageCodec
    {
        internal static SitpMessage Encode(byte[] payload)
        {
            return Encode(SitpMode.Auto, payload);
        }

        internal static SitpMessage Encode(SitpMode mode, byte[] payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            SitpMode encodeMode = ResolveMode(mode, payload.Length);

            byte[] header = SitpModeUtils.EncodeModeHeader(encodeMode, payload.Length);

            if (header == null)
                throw new InvalidOperationException("Failed to encode the SITP mode header.");

            int crcSize = SitpModeUtils.GetCrcBytesSize(encodeMode);

            if (crcSize <= 0)
                throw new InvalidOperationException("Unsupported SITP mode.");

            byte[] encodedData = new byte[header.Length + payload.Length + crcSize];

            Array.Copy(header, 0, encodedData, 0, header.Length);
            Array.Copy(payload, 0, encodedData, header.Length, payload.Length);

            byte[] crc = SitpModeUtils.CalculateCRC(encodedData, 0, encodeMode, payload.Length);

            if (crc == null || crc.Length != crcSize)
                throw new InvalidOperationException("Failed to calculate the SITP checksum.");

            Array.Copy(crc, 0, encodedData, header.Length + payload.Length, crc.Length);

            return new SitpMessage(encodeMode, payload, encodedData);
        }

        internal static SitpMessage Decode(byte[] data)
        {
            return Decode(data, 0);
        }

        internal static SitpMessage Decode(byte[] data, int offset)
        {
            // Decoding arbitrary input follows the C-style convention:
            // null is returned when no valid SITP message is found.

            if (data == null) return null;

            if (offset < 0 || offset >= data.Length) return null;

            ModeWithSize modeHeader = SitpModeUtils.DecodeModeHeader(data, offset);

            if (modeHeader == null) return null;

            SitpMode mode = modeHeader.Mode;
            int payloadSize = modeHeader.PayloadSize;

            int headerSize = SitpModeUtils.GetModeBytesSize(mode);
            int crcSize = SitpModeUtils.GetCrcBytesSize(mode);
            if (headerSize <= 0 || crcSize <= 0) return null;

            int encodedSize = headerSize + payloadSize + crcSize;
            if (encodedSize < headerSize) return null;
            if (offset > data.Length - encodedSize) return null;

            if (!SitpModeUtils.ValidateCRC(data, offset, mode, payloadSize))
                return null;

            byte[] payload = new byte[payloadSize];
            Array.Copy(data, offset + headerSize, payload, 0, payload.Length);

            byte[] encodedData = new byte[encodedSize];
            Array.Copy(data, offset, encodedData, 0, encodedData.Length);

            return new SitpMessage(mode, payload, encodedData);
        }

        private static SitpMode ResolveMode(SitpMode mode, int payloadSize)
        {
            if (payloadSize < 0)
                throw new ArgumentOutOfRangeException(nameof(payloadSize));

            if (mode == SitpMode.Auto)
            {
                if (payloadSize <= SitpModeUtils.GetModePayloadSize(SitpMode.Mode11))
                    return SitpMode.Mode11;

                if (payloadSize <= SitpModeUtils.GetModePayloadSize(SitpMode.Mode10))
                    return SitpMode.Mode10;

                if (payloadSize <= SitpModeUtils.GetModePayloadSize(SitpMode.Mode01))
                    return SitpMode.Mode01;

                throw new ArgumentException("Payload is too large for SITP-v1.", nameof(payloadSize));
            }

            int maximumSize = SitpModeUtils.GetModePayloadSize(mode);
            if (maximumSize < 0)
                throw new ArgumentException("Unsupported SITP mode.", nameof(mode));

            if (payloadSize > maximumSize)
                throw new ArgumentException("Payload size " + payloadSize.ToString() + " cannot be encoded using mode " + mode.ToString() + ".", nameof(payloadSize));

            return mode;
        }
    }
}
