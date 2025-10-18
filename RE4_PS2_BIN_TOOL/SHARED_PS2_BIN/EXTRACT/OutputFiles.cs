using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SHARED_TOOLS.ALL;
using SHARED_PS2_BIN.ALL;

namespace SHARED_PS2_BIN.EXTRACT
{
    public static class OutputFiles
    {

        //Studiomdl Data
        public static void CreateSMD(PS2BIN bin, string baseDirectory, string baseFileName)
        {

            TextWriter text = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".smd")).CreateText();

            text.WriteLine("version 1");
            text.WriteLine("nodes");

            //Bones Fix
            (uint BoneID, short BoneParent, float p1, float p2, float p3)[] FixedBones = new (uint BoneID, short BoneParent, float p1, float p2, float p3)[bin.Bones.Length];

            // Bone ID, number of times found
            Dictionary<byte, int> BoneCheck = new Dictionary<byte, int>();
            for (int i = bin.Bones.Length - 1; i >= 0; i--)
            {
                byte InBoneID = bin.Bones[i].BoneID;
                uint OutBoneID = InBoneID;
                if (BoneCheck.ContainsKey(InBoneID))
                {
                    OutBoneID += (uint)(0x100u * BoneCheck[InBoneID]);
                    BoneCheck[InBoneID]++;
                }
                else
                {
                    BoneCheck.Add(InBoneID, 1);
                }

                short BoneParent = bin.Bones[i].BoneParent;
                if (BoneParent == 0xFF)
                {
                    BoneParent = -1;
                }

                float p1 = bin.Bones[i].PositionX / CONSTs.GLOBAL_POSITION_SCALE;
                float p2 = bin.Bones[i].PositionZ * -1 / CONSTs.GLOBAL_POSITION_SCALE;
                float p3 = bin.Bones[i].PositionY / CONSTs.GLOBAL_POSITION_SCALE;

                FixedBones[i] = (OutBoneID, BoneParent, p1, p2, p3);
            }

            for (int i = 0; i < FixedBones.Length; i++)
            {
                text.WriteLine(FixedBones[i].BoneID + " \"BONE_" + FixedBones[i].BoneID.ToString("D3") + "\" " + FixedBones[i].BoneParent);
            }

            text.WriteLine("end");

            text.WriteLine("skeleton");
            text.WriteLine("time 0");

