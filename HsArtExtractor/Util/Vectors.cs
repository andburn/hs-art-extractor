namespace HsArtExtractor.Util
{
    public struct Vector4F
    {
        public float w, x, y, z;

        public Vector4F(float w, float x, float y, float z)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", w, x, y, z);
        }
    }

    public struct Vector3F
    {
        public float x, y, z;

        public Vector3F(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x, y, z);
        }
    }

    public struct Vector2F
    {
        public float x, y;

        public Vector2F(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", x, y);
        }
    }
}
