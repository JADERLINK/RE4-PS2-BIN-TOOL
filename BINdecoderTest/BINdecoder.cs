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
    03-02-2023
    */
    public static class BINdecoder
    {
        public static VertexLine[][][] vertexLines;

        public static void Decode(string file, bool ArquivoDeRoom = false)
        {
            var fileInfo = new FileInfo(file);
            string diretory = fileInfo.DirectoryName + "\\";
            string fileName = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length, fileInfo.Extension.Length);

            Stream stream = fileInfo.OpenRead();

            TextWriter text = new FileInfo(file + ".txt2").CreateText();
            TextWriter SubBoneTableText = new FileInfo(file + ".SubBoneTableList.txt2").CreateText();
            TextWriter VerticesText = new FileInfo(file + ".VerticesList.txt2").CreateText();
            TextWriter SubBoneHeaderText = new FileInfo(file + ".SubBoneHeaderList.txt2").CreateText();
            TextWriter TopTagVifHeaderText = new FileInfo(file + ".TopTagVifHeaderList.txt2").CreateText();
            TextWriter EndTagVifCommandText = new FileInfo(file + ".EndTagVifCommandList.txt2").CreateText();
            TextWriter NodeHeaderArrayText = new FileInfo(file + ".NodeHeaderArray.txt2").CreateText();


            TextWriter IDXBINtext = new FileInfo(diretory + fileName + ".idxbin").CreateText();
            IDXBINtext.WriteLine(":##BINdecoderTest##");
            IDXBINtext.WriteLine(":##Version A.1.0.0.0##");
            IDXBINtext.WriteLine("IsScenarioBin:" + ArquivoDeRoom);

            TextWriter MTLtext = new FileInfo(diretory + fileName + ".mtl").CreateText();
            MTLtext.WriteLine("##BINdecoderTest##");
            MTLtext.WriteLine("##Version A.1.0.0.0##");

            text.WriteLine("##BINdecoderTest##");
            text.WriteLine("##Version A.1.0.0.0##");
            text.WriteLine("");


            //fix0x3000 //unk001
            byte[] fix0x3000 = new byte[2];
            stream.Read(fix0x3000, 0, 2);
            text.WriteLine("fix0x3000: " + BitConverter.ToString(fix0x3000) + "  {hex}");
            IDXBINtext.WriteLine(": 2 bytes in hex");
            IDXBINtext.WriteLine("fix3000:" + BitConverter.ToString(fix0x3000).Replace("-", ""));

            //unknown1 //unk002
            byte[] unknown1 = new byte[2];
            stream.Read(unknown1, 0, 2);
            text.WriteLine("unknown1: " + BitConverter.ToString(unknown1) + "  {hex}");
            IDXBINtext.WriteLine(": 2 bytes in hex");
            IDXBINtext.WriteLine("unknown1:" + BitConverter.ToString(unknown1).Replace("-", ""));

            //bones point //bone_addr
            byte[] BonesPoint = new byte[4];
            stream.Read(BonesPoint, 0, 4);
            uint bonesPoint = BitConverter.ToUInt32(BonesPoint, 0);
            text.WriteLine("bonesPoint: 0x" + bonesPoint.ToString("X8"));

            //unknown2 //unk003
            byte[] unknown2 = new byte[1];
            stream.Read(unknown2, 0, 1);
            text.WriteLine("unknown2: " + BitConverter.ToString(unknown2) + "  {hex}");
            IDXBINtext.WriteLine(": 1 bytes in hex");
            IDXBINtext.WriteLine("unknown2:" + BitConverter.ToString(unknown2).Replace("-", ""));

            //BonesCount //bone_count
            byte[] BonesCount = new byte[1];
            stream.Read(BonesCount, 0, 1);
            text.WriteLine("BonesCount: " + BonesCount[0].ToString());
            IDXBINtext.WriteLine(": decimal value");
            IDXBINtext.WriteLine("BonesCount:" + BonesCount[0].ToString());

            //MaterialCount //table1_count
            byte[] MaterialCount = new byte[1];
            stream.Read(MaterialCount, 0, 1);
            text.WriteLine("MaterialCount: " + MaterialCount[0].ToString());
            IDXBINtext.WriteLine(": decimal value");
            IDXBINtext.WriteLine("MaterialCount:" + MaterialCount[0].ToString());

            //????
            byte[] unknown3 = new byte[1];
            stream.Read(unknown3, 0, 1);
            text.WriteLine("unknown3: " + BitConverter.ToString(unknown3) + "  {hex} pode ser parte de MaterialCount");
            IDXBINtext.WriteLine(": 1 bytes in hex");
            IDXBINtext.WriteLine("unknown3:" + BitConverter.ToString(unknown3).Replace("-", ""));


            //MaterialPoint //table1_addr
            byte[] MaterialsPoint = new byte[4];
            stream.Read(MaterialsPoint, 0, 4);
            uint materialsPoint = BitConverter.ToUInt32(MaterialsPoint, 0);
            text.WriteLine("MaterialsPoint: 0x" + materialsPoint.ToString("X8"));


            int unknown4offset = (int)stream.Position;

            //unknown4 = 0x40

            // sequencia desconhecida;
            
            int unknown4_lenght = (int)(bonesPoint - 0x10);

            byte[] unknown4 = new byte[unknown4_lenght];
            stream.Read(unknown4, 0, unknown4_lenght);
            text.WriteLine("unknown4: " + BitConverter.ToString(unknown4) + "  {hex}");
            IDXBINtext.WriteLine(": decimal value");
            IDXBINtext.WriteLine("unknown4length:" + unknown4_lenght);
            IDXBINtext.WriteLine("unknown4:" + BitConverter.ToString(unknown4).Replace("-", ""));


            text.WriteLine("unknown4 em partes:");
            stream.Position = unknown4offset;

            byte[] Pad8Bytes = new byte[8];
            stream.Read(Pad8Bytes, 0, 8);
            text.WriteLine("Pad8Bytes: " + BitConverter.ToString(Pad8Bytes) + "  {hex}");

            //
            byte[] unknown4_B = new byte[4];
            stream.Read(unknown4_B, 0, 4);
            text.WriteLine("unknown4_B: " + BitConverter.ToString(unknown4_B) + "  {hex}");

            //bonepair_addr
            byte[] bonepair_addr = new byte[4];
            stream.Read(bonepair_addr, 0, 4);
            uint bonepair_addr_ = BitConverter.ToUInt32(bonepair_addr, 0);
            text.WriteLine("bonepair_addr_: 0x" + bonepair_addr_.ToString("X8"));

            byte[] unknown4_unk008 = new byte[4];
            stream.Read(unknown4_unk008, 0, 4);
            text.WriteLine("unknown4_unk008: " + BitConverter.ToString(unknown4_unk008) + "  {hex}"); // --0 ?

            byte[] unknown4_unk009 = new byte[4];
            stream.Read(unknown4_unk009, 0, 4);
            text.WriteLine("unknown4_unk009: " + BitConverter.ToString(unknown4_unk009) + "  {hex}"); // -- flags

            byte[] boundbox_addr = new byte[4];
            stream.Read(boundbox_addr, 0, 4);
            uint boundbox_addr_ = BitConverter.ToUInt32(boundbox_addr, 0);
            text.WriteLine("boundbox_addr_: 0x" + boundbox_addr_.ToString("X8"));

            //
            byte[] unknown4_unk010 = new byte[4];
            stream.Read(unknown4_unk010, 0, 4);
            text.WriteLine("unknown4_unk010: " + BitConverter.ToString(unknown4_unk010) + "  {hex}");

            byte[] unk012_floatX = new byte[4];
            stream.Read(unk012_floatX, 0, 4);
            text.WriteLine("unk012_floatX: " + BitConverter.ToSingle(unk012_floatX, 0) + "f");

            byte[] unk012_floatY = new byte[4];
            stream.Read(unk012_floatY, 0, 4);
            text.WriteLine("unk012_floatY: " + BitConverter.ToSingle(unk012_floatY, 0) + "f");

            byte[] unk012_floatZ = new byte[4];
            stream.Read(unk012_floatZ, 0, 4);
            text.WriteLine("unk012_floatZ: " + BitConverter.ToSingle(unk012_floatZ, 0) + "f");

            byte[] unk012_floatW = new byte[4];
            stream.Read(unk012_floatW, 0, 4);
            text.WriteLine("unk012_floatW: " + BitConverter.ToSingle(unk012_floatW, 0) + "f");

            byte[] unk013_floatX = new byte[4];
            stream.Read(unk013_floatX, 0, 4);
            text.WriteLine("unk013_floatX: " + BitConverter.ToSingle(unk013_floatX, 0) + "f");

            byte[] unk013_floatY = new byte[4];
            stream.Read(unk013_floatY, 0, 4);
            text.WriteLine("unk013_floatY: " + BitConverter.ToSingle(unk013_floatY, 0) + "f");

            byte[] unk013_floatZ = new byte[4];
            stream.Read(unk013_floatZ, 0, 4);
            text.WriteLine("unk013_floatZ: " + BitConverter.ToSingle(unk013_floatZ, 0) + "f");

            //
            byte[] Pad4Bytes = new byte[4];
            stream.Read(Pad4Bytes, 0, 4);
            text.WriteLine("Pad4Bytes: " + BitConverter.ToString(Pad4Bytes) + "  {hex}");


            //
            stream.Position = bonesPoint;
            text.WriteLine("");
            text.WriteLine("bones:   Em hexadecimal");
            IDXBINtext.WriteLine(": boneLines");


            for (int i = 0; i < BonesCount[0]; i++)
            {
                byte[] boneLine = new byte[16];
                stream.Read(boneLine, 0, 16);
                text.WriteLine("[" + i + "]: " + BitConverter.ToString(boneLine));
                IDXBINtext.WriteLine("boneLine_" + i + ":" + BitConverter.ToString(boneLine).Replace("-", ""));
            }

            stream.Position = materialsPoint;

            text.WriteLine("");
            text.WriteLine("MaterialList:   Em hexadecimal");
            IDXBINtext.WriteLine(": materialLine");

            uint[] NodesTablePointers = new uint[MaterialCount[0]];

            for (int i = 0; i < MaterialCount[0]; i++)
            {
                byte[] materialLine = new byte[16];
                stream.Read(materialLine, 0, 16);
                //point to Nodes table
                uint nodeTablePoint = BitConverter.ToUInt32(materialLine, 12);
                NodesTablePointers[i] = nodeTablePoint;

                text.WriteLine("[" + i + "]: " + BitConverter.ToString(materialLine) + "     NodeTablePoint: 0x" + nodeTablePoint.ToString("X8"));

                MTLtext.WriteLine("");
                MTLtext.WriteLine("newmtl Material" + i);
                MTLtext.WriteLine("Ka 1.000 1.000 1.000");
                MTLtext.WriteLine("Kd 1.000 1.000 1.000");
                MTLtext.WriteLine("Ks 0.000 0.000 0.000");
                MTLtext.WriteLine("Ns 0");
                MTLtext.WriteLine("d 1");
                MTLtext.WriteLine("Tr 1"); 
                MTLtext.WriteLine("map_Kd " + fileName + "/" + materialLine[1].ToString() +".tga");
                MTLtext.WriteLine("");

                // ---
                IDXBINtext.WriteLine("materialLine_" + i + ":" + BitConverter.ToString(materialLine.Take(12).ToArray()).Replace("-", ""));
            }

            text.WriteLine("");
            text.WriteLine("---------------");
            IDXBINtext.WriteLine(": NodeHeaderLine");


            vertexLines = new VertexLine[MaterialCount[0]][][];


            int ContagemIndicePraObj = 1;

            for (int t = 0; t < NodesTablePointers.Length; t++) // t == Node_ID
            {
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

                //NodeHeaderArray[0] and NodeHeaderArray[0] = quantide de bytes do inicio do NodeHeaderArray ate o final do SubBoneTableArray


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


                int valorTotalDePartes = (int)(NodeHeaderArray[0x2]) + 1;

                text.WriteLine("valorTotalDePartes: " + valorTotalDePartes);

                vertexLines[t] = new VertexLine[valorTotalDePartes][];

                if (!ArquivoDeRoom)
                {
                    IDXBINtext.WriteLine("NodeHeaderLineP2_Length_" + t + ":" + QuantidadeTotalDeproximosBytes);
                    IDXBINtext.WriteLine("NodeHeaderLineP2_" + t + ":" + BitConverter.ToString(header.Skip(4).Take(QuantidadeTotalDeproximosBytes).ToArray()).Replace("-", ""));
                }

                //

                for (int i = 0; i < valorTotalDePartes; i++) // SubMesh
                {
                    text.WriteLine("");

                    // para arquivo que são de room, colocar true
                    //bool ArquivoDeRoom = false; // variavel definida na entrada do metodo

           
                    if (!ArquivoDeRoom) // arquivo bin dentro de .SMD não tem essa parte
                    {

                        //SubBoneHeader
                        text.WriteLine("stream.Position: 0x" + stream.Position.ToString("X8"));
                        uint SubBoneHeaderOffset = (uint)stream.Position;
                        byte[] SubBoneHeader = new byte[0x10]; 
                        stream.Read(SubBoneHeader, 0, 0x10);
                        text.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] SubBoneHeader: " + BitConverter.ToString(SubBoneHeader));
                        SubBoneHeaderText.WriteLine(SubBoneHeaderOffset.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "] SubBoneHeader: " + BitConverter.ToString(SubBoneHeader));

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

                        for (int a = 0; a < SubBoneTableLinesAmount; a++)
                        {
                            byte[] arr = SubBoneTableArray.Skip(temp).Take(32).ToArray();

                            temp += 32;
                            text.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + a.ToString("D2") + "]: " + BitConverter.ToString(arr));
                            SubBoneTableText.WriteLine(offsetSubBoneTable.ToString("X8") + ": [" + t.ToString("D2") + "][" + i.ToString("D2") + "][" + a.ToString("D2") + "]: " + BitConverter.ToString(arr));
                            offsetSubBoneTable += 32;
                        }

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

                    if (t == 0 && i == 0)
                    {
                        IDXBINtext.WriteLine(": 1 bytes in hex");
                        IDXBINtext.WriteLine("TopTagVif0x1F:" + TopTagVifHeader[0x1F].ToString("X2"));
                    }


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



                    VertexLine[] array = new VertexLine[LineAmount];

                    text.WriteLine("");
                    text.WriteLine("subMeshChunk:");

                    if (ArquivoDeRoom)
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

                        if (ArquivoDeRoom)
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

                    vertexLines[t][i] = array;

                    if (i > 0) // no final do primeiro não tem
                    {
                        // fixo 0x10
                        // EndCommand bytes = 00-00-00-10-00-00-00-00-00-00-00-00-00-00-00-17
                        text.WriteLine("stream.Position: 0x" + stream.Position.ToString("X8"));
                        byte[] EndTagVifCommand = new byte[0x10];
                        stream.Read(EndTagVifCommand, 0, 0x10);
                        text.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] EndTagVifCommand: " + BitConverter.ToString(EndTagVifCommand));
                        EndTagVifCommandText.WriteLine("[" + t.ToString("D2") + "][" + i.ToString("D2") + "] EndTagVifCommand: " + BitConverter.ToString(EndTagVifCommand));
                        text.WriteLine("");
                    }

                }

            }

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
            MTLtext.Close();
            IDXBINtext.Close();

            text.Close();
            stream.Close();

            montaObjTriangulated(diretory + fileName + ".obj", fileName + ".mtl");
        }


        private static void montaObjTriangulated(string nomeArq, string MTLname)
        {
            TextWriter text = new FileInfo(nomeArq).CreateText();
            text.WriteLine("##BINdecoderTest##");
            text.WriteLine("##version A.1.0.0.0##");
            text.WriteLine("mtllib " + MTLname);


            //List<string> indeces = new List<string>();
            int indexGeral = 1;

            for (int t = 0; t < vertexLines.Length; t++)
            {

                //usemtl Material*
                text.WriteLine("g Material" + t);
                text.WriteLine("usemtl Material" + t);

                for (int i = 0; i < vertexLines[t].Length; i++)
                {
                    for (int l = 0; l < vertexLines[t][i].Length; l++)
                    {
                        text.WriteLine("v " + ((float)vertexLines[t][i][l].VerticeX / 1000f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             ((float)vertexLines[t][i][l].VerticeY / 1000f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             ((float)vertexLines[t][i][l].VerticeZ / 1000f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

                        text.WriteLine("vt " + ((float)vertexLines[t][i][l].TextureU / 255f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                        ((float)vertexLines[t][i][l].TextureV / 255f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

                        text.WriteLine("vn " + ((float)vertexLines[t][i][l].NormalX / 127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             ((float)vertexLines[t][i][l].NormalY / 127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             ((float)vertexLines[t][i][l].NormalZ / 127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

                    }


                    bool invFace = false;
                    int contagem = 0;
                    while (contagem < vertexLines[t][i].Length)
                    {
                        text.WriteLine("# [" + t.ToString("D4") + "][" + i.ToString("D4") + "][" + contagem.ToString("D4") + "][" + indexGeral.ToString("D4") + "] IndexMount: " +
                                vertexLines[t][i][contagem].IndexMount.ToString("X4"));

                        string a = (indexGeral - 2).ToString();
                        string b = (indexGeral - 1).ToString();
                        string c = (indexGeral).ToString();

                        if ((contagem - 2) > -1
                            &&
                           !((vertexLines[t][i][contagem].IndexMount == 0 && (vertexLines[t][i][contagem - 1].IndexMount == 1 || vertexLines[t][i][contagem - 1].IndexMount == 0xFFFF))
                           || (vertexLines[t][i][contagem].IndexMount == 0 && (vertexLines[t][i][contagem - 2].IndexMount == 1 || vertexLines[t][i][contagem - 2].IndexMount == 0xFFFF))
                           )
                           )
                        {
                            //indeces.Add("g indices_" + a.PadLeft(3, '0') + "_" + b.PadLeft(3, '0') + "_"+ c.PadLeft(3, '0'));

                            if (invFace)
                            {
                                text.WriteLine("f " + c + "/" + c + "/" + c + " " +
                              b + "/" + b + "/" + b + " " +
                               a + "/" + a + "/" + a
                             );
                                invFace = false;
                            }
                            else 
                            {
                                text.WriteLine("f " + a + "/" + a + "/" + a + " " +
                                  b + "/" + b + "/" + b + " " +
                                   c + "/" + c + "/" + c
                                 );
                                invFace = true;
                            }
                       

                        }
                        else 
                        {
                            invFace = false;
                        }

                        /*
                        if (verticeslines[t][i][contagem].IndexMount != 0)
                        {
                            indeces.Add("f " + a + "/" + a + "/" + a + " " +
                                 b + "/" + b + "/" + b + " " +
                                  c + "/" + c + "/" + c
                                );
                        }
                        */


                        contagem++;
                        indexGeral++;
                    }




                }

            }

            text.Close();

        }


   



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

}