namespace ObjLoader.Loader.Data.VertexData
{
    public struct Texture
    {
        public Texture(float u, float v) : this()
        {
            U = u;
            V = v;
        }

        public float U { get; private set; }
        public float V { get; private set; }
    }
}