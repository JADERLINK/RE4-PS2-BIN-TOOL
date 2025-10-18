using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using SHARED_TOOLS.ALL;

namespace SHARED_PS2_BIN.ALL
{
    /// <summary>
    /// representa o arquivo MTL
    /// </summary>
    public class IdxMtl
    {
        /// <summary>
        /// material name, MtlObj
        /// </summary>
        public Dictionary<string, MtlObj> MtlDic;
    }


    /// <summary>
    /// representa um material do MTL
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
    /// é usado para definir o caminho das texturas no MTL
    /// </summary>
    public class TexPathRef
    {
        public string FolderName { get; private set; }
        public byte TextureID { get; private set; }
        public string Format { get; private set; }

        public TexPathRef(string FolderName, byte TextureID, string ImageFormat)
        {
            this.FolderName = FolderName ?? "null";
            this.TextureID = TextureID;
            Format = ImageFormat?.ToLowerInvariant() ?? "null";
        }

        public TexPathRef(string texturePath)
        {
            Format = "null";
            FolderName = "null";
            if (texturePath == null)
            {
                texturePath = "";
            }

            texturePath = texturePath.Replace("\\\\", "/").Replace("\\", "/"); // Coloca o tipo de barra no padrão linux
            var split = texturePath.Split('/').Where(s => s.Length != 0).ToArray(); // Divide o path em pasta e nome do arquivo

            try
            {
                var last = split.Last().Split('.').Where(s => s.Length != 0).ToArray(); // Pega o conteúdo com o nome do arquivo e divide por ponto, para separar o formato.
                string value = last.Last().Split('-').Last(); // Último valor, para caso não tiver formato, divide pelo char de menos.
                if (last.Length - 1 > 0) // no caso de ter o formato
                {
                    value = last[last.Length - 2].Split('-').Last(); // O penúltimo é o valor correto, divide pelo char de menos.
                }
                TextureID = byte.Parse(Utils.ReturnValidDecValue(value), NumberStyles.Integer, CultureInfo.InvariantCulture);
                Format = last.Last().ToLowerInvariant();
            }
            catch (Exception ex)
            {
                // Se aconteceu um erro, é porque não conseguiu pegar um ID válido, então o valor de TextureID vai ficar 0
                Console.WriteLine("Error getting TextureID from texture path: " + texturePath);
                Console.WriteLine(ex.Message);
            }

            if (split.Length - 1 > 0)
            {
                try
                {
                    // Aqui é splitado o ' ' char de espaço, pois pode ter as informações do specular na string
                    var resplit = split[split.Length - 2].Split(' ').Where(s => s.Length != 0).ToArray();
                    FolderName = resplit.Last();
                    // Se a lista for vazia, vai dar InvalidOperationException
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
            return (r / 255f).ToFloatString()
           + " " + (g / 255f).ToFloatString()
           + " " + (b / 255f).ToFloatString();
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
