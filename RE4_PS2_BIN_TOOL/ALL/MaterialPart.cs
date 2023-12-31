using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_PS2_BIN_TOOL.ALL
{
    /// <summary>
    /// representa um material do .bin
    /// </summary>
    public class MaterialPart
    {
        public byte material_flag;
        public byte diffuse_map;
        public byte bump_map;
        public byte opacity_map; //alpha
        public byte generic_specular_map;
        public byte intensity_specular_r;
        public byte intensity_specular_g;
        public byte intensity_specular_b;
        public byte unk_08;
        public byte unk_09;
        public byte specular_scale;
        public byte custom_specular_map;

        public MaterialPart()
        {
        }

        public MaterialPart(byte[] arr)
        {
            material_flag = arr[00];
            diffuse_map = arr[01];
            bump_map = arr[02];
            opacity_map = arr[03];
            generic_specular_map = arr[04];
            intensity_specular_r = arr[05];
            intensity_specular_g = arr[06];
            intensity_specular_b = arr[07];
            unk_08 = arr[08];
            unk_09 = arr[09];
            specular_scale = arr[10];
            custom_specular_map = arr[11];
        }

        public byte[] GetArray()
        {
            byte[] b = new byte[16];
            b[00] = material_flag;
            b[01] = diffuse_map;
            b[02] = bump_map;
            b[03] = opacity_map;
            b[04] = generic_specular_map;
            b[05] = intensity_specular_r;
            b[06] = intensity_specular_g;
            b[07] = intensity_specular_b;
            b[08] = unk_08;
            b[09] = unk_09;
            b[10] = specular_scale;
            b[11] = custom_specular_map;
            return b;
        }

        public override bool Equals(object obj)
        {
            return obj is MaterialPart m
                && m.material_flag == material_flag
                && m.diffuse_map == diffuse_map
                && m.bump_map == bump_map
                && m.opacity_map == opacity_map
                && m.generic_specular_map == generic_specular_map
                && m.intensity_specular_r == intensity_specular_r
                && m.intensity_specular_g == intensity_specular_g
                && m.intensity_specular_b == intensity_specular_b
                && m.unk_08 == unk_08
                && m.unk_09 == unk_09
                && m.specular_scale == specular_scale
                && m.custom_specular_map == custom_specular_map
                ;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + material_flag.GetHashCode();
                hash = hash * 23 + diffuse_map.GetHashCode();
                hash = hash * 23 + bump_map.GetHashCode();
                hash = hash * 23 + opacity_map.GetHashCode();
                hash = hash * 23 + generic_specular_map.GetHashCode();
                hash = hash * 23 + intensity_specular_r.GetHashCode();
                hash = hash * 23 + intensity_specular_g.GetHashCode();
                hash = hash * 23 + intensity_specular_b.GetHashCode();
                hash = hash * 23 + unk_08.GetHashCode();
                hash = hash * 23 + unk_09.GetHashCode();
                hash = hash * 23 + specular_scale.GetHashCode();
                hash = hash * 23 + custom_specular_map.GetHashCode();
                return hash;
            }
        }

    }

}
