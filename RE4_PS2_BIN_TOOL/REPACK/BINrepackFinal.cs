using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_BIN_TOOL.REPACK
{
    public static class BINrepackFinal
    {
        public static FinalStructure MakeFinalStructure(IntermediaryStructure intermediaryStructure, float ConversionFactorValue, float GlobalScale)
        {
            FinalStructure finalStructure = new FinalStructure();

            foreach (var item in intermediaryStructure.Groups)
            {
                FinalNode node = new FinalNode();
                node.MaterialName = item.Key;

                List<IntermediaryFace> FacesList = new List<IntermediaryFace>();
                FacesList.AddRange(item.Value.Faces);
                FacesList = FacesList.OrderByDescending(o => o.Vertexs.Count).ToList();

                List<FinalSegment> segments = new List<FinalSegment>();
                List<byte> usedBones = new List<byte>();

                int vertexCount = 0;

                FinalSegment tempSegment = null;
                List<IntermediaryWeightMap> IntermediaryWeightMapList = new List<IntermediaryWeightMap>();


                while (FacesList.Count != 0)
                {
                    if (tempSegment == null)
                    {
                        tempSegment = new FinalSegment();
                        IntermediaryWeightMapList.Clear();
                        segments.Add(tempSegment);
                    }

                    if (vertexCount <= 44)
                    {
                        IntermediaryFace intermediaryFace = null;
                        if (vertexCount + FacesList[0].Vertexs.Count <= 44 && CreateNewIntermediaryWeightMapList(IntermediaryWeightMapList, FacesList[0].WeightMapOnFace).Count() <= 15)
                        {
                            intermediaryFace = FacesList[0];
                            FacesList.RemoveAt(0);
                            vertexCount += intermediaryFace.Vertexs.Count;
                            IntermediaryWeightMapList = CreateNewIntermediaryWeightMapList(IntermediaryWeightMapList, intermediaryFace.WeightMapOnFace).ToList();
                        }
                        else
                        {
                            bool ok = false;
                            int iI = 1;
                            if (FacesList.Count > 1)
                            {
                                while (!ok)
                                {
                                    if (iI < FacesList.Count)
                                    {
                                        if (vertexCount + FacesList[iI].Vertexs.Count <= 44 && CreateNewIntermediaryWeightMapList(IntermediaryWeightMapList, FacesList[iI].WeightMapOnFace).Count() <= 15)
                                        {
                                            intermediaryFace = FacesList[iI];
                                            FacesList.RemoveAt(iI);
                                            vertexCount += intermediaryFace.Vertexs.Count;
                                            IntermediaryWeightMapList = CreateNewIntermediaryWeightMapList(IntermediaryWeightMapList, intermediaryFace.WeightMapOnFace).ToList();
                                            ok = true;
                                        }
                                        iI++;
                                    }
                                    else
                                    {
                                        vertexCount = int.MaxValue;
                                        ok = true;
                                        continue;
                                    }

                                }

                                if (intermediaryFace == null)
                                {
                                    vertexCount = int.MaxValue;
                                    continue;
                                }
                            }
                            else
                            {
                                vertexCount = int.MaxValue;
                                continue;
                            }

                        }

                        if (intermediaryFace != null)
                        {

                            ushort mountStatus = 0;
                            for (int v = 0; v < intermediaryFace.Vertexs.Count; v++)
                            {
                                IntermediaryVertex intermediaryVertex = intermediaryFace.Vertexs[v];
                                FinalVertex finalVertex = new FinalVertex();

                                finalVertex.PosX = Utils.ParseFloatToShort((intermediaryVertex.PosX * GlobalScale) / ConversionFactorValue);
                                finalVertex.PosY = Utils.ParseFloatToShort((intermediaryVertex.PosY * GlobalScale) / ConversionFactorValue);
                                finalVertex.PosZ = Utils.ParseFloatToShort((intermediaryVertex.PosZ * GlobalScale) / ConversionFactorValue);

                                finalVertex.NormalX = Utils.ParseFloatToShort(intermediaryVertex.NormalX * 127f);
                                finalVertex.NormalY = Utils.ParseFloatToShort(intermediaryVertex.NormalY * 127f);
                                finalVertex.NormalZ = Utils.ParseFloatToShort(intermediaryVertex.NormalZ * 127f);

                                finalVertex.TextureU = Utils.ParseFloatToShort(intermediaryVertex.TextureU * 255f);
                                finalVertex.TextureV = Utils.ParseFloatToShort(intermediaryVertex.TextureV * 255f);

                                finalVertex.ColorR = Utils.ParseFloatToShort(intermediaryVertex.ColorR * 0x80);
                                finalVertex.ColorG = Utils.ParseFloatToShort(intermediaryVertex.ColorG * 0x80);
                                finalVertex.ColorB = Utils.ParseFloatToShort(intermediaryVertex.ColorB * 0x80);
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


                                if (intermediaryVertex.Links >= 1)
                                {
                                    if (!usedBones.Contains((byte)intermediaryVertex.BoneID1))
                                    {
                                        usedBones.Add((byte)intermediaryVertex.BoneID1);
                                    }
                                    IndexBoneID1 = usedBones.IndexOf((byte)intermediaryVertex.BoneID1);
                                }

                                if (intermediaryVertex.Links >= 2)
                                {
                                    if (!usedBones.Contains((byte)intermediaryVertex.BoneID2))
                                    {
                                        usedBones.Add((byte)intermediaryVertex.BoneID2);
                                    }
                                    IndexBoneID2 = usedBones.IndexOf((byte)intermediaryVertex.BoneID2);
                                }

                                if (intermediaryVertex.Links >= 3)
                                {
                                    if (!usedBones.Contains((byte)intermediaryVertex.BoneID3))
                                    {
                                        usedBones.Add((byte)intermediaryVertex.BoneID3);
                                    }
                                    IndexBoneID3 = usedBones.IndexOf((byte)intermediaryVertex.BoneID3);
                                }


                                FinalWeightMap map = new FinalWeightMap();
                                map.Links = intermediaryVertex.Links;
                                map.BoneID1 = IndexBoneID1 * 4;
                                map.Weight1 = intermediaryVertex.Weight1;
                                map.BoneID2 = IndexBoneID2 * 4;
                                map.Weight2 = intermediaryVertex.Weight2;
                                map.BoneID3 = IndexBoneID3 * 4;
                                map.Weight3 = intermediaryVertex.Weight3;

                                if (!tempSegment.WeightMapList.Contains(map))
                                {
                                    tempSegment.WeightMapList.Add(map);
                                }

                                int index = tempSegment.WeightMapList.IndexOf(map);
                                finalVertex.WeightMapReference = (ushort)(uint)(index * 2);

                                tempSegment.VertexList.Add(finalVertex);

                            }

                        }


                    }
                    else
                    {
                        tempSegment = null;
                        IntermediaryWeightMapList.Clear();
                        vertexCount = 0;
                    }

                }


                for (int i = 0; i < segments.Count; i++)
                {
                    for (int l = 2; l < segments[i].VertexList.Count; l++)
                    {
                        if (segments[i].VertexList[l].IndexMount == 0x0000)
                        {
                            var vertex = segments[i].VertexList[l];
                            vertex.IndexComplement = 0x0001;
                            segments[i].VertexList[l] = vertex;
                        }

                    }
                }


                node.BonesIDs = usedBones.ToArray();
                node.Segments = segments.ToArray();
                finalStructure.Nodes.Add(item.Key, node);
            }

            return finalStructure;
        }

        private static IEnumerable<IntermediaryWeightMap> CreateNewIntermediaryWeightMapList(IEnumerable<IntermediaryWeightMap> List, IEnumerable<IntermediaryWeightMap> ToAdd)
        {
            List<IntermediaryWeightMap> res = new List<IntermediaryWeightMap>();
            res.AddRange(List);
            foreach (var item in ToAdd)
            {
                if (!res.Contains(item))
                {
                    res.Add(item);
                }
            }
            return res;
        }

    }
}
