using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace RE4_PS2_BIN_TOOL.ALL
{
    public static class IdxMaterialLoad
    {
        public static IdxMaterial Load(Stream stream)
        {
            StreamReader reader = new StreamReader(stream, Encoding.ASCII);

            IdxMaterial idx = new IdxMaterial();
            idx.MaterialDic = new Dictionary<string, MaterialPart>();

            MaterialPart temp = new MaterialPart();

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim().ToUpperInvariant();

                if (line == null || line.Length == 0 || line.StartsWith("\\") || line.StartsWith("/") || line.StartsWith("#") || line.StartsWith(":"))
                {
                    continue;
                }
                else if (line.StartsWith("USEMATERIAL"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        string name = split[1].Trim();

                        if (!idx.MaterialDic.ContainsKey(name))
                        {
                            temp = new MaterialPart();
                            idx.MaterialDic.Add(name, temp);
                        }
                    }
                }

                else if (line.StartsWith("MATERIAL_FLAG"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.material_flag = byte.Parse(Utils.ReturnValidHexValue(split[1]), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (line.StartsWith("DIFFUSE_MAP"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.diffuse_map = byte.Parse(Utils.ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (line.StartsWith("BUMP_MAP"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.bump_map = byte.Parse(Utils.ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (line.StartsWith("OPACITY_MAP"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.opacity_map = byte.Parse(Utils.ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (line.StartsWith("GENERIC_SPECULAR_MAP"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.generic_specular_map = byte.Parse(Utils.ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (line.StartsWith("INTENSITY_SPECULAR_R"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.intensity_specular_r = byte.Parse(Utils.ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (line.StartsWith("INTENSITY_SPECULAR_G"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.intensity_specular_g = byte.Parse(Utils.ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (line.StartsWith("INTENSITY_SPECULAR_B"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.intensity_specular_b = byte.Parse(Utils.ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (line.StartsWith("UNK_08"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.unk_08 = byte.Parse(Utils.ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (line.StartsWith("UNK_09"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.unk_09 = byte.Parse(Utils.ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (line.StartsWith("SPECULAR_SCALE"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.specular_scale = byte.Parse(Utils.ReturnValidHexValue(split[1]), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                else if (line.StartsWith("CUSTOM_SPECULAR_MAP"))
                {
                    var split = line.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            temp.custom_specular_map = byte.Parse(Utils.ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

            }

            return idx;
        }


    }
}