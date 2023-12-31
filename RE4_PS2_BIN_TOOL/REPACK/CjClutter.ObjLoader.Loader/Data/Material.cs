namespace ObjLoader.Loader.Data
{
    public class Material
    {
        public Material(string materialName)
        {
            Name = materialName;
        }

        public string Name { get; set; }

        /// <summary>
        /// Ka
        /// </summary>
        public Vec3 AmbientColor { get; set; }
        /// <summary>
        /// Kd
        /// </summary>
        public Vec3 DiffuseColor { get; set; }
        /// <summary>
        /// Ks
        /// </summary>
        public Vec3 SpecularColor { get; set; }
        /// <summary>
        /// Ns
        /// </summary>
        public float SpecularCoefficient { get; set; }

        /// <summary>
        /// d
        /// </summary>
        public float Transparency { get; set; }

        /// <summary>
        /// Tr
        /// </summary>
        public float TransparencyTr { get; set; }

        /// <summary>
        /// illum
        /// </summary>
        public int IlluminationModel { get; set; }

        /// <summary>
        /// map_Ka
        /// </summary>
        public string AmbientTextureMap { get; set; }
        /// <summary>
        /// map_Kd
        /// </summary>
        public string DiffuseTextureMap { get; set; }

        /// <summary>
        /// map_Ks
        /// </summary>
        public string SpecularTextureMap { get; set; }
        /// <summary>
        /// map_Ns
        /// </summary>
        public string SpecularHighlightTextureMap { get; set; }

        /// <summary>
        /// map_bump, bump
        /// </summary>
        public string BumpMap { get; set; }
        /// <summary>
        /// disp
        /// </summary>
        public string DisplacementMap { get; set; }
        /// <summary>
        /// decal
        /// </summary>
        public string StencilDecalMap { get; set; }

        /// <summary>
        /// map_d
        /// </summary>
        public string AlphaTextureMap { get; set; }
    }
}