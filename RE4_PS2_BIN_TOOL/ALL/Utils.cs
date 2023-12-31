using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_PS2_BIN_TOOL.ALL
{
    public static class Utils
    {
        public static string ReturnValidHexValue(string cont)
        {
            string res = "";
            foreach (var c in cont)
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
                    res += c;
                }
            }
            return res;
        }

        public static string ReturnValidDecValue(string cont)
        {
            string res = "";
            foreach (var c in cont)
            {
                if (char.IsDigit(c))
                {
                    res += c;
                }
            }
            return res;
        }

        public static string ReturnValidDecWithNegativeValue(string cont)
        {
            bool negative = false;

            string res = "";
            foreach (var c in cont)
            {
                if (negative == false && c == '-')
                {
                    res = c + res;
                    negative = true;
                }

                if (char.IsDigit(c))
                {
                    res += c;
                }
            }
            return res;
        }

        public static string ReturnValidFloatValue(string cont)
        {
            bool Dot = false;
            bool negative = false;

            string res = "";
            foreach (var c in cont)
            {
                if (negative == false && c == '-')
                {
                    res = c + res;
                    negative = true;
                }

                if (Dot == false && c == '.')
                {
                    res += c;
                    Dot = true;
                }
                if (char.IsDigit(c))
                {
                    res += c;
                }
            }
            return res;
        }

        public static short ParseFloatToShort(float value)
        {
            string sv = value.ToString("F", System.Globalization.CultureInfo.InvariantCulture).Split('.')[0];
            int iv = 0;
            try
            {
                iv = int.Parse(sv, System.Globalization.NumberStyles.Integer);
            }
            catch (Exception)
            {
            }
            if (iv > short.MaxValue)
            {
                iv = short.MaxValue;
            }
            else if (iv < short.MinValue)
            {
                iv = short.MinValue;
            }
            return (short)iv;
        }

        public static ushort ParseDoubleToUshort(double value)
        {
            string sv = value.ToString("F", System.Globalization.CultureInfo.InvariantCulture).Split('.')[0];
            int iv = 0;
            try
            {
                iv = int.Parse(sv, System.Globalization.NumberStyles.Integer);
            }
            catch (Exception)
            {
            }
            if (iv > ushort.MaxValue)
            {
                iv = ushort.MaxValue;
            }
            else if (iv < ushort.MinValue)
            {
                iv = ushort.MinValue;
            }
            return (ushort)iv;
        }

    }
}
