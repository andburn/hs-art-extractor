namespace HsArtExtractor.Util
{
    public struct QuaternionF
    {
        public float x, y, z, w;

        public QuaternionF(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", x, y, z, w);
        }
    }
}
