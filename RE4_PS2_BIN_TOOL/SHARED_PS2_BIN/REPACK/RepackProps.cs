using SHARED_PS2_BIN.ALL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHARED_PS2_BIN.REPACK
{
    public struct RepackProps
    {
        public (ushort b1, ushort b2, ushort b3, ushort b4)[] BonePairs;

        public bool EnableBonepairTag;
        public bool EnableAdjacentBoneTag;
        public bool EnableUnkFlag1;
        public bool EnableUnkFlag2;
        public bool EnableUnkFlag4;
        public bool IsScenarioBin;

        public BoundingBox BoundingBox;
    }
}