            for (int i = 0; i < FixedBones.Length; i++)
            {
                text.WriteLine(FixedBones[i].BoneID + "  " +
                               FixedBones[i].p1.ToFloatString() + " " +
                               FixedBones[i].p2.ToFloatString() + " " +
                               FixedBones[i].p3.ToFloatString() + "  0.0 0.0 0.0");
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
                            text.WriteLine(CONSTs.MATERIAL + t.ToString("D3"));

                            string[] pos = new string[3];
                            string[] normals = new string[3];
                            string[] uvs = new string[3];
                            string[] VertexWeights = new string[3];


                            for (int l = 2; l > -1; l--)
                            {
                                VertexLine vertexLine = bin.Nodes[t].Segments[i].vertexLines[counter - l];

                                pos[l] = (((float)vertexLine.VerticeX * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString() + " " +
                                          ((float)vertexLine.VerticeZ * -1f * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString() + " " +
                                          ((float)vertexLine.VerticeY * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString());

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
            text.WriteLine(SHARED_TOOLS.Shared.HeaderTextSmd());
            text.Close();

        }

        public static void CreateOBJ(PS2BIN bin, string baseDirectory, string baseFileName)
        {

            TextWriter text = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".obj")).CreateText();
            text.WriteLine(SHARED_TOOLS.Shared.HeaderText());

            text.WriteLine("mtllib " + baseFileName + ".mtl");

            int indexGeral = 1;

            for (int t = 0; t < bin.Nodes.Length; t++)
            {
                text.WriteLine("g " + CONSTs.MATERIAL + t.ToString("D3"));
                text.WriteLine("usemtl " + CONSTs.MATERIAL + t.ToString("D3"));

                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    for (int l = 0; l < bin.Nodes[t].Segments[i].vertexLines.Length; l++)
                    {
                        VertexLine vertexLine = bin.Nodes[t].Segments[i].vertexLines[l];

                        string v = "v " + 
                             ((float)vertexLine.VerticeX * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString() + " " +
                             ((float)vertexLine.VerticeY * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString() + " " +
                             ((float)vertexLine.VerticeZ * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString();

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

        public static void CreateIdxps2bin(PS2BIN bin, string baseDirectory, string baseFileName)
        {
            TextWriter idx = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".idxps2bin")).CreateText();
            idx.WriteLine(SHARED_TOOLS.Shared.HeaderText());

            idx.WriteLine();
            idx.WriteLine();
            idx.WriteLine("UseIdxMaterial:False");
            idx.WriteLine("IsScenarioBin:" + (bin.binType == BinType.ScenarioWithColors));
            idx.WriteLine("EnableAdjacentBoneTag:" + bin.ReturnsHasEnableAdjacentBoneTag());
            idx.WriteLine("EnableBonepairTag:" + bin.ReturnsHasEnableBonepairTag());
            idx.WriteLine("EnableUnkFlag1:" + bin.ReturnsHasEnableUnkFlag1());
            idx.WriteLine("EnableUnkFlag2:" + bin.ReturnsHasEnableUnkFlag2());
            idx.WriteLine("EnableUnkFlag4:" + bin.ReturnsHasEnableUnkFlag4());
            idx.WriteLine("ObjFileUseBone:" + bin.Bones.Min(x => x.BoneID).ToString());
           
            idx.WriteLine();
            idx.WriteLine();

            idx.WriteLine("AutoCalcBoundingBox:true");
            idx.WriteLine("## BoundingBox: <float>");
            idx.WriteLine("BoundingBoxPosX:" + (bin.BoundingBoxPosX / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString());
            idx.WriteLine("BoundingBoxPosY:" + (bin.BoundingBoxPosY / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString());
            idx.WriteLine("BoundingBoxPosZ:" + (bin.BoundingBoxPosZ / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString());
            idx.WriteLine("BoundingBoxWidth:" + (bin.BoundingBoxWidth / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString());
            idx.WriteLine("BoundingBoxHeight:" + (bin.BoundingBoxHeight / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString());
            idx.WriteLine("BoundingBoxDepth:" + (bin.BoundingBoxDepth / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString());


            idx.WriteLine();
            idx.WriteLine();
            idx.WriteLine("## BoneLine: <boneId:number> <ParentId:number> <x:float> <-z:float> <y:float>");
            for (int i = 0; i < bin.Bones.Length; i++)
            {
                float p1 = bin.Bones[i].PositionX / CONSTs.GLOBAL_POSITION_SCALE;
                float p2 = bin.Bones[i].PositionZ * -1 / CONSTs.GLOBAL_POSITION_SCALE;
                float p3 = bin.Bones[i].PositionY / CONSTs.GLOBAL_POSITION_SCALE;

                idx.WriteLine("BoneLine:" +
                    bin.Bones[i].BoneID.ToString().PadLeft(4) + " " +
                    (bin.Bones[i].BoneParent != 0xFF ? bin.Bones[i].BoneParent : -1).ToString().PadLeft(4) + "   " +
                    p1.ToFloatString() + "  " +
                    p2.ToFloatString() + "  " +
                    p3.ToFloatString()
                    );
            }

            if (bin.BonePairs != null && bin.BonePairs.Length != 0)
            {
                idx.WriteLine();
                idx.WriteLine();
                idx.WriteLine("## BonePair: <bone1:number> <bone2:number> <bone3:number> <unk:number>");

                for (int i = 0; i < bin.BonePairs.Length; i++)
                {
                    idx.WriteLine("BonePair:" +
                       bin.BonePairs[i].Bone1.ToString().PadLeft(4) + " " +
                       bin.BonePairs[i].Bone2.ToString().PadLeft(4) + " " +
                       bin.BonePairs[i].Bone3.ToString().PadLeft(4) + " " +
                       bin.BonePairs[i].Bone4.ToString().PadLeft(4)
                       );
                }
            }

            idx.Close();
        }

    }
}
