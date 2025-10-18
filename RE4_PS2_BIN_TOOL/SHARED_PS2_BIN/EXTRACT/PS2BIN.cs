using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHARED_PS2_BIN.EXTRACT
{
    #region representação do bin

    public class PS2BIN
    {
        //binHeader
        public ushort Magic;
        public ushort Tex_count; //nTex
        public uint BonesPoint;
        public byte Vertex_Scale; //frac
        public byte BonesCount;
        public ushort MaterialCount;
        public uint MaterialOffset;
        public uint Padding1; //CD-CD-CD-CD
        public uint Padding2; //CD-CD-CD-CD
        public uint Version_flags; //18-08-03-20 or 01-08-01-20
        public uint BonepairPoint; // normalmente é 0x0
        public uint UnusedOffset1; // padding, sempre 0x00
        public uint Bin_flags;
        public uint BoundboxPoint; //fixo 0x30
        public uint UnusedOffset2; // padding, sempre 0x00
        public float BoundingBoxPosX;
        public float BoundingBoxPosY;
        public float BoundingBoxPosZ;
        public float BoundingBoxPosW;
        public float BoundingBoxWidth;
        public float BoundingBoxHeight;
        public float BoundingBoxDepth;
        public uint Padding3; // CD-CD-CD-CD
        //end binHeader

        public BonePair[] BonePairs;

        //bonesList
        public Bone[] Bones;

        //materialsList
        public Material[] materials;

        //NodeList
        public Node[] Nodes;

        // tipo do bin
        public BinType binType = BinType.Default;
    }

    public struct BonePair
    {
        public ushort Bone1;
        public ushort Bone2;
        public ushort Bone3;
        public ushort Bone4;
    }


    public class Bone
    {
        public byte[] boneLine; // new byte[16];

        public byte BoneID { get { return boneLine[0x0]; } }
        public byte BoneParent { get { return boneLine[0x1]; } }

        public float PositionX { get { return BitConverter.ToSingle(boneLine, 0x4); } }
        public float PositionY { get { return BitConverter.ToSingle(boneLine, 0x8); } }
        public float PositionZ { get { return BitConverter.ToSingle(boneLine, 0xC); } }

    }

    public class Material
    {
        public byte[] materialLine; // new byte[16];
        public uint nodeTablePoint;
    }

    public class Node
    {
        //NodeHeader
        //tamanho total dinâmico, em blocos de 0x10
        public byte[] NodeHeaderArray;

        //partes
        public ushort TotalBytesAmount;
        public byte segmentAmountWithoutFirst;
        public byte BonesIdAmount;
        public byte[] NodeBoneList;

        //segmentos no node
        public Segment[] Segments;
    }

    public class Segment
    {
        public bool IsScenarioColor; // extra

        //new byte[0x10];
        public byte[] WeightMapHeader;

        //[quantidade de linhas]
        public WeightMapTableLine[] WeightMapTableLines;

        //--
        //TopTagVifHeader
        public byte[] TopTagVifHeader2080; //new byte[0x10];
        public byte[] TopTagVifHeaderWithScale; //new byte[0x10];
        public byte[] TopTagVifHeader2180; //new byte[0x10];
        public float ConversionFactorValue;
        //--

        //vertexLines
        public VertexLine[] vertexLines;

        //EndTagVifCommand
        public byte[] EndTagVifCommand; //new byte[0x10];
    }

    public class WeightMapTableLine
    {
        public byte[] weightMapTableLine;

        public uint boneId1 { get { return BitConverter.ToUInt32(weightMapTableLine, 0x0); } }
        public uint boneId2 { get { return BitConverter.ToUInt32(weightMapTableLine, 0x4); } }
        public uint boneId3 { get { return BitConverter.ToUInt32(weightMapTableLine, 0x8); } }
        public int Amount { get { return BitConverter.ToInt32(weightMapTableLine, 0xC); } }
        public float weight1 { get { return BitConverter.ToSingle(weightMapTableLine, 0x10); } }
        public float weight2 { get { return BitConverter.ToSingle(weightMapTableLine, 0x14); } }
        public float weight3 { get { return BitConverter.ToSingle(weightMapTableLine, 0x18); } }
    }


    public class VertexLine
    {
        public short VerticeX;
        public short VerticeY;
        public short VerticeZ;

        public ushort UnknownB; // WightMap ID or ColorA

        public short NormalX; // or ColorR
        public short NormalY; // or ColorG
        public short NormalZ; // or ColorB

        public ushort IndexComplement;

        public short TextureU;
        public short TextureV;

        public ushort UnknownA; // sempre 0x01

        public ushort IndexMount;
    }

    public enum BinType
    {
        Default,
        ScenarioWithColors
    }

    [Flags]
    public enum BinFlags : uint
    {
        Empty = 0x00_00_00_00,
        AlwaysActivated = 0x80_00_00_00,
        EnableUnkFlag4 = 0x40_00_00_00,
        EnableUnkFlag2 = 0x20_00_00_00,
        EnableUnkFlag1 = 0x10_00_00_00,
        EnableAdjacentBoneTag = 0x00_00_02_00,
        EnableBonepairTag = 0x00_00_01_00,
    }

    #endregion
}
