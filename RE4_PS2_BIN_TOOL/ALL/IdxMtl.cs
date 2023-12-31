using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace RE4_PS2_BIN_TOOL.ALL
{  /// <summary>
   /// representa o arquivo .mtl
   /// </summary>
    public class IdxMtl
    {
        /// <summary>
        /// material name, MtlObj
        /// </summary>
        public Dictionary<string, MtlObj> MtlDic;
    }

    /// <summary>
    /// representa um material do .mtl
    /// </summary>
    public class MtlObj
    {  
        /// <summary>
        /// diffuse_texture
        /// </summary>
        public TexPathRef map_Kd = null;

        /// <summary>
        /// bump_texture (bump)
        /// </summary>
        public TexPathRef map_Bump = null;

        /// <summary>
        /// (map_Ks) or (map_Ns) // generic_specular_map
        /// </summary>
        public TexPathRef ref_specular_map = null;

        /// <summary>
        /// specular_scale
        /// </summary>
        public byte specular_scale = 0;

        /// <summary>
        /// intensity_specular_r, intensity_specular_g, intensity_specular_b
        /// </summary>
        public KsClass Ks = null;

    }


    /// <summary>
    /// é usado para definir o caminho das texturas no mtl
    /// </summary>
    public class TexPathRef
    {
        public string FolderName { get; private set; }
        public byte TextureID { get; private set; }
        public string Format { get; private set; }

        public TexPathRef(string FolderName, byte TextureID, string ImageFormat)
        {
            this.FolderName = FolderName;
            this.TextureID = TextureID;
            this.Format = ImageFormat;
        }

        public TexPathRef(string texturePath)
        {
            texturePath = texturePath.Replace("\\", "/");
            var split = texturePath.Split('/').Where(s => s.Length != 0).ToArray();

            try
            {
                var last = split.Last().Split('.').Where(s => s.Length != 0).ToArray();
                TextureID = byte.Parse(Utils.ReturnValidDecValue(last[0]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                Format = last.Last();
            }
            catch (Exception)
            {
            }

            if (split.Length - 1 > 0)
            {
                try
                {
                    var resplit = split[split.Length - 2].Split(' ').Where(s => s.Length != 0).ToArray();
                    FolderName = resplit.Last();
                }
                catch (Exception)
                {
                }
            }
        }

        public override string ToString()
        {
            return GetPath();
        }

        public string GetPath()
        {
            return FolderName + "/" + TextureID.ToString("D4") + "." + Format;
        }

        public override bool Equals(object obj)
        {
            return obj is TexPathRef tpr && tpr.FolderName == FolderName && tpr.TextureID == TextureID && tpr.Format == Format;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + FolderName.GetHashCode();
                hash = hash * 23 + TextureID.GetHashCode();
                hash = hash * 23 + Format.GetHashCode();
                return hash;
            }
        }
    }

    public class KsClass
    {
        private byte r, g, b;

        public KsClass(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public KsClass(float r, float g, float b)
        {
            this.r = (byte)(r * 255f);
            this.g = (byte)(g * 255f);
            this.b = (byte)(b * 255f);
        }

        public override string ToString()
        {
            return GetKs();
        }

        public string GetKs()
        {
            return (r / 255f).ToString("f6", CultureInfo.InvariantCulture)
           + " " + (g / 255f).ToString("f6", CultureInfo.InvariantCulture)
           + " " + (b / 255f).ToString("f6", CultureInfo.InvariantCulture);
        }

        public byte GetR()
        {
            return r;
        }

        public byte GetG()
        {
            return g;
        }

        public byte GetB()
        {
            return b;
        }

        public override bool Equals(object obj)
        {
            return obj is KsClass ks && ks.r == r && ks.g == g && ks.b == b;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + r.GetHashCode();
                hash = hash * 23 + g.GetHashCode();
                hash = hash * 23 + b.GetHashCode();
                return hash;
            }
        }
    }


}
