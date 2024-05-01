using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_BIN_TOOL.EXTRACT
{
    public static class Debug
    {

        public static void Info(PS2BIN bin, string baseDirectory, string baseFileName)
        {
            var txt2 = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".Debug.txt2")).CreateText();

            txt2.WriteLine(Program.headerText());
            txt2.WriteLine("");

            txt2.WriteLine("Magic: 0x" + bin.Magic.ToString("X4"));
            txt2.WriteLine("Unknown1: 0x" + bin.Unknown1.ToString("X4"));
            txt2.WriteLine("BonesPoint: 0x" + bin.BonesPoint.ToString("X8"));
            txt2.WriteLine("Unknown2: 0x" + bin.Unknown2.ToString("X2"));
            txt2.WriteLine("BonesCount: " + bin.BonesCount.ToString());
            txt2.WriteLine("MaterialCount: " + bin.MaterialCount.ToString());
            txt2.WriteLine("MaterialOffset: 0x" + bin.MaterialOffset.ToString("X8"));
            txt2.WriteLine("Padding1: 0x" + bin.Padding1.ToString("X8"));
            txt2.WriteLine("Padding2: 0x" + bin.Padding2.ToString("X8"));
            txt2.WriteLine("VersionFlag: 0x" + bin.VersionFlag.ToString("X8 ")+ "  {unknown4_B}");
            txt2.WriteLine("BonepairPoint: 0x" + bin.BonepairPoint.ToString("X8"));
            txt2.WriteLine("Unknown408: 0x" + bin.Unknown408.ToString("X8") + "  {unknown4_unk008}");
            txt2.WriteLine("TextureFlags: 0x" + bin.TextureFlags.ToString("X8") + "  {unknown4_unk009}");
            txt2.WriteLine("BonepairPoint: 0x" + bin.BonepairPoint.ToString("X8"));
            txt2.WriteLine("Unknown410: 0x" + bin.Unknown410.ToString("X8") + "  {unknown4_unk010}");
            txt2.WriteLine("unk012: XYZ padding");
            txt2.WriteLine("DrawDistanceNegativeX: " + bin.DrawDistanceNegativeX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + "f");
            txt2.WriteLine("DrawDistanceNegativeY: " + bin.DrawDistanceNegativeY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + "f");
            txt2.WriteLine("DrawDistanceNegativeZ: " + bin.DrawDistanceNegativeZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + "f");
            txt2.WriteLine("DrawDistanceNegativePadding: " + bin.DrawDistanceNegativePadding.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + "f");
            txt2.WriteLine("unk013: XYZ");
            txt2.WriteLine("DrawDistancePositiveX: " + bin.DrawDistancePositiveX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + "f");
            txt2.WriteLine("DrawDistancePositiveY: " + bin.DrawDistancePositiveY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + "f");
            txt2.WriteLine("DrawDistancePositiveZ: " + bin.DrawDistancePositiveZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + "f");
            txt2.WriteLine("Padding3: " + bin.Padding3.ToString("X8"));
            txt2.WriteLine("");
            //end header

            //bonepair_addr_
            if (bin.BonepairPoint != 0)
            {
                txt2.WriteLine("");
                txt2.WriteLine("BonepairPoint: 0x" + bin.BonepairPoint.ToString("X8"));
                txt2.WriteLine("bonepairCount: " + bin.BonepairCount);
                for (int i = 0; i < bin.BonepairCount; i++)
                {
                    txt2.WriteLine("[" + i.ToString("X2") + "]: " + BitConverter.ToString(bin.bonepairLines[i]));
                }
                txt2.WriteLine("");
            }

            //bonesPoint
            txt2.WriteLine("");
            txt2.WriteLine("bones:   (in hexadecimal)");
            for (int i = 0; i < bin.BonesCount; i++)
            {
                txt2.WriteLine("[" + i.ToString("X2") + "]: " + BitConverter.ToString(bin.bones[i].boneLine));
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

        //debug
        public static void CreateDrawDistanceBoxObj(PS2BIN bin, string baseDirectory, string baseFileName)
        {
            TextWriter text = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".Debug.DrawDistanceBox.obj")).CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            string DrawDistanceNegativeX = (bin.DrawDistanceNegativeX / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeY = (bin.DrawDistanceNegativeY / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeZ = (bin.DrawDistanceNegativeZ / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);

            string DrawDistancePositiveX = ((bin.DrawDistanceNegativeX + bin.DrawDistancePositiveX) / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveY = ((bin.DrawDistanceNegativeY + bin.DrawDistancePositiveY) / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveZ = ((bin.DrawDistanceNegativeZ + bin.DrawDistancePositiveZ) / CONSTs.GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);

            // real

            //1
            text.WriteLine("v " + DrawDistanceNegativeX + " " + DrawDistanceNegativeY + " " + DrawDistanceNegativeZ);

            //2
            text.WriteLine("v " + DrawDistancePositiveX + " " + DrawDistancePositiveY + " " + DrawDistancePositiveZ);

            //inverso Y

            //3
            text.WriteLine("v " + DrawDistanceNegativeX + " " + DrawDistancePositiveY + " " + DrawDistanceNegativeZ);

            //4
            text.WriteLine("v " + DrawDistancePositiveX + " " + DrawDistanceNegativeY + " " + DrawDistancePositiveZ);

            // inveso Z

            //5
            text.WriteLine("v " + DrawDistanceNegativeX + " " + DrawDistanceNegativeY + " " + DrawDistancePositiveZ);

            //6
            text.WriteLine("v " + DrawDistancePositiveX + " " + DrawDistancePositiveY + " " + DrawDistanceNegativeZ);

            // inverso X

            //7
            text.WriteLine("v " + DrawDistancePositiveX + " " + DrawDistanceNegativeY + " " + DrawDistanceNegativeZ);

            //8
            text.WriteLine("v " + DrawDistanceNegativeX + " " + DrawDistancePositiveY + " " + DrawDistancePositiveZ);


            text.WriteLine("g Original");
            text.WriteLine("l 1 2");

            text.WriteLine("g Box");
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

        //debug
        public static void CreateScaleLimitBoxObj(PS2BIN bin, string baseDirectory, string baseFileName)
        {

            TextWriter text = new FileInfo(Path.Combine(baseDirectory, baseFileName + ".Debug.ScaleLimitBox.obj")).CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            float scalenodes = 0;
            for (int t = 0; t < bin.Nodes.Length; t++)
            {
                float allScales = 0;
                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    allScales += bin.Nodes[t].Segments[i].ConversionFactorValue;
                }

                scalenodes += allScales / bin.Nodes[t].Segments.Length;
            }
            float scale = scalenodes / bin.Nodes.Length;

            float maxPos = scale * short.MaxValue / CONSTs.GLOBAL_SCALE;
            float minPos = scale * short.MinValue / CONSTs.GLOBAL_SCALE;

            text.WriteLine("# scale: " + scale.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            text.WriteLine("");


            //1
            text.WriteLine("v " + minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //2
            text.WriteLine("v " + maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //inverso Y

            //3
            text.WriteLine("v " + minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //4
            text.WriteLine("v " + maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            // inveso Z

            //5
            text.WriteLine("v " + minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //6
            text.WriteLine("v " + maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            // inverso X

            //7
            text.WriteLine("v " + maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //8
            text.WriteLine("v " + minPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  maxPos.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //9
            text.WriteLine("v " + 0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  scale.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );

            //10
            text.WriteLine("v " + scale.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );



            //11
            text.WriteLine("v " + 0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                  0.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                                  );


            text.WriteLine("g shorterDistance");
            text.WriteLine("f 9 10 11");

            text.WriteLine("g Box");
            text.WriteLine("l 1 3");
            text.WriteLine("l 2 4");
            text.WriteLine("l 1 5");
            text.WriteLine("l 2 6");
            text.WriteLine("l 3 6");
            text.WriteLine("l 4 5");
            text.WriteLine("l 1 7");
            text.WriteLine("l 2 8");
            text.WriteLine("l 5 8");
            text.WriteLine("l 6 7");
            text.WriteLine("l 4 7");
            text.WriteLine("l 3 8");

            text.Close();

        }

    }
}
