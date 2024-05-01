using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_BIN_TOOL.REPACK
{
    /*
    Codigo feito por JADERLINK
    Pesquisas feitas por HardHain e JaderLink.
    https://www.youtube.com/@JADERLINK
    https://www.youtube.com/@HardRainModder
    */

    public static class BINrepackOBJ
    {

        public static void RepackOBJ(string idxbinPath, string objPath, string binpath, IdxMaterial material)
        {

            // carrega o arquivo .idxBin
            StreamReader idxFile = File.OpenText(idxbinPath);
            IdxBin idxBin = IdxBinLoader.Loader(idxFile);


            // load .obj file
            var objLoaderFactory = new ObjLoader.Loader.Loaders.ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var streamReader = new StreamReader(objPath, Encoding.ASCII);
            ObjLoader.Loader.Loaders.LoadResult arqObj = objLoader.Load(streamReader);
            streamReader.Close();


            // valor que representa a maior distancia do modelo, tanto para X, Y ou Z
            float FarthestVertex = 0;

            //--- crio a primeira estrutura:

            StartStructure startStructure = new StartStructure();

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
                            vertice.Texture = new Vector2(0, 0);
                        }
                        else
                        {
                            Vector2 texture = new Vector2(
                            arqObj.Textures[arqObj.Groups[iG].Faces[iF][iI].TextureIndex - 1].U,
                            arqObj.Textures[arqObj.Groups[iG].Faces[iF][iI].TextureIndex - 1].V
                            );

                            vertice.Texture = texture;
                        }

        
                        if (arqObj.Groups[iG].Faces[iF][iI].NormalIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1 >= arqObj.Normals.Count)
                        {
                            vertice.Normal = new Vector3(0, 0, 0);
                        }
                        else 
                        {
                            float nx = arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].X;
                            float ny = arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].Y;
                            float nz = arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].Z;
                            float NORMAL_FIX = (float)Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));
                            NORMAL_FIX = (NORMAL_FIX == 0) ? 1 : NORMAL_FIX;
                            nx /= NORMAL_FIX;
                            ny /= NORMAL_FIX;
                            nz /= NORMAL_FIX;

                            vertice.Normal = new Vector3(nx, ny, nz);
                        }


                        Vector4 color = new Vector4(
                            arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].R,
                            arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].G,
                            arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].B,
                            arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].A);

                        vertice.Color = color;
                        vertice.WeightMap = weightMap;

                        face.Add(vertice);


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

                    if (face.Count != 0)
                    {
                        facesList.Add(face);
                    }

                }

                string key = arqObj.Groups[iG].MaterialName.ToUpperInvariant();

                if (startStructure.FacesByMaterial.ContainsKey(key))
                {
                    startStructure.FacesByMaterial[key].Faces.AddRange(facesList);
                }
                else
                {
                    startStructure.FacesByMaterial.Add(key, new StartFacesGroup(facesList));
                }

            }


            // faz a compressão das vertives
            if (idxBin.CompressVertices == true)
            {
                startStructure.CompressAllFaces();
            }


            // calcula o fator de conversão
            float ConversionFactorValue = FarthestVertex / short.MaxValue * CONSTs.GLOBAL_SCALE;

            // estrutura intermediaria
            IntermediaryStructure intermediaryStructure = BINrepackIntermediary.MakeIntermediaryStructure(startStructure);

            // estrutura final
            FinalStructure finalStructure = BINrepackFinal.MakeFinalStructure(intermediaryStructure, ConversionFactorValue, CONSTs.GLOBAL_SCALE);


            //finaliza e cria o arquivo bin
            Stream stream = File.Create(binpath);
            BINmakeFile.MakeFinalBinFile(stream, 0, out _ , finalStructure, idxBin, idxBin.BoneLines, ConversionFactorValue, material);

        }


    }
}
