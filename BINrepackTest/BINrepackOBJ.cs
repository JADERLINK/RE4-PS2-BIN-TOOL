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
    08-09-2023
    version: beta.1.3.0.0
    */

    public static partial class BINrepack
    {

        public static void RepackObj(string idxbinPath, string objPath, string binpath)
        {

            // carrega o arquivo .idxBin
            StreamReader idxFile = File.OpenText(idxbinPath);
            IdxBin idxBin = IdxBinLoader.Loader(idxFile);

            // load .obj file
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create(new MaterialNullStreamProvider());

            var fileStream = new System.IO.FileStream(objPath, System.IO.FileMode.Open);
            LoadResult arqObj = objLoader.Load(fileStream);
            fileStream.Close();


            // valor que representa a maior distancia do modelo, tanto para X, Y ou Z
            float FarthestVertex = 0;

            //--- crio a primeira estrutura:

            StartStructure startStructure = new StartStructure();

            Vector4 color = new Vector4(1, 1, 1, 1);
            StartWeightMap weightMap = new StartWeightMap(1, 0, 1, 0, 0, 0, 0);

            for (int iG = 0; iG < arqObj.Groups.Count; iG++)
            {
                List<List<StartVertex>> facesList = new List<List<StartVertex>>();

                for (int iF = 0; iF < arqObj.Groups[iG].Faces.Count; iF++)
                {
                    List<StartVertex> face = new List<StartVertex>();

                    for (int iI = 0; iI < arqObj.Groups[iG].Faces[iF].Count; iI++)
                    {
                        StartVertex vertice = new StartVertex();

                        if (arqObj.Groups[iG].Faces[iF][iI].VertexIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1 >= arqObj.Vertices.Count)
                        {
                            throw new ArgumentException("Vertex Position Index is invalid! Value: " + arqObj.Groups[iG].Faces[iF][iI].VertexIndex);
                        }

                        Vector3 position = new Vector3(
                            arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].X,
                            arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].Y,
                            arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].Z
                            );

                        vertice.Position = position;

                      
                        if (arqObj.Groups[iG].Faces[iF][iI].TextureIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].TextureIndex - 1 >= arqObj.Textures.Count)
                        {
                            vertice.Texture = new Vector2();
                        }
                        else 
                        {
                            Vector2 texture = new Vector2(
                            arqObj.Textures[arqObj.Groups[iG].Faces[iF][iI].TextureIndex - 1].X,
                            arqObj.Textures[arqObj.Groups[iG].Faces[iF][iI].TextureIndex - 1].Y
                            );

                            vertice.Texture = texture;
                        }

        
                        if (arqObj.Groups[iG].Faces[iF][iI].NormalIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1 >= arqObj.Normals.Count)
                        {
                            vertice.Normal = new Vector3();
                        }
                        else 
                        {
                            Vector3 normal = new Vector3(
                            arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].X,
                            arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].Y,
                            arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].Z
                            );

                            vertice.Normal = normal;
                        }

                        vertice.Color = color;
                        vertice.WeightMap = weightMap;

                        face.Add(vertice);


                        // --- verifica o vertice mais distante

                        float temp = arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex-1].X;
                        if (temp < 0)
                        {
                            temp *= -1;
                        }
                        if (temp > FarthestVertex)
                        {
                            FarthestVertex = temp;
                        }

                        temp = arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].Y;
                        if (temp < 0)
                        {
                            temp *= -1;
                        }
                        if (temp > FarthestVertex)
                        {
                            FarthestVertex = temp;
                        }

                        temp = arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].Z;
                        if (temp < 0)
                        {
                            temp *= -1;
                        }
                        if (temp > FarthestVertex)
                        {
                            FarthestVertex = temp;
                        }

                    }

                    if (face.Count != 0)
                    {
                        facesList.Add(face);
                    }

                }

                if (startStructure.FacesByMaterial.ContainsKey(arqObj.Groups[iG].Name))
                {
                    startStructure.FacesByMaterial[arqObj.Groups[iG].Name].Faces.AddRange(facesList);
                }
                else
                {
                    startStructure.FacesByMaterial.Add(arqObj.Groups[iG].Name, new StartFacesGroup(facesList));
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


            //finaliza e cria o arquivo bin
            MakeFinalBinFile(binpath, finalStructure, idxBin, idxBin.BoneLines, ConversionFactorValue);

        }


    }
}
