using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ObjLoader.Loader.Loaders;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_BIN_TOOL.REPACK
{
    /*
     Codigo feito por JADERLINK
     Pesquisas feitas por HardHain e JaderLink.
     https://www.youtube.com/@JADERLINK
     https://www.youtube.com/@HardRainModder
     */

    public static class BINmakeFile
    {
        private static byte[] MakeBonePairArry(BonePairLine[] bonePairLines) 
        {
            if (bonePairLines.Length != 0)
            {
                int count = bonePairLines.Length;
                int calculation = (count * 8) + 4;
                int parts = calculation / 0x10;
                int div = calculation % 0x10;
                if (div != 0)
                {
                    parts += 1;
                }

                byte[] res = new byte[parts * 0x10];
                BitConverter.GetBytes(count).CopyTo(res, 0);
                int offset = 4;
                for (int i = 0; i < bonePairLines.Length; i++)
                {
                    bonePairLines[i].Line.CopyTo(res, offset);
                    offset += 8;
                }
                return res;
            }

            return new byte[0];
        }

        private static byte[] MakeNodeArray(FinalNode finalNode, float ConversionFactorValue, bool IsScenarioBin) 
        {
            List<byte> res = new List<byte>();

            // node header top

            byte b2 = (byte)(finalNode.Segments.Length - 1);
            if (finalNode.Segments.Length > 256)
            {
                b2 = 255;
            }

            byte b3 = (byte)finalNode.BonesIDs.Length;
            byte[] bonesIDs = finalNode.BonesIDs;

            if (IsScenarioBin)
            {
                b3 = 0;
                bonesIDs = new byte[0];
            }

            //---------------
            int totalOfBytesInNodeHeaderArray = 4 + b3;
            int counterNodeHeaderArrayLenght = 0x10;
            while (totalOfBytesInNodeHeaderArray > counterNodeHeaderArrayLenght)
            {
                counterNodeHeaderArrayLenght += 0x10;
            }
            // ----


            ushort lenghtFirtSubMeshB0B1;
            // --- calculo tamanho

            int calculation0 = finalNode.Segments[0].VertexList.Count * 24;
            int vertexBlockParts0 = calculation0 / 0x10;
            float div0 = calculation0 % 0x10;
            if (div0 != 0)
            {
                vertexBlockParts0 += 1;
            }

            if (IsScenarioBin)
            {
                lenghtFirtSubMeshB0B1 = (ushort)(counterNodeHeaderArrayLenght + 0x30 + (vertexBlockParts0 * 0x10));
            }
            else
            {
                int weightBlock = finalNode.Segments[0].WeightMapList.Count * 0x20;
                lenghtFirtSubMeshB0B1 = (ushort)(counterNodeHeaderArrayLenght + 0x10 + weightBlock + 0x30 + (vertexBlockParts0 * 0x10));
            }

            byte[] lenghtFirtSubMeshB0B1bytes = BitConverter.GetBytes(lenghtFirtSubMeshB0B1);


            byte[] headerFinal = new byte[counterNodeHeaderArrayLenght];
            headerFinal[0] = lenghtFirtSubMeshB0B1bytes[0];//b0;
            headerFinal[1] = lenghtFirtSubMeshB0B1bytes[1];//b1;
            headerFinal[2] = b2;
            headerFinal[3] = b3;
            bonesIDs.CopyTo(headerFinal, 4);
            res.AddRange(headerFinal);

            // end node header top


            // adiciona os segments

            for (int i = 0; i < finalNode.Segments.Length; i++)
            {
                if (!IsScenarioBin)
                {
                    int weightBlock = finalNode.Segments[i].WeightMapList.Count * 0x20;
                    byte weightBlockLength = (byte)(weightBlock / 0x10);

                    res.AddRange(new byte[] {
                        weightBlockLength, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x00, 0x80, weightBlockLength, 0x6C //WeightMapHeader
                        });

                    for (int iw = 0; iw < finalNode.Segments[i].WeightMapList.Count; iw++)
                    {
                        res.AddRange(MakeSubBoneTable(finalNode.Segments[i].WeightMapList[iw], finalNode.Segments[i].WeightMapList.Count));
                    }
                }


                // TopTagVifHeader
                res.AddRange(MakeTopTagVifHeader((byte)finalNode.Segments[i].VertexList.Count, ConversionFactorValue, i == 0));

                int calculationV = finalNode.Segments[i].VertexList.Count * 24;
                int vertexBlockParts = calculationV / 0x10;
                float div = calculationV % 0x10;
                if (div != 0)
                {
                    vertexBlockParts += 1;
                }

                byte[] submeshBlock = new byte[vertexBlockParts * 0x10];

                int count = 0;
                for (int l = 0; l < finalNode.Segments[i].VertexList.Count; l++)
                {
                    byte[] line = new byte[24];

                    var VerticeX = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].PosX);
                    var VerticeY = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].PosY);
                    var VerticeZ = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].PosZ);

                    line[0] = VerticeX[0];
                    line[1] = VerticeX[1];

                    line[2] = VerticeY[0];
                    line[3] = VerticeY[1];

                    line[4] = VerticeZ[0];
                    line[5] = VerticeZ[1];

                    var IndexComplement = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].IndexComplement);

                    line[0XE] = IndexComplement[0];
                    line[0XF] = IndexComplement[1];

                   
                    var TextureU = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].TextureU);
                    var TextureV = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].TextureV);

                    var IndexMount = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].IndexMount);
                    
                  
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

                        var ColorR = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].ColorR);
                        var ColorG = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].ColorG);
                        var ColorB = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].ColorB);
                        var ColorA = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].ColorA);

                        line[0X10] = ColorR[0];
                        line[0X11] = ColorR[1];

                        line[0X12] = ColorG[0];
                        line[0X13] = ColorG[1];

                        line[0X14] = ColorB[0];
                        line[0X15] = ColorB[1];

                        line[0X16] = ColorA[0];//0X80;
                        line[0X17] = ColorA[1];//0X00;
                    }
                    else
                    {
                        var WeightMapReference = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].WeightMapReference);

                        var NormalX = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].NormalX);
                        var NormalY = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].NormalY);
                        var NormalZ = BitConverter.GetBytes(finalNode.Segments[i].VertexList[l].NormalZ);

                        line[0X6] = WeightMapReference[0];
                        line[0X7] = WeightMapReference[1];

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

                res.AddRange(submeshBlock);


                if (i != 0)
                {
                    res.AddRange(MakeEndTagVifCommand(i == finalNode.Segments.Length - 1));
                }
            }


            return res.ToArray();
        }

        private static byte[] MakeSubBoneTable(FinalWeightMap finalWeightMap, int totalAmount)
        {
            byte[] res = new byte[0x20];
            BitConverter.GetBytes(finalWeightMap.BoneID1).CopyTo(res, 0x00);
            BitConverter.GetBytes(finalWeightMap.BoneID2).CopyTo(res, 0x04);
            BitConverter.GetBytes(finalWeightMap.BoneID3).CopyTo(res, 0x08);
            BitConverter.GetBytes(finalWeightMap.Links).CopyTo(res, 0x0C);
            BitConverter.GetBytes(finalWeightMap.Weight1).CopyTo(res, 0x010);
            BitConverter.GetBytes(finalWeightMap.Weight2).CopyTo(res, 0x014);
            BitConverter.GetBytes(finalWeightMap.Weight3).CopyTo(res, 0x018);
            BitConverter.GetBytes(totalAmount).CopyTo(res, 0x01C);
            return res;
        }

        private static byte[] MakeTopTagVifHeader(byte vertexAmount, float ConversionFactorValue, bool isFirst)
        {
            int calculation = vertexAmount * 24;
            int vertexBlockParts = calculation / 0x10;
            float div = calculation % 0x10;
            if (div != 0)
            {
                vertexBlockParts += 1;
            }

            int vertexBlockParts8 = (vertexBlockParts * 0x10) / 8;

            byte first = 0x10;
            if (isFirst)
            {
                first = 0x60;
            }

            byte[] factor = BitConverter.GetBytes(ConversionFactorValue);


            byte[] res = new byte[0x30] {
            0x01, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x20, 0x80, 0x01, 0x6C,
            vertexAmount, 0x80, 0x00, 0x00, 0x00, 0x40, 0x2E, 0x30, 0x12, 0x05, 0x00, 0x00, factor[0], factor[1], factor[2], factor[3],
            (byte)vertexBlockParts, 0x00, 0x00, first, 0x00, 0x00, 0x00, 0x00, 0x04, 0x04, 0x00, 0x01, 0x21, 0x80, (byte)vertexBlockParts8, 0x6D
            };
    
            return res;
        }

        private static byte[] MakeEndTagVifCommand(bool isEndEntry) 
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

        public static void MakeFinalBinFile(Stream binfile, long startOffset, out long endOffset, FinalStructure finalStructure, IdxBin idxBin, BoneLine[] bones, float ConversionFactorValue, IdxMaterial material) 
        {

            Dictionary<string, byte[]> nodesBytes = new Dictionary<string, byte[]>();

            foreach (var item in finalStructure.Nodes)
            {
                //MakeNodeArray
                nodesBytes.Add(item.Key, MakeNodeArray(item.Value, ConversionFactorValue, idxBin.IsScenarioBin));
            }

            nodesBytes = (from ob in nodesBytes
                          orderby ob.Key
                          select ob).ToDictionary(k => k.Key, v => v.Value);

            //-----

            MaterialNode[] materialNodes = new MaterialNode[nodesBytes.Count];
            int materialNodesCount = 0;
            foreach (var item in nodesBytes)
            {
                MaterialNode mn = new MaterialNode();
                mn.Name = item.Key;
                mn.NodeData = item.Value;

                byte[] header = new byte[0x10];

                string keyMaterial = item.Key.ToUpperInvariant();

                if (material.MaterialDic.ContainsKey(keyMaterial))
                {
                    Console.WriteLine($"Used material: " + keyMaterial);

                    material.MaterialDic[keyMaterial].GetArray().CopyTo(header, 0);
                }
                else
                {
                    Console.WriteLine($"Not found material: " + keyMaterial);

                    header[2] = 0xFF;
                    header[3] = 0xFF;
                    header[4] = 0xFF;
                    header[11] = 0xFF;
                }
                mn.MaterialHeader = header;
                materialNodes[materialNodesCount] = mn;
                materialNodesCount++;
            }

            //------

            byte[] bonepairArray = MakeBonePairArry(idxBin.BonePairLines);

            // ---


            // calculos de offset;

            int bonesPoint = 0x50;
            int bonepairPoint = 0x0;
            int MaterialsPoint = bonesPoint + (bones.Length * 0x10);

            if (bonepairArray.Length != 0)
            {
                bonepairPoint = bonesPoint + (bones.Length * 0x10);
                MaterialsPoint = bonepairPoint + bonepairArray.Length;
            }

            int firtNodePoint = MaterialsPoint + (materialNodes.Length * 0x10);
            byte[] firtNodePointBytes = BitConverter.GetBytes(firtNodePoint);
            firtNodePointBytes.CopyTo(materialNodes[0].MaterialHeader, 12); //0x10-4

            int tempOffset = firtNodePoint;
            for (int i = 1; i < materialNodes.Length; i++)
            {
                tempOffset = tempOffset + materialNodes[i - 1].NodeData.Length;
                byte[] anyNodePointBytes = BitConverter.GetBytes(tempOffset);
                anyNodePointBytes.CopyTo(materialNodes[i].MaterialHeader, 12); //0x10-4
            }

            //----

            byte[] binHeader = new byte[0x50];
            binHeader[0x0] = 0x30;
            binHeader[0x1] = 0x00;
            idxBin.unknown1.CopyTo(binHeader, 0x2);
            BitConverter.GetBytes(bonesPoint).CopyTo(binHeader, 0x4);
            binHeader[0x8] = idxBin.unknown2;
            binHeader[0x9] = (byte)bones.Length;
            BitConverter.GetBytes((ushort)materialNodes.Length).CopyTo(binHeader, 0xA);
            BitConverter.GetBytes(MaterialsPoint).CopyTo(binHeader, 0xC);
            //unknown4
            binHeader[0x10] = 0xCD;
            binHeader[0x11] = 0xCD;
            binHeader[0x12] = 0xCD;
            binHeader[0x13] = 0xCD;
            binHeader[0x14] = 0xCD;
            binHeader[0x15] = 0xCD;
            binHeader[0x16] = 0xCD;
            binHeader[0x17] = 0xCD;

            idxBin.unknown4_B.CopyTo(binHeader, 0x18);
            BitConverter.GetBytes(bonepairPoint).CopyTo(binHeader, 0x1C);
            idxBin.unknown4_unk009.CopyTo(binHeader, 0x24);

            binHeader[0x28] = 0x30;

            BitConverter.GetBytes(idxBin.DrawDistanceNegativeX).CopyTo(binHeader, 0x30);
            BitConverter.GetBytes(idxBin.DrawDistanceNegativeY).CopyTo(binHeader, 0x34);
            BitConverter.GetBytes(idxBin.DrawDistanceNegativeZ).CopyTo(binHeader, 0x38);
            BitConverter.GetBytes(idxBin.DrawDistanceNegativePadding).CopyTo(binHeader, 0x3C);
            BitConverter.GetBytes(idxBin.DrawDistancePositiveX).CopyTo(binHeader, 0x40);
            BitConverter.GetBytes(idxBin.DrawDistancePositiveY).CopyTo(binHeader, 0x44);
            BitConverter.GetBytes(idxBin.DrawDistancePositiveZ).CopyTo(binHeader, 0x48);

            binHeader[0x4C] = 0xCD;
            binHeader[0x4D] = 0xCD;
            binHeader[0x4E] = 0xCD;
            binHeader[0x4F] = 0xCD;


            //---
            binfile.Position = startOffset;

            binfile.Write(binHeader, 0, binHeader.Length);


            for (int i = 0; i < bones.Length; i++)
            {
                binfile.Write(bones[i].Line, 0, bones[i].Line.Length);
            }

            if (bonepairArray.Length != 0)
            {
                binfile.Write(bonepairArray, 0, bonepairArray.Length);
            }

            for (int i = 0; i < materialNodes.Length; i++)
            {
                binfile.Write(materialNodes[i].MaterialHeader, 0, materialNodes[i].MaterialHeader.Length);
            }

            for (int i = 0; i < materialNodes.Length; i++)
            {
                binfile.Write(materialNodes[i].NodeData, 0, materialNodes[i].NodeData.Length);
            }

            endOffset = binfile.Position;
        }

        private class MaterialNode
        {
            public string Name;
            public byte[] MaterialHeader;
            public byte[] NodeData;
        }
    }


}
