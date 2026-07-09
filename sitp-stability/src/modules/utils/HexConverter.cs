//Copyright(c) 2026 Oleksandr Havryliuk
using System.Globalization;

namespace Alifesoft.SITPResearch
{

    internal class HexConverter
    {
        protected static readonly HexConverter _instance = new HexConverter();

        protected Dictionary<int, char> _toHex = new Dictionary<int, char>();
        protected Dictionary<char, int> _fromHex = new Dictionary<char, int>();
        protected HexConverter()
        {
            char[] _hexDigits = "0123456789ABCDEF".ToCharArray();
            char[] _hexDigitsLo = "0123456789ABCDEF".ToLower(CultureInfo.InvariantCulture).ToCharArray();

            for (int i = 0; i < _hexDigits.Length; ++i)
            {
                _toHex[i] = _hexDigits[i];
                _fromHex[_hexDigits[i]] = i;
                if (!_fromHex.ContainsKey(_hexDigitsLo[i]))
                    _fromHex[_hexDigitsLo[i]] = i;
            }
        }

        protected byte[] _FromHexString(string hex)
        {
            if (hex == null) throw new ArgumentNullException("hex is null");
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have even length.", hex);

            int len = hex.Length / 2;
            byte[] res = new byte[len];
            for (int i = 0; i < len; i++)
                res[i] = (byte)(_fromHex[hex[2 * i]] << 4 | _fromHex[hex[2 * i + 1]]);
            return res;
        }

        protected string _ToHexString(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data is null");

            char[] chars = new char[data.Length * 2];
            for (int i = 0; i < data.Length; i++)
            {
                byte b = data[i];
                chars[i * 2] = _toHex[b >> 4];
                chars[i * 2 + 1] = _toHex[b & 0xF];
            }
            return new string(chars);
        }

        internal static byte[] FromHexString(string hex)
        {
            return _instance._FromHexString(hex);
        }

        internal static string ToHexString(byte[] data)
        {
            return _instance._ToHexString(data);
        }
    }
}
