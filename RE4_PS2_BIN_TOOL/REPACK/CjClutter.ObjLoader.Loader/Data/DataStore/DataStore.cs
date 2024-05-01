using System.Collections.Generic;
using System.Linq;
using ObjLoader.Loader.Common;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;

namespace ObjLoader.Loader.Data.DataStore
{
    public class DataStore : IDataStore, IFaceGroup, ILineGroup, IVertexDataStore, ITextureDataStore, INormalDataStore, IMtlLibDataStore,
                             IGroupNameDataStore, IMaterialNameDataStore, IObjectNameDataStore
    {
        private string _lastGroupName = "default";
        private string _lastMaterialName = "default";
        private string _lastObjectName = "default";

        private Group _currentGroup;

        private readonly List<Group> _groups = new List<Group>();
        private readonly List<Vertex> _vertices = new List<Vertex>();
        private readonly List<Texture> _textures = new List<Texture>();
        private readonly List<Normal> _normals = new List<Normal>();
        private readonly List<string> _mtlLibs = new List<string>();

        public IList<Vertex> Vertices
        {
            get { return _vertices; }
        }

        public IList<Texture> Textures
        {
            get { return _textures; }
        }

        public IList<Normal> Normals
        {
            get { return _normals; }
        }

        public IList<Group> Groups
        {
            get { return _groups.FindAll(g => g.Faces.Count != 0 || g.Lines.Count != 0); }
        }

        public IList<string> MtlLibs
        {
            get { return _mtlLibs; }
        }

        public void AddFace(Face face)
        {
            PushGroupIfNeeded();
            _currentGroup.AddFace(face);
        }

        public void AddLine(Line line)
        {
            PushGroupIfNeeded();
            _currentGroup.AddLine(line);
        }

        private void PushGroupIfNeeded()
        {
            if (_currentGroup == null)
            {
                _currentGroup = new Group(_lastGroupName, _lastMaterialName, _lastObjectName);
                _groups.Add(_currentGroup);
            }
        }

        public void PushGroup(string groupName)
        {
            _lastGroupName = groupName;
            _currentGroup = new Group(groupName, _lastMaterialName, _lastObjectName);
            _groups.Add(_currentGroup);
        }

        public void PushMaterial(string materialName)
        {
            _lastMaterialName = materialName;
            _currentGroup = new Group(_lastGroupName, materialName, _lastObjectName);
            _groups.Add(_currentGroup);
        }

        public void PushObject(string objectName)
        {
            _lastObjectName = objectName;
            _currentGroup = new Group(_lastGroupName, _lastMaterialName, objectName);
            _groups.Add(_currentGroup);
        }


        public void AddVertex(Vertex vertex)
        {
            _vertices.Add(vertex);
        }

        public void AddTexture(Texture texture)
        {
            _textures.Add(texture);
        }

        public void AddNormal(Normal normal)
        {
            _normals.Add(normal);
        }

        public void AddMtlLib(string mtlLib)
        {
           _mtlLibs.Add(mtlLib);
        }

        
    }


    public class DataStoreMtl : IDataStoreMtl, IMaterialDataStore
    {
        private readonly List<Material> _materials = new List<Material>();

        public IList<Material> Materials
        {
            get { return _materials; }
        }

        public void Push(Material material)
        {
            _materials.Add(material);
        }
    }

}