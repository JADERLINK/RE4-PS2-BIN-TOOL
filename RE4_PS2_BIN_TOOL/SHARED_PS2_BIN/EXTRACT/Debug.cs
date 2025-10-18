using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SHARED_TOOLS.ALL;
using SHARED_PS2_BIN.ALL;

namespace SHARED_PS2_BIN.EXTRACT
{
    public static class Debug
    {

        public static void Info(PS2BIN bin, string baseDirectory, string baseFileName)
        {
            var txt2 = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".Debug.txt2")).CreateText();

            txt2.WriteLine(SHARED_TOOLS.Shared.HeaderText());
            txt2.WriteLine("");

            txt2.WriteLine("Magic: 0x" + bin.Magic.ToString("X4"));
            txt2.WriteLine("Tex_count: 0x" + bin.Tex_count.ToString("X4"));
            txt2.WriteLine("BonesPoint: 0x" + bin.BonesPoint.ToString("X8"));
            txt2.WriteLine("Vertex_Scale: 0x" + bin.Vertex_Scale.ToString("X2"));
            txt2.WriteLine("BonesCount: " + bin.BonesCount.ToString());
            txt2.WriteLine("MaterialCount: " + bin.MaterialCount.ToString());
            txt2.WriteLine("MaterialOffset: 0x" + bin.MaterialOffset.ToString("X8"));
            txt2.WriteLine("Padding1: 0x" + bin.Padding1.ToString("X8"));
            txt2.WriteLine("Padding2: 0x" + bin.Padding2.ToString("X8"));
            txt2.WriteLine("Version_flags: 0x" + bin.Version_flags.ToString("X8"));
            txt2.WriteLine("BonepairPoint: 0x" + bin.BonepairPoint.ToString("X8"));
            txt2.WriteLine("UnusedOffset1: 0x" + bin.UnusedOffset1.ToString("X8"));
            txt2.WriteLine("Bin_flags: 0x" + bin.Bin_flags.ToString("X8"));
            txt2.WriteLine("BonepairPoint: 0x" + bin.BonepairPoint.ToString("X8"));
            txt2.WriteLine("UnusedOffset2: 0x" + bin.UnusedOffset2.ToString("X8"));
            txt2.WriteLine("BoundingBoxPosX: " + bin.BoundingBoxPosX.ToFloatString() + "f");
            txt2.WriteLine("BoundingBoxPosY: " + bin.BoundingBoxPosY.ToFloatString() + "f");
            txt2.WriteLine("BoundingBoxPosZ: " + bin.BoundingBoxPosZ.ToFloatString() + "f");
            txt2.WriteLine("BoundingBoxPosW: " + bin.BoundingBoxPosW.ToFloatString() + "f");
            txt2.WriteLine("BoundingBoxWidth: " + bin.BoundingBoxWidth.ToFloatString() + "f");
            txt2.WriteLine("BoundingBoxHeight: " + bin.BoundingBoxHeight.ToFloatString() + "f");
            txt2.WriteLine("BoundingBoxDepth: " + bin.BoundingBoxDepth.ToFloatString() + "f");
            txt2.WriteLine("Padding3: " + bin.Padding3.ToString("X8"));
            txt2.WriteLine("");
            //end header

            //bonepair_addr
            if (bin.BonepairPoint != 0)
            {
                txt2.WriteLine("");
                txt2.WriteLine("BonepairPoint: 0x" + bin.BonepairPoint.ToString("X8"));
                txt2.WriteLine("bonepairCount: " + bin.BonePairs.Length);
                for (int i = 0; i < bin.BonePairs.Length; i++)
                {
                    txt2.WriteLine("BonePair " + "[" + i.ToString("X2") + "]: " + 
                       bin.BonePairs[i].Bone1.ToString().PadLeft(4) + " " +
                       bin.BonePairs[i].Bone2.ToString().PadLeft(4) + " " +
                       bin.BonePairs[i].Bone3.ToString().PadLeft(4) + " " +
                       bin.BonePairs[i].Bone4.ToString().PadLeft(4)
                       );
                }
                txt2.WriteLine("");
            }

            //bonesPoint
            txt2.WriteLine("");
            txt2.WriteLine("bones:   (in hexadecimal)");
            for (int i = 0; i < bin.BonesCount; i++)
            {
                txt2.WriteLine("[" + i.ToString("X2") + "]: " + BitConverter.ToString(bin.Bones[i].boneLine));
            }
            txt2.WriteLine("");

            //MaterialOffset
            txt2.WriteLine("");
            txt2.WriteLine("MaterialList:   (in hexadecimal)");
            for (int i = 0; i < bin.MaterialCount; i++)
            {
                txt2.WriteLine("[" + i + "]: " + BitConverter.ToString(bin.materials[i].materialLine) + "     NodeTablePoint: 0x" + bin.materials[i].nodeTablePoint.ToString("X8"));
            }
            txt2.WriteLine("");
            txt2.WriteLine("---------------");

