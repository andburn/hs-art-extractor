namespace HearthstoneDisunity.Util
{
    // TODO: should be a Vector4F instead of this
    public struct ColorRGBA
    {
        public float r, g, b, a;

        public ColorRGBA(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", r, g, b, a);
        }
    }
}
