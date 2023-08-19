using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SMD_READER_API;

namespace BINrepackTest
{
     /*
     Codigo feito por JADERLINK
     Pesquisas feitas por HardHain e JaderLink.
     https://www.youtube.com/@JADERLINK
     https://www.youtube.com/@HardRainModder

     Em desenvolvimento
     Para Pesquisas
     13-08-2023
     version: alfa.1.2.0.0
     */

    public static partial class BINrepack
    {
        public static void RepackSMD(string idxbinPath, string smdPath, string binpath)
        {

            StreamReader idxFile = File.OpenText(idxbinPath);

            IdxBin idxBin = IdxBinLoader.Loader(idxFile);

            //---

            StreamReader stream = new FileInfo(smdPath).OpenText();
            SMD_READER_API.SMD smd = SMD_READER_API.SmdReader.Reader(stream);


            Dictionary<string, List<SMD_READER_API.Triangle>> materialGroups = new Dictionary<string, List<SMD_READER_API.Triangle>>();

            for (int i = 0; i < smd.Triangles.Count; i++)
            {
                string materialName = smd.Triangles[i].Material.ToUpperInvariant().Trim();
                if (!materialGroups.ContainsKey(materialName))
                {
                    List<SMD_READER_API.Triangle> list = new List<SMD_READER_API.Triangle>();
                    materialGroups.Add(materialName, list);
                }

                materialGroups[materialName].Add(smd.Triangles[i]);
                //
            }

            //=========
            
            float FarthestVertex = 0;


            IntermediaryStructure intermediary = new IntermediaryStructure();

            foreach (var item in materialGroups)
            {
                IntermediaryGroup group = new IntermediaryGroup();
                group.MaterialName = item.Key;

                for (int i = 0; i < item.Value.Count; i++)
                {
                    IntermediaryTriangle triangle = new IntermediaryTriangle();

                    for (int t = 0; t < item.Value[i].Vertexs.Count; t++)
                    {
                        IntermediaryVertex vertex = new IntermediaryVertex();

                        vertex.PosX = item.Value[i].Vertexs[t].PosX;
                        vertex.PosY = item.Value[i].Vertexs[t].PosZ;
                        vertex.PosZ = item.Value[i].Vertexs[t].PosY * -1;

                        vertex.NormalX = item.Value[i].Vertexs[t].NormX;
                        vertex.NormalY = item.Value[i].Vertexs[t].NormZ;
                        vertex.NormalZ = item.Value[i].Vertexs[t].NormY * -1;

                        vertex.TextureU = item.Value[i].Vertexs[t].U;
                        vertex.TextureV = item.Value[i].Vertexs[t].V;

                        vertex.ColorR = 1f;
                        vertex.ColorG = 1f;
                        vertex.ColorB = 1f;
                        vertex.ColorA = 1f;

                        if (item.Value[i].Vertexs[t].Links.Count == 0)
                        {
                            vertex.links = 1;
                            vertex.BoneID1 = item.Value[i].Vertexs[t].ParentBone;
                            vertex.Weight1 = 1f;
                        }
                        else 
                        {
                            var links = (from link in item.Value[i].Vertexs[t].Links
                                        orderby link.BoneID
                                        select link).ToArray();

                            if (links.Length >= 1)
                            {
                                vertex.links = 1;
                                vertex.BoneID1 = links[0].BoneID;
                                vertex.Weight1 = links[0].Weight;
                            }
                            if (links.Length >= 2)
                            {
                                vertex.links = 2;
                                vertex.BoneID2 = links[1].BoneID;
                                vertex.Weight2 = links[1].Weight;
                            }
                            if (links.Length >= 3)
                            {
                                vertex.links = 3;
                                vertex.BoneID3 = links[2].BoneID;
                                vertex.Weight3 = links[2].Weight;
                            }

                        }

                        triangle.Vertices.Add(vertex);

                        // ---

                        float temp = vertex.PosX;
                        if (temp < 0)
                        {
                            temp *= -1;
                        }
                        if (temp > FarthestVertex)
                        {
                            FarthestVertex = temp;
                        }

                        temp = vertex.PosY;
                        if (temp < 0)
                        {
                            temp *= -1;
                        }
                        if (temp > FarthestVertex)
                        {
                            FarthestVertex = temp;
                        }

                        temp = vertex.PosZ;
                        if (temp < 0)
                        {
                            temp *= -1;
                        }
                        if (temp > FarthestVertex)
                        {
                            FarthestVertex = temp;
                        }

                    }

                    group.Triangles.Add(triangle);
                
                }

                intermediary.Groups.Add(item.Key, group);
            }


            float ConversionFactorValue = FarthestVertex / short.MaxValue * idxBin.GlobalScale;
            if (idxBin.AutoConversionFactor == false && idxBin.ManualConversionFactor != 0)
            {
                ConversionFactorValue = idxBin.ManualConversionFactor;
            }


            FinalStructure finalStructure = new FinalStructure();


            foreach (var item in intermediary.Groups)
            {
                FinalNode node = new FinalNode();
                node.MaterialName = item.Key;

                List<FinalSegment> segments = new List<FinalSegment>();
                List<byte> usedBones = new List<byte>();

                int vertexCount = 0;

                FinalSegment tempSegment = null;

                for (int i = 0; i < item.Value.Triangles.Count;)
                {
                    if (tempSegment == null)
                    {
                        tempSegment = new FinalSegment();
                        segments.Add(tempSegment);
                    }

                    if (vertexCount < 44)
                    {
                        vertexCount += item.Value.Triangles[i].Vertices.Count;

                        ushort mountStatus = 0;
                        for (int v = 0; v < item.Value.Triangles[i].Vertices.Count; v++)
                        {
                            IntermediaryVertex intermediaryVertex = item.Value.Triangles[i].Vertices[v];
                            FinalVertex finalVertex = new FinalVertex();

                            finalVertex.PosX = Utils.ParseFloatToShort((intermediaryVertex.PosX * idxBin.GlobalScale) / ConversionFactorValue);
                            finalVertex.PosY = Utils.ParseFloatToShort((intermediaryVertex.PosY * idxBin.GlobalScale) / ConversionFactorValue);
                            finalVertex.PosZ = Utils.ParseFloatToShort((intermediaryVertex.PosZ * idxBin.GlobalScale) / ConversionFactorValue);

                            finalVertex.NormalX = Utils.ParseFloatToShort(intermediaryVertex.NormalX * 127f);
                            finalVertex.NormalY = Utils.ParseFloatToShort(intermediaryVertex.NormalY * 127f);
                            finalVertex.NormalZ = Utils.ParseFloatToShort(intermediaryVertex.NormalZ * 127f);

                            finalVertex.TextureU = Utils.ParseFloatToShort(intermediaryVertex.TextureU * 255f);
                            finalVertex.TextureV = Utils.ParseFloatToShort(intermediaryVertex.TextureV * 255f);

                            finalVertex.ColorR = Utils.ParseFloatToShort(intermediaryVertex.ColorR * 255);
                            finalVertex.ColorG = Utils.ParseFloatToShort(intermediaryVertex.ColorG * 255);
                            finalVertex.ColorB = Utils.ParseFloatToShort(intermediaryVertex.ColorB * 255);
                            finalVertex.ColorA = Utils.ParseFloatToShort(intermediaryVertex.ColorA * 0x80);

                            if (v >= 2)
                            {
                                if (mountStatus == 0x0000 || mountStatus == 0xFFFF)
                                {
                                    mountStatus = 0x0001;
                                    finalVertex.IndexMount = 0x0001;
                                }
                                else if (mountStatus == 0x0001)
                                {
                                    mountStatus = 0xFFFF;
                                    finalVertex.IndexMount = 0xFFFF;
                                }

                            }

                            int IndexBoneID1 = 0;
                            int IndexBoneID2 = 0;
                            int IndexBoneID3 = 0;


                            if (intermediaryVertex.links >= 1)
                            {
                                if (!usedBones.Contains((byte)intermediaryVertex.BoneID1))
                                {
                                    usedBones.Add((byte)intermediaryVertex.BoneID1);
                                }
                                IndexBoneID1 = usedBones.IndexOf((byte)intermediaryVertex.BoneID1);
                            }

                            if (intermediaryVertex.links >= 2)
                            {
                                if (!usedBones.Contains((byte)intermediaryVertex.BoneID2))
                                {
                                    usedBones.Add((byte)intermediaryVertex.BoneID2);
                                }
                                IndexBoneID2 = usedBones.IndexOf((byte)intermediaryVertex.BoneID2);
                            }

                            if (intermediaryVertex.links >= 3)
                            {
                                if (!usedBones.Contains((byte)intermediaryVertex.BoneID3))
                                {
                                    usedBones.Add((byte)intermediaryVertex.BoneID3);
                                }
                                IndexBoneID3 = usedBones.IndexOf((byte)intermediaryVertex.BoneID3);
                            }


                            FinalWeightMap map = new FinalWeightMap();
                            map.links = intermediaryVertex.links;
                            map.BoneID1 = IndexBoneID1 * 4;
                            map.Weight1 = intermediaryVertex.Weight1;
                            map.BoneID2 = IndexBoneID2 * 4;
                            map.Weight2 = intermediaryVertex.Weight2;
                            map.BoneID3 = IndexBoneID3 * 4;
                            map.Weight3 = intermediaryVertex.Weight3;

                            if (!tempSegment.SubBoneTable.Contains(map))
                            {
                                tempSegment.SubBoneTable.Add(map);


                            }

                            int index = tempSegment.SubBoneTable.IndexOf(map);
                            finalVertex.BoneReference = (ushort)(uint)(index * 2);

                            tempSegment.Vertices.Add(finalVertex);
                            
                        }

                        i++;
                    }
                    else 
                    {
                        tempSegment = null;
                        vertexCount = 0;
                    }

                }


                for (int i = 0; i < segments.Count; i++)
                {
                    for (int l = 2; l < segments[i].Vertices.Count; l++)
                    {
                        if (segments[i].Vertices[l].IndexMount == 0x0000)
                        {
                            segments[i].Vertices[l].IndexComplement = 0x0001;
                        }

                    }
                }


                node.BonesIDs = usedBones.ToArray();
                node.Segments = segments.ToArray();
                finalStructure.Nodes.Add(item.Key, node);
            }




            //-----
            Dictionary<string, byte[]> nodesBytes = new Dictionary<string, byte[]>();

            foreach (var item in finalStructure.Nodes)
            {
                //MakeNodeArray
                nodesBytes.Add(item.Key, MakeNodeArray(item.Value, ConversionFactorValue, idxBin.IsScenarioBin));
            }

            //-----

            MaterialNode[] materialNodes = new MaterialNode[nodesBytes.Count];
            int materialNodesCount = 0;
            foreach (var item in nodesBytes)
            {
                MaterialNode mn = new MaterialNode();
                mn.Name = item.Key;
                mn.NodeData = item.Value;

                byte[] header = new byte[0x10];

                string keyMaterial = "MATERIALLINE?" + item.Key;

                if (idxBin.MaterialLines.ContainsKey(item.Key))
                {
                    Console.WriteLine($"Used material: " + keyMaterial);

                    idxBin.MaterialLines[item.Key].Line.CopyTo(header, 0);
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

            materialNodes = (from ob in materialNodes
                             orderby ob.Name
                             select ob).ToArray();

            //------

            BoneLine[] bones = GetBoneLines(smd, idxBin.GlobalScale);

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
            idxBin.fix3000.CopyTo(binHeader, 0x0);
            idxBin.unknown1.CopyTo(binHeader, 0x2);
            BitConverter.GetBytes(bonesPoint).CopyTo(binHeader, 0x4);
            binHeader[0x8] = idxBin.unknown2;
            binHeader[0x9] = (byte)bones.Length;
            binHeader[0xA] = (byte)materialNodes.Length;
            binHeader[0xB] = idxBin.unknown3;
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
            idxBin.unknown4_unk008.CopyTo(binHeader, 0x20);
            idxBin.unknown4_unk009.CopyTo(binHeader, 0x24);
            idxBin.unknown4_unk010.CopyTo(binHeader, 0x2C);



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
            Stream binfile = File.Create(binpath);

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

            binfile.Close();

        }


        static BoneLine[] GetBoneLines(SMD smd, float globalScale) 
        {
            List<BoneLine> bones = new List<BoneLine>();

            Time time = (from tt in smd.Times
                         where tt.ID == 0
                         select tt).FirstOrDefault();

            for (int i = 0; i < smd.Nodes.Count; i++)
            {

                (float X, float Y, float Z) bonePos = (0, 0, 0);

                if (time != null)
                {
                    Skeleton skeleton = (from ss in time.Skeletons
                                         where ss.BoneID == smd.Nodes[i].ID
                                         select ss).FirstOrDefault();
                    if (skeleton != null)
                    {
                        bonePos.X = skeleton.PosX * globalScale;
                        bonePos.Y = skeleton.PosZ * globalScale;
                        bonePos.Z = skeleton.PosY * -1 * globalScale;
                    }
                }

                bones.Add(new BoneLine((sbyte)smd.Nodes[i].ID, (sbyte)smd.Nodes[i].ParentID, bonePos.X, bonePos.Y, bonePos.Z));
            }

            bones = (from bb in bones
                     orderby bb.BoneId
                     select bb).ToList();

            return bones.ToArray();
        }






    }






}