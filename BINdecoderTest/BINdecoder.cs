using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BINdecoderTest
{
    /*
    Codigo feito por JADERLINK
    Pesquisas feitas por HardHain e JaderLink.
    https://www.youtube.com/@JADERLINK
    https://www.youtube.com/@HardRainModder

    Em desenvolvimento
    Para Pesquisas
    08-09-2023
    version: beta.1.3.0.0
    */
    public static class BINdecoder
    {
        public const float GLOBAL_SCALE = 100f;
        public const string VERSION = "B.1.3.0.0";

        public static BIN Decode(Stream stream, string filePatchForTxt2, bool createTxt2 = false)
        {
            BinaryReader br = new BinaryReader(stream);

            BIN bin = new BIN();
            BinType binType = BinType.Default;

            AltTextWriter text = new AltTextWriter(filePatchForTxt2 + ".txt2", createTxt2);

            text.WriteLine("##BINdecoderTest##");
            text.WriteLine($"##Version {VERSION}##");
            text.WriteLine("");

         
            //fix0x3000 //unk001 //unknown0
            //offset 0x00 and 0x01
            byte[] unknown0 = br.ReadBytes(0x2);
            bin.unknown0 = unknown0;
            text.WriteLine("unknown0: " + BitConverter.ToString(unknown0) + "  {hex}");


            //unknown1 //unk002
            //offset 0x02 and 0x03
            byte[] unknown1 = br.ReadBytes(0x2);
            bin.unknown1 = unknown1;
            text.WriteLine("unknown1: " + BitConverter.ToString(unknown1) + "  {hex}");


            //bones point //bone_addr
            //offset 0x07, 0x06, 0x05 and 0x04
            uint bonesPoint = br.ReadUInt32();
            bin.bonesPoint = bonesPoint;
            text.WriteLine("bonesPoint: 0x" + bonesPoint.ToString("X8"));


            //unknown2 //unk003
            //offset 0x08
            byte unknown2 = br.ReadByte();
            bin.unknown2 = unknown2;
            text.WriteLine("unknown2: 0x" + unknown2.ToString("X2"));


            //BonesCount //bone_count
            //offset 0x09
            byte BonesCount = br.ReadByte();
            bin.BonesCount = BonesCount;
            text.WriteLine("BonesCount: " + BonesCount.ToString());


            //MaterialCount //table1_count
            //offset 0x0A
            byte MaterialCount = br.ReadByte();
            bin.MaterialCount = MaterialCount;
            text.WriteLine("MaterialCount: " + MaterialCount.ToString());


            //unknown3
            //offset 0x0B
            byte unknown3 = br.ReadByte();
            bin.unknown3 = unknown3;
            text.WriteLine("unknown3: 0x" + unknown3.ToString("X2"));


            //MaterialsPoint //table1_addr
            //offset 0x0F, 0x0E, 0x0D and 0x0C 
            uint materialsPoint = br.ReadUInt32();
            bin.materialsPoint = materialsPoint;
            text.WriteLine("MaterialsPoint: 0x" + materialsPoint.ToString("X8"));

            // parte do header referente ao "unknown4"

            text.WriteLine("unknown4 Parts:");

            //padding
            //offset 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 and 0x17 
            byte[] Pad8Bytes = br.ReadBytes(0x8);
            bin.Pad8Bytes = Pad8Bytes;
            text.WriteLine("Pad8Bytes: " + BitConverter.ToString(Pad8Bytes) + "  {hex}");

            //unknown4_B
            //offset 0x18, 0x19, 0x1A and 0x1B
            byte[] unknown4_B = br.ReadBytes(0x4);
            bin.unknown4_B = unknown4_B;
            text.WriteLine("unknown4_B: " + BitConverter.ToString(unknown4_B) + "  {hex}");

            //bonepair_addr
            //offset 0x1F, 0x1E, 0x1D and 0x1C
            uint bonepair_addr_ = br.ReadUInt32();
            bin.bonepairPoint = bonepair_addr_;
            text.WriteLine("bonepair_addr_: 0x" + bonepair_addr_.ToString("X8"));

            //unknown4_unk008
            //offset 0x20, 0x21, 0x22 and 0x23
            byte[] unknown4_unk008 = br.ReadBytes(0x4);
            bin.unknown4_unk008 = unknown4_unk008;
            text.WriteLine("unknown4_unk008: " + BitConverter.ToString(unknown4_unk008) + "  {hex}");

            //unknown4_unk009
            //offset 0x24, 0x25, 0x26 and 0x27
            byte[] unknown4_unk009 = br.ReadBytes(0x4);
            bin.unknown4_unk009 = unknown4_unk009;
            text.WriteLine("unknown4_unk009: " + BitConverter.ToString(unknown4_unk009) + "  {hex}");

            //boundbox_addr_
            //offset 0x2B, 0x2A, 0x29 and 0x28
            uint boundbox_addr_ = br.ReadUInt32();
            bin.boundboxPoint = boundbox_addr_;
            text.WriteLine("boundbox_addr_: 0x" + boundbox_addr_.ToString("X8"));

            //unknown4_unk010
            //offset 0x2C, 0x2D, 0x2E and 0x2F
            byte[] unknown4_unk010 = br.ReadBytes(0x4);
            bin.unknown4_unk010 = unknown4_unk010;
            text.WriteLine("unknown4_unk010: " + BitConverter.ToString(unknown4_unk010) + "  {hex}");

            text.WriteLine("unk012: XYZ padding");

            //unk012_floatX // DrawDistanceNegativeX
            //offset 0x33, 0x32, 0x31 and 0x30
            float DrawDistanceNegativeX = br.ReadSingle();
            bin.DrawDistanceNegativeX = DrawDistanceNegativeX;
            text.WriteLine("DrawDistanceNegativeX: " + DrawDistanceNegativeX.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //unk012_floatY // DrawDistanceNegativeY
            //offset 0x37, 0x36, 0x35 and 0x34
            float DrawDistanceNegativeY = br.ReadSingle();
            bin.DrawDistanceNegativeY = DrawDistanceNegativeY;
            text.WriteLine("DrawDistanceNegativeY: " + DrawDistanceNegativeY.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //unk012_floatZ // DrawDistanceNegativeZ
            //offset 0x3B, 0x3A, 0x39 and 0x38
            float DrawDistanceNegativeZ = br.ReadSingle();
            bin.DrawDistanceNegativeZ = DrawDistanceNegativeZ;
            text.WriteLine("DrawDistanceNegativeZ: " + DrawDistanceNegativeZ.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //unk012_floatW // DrawDistanceNegativePadding
            //offset 0x3F, 0x3E, 0x3D and 0x3C
            float DrawDistanceNegativePadding = br.ReadSingle();
            bin.DrawDistanceNegativePadding = DrawDistanceNegativePadding;
            text.WriteLine("DrawDistanceNegativePadding: " + DrawDistanceNegativePadding.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            text.WriteLine("unk013: XYZ");

            //unk013_floatX // DrawDistancePositiveX
            //offset 0x43, 0x42, 0x41 and 0x40
            float DrawDistancePositiveX = br.ReadSingle();
            bin.DrawDistancePositiveX = DrawDistancePositiveX;
            text.WriteLine("DrawDistancePositiveX: " + DrawDistancePositiveX.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //unk013_floatY // DrawDistancePositiveY
            //offset 0x47, 0x46, 0x45 and 0x44
            float DrawDistancePositiveY = br.ReadSingle();
            bin.DrawDistancePositiveY = DrawDistancePositiveY;
            text.WriteLine("DrawDistancePositiveY: " + DrawDistancePositiveY.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //unk013_floatZ // DrawDistancePositiveZ
            //offset 0x4B, 0x4A, 0x49 and 0x48
            float DrawDistancePositiveZ = br.ReadSingle();
            bin.DrawDistancePositiveZ = DrawDistancePositiveZ;
            text.WriteLine("DrawDistancePositiveZ: " + DrawDistancePositiveZ.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

            //padding
            //offset 0x4C, 0x4D, 0x4E and 0x4F
            byte[] Pad4Bytes = br.ReadBytes(0x4);
            bin.Pad4Bytes = Pad4Bytes;
            text.WriteLine("Pad4Bytes: " + BitConverter.ToString(Pad4Bytes) + "  {hex}");

            // fim da parte referente a "unknown4"
            // fim do primeiro header

            if (BitConverter.ToString(Pad8Bytes) != "CD-CD-CD-CD-CD-CD-CD-CD")
            {
                text.WriteLine("Invalid bin file, not supported!");
                text.Close();
                br.Close();
                throw new ArgumentException("Invalid bin file, not supported!");
            }

            //---------------------------------------------

            //bonepair_addr_
            if (bonepair_addr_ != 0x0)
            {
                br.BaseStream.Position = bonepair_addr_;
                text.WriteLine("");
                text.WriteLine("bonepair_addr_: 0x" + bonepair_addr_.ToString("X8"));

                uint bonepairCount = br.ReadUInt32();
                bin.bonepairCount = bonepairCount;
                text.WriteLine("bonepairCount: " + bonepairCount);

                List<byte[]> bonepairLines = new List<byte[]>();
                for (int i = 0; i < bonepairCount; i++)
                {
                    byte[] bonepairLine = br.ReadBytes(0x8);
                    text.WriteLine("[" + i.ToString("X2") + "]: " + BitConverter.ToString(bonepairLine));
                    bonepairLines.Add(bonepairLine);
                }
                bin.bonepairLines = bonepairLines.ToArray();
            }

            //---------------------------------------------


            //bonesPoint
            br.BaseStream.Position = bonesPoint;
            text.WriteLine("");
            text.WriteLine("bones:   (in hexadecimal)");


            Bone[] bones = new Bone[BonesCount];
            for (int i = 0; i < BonesCount; i++)
            {
                Bone bone = new Bone();

                byte[] boneLine = br.ReadBytes(16);
                text.WriteLine("[" + i.ToString("X2") + "]: " + BitConverter.ToString(boneLine));

                bone.boneLine = boneLine;
                bones[i] = bone;
            }
            bin.bones = bones;


            //---------------------------------------------

            //materialsPoint
            br.BaseStream.Position = materialsPoint;

            text.WriteLine("");
            text.WriteLine("MaterialList:   (in hexadecimal)");
            

            uint[] NodesTablePointers = new uint[MaterialCount];

            Material[] materials = new Material[MaterialCount];
            for (int i = 0; i < MaterialCount; i++)
            {
                Material material = new Material();

                byte[] materialLine = br.ReadBytes(16);
                //point to Nodes table
                uint nodeTablePoint = BitConverter.ToUInt32(materialLine, 12);
                NodesTablePointers[i] = nodeTablePoint;

                text.WriteLine("[" + i + "]: " + BitConverter.ToString(materialLine) + "     NodeTablePoint: 0x" + nodeTablePoint.ToString("X8"));

                material.materialLine = materialLine;
                material.nodeTablePoint = nodeTablePoint;
                materials[i] = material;
            }
            bin.materials = materials;

            //---------------------------------------------

            text.WriteLine("");
            text.WriteLine("---------------");


            Node[] nodes = new Node[NodesTablePointers.Length];

            int Index_Count_For_OBJ = 1;

            for (int t = 0; t < NodesTablePointers.Length; t++) // t == Node_ID
            {
                Node node = new Node();
                
                text.WriteLine("");

                br.BaseStream.Position = NodesTablePointers[t];

                text.WriteLine("NodeTablePointer: 0x" + NodesTablePointers[t].ToString("X8"));

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


                text.WriteLine("NodeHeaderArray: " + BitConverter.ToString(nodeHeader.ToArray()));
                text.WriteLine("TotalBytesAmount: 0x" + TotalBytesAmount.ToString("X4"));
                text.WriteLine("segmentAmountWithoutFirst: " + segmentAmountWithoutFirst);
                text.WriteLine("BonesIdAmount: " + BonesIdAmount);
                text.WriteLine("NodeBoneList: " + BitConverter.ToString(NodeBoneList.Take(BonesIdAmount).ToArray()) + "  {hex}");
                text.WriteLine("TotalNumberOfSegments: " + TotalNumberOfSegments);
                text.WriteLine("binType: " + Enum.GetName(typeof(BinType), binType));

                //
                Segment[] segments = new Segment[TotalNumberOfSegments];


                for (int i = 0; i < TotalNumberOfSegments; i++) // SubMesh //Segment
                {
                    Segment segment = new Segment();

                    text.WriteLine("");

                    if (binType == BinType.Default) // arquivo bin dentro de .SMD não tem essa parte
                    {
                        //WeightMap
                        text.WriteLine("stream.Position: 0x" + br.BaseStream.Position.ToString("X8"));

                        byte[] WeightMapHeader = br.ReadBytes(0x10);

                        text.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] WeightMapHeader: " + BitConverter.ToString(WeightMapHeader));

                        segment.WeightMapHeader = WeightMapHeader;

                        // proxima listagem de WeightMapTable
                        // quantidade de bytes na listagem WeightMapTable

                        int WeightMapTableBytesAmount = WeightMapHeader[0x0] * 0x10; // valor adquirido em 0x00 ou 0x0e
  

                        // tamanho variavel
                       
                        byte[] WeightMapArray = br.ReadBytes(WeightMapTableBytesAmount);


                        int WeightMapTableLinesAmount = WeightMapTableBytesAmount / 32;
                        text.WriteLine("WeightMapTableBytesAmount: 0x" + WeightMapTableBytesAmount.ToString("X4"));
                        text.WriteLine("WeightMapTableLinesAmount: " + WeightMapTableLinesAmount.ToString());

                        text.WriteLine("");
                        int temp = 0;

                        WeightMapTableLine[] WeightMapTableLines = new WeightMapTableLine[WeightMapTableLinesAmount];

                        for (int a = 0; a < WeightMapTableLinesAmount; a++)
                        {
                            byte[] arr = WeightMapArray.Skip(temp).Take(32).ToArray();

                            WeightMapTableLine line = new WeightMapTableLine();
                            line.weightMapTableLine = arr;
                            WeightMapTableLines[a] = line;

                            temp += 32;
                            text.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + a.ToString("D2") + "]: " + BitConverter.ToString(arr));
                        }

                        segment.WeightMapTableLines = WeightMapTableLines;

                        text.WriteLine("");
                    }


                    text.WriteLine("stream.Position: 0x" + br.BaseStream.Position.ToString("X8"));
                    //TopTagVifHeader // header que define a quantidade de dados do grupo de vertices abaixo.
                    // fixo 0x30
                    byte[] TopTagVifHeader = br.ReadBytes(0x30);
                    text.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] TopTagVifHeader: " + BitConverter.ToString(TopTagVifHeader));

                    float ConversionFactorValue = BitConverter.ToSingle(TopTagVifHeader, 0x1C);

                    segment.TopTagVifHeader = TopTagVifHeader;
                    segment.ConversionFactorValue = ConversionFactorValue;


                    int chunkByteAmount = TopTagVifHeader[0x20] * 0x10;
                    text.WriteLine("chunkByteAmount: 0x" + chunkByteAmount.ToString("X4"));

                    uint Line_Offset_Calculation = (uint)br.BaseStream.Position;

                    byte[] subMeshChunk = br.ReadBytes(chunkByteAmount);


                    int LineAmount = chunkByteAmount / 24; // nota: tem um valor em "TopTagVifHeader" que representa a guantidade de LineAmount //TopTagVifHeader[0x10]

                    text.WriteLine("LineAmount in decimal: " + LineAmount);
                    text.WriteLine("ConversionFactorValue: " + ConversionFactorValue.ToString(System.Globalization.CultureInfo.InvariantCulture));


                    VertexLine[] array = new VertexLine[LineAmount];

                    text.WriteLine("");
                    text.WriteLine("subMeshChunk:");

                    if (binType == BinType.ScenarioWithColors)
                    {
                        text.WriteLine("Offset in hexadecimal: [MaterialCount][valorTotalDePartes][linha]: VerticeX, VerticeY, VerticeZ, IndexMount, TextureU, TextureV, UnknownA, IndexComplement, ColorR, ColorG, ColorB, UnknownB");
                    }
                    else 
                    {
                        text.WriteLine("Offset in hexadecimal: [MaterialCount][valorTotalDePartes][linha]: VerticeX, VerticeY, VerticeZ, UnknownB, NormalX, NormalY, NormalZ, IndexComplement, TextureU, TextureV, UnknownA, IndexMount");
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

                            text.WriteLine(Line_Offset_Calculation.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + l.ToString("D2") + "]:" +
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

                            text.WriteLine(Line_Offset_Calculation.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + l.ToString("D2") + "]:" +
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

                        Index_Count_For_OBJ++;
                        Line_Offset_Calculation += 24;
                        tempOffset += 24;
                        array[l] = v;
                    }

                    segment.vertexLines = array;

                    if (i > 0) // no final do primeiro não tem
                    {
                        // fixo 0x10
                        // EndCommand bytes = 00-00-00-XX-00-00-00-00-00-00-00-00-00-00-00-17
                        text.WriteLine("stream.Position: 0x" + br.BaseStream.Position.ToString("X8"));
                        byte[] EndTagVifCommand = br.ReadBytes(0x10);
                        text.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] EndTagVifCommand: " + BitConverter.ToString(EndTagVifCommand));
                        text.WriteLine("");
                    }

                    segments[i] = segment;
                }

                node.Segments = segments;
                nodes[t] = node;
            }

            bin.Nodes = nodes;

            bin.binType = binType;


            // end file

            if (br.BaseStream.Position < br.BaseStream.Length) // no caso, não deve ter mais bytes
            {
                text.WriteLine("");
                text.WriteLine("stream.Position: 0x" + br.BaseStream.Position.ToString("X8"));
                text.WriteLine("Bytes at end of file");
                int lenght = (int)(br.BaseStream.Length - br.BaseStream.Position);
                byte[] end = new byte[lenght];
                br.BaseStream.Read(end, 0, lenght);
                text.WriteLine("EndFile: " + BitConverter.ToString(end));
            }

            text.Close();
            br.Close();

            return bin;
        }

        //Studiomdl Data
        public static void CreateSMD(BIN bin, string baseDiretory, string baseFileName, string baseTextureName)
        {
            if (baseDiretory[baseDiretory.Length - 1] != '\\')
            {
                baseDiretory += "\\";
            }

            TextWriter text = new FileInfo(baseDiretory + baseFileName + ".smd").CreateText();
            text.WriteLine("version 1");
            text.WriteLine("nodes");

            for (int i = 0; i < bin.bones.Length; i++)
            {
                text.WriteLine(bin.bones[i].BoneID + " \"BONE_" + i.ToString("D3")  + "\" " + bin.bones[i].BoneParent);
            }

            text.WriteLine("end");

            text.WriteLine("skeleton");
            text.WriteLine("time 0");

            for (int i = 0; i < bin.bones.Length; i++)
            {
                text.WriteLine(bin.bones[i].BoneID  + "  " + 
                    (bin.bones[i].PositionX / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                    (bin.bones[i].PositionZ * -1 / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " + 
                    (bin.bones[i].PositionY / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + "  0.000000 -0.000000 0.000000");
            }

            text.WriteLine("end");

            text.WriteLine("triangles");

            for (int t = 0; t < bin.Nodes.Length; t++)
            {
                int QuantidadeTotalDeproximosBytes = bin.Nodes[t].NodeHeaderArray[0x3];
                byte[] useBoneList = bin.Nodes[t].NodeHeaderArray.Skip(4).Take(QuantidadeTotalDeproximosBytes).ToArray();


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
                                    bin.Nodes[t].Segments[i].WeightMapTableLines[l].weight1.ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
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
                                    bin.Nodes[t].Segments[i].WeightMapTableLines[l].weight2.ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
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
                                    bin.Nodes[t].Segments[i].WeightMapTableLines[l].weight3.ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            Weights.Add(res);
                        }
                    }


                    bool invFace = false;
                    int contagem = 0;
                    while (contagem < bin.Nodes[t].Segments[i].vertexLines.Length)
                    {

                        if ((contagem - 2) > -1 &&
                           (bin.Nodes[t].Segments[i].vertexLines[contagem].IndexComplement == 0)
                           )
                        {
                            text.WriteLine("MATERIAL_" + t.ToString("D3"));


                            string[] pos = new string[3];
                            string[] normals = new string[3];
                            string[] uvs = new string[3];
                            string[] VertexWeights = new string[3];


                            for (int l = 2; l > -1; l--)
                            {
                                VertexLine vertexLine = bin.Nodes[t].Segments[i].vertexLines[contagem - l];

                                pos[l] = (((float)vertexLine.VerticeX * bin.Nodes[t].Segments[i].ConversionFactorValue / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                          ((float)vertexLine.VerticeZ * -1f * bin.Nodes[t].Segments[i].ConversionFactorValue / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                          ((float)vertexLine.VerticeY * bin.Nodes[t].Segments[i].ConversionFactorValue / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

                                uvs[l] = (((float)vertexLine.TextureU / 255f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                          ((float)vertexLine.TextureV / 255f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

                                if (bin.binType != BinType.ScenarioWithColors)
                                {
                                    normals[l] = (((float)vertexLine.NormalX /  127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                                  ((float)vertexLine.NormalZ * -1f / 127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                                  ((float)vertexLine.NormalY / 127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    normals[l] = "0.00000 0.00000 0.00000";
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

                        contagem++;
                    }



                }


            }

            text.WriteLine("end");
            text.WriteLine("//##BINdecoderTest##");
            text.WriteLine($"//##Version {VERSION}##");
            text.Close();

        }

        public static void CreateObjMtl(BIN bin, string baseDiretory, string baseFileName, string baseTextureName)
        {
            if (baseDiretory[baseDiretory.Length - 1] != '\\')
            {
                baseDiretory += "\\";
            }

            TextWriter MTLtext = new FileInfo(baseDiretory + baseFileName + ".mtl").CreateText();
            MTLtext.WriteLine("##BINdecoderTest##");
            MTLtext.WriteLine($"##Version {VERSION}##");
            MTLtext.WriteLine("");

            for (int i = 0; i < bin.materials.Length; i++)
            {
                MTLtext.WriteLine("");
                MTLtext.WriteLine("newmtl MATERIAL_" + i.ToString("D3"));
                MTLtext.WriteLine("Ka 1.000 1.000 1.000");
                MTLtext.WriteLine("Kd 1.000 1.000 1.000");
                MTLtext.WriteLine("Ks 0.000 0.000 0.000");
                MTLtext.WriteLine("Ns 0");
                MTLtext.WriteLine("d 1");
                MTLtext.WriteLine("Tr 1");
                MTLtext.WriteLine("map_Kd Textures/" + baseTextureName + "_" + bin.materials[i].materialLine[1].ToString() + ".tga");
                MTLtext.WriteLine("");
            }
            MTLtext.Close();

            //------

            TextWriter text = new FileInfo(baseDiretory + baseFileName + ".obj").CreateText();
            text.WriteLine("##BINdecoderTest##");
            text.WriteLine($"##Version {VERSION}##");
            text.WriteLine("mtllib " + baseFileName + ".mtl");

            int indexGeral = 1;

            for (int t = 0; t < bin.Nodes.Length; t++)
            {

                //usemtl Material*
                text.WriteLine("g MATERIAL_" + t.ToString("D3"));
                text.WriteLine("usemtl MATERIAL_" + t.ToString("D3"));

                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    for (int l = 0; l < bin.Nodes[t].Segments[i].vertexLines.Length; l++)
                    {
                        VertexLine vertexLine = bin.Nodes[t].Segments[i].vertexLines[l];

                        text.WriteLine("v " + ((float)vertexLine.VerticeX * bin.Nodes[t].Segments[i].ConversionFactorValue / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             ((float)vertexLine.VerticeY * bin.Nodes[t].Segments[i].ConversionFactorValue / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             ((float)vertexLine.VerticeZ * bin.Nodes[t].Segments[i].ConversionFactorValue / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

                        text.WriteLine("vt " + ((float)vertexLine.TextureU / 255f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                        ((float)vertexLine.TextureV / 255f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

                        if (bin.binType != BinType.ScenarioWithColors)
                        {
                            text.WriteLine("vn " + ((float)vertexLine.NormalX / 127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                 ((float)vertexLine.NormalY / 127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                                 ((float)vertexLine.NormalZ / 127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                        }
                    }


                    bool invFace = false;
                    int contagem = 0;
                    while (contagem < bin.Nodes[t].Segments[i].vertexLines.Length)
                    {
                        // text.WriteLine("# [" + t.ToString("D4") + "][" + i.ToString("D4") + "][" + contagem.ToString("D4") + "][" + indexGeral.ToString("D4") + "] IndexMount: " +
                        //         bin.Nodes[t].Segments[i].vertexLines[contagem].IndexMount.ToString("X4"));

                        string a = (indexGeral - 2).ToString();
                        string b = (indexGeral - 1).ToString();
                        string c = (indexGeral).ToString();

                        /*
                        if ((contagem - 2) > -1 &&
                       !((bin.Nodes[t].Segments[i].vertexLines[contagem].IndexMount == 0 && (bin.Nodes[t].Segments[i].vertexLines[contagem - 1].IndexMount == 1 || bin.Nodes[t].Segments[i].vertexLines[contagem - 1].IndexMount == 0xFFFF))
                       || (bin.Nodes[t].Segments[i].vertexLines[contagem].IndexMount == 0 && (bin.Nodes[t].Segments[i].vertexLines[contagem - 2].IndexMount == 1 || bin.Nodes[t].Segments[i].vertexLines[contagem - 2].IndexMount == 0xFFFF)))
                        */

                        if ((contagem - 2) > -1 &&
                           (bin.Nodes[t].Segments[i].vertexLines[contagem].IndexComplement == 0)
                           )
                        {

                            //text.Add("g indices_" + a.PadLeft(3, '0') + "_" + b.PadLeft(3, '0') + "_"+ c.PadLeft(3, '0'));

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

                        contagem++;
                        indexGeral++;
                    }




                }

            }

            text.Close();

        }

        public static void CreateIdxbin(BIN bin, string baseDiretory, string baseFileName) 
        {
            if (baseDiretory[baseDiretory.Length - 1] != '\\')
            {
                baseDiretory += "\\";
            }

            TextWriter IDXBINtext = new FileInfo(baseDiretory + baseFileName + ".idxbin").CreateText();
            IDXBINtext.WriteLine(":##BINdecoderTest##");
            IDXBINtext.WriteLine($":##Version {VERSION}##");

            IDXBINtext.WriteLine();

            IDXBINtext.WriteLine("GlobalScale:" + GLOBAL_SCALE);

            //conversion factor = "fator de conversão", isso é converter de float para short

            IDXBINtext.WriteLine("CompressVertices:True");
            IDXBINtext.WriteLine("AutoConversionFactor:True"); // old AutoScale


            float tempConversionFactor = 0;
            for (int t = 0; t < bin.materials.Length; t++)
            {
                float temp = 0;
                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    temp += bin.Nodes[t].Segments[i].ConversionFactorValue;
                }
                tempConversionFactor += temp / bin.Nodes[t].Segments.Length;
            }
            float ManualConversionFactor = tempConversionFactor / bin.materials.Length;
            IDXBINtext.WriteLine("ManualConversionFactor:" + ManualConversionFactor.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)); // prencher com valor


            IDXBINtext.WriteLine("IsScenarioBin:" + (bin.binType == BinType.ScenarioWithColors));

            IDXBINtext.WriteLine();

            IDXBINtext.WriteLine(": 2 bytes in hex");
            IDXBINtext.WriteLine("fix3000:" + BitConverter.ToString(bin.unknown0).Replace("-", ""));

            IDXBINtext.WriteLine(": 2 bytes in hex");
            IDXBINtext.WriteLine("unknown1:" + BitConverter.ToString(bin.unknown1).Replace("-", ""));

            IDXBINtext.WriteLine(": 1 bytes in hex");
            IDXBINtext.WriteLine("unknown2:" + bin.unknown2.ToString("X2"));
        
            IDXBINtext.WriteLine(": 1 bytes in hex");
            IDXBINtext.WriteLine("unknown3:" + bin.unknown3.ToString("X2"));

            // unknown4
            IDXBINtext.WriteLine(": 4 bytes in hex");
            IDXBINtext.WriteLine("unknown4_B:" + BitConverter.ToString(bin.unknown4_B).Replace("-", ""));

            IDXBINtext.WriteLine(": 4 bytes in hex");
            IDXBINtext.WriteLine("unknown4_unk008:" + BitConverter.ToString(bin.unknown4_unk008).Replace("-", ""));

            IDXBINtext.WriteLine(": 4 bytes in hex");
            IDXBINtext.WriteLine("unknown4_unk009:" + BitConverter.ToString(bin.unknown4_unk009).Replace("-", ""));

            IDXBINtext.WriteLine(": 4 bytes in hex");
            IDXBINtext.WriteLine("unknown4_unk010:" + BitConverter.ToString(bin.unknown4_unk010).Replace("-", ""));

            IDXBINtext.WriteLine();
            IDXBINtext.WriteLine(": ## DrawDistance float values ##");
            IDXBINtext.WriteLine("DrawDistanceNegativeX:" + bin.DrawDistanceNegativeX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistanceNegativeY:" + bin.DrawDistanceNegativeY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistanceNegativeZ:" + bin.DrawDistanceNegativeZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistanceNegativePadding:" + bin.DrawDistanceNegativePadding.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistancePositiveX:" + bin.DrawDistancePositiveX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistancePositiveY:" + bin.DrawDistancePositiveY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistancePositiveZ:" + bin.DrawDistancePositiveZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

            //-- unknown4 end

            if (bin.bonepairCount != 0)
            {
                IDXBINtext.WriteLine();
                IDXBINtext.WriteLine(": ## bonepair ##");
                IDXBINtext.WriteLine(": bonepairCount in decimal value");
                IDXBINtext.WriteLine("bonepairCount:" + bin.bonepairCount.ToString());

                IDXBINtext.WriteLine(": bonepairLines -> 8 bytes in hex");
                for (int i = 0; i < bin.bonepairCount; i++)
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


            IDXBINtext.WriteLine();
            IDXBINtext.WriteLine(": ## MaterialLines ##");
            IDXBINtext.WriteLine(": MaterialLine?MaterialName: -> 12 bytes in hex");

            for (int t = 0; t < bin.materials.Length; t++)
            {
                // old materialLine_
                IDXBINtext.WriteLine("MaterialLine?MATERIAL_" + t.ToString("D3") + ":" + BitConverter.ToString(bin.materials[t].materialLine.Take(12).ToArray()).Replace("-", ""));
            }


            IDXBINtext.Close();
        }

        //debug
        public static void CreateDrawDistanceBoxObj(BIN bin, string baseDiretory, string baseFileName)
        {
            //

            if (baseDiretory[baseDiretory.Length - 1] != '\\')
            {
                baseDiretory += "\\";
            }

            TextWriter text = new FileInfo(baseDiretory + baseFileName + ".DrawDistanceBox.obj").CreateText();
            text.WriteLine("##BINdecoderTest##");
            text.WriteLine($"##Version {VERSION}##");

            text.WriteLine("");

            string DrawDistanceNegativeX = (bin.DrawDistanceNegativeX / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeY = (bin.DrawDistanceNegativeY / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeZ = (bin.DrawDistanceNegativeZ / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);

            string DrawDistancePositiveX = (bin.DrawDistancePositiveX / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveY = (bin.DrawDistancePositiveY / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveZ = (bin.DrawDistancePositiveZ / GLOBAL_SCALE).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);

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
        public static void CreateScaleLimitBoxObj(BIN bin, string baseDiretory, string baseFileName)
        {

            //

            if (baseDiretory[baseDiretory.Length - 1] != '\\')
            {
                baseDiretory += "\\";
            }

            TextWriter text = new FileInfo(baseDiretory + baseFileName + ".ScaleLimitBox.obj").CreateText();
            text.WriteLine("##BINdecoderTest##");
            text.WriteLine($"##Version {VERSION}##");

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

            float maxPos = scale * short.MaxValue / GLOBAL_SCALE;
            float minPos = scale * short.MinValue / GLOBAL_SCALE;

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

    #region representação do bin

    public class BIN 
    {
        //new byte[2];
        public byte[] unknown0 = null;

        //new byte[2];
        public byte[] unknown1 = null;

        //bones point //bone_addr
        public uint bonesPoint;

        //unknown2 //unk003
        public byte unknown2;

        //BonesCount //bone_count
        public byte BonesCount;

        //MaterialCount //table1_count
        public byte MaterialCount;

        //????  //pode ser parte de MaterialCount
        public byte unknown3;

        //MaterialsPoint //table1_addr
        public uint materialsPoint;

        //unknown4 start

        //new byte[8];
        public byte[] Pad8Bytes; //CD-CD-CD-CD-CD-CD-CD-CD

        //byte[4];
        public byte[] unknown4_B; //18-08-03-20 or 01-08-01-20

        //bonepair_addr_
        public uint bonepairPoint; // 0x0

        //new byte[4];
        public byte[] unknown4_unk008; // padding

        //new byte[4];
        public byte[] unknown4_unk009;

        //boundbox_addr
        public uint boundboxPoint; //fixo 0x30

        //new byte[4];
        public byte[] unknown4_unk010; // padding

        public float DrawDistanceNegativeX;
        public float DrawDistanceNegativeY;
        public float DrawDistanceNegativeZ;
        public float DrawDistanceNegativePadding;

        public float DrawDistancePositiveX;
        public float DrawDistancePositiveY;
        public float DrawDistancePositiveZ;

        //new byte[4];
        public byte[] Pad4Bytes; // CD-CD-CD-CD

        // unknown4 end

        //bonepair
        public uint bonepairCount;
        public byte[][] bonepairLines;

        //bonesList
        public Bone[] bones;

        //materialsList
        public Material[] materials;

        //NodeList
        public Node[] Nodes;

        // tipo do bin
        public BinType binType = BinType.Default;
    }

    public class Bone 
    {
        // new byte[16];
        public byte[] boneLine;

        public sbyte BoneID { get { return (sbyte)boneLine[0x0]; } }
        public sbyte BoneParent { get { return (sbyte)boneLine[0x1]; } }

        public float PositionX { get { return BitConverter.ToSingle(boneLine, 0x4); } }
        public float PositionY { get { return BitConverter.ToSingle(boneLine, 0x8); } }
        public float PositionZ { get { return BitConverter.ToSingle(boneLine, 0xC); } }

    }

    public class Material
    {
        // new byte[16];
        public byte[] materialLine;

        //
        public uint nodeTablePoint;
    }

    public class Node 
    {
        //NodeHeader
        //tamanho total dinâmico, em blocos de 0x10
        public byte[] NodeHeaderArray;

        //segmentos no node (valorTotalDePartes), tinha sido nomeado como subMesh
        public Segment[] Segments; 
    }

    public class Segment
    {
        //new byte[0x10];
        public byte[] WeightMapHeader;

        //[quantidade de linhas][0x20]
        //public byte[][] SubBoneTableLines;
        public WeightMapTableLine[] WeightMapTableLines;

        //--
        //TopTagVifHeader
        //new byte[0x30];
        public byte[] TopTagVifHeader;
        public float ConversionFactorValue;
        //--

        //vertexLines
        public VertexLine[] vertexLines;

        //EndTagVifCommand
        //new byte[0x10];
        public byte[] EndTagVifCommand;
    }

    public class WeightMapTableLine 
    {
        public byte[] weightMapTableLine;

        public uint boneId1 { get { return BitConverter.ToUInt32(weightMapTableLine, 0x0); } }
        public uint boneId2 { get { return BitConverter.ToUInt32(weightMapTableLine, 0x4); } }
        public uint boneId3 { get { return BitConverter.ToUInt32(weightMapTableLine, 0x8); } }
        public int Amount { get { return BitConverter.ToInt32(weightMapTableLine, 0xC); } }
        public float weight1 { get { return BitConverter.ToSingle(weightMapTableLine, 0x10); } }
        public float weight2 { get { return BitConverter.ToSingle(weightMapTableLine, 0x14); } }
        public float weight3 { get { return BitConverter.ToSingle(weightMapTableLine, 0x18); } }
    }


    public class VertexLine
    {
        // linha total
        public byte[] line;

        public short VerticeX = 0;
        public short VerticeY = 0;
        public short VerticeZ = 0;

        public ushort UnknownB = 0;

        public short NormalX = 0;
        public short NormalY = 0;
        public short NormalZ = 0;

        public ushort IndexComplement = 0;

        public short TextureU = 0;
        public short TextureV = 0;

        public ushort UnknownA = 0;

        public ushort IndexMount = 0;
    }

    public enum BinType
    {
        Default,
        ScenarioWithColors
    }

    #endregion

    //AltTextWriter
    public class AltTextWriter
    {
        private TextWriter text;

        public AltTextWriter(string Filepatch, bool Create) 
        {
            if (Create)
            {
                text = new FileInfo(Filepatch).CreateText();
            }

        }

        public void WriteLine(string text) 
        {
            if (this.text != null)
            {
                this.text.WriteLine(text);
            }
        }

        public void Close() 
        {
            if (this.text != null)
            {
                this.text.Close();
            }
        }
    }
}