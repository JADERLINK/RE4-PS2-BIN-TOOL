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
        public static PS2BIN Decode(Stream stream , long startOffset, out long endOffset)
        {
            BinaryReader br = new BinaryReader(stream);
            br.BaseStream.Position = startOffset;

            PS2BIN bin = new PS2BIN();
            BinType binType = BinType.Default;

            //Magic //fixed 0x3000 //unknown0
            //offset 0x00 and 0x01
            bin.Magic = br.ReadUInt16();
           
            //unknown1
            //offset 0x02 and 0x03
            bin.Unknown1 = br.ReadUInt16();

            //bones Point
            //offset 0x07, 0x06, 0x05 and 0x04
            uint bonesPoint = br.ReadUInt32();
            bin.BonesPoint = bonesPoint;
            
            //unknown2
            //offset 0x08
            byte unknown2 = br.ReadByte();
            bin.Unknown2 = unknown2;
          
            //BonesCount
            //offset 0x09
            byte BonesCount = br.ReadByte();
            bin.BonesCount = BonesCount;

            //MaterialCount //table1_count
            //offset 0x0B, 0x0A
            ushort MaterialCount = br.ReadUInt16();
            bin.MaterialCount = MaterialCount;
          
            //MaterialsPoint //table1_addr
            //offset 0x0F, 0x0E, 0x0D and 0x0C 
            uint MaterialOffset = br.ReadUInt32();
            bin.MaterialOffset = MaterialOffset;
          
            //padding
            //offset 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 and 0x17 
            bin.Padding1 = br.ReadUInt32();
            bin.Padding2 = br.ReadUInt32();
         
            //unknown4_B
            //offset 0x18, 0x19, 0x1A and 0x1B
            bin.VersionFlag = br.ReadUInt32();
           
            //bonepair_addr
            //offset 0x1F, 0x1E, 0x1D and 0x1C
            uint bonepair_addr_ = br.ReadUInt32();
            bin.BonepairPoint = bonepair_addr_;
        
            //unknown4_unk008
            //offset 0x20, 0x21, 0x22 and 0x23
            bin.Unknown408 = br.ReadUInt32();

            //unknown4_unk009
            //offset 0x24, 0x25, 0x26 and 0x27
            bin.TextureFlags = br.ReadUInt32();
        
            //boundbox_addr_
            //offset 0x2B, 0x2A, 0x29 and 0x28
            uint boundbox_addr_ = br.ReadUInt32();
            bin.BoundboxPoint = boundbox_addr_;
          
            //unknown4_unk010
            //offset 0x2C, 0x2D, 0x2E and 0x2F
            bin.Unknown410 = br.ReadUInt32();
          
            //unk012_floatX // DrawDistanceNegativeX
            //offset 0x33, 0x32, 0x31 and 0x30
            float DrawDistanceNegativeX = br.ReadSingle();
            bin.DrawDistanceNegativeX = DrawDistanceNegativeX;
         
            //unk012_floatY // DrawDistanceNegativeY
            //offset 0x37, 0x36, 0x35 and 0x34
            float DrawDistanceNegativeY = br.ReadSingle();
            bin.DrawDistanceNegativeY = DrawDistanceNegativeY;
           

            //unk012_floatZ // DrawDistanceNegativeZ
            //offset 0x3B, 0x3A, 0x39 and 0x38
            float DrawDistanceNegativeZ = br.ReadSingle();
            bin.DrawDistanceNegativeZ = DrawDistanceNegativeZ;

            //unk012_floatW // DrawDistanceNegativePadding
            //offset 0x3F, 0x3E, 0x3D and 0x3C
            float DrawDistanceNegativePadding = br.ReadSingle();
            bin.DrawDistanceNegativePadding = DrawDistanceNegativePadding;
        
            //unk013_floatX // DrawDistancePositiveX
            //offset 0x43, 0x42, 0x41 and 0x40
            float DrawDistancePositiveX = br.ReadSingle();
            bin.DrawDistancePositiveX = DrawDistancePositiveX;
       
            //unk013_floatY // DrawDistancePositiveY
            //offset 0x47, 0x46, 0x45 and 0x44
            float DrawDistancePositiveY = br.ReadSingle();
            bin.DrawDistancePositiveY = DrawDistancePositiveY;
         
            //unk013_floatZ // DrawDistancePositiveZ
            //offset 0x4B, 0x4A, 0x49 and 0x48
            float DrawDistancePositiveZ = br.ReadSingle();
            bin.DrawDistancePositiveZ = DrawDistancePositiveZ;
            
            //padding
            //offset 0x4C, 0x4D, 0x4E and 0x4F
            bin.Padding3 = br.ReadUInt32();

            // fim do primeiro header

            if (bin.Magic != 0x0030 && bin.Padding1 != 0xCDCDCDCD)
            {           
                br.Close();
                throw new ArgumentException("Invalid bin file, not supported!");
            }

            //---------------------------------------------

            //bonepair_addr_
            if (bonepair_addr_ != 0x0)
            {
                br.BaseStream.Position = startOffset + bonepair_addr_;
             
                uint bonepairCount = br.ReadUInt32();
                bin.BonepairCount = bonepairCount;
          
                List<byte[]> bonepairLines = new List<byte[]>();
                for (int i = 0; i < bonepairCount; i++)
                {
                    byte[] bonepairLine = br.ReadBytes(0x8);
                    bonepairLines.Add(bonepairLine);
                }
                bin.bonepairLines = bonepairLines.ToArray();
            }

            //---------------------------------------------

            //bonesPoint
            br.BaseStream.Position = startOffset + bonesPoint;
         
            Bone[] bones = new Bone[BonesCount];
            for (int i = 0; i < BonesCount; i++)
            {
                Bone bone = new Bone();

                byte[] boneLine = br.ReadBytes(16);
                bone.boneLine = boneLine;
                bones[i] = bone;
            }
            bin.bones = bones;


            //---------------------------------------------

            //MaterialOffset
            br.BaseStream.Position = startOffset + MaterialOffset;

            uint[] NodesTablePointers = new uint[MaterialCount];

            Material[] materials = new Material[MaterialCount];
            for (int i = 0; i < MaterialCount; i++)
            {
                Material material = new Material();

                byte[] materialLine = br.ReadBytes(16);
                //point to Nodes table
                uint nodeTablePoint = BitConverter.ToUInt32(materialLine, 12);
                NodesTablePointers[i] = nodeTablePoint;
                material.materialLine = materialLine;
                material.nodeTablePoint = nodeTablePoint;
                materials[i] = material;
            }
            bin.materials = materials;

            //---------------------------------------------

            Node[] nodes = new Node[NodesTablePointers.Length];

            for (int t = 0; t < NodesTablePointers.Length; t++) // t == Node_ID
            {
                Node node = new Node();
                br.BaseStream.Position = startOffset + NodesTablePointers[t];

                //NodeHeader
                //tamanho total dinâmico
                List<byte> nodeHeader = new List<byte>();

                //ushot quantidade total de bytes do NodeHeader mais o primeiro segmento
                ushort TotalBytesAmount = br.ReadUInt16();
                node.TotalBytesAmount = TotalBytesAmount;
                nodeHeader.AddRange(BitConverter.GetBytes(TotalBytesAmount));
              
                //quantidade total de segmentos alem do primeiro
                byte segmentAmountWithoutFirst = br.ReadByte();
                int TotalNumberOfSegments = (int)(segmentAmountWithoutFirst) + 1;
                nodeHeader.Add(segmentAmountWithoutFirst);
                node.segmentAmountWithoutFirst = segmentAmountWithoutFirst;

                //quantidade de bones usados no node
                byte BonesIdAmount = br.ReadByte();
                nodeHeader.Add(BonesIdAmount);
                node.BonesIdAmount = BonesIdAmount;

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
                node.NodeBoneList = NodeBoneList.Take(BonesIdAmount).ToArray();

                node.NodeHeaderArray = nodeHeader.ToArray();

                //------------
                Segment[] segments = new Segment[TotalNumberOfSegments];

                for (int i = 0; i < TotalNumberOfSegments; i++) // SubMesh //Segment
                {

                    Segment segment = new Segment();
                    segment.IsScenarioColor = false;

                    byte[] Header1 = br.ReadBytes(0x10);

                    if (Header1[12] == 0x00 && Header1[14] > 1) // se verdadeiro tem WeightMapHeader
                    {
                        //WeightMap
                        // arquivo bin dentro de .SMD não tem essa parte
                        segment.WeightMapHeader = Header1;

                        // proxima listagem de WeightMapTable
                        // quantidade de bytes na listagem WeightMapTable
                        int WeightMapTableBytesAmount = Header1[0x0] * 0x10; // valor adquirido em 0x00 ou 0x0e

                        // tamanho variavel
                        byte[] WeightMapArray = br.ReadBytes(WeightMapTableBytesAmount);
                        int WeightMapTableLinesAmount = WeightMapTableBytesAmount / 32;

                        int temp = 0;
                        WeightMapTableLine[] WeightMapTableLines = new WeightMapTableLine[WeightMapTableLinesAmount];
                        for (int a = 0; a < WeightMapTableLinesAmount; a++)
                        {
                            byte[] arr = WeightMapArray.Skip(temp).Take(32).ToArray();

                            WeightMapTableLine line = new WeightMapTableLine();
                            line.weightMapTableLine = arr;
                            WeightMapTableLines[a] = line;
                            temp += 32;
                        }

                        segment.WeightMapTableLines = WeightMapTableLines;

                        // le o segundo header
                        Header1 = br.ReadBytes(0x10);
                    }
                    else 
                    {
                        segment.IsScenarioColor = true;
                        binType = BinType.ScenarioWithColors;
                    }

                    //TopTagVifHeader // header que define a quantidade de dados do grupo de vertices abaixo.

                    segment.TopTagVifHeader2080 = Header1;

                    byte[] Header2 = br.ReadBytes(0x10);
                    segment.TopTagVifHeaderWithScale = Header2;

                    byte[] Header3 = br.ReadBytes(0x10);
                    segment.TopTagVifHeader2180 = Header3;

                    float ConversionFactorValue = BitConverter.ToSingle(Header2, 0x0C);
                    segment.ConversionFactorValue = ConversionFactorValue;

                    int chunkByteAmount = Header3[0x00] * 0x10;
                    byte[] subMeshChunk = br.ReadBytes(chunkByteAmount);

                    int LineAmount = Header2[0x00];

                    VertexLine[] array = new VertexLine[LineAmount];

                    int tempOffset = 0;
                    for (int l = 0; l < LineAmount; l++)
                    {
                        // as linhas são compostas por 24 bytes
                        byte[] line = subMeshChunk.Skip(tempOffset).Take(24).ToArray();
                        VertexLine v = new VertexLine();
                        v.line = line;

                        if (segment.IsScenarioColor)
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
                        }

                        tempOffset += 24;
                        array[l] = v;
                    }

                    segment.vertexLines = array;

                    if (i > 0) // no final do primeiro não tem
                    {
                        // fixo 0x10
                        // EndCommand bytes = 00-00-00-XX-00-00-00-00-00-00-00-00-00-00-00-17
                        byte[] EndTagVifCommand = br.ReadBytes(0x10);
                        segment.EndTagVifCommand = EndTagVifCommand;
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