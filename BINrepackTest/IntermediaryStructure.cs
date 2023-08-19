using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BINrepackTest
{
    public class IntermediaryStructure
    {
        public Dictionary<string, IntermediaryGroup> Groups { get; set; }

        public IntermediaryStructure()
        {
            Groups = new Dictionary<string, IntermediaryGroup>();
        }
    }

    public class IntermediaryGroup 
    {
        public string MaterialName { get; set; }

        public List<IntermediaryTriangle> Triangles { get; set; }

        public IntermediaryGroup()
        {
            Triangles = new List<IntermediaryTriangle>();
        }
    }


    public class IntermediaryTriangle
    {
        public List<IntermediaryVertex> Vertices { get; set; }

        public IntermediaryTriangle() 
        {
            Vertices = new List<IntermediaryVertex>();
        }
    }

    public class IntermediaryVertex
    {
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public float NormalX { get; set; }
        public float NormalY { get; set; }
        public float NormalZ { get; set; }

        public float TextureU { get; set; }
        public float TextureV { get; set; }

        public float ColorR { get; set; }
        public float ColorG { get; set; }
        public float ColorB { get; set; }
        public float ColorA { get; set; }
        public int links { get; set; }
        public int BoneID1 { get; set; }
        public float Weight1 { get; set; }

        public int BoneID2 { get; set; }
        public float Weight2 { get; set; }

        public int BoneID3 { get; set; }
        public float Weight3 { get; set; }
    }




}
