using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.ALL;
using RE4_PS2_BIN_TOOL.EXTRACT;


namespace RE4_PS2_BIN_TOOL.REPACK
{
    public static class MtlConverter
    {
        public static void Convert(IdxMtl idxmtl, out IdxMaterial idxMaterial)
        {
            idxMaterial = new IdxMaterial();
            idxMaterial.MaterialDic = new Dictionary<string, MaterialPart>();

            foreach (var item in idxmtl.MtlDic)
            {
                MaterialPart mat = new MaterialPart();
                mat.material_flag = 0;
                mat.custom_specular_map = 255;
                mat.generic_specular_map = 255;
                mat.opacity_map = 255;
                mat.bump_map = 255;

                mat.diffuse_map = item.Value.map_Kd.TextureID;

                if (item.Value.map_Bump != null)
                {
                    mat.material_flag |= 0x01; // bump flag

                    mat.bump_map = item.Value.map_Bump.TextureID;
                    mat.generic_specular_map = 0;
                    mat.intensity_specular_b = 255;
                    mat.intensity_specular_g = 255;
                    mat.intensity_specular_r = 255;
                    mat.specular_scale = 0x00;
                }

                if (item.Value.ref_specular_map != null)
                {
                    mat.material_flag |= 0x02; //generic specular flag

                    mat.intensity_specular_b = item.Value.Ks.GetR();
                    mat.intensity_specular_g = item.Value.Ks.GetG();
                    mat.intensity_specular_r = item.Value.Ks.GetB();
                    mat.specular_scale = item.Value.specular_scale;
                    mat.generic_specular_map = item.Value.ref_specular_map.TextureID;
                }

                idxMaterial.MaterialDic.Add(item.Key, mat);
            }
  
        }

    }
}