            for (int t = 0; t < bin.MaterialCount; t++)
            {
                txt2.WriteLine("");
                txt2.WriteLine("NodeTablePointer: 0x" + bin.materials[t].nodeTablePoint.ToString("X8"));
                txt2.WriteLine("NodeHeaderArray: " + BitConverter.ToString(bin.Nodes[t].NodeHeaderArray) + "  {hex}");
                txt2.WriteLine("TotalBytesAmount: 0x" + bin.Nodes[t].TotalBytesAmount.ToString("X4"));
                txt2.WriteLine("segmentAmountWithoutFirst: " + bin.Nodes[t].segmentAmountWithoutFirst);
                txt2.WriteLine("BonesIdAmount: " + bin.Nodes[t].BonesIdAmount);
                txt2.WriteLine("NodeBoneList: " + BitConverter.ToString(bin.Nodes[t].NodeBoneList) + "  {hex}");
                txt2.WriteLine("TotalNumberOfSegments: " + (bin.Nodes[t].segmentAmountWithoutFirst + 1));
                txt2.WriteLine("");

                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {

                    if (bin.Nodes[t].Segments[i].WeightMapHeader != null)
                    {
                        //WeightMap
                        txt2.WriteLine("WeightMap");
                        txt2.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] WeightMapHeader: " + BitConverter.ToString(bin.Nodes[t].Segments[i].WeightMapHeader));
                        txt2.WriteLine("WeightMapTableBytesAmount: 0x" + (bin.Nodes[t].Segments[i].WeightMapTableLines.Length * 0x20).ToString("X4"));
                        txt2.WriteLine("WeightMapTableLinesAmount: " + bin.Nodes[t].Segments[i].WeightMapTableLines.Length.ToString());
                        for (int a = 0; a < bin.Nodes[t].Segments[i].WeightMapTableLines.Length; a++)
                        {
                            txt2.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + a.ToString("D2") + "]: " + BitConverter.ToString(bin.Nodes[t].Segments[i].WeightMapTableLines[a].weightMapTableLine));
                        }
                 
                        txt2.WriteLine("");
                    }

