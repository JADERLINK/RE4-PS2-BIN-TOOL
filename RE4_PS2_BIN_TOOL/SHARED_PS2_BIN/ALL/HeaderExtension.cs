using SHARED_PS2_BIN.EXTRACT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHARED_PS2_BIN.ALL
{
    public static class HeaderExtension
    {
        public static bool ReturnsHasEnableBonepairTag(this PS2BIN header)
        {
            return ((BinFlags)header.Bin_flags).HasFlag(BinFlags.EnableBonepairTag);
        }

        public static bool ReturnsHasEnableAdjacentBoneTag(this PS2BIN header)
        {
            return ((BinFlags)header.Bin_flags).HasFlag(BinFlags.EnableAdjacentBoneTag);
        }

        public static bool ReturnsHasEnableUnkFlag1(this PS2BIN header)
        {
            return ((BinFlags)header.Bin_flags).HasFlag(BinFlags.EnableUnkFlag1);
        }

        public static bool ReturnsHasEnableUnkFlag2(this PS2BIN header)
        {
            return ((BinFlags)header.Bin_flags).HasFlag(BinFlags.EnableUnkFlag2);
        }

        public static bool ReturnsHasEnableUnkFlag4(this PS2BIN header)
        {
            return ((BinFlags)header.Bin_flags).HasFlag(BinFlags.EnableUnkFlag4);
        }

    }
}
