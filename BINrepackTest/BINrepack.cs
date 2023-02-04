using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ObjLoader.Loader.Loaders;

namespace BINrepackTest
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
    public static class BINrepack
    {

        public static void Repack(string idxbipath, string objpath, string binpath) 
        {

            StreamReader idx = File.OpenText(idxbipath);

            Dictionary<string, string> pair = new Dictionary<string, string>();

            List<string> lines = new List<string>();

            string endLine = "";
            while (endLine != null)
            {
                endLine = idx.ReadLine();
                lines.Add(endLine);
            }

            idx.Close();

            foreach (var item in lines)
            {
                if (item != null)
                {
                    var split = item.Split(new char[] { ':' });
                    if (split.Length >= 2)
                    {
                        string key = split[0].ToUpper().Trim();
                        if (!pair.ContainsKey(key))
                        {
                            pair.Add(key, split[1]);
                        }
                    }
                }
            }

            // load obj
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create(new MaterialNullStreamProvider());

            var fileStream = new System.IO.FileStream(objpath, System.IO.FileMode.Open);
            LoadResult arqObj = objLoader.Load(fileStream);
            fileStream.Close();

            // --------------
          
            Dictionary<string, List<List<refFace>>> IndexLists = new Dictionary<string, List<List<refFace>>>();

            for (int iG = 0; iG < arqObj.Groups.Count; iG++)
            {
                List<List<refFace>> index = new List<List<refFace>>();

                for (int iF = 0; iF < arqObj.Groups[iG].Faces.Count; iF++)
                {
                    List<refFace> subface = new List<refFace>();

                    for (int iI = 0; iI < arqObj.Groups[iG].Faces[iF].Count; iI++)
                    {
                        refFace rf = new refFace();
                        rf.VertexIndex = arqObj.Groups[iG].Faces[iF][iI].VertexIndex;
                        rf.TextureIndex = arqObj.Groups[iG].Faces[iF][iI].TextureIndex;
                        rf.NormalIndex = arqObj.Groups[iG].Faces[iF][iI].NormalIndex;
                        rf.FaceVertexID = iI;
                        rf.FaceID = iF;
                        subface.Add(rf);
                    }

                    if (subface.Count != 0)
                    {
                        index.Add(subface);
                    }

                }

                if (IndexLists.ContainsKey(arqObj.Groups[iG].Name))
                {
                    IndexLists[arqObj.Groups[iG].Name].AddRange(index);
                }
                else 
                {
                    IndexLists.Add(arqObj.Groups[iG].Name, index);
                }

            }

            List<Noded> nodes = new List<Noded>();

            foreach (var facelists in IndexLists)
            {
                var triangles = facelists.Value;

                List<List<VertexLine>> binVerticesLineBase = new List<List<VertexLine>>();

                for (int i = 0; i < triangles.Count; i++)
                {
                    ushort oldIndexMount = 0x0000;

                    List<VertexLine> part = new List<VertexLine>();
                    for (int ip = 0; ip < triangles[i].Count; ip++)
                    {
                        VertexLine vertex = new VertexLine();
                        vertex.VerticeX = ParseFloatToShort(arqObj.Vertices[triangles[i][ip].VertexIndex - 1].X * 1000);
                        vertex.VerticeY = ParseFloatToShort(arqObj.Vertices[triangles[i][ip].VertexIndex - 1].Y * 1000);
                        vertex.VerticeZ = ParseFloatToShort(arqObj.Vertices[triangles[i][ip].VertexIndex - 1].Z * 1000);

                        vertex.NormalX = ParseFloatToShort(arqObj.Normals[triangles[i][ip].NormalIndex - 1].X * 127);
                        vertex.NormalY = ParseFloatToShort(arqObj.Normals[triangles[i][ip].NormalIndex - 1].Y * 127);
                        vertex.NormalZ = ParseFloatToShort(arqObj.Normals[triangles[i][ip].NormalIndex - 1].Z * 127);

                        vertex.TextureU = ParseFloatToShort(arqObj.Textures[triangles[i][ip].TextureIndex - 1].X * 255);
                        vertex.TextureV = ParseFloatToShort(arqObj.Textures[triangles[i][ip].TextureIndex - 1].Y * 255);

                        if (ip == 0 || ip == 1)
                        {
                            vertex.IndexMount = 0x0000;
                        }
                        else
                        {
                            if (oldIndexMount == 0x0000 || oldIndexMount == 0xFFFF)
                            {
                                vertex.IndexMount = 0x0001;
                                oldIndexMount = 0x0001;
                            }
                            else if (oldIndexMount == 0x0001)
                            {
                                vertex.IndexMount = 0xFFFF;
                                oldIndexMount = 0xFFFF;
                            }
                        }

                        part.Add(vertex);

                    }

                    binVerticesLineBase.Add(part);

                }

                // ---------------

                List<List<VertexLine>> subMeshparts = new List<List<VertexLine>>();
                List<VertexLine> tempV = new List<VertexLine>();
                tempV.AddRange(binVerticesLineBase[0]);

                for (int i = 1; i < binVerticesLineBase.Count; i++)
                {
                    if (tempV.Count + binVerticesLineBase[i].Count <= 44)
                    {
                        tempV.AddRange(binVerticesLineBase[i]);
                    }
                    else
                    {
                        subMeshparts.Add(tempV);
                        tempV = new List<VertexLine>();
                        tempV.AddRange(binVerticesLineBase[i]);
                    }
                }

                //ultima parte
                subMeshparts.Add(tempV);


                //-- arruma campo "IndexComplement"

                for (int i = 0; i < subMeshparts.Count; i++)
                {
                    for (int l = 2; l < subMeshparts[i].Count; l++)
                    {
                        if (subMeshparts[i][l].IndexMount == 0x0000)
                        {
                            subMeshparts[i][l].IndexComplement = 0x0001;
                        }

                    }
                }


                // -- final

                Noded noded = new Noded();
                noded.materialname = facelists.Key;
                noded.subMeshparts = subMeshparts;
                nodes.Add(noded);
            }


            nodes = (from no in nodes
                     orderby no.materialname
                     select no).ToList();


            //---------

            bool IsScenarioBin = false;

            if (pair.ContainsKey("ISSCENARIOBIN"))
            {
                try
                {
                    IsScenarioBin = bool.Parse(pair["ISSCENARIOBIN"].Trim());
                }
                catch (Exception)
                {
                }
            }

            byte TopTagVif0x1F = 0x00;

            if (pair.ContainsKey("TOPTAGVIF0X1F"))
            {
                string value = ReturnValidHexValue(pair["TOPTAGVIF0X1F"].ToUpper());
                value = value.PadRight(2, '0');
                value = value.Substring(0, 2);
                TopTagVif0x1F = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                if (IsScenarioBin)
                {
                    TopTagVif0x1F = 0x3E;
                }
                else
                {
                    TopTagVif0x1F = 0x3B;
                }
            }
      

            // nodes header top

            List<byte[]> NodesHeaderTop = new List<byte[]>();

            for (int t = 0; t < nodes.Count; t++)
            {

                byte b2 = (byte)(nodes[t].subMeshparts.Count -1);
                byte b3 = 0;

             
                if (!IsScenarioBin && pair.ContainsKey("NODEHEADERLINEP2_LENGTH_" + t))
                {
                    
                    try
                    {
                        string value = ReturnValidDecValue(pair["NODEHEADERLINEP2_LENGTH_" + t]);
                        b3 = byte.Parse(value, System.Globalization.NumberStyles.Integer);
                    }
                    catch (Exception)
                    {
                        b3 = 1;
                    }

                }
                else if (!IsScenarioBin)
                {
                    b3 = 1;
                }

                byte[] proximosBytes = new byte[b3];

                if (!IsScenarioBin && pair.ContainsKey("NODEHEADERLINEP2_" + t))
                {

                    string value = ReturnValidHexValue(pair["NODEHEADERLINEP2_" + t].ToUpper());
                    value = value.PadRight(b3 * 2, '0');

                    int cont = 0;
                    for (int ipros = 0; ipros < b3; ipros++)
                    {
                        string v = value[cont].ToString() + value[cont + 1].ToString();
                        proximosBytes[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                        cont += 2;
                    }

                }



                //---------------
                int totalBytesNoNodeHeaderArray = 4 + b3;
                int counterNodeHeaderArrayLenght = 0x10;
                while (totalBytesNoNodeHeaderArray > counterNodeHeaderArrayLenght)
                {
                    counterNodeHeaderArrayLenght += 0x10;
                }
                // ----


                ushort lenghtFirtSubMeshB0B1 = 0;
                // --- calculo tamanho

                int caculo = nodes[t].subMeshparts[0].Count * 24;
                int vertexBlocklength = caculo / 0x10;
                float div = caculo % 0x10;
                if (div != 0)
                {
                    vertexBlocklength += 1;
                }

                if (IsScenarioBin)
                {
                    lenghtFirtSubMeshB0B1 = (ushort)(counterNodeHeaderArrayLenght + 0x30 + (vertexBlocklength * 0x10));
                }
                else
                {
                    lenghtFirtSubMeshB0B1 = (ushort)(counterNodeHeaderArrayLenght + 0x10 + 0x20 + 0x30 + (vertexBlocklength * 0x10));
                }

                byte[] lenghtFirtSubMeshB0B1bytes = BitConverter.GetBytes(lenghtFirtSubMeshB0B1);




                byte[] headerFinal = new byte[counterNodeHeaderArrayLenght];
                headerFinal[0] = lenghtFirtSubMeshB0B1bytes[0];//b0;
                headerFinal[1] = lenghtFirtSubMeshB0B1bytes[1];//b1;
                headerFinal[2] = b2;
                headerFinal[3] = b3;
                proximosBytes.CopyTo(headerFinal, 4);
                NodesHeaderTop.Add(headerFinal);


            }


            List<List<byte>> NodesBlocks = new List<List<byte>>();

            for (int t = 0; t < nodes.Count; t++)
            {
                List<byte> nodeBytes = new List<byte>();
                nodeBytes.AddRange(NodesHeaderTop[t]);

                for (int i = 0; i < nodes[t].subMeshparts.Count; i++)
                {
                    if (!IsScenarioBin)
                    {
                        nodeBytes.AddRange(new byte[] {
                        0x02, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x00, 0x80, 0x02, 0x6C, //SubBoneHeader
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 //subBoneTable line 1
                        });
                    }

                    // TopTagVifHeader
                    nodeBytes.AddRange(MakeTopTagVifHeader((byte)nodes[t].subMeshparts[i].Count, TopTagVif0x1F, i == 0));

                    int caculo = nodes[t].subMeshparts[i].Count * 24;
                    int vertexBlocklength = caculo / 0x10;
                    float div = caculo % 0x10;
                    if (div != 0)
                    {
                        vertexBlocklength += 1;
                    }

                    byte[] submeshBlock = new byte[vertexBlocklength * 0x10];

                    int count = 0;
                    for (int l = 0; l < nodes[t].subMeshparts[i].Count; l++)
                    {
                        byte[] line = new byte[24];

                        var VerticeX = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].VerticeX);
                        var VerticeY = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].VerticeY);
                        var VerticeZ = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].VerticeZ);

                        line[0] = VerticeX[0];
                        line[1] = VerticeX[1];

                        line[2] = VerticeY[0];
                        line[3] = VerticeY[1];

                        line[4] = VerticeZ[0];
                        line[5] = VerticeZ[1];

                        var IndexComplement = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].IndexComplement);

                        line[0XE] = IndexComplement[0];
                        line[0XF] = IndexComplement[1];

                        var NormalX = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].NormalX);
                        var NormalY = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].NormalY);
                        var NormalZ = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].NormalZ);

                        var TextureU = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].TextureU);
                        var TextureV = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].TextureV);

                        var IndexMount = BitConverter.GetBytes(nodes[t].subMeshparts[i][l].IndexMount);

                        // -----------

                        if (IsScenarioBin)
                        {

                            line[0X6] = IndexMount[0];
                            line[0X7] = IndexMount[1];

                            line[0X8] = TextureU[0];
                            line[0X9] = TextureU[1];

                            line[0XA] = TextureV[0];
                            line[0XB] = TextureV[1];

                            line[0XC] = 0X01;
                            line[0XD] = 0X00;

                            line[0X10] = 255;//NormalX[0];
                            line[0X11] = 0;//NormalX[1];

                            line[0X12] = 255;//NormalY[0];
                            line[0X13] = 0;// NormalY[1];

                            line[0X14] = 255;// NormalZ[0];
                            line[0X15] = 0;// NormalZ[1];

                            line[0X16] = 0X80;
                            line[0X17] = 0X00;
                        }
                        else 
                        {
                            line[0X8] = NormalX[0];
                            line[0X9] = NormalX[1];

                            line[0XA] = NormalY[0];
                            line[0XB] = NormalY[1];

                            line[0XC] = NormalZ[0];
                            line[0XD] = NormalZ[1];

                            line[0X10] = TextureU[0];
                            line[0X11] = TextureU[1];

                            line[0X12] = TextureV[0];
                            line[0X13] = TextureV[1];

                            line[0X14] = 0X01;
                            line[0X15] = 0X00;

                            line[0X16] = IndexMount[0];
                            line[0X17] = IndexMount[1];

                        }

                        line.CopyTo(submeshBlock, count);
                        count += 24;
                    }

                    nodeBytes.AddRange(submeshBlock);


                    if (i != 0)
                    {
                        nodeBytes.AddRange(MakeEndTagVifCommand(i == nodes[t].subMeshparts.Count -1));
                    }
                }

                NodesBlocks.Add(nodeBytes);
            }


            // ORDEM
            // mainHeader 16 bytes
            // unknown4 bytes 64 ou variavel
            // bones
            // materials
            // nodes

            // mainheader
            byte[] Fix3000 = new byte[2] {0x30, 0x00 };
            byte[] unknown1 = new byte[2];
            int bonesPoint = 0;
            byte unknown2 = 0x0;
            byte BonesCount = 0;
            byte MaterialCount = (byte)NodesBlocks.Count;
            byte unknown3 = 0;
            int MaterialsPoint = 0;
            byte[] unknown4 = new byte[0];

            List<byte[]> Bones = new List<byte[]>();
            List<byte[]> materials = new List<byte[]>();

            if (pair.ContainsKey("FIX3000"))
            {
                string value = ReturnValidHexValue(pair["FIX3000"].ToUpper());
                value = value.PadRight(4, '0');
                string bA = value.Substring(0, 2);
                string bB = value.Substring(2, 2);
                Fix3000[0] = byte.Parse(bA, System.Globalization.NumberStyles.HexNumber);
                Fix3000[1] = byte.Parse(bB, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("UNKNOWN1"))
            {
                string value = ReturnValidHexValue(pair["UNKNOWN1"].ToUpper());
                value = value.PadRight(4, '0');
                string bA = value.Substring(0, 2);
                string bB = value.Substring(2, 2);
                unknown1[0] = byte.Parse(bA, System.Globalization.NumberStyles.HexNumber);
                unknown1[1] = byte.Parse(bB, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("UNKNOWN2"))
            {
                string value = ReturnValidHexValue(pair["UNKNOWN2"].ToUpper());
                value = value.PadRight(2, '0');
                value = value.Substring(0, 2);
                unknown2 = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("UNKNOWN3"))
            {
                string value = ReturnValidHexValue(pair["UNKNOWN3"].ToUpper());
                value = value.PadRight(2, '0');
                value = value.Substring(0, 2);
                unknown3 = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
            }

            if (pair.ContainsKey("BONESCOUNT"))
            {

                try
                {
                    string value = ReturnValidDecValue(pair["BONESCOUNT"]);
                    BonesCount = byte.Parse(value, System.Globalization.NumberStyles.Integer);
                }
                catch (Exception)
                {
                }

            }

            int unknown4Lenght = 0x40;

            if (pair.ContainsKey("UNKNOWN4LENGTH"))
            {

                try
                {
                    string value = ReturnValidDecValue(pair["UNKNOWN4LENGTH"]);
                    unknown4Lenght = byte.Parse(value, System.Globalization.NumberStyles.Integer);
                }
                catch (Exception)
                {
                }

            }

            unknown4 = new byte[unknown4Lenght];

            if (pair.ContainsKey("UNKNOWN4"))
            {

                string value = ReturnValidHexValue(pair["UNKNOWN4"].ToUpper());
                value = value.PadRight(unknown4Lenght * 2, '0');

                int cont = 0;
                for (int ipros = 0; ipros < unknown4Lenght; ipros++)
                {
                    string v = value[cont].ToString() + value[cont + 1].ToString();
                    unknown4[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                    cont += 2;
                }

            }

            for (int i = 0; i < BonesCount; i++)
            {
                byte[] boneLine = new byte[0x10];

                if (pair.ContainsKey("BONELINE_" + i))
                {

                    string value = ReturnValidHexValue(pair["BONELINE_" + i].ToUpper());
                    value = value.PadRight(0x10 * 2, '0');

                    int cont = 0;
                    for (int ipros = 0; ipros < boneLine.Length; ipros++)
                    {
                        string v = value[cont].ToString() + value[cont + 1].ToString();
                        boneLine[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                        cont += 2;
                    }

                }

                Bones.Add(boneLine);
            }

            for (int i = 0; i < NodesBlocks.Count; i++)
            {
                //MATERIALLINE_0
                byte[] materialLine = new byte[0x10];

                if (pair.ContainsKey("MATERIALLINE_" + i))
                {

                    string value = ReturnValidHexValue(pair["MATERIALLINE_" + i].ToUpper());
                    value = value.PadRight(0x10 * 2, '0');

                    int cont = 0;
                    for (int ipros = 0; ipros < materialLine.Length; ipros++)
                    {
                        string v = value[cont].ToString() + value[cont + 1].ToString();
                        materialLine[ipros] = byte.Parse(v, System.Globalization.NumberStyles.HexNumber);
                        cont += 2;
                    }

                }
                else 
                {
                    materialLine = new byte[0x10] { 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00 };
                }

                materials.Add(materialLine);
            }

            // calculos de offset;

            bonesPoint = 0x10 + unknown4Lenght;
            MaterialsPoint = bonesPoint + (BonesCount * 0x10);

            int firtNodePoint = MaterialsPoint + (NodesBlocks.Count * 0x10);
            byte[] firtNodePointBytes = BitConverter.GetBytes(firtNodePoint);
            firtNodePointBytes.CopyTo(materials[0], 12); //0x10-4

            int tempOffset = firtNodePoint;
            for (int i = 1; i < NodesBlocks.Count; i++)
            {
                tempOffset = tempOffset + NodesBlocks[i - 1].Count;
                byte[] anyNodePointBytes = BitConverter.GetBytes(tempOffset);
                anyNodePointBytes.CopyTo(materials[i], 12); //0x10-4
            }


            //------------------

            Stream binfile = File.Create(binpath);

            binfile.Write(Fix3000, 0, Fix3000.Length);
            binfile.Write(unknown1, 0, unknown1.Length);

            byte[] bonesPointBytes = BitConverter.GetBytes(bonesPoint);
            binfile.Write(bonesPointBytes, 0, bonesPointBytes.Length);

            binfile.WriteByte(unknown2);
            binfile.WriteByte(BonesCount);
            binfile.WriteByte(MaterialCount);
            binfile.WriteByte(unknown3);

            byte[] MaterialsPointBytes = BitConverter.GetBytes(MaterialsPoint);
            binfile.Write(MaterialsPointBytes, 0, MaterialsPointBytes.Length);

            binfile.Write(unknown4, 0, unknown4.Length);

            for (int i = 0; i < Bones.Count; i++)
            {
                binfile.Write(Bones[i], 0, Bones[i].Length);          
            }

            for (int i = 0; i < materials.Count; i++)
            {
                binfile.Write(materials[i], 0, materials[i].Length);
            }

            for (int i = 0; i < NodesBlocks.Count; i++)
            {
                binfile.Write(NodesBlocks[i].ToArray(), 0, NodesBlocks[i].Count);
            }


            binfile.Close();
        }


        static byte[] MakeTopTagVifHeader(byte vertexlength, byte TopTagVif0x1F, bool isFirt)
        {
            int caculo = vertexlength * 24;
            int vertexBlocklength = caculo / 0x10;
            float div = caculo % 0x10;
            if (div != 0)
            {
                vertexBlocklength += 1;
            }

            int vertexBlocklength8 = (vertexBlocklength * 0x10) / 8;

            byte firt = 0x10;
            if (isFirt)
            {
                firt = 0x60;
            }

            //vertexlength in 0x10

            byte[] res = new byte[0x30] {
            0x01, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x20, 0x80, 0x01, 0x6C, vertexlength, 0x80, 0x00, 0x00, 0x00, 0x40, 0x2E, 0x30, 0x12, 0x05, 0x00, 0x00, 0x00, 0x00, 0x80, TopTagVif0x1F, (byte)vertexBlocklength, 0x00, 0x00, firt, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x21, 0x80, (byte)vertexBlocklength8, 0x6D
            };
    
            return res;
        }

        static byte[] MakeEndTagVifCommand(bool isEndEntry) 
        {
            byte EndEntry = 0x10;
            if (isEndEntry)
            {
                EndEntry = 0x60;
            }

            byte[] res = new byte[0x10]
            {
                0x00, 0x00, 0x00, EndEntry, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17
            };
            return res;
        }


        static string ReturnValidHexValue(string cont) 
        {
            string res = "";
            foreach (var c in cont)
            {
                if (char.IsDigit(c)
                    || c == 'A'
                    || c == 'B'
                    || c == 'C'
                    || c == 'D'
                    || c == 'E'
                    || c == 'F'
                    )
                {
                    res += c;
                }
            }
            return res;
        }

        static string ReturnValidDecValue(string cont)
        {
            string res = "";
            foreach (var c in cont)
            {
                if (char.IsDigit(c))
                {
                    res += c;
                }
            }
            return res;
        }


        static short ParseFloatToShort(float value) 
        {
            string sv = value.ToString("F", System.Globalization.CultureInfo.InvariantCulture).Split('.')[0];
            int iv = 0;
            try
            {
                iv = int.Parse(sv, System.Globalization.NumberStyles.Integer);
            }
            catch (Exception)
            {
            }
            if (iv > short.MaxValue)
            {
                iv = short.MaxValue;
            }
            else if (iv < short.MinValue)
            {
                iv = short.MinValue;
            }
            return (short)iv;
        }

    }


    public struct refFace 
    {
        public int VertexIndex;
        public int TextureIndex;
        public int NormalIndex;
        public int FaceVertexID;
        public int FaceID;
    }

    public class VertexLine
    {
        // linha total
        //public byte[] line;

        public short VerticeX = 0;
        public short VerticeY = 0;
        public short VerticeZ = 0;

        //public ushort UnknownB = 0;

        public short NormalX = 0;
        public short NormalY = 0;
        public short NormalZ = 0;

        public ushort IndexComplement = 0;

        public short TextureU = 0;
        public short TextureV = 0;

        //public ushort UnknownA = 0

        public ushort IndexMount = 0;
    }

    public class Noded 
    {
        public string materialname = null;
        public List<List<VertexLine>> subMeshparts = new List<List<VertexLine>>();

    }

}
