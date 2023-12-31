using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace RE4_PS2_BIN_TOOL.ALL
{
    public static class IdxMtlParser
    {
        public static IdxMtl Parser(IdxMaterial idxMaterial, string FileBaseName)
        {
            IdxMtl idx = new IdxMtl();
            idx.MtlDic = new Dictionary<string, MtlObj>();

            foreach (var mat in idxMaterial.MaterialDic)
            {
                MtlObj mtl = new MtlObj();

                mtl.map_Kd = GetTexPathRef(FileBaseName, mat.Value.diffuse_map);

                mtl.Ks = new KsClass(mat.Value.intensity_specular_r, mat.Value.intensity_specular_g, mat.Value.intensity_specular_b);

                mtl.specular_scale = mat.Value.specular_scale;

                if (mat.Value.bump_map != 255)
                {
                    mtl.map_Bump = GetTexPathRef(FileBaseName, mat.Value.bump_map);
                }

                if (mat.Value.generic_specular_map != 255)
                {
                    mtl.ref_specular_map = GetTexPathRef(CONSTs.SpecularFolder, mat.Value.generic_specular_map);
                }

                idx.MtlDic.Add(mat.Key, mtl);
            }

            return idx;
        }


        private static TexPathRef GetTexPathRef(string FolderName, byte TextureID)
        {
            return new TexPathRef(FolderName, TextureID, "TGA");
        }

    }
}
