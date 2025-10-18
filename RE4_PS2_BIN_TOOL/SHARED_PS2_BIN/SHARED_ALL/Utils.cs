using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SHARED_TOOLS.ALL
{
    public static class Utils
    {
        public static string ReturnValidHexValue(string cont)
        {
            StringBuilder res = new StringBuilder();
            foreach (var c in cont.ToUpperInvariant())
            {
                if (char.IsDigit(c)
                    || c == 'A'
                    || c == 'B'
                    || c == 'C'
                    || c == 'D'
                    || c == 'E'
                    || c == 'F'
                    )
                {
                    res.Append(c);
                }
            }
            return res.ToString();
        }

        public static string ReturnValidDecValue(string cont)
        {
            StringBuilder res = new StringBuilder();
            foreach (var c in cont)
            {
                if (char.IsDigit(c))
                {
                    res.Append(c);
                }
            }
            return res.ToString();
        }

        public static string ReturnValidDecWithNegativeValue(string cont)
        {
            bool negative = false;

            StringBuilder res = new StringBuilder();
            foreach (var c in cont)
            {
                if (negative == false && c == '-')
                {
                    res.Insert(0, c);
                    negative = true;
                    continue;
                }

                if (char.IsDigit(c))
                {
                    res.Append(c);
                }
            }
            return res.ToString();
        }

        public static string ReturnValidFloatValue(string cont)
        {
            bool dot = false;
            bool negative = false;

            StringBuilder res = new StringBuilder();
            foreach (var c in cont)
            {
                if (negative == false && c == '-')
                {
                    res.Insert(0, c);
                    negative = true;
                    continue;
                }

                if (dot == false && c == '.')
                {
                    res.Append(c);
                    dot = true;
                    continue;
                }

                if (char.IsDigit(c))
                {
                    res.Append(c);
                }
            }
            return res.ToString();
        }

        public static short ParseFloatToShort(float value)
        {
            int iv = (int)Math.Round(value, MidpointRounding.AwayFromZero);
            if (iv > short.MaxValue) { return short.MaxValue; }
            if (iv < short.MinValue) { return short.MinValue; }
            return (short)iv;
        }

        public static bool SetByteHex(ref string line, string key, ref byte varToSet)
        {
            if (line.StartsWith(key))
            {
                var split = line.Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = byte.Parse(ReturnValidHexValue(split[1]), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SetByteDec(ref string line, string key, ref byte varToSet)
        {
            if (line.StartsWith(key))
            {
                var split = line.Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = byte.Parse(ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SetUintHex(ref string line, string key, ref uint varToSet)
        {
            if (line.StartsWith(key))
            {
                var split = line.Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = uint.Parse(ReturnValidHexValue(split[1]), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SetUintDec(ref string line, string key, ref uint varToSet)
        {
            if (line.StartsWith(key))
            {
                var split = line.Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = uint.Parse(ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SetUshortDec(ref string line, string key, ref ushort varToSet)
        {
            if (line.StartsWith(key))
            {
                var split = line.Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = ushort.Parse(ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SetUshortHex(ref string line, string key, ref ushort varToSet)
        {
            if (line.StartsWith(key))
            {
                var split = line.Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = ushort.Parse(ReturnValidHexValue(split[1]), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SetSbyteDec(ref string line, string key, ref sbyte varToSet)
        {
            if (line.StartsWith(key))
            {
                var split = line.Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = sbyte.Parse(ReturnValidDecWithNegativeValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SetShortDec(ref string line, string key, ref short varToSet)
        {
            if (line.StartsWith(key))
            {
                var split = line.Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = short.Parse(ReturnValidDecWithNegativeValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SetintDec(ref string line, string key, ref int varToSet)
        {
            if (line.StartsWith(key))
            {
                var split = line.Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = int.Parse(ReturnValidDecWithNegativeValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SetFloatDec(ref string line, string key, ref float varToSet)
        {
            if (line.StartsWith(key))
            {
                var split = line.Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = float.Parse(ReturnValidFloatValue(split[1]), NumberStyles.Float, CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }

        public static bool SetBoolean(ref string line, string key, ref bool varToSet) 
        {
            if (line.StartsWith(key))
            {
                var split = line.ToLowerInvariant().Split(':');
                if (split.Length >= 2)
                {
                    try
                    {
                        varToSet = bool.Parse(split[1].Trim());
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }
            return false;
        }
    }
}
