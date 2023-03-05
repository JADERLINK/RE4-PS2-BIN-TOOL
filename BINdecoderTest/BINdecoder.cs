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
    05-03-2023
    version: alfa.1.0.0.2
    */
    public static class BINdecoder
    {
        public static BIN Decode(Stream stream, string filepatchfortxt2, bool ForceDefaultBinType = false, bool createTxt2Files = false)
        {

            AltTextWriter text = new AltTextWriter(filepatchfortxt2 + ".txt2", createTxt2Files);
            AltTextWriter SubBoneTableText = new AltTextWriter(filepatchfortxt2 + ".SubBoneTableList.txt2", createTxt2Files);
            AltTextWriter VerticesText = new AltTextWriter(filepatchfortxt2 + ".VerticesList.txt2", createTxt2Files);
            AltTextWriter SubBoneHeaderText = new AltTextWriter(filepatchfortxt2 + ".SubBoneHeaderList.txt2", createTxt2Files);
            AltTextWriter TopTagVifHeaderText = new AltTextWriter(filepatchfortxt2 + ".TopTagVifHeaderList.txt2", createTxt2Files);
            AltTextWriter EndTagVifCommandText = new AltTextWriter(filepatchfortxt2 + ".EndTagVifCommandList.txt2", createTxt2Files);
            AltTextWriter NodeHeaderArrayText = new AltTextWriter(filepatchfortxt2 + ".NodeHeaderArray.txt2", createTxt2Files);

            text.WriteLine("##BINdecoderTest##");
            text.WriteLine("##Version A.1.0.0.2##");
            text.WriteLine("");

            BIN bin = new BIN();

            BinType binType = BinType.Default;

            //fix0x3000 //unk001 //unknown0
            //offset 0x00 and 0x01
            byte[] unknown0 = new byte[2];
            stream.Read(unknown0, 0, 2);
            bin.unknown0 = unknown0;
            text.WriteLine("unknown0: " + BitConverter.ToString(unknown0) + "  {hex}");


            //unknown1 //unk002
            // offset 0x02 and 0x03
            byte[] unknown1 = new byte[2];
            stream.Read(unknown1, 0, 2);
            bin.unknown1 = unknown1;
            text.WriteLine("unknown1: " + BitConverter.ToString(unknown1) + "  {hex}");


            //bones point //bone_addr
            byte[] BonesPoint = new byte[4];
            stream.Read(BonesPoint, 0, 4);
            uint bonesPoint = BitConverter.ToUInt32(BonesPoint, 0);
            bin.bonesPoint = bonesPoint;
            text.WriteLine("bonesPoint: 0x" + bonesPoint.ToString("X8"));


            //unknown2 //unk003
            byte[] unknown2 = new byte[1];
            stream.Read(unknown2, 0, 1);
            bin.unknown2 = unknown2[0];
            text.WriteLine("unknown2: " + BitConverter.ToString(unknown2) + "  {hex}");


            //BonesCount //bone_count
            byte[] BonesCount = new byte[1];
            stream.Read(BonesCount, 0, 1);
            bin.BonesCount = BonesCount[0];
            text.WriteLine("BonesCount: " + BonesCount[0].ToString());


            //MaterialCount //table1_count
            byte[] MaterialCount = new byte[1];
            stream.Read(MaterialCount, 0, 1);
            bin.MaterialCount = MaterialCount[0];
            text.WriteLine("MaterialCount: " + MaterialCount[0].ToString());


            //????  //pode ser parte de MaterialCount
            byte[] unknown3 = new byte[1];
            stream.Read(unknown3, 0, 1);
            bin.unknown3 = unknown3[0];
            text.WriteLine("unknown3: " + BitConverter.ToString(unknown3) + "  {hex}");


            //MaterialsPoint //table1_addr
            byte[] MaterialsPoint = new byte[4];
            stream.Read(MaterialsPoint, 0, 4);
            uint materialsPoint = BitConverter.ToUInt32(MaterialsPoint, 0);
            bin.materialsPoint = materialsPoint;
            text.WriteLine("MaterialsPoint: 0x" + materialsPoint.ToString("X8"));


                // parte do header referente ao "unknown4"
            {
                int unknown4offset = (int)stream.Position;

                //unknown4 = 0x40

                // sequencia desconhecida;

                int unknown4_lenght = (int)(bonesPoint - 0x10);

                byte[] unknown4 = new byte[unknown4_lenght];
                stream.Read(unknown4, 0, unknown4_lenght);
                text.WriteLine("unknown4: " + BitConverter.ToString(unknown4) + "  {hex}");
                bin.unknown4 = unknown4;

                text.WriteLine("unknown4 em partes:");
                stream.Position = unknown4offset;

                byte[] Pad8Bytes = new byte[8];
                stream.Read(Pad8Bytes, 0, 8);
                bin.Pad8Bytes = Pad8Bytes;
                text.WriteLine("Pad8Bytes: " + BitConverter.ToString(Pad8Bytes) + "  {hex}");

                //
                byte[] unknown4_B = new byte[4];
                stream.Read(unknown4_B, 0, 4);
                bin.unknown4_B = unknown4_B;
                text.WriteLine("unknown4_B: " + BitConverter.ToString(unknown4_B) + "  {hex}");

                //bonepair_addr
                byte[] bonepair_addr = new byte[4];
                stream.Read(bonepair_addr, 0, 4);
                uint bonepair_addr_ = BitConverter.ToUInt32(bonepair_addr, 0);
                bin.bonepairPoint = bonepair_addr_;
                text.WriteLine("bonepair_addr_: 0x" + bonepair_addr_.ToString("X8"));

                byte[] unknown4_unk008 = new byte[4];
                stream.Read(unknown4_unk008, 0, 4);
                bin.unknown4_unk008 = unknown4_unk008;
                text.WriteLine("unknown4_unk008: " + BitConverter.ToString(unknown4_unk008) + "  {hex}"); // --0 ?

                byte[] unknown4_unk009 = new byte[4];
                stream.Read(unknown4_unk009, 0, 4);
                bin.unknown4_unk009 = unknown4_unk009;
                text.WriteLine("unknown4_unk009: " + BitConverter.ToString(unknown4_unk009) + "  {hex}"); // -- flags

                byte[] boundbox_addr = new byte[4];
                stream.Read(boundbox_addr, 0, 4);
                uint boundbox_addr_ = BitConverter.ToUInt32(boundbox_addr, 0);
                bin.boundboxPoint = boundbox_addr_;
                text.WriteLine("boundbox_addr_: 0x" + boundbox_addr_.ToString("X8"));

                //
                byte[] unknown4_unk010 = new byte[4];
                stream.Read(unknown4_unk010, 0, 4);
                bin.unknown4_unk010 = unknown4_unk010;
                text.WriteLine("unknown4_unk010: " + BitConverter.ToString(unknown4_unk010) + "  {hex}");

                text.WriteLine("unk012: XYZ padding");

                byte[] unk012_floatX = new byte[4];
                stream.Read(unk012_floatX, 0, 4);
                float DrawDistanceNegativeX = BitConverter.ToSingle(unk012_floatX, 0);
                bin.DrawDistanceNegativeX = DrawDistanceNegativeX;
                text.WriteLine("DrawDistanceNegativeX: " + DrawDistanceNegativeX.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

                byte[] unk012_floatY = new byte[4];
                stream.Read(unk012_floatY, 0, 4);
                float DrawDistanceNegativeY = BitConverter.ToSingle(unk012_floatY, 0);
                bin.DrawDistanceNegativeY = DrawDistanceNegativeY;
                text.WriteLine("DrawDistanceNegativeY: " + DrawDistanceNegativeY.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

                byte[] unk012_floatZ = new byte[4];
                stream.Read(unk012_floatZ, 0, 4);
                float DrawDistanceNegativeZ = BitConverter.ToSingle(unk012_floatZ, 0);
                bin.DrawDistanceNegativeZ = DrawDistanceNegativeZ;
                text.WriteLine("DrawDistanceNegativeZ: " + DrawDistanceNegativeZ.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

                byte[] unk012_floatW = new byte[4];
                stream.Read(unk012_floatW, 0, 4);
                float DrawDistanceNegativePadding = BitConverter.ToSingle(unk012_floatW, 0);
                bin.DrawDistanceNegativePadding = DrawDistanceNegativePadding;
                text.WriteLine("DrawDistanceNegativePadding: " + DrawDistanceNegativePadding.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

                text.WriteLine("unk013: XYZ");

                byte[] unk013_floatX = new byte[4];
                stream.Read(unk013_floatX, 0, 4);
                float DrawDistancePositiveX = BitConverter.ToSingle(unk013_floatX, 0);
                bin.DrawDistancePositiveX = DrawDistancePositiveX;
                text.WriteLine("DrawDistancePositiveX: " + DrawDistancePositiveX.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

                byte[] unk013_floatY = new byte[4];
                stream.Read(unk013_floatY, 0, 4);
                float DrawDistancePositiveY = BitConverter.ToSingle(unk013_floatY, 0);
                bin.DrawDistancePositiveY = DrawDistancePositiveY;
                text.WriteLine("DrawDistancePositiveY: " + DrawDistancePositiveY.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

                byte[] unk013_floatZ = new byte[4];
                stream.Read(unk013_floatZ, 0, 4);
                float DrawDistancePositiveZ = BitConverter.ToSingle(unk013_floatZ, 0);
                bin.DrawDistancePositiveZ = DrawDistancePositiveZ;
                text.WriteLine("DrawDistancePositiveZ: " + DrawDistancePositiveZ.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f");

                //
                byte[] Pad4Bytes = new byte[4];
                stream.Read(Pad4Bytes, 0, 4);
                bin.Pad4Bytes = Pad4Bytes;
                text.WriteLine("Pad4Bytes: " + BitConverter.ToString(Pad4Bytes) + "  {hex}");

                //binType

                uint U_unknown4_B = BitConverter.ToUInt32(unknown4_B, 0);

                if (U_unknown4_B == 0x20010801)
                {
                    uint U_unknown4_unk009 = BitConverter.ToUInt32(unknown4_unk009, 0);

                    if (U_unknown4_unk009 == 0xA0000000)
                    {
                        binType = BinType.ScenarioWithNormals;
                    }
                    else if (U_unknown4_unk009 == 0xE0000000)
                    {
                        binType = BinType.ScenarioWithColors;
                    }
                    else if (U_unknown4_unk009 == 0xB0000000 || U_unknown4_unk009 == 0xF0000000)
                    {
                        binType = BinType.ScenarioAlt;
                    }
                }

                if (ForceDefaultBinType)
                {
                    binType = BinType.Default;
                }

                bin.binType = binType;

                text.WriteLine("binType: " + Enum.GetName(typeof(BinType), binType));


            }// fim da parte referente a "unknown4"

            // fim do primeiro header


            //
            stream.Position = bonesPoint;
            text.WriteLine("");
            text.WriteLine("bones:   Em hexadecimal");


            Bone[] bones = new Bone[BonesCount[0]];
            for (int i = 0; i < BonesCount[0]; i++)
            {
                Bone bone = new Bone();

                byte[] boneLine = new byte[16];
                stream.Read(boneLine, 0, 16);
                text.WriteLine("[" + i.ToString("X2") + "]: " + BitConverter.ToString(boneLine));

                bone.boneLine = boneLine;
                bones[i] = bone;
            }
            bin.bones = bones;

            stream.Position = materialsPoint;

            text.WriteLine("");
            text.WriteLine("MaterialList:   Em hexadecimal");
            

            uint[] NodesTablePointers = new uint[MaterialCount[0]];

            Material[] materials = new Material[MaterialCount[0]];
            for (int i = 0; i < MaterialCount[0]; i++)
            {
                Material material = new Material();

                byte[] materialLine = new byte[16];
                stream.Read(materialLine, 0, 16);
                //point to Nodes table
                uint nodeTablePoint = BitConverter.ToUInt32(materialLine, 12);
                NodesTablePointers[i] = nodeTablePoint;

                text.WriteLine("[" + i + "]: " + BitConverter.ToString(materialLine) + "     NodeTablePoint: 0x" + nodeTablePoint.ToString("X8"));

                material.materialLine = materialLine;
                material.nodeTablePoint = nodeTablePoint;
                materials[i] = material;
            }
            bin.materials = materials;

            text.WriteLine("");
            text.WriteLine("---------------");


            Node[] nodes = new Node[NodesTablePointers.Length];

            int ContagemIndicePraObj = 1;

            for (int t = 0; t < NodesTablePointers.Length; t++) // t == Node_ID
            {
                Node node = new Node();
                
                text.WriteLine("");

                stream.Position = NodesTablePointers[t];

                text.WriteLine("NodeTablePointer: 0x" + NodesTablePointers[t].ToString("X8"));

                //IDXBINtext
                List<byte> header = new List<byte>();

                //NodeHeader
                //tamanho total dinâmico
                byte[] NodeHeaderArray = new byte[0x10]; 
                stream.Read(NodeHeaderArray, 0, 0x10);
                header.AddRange(NodeHeaderArray);
                text.WriteLine("NodeHeaderArray: " + BitConverter.ToString(NodeHeaderArray));
                NodeHeaderArrayText.WriteLine(".[" + t.ToString("D4") +"]: " + BitConverter.ToString(NodeHeaderArray));

                //NodeHeaderArray[0] and NodeHeaderArray[1] = quantide de bytes do inicio do NodeHeaderArray ate o final do SubBoneTableArray

                //SplitCount
                int QuantidadeTotalDeproximosBytes = NodeHeaderArray[0x3];
                text.WriteLine("QuantidadeTotalDeproximosBytes: " + QuantidadeTotalDeproximosBytes);

                // calculo pra saber quantas partes tem o NodeHeaderArray

                int totalBytesNoNodeHeaderArray = 4 + QuantidadeTotalDeproximosBytes;

                int counterNodeHeaderArrayLenght = 0x10;
                while (totalBytesNoNodeHeaderArray > counterNodeHeaderArrayLenght)
                {
                    //continuação do NodeHeaderArray, caso tiver
                    // cada parte de continuação é 0x10, ao todo
                    byte[] NodeHeaderArrayContinuation = new byte[0x10];
                    stream.Read(NodeHeaderArrayContinuation, 0, 0x10);
                    header.AddRange(NodeHeaderArrayContinuation);
                    text.WriteLine("NodeHeaderArrayContinuation: " + BitConverter.ToString(NodeHeaderArrayContinuation));
                    NodeHeaderArrayText.WriteLine("NodeHeaderArrayContinuation: " + BitConverter.ToString(NodeHeaderArrayContinuation));
                    counterNodeHeaderArrayLenght += 0x10;
                }

                node.NodeHeaderArray = NodeHeaderArray;


                int valorTotalDeSegmentos = (int)(NodeHeaderArray[0x2]) + 1;

                text.WriteLine("valorTotalDeSegmentos: " + valorTotalDeSegmentos);

                //
                Segment[] segments = new Segment[valorTotalDeSegmentos];


                for (int i = 0; i < valorTotalDeSegmentos; i++) // SubMesh //Segment
                {
                    Segment segment = new Segment();

                    text.WriteLine("");

                    if (binType == BinType.Default || binType == BinType.ScenarioAlt) // arquivo bin dentro de .SMD não tem essa parte
                    {
                        //SubBoneHeader
                        text.WriteLine("stream.Position: 0x" + stream.Position.ToString("X8"));
                        uint SubBoneHeaderOffset = (uint)stream.Position;
                        byte[] SubBoneHeader = new byte[0x10]; 
                        stream.Read(SubBoneHeader, 0, 0x10);
                        text.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] SubBoneHeader: " + BitConverter.ToString(SubBoneHeader));
                        SubBoneHeaderText.WriteLine(SubBoneHeaderOffset.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "] SubBoneHeader: " + BitConverter.ToString(SubBoneHeader));

                        segment.SubBoneHeader = SubBoneHeader;

                        // proxima listagem de SubBoneTable
                        // quantidade de bytes na listagem SubBoneTable

                        int SubBoneTableAmount = SubBoneHeader[0x0]; // valor adquirido em 0x00 ou 0x0e
                        int subBoneTableAmountFix = SubBoneTableAmount * 0x10;


                        // tamanho variavel
                       
                        uint offsetSubBoneTable = (uint)stream.Position;
                        byte[] SubBoneTableArray = new byte[subBoneTableAmountFix];
                        stream.Read(SubBoneTableArray, 0, subBoneTableAmountFix);


                        int SubBoneTableLinesAmount = subBoneTableAmountFix / 32;
                        text.WriteLine("subBoneTableAmountFix: 0x" + subBoneTableAmountFix.ToString("X4"));
                        text.WriteLine("SubBoneTableLinesAmount em float:" + (float)subBoneTableAmountFix / 32);

                        text.WriteLine("");
                        int temp = 0;

                        byte[][] SubBoneTableLines = new byte[SubBoneTableLinesAmount][];

                        for (int a = 0; a < SubBoneTableLinesAmount; a++)
                        {
                            byte[] arr = SubBoneTableArray.Skip(temp).Take(32).ToArray();

                            SubBoneTableLines[a] = arr;

                            temp += 32;
                            text.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + a.ToString("D2") + "]: " + BitConverter.ToString(arr));
                            SubBoneTableText.WriteLine(offsetSubBoneTable.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + a.ToString("D2") + "]: " + BitConverter.ToString(arr));
                            offsetSubBoneTable += 32;
                        }

                        segment.SubBoneTableLines = SubBoneTableLines;

                        text.WriteLine("");
                    }


                    uint offsetTopHeader = (uint)stream.Position;
                    text.WriteLine("stream.Position: 0x" + stream.Position.ToString("X8"));
                    //TopTagVifHeader // header que define a quantidade de dados do grupo de vertices abaixo.
                    // fixo 0x30
                    byte[] TopTagVifHeader = new byte[0x30];
                    stream.Read(TopTagVifHeader, 0, 0x30);
                    text.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] TopTagVifHeader: " + BitConverter.ToString(TopTagVifHeader));
                    TopTagVifHeaderText.WriteLine(offsetTopHeader.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "] TopTagVifHeader: " + BitConverter.ToString(TopTagVifHeader));

                    float Scale = BitConverter.ToSingle(TopTagVifHeader, 0x1C);

                    segment.TopTagVifHeader = TopTagVifHeader;
                    segment.Scale = Scale;


                    int chunkByteAmount = TopTagVifHeader[0x20];

                    int chunkByteAmountFix = chunkByteAmount * 0x10;

                    byte[] subMeshChunk = new byte[chunkByteAmountFix];

                    text.WriteLine("stream.Position: 0x" + stream.Position.ToString("X8"));
                    uint CalculoOffsetLinha = (uint)stream.Position;
                    stream.Read(subMeshChunk, 0, chunkByteAmountFix);

                    text.WriteLine("chunkByteAmount: 0x" + chunkByteAmount.ToString("X2"));
                    text.WriteLine("chunkByteAmountFix: 0x" + chunkByteAmountFix.ToString("X4"));


                    int LineAmount = chunkByteAmountFix / 24; // nota: tem um valor em "TopTagVifHeader" que representa a guantidade de LineAmount //TopTagVifHeader[0x10]

                    text.WriteLine("LineAmount em decimal: " + LineAmount);
                    text.WriteLine("LineAmount resposta em float: " + (float)chunkByteAmountFix / 24);

                    text.WriteLine("Scale em float: " + Scale.ToString(System.Globalization.CultureInfo.InvariantCulture));


                    VertexLine[] array = new VertexLine[LineAmount];

                    text.WriteLine("");
                    text.WriteLine("subMeshChunk:");

                    if (binType == BinType.ScenarioWithColors || binType == BinType.ScenarioWithNormals)
                    {
                        text.WriteLine("Offset em hexadecimal: [MaterialCount][valorTotalDePartes][linha]: VerticeX, VerticeY, VerticeZ, IndexMount, TextureU, TextureV, UnknownA, IndexComplement, NormalX, NormalY, NormalZ, UnknownB");
                    }
                    else 
                    {
                        text.WriteLine("Offset em hexadecimal: [MaterialCount][valorTotalDePartes][linha]: VerticeX, VerticeY, VerticeZ, UnknownB, NormalX, NormalY, NormalZ, IndexComplement, TextureU, TextureV, UnknownA, IndexMount");
                    }
                    

                    int tempOffset = 0;
                    for (int l = 0; l < LineAmount; l++)
                    {
                        // as linhas são compostas por 24 bytes
                        byte[] line = subMeshChunk.Skip(tempOffset).Take(24).ToArray();
                        VertexLine v = new VertexLine();
                        v.line = line;

                        if (binType == BinType.ScenarioWithColors || binType == BinType.ScenarioWithNormals)
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

                            text.WriteLine(CalculoOffsetLinha.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + l.ToString("D2") + "]:" +
                               "  vX: " + v.VerticeX.ToString().PadLeft(6) +
                               "  vY: " + v.VerticeY.ToString().PadLeft(6) +
                               "  vZ: " + v.VerticeZ.ToString().PadLeft(6) +
                               "  IndexMount: " + v.IndexMount.ToString("X4") +
                               "  tU: " + v.TextureU.ToString().PadLeft(6) +
                               "  tV: " + v.TextureV.ToString().PadLeft(6) +
                               "  UnkA: " + v.UnknownA.ToString("X4") +
                               "  IdxComp: " + v.IndexComplement.ToString("X4") +
                               "  nX: " + v.NormalX.ToString().PadLeft(6) +
                               "  nY: " + v.NormalY.ToString().PadLeft(6) +
                               "  nZ: " + v.NormalZ.ToString().PadLeft(6) +
                               "  UnkB: " + v.UnknownB.ToString("X4")
                              );

                            VerticesText.WriteLine(CalculoOffsetLinha.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + l.ToString("D2") + "][" + ContagemIndicePraObj.ToString("D4") + "]:" +
                               "  vX: " + v.VerticeX.ToString().PadLeft(6) +
                               "  vY: " + v.VerticeY.ToString().PadLeft(6) +
                               "  vZ: " + v.VerticeZ.ToString().PadLeft(6) +
                               "  IndexMount: " + v.IndexMount.ToString("X4") +
                               "  tU: " + v.TextureU.ToString().PadLeft(6) +
                               "  tV: " + v.TextureV.ToString().PadLeft(6) +
                               "  UnkA: " + v.UnknownA.ToString("X4") +
                               "  IdxComp: " + v.IndexComplement.ToString("X4") +
                               "  nX: " + v.NormalX.ToString().PadLeft(6) +
                               "  nY: " + v.NormalY.ToString().PadLeft(6) +
                               "  nZ: " + v.NormalZ.ToString().PadLeft(6) +
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

                            text.WriteLine(CalculoOffsetLinha.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + l.ToString("D2") + "]:" +
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

                            VerticesText.WriteLine(CalculoOffsetLinha.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + l.ToString("D2") + "][" + ContagemIndicePraObj.ToString("D4") + "]:" +
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

                        ContagemIndicePraObj++;
                        CalculoOffsetLinha += 24;
                        tempOffset += 24;
                        array[l] = v;
                    }

                    segment.vertexLines = array;

                    if (i > 0) // no final do primeiro não tem
                    {
                        // fixo 0x10
                        // EndCommand bytes = 00-00-00-XX-00-00-00-00-00-00-00-00-00-00-00-17
                        text.WriteLine("stream.Position: 0x" + stream.Position.ToString("X8"));
                        byte[] EndTagVifCommand = new byte[0x10];
                        stream.Read(EndTagVifCommand, 0, 0x10);
                        text.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] EndTagVifCommand: " + BitConverter.ToString(EndTagVifCommand));
                        EndTagVifCommandText.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] EndTagVifCommand: " + BitConverter.ToString(EndTagVifCommand));
                        text.WriteLine("");
                    }

                    segments[i] = segment;
                }

                node.Segments = segments;
                nodes[t] = node;
            }

            bin.Nodes = nodes;

            // end file

            if (stream.Position < stream.Length) // no caso, não deve ter mais bytes
            {
                text.WriteLine("");
                text.WriteLine("stream.Position: 0x" + stream.Position.ToString("X8"));
                text.WriteLine("Bytes no final do arquivo");
                int lenght = (int)(stream.Length - stream.Position);
                byte[] end = new byte[lenght];
                stream.Read(end, 0, lenght);
                text.WriteLine("EndFile: " + BitConverter.ToString(end));
            }


            VerticesText.Close();
            SubBoneTableText.Close();
            SubBoneHeaderText.Close();
            TopTagVifHeaderText.Close();
            NodeHeaderArrayText.Close();
            EndTagVifCommandText.Close();
            text.Close();
            stream.Close();

            return bin;
        }

        public static void CreateObjMtl(BIN bin, string baseDiretory, string baseFileName, string baseTextureName)
        {
            if (baseDiretory[baseDiretory.Length - 1] != '\\')
            {
                baseDiretory += "\\";
            }

            TextWriter MTLtext = new FileInfo(baseDiretory + baseFileName + ".mtl").CreateText();
            MTLtext.WriteLine("##BINdecoderTest##");
            MTLtext.WriteLine("##Version A.1.0.0.2##");
            MTLtext.WriteLine("");

            for (int i = 0; i < bin.materials.Length; i++)
            {
                MTLtext.WriteLine("");
                MTLtext.WriteLine("newmtl Material" + i);
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
            text.WriteLine("##version A.1.0.0.2##");
            text.WriteLine("mtllib " + baseFileName + ".mtl");

            int indexGeral = 1;

            for (int t = 0; t < bin.Nodes.Length; t++)
            {

                //usemtl Material*
                text.WriteLine("g Material" + t);
                text.WriteLine("usemtl Material" + t);

                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    for (int l = 0; l < bin.Nodes[t].Segments[i].vertexLines.Length; l++)
                    {
                        VertexLine vertexLine = bin.Nodes[t].Segments[i].vertexLines[l];

                        text.WriteLine("v " + ((float)vertexLine.VerticeX * bin.Nodes[t].Segments[i].Scale).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             ((float)vertexLine.VerticeY * bin.Nodes[t].Segments[i].Scale).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             ((float)vertexLine.VerticeZ * bin.Nodes[t].Segments[i].Scale).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

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
            IDXBINtext.WriteLine(":##Version A.1.0.0.2##");

            IDXBINtext.WriteLine("CompressVertices:True");
            IDXBINtext.WriteLine("AutoScale:True");

            IDXBINtext.WriteLine("IsScenarioBin:" + (bin.binType == BinType.ScenarioWithNormals || bin.binType == BinType.ScenarioWithColors));

            if (bin.binType == BinType.ScenarioWithNormals || bin.binType == BinType.ScenarioWithColors)
            {
                IDXBINtext.WriteLine("ScenarioUseColors:" + (bin.binType == BinType.ScenarioWithColors));
            }

            IDXBINtext.WriteLine(": 2 bytes in hex");
            IDXBINtext.WriteLine("fix3000:" + BitConverter.ToString(bin.unknown0).Replace("-", ""));

            IDXBINtext.WriteLine(": 2 bytes in hex");
            IDXBINtext.WriteLine("unknown1:" + BitConverter.ToString(bin.unknown1).Replace("-", ""));

            IDXBINtext.WriteLine(": 1 bytes in hex");
            IDXBINtext.WriteLine("unknown2:" + bin.unknown2.ToString("X2"));

            IDXBINtext.WriteLine(": decimal value");
            IDXBINtext.WriteLine("BonesCount:" + bin.BonesCount.ToString());

            IDXBINtext.WriteLine(": decimal value");
            IDXBINtext.WriteLine("MaterialCount:" + bin.MaterialCount.ToString());

            IDXBINtext.WriteLine(": 1 bytes in hex");
            IDXBINtext.WriteLine("unknown3:" + bin.unknown3.ToString("X2"));

            //IDXBINtext.WriteLine(": decimal value");
            //IDXBINtext.WriteLine("unknown4length:" + bin.unknown4.Length);
            //IDXBINtext.WriteLine("unknown4:" + BitConverter.ToString(bin.unknown4).Replace("-", ""));

            // unknown4
            IDXBINtext.WriteLine(": 4 bytes in hex");
            IDXBINtext.WriteLine("unknown4_B:" + BitConverter.ToString(bin.unknown4_B).Replace("-", ""));

            IDXBINtext.WriteLine(": 4 bytes in hex");
            IDXBINtext.WriteLine("unknown4_unk008:" + BitConverter.ToString(bin.unknown4_unk008).Replace("-", ""));

            IDXBINtext.WriteLine(": 4 bytes in hex");
            IDXBINtext.WriteLine("unknown4_unk009:" + BitConverter.ToString(bin.unknown4_unk009).Replace("-", ""));

            IDXBINtext.WriteLine(": 4 bytes in hex");
            IDXBINtext.WriteLine("unknown4_unk010:" + BitConverter.ToString(bin.unknown4_unk010).Replace("-", ""));

            IDXBINtext.WriteLine(": DrawDistance float values");
            IDXBINtext.WriteLine("DrawDistanceNegativeX:" + bin.DrawDistanceNegativeX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistanceNegativeY:" + bin.DrawDistanceNegativeY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistanceNegativeZ:" + bin.DrawDistanceNegativeZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistanceNegativePadding:" + bin.DrawDistanceNegativePadding.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistancePositiveX:" + bin.DrawDistancePositiveX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistancePositiveY:" + bin.DrawDistancePositiveY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            IDXBINtext.WriteLine("DrawDistancePositiveZ:" + bin.DrawDistancePositiveZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

            //-- unknown4 end

            IDXBINtext.WriteLine(": boneLines");

            for (int i = 0; i < bin.bones.Length; i++)
            {
                IDXBINtext.WriteLine("boneLine_" + i + ":" + BitConverter.ToString(bin.bones[i].boneLine).Replace("-", ""));
            }
                
            IDXBINtext.WriteLine(": materialLine and NodeHeaderLine");
            IDXBINtext.WriteLine(": materialLine_* -> 12 bytes");

            for (int t = 0; t < bin.materials.Length; t++)
            {
                IDXBINtext.WriteLine("materialLine_" + t + ":" + BitConverter.ToString(bin.materials[t].materialLine.Take(12).ToArray()).Replace("-", ""));

                if (bin.binType == BinType.Default || bin.binType == BinType.ScenarioAlt)
                {
                    byte QuantidadeTotalDeproximosBytes = bin.Nodes[t].NodeHeaderArray[3];

                    IDXBINtext.WriteLine("NodeHeader_SplitCount_" + t + ":" + QuantidadeTotalDeproximosBytes);
                    IDXBINtext.WriteLine("NodeHeader_SplitValues_" + t + ":" + BitConverter.ToString(bin.Nodes[t].NodeHeaderArray.Skip(4).Take(QuantidadeTotalDeproximosBytes).ToArray()).Replace("-", ""));
                }

                if (bin.binType == BinType.ScenarioWithColors)
                {
                    IDXBINtext.WriteLine("ScenarioVerticeColor_" + t + ":FFFFFF");
                }

                // scale media

                float allScales = 0;
                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    allScales += bin.Nodes[t].Segments[i].Scale;
                }

                float scaleMedia = allScales / bin.Nodes[t].Segments.Length;

                IDXBINtext.WriteLine("TopTagVifHeader_Scale_" + t + ":" + scaleMedia.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            IDXBINtext.Close();
        }

        public static void CreateDrawDistanceBoxObj(BIN bin, string baseDiretory, string baseFileName)
        {
            //

            if (baseDiretory[baseDiretory.Length - 1] != '\\')
            {
                baseDiretory += "\\";
            }

            TextWriter text = new FileInfo(baseDiretory + baseFileName + ".DrawDistanceBox.obj").CreateText();
            text.WriteLine("##BINdecoderTest##");
            text.WriteLine("##version A.1.0.0.2##");

            text.WriteLine("");

            // real

            //1
            text.WriteLine("v " + bin.DrawDistanceNegativeX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                bin.DrawDistanceNegativeY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                bin.DrawDistanceNegativeZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                );

            //2
            text.WriteLine("v " + bin.DrawDistancePositiveX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
            bin.DrawDistancePositiveY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
            bin.DrawDistancePositiveZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
            );

            //inverso Y

            //3
            text.WriteLine("v " + bin.DrawDistanceNegativeX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                bin.DrawDistancePositiveY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                bin.DrawDistanceNegativeZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                );

            //4
            text.WriteLine("v " + bin.DrawDistancePositiveX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
            bin.DrawDistanceNegativeY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
            bin.DrawDistancePositiveZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
            );

            // inveso Z

            //5
            text.WriteLine("v " + bin.DrawDistanceNegativeX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
           bin.DrawDistanceNegativeY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
           bin.DrawDistancePositiveZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
           );

            //6
            text.WriteLine("v " + bin.DrawDistancePositiveX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
            bin.DrawDistancePositiveY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
            bin.DrawDistanceNegativeZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
            );

            // inverso X

            //7
            text.WriteLine("v " + bin.DrawDistancePositiveX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                bin.DrawDistanceNegativeY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                bin.DrawDistanceNegativeZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
                );

            //8
            text.WriteLine("v " + bin.DrawDistanceNegativeX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
            bin.DrawDistancePositiveY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
            bin.DrawDistancePositiveZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture)
            );


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

        public static void CreateScaleLimitBoxObj(BIN bin, string baseDiretory, string baseFileName)
        {

            //

            if (baseDiretory[baseDiretory.Length - 1] != '\\')
            {
                baseDiretory += "\\";
            }

            TextWriter text = new FileInfo(baseDiretory + baseFileName + ".ScaleLimitBox.obj").CreateText();
            text.WriteLine("##BINdecoderTest##");
            text.WriteLine("##version A.1.0.0.2##");

            text.WriteLine("");

            float scalenodes = 0;
            for (int t = 0; t < bin.Nodes.Length; t++)
            {
                float allScales = 0;
                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    allScales += bin.Nodes[t].Segments[i].Scale;
                }

                scalenodes += allScales / bin.Nodes[t].Segments.Length;
            }
            float scale = scalenodes / bin.Nodes.Length;

            float maxPos = scale * short.MaxValue;
            float minPos = scale * short.MinValue;

            text.WriteLine("# scale: " + scale.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
            text.WriteLine("");

            //float shorterDistance = scale * 1;

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
        //compatibilidade
        public byte[] unknown4;

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
        public byte[] SubBoneHeader;

        //[quantidade de linhas][0x20]
        public byte[][] SubBoneTableLines;

        //--
        //TopTagVifHeader
        //new byte[0x30];
        public byte[] TopTagVifHeader;
        public float Scale;
        //--

        //vertexLines
        public VertexLine[] vertexLines;

        //EndTagVifCommand
        //new byte[0x10];
        public byte[] EndTagVifCommand;
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
        ScenarioAlt,
        ScenarioWithNormals,
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