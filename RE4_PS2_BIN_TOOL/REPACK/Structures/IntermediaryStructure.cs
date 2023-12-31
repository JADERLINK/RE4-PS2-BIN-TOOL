using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_PS2_BIN_TOOL.REPACK
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

        public List<IntermediaryFace> Faces { get; set; }

        public IntermediaryGroup()
        {
            Faces = new List<IntermediaryFace>();
        }
    }


    public class IntermediaryFace
    {
        public List<IntermediaryVertex> Vertexs { get; set; }
       
        public List<IntermediaryWeightMap> WeightMapOnFace { get; set; }

        public IntermediaryFace() 
        {
            Vertexs = new List<IntermediaryVertex>();
            WeightMapOnFace = new List<IntermediaryWeightMap>();
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
        public int Links { get; set; }
        public int BoneID1 { get; set; }
        public float Weight1 { get; set; }

        public int BoneID2 { get; set; }
        public float Weight2 { get; set; }

        public int BoneID3 { get; set; }
        public float Weight3 { get; set; }

        public IntermediaryWeightMap GetIntermediaryWeightMap() 
        {
            IntermediaryWeightMap weightMap = new IntermediaryWeightMap();
            weightMap.Links = Links;
            weightMap.BoneID1 = BoneID1;
            weightMap.BoneID2 = BoneID2;
            weightMap.BoneID3 = BoneID3;
            weightMap.Weight1 = Weight1;
            weightMap.Weight2 = Weight2;
            weightMap.Weight3 = Weight3;
            return weightMap;
        }
    }


    public class IntermediaryWeightMap : IEquatable<IntermediaryWeightMap>
    {
        public int Links { get; set; }

        public int BoneID1 { get; set; }
        public float Weight1 { get; set; }

        public int BoneID2 { get; set; }
        public float Weight2 { get; set; }

        public int BoneID3 { get; set; }
        public float Weight3 { get; set; }

        public override bool Equals(object obj)
        {
            return obj is IntermediaryWeightMap map
                && map.Links == Links
                && map.BoneID1 == BoneID1
                && map.BoneID2 == BoneID2
                && map.BoneID3 == BoneID3
                && map.Weight1 == Weight1
                && map.Weight2 == Weight2
                && map.Weight3 == Weight3;
        }

        public bool Equals(IntermediaryWeightMap other)
        {
            return other.Links == Links
                && other.BoneID1 == BoneID1
                && other.BoneID2 == BoneID2
                && other.BoneID3 == BoneID3
                && other.Weight1 == Weight1
                && other.Weight2 == Weight2
                && other.Weight3 == Weight3;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Links.GetHashCode();
                hash = hash * 23 + BoneID1.GetHashCode();
                hash = hash * 23 + Weight1.GetHashCode();
                hash = hash * 23 + BoneID2.GetHashCode();
                hash = hash * 23 + Weight2.GetHashCode();
                hash = hash * 23 + BoneID3.GetHashCode();
                hash = hash * 23 + Weight3.GetHashCode();
                return hash;
            }
        }

    }


}
