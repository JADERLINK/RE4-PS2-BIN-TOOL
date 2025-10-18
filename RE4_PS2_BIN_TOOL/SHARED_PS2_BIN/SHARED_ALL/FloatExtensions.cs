using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHARED_TOOLS.ALL
{
    public static class FloatExtensions
    {
        public static string ToFloatString(this float value)
        {
            string raw = value.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
            if (raw.IndexOf('E') >= 0 || raw.IndexOf('e') >= 0)
            {
                string s = value.ToString("F9", System.Globalization.CultureInfo.InvariantCulture);
                int end = s.Length - 1;
                while (end > 0 && s[end] == '0' && s[end - 1] != '.')
                {
                    end--;
                }
                if (s[end] == '.')
                {
                    return s.Substring(0, end + 2);
                }
                return s.Substring(0, end + 1);
            }
            if (raw.IndexOf('.') < 0)
            {
                return raw + ".0";
            }
            return raw;
        }

    }
}
