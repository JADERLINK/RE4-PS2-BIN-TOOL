using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SHARED_PS2_BIN.REPACK.Structures
{
    public class StartStructure
    {
        /// <summary>
        /// Dicionario, sendo string o nome do material, e StartFacesGroup, objeto que representa as faces do modelo.
        /// </summary>
        public IDictionary<string, StartFacesGroup> FacesByMaterial { get; set; }

        public StartStructure()
        {
            FacesByMaterial = new ConcurrentDictionary<string, StartFacesGroup>();
        }
    }


    /// <summary>
    /// Representação de um Vector3, usado para Position/Normal
    /// </summary>
    public readonly struct Vector3 : IEquatable<Vector3>
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        private readonly int hashCode;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            unchecked
            {
                hashCode = 17;
                hashCode = hashCode * 23 + X.GetHashCode();
                hashCode = hashCode * 23 + Y.GetHashCode();
                hashCode = hashCode * 23 + Z.GetHashCode();
            }
        }


        public static bool operator ==(Vector3 lhs, Vector3 rhs) => lhs.Equals(rhs);

        public static bool operator !=(Vector3 lhs, Vector3 rhs) => !(lhs == rhs);

        public bool Equals(Vector3 obj)
        {
            return obj.X == X && obj.Y == Y && obj.Z == Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 o && o.X == X && o.Y == Y && o.Z == Z;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }


    /// <summary>
    /// Representação de um Vector2, usado para TextureUV
    /// </summary>
    public readonly struct Vector2 : IEquatable<Vector2>
    {
        public readonly float U;
        public readonly float V;

        private readonly int hashCode;

        public Vector2(float u, float v)
        {
            U = u;
            V = v;
            unchecked
            {
                hashCode = 17;
                hashCode = hashCode * 23 + U.GetHashCode();
                hashCode = hashCode * 23 + V.GetHashCode();
            }
        }

        public static bool operator ==(Vector2 lhs, Vector2 rhs) => lhs.Equals(rhs);

        public static bool operator !=(Vector2 lhs, Vector2 rhs) => !(lhs == rhs);

        public bool Equals(Vector2 obj)
        {
            return obj.U == U && obj.V == V;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 o && o.U == U && o.V == V;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }


    /// <summary>
    /// Representação de um Vector4, usado para Colors
    /// </summary>
    public readonly struct Vector4 : IEquatable<Vector4>
    {
        public readonly float R;
        public readonly float G;
        public readonly float B;
        public readonly float A;

        private readonly int hashCode;

        public Vector4(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
            unchecked
            {
                hashCode = 17;
                hashCode = hashCode * 23 + R.GetHashCode();
                hashCode = hashCode * 23 + G.GetHashCode();
                hashCode = hashCode * 23 + B.GetHashCode();
                hashCode = hashCode * 23 + A.GetHashCode();
            }
        }

        public static bool operator ==(Vector4 lhs, Vector4 rhs) => lhs.Equals(rhs);

        public static bool operator !=(Vector4 lhs, Vector4 rhs) => !(lhs == rhs);

        public bool Equals(Vector4 obj)
        {
            return obj.A == A && obj.R == R && obj.G == G && obj.B == B;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector4 o && o.A == A && o.R == R && o.G == G && o.B == B;
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

    }


    /// <summary>
    /// Representa o conjunto de pesos associado a um vértice;
    /// </summary>
    public struct StartWeightMap : IEquatable<StartWeightMap>
    {
        public int Links;

        public int BoneID1;
        public float Weight1;

        public int BoneID2;
        public float Weight2;

        public int BoneID3;
        public float Weight3;

        public StartWeightMap(int links, int boneID1, float weight1, int boneID2, float weight2, int boneID3, float weight3)
        {
            Links = links;
            BoneID1 = boneID1;
            Weight1 = weight1;
            BoneID2 = boneID2;
            Weight2 = weight2;
            BoneID3 = boneID3;
            Weight3 = weight3;
        }

        public static bool operator ==(StartWeightMap lhs, StartWeightMap rhs) => lhs.Equals(rhs);

        public static bool operator !=(StartWeightMap lhs, StartWeightMap rhs) => !(lhs == rhs);

        public bool Equals(StartWeightMap obj)
        {
            return obj.Links == Links
                && obj.BoneID1 == BoneID1
                && obj.BoneID2 == BoneID2
                && obj.BoneID3 == BoneID3
                && obj.Weight1 == Weight1
                && obj.Weight2 == Weight2
                && obj.Weight3 == Weight3;
        }

        public override bool Equals(object obj)
        {
            return obj is StartWeightMap map
                && map.Links == Links
                && map.BoneID1 == BoneID1
                && map.BoneID2 == BoneID2
                && map.BoneID3 == BoneID3
                && map.Weight1 == Weight1
                && map.Weight2 == Weight2
                && map.Weight3 == Weight3;
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


    /// <summary>
    /// Represente um vertice
    /// </summary>
    public struct StartVertex
    {
        public Vector3 Position;
        public Vector2 Texture;
        public Vector3 Normal;
        public Vector4 Color;
        public StartWeightMap WeightMap;

        public bool Equals(StartVertex obj)
        {
            return obj.Position == Position
                && obj.Texture == Texture
                && obj.Normal == Normal
                && obj.Color == Color
                && obj.WeightMap == WeightMap;
        }

        public override bool Equals(object obj)
        {
            return obj is StartVertex o
                && o.Position == Position
                && o.Texture == Texture
                && o.Normal == Normal
                && o.Color == Color
                && o.WeightMap == WeightMap;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() + Texture.GetHashCode() + Normal.GetHashCode() + Color.GetHashCode() + WeightMap.GetHashCode()).GetHashCode();
            }
        }

    }


    /// <summary>
    /// Reprenta um triangulo
    /// </summary>
    public class StartTriangle
    {
        /// <summary>
        /// D  (1)
        /// </summary>
        public StartVertex A;
        /// <summary>
        /// E  (2)
        /// </summary>
        public StartVertex B;
        /// <summary>
        /// F  (3)
        /// </summary>
        public StartVertex C;

        /// <summary>
        /// contagem a partir de 1
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public StartVertex this[int i]
        {
            get
            {
                if (i == 1)
                {
                    return A;
                }
                else if (i == 2)
                {
                    return B;
                }
                else if (i == 3)
                {
                    return C;
                }
                return new StartVertex();
            }
        }


        public bool Equals(StartTriangle obj)
        {
            return obj.A.Equals(A) && obj.B.Equals(B) && obj.C.Equals(C);
        }

        public override bool Equals(object obj)
        {
            return obj is StartTriangle T && T.A.Equals(A) && T.B.Equals(B) && T.C.Equals(C);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (A.GetHashCode() + B.GetHashCode() + C.GetHashCode()).GetHashCode();
            }
        }

    }


    public class StartFacesGroup
    {
        /// <summary>
        /// o primeiro List é o conjunto de faces, e o segundo List é o conjunto de vertices
        /// </summary>
        public List<List<StartVertex>> Faces { get; set; }

        public StartFacesGroup()
        {
            Faces = new List<List<StartVertex>>();
        }

        public StartFacesGroup(List<List<StartVertex>> faces)
        {
            Faces = faces;
        }
    }


    public readonly struct TriOrder
    {
        public readonly (int r1, int r2, int r3) op1;
        public readonly (int r1, int r2, int r3) op2;
        public readonly (int r1, int r2, int r3) op3;

        public static TriOrder order_1_2_3 = new TriOrder(_op1: (1, 2, 3), _op2: (2, 3, 1), _op3: (3, 1, 2));

        public static TriOrder order_3_2_1 = new TriOrder(_op1: (1, 3, 2), _op2: (2, 1, 3), _op3: (3, 2, 1));

        public TriOrder(
            (int r1, int r2, int r3) _op1,
            (int r1, int r2, int r3) _op2,
            (int r1, int r2, int r3) _op3
            )
        {
            op1 = _op1;
            op2 = _op2;
            op3 = _op3;
        }
    }

}
