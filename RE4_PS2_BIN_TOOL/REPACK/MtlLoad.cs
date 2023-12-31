using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_BIN_TOOL.REPACK
{
    public static class MtlLoad
    {

        public static void Load(Stream mtlFile, out IdxMtl idxmtl)
        {
            List<ObjLoader.Loader.Data.Material> MtlMaterials = new List<ObjLoader.Loader.Data.Material>();

            try
            {
                var mtlLoaderFactory = new ObjLoader.Loader.Loaders.MtlLoaderFactory();
                var mtlLoader = mtlLoaderFactory.Create();
                var streamReaderMtl = new StreamReader(mtlFile, Encoding.ASCII);
                ObjLoader.Loader.Loaders.LoadResultMtl arqMtl = mtlLoader.Load(streamReaderMtl);
                streamReaderMtl.Close();
                MtlMaterials = arqMtl.Materials.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading Mtl file: " + ex.Message);
            }

            idxmtl = new IdxMtl();
            idxmtl.MtlDic = new Dictionary<string, MtlObj>();

            foreach (var mat in MtlMaterials)
            {
                string name = mat.Name.Trim().ToUpperInvariant();
                MtlObj mtlObj = new MtlObj();
                mtlObj.map_Kd = new TexPathRef(mat.DiffuseTextureMap);
                mtlObj.Ks = new KsClass(mat.SpecularColor.X, mat.SpecularColor.Y, mat.SpecularColor.Z);

                if (mat.BumpMap != null)
                {
                    mtlObj.map_Bump = new TexPathRef(mat.BumpMap);
                }

                if (mat.SpecularHighlightTextureMap != null)
                {

                    mtlObj.ref_specular_map = new TexPathRef(mat.SpecularHighlightTextureMap);

                    if (mat.SpecularHighlightTextureMap.ToLowerInvariant().StartsWith("-s"))
                    {
                        float x = 0;
                        float y = 0;
                        var split = mat.SpecularHighlightTextureMap.ToLowerInvariant().Split(' ').Where(s => s.Length != 0).ToArray();
                        if (split.Length >= 3)
                        {
                            try
                            {
                                x = float.Parse(Utils.ReturnValidFloatValue(split[1]), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                            {
                            }

                            try
                            {
                                y = float.Parse(Utils.ReturnValidFloatValue(split[2]), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        unchecked
                        {
                            byte bx = (byte)(x < 1 ? 0 : (x > 16 ? 15 : x - 1));
                            byte by = (byte)(y < 1 ? 0 : (y > 16 ? 15 : y - 1));

                            mtlObj.specular_scale = (byte)((bx << 4) + (by & 0x0F));
                        }
                    }

                }

                if (!idxmtl.MtlDic.ContainsKey(name))
                {
                    idxmtl.MtlDic.Add(name, mtlObj);
                }


            }

        }

    }
}
