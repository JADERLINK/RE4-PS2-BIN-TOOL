﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//using SMD_READER_API;

namespace BINrepackTest
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

    public static partial class BINrepack
    {
        public static void RepackSMD(string idxbinPath, string smdPath, string binpath)
        {

            // carrega o arquivo .idxBin
            StreamReader idxFile = File.OpenText(idxbinPath);
            IdxBin idxBin = IdxBinLoader.Loader(idxFile);

            //carrega o arquivo smd;
            StreamReader stream = new FileInfo(smdPath).OpenText();
            SMD_READER_API.SMD smd = SMD_READER_API.SmdReader.Reader(stream);


            // valor que representa a maior distancia do modelo, tanto para X, Y ou Z
            float FarthestVertex = 0;

            //--- crio a primeira estrutura:

            StartStructure startStructure = new StartStructure();
            
            Vector4 color = new Vector4(1, 1, 1, 1);

            for (int i = 0; i < smd.Triangles.Count; i++)
            {
                string materialName = smd.Triangles[i].Material.ToUpperInvariant().Trim();

                List<StartVertex> verticeList = new List<StartVertex>();

                for (int t = 0; t < smd.Triangles[i].Vertexs.Count; t++)
                {
                    // cria o objeto com os indices.
                    StartVertex vertice = new StartVertex();
                    vertice.Color = color;


                    Vector3 position = new Vector3(
                            smd.Triangles[i].Vertexs[t].PosX,
                            smd.Triangles[i].Vertexs[t].PosZ,
                            smd.Triangles[i].Vertexs[t].PosY * -1
                            );

                    vertice.Position = position;

                    Vector3 normal = new Vector3(
                            smd.Triangles[i].Vertexs[t].NormX,
                            smd.Triangles[i].Vertexs[t].NormZ,
                            smd.Triangles[i].Vertexs[t].NormY * -1
                            );

                    vertice.Normal = normal;

                    Vector2 texture = new Vector2(
                    smd.Triangles[i].Vertexs[t].U,
                    smd.Triangles[i].Vertexs[t].V
                    );

                    vertice.Texture = texture;

                    //cria o objetos com os weight
                    // e corrige a soma total para dar 1

                    if (smd.Triangles[i].Vertexs[t].Links.Count == 0)
                    {
                        StartWeightMap weightMap = new StartWeightMap();
                        weightMap.Links = 1;
                        weightMap.BoneID1 = smd.Triangles[i].Vertexs[t].ParentBone;
                        weightMap.Weight1 = 1f;

                        vertice.WeightMap = weightMap;
                    }
                    else
                    {
                        StartWeightMap weightMap = new StartWeightMap();

                        var links = (from link in smd.Triangles[i].Vertexs[t].Links
                                     orderby link.Weight
                                     select link).ToArray();

                        if (links.Length >= 1)
                        {
                            weightMap.Links = 1;
                            weightMap.BoneID1 = links[0].BoneID;
                            weightMap.Weight1 = links[0].Weight;
                        }
                        if (links.Length >= 2)
                        {
                            weightMap.Links = 2;
                            weightMap.BoneID2 = links[1].BoneID;
                            weightMap.Weight2 = links[1].Weight;
                        }
                        if (links.Length >= 3)
                        {
                            weightMap.Links = 3;
                            weightMap.BoneID3 = links[2].BoneID;
                            weightMap.Weight3 = links[2].Weight;
                        }

                        // verificação para soma total dar 1

                        float sum = weightMap.Weight1 + weightMap.Weight2 + weightMap.Weight3;

                        if (sum > 1  // se por algum motivo aleatorio ficar maior que 1
                            || sum < 1) // ou se caso for menor que 1
                         {
                            float difference = sum - 1; // se for maior diferença é positiva, e se for menor é positiva
                            float average = difference / weightMap.Links; // aqui mantem o sinal da operação

                            if (weightMap.Links >= 1)
                            {
                                weightMap.Weight1 -= average; // se for positivo tem que dimiuir,
                                                              // porem se for negativo tem que aumentar,
                                                              // porem menos com menos da mais, então esta certo.
                            }
                            if (weightMap.Links >= 2)
                            {
                                weightMap.Weight2 -= average;
                            }
                            if (weightMap.Links >= 3)
                            {
                                weightMap.Weight3 -= average;
                            }

                            //re verifica se ainda tem diferença
                            float newSum = weightMap.Weight1 + weightMap.Weight2 + weightMap.Weight3;
                            float newDifference = newSum - 1;

                            if (newDifference != 1)
                            {
                                weightMap.Weight1 -= newDifference;
                            }
                        }

                        vertice.WeightMap = weightMap;
                    }


                    verticeList.Add(vertice);

                    // --- verifica o vertice mais distante

                    float temp = position.X;
                    if (temp < 0)
                    {
                        temp *= -1;
                    }
                    if (temp > FarthestVertex)
                    {
                        FarthestVertex = temp;
                    }

                    temp = position.Y;
                    if (temp < 0)
                    {
                        temp *= -1;
                    }
                    if (temp > FarthestVertex)
                    {
                        FarthestVertex = temp;
                    }

                    temp = position.Z;
                    if (temp < 0)
                    {
                        temp *= -1;
                    }
                    if (temp > FarthestVertex)
                    {
                        FarthestVertex = temp;
                    }

                }

                if (startStructure.FacesByMaterial.ContainsKey(materialName))
                {
                    startStructure.FacesByMaterial[materialName].Faces.Add(verticeList);
                }
                else // cria novo
                {
                    StartFacesGroup facesGroup = new StartFacesGroup();
                    facesGroup.Faces.Add(verticeList);
                    startStructure.FacesByMaterial.Add(materialName, facesGroup);
                }

            }


            // faz a compressão das vertives
            if (idxBin.CompressVertices == true)
            {
                startStructure.CompressAllFaces();
            }


            // calcula o fator de conversão
            float ConversionFactorValue = FarthestVertex / short.MaxValue * idxBin.GlobalScale;
            if (idxBin.AutoConversionFactor == false && idxBin.ManualConversionFactor != 0)
            {
                ConversionFactorValue = idxBin.ManualConversionFactor;
            }

            // estrutura intermediaria
            IntermediaryStructure intermediaryStructure = MakeIntermediaryStructure(startStructure);

            // estrutura final
            FinalStructure finalStructure = MakeFinalStructure(intermediaryStructure, ConversionFactorValue, idxBin.GlobalScale);

            // aqui é usado os bones do arquivo smd
            BoneLine[] bones = GetBoneLines(smd, idxBin.GlobalScale);


            //finaliza e cria o arquivo bin
            MakeFinalBinFile(binpath, finalStructure, idxBin, bones, ConversionFactorValue);
        }


        private static BoneLine[] GetBoneLines(SMD_READER_API.SMD smd, float globalScale) 
        {
            List<BoneLine> bones = new List<BoneLine>();

            SMD_READER_API.Time time = (from tt in smd.Times
                         where tt.ID == 0
                         select tt).FirstOrDefault();

            for (int i = 0; i < smd.Nodes.Count; i++)
            {

                (float X, float Y, float Z) bonePos = (0, 0, 0);

                if (time != null)
                {
                    SMD_READER_API.Skeleton skeleton = (from ss in time.Skeletons
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