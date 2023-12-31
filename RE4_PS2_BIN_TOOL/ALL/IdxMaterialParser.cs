using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_PS2_BIN_TOOL.EXTRACT;

namespace RE4_PS2_BIN_TOOL.ALL
{
    public static class IdxMaterialParser
    {
        public static IdxMaterial Parser(BIN bin)
        {
            IdxMaterial idx = new IdxMaterial();
            idx.MaterialDic = new Dictionary<string, MaterialPart>();

            for (int i = 0; i < bin.materials.Length; i++)
            {
                idx.MaterialDic.Add(CONSTs.PS2_MATERIAL + i.ToString("D3"), new MaterialPart(bin.materials[i].materialLine));
            }

            return idx;
        }

    }
}
