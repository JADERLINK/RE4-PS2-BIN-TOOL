/*
MIT License

Copyright (c) 2023 JADERLINK

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SMD_READER_API
{
    // Codigo escrito por JADERLINK
    //https://github.com/JADERLINK
    // referencia do formato SMD
    //https://developer.valvesoftware.com/wiki/SMD
    // versão do codigo: 1.1.0.0

    public static class SmdReader
    {
        //Arquivo invalido, não começa com a tag "version 1"
        private const string versionError = "Invalid file, does not start with the tag \"version 1\"";

        //Arquivo invalido, não tem a tag "end"
        private const string endError = "Invalid file, does not have the tag \"end\"";

        //Erro ou comando não implementado 
        private const string commandError = "Error or command not implemented";

        public static SMD Reader(StreamReader stream) 
        {
            SMD smd = new SMD();
            smd.Nodes = new List<Node>();
            smd.Times = new List<Time>();
            smd.Triangles = new List<Triangle>();
            smd.VertexAnimation = new List<VertexAnimationTime>();

            Status status = Status.RequiresVersion1;
            int triangleStatus = 0;

            Time lastTime = null;
            VertexAnimationTime lastVATime = null;
            Triangle lastTriangle = null;
            int triangleCount = 0;
            int vertexCount = 0;

            while (!stream.EndOfStream)
            {
                string line = stream.ReadLine();

                if (line.Length == 0)
                {
                    // linha vazia
                    continue;
                }
                else if (line.StartsWith("//"))
                {
                    // linha comentada
                    continue;
                }
                else if (status == Status.RequiresVersion1)
                {
                    if (line.ToLowerInvariant().Trim().StartsWith("version"))
                    {
                        status = Status.RequiresCommand;
                        // arquivo valido
                        continue;
                    }
                    else
                    {
                        throw new ArgumentException(versionError);
                    }
                }
                else 
                {

                    if (status == Status.RequiresCommand)
                    {
                        line = line.ToLowerInvariant().Trim();

                        if (line.StartsWith("nodes"))
                        {
                            status = Status.Nodes;
                            continue;
                        }
                        else if (line.StartsWith("skeleton"))
                        {
                            status = Status.Skeleton;
                            continue;
                        }
                        else if (line.StartsWith("triangles"))
                        {
                            status = Status.Triangles;
                            continue;
                        }
                        else if (line.StartsWith("vertexanimation"))
                        {
                            status = Status.VertexAnimation;
                            continue;
                        }
                        else
                        {
                            throw new ArgumentException(commandError);
                        }
                    }

                    else if (status == Status.Nodes)
                    {
                        line = line.TrimStart();

                        if (line.ToLowerInvariant().StartsWith("end"))
                        {
                            status = Status.RequiresCommand;
                            continue;
                        }
                        else 
                        {
                            // é um conteudo de node
                            smd.Nodes.Add(GetNode(NormalizeLine(line)));
                        }
                    }

                    else if (status == Status.Skeleton)
                    {
                        line = line.TrimStart();

                        if (line.ToLowerInvariant().StartsWith("end"))
                        {
                            status = Status.RequiresCommand;
                            if (lastTime != null)
                            {
                                smd.Times.Add(lastTime);
                            }
                            continue;
                        }
                        if (line.ToLowerInvariant().StartsWith("time"))
                        {
                            if (lastTime != null)
                            {
                                smd.Times.Add(lastTime);
                            }

                            int timeID = 0;
                            line = NormalizeLine(line);
                            var split = line.Split(' ');

                            if (split.Length > 1)
                            {
                                timeID = GetInteger(split[1]);
                            }

                            lastTime = new Time(timeID);
                            lastTime.Skeletons = new List<Skeleton>();
                        }
                        else
                        {
                            // conteudo
                            lastTime.Skeletons.Add(GetSkeleton(NormalizeLine(line)));
                        }
                    }

                    else if (status == Status.Triangles) 
                    {
                        line = line.TrimStart();

                        if (line.ToLowerInvariant().StartsWith("end"))
                        {
                            status = Status.RequiresCommand;
                            if (lastTriangle != null)
                            {
                                smd.Triangles.Add(lastTriangle);
                            }
                            triangleStatus = 0;
                            continue;
                        }
                        else if (triangleStatus == 0)
                        {
                            // material name

                            if (lastTriangle != null)
                            {
                                smd.Triangles.Add(lastTriangle);
                            }

                            lastTriangle = new Triangle();
                            lastTriangle.ID = triangleCount;
                            triangleCount++;
                            lastTriangle.Material = line.Trim();
                            lastTriangle.Vertexs = new List<Vertex>();

                            triangleStatus++;
                        }
                        else
                        {
                            // conteudo
                            lastTriangle.Vertexs.Add(GetVertex(NormalizeLine(line), vertexCount));

                            vertexCount++;

                            triangleStatus++;

                            if (triangleStatus == 4)
                            {
                                triangleStatus = 0;
                            }
                        }
                    }

                    else if (status == Status.VertexAnimation)
                    {
                        line = line.TrimStart();

                        if (line.ToLowerInvariant().StartsWith("end"))
                        {
                            status = Status.RequiresCommand;
                            if (lastVATime != null)
                            {
                                smd.VertexAnimation.Add(lastVATime);
                            }
                            continue;
                        }
                        if (line.ToLowerInvariant().StartsWith("time"))
                        {
                            if (lastVATime != null)
                            {
                                smd.VertexAnimation.Add(lastVATime);
                            }

                            int timeID = 0;
                            line = NormalizeLine(line);
                            var split = line.Split(' ');

                            if (split.Length > 1)
                            {
                                timeID = GetInteger(split[1]);
                            }

                            lastVATime = new VertexAnimationTime(timeID);
                            lastVATime.Vextexs = new List<VertexAnimationLine>();
                        }
                        else 
                        {
                            lastVATime.Vextexs.Add(GetVertexAnimationLine(NormalizeLine(line)));
                        }

                    }

                    else
                    {
                        throw new ArgumentException(commandError);
                    }

                }

            }

            if (status == Status.RequiresVersion1)
            {
                throw new ArgumentException(versionError);
            }
            else if (status == Status.Nodes || status == Status.Skeleton || status == Status.Triangles || status == Status.VertexAnimation)
            {
                throw new ArgumentException(endError);
            }

            stream.Close();
            return smd;
        }

        private static Vertex GetVertex(string line, int vertexID) 
        {
            //<int|Parent bone> <float|PosX PosY PosZ> <normal|NormX NormY NormZ> <float|U V> <int|links> <int|Bone ID> <float|Weight> [...]

            Vertex vertex = new Vertex();
            vertex.VertexID = vertexID;

            var split = line.Split(' ');
            if (split.Length > 0)
            {
                vertex.ParentBone = GetInteger(split[0]);
            }

            if (split.Length > 1)
            {
                vertex.PosX = GetFloat(split[1]);
            }
            if (split.Length > 2)
            {
                vertex.PosY = GetFloat(split[2]);
            }
            if (split.Length > 3)
            {
                vertex.PosZ = GetFloat(split[3]);
            }

            if (split.Length > 4)
            {
                vertex.NormX = GetFloat(split[4]);
            }
            if (split.Length > 5)
            {
                vertex.NormY = GetFloat(split[5]);
            }
            if (split.Length > 6)
            {
                vertex.NormZ = GetFloat(split[6]);
            }


            if (split.Length > 7)
            {
                vertex.U = GetFloat(split[7]);
            }
            if (split.Length > 8)
            {
                vertex.V = GetFloat(split[8]);
            }

            int links = 0;
            if (split.Length > 9)
            {
                links = GetInteger(split[9]);
            }
            vertex.Links = new List<WeightMap>();

            for (int i = 0; i < links * 2; i+=2)
            {
                int boneID = 0;
                float weight = 0;

                if (split.Length > 10 + i)
                {
                    boneID = GetInteger(split[10 + i]);
                }

                if (split.Length > 11 + i)
                {
                    weight = GetFloat(split[11 + i]);
                }

                WeightMap weightMap = new WeightMap();
                weightMap.BoneID = boneID;
                weightMap.Weight = weight;
                vertex.Links.Add(weightMap);
            }
            return vertex;
        }

        private static Skeleton GetSkeleton(string line) 
        {
            //<int|bone ID> <float|PosX PosY PosZ> <float|RotX RotY RotZ>

            int boneID = 0;

            var split = line.Split(' ');
            if (split.Length > 0)
            {
                boneID = GetInteger(split[0]);
            }
            Skeleton skeleton = new Skeleton(boneID);

            if (split.Length > 1)
            {
                skeleton.PosX = GetFloat(split[1]);
            }
            if (split.Length > 2)
            {
                skeleton.PosY = GetFloat(split[2]);
            }
            if (split.Length > 3)
            {
                skeleton.PosZ = GetFloat(split[3]);
            }
            if (split.Length > 4)
            {
                skeleton.RotX = GetFloat(split[4]);
            }
            if (split.Length > 5)
            {
                skeleton.RotY = GetFloat(split[5]);
            }
            if (split.Length > 6)
            {
                skeleton.RotZ = GetFloat(split[6]);
            }
            return skeleton;
        }

        private static Node GetNode(string line) 
        {
            //<int|ID> "<string|Bone Name>" <int|Parent ID>

            int id = 0;
            string name = "unknown";
            int parentID = -1;
            var split = line.Split(' ');
            if (split.Length > 0)
            {
                id = GetInteger(split[0]);
            }
            if (split.Length > 1)
            {
                name = split[1].Trim('"');
            }
            if (split.Length > 2)
            {
                parentID = GetInteger(split[2]);
            }
            return new Node(id, name, parentID);
        }

        private static VertexAnimationLine GetVertexAnimationLine(string line) 
        {
            //<int|ID> <float|PosX PosY PosZ> <normal|NormX NormY NormZ>

            VertexAnimationLine vertex = new VertexAnimationLine();

            var split = line.Split(' ');
            if (split.Length > 0)
            {
                vertex.VertexID = GetInteger(split[0]);
            }

            if (split.Length > 1)
            {
                vertex.PosX = GetFloat(split[1]);
            }
            if (split.Length > 2)
            {
                vertex.PosY = GetFloat(split[2]);
            }
            if (split.Length > 3)
            {
                vertex.PosZ = GetFloat(split[3]);
            }

            if (split.Length > 4)
            {
                vertex.NormX = GetFloat(split[4]);
            }
            if (split.Length > 5)
            {
                vertex.NormY = GetFloat(split[5]);
            }
            if (split.Length > 6)
            {
                vertex.NormZ = GetFloat(split[6]);
            }

            return vertex;
        }

        private static string NormalizeLine(string line) 
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\s+");
            return regex.Replace(line, " ").Trim();
        }

        private static int GetInteger(string value) 
        {
            int response;
            string temp = "";
            bool negative = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsDigit(value[i]))
                {
                    temp += value[i];
                }
                else if (value[i] == '-' && !negative) 
                {
                    temp = "-" + temp;
                    negative = true;
                }
            }
            if (temp.Length == 0)
            {
                temp = "0";
            }

            int.TryParse(temp,System.Globalization.NumberStyles.Integer , System.Globalization.CultureInfo.InvariantCulture, out response);
            return response;
        }

        private static float GetFloat(string value) 
        {
            float response;
            string temp = "";
            bool negative = false;
            bool dot = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsDigit(value[i]))
                {
                    temp += value[i];
                }
                else if (value[i] == '-' && !negative)
                {
                    temp = "-" + temp;
                    negative = true;
                }
                else if (value[i] == '.' && !dot) 
                {
                    temp += ".";
                    dot = true;
                }
            }
            if (temp.Length == 0)
            {
                temp = "0";
            }

            float.TryParse(temp, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out response);
            return response;

        }

        private enum Status
        {
            RequiresVersion1,
            RequiresCommand,
            Nodes,
            Skeleton,
            Triangles,
            VertexAnimation
        }
    }


    public class SMD 
    {
        public IList<Node> Nodes { get; set; }
        
        public IList<Time> Times { get; set; }

        public IList<Triangle> Triangles { get; set; }

        public IList<VertexAnimationTime> VertexAnimation { get; set; }
    }

    public class Node 
    {
        public int ID { get; private set; }
        public string BoneName { get; private set; }
        public int ParentID { get; private set; }

        public Node(int ID, string BoneName, int ParentID) 
        {
            this.ID = ID;
            this.BoneName = BoneName;
            this.ParentID = ParentID;
        }
    }

    public class Time 
    {
        public int ID { get; private set; }
        public IList<Skeleton> Skeletons { get; set; } 

        public Time(int ID) 
        {
            this.ID = ID;
        }
    }

    public class Skeleton 
    {
        public int BoneID { get; private set; }
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public float RotX { get; set; }
        public float RotY { get; set; }
        public float RotZ { get; set; }

        public Skeleton(int BoneID)
        {
            this.BoneID = BoneID;
        }
    }

    public class Triangle 
    {
        public int ID { get; set; }
        public string Material { get; set; }
        public IList<Vertex> Vertexs { get; set; }
    }

    public class Vertex 
    {
        //<int|Parent bone> <float|PosX PosY PosZ> <normal|NormX NormY NormZ> <float|U V> <int|links> <int|Bone ID> <float|Weight> [...]

        /// <summary>
        /// representa em qual posição o vertex apareçe no arquivo, sendo o primeiro com indice 0 
        /// </summary>
        public int VertexID { get; set; }

        public int ParentBone { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public float NormX { get; set; }
        public float NormY { get; set; }
        public float NormZ { get; set; }

        public float U { get; set; }
        public float V { get; set; }

        public IList<WeightMap> Links { get; set; }

    }

    public class WeightMap 
    {
        public int BoneID { get; set; }
        public float Weight { get; set; }
    }

    public class VertexAnimationTime 
    {
        public int ID { get; private set; }
        public IList<VertexAnimationLine> Vextexs { get; set; }

        public VertexAnimationTime(int ID)
        {
            this.ID = ID;
        }
    }

    public class VertexAnimationLine 
    {
        //<int|ID> <float|PosX PosY PosZ> <normal|NormX NormY NormZ>
        public int VertexID { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public float NormX { get; set; }
        public float NormY { get; set; }
        public float NormZ { get; set; }
    }

}
