using System;
using System.Globalization;
using System.Collections.Generic;

namespace TCell.Text
{
    public enum NaryType
    {
        Binary, Decimal, Hexdecimal
    }

    sealed public class NumericTextParser
    {
        static public byte? ParseBinary(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            if (str.Length != 8)
                return null;

            try
            {
                byte by = 0;
                by += (byte)(ParseBitValue(str.Substring(7, 1)) * 1);
                by += (byte)(ParseBitValue(str.Substring(6, 1)) * 2);
                by += (byte)(ParseBitValue(str.Substring(5, 1)) * 4);
                by += (byte)(ParseBitValue(str.Substring(4, 1)) * 8);
                by += (byte)(ParseBitValue(str.Substring(3, 1)) * 16);
                by += (byte)(ParseBitValue(str.Substring(2, 1)) * 32);
                by += (byte)(ParseBitValue(str.Substring(1, 1)) * 64);
                by += (byte)(ParseBitValue(str.Substring(0, 1)) * 128);

                return by;
            }
            catch { return null; }
        }

        static public byte? ParseArbitaryBinary(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            try
            {
                byte by = 0;
                for (int i = str.Length - 1; i >= 0; i--)
                {
                    by += (byte)(ParseBitValue(str.Substring(i, 1)) * Math.Pow(2, (str.Length - 1 - i)));
                }

                return by;
            }
            catch { return null; }
        }

        static private byte ParseBitValue(string bit)
        {
            if (bit == "1")
                return 1;
            else if (bit == "0")
                return 0;
            else
                throw new ArgumentException($"Invalid bit string value {bit}.", nameof(bit));
        }

        static public byte? ParseDecimal(string str)
        {
            byte by;
            if (!byte.TryParse(str, out by))
                return null;

            return by;
        }

        static public byte? ParseHexdecimal(string str)
        {
            byte by;
            string hexString = str;
            if (str.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                hexString = str.Substring(2);

            if (!byte.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out by))
                return null;

            return by;
        }

        static public byte[] ParseMAC(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            bool isHexString = str.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase);
            string[] strArr = str.ToLower().Replace("0x", string.Empty).Split(new char[] { '-', ' ' });
            if (strArr == null || strArr.Length != 6)
                return null;

            byte[] mac = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                byte by;
                if (isHexString)
                {
                    byte? byObj = ParseHexdecimal(strArr[i]);
                    if (byObj == null)
                        return null;

                    by = byObj.Value;
                }
                else
                {
                    byte? byObj = ParseDecimal(strArr[i]);
                    if (byObj == null)
                        return null;

                    by = byObj.Value;
                }

                mac[i] = by;
            }

            return mac;
        }

        static public byte[] ParseNumericStringToArray(NaryType nary, string literal, params char[] seperator)
        {
            if (string.IsNullOrEmpty(literal))
                return null;

            string[] strArr = literal.Split(seperator);
            if (strArr == null || strArr.Length == 0)
                return null;

            List<byte> byList = null;
            foreach (string str in strArr)
            {
                byte? by = null;
                switch (nary)
                {
                    case NaryType.Binary:
                        by = NumericTextParser.ParseBinary(str);
                        break;
                    case NaryType.Decimal:
                        by = NumericTextParser.ParseDecimal(str);
                        break;
                    case NaryType.Hexdecimal:
                        by = NumericTextParser.ParseHexdecimal(str);
                        break;
                    default:
                        break;
                }
                if (by == null)
                    break;

                if (byList == null)
                    byList = new List<byte>();
                byList.Add(by.Value);
            }

            if (byList == null || byList.Count == 0)
                return null;
            else
                return byList.ToArray();
        }

    }
}
