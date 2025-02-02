using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_BIN_TOOL.EXTRACT
{
    public static class OutputFiles
    {

        //Studiomdl Data
        public static void CreateSMD(PS2BIN bin, string baseDirectory, string baseFileName)
        {

            TextWriter text = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".smd")).CreateText();

            text.WriteLine("version 1");
            text.WriteLine("nodes");

            for (int i = 0; i < bin.bones.Length; i++)
            {
                short BoneParent = bin.bones[i].BoneParent;
                if (BoneParent == 0xFF)
                {
                    BoneParent = -1;
                }
                text.WriteLine(bin.bones[i].BoneID + " \"BONE_" + bin.bones[i].BoneID.ToString("D3") + "\" " + BoneParent);
            }

            text.WriteLine("end");

            text.WriteLine("skeleton");
            text.WriteLine("time 0");

            for (int i = 0; i < bin.bones.Length; i++)
            {
                text.WriteLine(bin.bones[i].BoneID + "  " +
                    (bin.bones[i].PositionX / CONSTs.GLOBAL_SCALE).ToFloatString() + " " +
                    (bin.bones[i].PositionZ * -1 / CONSTs.GLOBAL_SCALE).ToFloatString() + " " +
                    (bin.bones[i].PositionY / CONSTs.GLOBAL_SCALE).ToFloatString() + "  0.0 0.0 0.0");
            }

            text.WriteLine("end");

            text.WriteLine("triangles");

            for (int t = 0; t < bin.Nodes.Length; t++)
            {
                int BonesIdAmount = bin.Nodes[t].NodeHeaderArray[0x3];
                byte[] useBoneList = bin.Nodes[t].NodeHeaderArray.Skip(4).Take(BonesIdAmount).ToArray();


                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    List<string> Weights = new List<string>();

                    if (bin.Nodes[t].Segments[i].WeightMapTableLines != null)
                    {
                        for (int l = 0; l < bin.Nodes[t].Segments[i].WeightMapTableLines.Length; l++)
                        {
                            int Amount = bin.Nodes[t].Segments[i].WeightMapTableLines[l].Amount;
                            string res = Amount.ToString();

                            if (Amount > 0)
                            {
                                uint UseId = (uint)(bin.Nodes[t].Segments[i].WeightMapTableLines[l].boneId1 / 4);
                                uint UseBone = 0;
                                if (UseId < useBoneList.Length)
                                {
                                    UseBone = useBoneList[UseId];
                                }

                                res += " " + UseBone + " " +
                                    bin.Nodes[t].Segments[i].WeightMapTableLines[l].weight1.ToFloatString();
                            }

                            if (Amount > 1)
                            {
                                uint UseId = (uint)(bin.Nodes[t].Segments[i].WeightMapTableLines[l].boneId2 / 4);
                                uint UseBone = 0;
                                if (UseId < useBoneList.Length)
                                {
                                    UseBone = useBoneList[UseId];
                                }

                                res += " " + UseBone + " " +
                                    bin.Nodes[t].Segments[i].WeightMapTableLines[l].weight2.ToFloatString();
                            }

                            if (Amount > 2)
                            {
                                uint UseId = (uint)(bin.Nodes[t].Segments[i].WeightMapTableLines[l].boneId3 / 4);
                                uint UseBone = 0;
                                if (UseId < useBoneList.Length)
                                {
                                    UseBone = useBoneList[UseId];
                                }

                                res += " " + UseBone + " " +
                                    bin.Nodes[t].Segments[i].WeightMapTableLines[l].weight3.ToFloatString();
                            }
                            Weights.Add(res);
                        }
                    }


                    bool invFace = false;
                    int counter = 0;
                    while (counter < bin.Nodes[t].Segments[i].vertexLines.Length)
                    {

                        if ((counter - 2) > -1 &&
                           (bin.Nodes[t].Segments[i].vertexLines[counter].IndexComplement == 0)
                           )
                        {
                            text.WriteLine(CONSTs.PS2_MATERIAL + t.ToString("D3"));

                            string[] pos = new string[3];
                            string[] normals = new string[3];
                            string[] uvs = new string[3];
                            string[] VertexWeights = new string[3];


                            for (int l = 2; l > -1; l--)
                            {
                                VertexLine vertexLine = bin.Nodes[t].Segments[i].vertexLines[counter - l];

                                pos[l] = (((float)vertexLine.VerticeX * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_SCALE).ToFloatString() + " " +
                                          ((float)vertexLine.VerticeZ * -1f * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_SCALE).ToFloatString() + " " +
                                          ((float)vertexLine.VerticeY * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_SCALE).ToFloatString());

                                uvs[l] = (((float)vertexLine.TextureU / 255f).ToFloatString() + " " +
                                          ((float)vertexLine.TextureV / 255f).ToFloatString());

                                if (bin.binType != BinType.ScenarioWithColors)
                                {
                                    float nx = vertexLine.NormalX;
                                    float ny = vertexLine.NormalY;
                                    float nz = vertexLine.NormalZ;

                                    float NORMAL_FIX = (float)Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));
                                    NORMAL_FIX = (NORMAL_FIX == 0) ? 1 : NORMAL_FIX;
                                    nx /= NORMAL_FIX;
                                    ny /= NORMAL_FIX;
                                    nz /= NORMAL_FIX * -1;

                                    normals[l] = ((nx).ToFloatString() + " " +
                                                  (nz).ToFloatString() + " " +
                                                  (ny).ToFloatString());
                                }
                                else
                                {
                                    normals[l] = "0.0 0.0 0.0";
                                }

                                int WeightIndex = (vertexLine.UnknownB / 2);
                                if (Weights.Count != 0 && WeightIndex < Weights.Count)
                                {

                                    VertexWeights[l] = Weights[WeightIndex];
                                }
                                else
                                {
                                    VertexWeights[l] = "0";
                                }
                            }


                            string a = "0 " + pos[0] + " " + normals[0] + " " + uvs[0] + " " + VertexWeights[0];
                            string b = "0 " + pos[1] + " " + normals[1] + " " + uvs[1] + " " + VertexWeights[1];
                            string c = "0 " + pos[2] + " " + normals[2] + " " + uvs[2] + " " + VertexWeights[2];


                            if (invFace)
                            {
                                text.WriteLine(a);
                                text.WriteLine(b);
                                text.WriteLine(c);

                                invFace = false;
                            }
                            else
                            {
                                text.WriteLine(c);
                                text.WriteLine(b);
                                text.WriteLine(a);

                                invFace = true;
                            }

                        }
                        else
                        {
                            invFace = false;
                        }

                        counter++;
                    }

                }

            }

            text.WriteLine("end");
            text.WriteLine("// RE4_PS2_BIN_TOOL" + Environment.NewLine +
                           "// By JADERLINK and HardRain" + Environment.NewLine +
                          $"// Version {Program.VERSION}");
            text.Close();

        }

        public static void CreateOBJ(PS2BIN bin, string baseDirectory, string baseFileName)
        {

            TextWriter text = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".obj")).CreateText();
            text.WriteLine(Program.headerText());

            text.WriteLine("mtllib " + baseFileName + ".mtl");

            int indexGeral = 1;

            for (int t = 0; t < bin.Nodes.Length; t++)
            {
                text.WriteLine("g " + CONSTs.PS2_MATERIAL + t.ToString("D3"));
                text.WriteLine("usemtl " + CONSTs.PS2_MATERIAL + t.ToString("D3"));

                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    for (int l = 0; l < bin.Nodes[t].Segments[i].vertexLines.Length; l++)
                    {
                        VertexLine vertexLine = bin.Nodes[t].Segments[i].vertexLines[l];

                        string v = "v " + 
                             ((float)vertexLine.VerticeX * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_SCALE).ToFloatString() + " " +
                             ((float)vertexLine.VerticeY * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_SCALE).ToFloatString() + " " +
                             ((float)vertexLine.VerticeZ * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_SCALE).ToFloatString();

                        if (bin.binType == BinType.ScenarioWithColors)
                        {
                            v += " " +
                             ((float)vertexLine.NormalX / 128f).ToFloatString() + " " +
                             ((float)vertexLine.NormalY / 128f).ToFloatString() + " " +
                             ((float)vertexLine.NormalZ / 128f).ToFloatString() + " " +
                             ((float)vertexLine.UnknownB / 128f).ToFloatString();
                        }

                        text.WriteLine(v);

                        text.WriteLine("vt " + (vertexLine.TextureU / 255f).ToFloatString() + " " + (vertexLine.TextureV / 255f).ToFloatString());

                        if (bin.binType != BinType.ScenarioWithColors)
                        {
                            float nx = vertexLine.NormalX; // antigamente era dividido por 127f 
                            float ny = vertexLine.NormalY;
                            float nz = vertexLine.NormalZ;

                            float NORMAL_FIX = (float)Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));
                            NORMAL_FIX = (NORMAL_FIX == 0) ? 1 : NORMAL_FIX;
                            nx /= NORMAL_FIX;
                            ny /= NORMAL_FIX;
                            nz /= NORMAL_FIX;

                            text.WriteLine("vn " + nx.ToFloatString() + " " + ny.ToFloatString() + " " + nz.ToFloatString());
                        }
                    }


                    bool invFace = false;
                    int counter = 0;
                    while (counter < bin.Nodes[t].Segments[i].vertexLines.Length)
                    {
                        string a = (indexGeral - 2).ToString();
                        string b = (indexGeral - 1).ToString();
                        string c = (indexGeral).ToString();

                        if ((counter - 2) > -1 &&
                           (bin.Nodes[t].Segments[i].vertexLines[counter].IndexComplement == 0)
                           )
                        {
                            if (invFace)
                            {
                                if (bin.binType != BinType.ScenarioWithColors)
                                {
                                    text.WriteLine("f " + c + "/" + c + "/" + c + " " +
                                                          b + "/" + b + "/" + b + " " +
                                                          a + "/" + a + "/" + a);
                                }
                                else
                                {
                                    text.WriteLine("f " + c + "/" + c + " " +
                                                          b + "/" + b + " " +
                                                          a + "/" + a);
                                }

                                invFace = false;
                            }
                            else
                            {
                                if (bin.binType != BinType.ScenarioWithColors)
                                {
                                    text.WriteLine("f " + a + "/" + a + "/" + a + " " +
                                                          b + "/" + b + "/" + b + " " +
                                                          c + "/" + c + "/" + c);
                                }
                                else
                                {
                                    text.WriteLine("f " + a + "/" + a + " " +
                                                          b + "/" + b + " " +
                                                          c + "/" + c);
                                }

                                invFace = true;
                            }


                        }
                        else
                        {
                            invFace = false;
                        }

                        counter++;
                        indexGeral++;
                    }


                }

            }

            text.Close();

        }

        public static void CreateIdxbin(PS2BIN bin, string baseDirectory, string baseFileName)
        {
            var inv = System.Globalization.CultureInfo.InvariantCulture;

            TextWriter IDXBINtext = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".idxbin")).CreateText();
            IDXBINtext.WriteLine(Program.headerText());

            IDXBINtext.WriteLine();
            IDXBINtext.WriteLine("CompressVertices:True");
            IDXBINtext.WriteLine("IsScenarioBin:" + (bin.binType == BinType.ScenarioWithColors));

            IDXBINtext.WriteLine();

            IDXBINtext.WriteLine(": 2 bytes in hex (nTex)");
            IDXBINtext.WriteLine("unknown1:" + BitConverter.ToString(BitConverter.GetBytes(bin.Unknown1)).Replace("-", "")); // compatibilidade com versões antigas

            IDXBINtext.WriteLine(": 1 bytes in hex (frac)");
            IDXBINtext.WriteLine("unknown2:" + bin.Unknown2.ToString("X2"));

            IDXBINtext.WriteLine(": 4 bytes in hex (versionFlag)");
            IDXBINtext.WriteLine("unknown4_B:" + BitConverter.ToString(BitConverter.GetBytes(bin.VersionFlag)).Replace("-", "")); // compatibilidade com versões antigas

            IDXBINtext.WriteLine(": 4 bytes in hex (textureFlags)");
            IDXBINtext.WriteLine("unknown4_unk009:" + BitConverter.ToString(BitConverter.GetBytes(bin.TextureFlags)).Replace("-", "")); // compatibilidade com versões antigas

            IDXBINtext.WriteLine();
            IDXBINtext.WriteLine(": ## DrawDistance float values ##");
            IDXBINtext.WriteLine("DrawDistanceNegativeX:" + bin.DrawDistanceNegativeX.ToString("f9", inv));
            IDXBINtext.WriteLine("DrawDistanceNegativeY:" + bin.DrawDistanceNegativeY.ToString("f9", inv));
            IDXBINtext.WriteLine("DrawDistanceNegativeZ:" + bin.DrawDistanceNegativeZ.ToString("f9", inv));
            IDXBINtext.WriteLine("DrawDistanceNegativePadding:" + bin.DrawDistanceNegativePadding.ToString("f9", inv));
            IDXBINtext.WriteLine("DrawDistancePositiveX:" + bin.DrawDistancePositiveX.ToString("f9", inv));
            IDXBINtext.WriteLine("DrawDistancePositiveY:" + bin.DrawDistancePositiveY.ToString("f9", inv));
            IDXBINtext.WriteLine("DrawDistancePositiveZ:" + bin.DrawDistancePositiveZ.ToString("f9", inv));


            if (bin.BonepairCount != 0)
            {
                IDXBINtext.WriteLine();
                IDXBINtext.WriteLine(": ## bonepair ##");
                IDXBINtext.WriteLine(": bonepairCount in decimal value");
                IDXBINtext.WriteLine("bonepairCount:" + bin.BonepairCount.ToString());

                IDXBINtext.WriteLine(": bonepairLines -> 8 bytes in hex");
                for (int i = 0; i < bin.BonepairCount; i++)
                {
                    IDXBINtext.WriteLine("bonepairLine_" + i + ":" + BitConverter.ToString(bin.bonepairLines[i]).Replace("-", ""));
                }
            }


            IDXBINtext.WriteLine();
            IDXBINtext.WriteLine(": ## Bones ##");
            IDXBINtext.WriteLine(": BonesCount in decimal value");
            IDXBINtext.WriteLine("BonesCount:" + bin.BonesCount.ToString());

            IDXBINtext.WriteLine(": BoneLines -> 16 bytes in hex");
            for (int i = 0; i < bin.bones.Length; i++)
            {
                IDXBINtext.WriteLine("BoneLine_" + i + ":" + BitConverter.ToString(bin.bones[i].boneLine).Replace("-", ""));
            }

            IDXBINtext.Close();
        }

    }
}
