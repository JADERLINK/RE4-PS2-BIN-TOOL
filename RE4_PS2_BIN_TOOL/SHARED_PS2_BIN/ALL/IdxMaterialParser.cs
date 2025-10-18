using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHARED_PS2_BIN.EXTRACT;
using SHARED_TOOLS.ALL;

namespace SHARED_PS2_BIN.ALL
{
    public static class IdxMaterialParser
    {
        public static IdxMaterial Parser(PS2BIN bin)
        {
            IdxMaterial idx = new IdxMaterial();
            idx.MaterialDic = new Dictionary<string, MaterialPart>();

            for (int i = 0; i < bin.materials.Length; i++)
            {
                idx.MaterialDic.Add(CONSTs.MATERIAL + i.ToString("D3"), new MaterialPart(bin.materials[i].materialLine));
            }

            return idx;
        }

    }
}
