using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_BIN_TOOL.EXTRACT
{
    /*
    Codigo feito por JADERLINK
    Pesquisas feitas por HardHain e JaderLink.
    https://www.youtube.com/@JADERLINK
    https://www.youtube.com/@HardRainModder
    */
    public static class BINdecoder
    {
        public static BIN Decode(Stream stream , long startOffset, out long endOffset, AltTextWriter txt2)
        {
            BinaryReader br = new BinaryReader(stream);
            br.BaseStream.Position = startOffset;

            BIN bin = new BIN();
            BinType binType = BinType.Default;

            txt2.WriteLine(Program.headerText());
            txt2.WriteLine("");

            //fix0x3000 //unk001 //unknown0
            //offset 0x00 and 0x01
            byte[] unknown0 = br.ReadBytes(0x2);
            bin.unknown0 = unknown0;
            txt2.WriteLine("unknown0: " + BitConverter.ToString(unknown0) + "  {hex}");


            //unknown1 //unk002
            //offset 0x02 and 0x03
            byte[] unknown1 = br.ReadBytes(0x2);
            bin.unknown1 = unknown1;
            txt2.WriteLine("unknown1: " + BitConverter.ToString(unknown1) + "  {hex}");


            //bones point //bone_addr
            //offset 0x07, 0x06, 0x05 and 0x04
            uint bonesPoint = br.ReadUInt32();
            bin.bonesPoint = bonesPoint;
            txt2.WriteLine("bonesPoint: 0x" + bonesPoint.ToString("X8"));


            //unknown2 //unk003
            //offset 0x08
            byte unknown2 = br.ReadByte();
            bin.unknown2 = unknown2;
            txt2.WriteLine("unknown2: 0x" + unknown2.ToString("X2"));


            //BonesCount //bone_count
            //offset 0x09
            byte BonesCount = br.ReadByte();
            bin.BonesCount = BonesCount;
            txt2.WriteLine("BonesCount: " + BonesCount.ToString());


            //MaterialCount //table1_count
            //offset 0x0B, 0x0A
            ushort MaterialCount = br.ReadUInt16();
            bin.MaterialCount = MaterialCount;
            txt2.WriteLine("MaterialCount: " + MaterialCount.ToString());

            //MaterialsPoint //table1_addr
            //offset 0x0F, 0x0E, 0x0D and 0x0C 
            uint MaterialOffset = br.ReadUInt32();
            bin.MaterialOffset = MaterialOffset;
            txt2.WriteLine("MaterialOffset: 0x" + MaterialOffset.ToString("X8"));

            // parte do header referente ao "unknown4"

            txt2.WriteLine("unknown4 Parts:");

            //padding
            //offset 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 and 0x17 
            byte[] Pad8Bytes = br.ReadBytes(0x8);
            bin.Pad8Bytes = Pad8Bytes;
            txt2.WriteLine("Pad8Bytes: " + BitConverter.ToString(Pad8Bytes) + "  {hex}");

            //unknown4_B
            //offset 0x18, 0x19, 0x1A and 0x1B
            byte[] unknown4_B = br.ReadBytes(0x4);
            bin.unknown4_B = unknown4_B;
            txt2.WriteLine("unknown4_B: " + BitConverter.ToString(unknown4_B) + "  {hex}");

            //bonepair_addr
            //offset 0x1F, 0x1E, 0x1D and 0x1C
            uint bonepair_addr_ = br.ReadUInt32();
            bin.bonepairPoint = bonepair_addr_;
            txt2.WriteLine("bonepair_addr_: 0x" + bonepair_addr_.ToString("X8"));

            //unknown4_unk008
            //offset 0x20, 0x21, 0x22 and 0x23
            byte[] unknown4_unk008 = br.ReadBytes(0x4);
            bin.unknown4_unk008 = unknown4_unk008;
            txt2.WriteLine("unknown4_unk008: " + BitConverter.ToString(unknown4_unk008) + "  {hex}");

            //unknown4_unk009
            //offset 0x24, 0x25, 0x26 and 0x27
            byte[] unknown4_unk009 = br.ReadBytes(0x4);
            bin.unknown4_unk009 = unknown4_unk009;
            txt2.WriteLine("unknown4_unk009: " + BitConverter.ToString(unknown4_unk009) + "  {hex}");

            //boundbox_addr_
            //offset 0x2B, 0x2A, 0x29 and 0x28
            uint boundbox_addr_ = br.ReadUInt32();
            bin.boundboxPoint = boundbox_addr_;
            txt2.WriteLine("boundbox_addr_: 0x" + boundbox_addr_.ToString("X8"));

            //unknown4_unk010
            //offset 0x2C, 0x2D, 0x2E and 0x2F
            byte[] unknown4_unk010 = br.ReadBytes(0x4);
            bin.unknown4_unk010 = unknown4_unk010;
            txt2.WriteLine("unknown4_unk010: " + BitConverter.ToString(unknown4_unk010) + "  {hex}");

            txt2.WriteLine("unk012: XYZ padding");

            //unk012_floatX // DrawDistanceNegativeX
            //offset 0x33, 0x32, 0x31 and 0x30
            float DrawDistanceNegativeX = br.ReadSingle();
            bin.DrawDistanceNegativeX = DrawDistanceNegativeX;
            txt2.WriteLine("DrawDistanceNegativeX: " + DrawDistanceNegativeX.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //unk012_floatY // DrawDistanceNegativeY
            //offset 0x37, 0x36, 0x35 and 0x34
            float DrawDistanceNegativeY = br.ReadSingle();
            bin.DrawDistanceNegativeY = DrawDistanceNegativeY;
            txt2.WriteLine("DrawDistanceNegativeY: " + DrawDistanceNegativeY.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //unk012_floatZ // DrawDistanceNegativeZ
            //offset 0x3B, 0x3A, 0x39 and 0x38
            float DrawDistanceNegativeZ = br.ReadSingle();
            bin.DrawDistanceNegativeZ = DrawDistanceNegativeZ;
            txt2.WriteLine("DrawDistanceNegativeZ: " + DrawDistanceNegativeZ.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //unk012_floatW // DrawDistanceNegativePadding
            //offset 0x3F, 0x3E, 0x3D and 0x3C
            float DrawDistanceNegativePadding = br.ReadSingle();
            bin.DrawDistanceNegativePadding = DrawDistanceNegativePadding;
            txt2.WriteLine("DrawDistanceNegativePadding: " + DrawDistanceNegativePadding.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            txt2.WriteLine("unk013: XYZ");

            //unk013_floatX // DrawDistancePositiveX
            //offset 0x43, 0x42, 0x41 and 0x40
            float DrawDistancePositiveX = br.ReadSingle();
            bin.DrawDistancePositiveX = DrawDistancePositiveX;
            txt2.WriteLine("DrawDistancePositiveX: " + DrawDistancePositiveX.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //unk013_floatY // DrawDistancePositiveY
            //offset 0x47, 0x46, 0x45 and 0x44
            float DrawDistancePositiveY = br.ReadSingle();
            bin.DrawDistancePositiveY = DrawDistancePositiveY;
            txt2.WriteLine("DrawDistancePositiveY: " + DrawDistancePositiveY.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //unk013_floatZ // DrawDistancePositiveZ
            //offset 0x4B, 0x4A, 0x49 and 0x48
            float DrawDistancePositiveZ = br.ReadSingle();
            bin.DrawDistancePositiveZ = DrawDistancePositiveZ;
            txt2.WriteLine("DrawDistancePositiveZ: " + DrawDistancePositiveZ.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //padding
            //offset 0x4C, 0x4D, 0x4E and 0x4F
            byte[] Pad4Bytes = br.ReadBytes(0x4);
            bin.Pad4Bytes = Pad4Bytes;
            txt2.WriteLine("Pad4Bytes: " + BitConverter.ToString(Pad4Bytes) + "  {hex}");

            // fim da parte referente a "unknown4"
            // fim do primeiro header

            if (BitConverter.ToString(Pad8Bytes) != "CD-CD-CD-CD-CD-CD-CD-CD")
            {
                txt2.WriteLine("Invalid bin file, not supported!");
                txt2.Close();
                br.Close();
                throw new ArgumentException("Invalid bin file, not supported!");
            }

            //---------------------------------------------

            //bonepair_addr_
            if (bonepair_addr_ != 0x0)
            {
                br.BaseStream.Position = bonepair_addr_;
                txt2.WriteLine("");
                txt2.WriteLine("bonepair_addr_: 0x" + bonepair_addr_.ToString("X8"));

                uint bonepairCount = br.ReadUInt32();
                bin.bonepairCount = bonepairCount;
                txt2.WriteLine("bonepairCount: " + bonepairCount);

                List<byte[]> bonepairLines = new List<byte[]>();
                for (int i = 0; i < bonepairCount; i++)
                {
                    byte[] bonepairLine = br.ReadBytes(0x8);
                    txt2.WriteLine("[" + i.ToString("X2") + "]: " + BitConverter.ToString(bonepairLine));
                    bonepairLines.Add(bonepairLine);
                }
                bin.bonepairLines = bonepairLines.ToArray();
            }

            //---------------------------------------------


            //bonesPoint
            br.BaseStream.Position = bonesPoint;
            txt2.WriteLine("");
            txt2.WriteLine("bones:   (in hexadecimal)");


            Bone[] bones = new Bone[BonesCount];
            for (int i = 0; i < BonesCount; i++)
            {
                Bone bone = new Bone();

                byte[] boneLine = br.ReadBytes(16);
                txt2.WriteLine("[" + i.ToString("X2") + "]: " + BitConverter.ToString(boneLine));

                bone.boneLine = boneLine;
                bones[i] = bone;
            }
            bin.bones = bones;


            //---------------------------------------------

            //MaterialOffset
            br.BaseStream.Position = MaterialOffset;

            txt2.WriteLine("");
            txt2.WriteLine("MaterialList:   (in hexadecimal)");
            

            uint[] NodesTablePointers = new uint[MaterialCount];

            Material[] materials = new Material[MaterialCount];
            for (int i = 0; i < MaterialCount; i++)
            {
                Material material = new Material();

                byte[] materialLine = br.ReadBytes(16);
                //point to Nodes table
                uint nodeTablePoint = BitConverter.ToUInt32(materialLine, 12);
                NodesTablePointers[i] = nodeTablePoint;

                txt2.WriteLine("[" + i + "]: " + BitConverter.ToString(materialLine) + "     NodeTablePoint: 0x" + nodeTablePoint.ToString("X8"));

                material.materialLine = materialLine;
                material.nodeTablePoint = nodeTablePoint;
                materials[i] = material;
            }
            bin.materials = materials;

            //---------------------------------------------

            txt2.WriteLine("");
            txt2.WriteLine("---------------");

            Node[] nodes = new Node[NodesTablePointers.Length];

            for (int t = 0; t < NodesTablePointers.Length; t++) // t == Node_ID
            {
                Node node = new Node();
                
                txt2.WriteLine("");

                br.BaseStream.Position = NodesTablePointers[t];

                txt2.WriteLine("NodeTablePointer: 0x" + NodesTablePointers[t].ToString("X8"));

                //IDXBINtext
                List<byte> nodeHeader = new List<byte>();

                //NodeHeader
                //tamanho total dinâmico

                //ushot quantidade total de bytes no NodeHeader mais o primeiro segmento
                ushort TotalBytesAmount = br.ReadUInt16();
                nodeHeader.AddRange(BitConverter.GetBytes(TotalBytesAmount));
              
                //quantidade total de segmentos alem do primeiro
                byte segmentAmountWithoutFirst = br.ReadByte();
                int TotalNumberOfSegments = (int)(segmentAmountWithoutFirst) + 1;
                nodeHeader.Add(segmentAmountWithoutFirst);
              
                //quantidade de bones usados no node
                byte BonesIdAmount = br.ReadByte();
                nodeHeader.Add(BonesIdAmount);
               

                if (BonesIdAmount == 0)
                {
                    binType = BinType.ScenarioWithColors;
                }


                int calculation = 4 + BonesIdAmount;
                int parts = calculation / 16;
                int div = calculation % 16;
                if (div != 0)
                {
                    parts++;
                }
                int BoneListAmount = (parts * 16) - 4;

                byte[] NodeBoneList = br.ReadBytes(BoneListAmount);
                nodeHeader.AddRange(NodeBoneList);
            
                node.NodeHeaderArray = nodeHeader.ToArray();

                txt2.WriteLine("NodeHeaderArray: " + BitConverter.ToString(nodeHeader.ToArray()));
                txt2.WriteLine("TotalBytesAmount: 0x" + TotalBytesAmount.ToString("X4"));
                txt2.WriteLine("segmentAmountWithoutFirst: " + segmentAmountWithoutFirst);
                txt2.WriteLine("BonesIdAmount: " + BonesIdAmount);
                txt2.WriteLine("NodeBoneList: " + BitConverter.ToString(NodeBoneList.Take(BonesIdAmount).ToArray()) + "  {hex}");
                txt2.WriteLine("TotalNumberOfSegments: " + TotalNumberOfSegments);
                txt2.WriteLine("binType: " + Enum.GetName(typeof(BinType), binType));

                //
                Segment[] segments = new Segment[TotalNumberOfSegments];


                for (int i = 0; i < TotalNumberOfSegments; i++) // SubMesh //Segment
                {
                    Segment segment = new Segment();

                    txt2.WriteLine("");

                    if (binType == BinType.Default) // arquivo bin dentro de .SMD não tem essa parte
                    {
                        //WeightMap
                        txt2.WriteLine("stream.Position: 0x" + br.BaseStream.Position.ToString("X8"));

                        byte[] WeightMapHeader = br.ReadBytes(0x10);

                        txt2.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] WeightMapHeader: " + BitConverter.ToString(WeightMapHeader));

                        segment.WeightMapHeader = WeightMapHeader;

                        // proxima listagem de WeightMapTable
                        // quantidade de bytes na listagem WeightMapTable

                        int WeightMapTableBytesAmount = WeightMapHeader[0x0] * 0x10; // valor adquirido em 0x00 ou 0x0e
  

                        // tamanho variavel
                       
                        byte[] WeightMapArray = br.ReadBytes(WeightMapTableBytesAmount);


                        int WeightMapTableLinesAmount = WeightMapTableBytesAmount / 32;
                        txt2.WriteLine("WeightMapTableBytesAmount: 0x" + WeightMapTableBytesAmount.ToString("X4"));
                        txt2.WriteLine("WeightMapTableLinesAmount: " + WeightMapTableLinesAmount.ToString());

                        txt2.WriteLine("");
                        int temp = 0;

                        WeightMapTableLine[] WeightMapTableLines = new WeightMapTableLine[WeightMapTableLinesAmount];

                        for (int a = 0; a < WeightMapTableLinesAmount; a++)
                        {
                            byte[] arr = WeightMapArray.Skip(temp).Take(32).ToArray();

                            WeightMapTableLine line = new WeightMapTableLine();
                            line.weightMapTableLine = arr;
                            WeightMapTableLines[a] = line;

                            temp += 32;
                            txt2.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + a.ToString("D2") + "]: " + BitConverter.ToString(arr));
                        }

                        segment.WeightMapTableLines = WeightMapTableLines;

                        txt2.WriteLine("");
                    }


                    txt2.WriteLine("stream.Position: 0x" + br.BaseStream.Position.ToString("X8"));
                    //TopTagVifHeader // header que define a quantidade de dados do grupo de vertices abaixo.
                    // fixo 0x30
                    byte[] TopTagVifHeader = br.ReadBytes(0x30);
                    txt2.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] TopTagVifHeader: " + BitConverter.ToString(TopTagVifHeader));

                    float ConversionFactorValue = BitConverter.ToSingle(TopTagVifHeader, 0x1C);

                    segment.TopTagVifHeader = TopTagVifHeader;
                    segment.ConversionFactorValue = ConversionFactorValue;


                    int chunkByteAmount = TopTagVifHeader[0x20] * 0x10;
                    txt2.WriteLine("chunkByteAmount: 0x" + chunkByteAmount.ToString("X4"));

                    uint Line_Offset_Calculation = (uint)br.BaseStream.Position;

                    byte[] subMeshChunk = br.ReadBytes(chunkByteAmount);


                    int LineAmount = chunkByteAmount / 24; // nota: tem um valor em "TopTagVifHeader" que representa a guantidade de LineAmount //TopTagVifHeader[0x10]

                    txt2.WriteLine("LineAmount in decimal: " + LineAmount);
                    txt2.WriteLine("ConversionFactorValue: " + ConversionFactorValue.ToString(System.Globalization.CultureInfo.InvariantCulture));


                    VertexLine[] array = new VertexLine[LineAmount];

                    txt2.WriteLine("");
                    txt2.WriteLine("subMeshChunk:");

                    if (binType == BinType.ScenarioWithColors)
                    {
                        txt2.WriteLine("Offset in hexadecimal: [MaterialCount][valorTotalDePartes][linha]: VerticeX, VerticeY, VerticeZ, IndexMount, TextureU, TextureV, UnknownA, IndexComplement, ColorR, ColorG, ColorB, UnknownB");
                    }
                    else 
                    {
                        txt2.WriteLine("Offset in hexadecimal: [MaterialCount][valorTotalDePartes][linha]: VerticeX, VerticeY, VerticeZ, UnknownB, NormalX, NormalY, NormalZ, IndexComplement, TextureU, TextureV, UnknownA, IndexMount");
                    }
                    

                    int tempOffset = 0;
                    for (int l = 0; l < LineAmount; l++)
                    {
                        // as linhas são compostas por 24 bytes
                        byte[] line = subMeshChunk.Skip(tempOffset).Take(24).ToArray();
                        VertexLine v = new VertexLine();
                        v.line = line;

                        if (binType == BinType.ScenarioWithColors)
                        {
                            v.VerticeX = BitConverter.ToInt16(line, 0);
                            v.VerticeY = BitConverter.ToInt16(line, 2);
                            v.VerticeZ = BitConverter.ToInt16(line, 4);
                            
                            v.IndexMount = BitConverter.ToUInt16(line, 6);

                            v.TextureU = BitConverter.ToInt16(line, 8);
                            v.TextureV = BitConverter.ToInt16(line, 10);

                            v.UnknownA = BitConverter.ToUInt16(line, 12);
                            v.IndexComplement = BitConverter.ToUInt16(line, 14);

                            v.NormalX = BitConverter.ToInt16(line, 16);
                            v.NormalY = BitConverter.ToInt16(line, 18);
                            v.NormalZ = BitConverter.ToInt16(line, 20);

                            v.UnknownB = BitConverter.ToUInt16(line, 22);

                            txt2.WriteLine(Line_Offset_Calculation.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + l.ToString("D2") + "]:" +
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
                            v.VerticeX = BitConverter.ToInt16(line, 0);
                            v.VerticeY = BitConverter.ToInt16(line, 2);
                            v.VerticeZ = BitConverter.ToInt16(line, 4);
                            
                            v.UnknownB = BitConverter.ToUInt16(line, 6);

                            v.NormalX = BitConverter.ToInt16(line, 8);
                            v.NormalY = BitConverter.ToInt16(line, 10);
                            v.NormalZ = BitConverter.ToInt16(line, 12);
                            
                            v.IndexComplement = BitConverter.ToUInt16(line, 14);

                            v.TextureU = BitConverter.ToInt16(line, 16);
                            v.TextureV = BitConverter.ToInt16(line, 18);

                            v.UnknownA = BitConverter.ToUInt16(line, 20);
                            v.IndexMount = BitConverter.ToUInt16(line, 22);

                            txt2.WriteLine(Line_Offset_Calculation.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + l.ToString("D2") + "]:" +
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

                        Line_Offset_Calculation += 24;
                        tempOffset += 24;
                        array[l] = v;
                    }

                    segment.vertexLines = array;

                    if (i > 0) // no final do primeiro não tem
                    {
                        // fixo 0x10
                        // EndCommand bytes = 00-00-00-XX-00-00-00-00-00-00-00-00-00-00-00-17
                        txt2.WriteLine("stream.Position: 0x" + br.BaseStream.Position.ToString("X8"));
                        byte[] EndTagVifCommand = br.ReadBytes(0x10);
                        txt2.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] EndTagVifCommand: " + BitConverter.ToString(EndTagVifCommand));
                        txt2.WriteLine("");
                    }

                    segments[i] = segment;
                }

                node.Segments = segments;
                nodes[t] = node;
            }

            bin.Nodes = nodes;
            bin.binType = binType;

            // end file
            endOffset = br.BaseStream.Position;

            return bin;
        }


    }

}