                    txt2.WriteLine("");
                    txt2.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] TopTagVifHeader:");
                    txt2.WriteLine("TopTagVifHeader2080:      " + BitConverter.ToString(bin.Nodes[t].Segments[i].TopTagVifHeader2080));
                    txt2.WriteLine("TopTagVifHeaderWithScale: " + BitConverter.ToString(bin.Nodes[t].Segments[i].TopTagVifHeaderWithScale));
                    txt2.WriteLine("TopTagVifHeader2180:      " + BitConverter.ToString(bin.Nodes[t].Segments[i].TopTagVifHeader2180));
                    txt2.WriteLine("chunkByteAmount: 0x" + (bin.Nodes[t].Segments[i].TopTagVifHeader2180[0] * 0x10).ToString("X4"));
                    txt2.WriteLine("LineAmount in decimal: " + bin.Nodes[t].Segments[i].TopTagVifHeaderWithScale[0]);
                    txt2.WriteLine("ConversionFactorValue: " + bin.Nodes[t].Segments[i].ConversionFactorValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    txt2.WriteLine("IsScenarioColor: " + bin.Nodes[t].Segments[i].IsScenarioColor);
                    txt2.WriteLine("");
                    txt2.WriteLine("subMeshChunk:");

                    if (bin.Nodes[t].Segments[i].IsScenarioColor)
                    {
                        txt2.WriteLine(".[MaterialCount][TotalNumberOfSegments][line]: VerticeX, VerticeY, VerticeZ, IndexMount, TextureU, TextureV, UnknownA, IndexComplement, ColorR, ColorG, ColorB, UnknownB");
                    }
                    else
                    {
                        txt2.WriteLine(".[MaterialCount][TotalNumberOfSegments][line]: VerticeX, VerticeY, VerticeZ, UnknownB, NormalX, NormalY, NormalZ, IndexComplement, TextureU, TextureV, UnknownA, IndexMount");
                    }


                    for (int l = 0; l < bin.Nodes[t].Segments[i].vertexLines.Length; l++)
                    {
                        var v = bin.Nodes[t].Segments[i].vertexLines[l];
                        if (bin.Nodes[t].Segments[i].IsScenarioColor)
                        {
                            txt2.WriteLine(".[" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + l.ToString("D2") + "]:" +
                              "  vX: " + v.VerticeX.ToString().PadLeft(6) +
                              "  vY: " + v.VerticeY.ToString().PadLeft(6) +
                              "  vZ: " + v.VerticeZ.ToString().PadLeft(6) +
                              "  IndexMount: " + v.IndexMount.ToString("X4") +
                              "  tU: " + v.TextureU.ToString().PadLeft(6) +
                              "  tV: " + v.TextureV.ToString().PadLeft(6) +
                              "  UnkA: " + v.UnknownA.ToString("X4") +
                              "  IdxComp: " + v.IndexComplement.ToString("X4") +
                              "  cR: " + v.NormalX.ToString().PadLeft(6) +
                              "  cG: " + v.NormalY.ToString().PadLeft(6) +
                              "  cB: " + v.NormalZ.ToString().PadLeft(6) +
                              "  UnkB: " + v.UnknownB.ToString("X4")
                             );
                        }
                        else 
                        {
                            txt2.WriteLine(".[" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + l.ToString("D2") + "]:" +
                                  "  vX: " + v.VerticeX.ToString().PadLeft(6) +
                                  "  vY: " + v.VerticeY.ToString().PadLeft(6) +
                                  "  vZ: " + v.VerticeZ.ToString().PadLeft(6) +
                                  "  UnkB: " + v.UnknownB.ToString("X4") +
                                  "  nX: " + v.NormalX.ToString().PadLeft(6) +
                                  "  nY: " + v.NormalY.ToString().PadLeft(6) +
                                  "  nZ: " + v.NormalZ.ToString().PadLeft(6) +
                                  "  IdxComp: " + v.IndexComplement.ToString("X4") +
                                  "  tU: " + v.TextureU.ToString().PadLeft(6) +
                                  "  tV: " + v.TextureV.ToString().PadLeft(6) +
                                  "  UnkA: " + v.UnknownA.ToString("X4") +
                                  "  IndexMount: " + v.IndexMount.ToString("X4"));
                        }

                    }

                    txt2.WriteLine("");

                    if (bin.Nodes[t].Segments[i].EndTagVifCommand != null)
                    {
                        txt2.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] EndTagVifCommand: " + BitConverter.ToString(bin.Nodes[t].Segments[i].EndTagVifCommand));
                        txt2.WriteLine("");
                    }

                }
            }

            txt2.WriteLine("");
            txt2.WriteLine("End File");

            txt2.Close();
        }

        public static void CreateBoundingBoxOBJ(string fullFilePath, BoundingBox box)
        {
            TextWriter text = new FileInfo(fullFilePath).CreateText();
            text.WriteLine(SHARED_TOOLS.Shared.HeaderText());
            text.WriteLine("");

            string NX = ((box.BoundingBoxPosX - box.BoundingBoxWidth) / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString();
            string NY = ((box.BoundingBoxPosY - box.BoundingBoxHeight) / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString();
            string NZ = ((box.BoundingBoxPosZ - box.BoundingBoxDepth) / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString();

            string PX = ((box.BoundingBoxPosX + box.BoundingBoxWidth) / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString();
            string PY = ((box.BoundingBoxPosY + box.BoundingBoxHeight) / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString();
            string PZ = ((box.BoundingBoxPosZ + box.BoundingBoxDepth) / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString();

            // real

            //1
            text.WriteLine("v " + NX + " " + NY + " " + NZ);

            //2
            text.WriteLine("v " + PX + " " + PY + " " + PZ);

            //inverso Y

            //3
            text.WriteLine("v " + NX + " " + PY + " " + NZ);

            //4
            text.WriteLine("v " + PX + " " + NY + " " + PZ);

            // inveso Z

            //5
            text.WriteLine("v " + NX + " " + NY + " " + PZ);

            //6
            text.WriteLine("v " + PX + " " + PY + " " + NZ);

            // inverso X

            //7
            text.WriteLine("v " + PX + " " + NY + " " + NZ);

            //8
            text.WriteLine("v " + NX + " " + PY + " " + PZ);


            text.WriteLine("g BoundingBoxLine");
            text.WriteLine("l 1 2");

            text.WriteLine("g BoundingBox");
            //text.WriteLine("g l13");
            text.WriteLine("l 1 3"); //ok
            //text.WriteLine("g l24");
            text.WriteLine("l 2 4"); //ok
            //text.WriteLine("g l15");
            text.WriteLine("l 1 5"); //ok
            //text.WriteLine("g l26");
            text.WriteLine("l 2 6"); //ok
            //text.WriteLine("g l36");
            text.WriteLine("l 3 6"); //ok
            //text.WriteLine("g l45");
            text.WriteLine("l 4 5"); //ok
            //text.WriteLine("g l17");
            text.WriteLine("l 1 7"); //ok
            //text.WriteLine("g l28");
            text.WriteLine("l 2 8"); //ok
            //text.WriteLine("g l58");
            text.WriteLine("l 5 8"); //ok
            //text.WriteLine("g l67");
            text.WriteLine("l 6 7"); //ok
            //text.WriteLine("g l47");
            text.WriteLine("l 4 7"); //ok
            //text.WriteLine("g l38");
            text.WriteLine("l 3 8"); //ok

            text.Close();
        }

    }
}
