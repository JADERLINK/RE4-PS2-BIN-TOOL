using System;

namespace SHARED_TOOLS
{
    public static class Shared
    {
        private const string VERSION = "V.1.5.0 (2025-10-18)";

        public static string HeaderText()
        {
            return "# github.com/JADERLINK/RE4-PS2-BIN-TOOL" + Environment.NewLine +
                   "# youtube.com/@JADERLINK" + Environment.NewLine +
                   "# RE4_PS2_BIN_TOOL by: JADERLINK" + Environment.NewLine +
                   "# Thanks to \"HardRain\"" + Environment.NewLine +
                   "# Material information by \"Albert\"" + Environment.NewLine +
                  $"# Version {VERSION}";
        }

        public static string HeaderTextSmd()
        {
            return "// RE4_PS2_BIN_TOOL" + Environment.NewLine +
                   "// by: JADERLINK" + Environment.NewLine +
                   "// youtube.com/@JADERLINK" + Environment.NewLine +
                   "// Thanks to \"HardRain\"" + Environment.NewLine +
                  $"// Version {VERSION}";
        }
    }
}
