using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_BIN_TOOL.EXTRACT
{
    public static class OutputMaterial
    {
      
        public static void CreateIdxMaterial(IdxMaterial idxmaterial, string baseDirectory, string baseFileName)
        {
            TextWriter text = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".idxmaterial")).CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine();
            text.WriteLine();

            foreach (var mat in idxmaterial.MaterialDic)
            {
                text.WriteLine("UseMaterial:" + mat.Key);
                text.WriteLine("material_flag:" + mat.Value.material_flag.ToString("X2"));
                text.WriteLine("diffuse_map:" + mat.Value.diffuse_map);
                text.WriteLine("bump_map:" + mat.Value.bump_map);
                text.WriteLine("opacity_map:" + mat.Value.opacity_map);
                text.WriteLine("generic_specular_map:" + mat.Value.generic_specular_map);
                text.WriteLine("intensity_specular_r:" + mat.Value.intensity_specular_r);
                text.WriteLine("intensity_specular_g:" + mat.Value.intensity_specular_g);
                text.WriteLine("intensity_specular_b:" + mat.Value.intensity_specular_b);
                text.WriteLine("unk_08:" + mat.Value.unk_08);
                text.WriteLine("unk_09:" + mat.Value.unk_09);
                text.WriteLine("specular_scale:" + mat.Value.specular_scale.ToString("X2"));
                text.WriteLine("custom_specular_map:" + mat.Value.custom_specular_map);

                text.WriteLine();
                text.WriteLine();
            }


            text.Close();
        }

        public static void CreateMTL(IdxMtl idxmtl, string baseDirectory, string baseFileName)
        {
            var inv = CultureInfo.InvariantCulture;

            TextWriter text = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".mtl")).CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine();
            text.WriteLine();

            foreach (var item in idxmtl.MtlDic)
            {
                text.WriteLine("newmtl " + item.Key);
                text.WriteLine("Ka 1.000 1.000 1.000");
                text.WriteLine("Kd 1.000 1.000 1.000");
                text.WriteLine("Ks " + item.Value.Ks);
                text.WriteLine("Ns 0");
                text.WriteLine("d 1");
                text.WriteLine("map_Kd " + item.Value.map_Kd);
                text.WriteLine("map_d " + item.Value.map_Kd);

                if (item.Value.map_Bump != null)
                {
                    text.WriteLine("map_Bump " + item.Value.map_Bump);
                    text.WriteLine("Bump " + item.Value.map_Bump);
                }

                if (item.Value.ref_specular_map != null)
                {
                    byte x = (byte)((item.Value.specular_scale & 0xF0) >> 4);
                    byte y = (byte)(item.Value.specular_scale & 0x0F);
                    float fx = x + 1f;
                    float fy = y + 1f;

                    text.WriteLine("map_Ns -s " + fx.ToString("f6", inv)
                        + " " + fy.ToString("f6", inv) + " 1 " + item.Value.ref_specular_map);
                }

                text.WriteLine();
                text.WriteLine();
            }

            text.Close();
        }

    }
}
